using System;
using System.Collections.Generic;
using System.Text;

namespace ArchonLightingSystem
{
    partial class UsbApp
    {
        public const uint CONTROL_BUFFER_SIZE = 960;
        public const uint USB_BUFFER_SIZE = 960 + 64;

        public enum CONTROL_ERROR_CODES
        {
            RESPONSE_TOO_LONG = 0x00,
            RECEIVE_TOO_LONG,
            EEPROM_FAILED,
            USB_PACKET_OVERFLOW = 0xF0,
            INVALID_MULTIPACKET,
            UNKNOWN_COMMAND = 0xFF
        };

        public enum CONTROL_CMD
        {
            CMD_READ_EE_DEBUG = 0x10,
            CMD_READ_CONFIG = 0x30,
            CMD_WRITE_CONFIG = 0x31,
            CMD_READ_FANSPEED = 0x35,
            CMD_READ_EEPROM = 0x38,
            CMD_WRITE_EEPROM = 0x39,
            CMD_ERROR_OCCURED = 0x40
        };
    }
}
