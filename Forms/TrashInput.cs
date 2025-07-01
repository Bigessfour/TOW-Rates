using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Forms;

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
            SetupValidation();
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

        private void SetupValidation()
        {
            // Add Save button for validation
            var saveButton = new Button
            {
                Text = "Save & Validate",
                Size = new Size(120, 30),
                Location = new Point(10, 10),
                BackColor = Color.LightGreen
            };
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            // Move data grid down to make room for button
            trashDataGrid.Location = new Point(0, 50);
            trashDataGrid.Height = this.Height - 50;
            trashDataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            ValidateAllData();
        }

        private void ValidateAllData()
        {
            var validationErrors = new List<string>();
            var validationWarnings = new List<string>();

            for (int i = 0; i < trashData.Count; i++)
            {
                var district = trashData[i];
                var rowPrefix = $"Row {i + 1} ({district.Account}): ";

                // Account validation
                if (!ValidationHelper.ValidateAccount(district.Account, "T"))
                {
                    validationErrors.Add($"{rowPrefix}Account must start with 'T' followed by 3-9 alphanumeric characters");
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
                    validationWarnings.Add($"{rowPrefix}YTD Spending exceeds annual budget");
                }

                if (!ValidationHelper.ValidatePercentage(district.PercentOfBudget))
                {
                    validationErrors.Add($"{rowPrefix}Percent of Budget must be between 0.0 and 1.0");
                }

                if (!ValidationHelper.ValidateRate(district.RequiredRate))
                {
                    validationErrors.Add($"{rowPrefix}Required Rate must be between $0 and $10,000");
                }

                // Trash-specific validations
                if (district.SeasonalAdjustment < 0)
                {
                    validationErrors.Add($"{rowPrefix}Seasonal Adjustment cannot be negative");
                }

                if (district.SeasonalAdjustment > district.CurrentFYBudget * 0.5m)
                {
                    validationWarnings.Add($"{rowPrefix}Seasonal Adjustment exceeds 50% of annual budget");
                }

                // Business rule validations
                decimal calculatedRemaining = district.CurrentFYBudget - district.YearToDateSpending;
                if (Math.Abs(district.BudgetRemaining - calculatedRemaining) > 0.01m)
                {
                    validationWarnings.Add($"{rowPrefix}Budget Remaining should be {calculatedRemaining:C} (Budget - YTD)");
                }

                if (district.CurrentFYBudget > 0)
                {
                    decimal calculatedPercent = district.YearToDateSpending / district.CurrentFYBudget;
                    if (Math.Abs(district.PercentOfBudget - calculatedPercent) > 0.01m)
                    {
                        validationWarnings.Add($"{rowPrefix}Percent of Budget should be {calculatedPercent:P2} (YTD/Budget)");
                    }
                }

                // Check if monthly input aligns with annual budget
                if (district.MonthlyInput > 0 && district.CurrentFYBudget > 0)
                {
                    decimal annualFromMonthly = district.MonthlyInput * 12;
                    decimal variance = Math.Abs(annualFromMonthly - district.CurrentFYBudget) / district.CurrentFYBudget;
                    if (variance > 0.1m)
                    {
                        validationWarnings.Add($"{rowPrefix}Monthly Input * 12 ({annualFromMonthly:C}) differs significantly from Annual Budget");
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
                var title = validationErrors.Any() ? "Trash Data Validation Errors" : "Trash Data Validation Warnings";

                MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
            }
            else
            {
                MessageBox.Show("All trash data validations passed successfully!", "Validation Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public BindingList<SanitationDistrict> GetTrashData()
        {
            return trashData;
        }
    }
}
