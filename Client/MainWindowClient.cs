using System;
using System.Collections.Generic;
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
            EventsRepeater repeater = new EventsRepeater();
            repeater.ChangeEvent += ChangeOperation;
            repeater.UpdateLockingEvent += new UpdateLockingTimeDelegate(UpdateTimer);
            _market.ChangeEvent += new ChangeDelegate(repeater.ChangeRepeater);
            _market.UpdateLockingEvent += new UpdateLockingTimeDelegate(repeater.LockingRepeater);


            InitializeComponent();
        }

        private void InitialSetup(object sender, EventArgs e)
        {
            labelWelcome.Text = "Welcome" + (_user == null ? "" : " " + _user.Name) + "!";
            labelSharePrice.Text = _market.SharePrice.ToString(CultureInfo.InvariantCulture);
            labelNumberDiginotes.Text = _user.Diginotes.Count.ToString();
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

                LockButtons(true);
            }

        }

        public void UpdateTimer(int seconds)
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker) delegate { UpdateTimer(seconds); }); // Invoke using an anonymous delegate
            else
            {
                if (!labelCountDown.Visible)
                {
                    labelCountDown.Visible = true;
                    labelLocked.Visible = true;
                }

                labelCountDown.Text = seconds.ToString();

                if (seconds == 0)
                {
                    labelCountDown.Visible = false;
                    labelLocked.Visible = false;
                    LockButtons(false);
                }
            }

           
               
        }

        private void LockButtons(bool locked)
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { LockButtons(locked); }); // Invoke using an anonymous delegate
            else
            {
                List<Control> list = new List<Control>();

                GetAllControl(this, list);

                foreach (Control control in list)
                {
                    if (control.GetType() == typeof(Button))
                    {
                        control.Enabled = !locked;
                    }
                }
            }

        }

        private void GetAllControl(Control c, List<Control> list)
        {
            foreach (Control control in c.Controls)
            {
                list.Add(control);
                if (control.GetType() == typeof(Panel) || control.GetType() == typeof(TabControl) || control.GetType() == typeof(TabPage))
                    GetAllControl(control, list);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
           // _market.SuggestNewSharePrice((float)(Math.Floor((new Random()).NextDouble() * 100) / 100.0), _user);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int result = _market.SellDiginotes((int)numericUpDown1.Value,_user);
            if (result < numericUpDown1.Value)
            {
                using (NewPrice np = new NewPrice((int)numericUpDown1.Value, (int)numericUpDown1.Value - result, (decimal)_market.SharePrice))
                {
                    var r1 = np.ShowDialog();
                    _market.SuggestNewSharePrice((float)np.newValue, _user, true, (int)numericUpDown1.Value - result);
                }

            }
            else
            {
                MessageBox.Show("Your diginotes have been sold!", "Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            numericUpDown1.Value = 0;
            numericUpDown1.Maximum = _user.Diginotes.Count;
        }
    }

}
