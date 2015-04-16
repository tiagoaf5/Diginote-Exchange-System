using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.Remoting;
using System.Text;
using System.Windows.Forms;
using Common;

namespace Client
{
    public partial class MainWindowClient : Form
    {
        private IUser _user;
        private IMarket _market;

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
            numericUpDown1.DecimalPlaces = 0;
            numericUpDown1.Minimum = 0;

            UpdateView();
        }

        private void UpdateView()
        {
            labelSharePrice.Text = _market.SharePrice.ToString(CultureInfo.InvariantCulture);
            int nDiginotes = _market.GetUserDiginotes(_user).Count;
            labelNumberDiginotes.Text = nDiginotes.ToString();
            numericUpDown1.Value = 0;
            numericUpDown1.Maximum = nDiginotes;
            UpdateChart();

            listView_sell.Items.Clear();
            IOrder order = _market.GetUserPendingOrder(_user);
            if (order != null)
            {
                button2.Enabled = false;
                button4.Enabled = false;
                string[] info = { Convert.ToString(order.Wanted), Convert.ToString(order.Satisfied), Convert.ToString(order.Wanted - order.Satisfied) };
                if (order.OrderType == OrderOptionEnum.Sell)
                    listView_sell.Items.Add(new ListViewItem(info));
                else listView_buy.Items.Add(new ListViewItem(info));

            }

        }

        public void ChangeOperation(ChangeOperation change)
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { ChangeOperation(change); }); // Invoke using an anonymous delegate
            else
            {
                switch (change)
                {
                    case Common.ChangeOperation.ShareChange:
                        {

                            break;
                        }
                    case Common.ChangeOperation.UpdateInterface:
                        {

                            break;
                        }
                }
                LockButtons(true);
                //TODO: checkPendingOrders();
                UpdateView();
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
                series1.Points.Clear();
                ArrayList history = _market.GetSharePricesList();

                int size = history.Count;
                for (int i = 0; i < size; i++)
                    series1.Points.AddXY(i, history[i]);
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

            if (numericUpDown1.Value <= 0)
                return;

            int result = _market.SellDiginotes((int)numericUpDown1.Value, _user);
            if (result < numericUpDown1.Value)
            {
                using (NewPrice np = new NewPrice((int)numericUpDown1.Value, (int)numericUpDown1.Value - result, (decimal)_market.SharePrice, true))
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


        public void AddMessage(string message)
        {
           /* MessageBox.Show(message, "Message from server", MessageBoxButtons.OK,
                 MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);*/
            button1.BackColor = Color.Red;
        }
    }

}
