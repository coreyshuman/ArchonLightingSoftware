
using ArchonLightingSystem.Models;
using ArchonLightingSystem.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchonLightingSystem.OpenHardware;
using System.Collections.ObjectModel;
using ArchonLightingSystem.Components;

namespace ArchonLightingSystem.Forms
{
    public partial class FanConfigurationForm : Form
    {
        private HardwareManager hardwareManager;
        private ApplicationData appData;
        private DragWindowSupport dragSupport = new DragWindowSupport();
        private Node currentNode = null;
        private string identifier = "";

        public string Identifier
        {
            get
            {
                return identifier;
            }
        }

        public FanConfigurationForm()
        {
            InitializeComponent();
            dragSupport.Initialize(this);
        }

        public void InitializeForm(ApplicationData ad, HardwareManager hm)
        {
            appData = ad;
            hardwareManager = hm;
            InitializeGrid();
        }

        void InitializeGrid()
        {
            ImageList imageList = new ImageList { ImageSize = new Size(16, 16) };
            ResourceManager resourceManager = new ResourceManager(typeof(Resources));

            ResourceSet resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                string resourceKey = entry.Key.ToString();
                object resource = entry.Value;
                if(resource.GetType().BaseType == typeof(Image))
                    imageList.Images.Add(resourceKey, (Image)resource);
            }
            
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.SmallImageList = imageList;

            listView1.Columns.Add("Type", 150, HorizontalAlignment.Center);
            listView1.Columns.Add("Name", 240, HorizontalAlignment.Left);
            listView1.Columns.Add("Children", 120, HorizontalAlignment.Left);

            LoadGridForNode();
        }

        private void LoadGridForNode(Node topNode = null)
        {
            listView1.Items.Clear();
            Collection<Node> nodes = null;

            if(topNode == null)
            {
                nodes = hardwareManager.Nodes;
                currentNode = null;
            }
            else
            {
                nodes = topNode.Nodes;
                currentNode = topNode;
            }

            foreach (Node node in nodes)
            {
                var item = new ListViewItem(node.Text, node.ImageKey);
                item.SubItems.Add(node.Text);

                var nodeType = node.GetType();
                string colName = "Children";
                string value = node.Nodes.Count.ToString();
                if (nodeType == typeof(SensorNode))
                {
                    colName = "Value";
                    var sensor = (SensorNode)node;
                    value = sensor.Value;
                    item.Text = ((TypeNode)node.Parent).Text;
                    item.ImageKey = ((TypeNode)node.Parent).ImageKey;
                }
                else if(nodeType == typeof(HardwareNode))
                {
                    item.Text = ((HardwareNode)node).Hardware.HardwareType.ToString();
                }

                listView1.Columns[2].Text = colName;
                item.SubItems.Add(value);

                item.Tag = node;
                listView1.Items.Add(item);
            }
        }

        private void SelectSensor(SensorNode node)
        {
            lbl_Selected.Text = node.Text;
            identifier = node.Sensor.Identifier.ToString();
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            int selectedIndex = ((ListView)sender).SelectedIndices[0];
            Node node = (Node)listView1.Items[selectedIndex].Tag;
            if(node?.Nodes.Count > 0)
            {
                LoadGridForNode(node);
            }
            else
            {
                SelectSensor((SensorNode)node);
            }
        }

        private void btn_Back_Click(object sender, EventArgs e)
        {
            if(currentNode?.Parent != null)
            {
                LoadGridForNode(currentNode.Parent);
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var test = 1;
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            Node node = (Node)((ListView)sender).FocusedItem.Tag;

            if (node?.Nodes.Count == 0)
            {
                SelectSensor((SensorNode)node);
            }
        }
    }
}
