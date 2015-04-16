using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
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
            EventsRepeater repeater = new EventsRepeater();
            repeater.ChangeEvent += ChangeOperation;
            repeater.UpdateLockingEvent += new UpdateLockingTimeDelegate(UpdateTimer);
            _market.ChangeEvent += new ChangeDelegate(repeater.ChangeRepeater);
            _market.UpdateLockingEvent += new UpdateLockingTimeDelegate(repeater.LockingRepeater);

            this.Closing += Form1_FormClosing;
            InitializeComponent();
        }

        //Loads what needs to be shown on the window - gets data from _market and _user
        private void InitialSetup(object sender, EventArgs e)
        {
            labelWelcome.Text = "Welcome" + (_user == null ? "" : " " + _user.Name) + "!";
            labelSharePrice.Text = _market.SharePrice.ToString(CultureInfo.InvariantCulture);

            int nDiginotes = _market.GetUserDiginotes(_user).Count;
            labelNumberDiginotes.Text = nDiginotes.ToString();
            numericUpDown1.DecimalPlaces = 0;
            numericUpDown1.Maximum = nDiginotes;
            numericUpDown1.Minimum = 0;

            UpdateChart();
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
                //TODO: checkPendingOrders();
                UpdateChart();
            }

        }

        private void checkPendingOrders()
        {
            //TODO CHECK if I have pendent buyOrders and sell orders
            String text = String.Format("Do you want to {0} the remaining diginotes ({1}) at the new share price ({2}€)?", "sell", 10, 0.51);
            DialogResult result1 = MessageBox.Show(text,
            "Share price changed!",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result1 == DialogResult.Yes)
            {
                Debug.WriteLine("->YES");
                InitialSetup(null, null);
            }
            else
            {
                Debug.WriteLine("->NO");
            }
        }

        public void UpdateTimer(int seconds)
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { UpdateTimer(seconds); }); // Invoke using an anonymous delegate
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

        private void UpdateChart()
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { UpdateChart(); }); // Invoke using an anonymous delegate
            else
            {
                series2.Points.Clear();
                ArrayList history = _market.GetSharePricesList();

                int size = history.Count;
                for (int i = 0; i < size; i++)
                    series2.Points.AddXY(i, history[i]);
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
            //_market.SuggestNewSharePrice((float)(Math.Floor((new Random()).NextDouble() * 100) / 100.0), _user);
            //series1.Points.Add(new DataPoint(12, 3));
            checkPendingOrders();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int result = _market.SellDiginotes((int)numericUpDown1.Value, _user);
            if (result < numericUpDown1.Value)
            {
                using (NewPrice np = new NewPrice((int)numericUpDown1.Value, (int)numericUpDown1.Value - result, (decimal)_market.SharePrice,true))
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
            listView_sell.Items.Clear();
            if (result < numericUpDown1.Value)
            {
                string[] info = { "" + numericUpDown1.Value, "" + result, "" + ((int)numericUpDown1.Value - result) };
                listView_sell.Items.Add(new ListViewItem(info));
            }
            numericUpDown1.Value = 0;
            numericUpDown1.Maximum = _market.GetUserDiginotes(_user).Count;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int result = _market.BuyDiginotes((int)numericUpDown2.Value, _user);
            if (result < numericUpDown2.Value)
            {
                using (NewPrice np = new NewPrice((int)numericUpDown2.Value, (int)numericUpDown2.Value - result, (decimal)_market.SharePrice, false))
                {
                    var r1 = np.ShowDialog();
                    _market.SuggestNewSharePrice((float)np.newValue, _user, false, (int)numericUpDown2.Value - result);
                }

            }
            else
            {
                MessageBox.Show("Your have new diginotes!", "Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            numericUpDown1.Value = 0;
            numericUpDown1.Maximum = _market.GetUserDiginotes(_user).Count;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosing(object sender, EventArgs e)
        {
            _market.Logout(_user);
        }


    }

}
