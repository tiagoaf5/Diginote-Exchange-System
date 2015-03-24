using System;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Common;

namespace Client
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            RemotingConfiguration.Configure("Client.exe.config", false);
            InitializeComponent();

            IUsers users = (IUsers)RemoteNew.New(typeof(IUsers));
            IUser user = users.LogUser("nabo", "nabo");

            Console.WriteLine("user: {0}, {1}", user.Name, user.Nickname);
        }
    }
}
