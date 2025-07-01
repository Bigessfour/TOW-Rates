using System;
using System.Windows.Forms;

namespace WileyBudgetManagement.Forms
{
    public partial class SummaryPage : Form
    {
        public SummaryPage()
        {
            InitializeComponent();
            // Bind mock data to DataGridView
            dataGridViewSummary.DataSource = MockSanitationData.GetSampleData();

            // Example: Bind a simple LiveCharts2 column series for CurrentFYBudget
            var data = MockSanitationData.GetSampleData();
            chartSummary.Series = new LiveChartsCore.ISeries[]
            {
                new LiveChartsCore.SkiaSharpView.ColumnSeries<decimal>
                {
                    Values = data.ConvertAll(d => d.CurrentFYBudget),
                    Name = "Current FY Budget"
                }
            };
            chartSummary.XAxes = new[]
            {
                new LiveChartsCore.SkiaSharpView.WinForms.Axis
                {
                    Labels = data.ConvertAll(d => d.Label)
                }
            };
        }
    }
}
