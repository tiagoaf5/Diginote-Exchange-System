using System.ComponentModel;
using System.Windows.Forms;

namespace Server
{
    partial class MainWindowServer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnNickname = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelCountDown = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnNickname});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(121, 308);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnNickname
            // 
            this.columnNickname.Text = "Market";
            this.columnNickname.Width = 121;
            // 
            // labelCountDown
            // 
            this.labelCountDown.AutoSize = true;
            this.labelCountDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCountDown.ForeColor = System.Drawing.Color.Red;
            this.labelCountDown.Location = new System.Drawing.Point(649, 9);
            this.labelCountDown.Name = "labelCountDown";
            this.labelCountDown.Size = new System.Drawing.Size(22, 16);
            this.labelCountDown.TabIndex = 1;
            this.labelCountDown.Text = "59";
            // 
            // MainWindowServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 308);
            this.Controls.Add(this.labelCountDown);
            this.Controls.Add(this.listView1);
            this.Name = "MainWindowServer";
            this.Text = "Diginote Exchange System - Server";
            this.Shown += new System.EventHandler(this.InitialSetup);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private ListView listView1;
        private ColumnHeader columnNickname;
        private Label labelCountDown;

    }
}

