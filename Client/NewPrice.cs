using System;
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

        public NewPrice(int total, int remaining, decimal current_value)
        {
            InitializeComponent();
            valueUpDown.DecimalPlaces = 2;
            valueUpDown.Increment = 0.01M;
            valueUpDown.Minimum = 0.01M;
            valueUpDown.Maximum = current_value;
            valueUpDown.Value = current_value;

            label.Text = "We were able to sell " + (total-remaining) + " diginotes.\n Suggest a new price for the remaining " + remaining + " diginotes.";
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
