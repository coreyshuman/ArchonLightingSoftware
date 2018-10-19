using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchonLightingSystem.Common;
using ArchonLightingSystem.Properties;
using OpenHardwareMonitor.Collections;
using OpenHardwareMonitor.Hardware;
using OpenHardwareMonitor.Software;

namespace ArchonLightingSystem.OpenHardware
{
    public class HardwareManager
    {
        private UpdateVisitor updateVisitor = new UpdateVisitor();

        private HardwareSettings settings;
        private string settingsFilename;
        private UnitManager unitManager;
        private Computer computer;
        private Node rootNode;
        

        public HardwareManager()
        {
            this.settings = new HardwareSettings();
            settingsFilename = Path.ChangeExtension(
                System.Windows.Forms.Application.ExecutablePath, ".config");
            this.settings.Load(settingsFilename);

            this.unitManager = new UnitManager(settings);

            rootNode = new Node(System.Environment.MachineName);
            //rootNode.Image = Resources.computer;
            rootNode.ImageKey = Util.GetPropertyName(() => Resources.computer);

            this.computer = new Computer(settings);

            computer.HardwareAdded += new HardwareEventHandler(HardwareAdded);
            computer.HardwareRemoved += new HardwareEventHandler(HardwareRemoved);

            computer.Open();

            computer.MainboardEnabled = true;
            computer.CPUEnabled = true;
            computer.GPUEnabled = true;
            computer.HDDEnabled = true;
            computer.RAMEnabled = true;
        }

        public Collection<Node> Nodes
        {
            get
            {
                return rootNode.Nodes;
            }
        }

        public void UpdateReadings()
        {
            computer.Accept(updateVisitor);
        }

        public void Close()
        {
            computer.Close();

            try
            {
                settings.Save(settingsFilename);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Access to the path '" + settingsFilename + "' is denied. " +
                  "The current settings could not be saved.",
                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException)
            {
                MessageBox.Show("The path '" + settingsFilename + "' is not writeable. " +
                  "The current settings could not be saved.",
                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public ISensor GetSensorByIdentifier(string identifier, Node node = null)
        {
            if (node == null) node = this.rootNode;

            var type = node.GetType();
            
             if (type == typeof(HardwareNode))
            {
                HardwareNode hNode = (HardwareNode)node;
                if (identifier.Contains(hNode.Hardware.Identifier.ToString()))
                {
                    foreach (ISensor sensor in hNode.Hardware.Sensors)
                    {
                        if (sensor.Identifier.ToString() == identifier)
                        {
                            return sensor;
                        }
                    }
                }

                foreach (Hardware subHardware in hNode.Hardware.SubHardware)
                {
                    if (identifier.Contains(subHardware.Identifier.ToString())) {
                        foreach (ISensor sensor in subHardware.Sensors)
                        {
                            if (sensor.Identifier.ToString() == identifier)
                            {
                                return sensor;
                            }
                        }
                    }
                }

            }
            else if (type == typeof(SensorNode))
            {
                if (identifier == ((SensorNode)node).Sensor.Identifier.ToString())
                {
                    return ((SensorNode)node).Sensor;
                }
            }
            else
            {
                foreach (Node subnode in node.Nodes)
                {
                    var result = GetSensorByIdentifier(identifier, subnode);
                    if (result != null) return result;
                }
            }

            return null;
        }

        private void InsertSorted(Collection<Node> nodes, HardwareNode node)
        {
            int i = 0;
            while (i < nodes.Count && nodes[i] is HardwareNode &&
              ((HardwareNode)nodes[i]).Hardware.HardwareType <
                node.Hardware.HardwareType)
                i++;
            nodes.Insert(i, node);
        }

        private void SubHardwareAdded(IHardware hardware, Node node)
        {
            HardwareNode hardwareNode =
              new HardwareNode(hardware, settings, unitManager);

            InsertSorted(node.Nodes, hardwareNode);

            foreach (IHardware subHardware in hardware.SubHardware)
                SubHardwareAdded(subHardware, hardwareNode);
        }

        private void HardwareAdded(IHardware hardware)
        {
            SubHardwareAdded(hardware, rootNode);
        }

        private void HardwareRemoved(IHardware hardware)
        {
            List<HardwareNode> nodesToRemove = new List<HardwareNode>();
            foreach (Node node in rootNode.Nodes)
            {
                HardwareNode hardwareNode = node as HardwareNode;
                if (hardwareNode != null && hardwareNode.Hardware == hardware)
                    nodesToRemove.Add(hardwareNode);
            }
            foreach (HardwareNode hardwareNode in nodesToRemove)
            {
                rootNode.Nodes.Remove(hardwareNode);
            }
        }
    }
}
