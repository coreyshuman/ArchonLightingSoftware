using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace ArchonLightingSystem
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string[] args = Environment.GetCommandLineArgs();
            SingleInstanceController controller = new SingleInstanceController();
            controller.Run(args);
            //Application.Run(new AppForm());
        }

        public class SingleInstanceController : WindowsFormsApplicationBase
        {
            private bool startInBackground = false;

            public SingleInstanceController()
            {
                IsSingleInstance = true;

                Startup += StartupHandler;
                StartupNextInstance += StartupNextInstanceHandler;
            }

            private void StartupHandler(object sender, StartupEventArgs e)
            {
                ProcessStartupArguments(e.CommandLine);
            }

            void StartupNextInstanceHandler(object sender, StartupNextInstanceEventArgs e)
            {
                ProcessStartupArguments(e.CommandLine);
                if (!startInBackground)
                {
                    AppForm form = MainForm as AppForm;
                    form.ShowForm();
                }
            }

            protected override void OnCreateMainForm()
            {
                var test = ArchonLightingSDKIntegration.AIDA64Integration.ReadData();
                MainForm = new AppForm(startInBackground);
            }

            private void ProcessStartupArguments(ReadOnlyCollection<string> args)
            {
                startInBackground = false;

                foreach (string arg in args)
                {
                    switch(arg)
                    {
                        case "/background": startInBackground = true; break;
                    }
                }


            }
        }
    }
}