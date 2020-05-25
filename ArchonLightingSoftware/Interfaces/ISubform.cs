using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchonLightingSystem.Models;

namespace ArchonLightingSystem.Interfaces
{
    public interface ISubform
    {
        void InitializeForm(ApplicationData data);
        void UpdateFormData();
    }
}
