using ArchonLightingSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchonLightingSystem.Bootloader
{
    enum RecordType
    {
        DATA_RECORD = 0,
        END_OF_FILE_RECORD = 1,
        EXT_SEG_ADRS_RECORD = 2,
        EXT_LIN_ADRS_RECORD = 4
    }

    class HexRecord
    {
        public byte RecDataLen;
        public UInt32 Address;
        public UInt32 MaxAddress;
        public UInt32 MinAddress;
        public RecordType RecType;
        public byte[] Data;
        public byte CheckSum;
        public UInt32 ExtSegAddress;
        public UInt32 ExtLinAddress;
    }

    public class Hex
    {
        // Virtual Flash.
        const int KB = (1024);
        const int MB = (KB * KB);
        const UInt32 BOOT_SECTOR_BEGIN = 0x9FC00000u;
        const UInt32 APPLICATION_START = 0x9D000000u;

        // 5 MB flash
        Byte[] VirtualFlash = new byte[5 * MB];
        string[] HexFile = null;
        UInt32 HexFileCurrentLine = 0;

        static UInt32 PA_TO_VFA(UInt32 x)
        {
            return (x - APPLICATION_START);
        }

        static UInt32 PA_TO_KVA0(UInt32 x)
        {
            return (x | 0x80000000u);
        }

        /// <summary>
        /// Load Hex File
        /// </summary>
        /// <param name=""></param>
        /// <returns>true if hex file loads successfully</returns>
        public bool LoadHexFile(string filePath)
        {
            try
            {
                HexFile = System.IO.File.ReadAllLines(filePath);
                if(HexFile.Length == 0)
                {
                    throw new Exception("Empty File");
                }
            }
            catch(Exception e)
            {
                return false;
            }

            HexFileCurrentLine = 0;

            return true;
        }

        /// <summary>
        /// Gets next hex record from the hex file
        /// </summary>
        /// <param name="HexRec">Reference array to store Hex record in</param>
        /// <param name="BuffLen">Buffer Length</param>
        /// <returns>Length of the hex record in bytes</returns>
        private UInt16 GetNextHexRecord(ref byte[] HexRec, UInt32 BuffLen)
        {
	        UInt16 len = 0;
            string line;
            if(HexFileCurrentLine == HexFile.Length)
            {
                return 0;
            }

            line = HexFile[HexFileCurrentLine++];

		    if(line[0] != ':')
		    {
			    // Not a valid hex record.
			    return 0;
		    }
		    // Convert rest to hex.
		    len = ConvertAsciiToHex(line.Substring(1), ref HexRec);
	        return len;
        }

         /// <summary>
         /// Resets hex file pointer
         /// </summary>
         /// <param name=""></param>
         /// <returns>True if resets file pointer</returns>
        public bool ResetHexFilePointer()
        {
	        // Reset file pointer.
	        if(HexFile == null)
	        {
		        return false;
	        }
	        else
	        {
                HexFileCurrentLine = 0;
		        return true;
	        }
        }


        /// <summary>
        /// Converts ASCII to hex
        /// </summary>
        /// <param name="AsciiRec">Hex Record in ASCII format</param>
        /// <param name="HexRec">Hex record in Hex format</param>
        /// <returns>Number of bytes in Hex record (hex format)</returns>
        private UInt16 ConvertAsciiToHex(string AsciiRec, ref Byte[] HexRec)
        {
            UInt16 i;
            List<string> hexValues = new List<string>();
            // split asci into 2-character hex values
            for (i = 0; i < AsciiRec.Length; i += 2)
            {
                string hexValueAscii = AsciiRec.Substring(i, Math.Min(2, AsciiRec.Length - i));
                HexRec[i / 2] = (byte)Convert.ToInt16(hexValueAscii, 16);
            }
                
	        return (UInt16)(i-2); 
        }

        /// <summary>
        /// Verifies flash
        /// </summary>
        /// <param name="StartAdress">Pointer to program start address</param>
        /// <param name="ProgLen">Pointer to program length in bytes</param>
        /// <param name="crc">pointer to crc</param>
        public void VerifyFlash(ref UInt32 StartAdress, ref UInt32 ProgLen, ref UInt16 crc)
        {
	        UInt16 HexRecLen;
	        byte[] HexRec = new byte[255];
            HexRecord HexRecordSt = new HexRecord();
	        UInt32 VirtualFlashAdrs;
	        UInt32 ProgAddress;
	
	        // Virtual Flash Erase (Set all bytes to 0xFF)
	        for(int i = 0; i < VirtualFlash.Length; i++)
            {
                VirtualFlash[i] = 0xFF;
            }

            // Start decoding the hex file and write into virtual flash
            // Reset file pointer.
            ResetHexFilePointer();

	        // Reset max address and min address.
	        HexRecordSt.MaxAddress = 0;
	        HexRecordSt.MinAddress = 0xFFFFFFFF;

            while((HexRecLen = GetNextHexRecord(ref HexRec, 255)) != 0)
	        {
		        HexRecordSt.RecDataLen = HexRec[0];
		        HexRecordSt.RecType = (RecordType)HexRec[3];
                HexRecordSt.CheckSum = HexRec[4 + HexRecordSt.RecDataLen];
		        HexRecordSt.Data = HexRec.Where((byte data, int index) => index >= 4).ToArray();

                byte calculatedCrc = CalculatedHexRecCrc(HexRec, HexRecordSt.RecDataLen);

                if (HexRecordSt.CheckSum != calculatedCrc)
                {
                    throw new Exception($"Invalid checksum on line {HexFileCurrentLine}. Read=[{HexRecordSt.CheckSum.ToString("X")}] Expected=[{calculatedCrc.ToString("X")}]");
                }

		        switch(HexRecordSt.RecType)
		        {

			        case RecordType.DATA_RECORD:  //Record Type 00, data record.
				        HexRecordSt.Address = (UInt32)(((HexRec[1] << 8) & 0x0000FF00) | (HexRec[2] & 0x000000FF)) & (0x0000FFFF);
				        HexRecordSt.Address = HexRecordSt.Address + HexRecordSt.ExtLinAddress + HexRecordSt.ExtSegAddress;
				
				        ProgAddress = PA_TO_KVA0(HexRecordSt.Address);

				        if(ProgAddress < BOOT_SECTOR_BEGIN) // Make sure we are not writing boot sector.
				        {
					        if(HexRecordSt.MaxAddress < (ProgAddress + HexRecordSt.RecDataLen))
					        {
						        HexRecordSt.MaxAddress = ProgAddress + HexRecordSt.RecDataLen;
					        }

					        if(HexRecordSt.MinAddress > ProgAddress)
					        {
						        HexRecordSt.MinAddress = ProgAddress;
					        }
				
					        VirtualFlashAdrs = PA_TO_VFA(ProgAddress); // Program address to local virtual flash address

                            CopyArray(ref VirtualFlash, VirtualFlashAdrs, ref HexRec, 4u, HexRecordSt.RecDataLen);

                        }
				        break;
			
			        case RecordType.EXT_SEG_ADRS_RECORD:  // Record Type 02, defines 4 to 19 of the data address.
				        HexRecordSt.ExtSegAddress = (UInt32)((UInt32)(HexRecordSt.Data[0] << 16) & (UInt32)0x00FF0000) | ((UInt32)(HexRecordSt.Data[1] << 8) & (UInt32)0x0000FF00);				
				        HexRecordSt.ExtLinAddress = 0;
				        break;
					
			        case RecordType.EXT_LIN_ADRS_RECORD:
				        HexRecordSt.ExtLinAddress = (UInt32)((UInt32)(HexRecordSt.Data[0] << 24) & (UInt32)0xFF000000) | ((UInt32)(HexRecordSt.Data[1] << 16) & (UInt32)0x00FF0000);
				        HexRecordSt.ExtSegAddress = 0;
				        break;


			        case RecordType.END_OF_FILE_RECORD:  //Record Type 01
			        default: 
				        HexRecordSt.ExtSegAddress = 0;
				        HexRecordSt.ExtLinAddress = 0;
				        break;
		        }	
	        }

	        HexRecordSt.MinAddress -= HexRecordSt.MinAddress % 4;
	        HexRecordSt.MaxAddress += HexRecordSt.MaxAddress % 4;

	        ProgLen = HexRecordSt.MaxAddress - HexRecordSt.MinAddress;
	        StartAdress = HexRecordSt.MinAddress;
	        VirtualFlashAdrs = PA_TO_VFA(HexRecordSt.MinAddress);
	        crc = Util.CalculateCrc(VirtualFlash, VirtualFlashAdrs, ProgLen);	
        }

        private void CopyArray(ref byte[] dest, uint destStart, ref byte[] src, uint srcStart, int length)
        {
            for(int i = 0; i < length; i++)
            {
                dest[i + destStart] = src[i + srcStart];
            }
        }

        private byte CalculatedHexRecCrc(byte[] data, uint length)
        {
            byte checksum = 0;
            int i;
            for (i = 0; i < length + 4; i++)
            {
                checksum += data[i];
            }
            return (byte)(~checksum + 1);
        }
    }
}
