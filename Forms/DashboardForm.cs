using System;

using System.Windows.Forms;
using WileyBudgetManagement.Models;

namespace WileyBudgetManagement.Forms
{
    public partial class DashboardForm : Form
    {
        public DashboardForm()
        {
            InitializeComponent();
        }

        private void btnWaterInput_Click(object sender, EventArgs e)
        {
            ShowFormInMainPanel(new WaterInput());
        }

        private void btnApartmentsInput_Click(object sender, EventArgs e)
        {
            ShowFormInMainPanel(new ApartmentsInput());
        }

        private void btnTrashInput_Click(object sender, EventArgs e)
        {
            ShowFormInMainPanel(new TrashInput());
        }

        private void btnSummaryPage_Click(object sender, EventArgs e)
        {
            ShowFormInMainPanel(new SummaryPage());
        }

        private void ShowFormInMainPanel(Form form)
        {
            // Remove previous controls
            mainPanel.Controls.Clear();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(form);
            form.Show();
        }
    }
}
