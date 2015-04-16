using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

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
            chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint1 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 1D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(1D, 1.5D);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "22:03:2000 12:00",
            "tiagoaf",
            "buy",
            "100",
            "20",
            "80",
            "open"}, -1);
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnNickname = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelCountDown = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnUser = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTotal = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSatisfied = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnRemaining = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnNickname});
            this.listView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(121, 365);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemActivate += new System.EventHandler(this.ListView1_ItemActivate);
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
            this.labelCountDown.Location = new System.Drawing.Point(370, 9);
            this.labelCountDown.Name = "labelCountDown";
            this.labelCountDown.Size = new System.Drawing.Size(22, 16);
            this.labelCountDown.TabIndex = 1;
            this.labelCountDown.Text = "59";
            // 
            // chart1
            // 
            chartArea1.AxisX.LabelStyle.Enabled = false;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Column;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(135, 36);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Red;
            series1.IsVisibleInLegend = false;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series1.Points.Add(dataPoint1);
            series1.Points.Add(dataPoint2);
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(544, 141);
            this.chart1.TabIndex = 20;
            this.chart1.Text = "Share Price";
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnDate,
            this.columnUser,
            this.columnType,
            this.columnTotal,
            this.columnSatisfied,
            this.columnRemaining,
            this.columnStatus});
            this.listView2.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.listView2.Location = new System.Drawing.Point(135, 186);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(544, 167);
            this.listView2.TabIndex = 21;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // columnDate
            // 
            this.columnDate.Text = "Date";
            this.columnDate.Width = 95;
            // 
            // columnUser
            // 
            this.columnUser.Text = "User";
            this.columnUser.Width = 82;
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            // 
            // columnTotal
            // 
            this.columnTotal.Text = "Total";
            // 
            // columnSatisfied
            // 
            this.columnSatisfied.Text = "Satisfied";
            // 
            // columnRemaining
            // 
            this.columnRemaining.Text = "Remaining";
            this.columnRemaining.Width = 63;
            // 
            // columnStatus
            // 
            this.columnStatus.Text = "Status";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(510, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 22;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainWindowServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 365);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.labelCountDown);
            this.Controls.Add(this.listView1);
            this.Name = "MainWindowServer";
            this.Text = "Diginote Exchange System - Server";
            this.Shown += new System.EventHandler(this.InitialSetup);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private ListView listView1;
        private ColumnHeader columnNickname;
        private Label labelCountDown;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private Series series1;
        private ListView listView2;
        private ColumnHeader columnDate;
        private ColumnHeader columnUser;
        private ColumnHeader columnType;
        private ColumnHeader columnTotal;
        private ColumnHeader columnSatisfied;
        private ColumnHeader columnRemaining;
        private ColumnHeader columnStatus;
        private Legend legend1;
        private ChartArea chartArea1;
        private Button button1;
    }
}

