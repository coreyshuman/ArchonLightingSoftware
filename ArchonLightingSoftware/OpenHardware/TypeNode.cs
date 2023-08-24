/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2009-2012 Michael Möller <mmoeller@openhardwaremonitor.org>
  Copyright (C) 2018-2019 Corey Shuman <ctshumancode@gmail.com>
	
*/

using ArchonLightingSystem.Common;
using ArchonLightingSystem.Properties;
using LibreHardwareMonitor.Hardware;

namespace ArchonLightingSystem.OpenHardware
{
    public class TypeNode : Node
    {
        private SensorType sensorType;

        public TypeNode(SensorType sensorType) : base()
        {
            this.sensorType = sensorType;

            switch (sensorType)
            {
                case SensorType.Voltage:
                    this.ImageKey = Util.GetPropertyName(() => Resources.voltage);
                    this.Text = "Voltage";
                    break;       
                case SensorType.Clock:
                    this.ImageKey = Util.GetPropertyName(() => Resources.clock);
                    this.Text = "Clock";
                    break;       
                case SensorType.Load:
                    this.ImageKey = Util.GetPropertyName(() => Resources.load);
                    this.Text = "Load";
                    break;
                case SensorType.Temperature:
                    this.ImageKey = Util.GetPropertyName(() => Resources.temperature);
                    this.Text = "Temperature";
                    break;
                case SensorType.Fan:
                    this.ImageKey = Util.GetPropertyName(() => Resources.fan);
                    this.Text = "Fan";
                    break;
                case SensorType.Flow:
                    this.ImageKey = Util.GetPropertyName(() => Resources.flow);
                    this.Text = "Flow";
                    break;
                case SensorType.Control:
                    this.ImageKey = Util.GetPropertyName(() => Resources.control);
                    this.Text = "Control";
                    break;
                case SensorType.Level:
                    this.ImageKey = Util.GetPropertyName(() => Resources.level);
                    this.Text = "Level";
                    break;
                case SensorType.Power:
                    this.ImageKey = Util.GetPropertyName(() => Resources.power);
                    this.Text = "Power";
                    break;
                case SensorType.Data:
                    this.ImageKey = Util.GetPropertyName(() => Resources.data);
                    this.Text = "Data";
                    break;
                case SensorType.SmallData:
                    this.ImageKey = Util.GetPropertyName(() => Resources.data);
                    this.Text = "Data";
                    break;
                case SensorType.Factor:
                    this.ImageKey = Util.GetPropertyName(() => Resources.factor);
                    this.Text = "Factor";
                    break;
                case SensorType.Frequency:
                    this.ImageKey = Util.GetPropertyName(() => Resources.clock);
                    this.Text = "Frequency";
                    break;
                case SensorType.Throughput:
                    this.ImageKey = Util.GetPropertyName(() => Resources.throughput);
                    this.Text = "Throughput";
                    break;
                case SensorType.Current:
                    this.ImageKey = Util.GetPropertyName(() => Resources.current);
                    this.Text = "Current";
                    break;
                case SensorType.Noise:
                    this.ImageKey = Util.GetPropertyName(() => Resources.loudspeaker);
                    this.Text = "Noise Level";
                    break;
                case SensorType.TimeSpan:
                    this.ImageKey = Util.GetPropertyName(() => Resources.time);
                    this.Text = "Time";
                    break;
                case SensorType.Energy:
                    this.ImageKey = Util.GetPropertyName(() => Resources.battery);
                    this.Text = "Capacity";
                    break;
                default:
                    this.Text = "Unknown";
                    break;
            }

            NodeAdded += new NodeEventHandler(TypeNode_NodeAdded);
            NodeRemoved += new NodeEventHandler(TypeNode_NodeRemoved);
        }

        private void TypeNode_NodeRemoved(Node node)
        {
            node.IsVisibleChanged -= new NodeEventHandler(node_IsVisibleChanged);
            node_IsVisibleChanged(null);
        }

        private void TypeNode_NodeAdded(Node node)
        {
            node.IsVisibleChanged += new NodeEventHandler(node_IsVisibleChanged);
            node_IsVisibleChanged(null);
        }

        private void node_IsVisibleChanged(Node node)
        {
            foreach (Node n in Nodes)
                if (n.IsVisible)
                {
                    this.IsVisible = true;
                    return;
                }
            this.IsVisible = false;
        }

        public SensorType SensorType
        {
            get { return sensorType; }
        }
    }
}
