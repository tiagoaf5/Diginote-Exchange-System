using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Common
{
    public partial class DiginotesWindow : Form
    {

        private readonly List<IDiginote> _diginotes;
        public DiginotesWindow(List<IDiginote> diginotes)
        {
            _diginotes = diginotes;
            InitializeComponent();
        }


        private void InitialSetup(object sender, EventArgs e)
        {
            label1.Text = "This wallet has " + _diginotes.Count + (_diginotes.Count == 1 ? " diginote!" : " diginotes!");

            foreach (var diginote in _diginotes)
            {
                listView1.Items.Add(new ListViewItem(new[] {
                    diginote.SerialNumber}, -1, Color.Empty, Color.Empty, new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0)));
            }
        }
    }
}
