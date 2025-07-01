using System;
using System.ComponentModel;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using WileyBudgetManagement.Models;

namespace WileyBudgetManagement.Forms
{
    public partial class WaterInput : Form
    {
        private SfDataGrid waterDataGrid = null!;
        private BindingList<SanitationDistrict> waterData = null!;

        public WaterInput()
        {
            InitializeComponent();
            InitializeWaterDataGrid();
            LoadWaterData();
        }

        private void InitializeWaterDataGrid()
        {
            waterDataGrid = new SfDataGrid()
            {
                Dock = DockStyle.Fill,
                AllowEditing = true,
                AllowResizingColumns = true,
                AutoGenerateColumns = false,
                ShowGroupDropArea = true
            };

            // Configure columns for Water-specific data
            waterDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account #", Width = 100 });
            waterDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 200 });
            waterDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Section", HeaderText = "Section", Width = 100 });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CurrentFYBudget", HeaderText = "Current FY Budget", Width = 120, Format = "C" });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "MonthlyInput", HeaderText = "Monthly Input", Width = 120, Format = "C" });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "YearToDateSpending", HeaderText = "YTD Spending", Width = 120, Format = "C" });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "PercentOfBudget", HeaderText = "% of Budget", Width = 100, Format = "P2" });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "BudgetRemaining", HeaderText = "Budget Remaining", Width = 120, Format = "C" });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "MonthlyUsage", HeaderText = "Monthly Usage", Width = 120 });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "RequiredRate", HeaderText = "Required Rate", Width = 120, Format = "C" });

            this.Controls.Add(waterDataGrid);
        }

        private void LoadWaterData()
        {
            // Initialize with sample water data - will be replaced with database integration
            waterData = new BindingList<SanitationDistrict>
            {
                new SanitationDistrict
                {
                    Account = "W001",
                    Label = "Water Treatment Plant Operations",
                    Section = "Operations",
                    CurrentFYBudget = 250000,
                    MonthlyInput = 20833.33m,
                    YearToDateSpending = 125000,
                    PercentOfBudget = 0.50m,
                    BudgetRemaining = 125000,
                    MonthlyUsage = 1500000,
                    RequiredRate = 0.167m
                },
                new SanitationDistrict
                {
                    Account = "W002",
                    Label = "Water Distribution Maintenance",
                    Section = "Maintenance",
                    CurrentFYBudget = 180000,
                    MonthlyInput = 15000,
                    YearToDateSpending = 90000,
                    PercentOfBudget = 0.50m,
                    BudgetRemaining = 90000,
                    MonthlyUsage = 1200000,
                    RequiredRate = 0.125m
                },
                new SanitationDistrict
                {
                    Account = "W003",
                    Label = "Water Quality Testing",
                    Section = "Quality",
                    CurrentFYBudget = 45000,
                    MonthlyInput = 3750,
                    YearToDateSpending = 22500,
                    PercentOfBudget = 0.50m,
                    BudgetRemaining = 22500,
                    MonthlyUsage = 0,
                    RequiredRate = 0.0375m
                }
            };

            waterDataGrid.DataSource = waterData;
        }

        public BindingList<SanitationDistrict> GetWaterData()
        {
            return waterData;
        }
    }
}
