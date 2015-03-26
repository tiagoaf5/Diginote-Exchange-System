using System;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Common;

namespace Client
{
    public partial class FormLogin : Form
    {
        private IUser user;

        public FormLogin()
        {
            RemotingConfiguration.Configure("Client.exe.config", false);
            InitializeComponent();



        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (textUsername.Text != "" && textPassword.Text != "")
            {
                IUsers users = (IUsers)RemoteNew.New(typeof(IUsers));
                user = users.LogUser(textUsername.Text, textPassword.Text);
                if (user != null)
                {
                    MessageBox.Show("Login successful!", "Welcome", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
                else
                {
                    MessageBox.Show("Wrong credentials!", "Error", MessageBoxButtons.OK,
                  MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                MessageBox.Show("Please insert username and password!", "Warning", MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }
    }
}
