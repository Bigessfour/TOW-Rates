using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using WileyBudgetManagement.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Syncfusion.WinForms.DataGrid;
using SkiaSharp;

namespace WileyBudgetManagement.Forms
{
    public partial class MainForm : Form
    {
        private SfDataGrid? sanitationGrid;
        private CartesianChart? budgetChart;
        private BindingList<SanitationDistrict>? sanitationData;

        public MainForm()
        {
            InitializeSanitationGrid();
            InitializeBudgetChart();
            LoadData();
        }

        private void InitializeSanitationGrid()
        {
            sanitationGrid = new SfDataGrid
            {
                Dock = DockStyle.Top,
                Height = 300,
                AutoGenerateColumns = false,
                AllowEditing = true,
                AllowResizingColumns = true,
                ShowGroupDropArea = true
            };

            // Configure key columns for overview
            sanitationGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account", Width = 100 });
            sanitationGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 200 });
            sanitationGrid.Columns.Add(new GridTextColumn() { MappingName = "Section", HeaderText = "Section", Width = 100 });
            sanitationGrid.Columns.Add(new GridNumericColumn() { MappingName = "CurrentFYBudget", HeaderText = "FY Budget", Width = 120, Format = "C" });
            sanitationGrid.Columns.Add(new GridNumericColumn() { MappingName = "YearToDateSpending", HeaderText = "YTD Spending", Width = 120, Format = "C" });
            sanitationGrid.Columns.Add(new GridNumericColumn() { MappingName = "PercentOfBudget", HeaderText = "% of Budget", Width = 100, Format = "P2" });
            sanitationGrid.Columns.Add(new GridNumericColumn() { MappingName = "BudgetRemaining", HeaderText = "Remaining", Width = 120, Format = "C" });

            Controls.Add(sanitationGrid);
        }

        private void InitializeBudgetChart()
        {
            budgetChart = new CartesianChart
            {
                Dock = DockStyle.Fill
            };

            // LiveCharts 2 example: Budget vs Actual spending comparison
            budgetChart.Series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = new double[] { 250000, 180000, 45000, 320000, 95000, 125000 },
                    Name = "Budget",
                    Fill = new SolidColorPaint(SKColors.SteelBlue)
                },
                new ColumnSeries<double>
                {
                    Values = new double[] { 125000, 90000, 22500, 160000, 47500, 62500 },
                    Name = "YTD Spending",
                    Fill = new SolidColorPaint(SKColors.Orange)
                }
            };

            budgetChart.XAxes = new[]
            {
                new Axis { Labels = new List<string>{"Water Treatment", "Water Dist.", "Water Quality", "Trash Collection", "Recycling", "Landfill"} }
            };

            budgetChart.YAxes = new[]
            {
                new Axis { Name = "Amount ($)" }
            };

            Controls.Add(budgetChart);
        }

        private void LoadData()
        {
            // Load comprehensive data from all departments
            sanitationData = new BindingList<SanitationDistrict>(MockSanitationData.GetSampleData());

            if (sanitationGrid != null)
            {
                sanitationGrid.DataSource = sanitationData;
            }
        }

        public void RefreshData()
        {
            LoadData();
        }
    }
}
