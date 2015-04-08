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

            //creating users object to give a reference to MainWindow
            Users users = (Users)RemotingServices.Connect(typeof(Users), "tcp://localhost:9000/Server/UsersManager");
            MainWindowServer myWindow = new MainWindowServer(users);
            users.AddWindow(myWindow);

            Debug.WriteLine("---------------------------------");

            Application.Run(myWindow);

            
        }

    }

    public class Diginote : MarshalByRefObject, IDiginote
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

    public class Market : MarshalByRefObject, IMarket
    {
        public event ChangeDelegate ChangeEvent;
        public List<IDiginote> BuyDiginotes(int quantity)
        {
            throw new NotImplementedException();
        }

        public int SellDiginotes(int quantity)
        {
            throw new NotImplementedException();
        }

        public void SuggestNewSharePrice(float newPrice)
        {
            if (ChangeEvent != null)
            {
                Delegate[] invkList = ChangeEvent.GetInvocationList();

                foreach (ChangeDelegate handler in invkList)
                {
                    var handler1 = handler;
                    new Thread(() =>
                    {
                        try
                        {
                            handler1(newPrice, ChangeOperation.ShareUp);
                            Debug.WriteLine("Invoking event handler");
                        }
                        catch (Exception)
                        {
                            ChangeEvent -= handler1;
                            Debug.WriteLine("Exception: Removed an event handler");
                        }
                    }).Start();
                }
            }
        }
    }


}
