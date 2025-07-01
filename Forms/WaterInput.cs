using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using Syncfusion.WinForms.DataGrid.Events;
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Database;
using WileyBudgetManagement.Forms;

namespace WileyBudgetManagement.Forms
{
    public partial class WaterInput : Form
    {
        private SfDataGrid waterDataGrid = null!;
        private BindingList<SanitationDistrict> waterData = null!;
        private readonly ISanitationRepository _repository;
        private readonly DatabaseManager _databaseManager;

        public WaterInput()
        {
            _databaseManager = new DatabaseManager();
            _repository = new SanitationRepository(_databaseManager);

            InitializeComponent();
            InitializeWaterDataGrid();
            LoadWaterDataAsync();
            SetupValidation();
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

        private void SetupValidation()
        {
            // Add Save button for validation
            var saveButton = new Button
            {
                Text = "Save & Validate",
                Size = new Size(120, 30),
                Location = new Point(10, 10),
                BackColor = Color.LightBlue
            };
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            // Move data grid down to make room for button
            waterDataGrid.Location = new Point(0, 50);
            waterDataGrid.Height = this.Height - 50;
            waterDataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            ValidateAllData();
            SaveWaterDataAsync();
        }

        private void ValidateAllData()
        {
            var validationErrors = new List<string>();
            var validationWarnings = new List<string>();

            for (int i = 0; i < waterData.Count; i++)
            {
                var district = waterData[i];
                var rowPrefix = $"Row {i + 1} ({district.Account}): ";

                // Account validation
                if (!ValidationHelper.ValidateAccount(district.Account, "W"))
                {
                    validationErrors.Add($"{rowPrefix}Account must start with 'W' followed by 3-9 alphanumeric characters");
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

                if (district.MonthlyUsage < 0)
                {
                    validationErrors.Add($"{rowPrefix}Monthly Usage cannot be negative");
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
            }

            // Display validation results
            if (validationErrors.Any() || validationWarnings.Any())
            {
                string message = "";

                if (validationErrors.Any())
                {
                    message += "ERRORS (must be fixed):\n";
                    message += string.Join("\n", validationErrors.Take(10)); // Limit to first 10 errors
                    if (validationErrors.Count > 10)
                        message += $"\n... and {validationErrors.Count - 10} more errors";
                    message += "\n\n";
                }

                if (validationWarnings.Any())
                {
                    message += "WARNINGS (recommended to fix):\n";
                    message += string.Join("\n", validationWarnings.Take(10)); // Limit to first 10 warnings
                    if (validationWarnings.Count > 10)
                        message += $"\n... and {validationWarnings.Count - 10} more warnings";
                }

                var icon = validationErrors.Any() ? MessageBoxIcon.Error : MessageBoxIcon.Warning;
                var title = validationErrors.Any() ? "Validation Errors" : "Validation Warnings";

                MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
            }
            else
            {
                MessageBox.Show("All validations passed successfully!", "Validation Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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

        private async void LoadWaterDataAsync()
        {
            try
            {
                // Load data from database or fallback to mock data
                waterData = await _repository.GetWaterDataAsync();
                waterDataGrid.DataSource = waterData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading water data: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Fallback to empty list
                waterData = new BindingList<SanitationDistrict>();
                waterDataGrid.DataSource = waterData;
            }
        }

        public BindingList<SanitationDistrict> GetWaterData()
        {
            return waterData;
        }

        public async void SaveWaterDataAsync()
        {
            try
            {
                bool success = await _repository.SaveAllAsync(waterData, "Water");
                if (success)
                {
                    MessageBox.Show("Water data saved successfully!", "Save Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to save water data.", "Save Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving water data: {ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
