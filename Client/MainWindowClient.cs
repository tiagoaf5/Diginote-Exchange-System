using System;
using System.Globalization;
using System.Windows.Forms;
using Common;

namespace Client
{
    public partial class MainWindowClient : Form
    {
        private readonly IUser _user;
        private readonly IMarket _market;

        public MainWindowClient(IUser user, IMarket market)
        {
            _user = user;
            _market = market;
            //_market = (IMarket)RemoteNew.New(typeof(IMarket));
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
            _market.SuggestNewSharePrice((float)(new Random()).NextDouble(), _user);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }

}
