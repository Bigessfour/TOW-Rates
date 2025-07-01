using System.Windows.Forms;

namespace WileyBudgetManagement.Forms
{
    partial class DashboardForm
    {
        private Panel sidePanel;
        private Panel mainPanel;
        private Button btnWaterInput;
        private Button btnApartmentsInput;
        private Button btnTrashInput;
        private Button btnSummaryPage;

        private void InitializeComponent()
        {
            this.sidePanel = new System.Windows.Forms.Panel();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.btnWaterInput = new System.Windows.Forms.Button();
            this.btnApartmentsInput = new System.Windows.Forms.Button();
            this.btnTrashInput = new System.Windows.Forms.Button();
            this.btnSummaryPage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // sidePanel
            // 
            this.sidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidePanel.Width = 180;
            this.sidePanel.BackColor = System.Drawing.Color.LightSteelBlue;
            this.sidePanel.Controls.Add(this.btnWaterInput);
            this.sidePanel.Controls.Add(this.btnApartmentsInput);
            this.sidePanel.Controls.Add(this.btnTrashInput);
            this.sidePanel.Controls.Add(this.btnSummaryPage);
            // 
            // mainPanel
            // 
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            // 
            // btnWaterInput
            // 
            this.btnWaterInput.Text = "Water Input";
            this.btnWaterInput.Top = 30;
            this.btnWaterInput.Left = 20;
            this.btnWaterInput.Width = 140;
            this.btnWaterInput.Click += new System.EventHandler(this.btnWaterInput_Click);
            // 
            // btnApartmentsInput
            // 
            this.btnApartmentsInput.Text = "Apartments Input";
            this.btnApartmentsInput.Top = 80;
            this.btnApartmentsInput.Left = 20;
            this.btnApartmentsInput.Width = 140;
            this.btnApartmentsInput.Click += new System.EventHandler(this.btnApartmentsInput_Click);
            // 
            // btnTrashInput
            // 
            this.btnTrashInput.Text = "Trash Input";
            this.btnTrashInput.Top = 130;
            this.btnTrashInput.Left = 20;
            this.btnTrashInput.Width = 140;
            this.btnTrashInput.Click += new System.EventHandler(this.btnTrashInput_Click);
            // 
            // btnSummaryPage
            // 
            this.btnSummaryPage.Text = "Summary";
            this.btnSummaryPage.Top = 180;
            this.btnSummaryPage.Left = 20;
            this.btnSummaryPage.Width = 140;
            this.btnSummaryPage.Click += new System.EventHandler(this.btnSummaryPage_Click);
            // 
            // DashboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.sidePanel);
            this.Name = "DashboardForm";
            this.Text = "Wiley Budget Management Dashboard";
            this.ResumeLayout(false);
        }
    }
}
