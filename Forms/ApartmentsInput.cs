using System;
using System.ComponentModel;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using WileyBudgetManagement.Models;

namespace WileyBudgetManagement.Forms
{
    public partial class ApartmentsInput : Form
    {
        private SfDataGrid apartmentsDataGrid = null!;
        private BindingList<SanitationDistrict> apartmentsData = null!;

        public ApartmentsInput()
        {
            InitializeComponent();
            InitializeApartmentsDataGrid();
            LoadApartmentsData();
        }

        private void InitializeApartmentsDataGrid()
        {
            apartmentsDataGrid = new SfDataGrid()
            {
                Dock = DockStyle.Fill,
                AllowEditing = true,
                AllowResizingColumns = true,
                AutoGenerateColumns = false,
                ShowGroupDropArea = true
            };

            // Configure columns for Apartment/Multi-family housing data
            apartmentsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account #", Width = 100 });
            apartmentsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Property Description", Width = 250 });
            apartmentsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Section", HeaderText = "Zone", Width = 100 });
            apartmentsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CurrentFYBudget", HeaderText = "Annual Revenue", Width = 120, Format = "C" });
            apartmentsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "MonthlyInput", HeaderText = "Monthly Revenue", Width = 120, Format = "C" });
            apartmentsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "YearToDateSpending", HeaderText = "YTD Collection", Width = 120, Format = "C" });
            apartmentsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "PercentOfBudget", HeaderText = "% Collected", Width = 100, Format = "P2" });
            apartmentsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "MonthlyUsage", HeaderText = "Units", Width = 80 });
            apartmentsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "RequiredRate", HeaderText = "Rate per Unit", Width = 120, Format = "C" });
            apartmentsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CustomerAffordabilityIndex", HeaderText = "Affordability Index", Width = 130, Format = "N2" });

            this.Controls.Add(apartmentsDataGrid);
        }

        private void LoadApartmentsData()
        {
            // Initialize with sample apartment/multi-family data - Account numbers will be added once provided
            apartmentsData = new BindingList<SanitationDistrict>
            {
                new SanitationDistrict
                {
                    Account = "APT001",
                    Label = "Meadowbrook Apartments (24 units)",
                    Section = "Zone A",
                    CurrentFYBudget = 43200,
                    MonthlyInput = 3600,
                    YearToDateSpending = 21600,
                    PercentOfBudget = 0.50m,
                    BudgetRemaining = 21600,
                    MonthlyUsage = 24,
                    RequiredRate = 150.00m,
                    CustomerAffordabilityIndex = 0.85m
                },
                new SanitationDistrict
                {
                    Account = "APT002",
                    Label = "Sunset Manor (36 units)",
                    Section = "Zone B",
                    CurrentFYBudget = 64800,
                    MonthlyInput = 5400,
                    YearToDateSpending = 32400,
                    PercentOfBudget = 0.50m,
                    BudgetRemaining = 32400,
                    MonthlyUsage = 36,
                    RequiredRate = 150.00m,
                    CustomerAffordabilityIndex = 0.78m
                },
                new SanitationDistrict
                {
                    Account = "APT003",
                    Label = "Riverside Condos (18 units)",
                    Section = "Zone A",
                    CurrentFYBudget = 32400,
                    MonthlyInput = 2700,
                    YearToDateSpending = 16200,
                    PercentOfBudget = 0.50m,
                    BudgetRemaining = 16200,
                    MonthlyUsage = 18,
                    RequiredRate = 150.00m,
                    CustomerAffordabilityIndex = 0.92m
                },
                new SanitationDistrict
                {
                    Account = "APT004",
                    Label = "Oak Street Townhomes (12 units)",
                    Section = "Zone C",
                    CurrentFYBudget = 21600,
                    MonthlyInput = 1800,
                    YearToDateSpending = 10800,
                    PercentOfBudget = 0.50m,
                    BudgetRemaining = 10800,
                    MonthlyUsage = 12,
                    RequiredRate = 150.00m,
                    CustomerAffordabilityIndex = 0.88m
                }
            };

            apartmentsDataGrid.DataSource = apartmentsData;
        }

        public BindingList<SanitationDistrict> GetApartmentsData()
        {
            return apartmentsData;
        }
    }
}
