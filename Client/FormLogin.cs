using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Common;

namespace Client
{
    public partial class FormLogin : Form
    {
        private int _port;
        private IUser _user;
        private readonly IMarket _market;
        private MainWindowClient _mainWindow;

        public FormLogin(int port)
        {
            this._port = port;
            //RemotingConfiguration.Configure("Client.exe.config", false);
            _market = (IMarket)RemoteNew.New(typeof(IMarket));
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (textUsername.Text != "" && textPassword.Text != "")
            {
                try
                {
                    _user = _market.LogUser(textUsername.Text, textPassword.Text, "tcp://localhost:" + _port.ToString() + "/ClientNotify");
                }
                catch (System.Net.Sockets.SocketException exception)
                {
                    MessageBox.Show(exception.Message, "Server is not online!", MessageBoxButtons.OK,
                      MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    this.Close();
                }

                if (_user != null) //success
                {
                    MessageBox.Show("Login successful!", "Welcome", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    
                    OpenMainWindow();

                }
                else //wrong credentials
                {
                    MessageBox.Show("Wrong credentials!", "Error", MessageBoxButtons.OK,
                  MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
            else //not all fields filled in
            {
                MessageBox.Show("Please insert username and password!", "Warning", MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            var name = textRName.Text;
            var nickname = textRNickname.Text;
            var password = textRpassword.Text;
            var password1 = textRpassword1.Text;

            if (name == "" || nickname == "" || password == "" || password1 == "") //not everything fillen in
            {
                MessageBox.Show("You must fill in all fields!", "Warning", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
            else if (password != password1) //passwords don't match
            {
                MessageBox.Show("Passwords don't match!", "Warning", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                textRpassword.Text = "";
                textRpassword1.Text = "";
            }
            else //success
            {
                try
                {
                    _user = _market.RegisterUser(nickname, password, name, "tcp://localhost:" + _port.ToString() + "/ClientNotify");
                }
                catch (System.Net.Sockets.SocketException exception)
                {
                    MessageBox.Show(exception.Message, "Server is not online!", MessageBoxButtons.OK,
                      MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    this.Close();
                }

                if (_user == null)
                {
                    MessageBox.Show("Username already taken! Choose another one!", "Warning", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    textRNickname.Text = "";
                }
                else //register successful
                {
                    MessageBox.Show("You've registered with success!", "Welcome", MessageBoxButtons.OK,
                     MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                    OpenMainWindow();
                }
            }
        }

        private void OpenMainWindow()
        {
            this.Hide();

            _mainWindow = new MainWindowClient(_user, _market)
            {
                FormBorderStyle = FormBorderStyle.FixedSingle
            };

            ClientNotify r = (ClientNotify)RemotingServices.Connect(typeof(ClientNotify), "tcp://localhost:" + _port.ToString() + "/ClientNotify");    // connect to the registered my remote object here
            r.PutMyForm(_mainWindow);
            _mainWindow.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new ConfirmChangeWindow(null,null,null,"","", "nabo").Show();
        }

    }
}
