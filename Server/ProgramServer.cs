using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Threading;
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

            //creating Market object to give a reference to MainWindow
            Market _market = (Market)RemotingServices.Connect(typeof(Market), "tcp://localhost:9000/Server/MarketManager");
            MainWindowServer myWindow = new MainWindowServer(_market);
            //_market.AddWindow(myWindow);

            Debug.WriteLine("---------------------------------");

            Application.Run(myWindow);

            
        }

    }

    public class Diginote : MarshalByRefObject, IDiginote
    {
        public string SerialNumber { get; set; }
        public int Value { get; private set; }

        public IUser User { get; set; }

        public Diginote(string serialNumber)
        {
            SerialNumber = serialNumber;
            Value = 1;
        }
    }


}
