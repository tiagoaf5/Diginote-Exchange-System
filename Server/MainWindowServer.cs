using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Common;

namespace Server
{
    public partial class MainWindowServer : Form
    {

        private readonly Market _market;
        private int _countDown;
        private System.Threading.Timer _timer;

        public MainWindowServer(Market market)
        {
            _market = market;
            _market.AddWindow(this);
            _market.UpdateLockingEvent += TimerLock;
            InitializeComponent();
        }


        public void AddUser(IUser x, bool online)
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { AddUser(x, online); }); // Invoke using an anonymous delegate
            else
            {
                var color = online ? Color.Green : Color.Maroon;
                ListViewItem item = listView1.FindItemWithText(x.Nickname);

                if (item != null)
                {
                    item.ForeColor = color;
                    return;
                }

                var listViewItem1 = new ListViewItem(new[] { x.Nickname }, -1, color, Color.Empty, null);
                listView1.Items.Add(listViewItem1);
            }
        }


        public void TimerLock(bool start)
        {
            if (start)
            {
                Debug.WriteLine("setting Timer");
                _countDown = Constants.TimerSeconds;
                _timer = new System.Threading.Timer(timer1_Tick, null, 0, 1000);
            }
            else
            {
                _timer.Dispose();
            }
        }

        private void timer1_Tick(object sender)
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { timer1_Tick(_countDown); }); // Invoke using an anonymous delegate
            else
            {
                labelCountDown.Text = _countDown.ToString();
                _countDown--;

                if (_countDown == 0)
                    _timer.Dispose();
            }
        }

        //Loads what needs to be shown on the window - gets data from _market
        private void InitialSetup(object sender, EventArgs e)
        {
            _market.LoadUsers();
            UpdateView();
        }

        public void UpdateView()
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { UpdateView(); }); // Invoke using an anonymous delegate
            else
            {
                List<IOrder> orders = _market.GetOrdersHistoy(null);

                foreach (var order in orders)
                {
                    ListViewItem listViewItem1 = new ListViewItem(new string[]
                    {
                        order.Date,
                        order.IdUser.ToString(),
                        order.OrderType == OrderOptionEnum.Sell ? "sell" : "buy",
                        order.Wanted.ToString(),
                        order.Satisfied.ToString(),
                        (order.Wanted - order.Satisfied).ToString(),
                        order.SharePrice.ToString(CultureInfo.InvariantCulture),
                        order.Closed ? "closed" : "open"
                    }, -1);
                    listView2.Items.Add(listViewItem1);
                }

                listView2.Items[listView2.Items.Count - 1].EnsureVisible();

                UpdateChart();
            }
        }
        public void UpdateChart()
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

        private void ListView1_ItemActivate(Object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
                new DiginotesWindow(_market.GetUserDiginotes(listView1.SelectedItems[0].Text)).ShowDialog();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            _market.GetUserChannel(1).SomeMessage("oi gatinho");
        }

    }
}
