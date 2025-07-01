using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;

namespace WileyBudgetManagement.Forms
{
    partial class SummaryPage
    {
        private DataGridView dataGridViewSummary;
        private CartesianChart chartSummary;

        private void InitializeComponent()
        {
            this.dataGridViewSummary = new System.Windows.Forms.DataGridView();
            this.chartSummary = new LiveChartsCore.SkiaSharpView.WinForms.CartesianChart();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSummary)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewSummary
            // 
            this.dataGridViewSummary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSummary.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewSummary.Name = "dataGridViewSummary";
            this.dataGridViewSummary.Size = new System.Drawing.Size(600, 250);
            this.dataGridViewSummary.TabIndex = 0;
            // 
            // chartSummary
            // 
            this.chartSummary.Location = new System.Drawing.Point(12, 280);
            this.chartSummary.Name = "chartSummary";
            this.chartSummary.Size = new System.Drawing.Size(600, 250);
            this.chartSummary.TabIndex = 1;
            // 
            // SummaryPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 561);
            this.Controls.Add(this.dataGridViewSummary);
            this.Controls.Add(this.chartSummary);
            this.Name = "SummaryPage";
            this.Text = "SummaryPage";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSummary)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
