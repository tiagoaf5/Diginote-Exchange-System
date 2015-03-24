using System;
using System.Windows.Forms;
using Common;

namespace Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public class Users : MarshalByRefObject, IUsers
    {

        public IUser LogUser(string nickname, string password)
        {
            return new User();
        }

        public bool RegisterUser(string nickname, string password, string name)
        {
            throw new NotImplementedException();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }

    public class User : MarshalByRefObject, IUser
    {


        public User()
        {
            Name = "Manuel";
            Nickname = "Manel";
        }
        public string Name
        {
            get;
            private set;
        }

        public string Nickname { get; private set; }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
