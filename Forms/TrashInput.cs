using System;
using System.ComponentModel;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using WileyBudgetManagement.Models;

namespace WileyBudgetManagement.Forms
{
    public partial class TrashInput : Form
    {
        private SfDataGrid trashDataGrid = null!;
        private BindingList<SanitationDistrict> trashData = null!;

        public TrashInput()
        {
            InitializeComponent();
            InitializeTrashDataGrid();
            LoadTrashData();
        }

        private void InitializeTrashDataGrid()
        {
            trashDataGrid = new SfDataGrid()
            {
                Dock = DockStyle.Fill,
                AllowEditing = true,
                AllowResizingColumns = true,
                AutoGenerateColumns = false,
                ShowGroupDropArea = true
            };

            // Configure columns for Trash/Sanitation-specific data
            trashDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account #", Width = 100 });
            trashDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Service Description", Width = 200 });
            trashDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Section", HeaderText = "Section", Width = 100 });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CurrentFYBudget", HeaderText = "Current FY Budget", Width = 120, Format = "C" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "MonthlyInput", HeaderText = "Monthly Input", Width = 120, Format = "C" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "YearToDateSpending", HeaderText = "YTD Spending", Width = 120, Format = "C" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "PercentOfBudget", HeaderText = "% of Budget", Width = 100, Format = "P2" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "BudgetRemaining", HeaderText = "Budget Remaining", Width = 120, Format = "C" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "SeasonalAdjustment", HeaderText = "Seasonal Adjustment", Width = 130, Format = "C" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "RequiredRate", HeaderText = "Required Rate", Width = 120, Format = "C" });

            this.Controls.Add(trashDataGrid);
        }

        private void LoadTrashData()
        {
            // Initialize with sample trash/sanitation data - will be replaced with database integration
            trashData = new BindingList<SanitationDistrict>
            {
                new SanitationDistrict
                {
                    Account = "T001",
                    Label = "Residential Trash Collection",
                    Section = "Collections",
                    CurrentFYBudget = 320000,
                    MonthlyInput = 26666.67m,
                    YearToDateSpending = 160000,
                    PercentOfBudget = 0.50m,
                    BudgetRemaining = 160000,
                    SeasonalAdjustment = 5000,
                    RequiredRate = 22.50m
                },
                new SanitationDistrict
                {
                    Account = "T002",
                    Label = "Commercial Trash Collection",
                    Section = "Collections",
                    CurrentFYBudget = 180000,
                    MonthlyInput = 15000,
                    YearToDateSpending = 90000,
                    PercentOfBudget = 0.50m,
                    BudgetRemaining = 90000,
                    SeasonalAdjustment = 2500,
                    RequiredRate = 45.00m
                },
                new SanitationDistrict
                {
                    Account = "T003",
                    Label = "Recycling Program",
                    Section = "Recycling",
                    CurrentFYBudget = 95000,
                    MonthlyInput = 7916.67m,
                    YearToDateSpending = 47500,
                    PercentOfBudget = 0.50m,
                    BudgetRemaining = 47500,
                    SeasonalAdjustment = 1500,
                    RequiredRate = 8.75m
                },
                new SanitationDistrict
                {
                    Account = "T004",
                    Label = "Landfill Operations",
                    Section = "Operations",
                    CurrentFYBudget = 125000,
                    MonthlyInput = 10416.67m,
                    YearToDateSpending = 62500,
                    PercentOfBudget = 0.50m,
                    BudgetRemaining = 62500,
                    SeasonalAdjustment = 3000,
                    RequiredRate = 15.25m
                }
            };

            trashDataGrid.DataSource = trashData;
        }

        public BindingList<SanitationDistrict> GetTrashData()
        {
            return trashData;
        }
    }
}
