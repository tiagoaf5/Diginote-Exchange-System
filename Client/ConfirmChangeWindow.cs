using System;
using System.Windows.Forms;
using Common;

namespace Client
{
    public partial class ConfirmChangeWindow : Form
    {


        private readonly string _message1;
        private readonly string _message2;
        private readonly string _caption;
        private readonly IMarket _market;
        private readonly IOrder _order;
        private int _countDown;
        private System.Threading.Timer _timer;
        private readonly MainWindowClient _windowClient;


        public ConfirmChangeWindow(IMarket m, IOrder order, MainWindowClient windowClient, string message1, string message2, string caption)
        {
            InitializeComponent();
            _market = m;
            _order = order;
            _message1 = message1;
            _message2 = message2;
            _caption = caption;
            _windowClient = windowClient;
        }

        private void InitialSetup(object sender, EventArgs e)
        {
            Text = _caption;

            lblMessage.Text = _message1;
            lblMessage2.Text = _message2;

            _countDown = Constants.TimerSeconds;
            _timer = new System.Threading.Timer(timer1_Tick, null, 0, 1000);
        }


        private void btnYes_Click(object sender, EventArgs e)
        {
            _market.KeepOrderOn(_order);
            _windowClient.UpdateView();
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            _market.RevokeOrder(_order);
            _windowClient.UpdateView();
            this.Close();
        }

        private void timer1_Tick(object sender)
        {
            if (InvokeRequired) // I'm not in UI thread
                BeginInvoke((MethodInvoker) delegate { timer1_Tick(sender); }); // Invoke using an anonymous delegate
            else
            {
                btnNo.Text = "&No" + " (" + _countDown + ")";
                if (_countDown == 0)
                {
                    _timer.Dispose();
                    this.Close();
                }

                _countDown--;
            }
        }

    }
}
