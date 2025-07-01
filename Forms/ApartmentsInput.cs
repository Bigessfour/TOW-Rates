using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Forms;

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
            SetupValidation();
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

        private void SetupValidation()
        {
            // Add Save button for validation
            var saveButton = new Button
            {
                Text = "Save & Validate",
                Size = new Size(120, 30),
                Location = new Point(10, 10),
                BackColor = Color.LightCoral
            };
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            // Move data grid down to make room for button
            apartmentsDataGrid.Location = new Point(0, 50);
            apartmentsDataGrid.Height = this.Height - 50;
            apartmentsDataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            ValidateAllData();
        }

        private void ValidateAllData()
        {
            var validationErrors = new List<string>();
            var validationWarnings = new List<string>();

            for (int i = 0; i < apartmentsData.Count; i++)
            {
                var district = apartmentsData[i];
                var rowPrefix = $"Row {i + 1} ({district.Account}): ";

                // Account validation
                if (!ValidationHelper.ValidateAccount(district.Account, "APT"))
                {
                    validationErrors.Add($"{rowPrefix}Account must start with 'APT' followed by alphanumeric characters");
                }

                // Label validation
                if (!ValidationHelper.ValidateLabel(district.Label))
                {
                    validationErrors.Add($"{rowPrefix}Label must be 3-100 characters with valid characters only");
                }

                // Section validation
                if (!ValidationHelper.ValidateSection(district.Section))
                {
                    validationErrors.Add($"{rowPrefix}Section must be 2-50 alphanumeric characters");
                }

                // Budget validations
                if (!ValidationHelper.ValidateBudgetAmount(district.CurrentFYBudget))
                {
                    validationErrors.Add($"{rowPrefix}Current FY Budget must be between $0 and $10,000,000");
                }

                if (district.MonthlyInput < 0)
                {
                    validationErrors.Add($"{rowPrefix}Monthly Input cannot be negative");
                }

                if (district.YearToDateSpending < 0)
                {
                    validationErrors.Add($"{rowPrefix}Year to Date Spending cannot be negative");
                }

                if (district.YearToDateSpending > district.CurrentFYBudget)
                {
                    validationWarnings.Add($"{rowPrefix}YTD Collection exceeds annual revenue budget");
                }

                if (!ValidationHelper.ValidatePercentage(district.PercentOfBudget))
                {
                    validationErrors.Add($"{rowPrefix}Percent Collected must be between 0.0 and 1.0");
                }

                if (!ValidationHelper.ValidateRate(district.RequiredRate))
                {
                    validationErrors.Add($"{rowPrefix}Rate per Unit must be between $0 and $10,000");
                }

                // Apartments-specific validations
                if (district.MonthlyUsage <= 0)
                {
                    validationErrors.Add($"{rowPrefix}Number of Units must be greater than 0");
                }

                if (district.MonthlyUsage > 1000)
                {
                    validationWarnings.Add($"{rowPrefix}Number of Units seems unusually high (>1000)");
                }

                if (district.CustomerAffordabilityIndex > 0 && !ValidationHelper.ValidateAffordabilityIndex(district.CustomerAffordabilityIndex))
                {
                    validationErrors.Add($"{rowPrefix}Customer Affordability Index must be between 0.1 and 2.0");
                }

                // Business rule validations
                decimal calculatedRemaining = district.CurrentFYBudget - district.YearToDateSpending;
                if (Math.Abs(district.BudgetRemaining - calculatedRemaining) > 0.01m)
                {
                    validationWarnings.Add($"{rowPrefix}Budget Remaining should be {calculatedRemaining:C} (Revenue - YTD Collection)");
                }

                if (district.CurrentFYBudget > 0)
                {
                    decimal calculatedPercent = district.YearToDateSpending / district.CurrentFYBudget;
                    if (Math.Abs(district.PercentOfBudget - calculatedPercent) > 0.01m)
                    {
                        validationWarnings.Add($"{rowPrefix}Percent Collected should be {calculatedPercent:P2} (YTD/Revenue)");
                    }
                }

                // Check rate per unit calculation
                if (district.MonthlyUsage > 0 && district.MonthlyInput > 0)
                {
                    decimal calculatedRate = district.MonthlyInput / district.MonthlyUsage;
                    if (Math.Abs(district.RequiredRate - calculatedRate) > 0.01m)
                    {
                        validationWarnings.Add($"{rowPrefix}Rate per Unit should be {calculatedRate:C} (Monthly Revenue / Units)");
                    }
                }

                // Check monthly vs annual consistency
                if (district.MonthlyInput > 0 && district.CurrentFYBudget > 0)
                {
                    decimal annualFromMonthly = district.MonthlyInput * 12;
                    decimal variance = Math.Abs(annualFromMonthly - district.CurrentFYBudget) / district.CurrentFYBudget;
                    if (variance > 0.1m)
                    {
                        validationWarnings.Add($"{rowPrefix}Monthly Revenue * 12 ({annualFromMonthly:C}) differs from Annual Revenue");
                    }
                }
            }

            // Display validation results
            if (validationErrors.Any() || validationWarnings.Any())
            {
                string message = "";

                if (validationErrors.Any())
                {
                    message += "ERRORS (must be fixed):\n";
                    message += string.Join("\n", validationErrors.Take(10));
                    if (validationErrors.Count > 10)
                        message += $"\n... and {validationErrors.Count - 10} more errors";
                    message += "\n\n";
                }

                if (validationWarnings.Any())
                {
                    message += "WARNINGS (recommended to fix):\n";
                    message += string.Join("\n", validationWarnings.Take(10));
                    if (validationWarnings.Count > 10)
                        message += $"\n... and {validationWarnings.Count - 10} more warnings";
                }

                var icon = validationErrors.Any() ? MessageBoxIcon.Error : MessageBoxIcon.Warning;
                var title = validationErrors.Any() ? "Apartments Data Validation Errors" : "Apartments Data Validation Warnings";

                MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
            }
            else
            {
                MessageBox.Show("All apartments data validations passed successfully!", "Validation Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public BindingList<SanitationDistrict> GetApartmentsData()
        {
            return apartmentsData;
        }
    }
}
