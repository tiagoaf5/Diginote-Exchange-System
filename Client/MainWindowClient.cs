using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;

namespace Client
{
    public partial class MainWindowClient : Form
    {
        private IUser _user;
        private readonly IUsers _users;

        public MainWindowClient(IUser user, IUsers users)
        {
            _user = user;
            _users = users;
            InitializeComponent();
        }

        private void InitialSetup(object sender, EventArgs e)
        {
            labelWelcome.Text = "Welcome" + (_user == null ? "" : " " + _user.Name) + "!";
        }
    }
}
