using System;
using System.Globalization;
using System.Windows.Forms;
using Common;
using System.Diagnostics;

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
            numericUpDown1.DecimalPlaces = 0;
            numericUpDown1.Maximum = _user.Diginotes.Count;
            numericUpDown1.Minimum = 0;
        }

        public void ChangeOperation(float newPrice, ChangeOperation change)
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { ChangeOperation(newPrice, change); }); // Invoke using an anonymous delegate
            else
            {
                labelSharePrice.Text = newPrice.ToString(CultureInfo.CurrentCulture);
                Debug.WriteLine(_market.SharePrice);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _market.SuggestNewSharePrice((float)(Math.Floor((new Random()).NextDouble() * 100) / 100.0), _user);
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

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int result = _market.SellDiginotes((int)numericUpDown1.Value);
            if (result < numericUpDown1.Value)
            {
                using (NewPrice np = new NewPrice((int)numericUpDown1.Value,(int)numericUpDown1.Value-result, (decimal)_market.SharePrice))
                {
                    var r1 = np.ShowDialog();
                    _market.SuggestNewSharePrice((float)np.newValue, _user);
                }

            }
            else
            {
                MessageBox.Show("Information", "Your diginotes have been sold!", MessageBoxButtons.OK,
                MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            numericUpDown1.Value = 0;
            numericUpDown1.Maximum = _user.Diginotes.Count;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }

}
