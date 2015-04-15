using System;
using System.Windows.Forms;

namespace Client
{
    static class ProgramClient
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormLogin {FormBorderStyle = FormBorderStyle.FixedSingle});
        }
    }
}
