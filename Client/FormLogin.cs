﻿using System;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Common;

namespace Client
{
    public partial class FormLogin : Form
    {
        private IUser _user;
        private readonly IUsers _users;

        public FormLogin()
        {
            RemotingConfiguration.Configure("Client.exe.config", false);
            _users = (IUsers)RemoteNew.New(typeof(IUsers));
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (textUsername.Text != "" && textPassword.Text != "")
            {
                try
                {
                    _user = _users.LogUser(textUsername.Text, textPassword.Text);
                }
                catch (System.Net.Sockets.SocketException exception)
                {
                    MessageBox.Show(exception.Message, "Server is not online!", MessageBoxButtons.OK,
                      MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    this.Close();
                }

                if (_user != null)
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
                    _user = _users.RegisterUser(nickname, password, name);
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
                }
            }
        }

    }
}
