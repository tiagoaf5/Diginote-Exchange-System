using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Common;

namespace Server
{
    static class ProgramServer
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            RemotingConfiguration.Configure("Server.exe.config", false);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //creating users object to give a reference to MainWindow
            Users users = (Users)RemotingServices.Connect(typeof(Users), "tcp://localhost:9000/Server/UsersManager");
            MainWindowServer myWindow = new MainWindowServer(users);
            users.AddWindow(myWindow);

            Debug.WriteLine("---------------------------------");

            Application.Run(myWindow);

            
        }

    }

    public class Diginote : IDiginote
    {
        public String SerialNumber { get; set; }
        public int Value { get; private set; }

        public IUser User { get; set; }

        public Diginote(string serialNumber)
        {
            SerialNumber = serialNumber;
            Value = 1;
        }
    }


}
