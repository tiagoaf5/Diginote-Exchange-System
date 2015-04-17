using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Net.Sockets;
using System.Windows.Forms;
using Common;
using Timer = System.Threading.Timer;

namespace Client
{
    public partial class MainWindowClient : Form
    {
        private readonly IUser _user;
        private readonly IMarket _market;
        private int _countDown;
        private Timer _timer;

        public MainWindowClient(IUser user, IMarket market)
        {
            try
            {

                _user = user;
                _market = market;
                //_market = (IMarket)RemoteNew.New(typeof(IMarket));
                EventsRepeater repeater = new EventsRepeater();
                repeater.ChangeEvent += ChangeOperation;
                repeater.UpdateLockingEvent += new UpdateTimerLockingDelegate(TimerLock);
                _market.ChangeEvent += new ChangeDelegate(repeater.ChangeRepeater);
                _market.UpdateLockingEvent += new UpdateTimerLockingDelegate(repeater.LockingRepeater);

                this.Closing += Form1_FormClosing;
                InitializeComponent();
            }
            catch (SocketException exception)
            {
                ServerDown(exception);
            }

        }

        //Loads what needs to be shown on the window - gets data from _market and _user
        private void InitialSetup(object sender, EventArgs e)
        {
            labelWelcome.Text = "Welcome" + (_user == null ? "" : " " + _user.Name) + "!";
            numericUpDown1.DecimalPlaces = 0;
            numericUpDown1.Minimum = 0;

            UpdateView();
        }

        public void UpdateView()
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { UpdateView(); }); // Invoke using an anonymous delegate
            else
            {
                try
                {
                    labelSharePrice.Text = _market.SharePrice.ToString(CultureInfo.InvariantCulture);
                    int nDiginotes = _market.GetUserDiginotes(_user).Count;
                    labelNumberDiginotes.Text = nDiginotes.ToString();
                    numericUpDown1.Value = 0;
                    numericUpDown1.Maximum = nDiginotes;
                    UpdateChart();

                    listView_sell.Items.Clear();
                    listView_buy.Items.Clear();
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
                    else
                    {
                        button2.Enabled = true;
                        button4.Enabled = true;
                    }

                    labelAvailable.Text = _market.GetNumberOfAvailableDiginotes().ToString();
                    labelDemand.Text = _market.GetNumberOfDemmandingDiginotes().ToString();
                }
                catch (SocketException exception)
                {
                    ServerDown(exception);
                }
            }

        }

        public void ChangeOperation(ChangeOperation change, int value)
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { ChangeOperation(change,value); }); // Invoke using an anonymous delegate
            else
            {
                switch (change)
                {
                    case Common.ChangeOperation.ShareChange:
                        {
                            UpdateView();
                            CheckPendingOrders(value);
                            break;
                        }
                    case Common.ChangeOperation.UpdateInterface:
                        {

                            break;
                        }
                }
                LockButtons(true);

            }

        }

        private void CheckPendingOrders(int idUser)
        {
            //TODO CHECK if I have pendent buyOrders and sell orders

            if (idUser == _user.IdUser)
                return;

            IOrder order = _market.GetUserPendingOrder(_user);
            if (order == null)
                return;


            String text = String.Format("Do you want to {0} the remaining diginotes ({1}) at the new share price ({2}€)?",
                order.OrderType == OrderOptionEnum.Sell ? "sell" : "buy", order.Wanted - order.Satisfied, _market.SharePrice);
            DialogResult result1 = MessageBox.Show(text, "Share price changed!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result1 == DialogResult.Yes)
            {
                _market.KeepOrderOn(order);
            }
            else
            {
                _market.RevokeOrder(order);
                UpdateView();
            }
        }


        public void TimerLock(bool start)
        {
            if (start)
            {
                Debug.WriteLine("setting Timer");
                _countDown = Constants.TimerSeconds;
                _timer = new Timer(timer1_Tick, null, 0, 1000);
            }
            else
            {
                LockButtons(false);
                _timer.Dispose();
                //labelCountDown.Visible = false;
            }
        }

        private void timer1_Tick(object sender)
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { timer1_Tick(_countDown); }); // Invoke using an anonymous delegate
            else
            {


                if (!labelCountDown.Visible)
                {
                    labelCountDown.Visible = true;
                }

                labelCountDown.Text = _countDown.ToString();
                _countDown--;

                if (_countDown != 0) return;

                labelCountDown.Visible = false;
                LockButtons(false);
                _timer.Dispose();
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

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (numericUpDown1.Value <= 0)
                    return;

                int result = _market.SellDiginotes((int)numericUpDown1.Value, _user);
                if (result < numericUpDown1.Value)
                {
                    using (NewPrice np = new NewPrice((int)numericUpDown1.Value, (int)numericUpDown1.Value - result, (decimal)_market.SharePrice, true))
                    {
                        np.ShowDialog();
                        if(np.newValue > 0)
                            _market.SuggestNewSharePrice((float)np.newValue, _user, true, (int)numericUpDown1.Value - result);
                    }

                }
                else
                {
                    MessageBox.Show("Your diginotes have been sold!", "Success", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
                UpdateView();
            }
            catch (SocketException exception)
            {
                ServerDown(exception);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (numericUpDown2.Value <= 0)
                    return;

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
                UpdateView();
            }
            catch (SocketException exception)
            {
                ServerDown(exception);
            }
        }

        private void ServerDown(SocketException exception)
        {
            MessageBox.Show(exception.Message, "Server is not online!", MessageBoxButtons.OK,
                MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            this.Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosing(object sender, EventArgs e)
        {
            try
            {
                _market.Logout(_user);
            }
            catch (SocketException exception)
            {

            }
        }


        public void AddMessage(string message)
        {
            /* MessageBox.Show(message, "Message from server", MessageBoxButtons.OK,
                  MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);*/
            button1.BackColor = Color.Red;
        }

        private void labelNumberDiginotes_Click(object sender, EventArgs e)
        {
            new DiginotesWindow(_market.GetUserDiginotes(_user)).ShowDialog();
        }
    }

}