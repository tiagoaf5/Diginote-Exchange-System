using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
        private IMarket _market;

        public MainWindowClient(IUser user, IUsers users)
        {
            _user = user;
            _users = users;

            _market = (IMarket)RemoteNew.New(typeof(IMarket));
            ChangeEventRepeater repeater = new ChangeEventRepeater();
            repeater.ChangeEvent += new ChangeDelegate(ChangeOperation);
            _market.ChangeEvent += new ChangeDelegate(repeater.Repeater);


            InitializeComponent();
        }

        private void InitialSetup(object sender, EventArgs e)
        {
            labelWelcome.Text = "Welcome" + (_user == null ? "" : " " + _user.Name) + "!";
        }

        public void ChangeOperation(float newPrice, ChangeOperation change)
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { ChangeOperation(newPrice, change); }); // Invoke using an anonymous delegate
            else
            {
                labelSharePrice.Text = newPrice.ToString(CultureInfo.CurrentCulture);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _market.SuggestNewSharePrice((float)(new Random()).NextDouble());
        }
    }

}
