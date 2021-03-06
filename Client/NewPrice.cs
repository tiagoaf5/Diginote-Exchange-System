﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class NewPrice : Form
    {

        public decimal newValue { get; set; }

        public NewPrice(int total, int remaining, decimal current_value, bool sell)
        {
            InitializeComponent();
            valueUpDown.DecimalPlaces = 2;
            valueUpDown.Increment = 0.01M;
            if (sell)
            {
                valueUpDown.Minimum = 0.01M;
                valueUpDown.Maximum = current_value;
            }
            else
            {
                valueUpDown.Minimum = current_value;
            }

            valueUpDown.Value = current_value;

            newValue = -1;

            string sell_buy;
            if (sell)
                sell_buy = "sell ";
            else sell_buy = "buy ";

            label.Text = "We were able to " + sell_buy + (total-remaining) + " diginotes.\n Suggest a new price for the remaining " + remaining + " diginotes.";
        }

        public NewPrice(int number, decimal current_value, bool sell)
        {
            InitializeComponent();
            valueUpDown.DecimalPlaces = 2;
            valueUpDown.Increment = 0.01M;
            if (sell)
            {
                valueUpDown.Minimum = 0.01M;
                valueUpDown.Maximum = current_value-0.01M;
                valueUpDown.Value = current_value - 0.01M;
            }
            else
            {
                valueUpDown.Minimum = current_value + 0.01M ;
                valueUpDown.Value = current_value + 0.01M;
            }


            newValue = -1;

            label.Text = "You are currently selling at "+ current_value +"€.\n Suggest a new price for  the " + number + " diginotes.";

        }

        private void valueUpDown_ValueChanged(object sender, EventArgs e)
        {
           
        }

        private void submit_Click(object sender, EventArgs e)
        {
            newValue = valueUpDown.Value;
            this.Close();
        }

        private void label_Click(object sender, EventArgs e)
        {

        }
    }
}
