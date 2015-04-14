using System;
using System.Drawing;
using System.Windows.Forms;
using Common;

namespace Server
{
    public partial class MainWindowServer : Form
    {

        private Market _market;

        public MainWindowServer(Market market)
        {
            _market = market;
            _market.AddWindow(this);
            _market.UpdateLockingEvent += new UpdateLockingTimeDelegate(UpdateTimer);
            InitializeComponent();
        }


        public void AddUser(User x, bool online)
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker) delegate { AddUser(x, online); }); // Invoke using an anonymous delegate
            else
            {
                var color = online ? Color.Green : Color.Maroon;
                ListViewItem item = listView1.FindItemWithText(x.Nickname);
                
                if ( item != null)
                {
                    item.ForeColor = color;
                    return;
                }

                var listViewItem1 = new ListViewItem(new[] { x.Nickname}, -1, color, Color.Empty, null);
                listView1.Items.Add(listViewItem1);
            }
        }


        public void UpdateTimer(int seconds)
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { UpdateTimer(seconds); }); // Invoke using an anonymous delegate
            else
            {
                labelCountDown.Text = seconds.ToString();
            }
        }

        private void InitialSetup(object sender, EventArgs e)
        {
            _market.LoadUsers();
        }
 
    }
}
