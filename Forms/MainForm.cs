using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WileyBudgetManagement.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;

namespace WileyBudgetManagement.Forms
{
    public partial class MainForm : Form
    {
        private DataGridView? sanitationGrid;
        private CartesianChart? budgetChart;
        private BindingSource? sanitationBinding;


        public MainForm()
        {
            InitializeSanitationGrid();
            InitializeBudgetChart();
        }

        private void InitializeSanitationGrid()
        {
            sanitationGrid = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 250,
                AutoGenerateColumns = true,
                DataSource = null
            };
            sanitationBinding = new BindingSource();
            sanitationBinding.DataSource = MockSanitationData.GetSampleData();
            sanitationGrid.DataSource = sanitationBinding;
            Controls.Add(sanitationGrid);
        }

        private void InitializeBudgetChart()
        {
            budgetChart = new CartesianChart
            {
                Dock = DockStyle.Fill
            };
            // Example: Line chart for % of Budget over 12 months (mock data)
            budgetChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new double[] { 10, 20, 35, 50, 60, 70, 80, 85, 90, 95, 98, 100 },
                    Name = "% of Budget"
                }
            };
            budgetChart.XAxes = new[]
            {
                new Axis { Labels = new List<string>{"Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"} }
            };
            Controls.Add(budgetChart);
        }
    }
}
