using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchonLightingSystem.Interfaces;
using ArchonLightingSystem.Models;

namespace ArchonLightingSystem
{
    public partial class SubformBase : Form, ISubform
    {
        public SubformBase()
        {
            InitializeComponent();
        }

        public void InitializeForm(ApplicationData data)
        {
            throw new NotImplementedException();
        }

        public void UpdateFormData()
        {
            throw new NotImplementedException();
        }
    }
}
