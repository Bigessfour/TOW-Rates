using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using Syncfusion.WinForms.DataGrid.Events;
using Syncfusion.Data;
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Database;

namespace WileyBudgetManagement.Forms
{
    public partial class SanitationDistrictInput : Form
    {
        private SfDataGrid sanitationDataGrid = null!;
        private BindingList<SanitationDistrict> sanitationData = null!;
        private readonly ISanitationRepository _repository;
        private readonly DatabaseManager _databaseManager;
        private Button saveButton = null!;
        private Button addRowButton = null!;
        private Button deleteRowButton = null!;
        private ComboBox sectionFilterCombo = null!;
        private Label statusLabel = null!;

        public SanitationDistrictInput()
        {
            _databaseManager = new DatabaseManager();
            _repository = new SanitationRepository(_databaseManager);

            InitializeComponent();
            InitializeControls();
            InitializeSanitationDataGrid();
            LoadSanitationDataAsync();
            SetupValidation();
        }

        private void InitializeControls()
        {
            this.Text = "Sanitation District - Revenue & Expenses";
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create toolbar panel
            var toolbarPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top,
                BackColor = Color.LightGray
            };

            // Save button
            saveButton = new Button
            {
                Text = "Save & Validate",
                Size = new Size(120, 30),
                Location = new Point(10, 10),
                BackColor = Color.LightBlue
            };
            saveButton.Click += SaveButton_Click;

            // Add Row button
            addRowButton = new Button
            {
                Text = "Add Row",
                Size = new Size(80, 30),
                Location = new Point(140, 10),
                BackColor = Color.LightGreen
            };
            addRowButton.Click += AddRowButton_Click;

            // Delete Row button
            deleteRowButton = new Button
            {
                Text = "Delete Row",
                Size = new Size(90, 30),
                Location = new Point(230, 10),
                BackColor = Color.LightCoral
            };
            deleteRowButton.Click += DeleteRowButton_Click;

            // Section filter
            var filterLabel = new Label
            {
                Text = "Filter by Section:",
                Location = new Point(350, 15),
                Size = new Size(100, 20)
            };

            sectionFilterCombo = new ComboBox
            {
                Size = new Size(150, 25),
                Location = new Point(450, 12),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            sectionFilterCombo.Items.AddRange(new[] { "All", "Revenue", "Operating", "Admin" });
            sectionFilterCombo.SelectedIndex = 0;
            sectionFilterCombo.SelectedIndexChanged += SectionFilterCombo_SelectedIndexChanged;

            // Status label
            statusLabel = new Label
            {
                Text = "Ready",
                Location = new Point(620, 15),
                Size = new Size(300, 20),
                ForeColor = Color.DarkGreen
            };

            toolbarPanel.Controls.AddRange(new Control[] {
                saveButton, addRowButton, deleteRowButton, filterLabel, sectionFilterCombo, statusLabel
            });

            this.Controls.Add(toolbarPanel);
        }

        private void InitializeSanitationDataGrid()
        {
            sanitationDataGrid = new SfDataGrid()
            {
                Dock = DockStyle.Fill,
                AllowEditing = true,
                AllowResizingColumns = true,
                AutoGenerateColumns = false,
                ShowGroupDropArea = true,
                SelectionMode = Syncfusion.WinForms.DataGrid.Enums.GridSelectionMode.Single
            };

            // Configure columns based on the Rate Study Methodology
            sanitationDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account #", Width = 80 });
            sanitationDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 250 });
            sanitationDataGrid.Columns.Add(new GridComboBoxColumn()
            {
                MappingName = "Section",
                HeaderText = "Section",
                Width = 100,
                DataSource = new[] { "Revenue", "Operating", "Admin" }
            });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CurrentFYBudget", HeaderText = "Current FY Budget", Width = 120, Format = "C" });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "SeasonalAdjustment", HeaderText = "Seasonal Adj", Width = 100, Format = "C" });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "MonthlyInput", HeaderText = "Monthly Input", Width = 110, Format = "C" });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "SeasonalRevenueFactor", HeaderText = "Seasonal Factor", Width = 100, Format = "N2" });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "YearToDateSpending", HeaderText = "YTD Spending", Width = 110, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightGray } });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "PercentOfBudget", HeaderText = "% of Budget", Width = 90, Format = "P2", AllowEditing = false, CellStyle = { BackColor = Color.LightGray } });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "BudgetRemaining", HeaderText = "Budget Remaining", Width = 120, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightGray } });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "GoalAdjustment", HeaderText = "Goal Adjustment", Width = 110, Format = "C" });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "ReserveTarget", HeaderText = "Reserve Target", Width = 110, Format = "C" });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "TimeOfUseFactor", HeaderText = "TOU Factor", Width = 90, Format = "N2" });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CustomerAffordabilityIndex", HeaderText = "Affordability", Width = 100, Format = "N2" });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "PercentAllocation", HeaderText = "% Allocation", Width = 100, Format = "P2" });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Scenario1", HeaderText = "Scenario 1", Width = 90, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightYellow } });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Scenario2", HeaderText = "Scenario 2", Width = 90, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightYellow } });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Scenario3", HeaderText = "Scenario 3", Width = 90, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightYellow } });
            sanitationDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "RequiredRate", HeaderText = "Required Rate", Width = 100, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightBlue } });

            // Handle cell value changes for real-time calculations
            sanitationDataGrid.CurrentCellEndEdit += SanitationDataGrid_CurrentCellEndEdit;

            this.Controls.Add(sanitationDataGrid);
        }

        private void SanitationDataGrid_CurrentCellEndEdit(object? sender, CurrentCellEndEditEventArgs e)
        {
            if (sanitationData != null && sanitationDataGrid.CurrentCell != null)
            {
                var rowIndex = sanitationDataGrid.CurrentCell.RowIndex;
                if (rowIndex >= 0 && rowIndex < sanitationData.Count)
                {
                    var district = sanitationData[rowIndex];
                    CalculateFields(district);
                    RefreshGrid();
                }
            }
        }

        private void CalculateFields(SanitationDistrict district)
        {
            try
            {
                // Calculate YTD Spending with seasonal considerations
                CalculateYearToDateSpending(district);

                // Calculate Budget Remaining
                district.BudgetRemaining = district.CurrentFYBudget - district.YearToDateSpending;

                // Calculate Percent of Budget
                if (district.CurrentFYBudget > 0)
                {
                    district.PercentOfBudget = district.YearToDateSpending / district.CurrentFYBudget;
                }
                else
                {
                    district.PercentOfBudget = 0;
                }

                // Calculate Total for summary purposes
                district.Total = district.CurrentFYBudget + district.SeasonalAdjustment + district.GoalAdjustment;

                // Calculate Scenarios (based on Rate Study Methodology)
                CalculateScenarios(district);

                // Calculate Required Rate
                CalculateRequiredRate(district);

                // Calculate Quarterly Summary
                district.QuarterlySummary = (district.MonthlyInput * 3) + (district.SeasonalAdjustment / 4);
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Calculation error: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
            }
        }

        private void CalculateYearToDateSpending(SanitationDistrict district)
        {
            // Get current month for YTD calculation
            int currentMonth = DateTime.Now.Month;

            // Base YTD calculation
            decimal baseYTD = district.MonthlyInput * currentMonth;

            // Apply seasonal adjustments based on section type
            decimal seasonalImpact = 0;

            if (district.Section == "Revenue")
            {
                // Revenue items may have seasonal variations
                seasonalImpact = district.SeasonalAdjustment * district.SeasonalRevenueFactor;
            }
            else
            {
                // Operating and Admin expenses may have different seasonal patterns
                seasonalImpact = district.SeasonalAdjustment;
            }

            // Calculate final YTD with seasonal considerations
            district.YearToDateSpending = baseYTD + seasonalImpact;

            // Ensure YTD doesn't exceed reasonable bounds
            if (district.YearToDateSpending < 0)
            {
                district.YearToDateSpending = 0;
            }
            else if (district.YearToDateSpending > district.CurrentFYBudget * 1.2m)
            {
                // Cap at 120% of budget to prevent unrealistic values
                district.YearToDateSpending = district.CurrentFYBudget * 1.2m;
            }
        }

        private void CalculateScenarios(SanitationDistrict district)
        {
            // Base monthly amount for all scenarios
            decimal baseMonthly = district.MonthlyInput;

            // Scenario 1: New Trash Truck ($350,000, 12-year lifespan, 5% interest)
            // Annual Payment: $350,000 / 12 years + interest = $32,083.34/year
            // Monthly Impact: $2,673.61
            decimal trashTruckMonthlyImpact = 2673.61m;

            // Scenario 2: Reserve Fund Building ($50,000 over 5 years, no interest)
            // Annual: $10,000, Monthly: $833.33
            decimal reserveMonthlyImpact = 833.33m;

            // Scenario 3: EPA Grant Repayment ($100,000, 5 years, 3% interest)
            // Using PMT calculation: $100,000 at 3% for 5 years = $21,907.50/year
            // Monthly Payment: $1,825.63
            decimal grantMonthlyImpact = 1825.63m;

            // Apply scenario calculations based on section type
            switch (district.Section)
            {
                case "Revenue":
                    // Revenue items need to cover additional costs
                    district.Scenario1 = baseMonthly + (trashTruckMonthlyImpact * district.PercentAllocation);
                    district.Scenario2 = baseMonthly + (reserveMonthlyImpact * district.PercentAllocation);
                    district.Scenario3 = baseMonthly + (grantMonthlyImpact * district.PercentAllocation);
                    break;

                case "Operating":
                    // Operating expenses directly impacted
                    district.Scenario1 = baseMonthly + trashTruckMonthlyImpact + district.GoalAdjustment;
                    district.Scenario2 = baseMonthly + (district.ReserveTarget / 12) + reserveMonthlyImpact;
                    district.Scenario3 = baseMonthly + grantMonthlyImpact;
                    break;

                case "Admin":
                    // Administrative costs have proportional impact
                    decimal adminFactor = district.PercentAllocation > 0 ? district.PercentAllocation : 0.15m; // Default 15%
                    district.Scenario1 = baseMonthly + (trashTruckMonthlyImpact * adminFactor);
                    district.Scenario2 = baseMonthly + (reserveMonthlyImpact * adminFactor);
                    district.Scenario3 = baseMonthly + (grantMonthlyImpact * adminFactor);
                    break;

                default:
                    // Default case - no impact
                    district.Scenario1 = baseMonthly;
                    district.Scenario2 = baseMonthly;
                    district.Scenario3 = baseMonthly;
                    break;
            }

            // Apply time-of-use and affordability adjustments
            if (district.TimeOfUseFactor > 0)
            {
                district.Scenario1 *= district.TimeOfUseFactor;
                district.Scenario2 *= district.TimeOfUseFactor;
                district.Scenario3 *= district.TimeOfUseFactor;
            }

            if (district.CustomerAffordabilityIndex > 0)
            {
                district.Scenario1 *= district.CustomerAffordabilityIndex;
                district.Scenario2 *= district.CustomerAffordabilityIndex;
                district.Scenario3 *= district.CustomerAffordabilityIndex;
            }

            // Ensure scenarios are not negative
            district.Scenario1 = Math.Max(0, district.Scenario1);
            district.Scenario2 = Math.Max(0, district.Scenario2);
            district.Scenario3 = Math.Max(0, district.Scenario3);
        }

        private void CalculateRequiredRate(SanitationDistrict district)
        {
            try
            {
                decimal calculatedRate = 0;

                switch (district.Section)
                {
                    case "Revenue":
                        calculatedRate = CalculateRevenueRequiredRate(district);
                        break;

                    case "Operating":
                        calculatedRate = CalculateOperatingRequiredRate(district);
                        break;

                    case "Admin":
                        calculatedRate = CalculateAdminRequiredRate(district);
                        break;

                    default:
                        calculatedRate = 0;
                        break;
                }

                // Apply adjustment factors
                if (district.TimeOfUseFactor > 0 && district.TimeOfUseFactor != 1.0m)
                {
                    calculatedRate *= district.TimeOfUseFactor;
                }

                if (district.CustomerAffordabilityIndex > 0 && district.CustomerAffordabilityIndex != 1.0m)
                {
                    calculatedRate *= district.CustomerAffordabilityIndex;
                }

                // Set the calculated rate
                district.RequiredRate = Math.Max(0, calculatedRate);
            }
            catch (Exception)
            {
                // If calculation fails, set to 0
                district.RequiredRate = 0;
            }
        }

        private decimal CalculateRevenueRequiredRate(SanitationDistrict district)
        {
            // For revenue items, calculate rate needed to cover total expenses
            decimal totalExpenses = GetTotalExpenses();
            decimal totalRevenue = GetTotalRevenue();
            decimal customerBase = GetTotalCustomerBase();

            if (customerBase <= 0) return 0;

            // Calculate proportional share of this revenue item
            decimal revenueShare = totalRevenue > 0 ? district.CurrentFYBudget / totalRevenue : 0;

            // Required monthly rate per customer for this revenue item
            decimal monthlyRateNeeded = (totalExpenses * revenueShare) / customerBase / 12;

            return monthlyRateNeeded;
        }

        private decimal CalculateOperatingRequiredRate(SanitationDistrict district)
        {
            // For operating expenses, show cost per customer per month
            decimal customerBase = GetTotalCustomerBase();

            if (customerBase <= 0) return 0;

            // Annual cost divided by customers divided by 12 months
            return district.CurrentFYBudget / customerBase / 12;
        }

        private decimal CalculateAdminRequiredRate(SanitationDistrict district)
        {
            // Administrative costs are allocated proportionally
            decimal totalAdminCosts = sanitationData?.Where(d => d.Section == "Admin")
                                                   .Sum(d => d.CurrentFYBudget) ?? 0;
            decimal customerBase = GetTotalCustomerBase();

            if (customerBase <= 0 || totalAdminCosts <= 0) return 0;

            // This admin item's share of total admin costs
            decimal adminShare = district.CurrentFYBudget / totalAdminCosts;

            // Admin allocation (typically 15-25% of total costs)
            decimal adminAllocation = district.PercentAllocation > 0 ? district.PercentAllocation : 0.20m;

            return (totalAdminCosts * adminShare * adminAllocation) / customerBase / 12;
        }

        private decimal GetTotalExpenses()
        {
            return sanitationData?.Where(d => d.Section == "Operating" || d.Section == "Admin")
                                 .Sum(d => d.CurrentFYBudget) ?? 0;
        }

        private decimal GetTotalRevenue()
        {
            return sanitationData?.Where(d => d.Section == "Revenue")
                                 .Sum(d => d.CurrentFYBudget) ?? 0;
        }

        private decimal GetTotalCustomerBase()
        {
            // Estimate customer base - in real implementation this would come from customer data
            return 850; // Town of Wiley approximate customer base
        }

        private void RefreshGrid()
        {
            sanitationDataGrid.View?.Refresh();
        }

        private void SetupValidation()
        {
            // Additional setup for grid behavior
            sanitationDataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            ValidateAllData();
            SaveSanitationDataAsync();
        }

        private void AddRowButton_Click(object? sender, EventArgs e)
        {
            var newDistrict = new SanitationDistrict
            {
                Account = $"S{sanitationData.Count + 1:000}",
                Label = "New Item",
                Section = "Revenue",
                EntryDate = DateTime.Now,
                TimeOfUseFactor = 1.0m,
                CustomerAffordabilityIndex = 1.0m,
                SeasonalRevenueFactor = 1.0m
            };

            sanitationData.Add(newDistrict);
            statusLabel.Text = "New row added";
            statusLabel.ForeColor = Color.DarkGreen;
        }

        private void DeleteRowButton_Click(object? sender, EventArgs e)
        {
            if (sanitationDataGrid.SelectedIndex >= 0 && sanitationDataGrid.SelectedIndex < sanitationData.Count)
            {
                var result = MessageBox.Show("Are you sure you want to delete this row?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    sanitationData.RemoveAt(sanitationDataGrid.SelectedIndex);
                    statusLabel.Text = "Row deleted";
                    statusLabel.ForeColor = Color.DarkOrange;
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SectionFilterCombo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string selectedSection = sectionFilterCombo.SelectedItem?.ToString() ?? "All";

            if (selectedSection == "All")
            {
                sanitationDataGrid.View.Filter = null;
            }
            else
            {
                sanitationDataGrid.View.Filter = item =>
                {
                    var district = item as SanitationDistrict;
                    return district?.Section == selectedSection;
                };
            }

            sanitationDataGrid.View.RefreshFilter();
            statusLabel.Text = $"Filtered by: {selectedSection}";
            statusLabel.ForeColor = Color.Blue;
        }

        private void ValidateAllData()
        {
            var allErrors = new List<string>();
            var allWarnings = new List<string>();

            for (int i = 0; i < sanitationData.Count; i++)
            {
                var district = sanitationData[i];
                var rowPrefix = $"Row {i + 1} ({district.Account}): ";

                // Use comprehensive validation helper
                var fieldValidation = ValidationHelper.ValidateSanitationDistrict(district, "Sanitation");
                var businessValidation = ValidationHelper.ValidateBusinessRules(district);

                // Collect field validation errors
                foreach (var error in fieldValidation.Errors)
                {
                    allErrors.Add($"{rowPrefix}{error.Message}");
                }

                // Collect field validation warnings
                foreach (var warning in fieldValidation.Warnings)
                {
                    allWarnings.Add($"{rowPrefix}{warning.Message}");
                }

                // Collect business rule validation errors
                foreach (var error in businessValidation.Errors)
                {
                    allErrors.Add($"{rowPrefix}{error.Message}");
                }

                // Collect business rule validation warnings
                foreach (var warning in businessValidation.Warnings)
                {
                    allWarnings.Add($"{rowPrefix}{warning.Message}");
                }

                // Additional sanitation-specific validations
                ValidateSanitationSpecificRules(district, rowPrefix, allErrors, allWarnings);
            }

            // Global validations (across all rows)
            ValidateGlobalRules(allWarnings);

            // Display validation results
            DisplayValidationResults(allErrors, allWarnings);
        }

        private void ValidateSanitationSpecificRules(SanitationDistrict district, string rowPrefix, List<string> errors, List<string> warnings)
        {
            // Sanitation-specific account validation
            if (!district.Account.StartsWith("S") && !string.IsNullOrWhiteSpace(district.Account))
            {
                errors.Add($"{rowPrefix}Sanitation accounts should start with 'S' prefix");
            }

            // Section-specific validations
            var validSections = new[] { "Revenue", "Operating", "Admin" };
            if (!validSections.Contains(district.Section))
            {
                errors.Add($"{rowPrefix}Section must be one of: {string.Join(", ", validSections)}");
            }

            // Seasonal factor validation for sanitation
            if (district.SeasonalRevenueFactor < 0.5m || district.SeasonalRevenueFactor > 2.0m)
            {
                warnings.Add($"{rowPrefix}Seasonal Revenue Factor should typically be between 0.5 and 2.0");
            }

            // Sanitation-specific scenario validations
            if (district.Section == "Operating")
            {
                // Check if equipment replacement scenarios are reasonable
                if (district.Scenario1 > district.CurrentFYBudget * 2)
                {
                    warnings.Add($"{rowPrefix}Scenario 1 (Trash Truck) seems high compared to current budget");
                }

                if (district.Scenario2 > district.CurrentFYBudget * 1.5m)
                {
                    warnings.Add($"{rowPrefix}Scenario 2 (Reserve Fund) seems high compared to current budget");
                }
            }

            // Revenue allocation validation
            if (district.Section == "Revenue" && district.PercentAllocation > 0 && district.PercentAllocation > 1.0m)
            {
                errors.Add($"{rowPrefix}Percent Allocation cannot exceed 100% (1.0)");
            }

            // Check for unrealistic required rates
            if (district.RequiredRate > 1000m)
            {
                warnings.Add($"{rowPrefix}Required Rate ({district.RequiredRate:C}) seems unusually high");
            }
        }

        private void ValidateGlobalRules(List<string> warnings)
        {
            decimal totalRevenue = GetTotalRevenue();
            decimal totalExpenses = GetTotalExpenses();
            decimal revenueExpenseRatio = totalExpenses > 0 ? totalRevenue / totalExpenses : 0;

            // Revenue vs Expense balance validation
            if (totalExpenses > totalRevenue)
            {
                decimal deficit = totalExpenses - totalRevenue;
                warnings.Add($"BUDGET IMBALANCE: Total Expenses ({totalExpenses:C}) exceed Total Revenue ({totalRevenue:C}) by {deficit:C}");
            }

            // Check if revenue-expense ratio is within reasonable bounds
            if (revenueExpenseRatio < 0.95m && totalRevenue > 0)
            {
                warnings.Add($"Revenue Coverage Ratio ({revenueExpenseRatio:P1}) is low - consider rate increases");
            }
            else if (revenueExpenseRatio > 1.20m)
            {
                warnings.Add($"Revenue Coverage Ratio ({revenueExpenseRatio:P1}) is high - consider rate decreases or reserve funding");
            }

            // Check total budget size reasonableness
            decimal totalBudget = totalRevenue + totalExpenses;
            if (totalBudget < 50000m)
            {
                warnings.Add("Total budget seems low for a municipal sanitation district");
            }
            else if (totalBudget > 5000000m)
            {
                warnings.Add("Total budget seems high - please verify all amounts");
            }

            // Validate scenario planning totals
            decimal totalScenario1 = sanitationData.Sum(d => d.Scenario1);
            decimal totalScenario2 = sanitationData.Sum(d => d.Scenario2);
            decimal totalScenario3 = sanitationData.Sum(d => d.Scenario3);

            if (totalScenario1 > totalRevenue * 1.5m)
            {
                warnings.Add($"Total Scenario 1 impact ({totalScenario1:C}) may require significant rate increases");
            }
        }

        private void DisplayValidationResults(List<string> errors, List<string> warnings)
        {
            if (errors.Any() || warnings.Any())
            {
                string message = "";

                if (errors.Any())
                {
                    message += "ERRORS (must be fixed):\n";
                    message += string.Join("\n", errors.Take(10));
                    if (errors.Count > 10)
                        message += $"\n... and {errors.Count - 10} more errors";
                    message += "\n\n";
                }

                if (warnings.Any())
                {
                    message += "WARNINGS (recommended to fix):\n";
                    message += string.Join("\n", warnings.Take(10));
                    if (warnings.Count > 10)
                        message += $"\n... and {warnings.Count - 10} more warnings";
                }

                var icon = errors.Any() ? MessageBoxIcon.Error : MessageBoxIcon.Warning;
                var title = errors.Any() ? "Validation Errors" : "Validation Warnings";

                MessageBox.Show(message, title, MessageBoxButtons.OK, icon);

                statusLabel.Text = errors.Any() ? "Validation failed" : "Validation warnings";
                statusLabel.ForeColor = errors.Any() ? Color.Red : Color.Orange;
            }
            else
            {
                MessageBox.Show("All validations passed successfully!", "Validation Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                statusLabel.Text = "Validation passed";
                statusLabel.ForeColor = Color.DarkGreen;
            }
        }

        private void LoadSanitationDataAsync()
        {
            try
            {
                // Initialize with predefined sanitation district data from Rate Study Methodology
                sanitationData = GetDefaultSanitationData();
                sanitationDataGrid.DataSource = sanitationData;

                statusLabel.Text = "Default sanitation data loaded";
                statusLabel.ForeColor = Color.Blue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sanitation data: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                sanitationData = new BindingList<SanitationDistrict>();
                sanitationDataGrid.DataSource = sanitationData;

                statusLabel.Text = "Error loading data";
                statusLabel.ForeColor = Color.Red;
            }
        }

        private BindingList<SanitationDistrict> GetDefaultSanitationData()
        {
            return new BindingList<SanitationDistrict>
            {
                // Revenue Items (based on Rate Study Methodology)
                new SanitationDistrict { Account = "311.00", Label = "Specific Ownership Taxes", Section = "Revenue", CurrentFYBudget = 15500.00m, MonthlyInput = 1291.67m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "301.00", Label = "Sewage Sales", Section = "Revenue", CurrentFYBudget = 100000.00m, MonthlyInput = 8333.33m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.2m, TimeOfUseFactor = 1.1m, CustomerAffordabilityIndex = 0.9m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "310.10", Label = "Delinquent Taxes", Section = "Revenue", CurrentFYBudget = 2500.00m, MonthlyInput = 208.33m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "313.00", Label = "Senior Homestead Exemption", Section = "Revenue", CurrentFYBudget = 100.00m, MonthlyInput = 8.33m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "320.00", Label = "Penalties and Interest", Section = "Revenue", CurrentFYBudget = 15000.00m, MonthlyInput = 1250.00m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "321.00", Label = "Misc Income", Section = "Revenue", CurrentFYBudget = 2000.00m, MonthlyInput = 166.67m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "315.00", Label = "Interest on Investments", Section = "Revenue", CurrentFYBudget = 48500.00m, MonthlyInput = 4041.67m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "322.00", Label = "Grant", Section = "Revenue", CurrentFYBudget = 0.00m, MonthlyInput = 0.00m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },

                // Operating Expenses
                new SanitationDistrict { Account = "401.00", Label = "Permits and Assessments", Section = "Operating", CurrentFYBudget = 976.00m, MonthlyInput = 81.33m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "401.10", Label = "Bank Service", Section = "Operating", CurrentFYBudget = 85.00m, MonthlyInput = 7.08m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "405.00", Label = "Outside Service Lab Fees", Section = "Operating", CurrentFYBudget = 650.00m, MonthlyInput = 54.17m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "405.10", Label = "Budget, Audit, Legal", Section = "Operating", CurrentFYBudget = 2000.00m, MonthlyInput = 166.67m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "410.00", Label = "Office Supplies / Postage", Section = "Operating", CurrentFYBudget = 1000.00m, MonthlyInput = 83.33m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "413.40", Label = "Education", Section = "Operating", CurrentFYBudget = 8325.00m, MonthlyInput = 693.75m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "415.00", Label = "Capital Outlay", Section = "Operating", CurrentFYBudget = 25000.00m, MonthlyInput = 2083.33m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "416.00", Label = "Dues and Subscriptions", Section = "Operating", CurrentFYBudget = 100.00m, MonthlyInput = 8.33m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "418.00", Label = "Lift Station Utilities", Section = "Operating", CurrentFYBudget = 15000.00m, MonthlyInput = 1250.00m, SeasonalAdjustment = 500, TimeOfUseFactor = 1.2m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "420.00", Label = "Collection Fee", Section = "Operating", CurrentFYBudget = 12000.00m, MonthlyInput = 1000.00m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "425.00", Label = "Supplies and Expenses", Section = "Operating", CurrentFYBudget = 2000.00m, MonthlyInput = 166.67m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "430.00", Label = "Insurance", Section = "Operating", CurrentFYBudget = 3500.00m, MonthlyInput = 291.67m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "432.53", Label = "Sewer Cleaning", Section = "Operating", CurrentFYBudget = 7600.00m, MonthlyInput = 633.33m, SeasonalAdjustment = 1500, TimeOfUseFactor = 1.5m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "445.00", Label = "Treasurer Fees", Section = "Operating", CurrentFYBudget = 2000.00m, MonthlyInput = 166.67m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "484.00", Label = "Property Taxes", Section = "Operating", CurrentFYBudget = 1200.00m, MonthlyInput = 100.00m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "486.00", Label = "Equipment Repairs", Section = "Operating", CurrentFYBudget = 3000.00m, MonthlyInput = 250.00m, SeasonalAdjustment = 800, TimeOfUseFactor = 1.3m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "489.00", Label = "Pickup Usage Fee", Section = "Operating", CurrentFYBudget = 2400.00m, MonthlyInput = 200.00m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "491.00", Label = "Fuel", Section = "Operating", CurrentFYBudget = 4500.00m, MonthlyInput = 375.00m, SeasonalAdjustment = 200, TimeOfUseFactor = 1.2m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "491.01", Label = "Misc Operating", Section = "Operating", CurrentFYBudget = 500.00m, MonthlyInput = 41.67m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },

                // Administrative & General Expenses
                new SanitationDistrict { Account = "460.00", Label = "Supt Salaries", Section = "Admin", CurrentFYBudget = 26000.00m, MonthlyInput = 2166.67m, SeasonalAdjustment = 0, PercentAllocation = 0.40m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "460.10", Label = "Clerk Salaries", Section = "Admin", CurrentFYBudget = 26000.00m, MonthlyInput = 2166.67m, SeasonalAdjustment = 0, PercentAllocation = 0.40m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "460.12", Label = "Part-Time Clerk Salaries", Section = "Admin", CurrentFYBudget = 5000.00m, MonthlyInput = 416.67m, SeasonalAdjustment = 0, PercentAllocation = 0.40m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "465.00", Label = "Office Supplies/Postage", Section = "Admin", CurrentFYBudget = 3000.00m, MonthlyInput = 250.00m, SeasonalAdjustment = 0, PercentAllocation = 0.40m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "480.00", Label = "Outside Service-Lab", Section = "Admin", CurrentFYBudget = 2000.00m, MonthlyInput = 166.67m, SeasonalAdjustment = 0, PercentAllocation = 0.40m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "480.01", Label = "Insurance: Building/HCL", Section = "Admin", CurrentFYBudget = 1200.00m, MonthlyInput = 100.00m, SeasonalAdjustment = 0, PercentAllocation = 0.40m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "480.10", Label = "Insurance: Workmans Comp", Section = "Admin", CurrentFYBudget = 1500.00m, MonthlyInput = 125.00m, SeasonalAdjustment = 0, PercentAllocation = 0.40m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "483.00", Label = "Insurance: Trash Truck", Section = "Admin", CurrentFYBudget = 2000.00m, MonthlyInput = 166.67m, SeasonalAdjustment = 0, PercentAllocation = 0.40m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "487.00", Label = "Payroll Taxes", Section = "Admin", CurrentFYBudget = 6100.00m, MonthlyInput = 508.33m, SeasonalAdjustment = 0, PercentAllocation = 0.40m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "491.10", Label = "Interest", Section = "Admin", CurrentFYBudget = 1300.00m, MonthlyInput = 108.33m, SeasonalAdjustment = 0, PercentAllocation = 0.40m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "491.11", Label = "Employee Benefits", Section = "Admin", CurrentFYBudget = 16000.00m, MonthlyInput = 1333.33m, SeasonalAdjustment = 0, PercentAllocation = 0.40m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now }
            };
        }

        public BindingList<SanitationDistrict> GetSanitationData()
        {
            return sanitationData;
        }

        public async void SaveSanitationDataAsync()
        {
            try
            {
                // Calculate all fields before saving
                foreach (var district in sanitationData)
                {
                    CalculateFields(district);
                }

                bool success = await _repository.SaveAllAsync(sanitationData, "SanitationDistrict");
                if (success)
                {
                    MessageBox.Show("Sanitation District data saved successfully!", "Save Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    statusLabel.Text = "Data saved successfully";
                    statusLabel.ForeColor = Color.DarkGreen;
                }
                else
                {
                    MessageBox.Show("Failed to save sanitation district data.", "Save Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    statusLabel.Text = "Save failed";
                    statusLabel.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving sanitation district data: {ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                statusLabel.Text = "Save error";
                statusLabel.ForeColor = Color.Red;
            }
        }

        // Export functionality for reporting
        public void ExportToCSV(string filePath)
        {
            try
            {
                using (var writer = new System.IO.StreamWriter(filePath))
                {
                    // Write header
                    writer.WriteLine("Account,Label,Section,CurrentFYBudget,MonthlyInput,YearToDateSpending,PercentOfBudget,BudgetRemaining,Scenario1,Scenario2,Scenario3,RequiredRate");

                    // Write data
                    foreach (var district in sanitationData)
                    {
                        writer.WriteLine($"{district.Account},{district.Label},{district.Section},{district.CurrentFYBudget:F2},{district.MonthlyInput:F2},{district.YearToDateSpending:F2},{district.PercentOfBudget:F4},{district.BudgetRemaining:F2},{district.Scenario1:F2},{district.Scenario2:F2},{district.Scenario3:F2},{district.RequiredRate:F2}");
                    }
                }

                statusLabel.Text = "Data exported successfully";
                statusLabel.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                statusLabel.Text = "Export failed";
                statusLabel.ForeColor = Color.Red;
            }
        }

        // Get summary statistics
        public Dictionary<string, decimal> GetSummaryStatistics()
        {
            var stats = new Dictionary<string, decimal>();

            try
            {
                stats["TotalRevenue"] = GetTotalRevenue();
                stats["TotalExpenses"] = GetTotalExpenses();
                stats["NetIncome"] = stats["TotalRevenue"] - stats["TotalExpenses"];
                stats["AverageRequiredRate"] = sanitationData.Average(d => d.RequiredRate);
                stats["TotalYTDSpending"] = sanitationData.Sum(d => d.YearToDateSpending);
                stats["TotalBudgetRemaining"] = sanitationData.Sum(d => d.BudgetRemaining);
                stats["OverallBudgetUtilization"] = stats["TotalYTDSpending"] / sanitationData.Sum(d => d.CurrentFYBudget);

                // Scenario totals
                stats["Scenario1Total"] = sanitationData.Sum(d => d.Scenario1);
                stats["Scenario2Total"] = sanitationData.Sum(d => d.Scenario2);
                stats["Scenario3Total"] = sanitationData.Sum(d => d.Scenario3);
            }
            catch (Exception)
            {
                // Return empty stats if calculation fails
                stats.Clear();
            }

            return stats;
        }

        // Apply bulk adjustments
        public void ApplyBulkAdjustment(string section, decimal adjustmentPercent)
        {
            try
            {
                var itemsToAdjust = sanitationData.Where(d => d.Section == section).ToList();

                foreach (var item in itemsToAdjust)
                {
                    item.CurrentFYBudget *= (1 + adjustmentPercent);
                    item.MonthlyInput *= (1 + adjustmentPercent);
                    CalculateFields(item);
                }

                RefreshGrid();
                statusLabel.Text = $"Applied {adjustmentPercent:P1} adjustment to {section} items";
                statusLabel.ForeColor = Color.Blue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying bulk adjustment: {ex.Message}", "Adjustment Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Reset calculated fields
        public void RecalculateAll()
        {
            try
            {
                foreach (var district in sanitationData)
                {
                    CalculateFields(district);
                }

                RefreshGrid();
                statusLabel.Text = "All calculations refreshed";
                statusLabel.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Recalculation error: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
            }
        }

        // Check for data inconsistencies
        public List<string> CheckDataConsistency()
        {
            var issues = new List<string>();

            try
            {
                for (int i = 0; i < sanitationData.Count; i++)
                {
                    var district = sanitationData[i];
                    var rowRef = $"Row {i + 1} ({district.Account})";

                    // Check for missing required data
                    if (string.IsNullOrWhiteSpace(district.Account))
                        issues.Add($"{rowRef}: Missing account number");

                    if (string.IsNullOrWhiteSpace(district.Label))
                        issues.Add($"{rowRef}: Missing label/description");

                    if (string.IsNullOrWhiteSpace(district.Section))
                        issues.Add($"{rowRef}: Missing section");

                    // Check for unrealistic values
                    if (district.CurrentFYBudget < 0)
                        issues.Add($"{rowRef}: Negative budget amount");

                    if (district.PercentOfBudget > 2.0m)
                        issues.Add($"{rowRef}: Budget utilization over 200%");

                    if (district.RequiredRate < 0)
                        issues.Add($"{rowRef}: Negative required rate");
                }

                // Check totals
                decimal totalRevenue = GetTotalRevenue();
                decimal totalExpenses = GetTotalExpenses();

                if (totalRevenue <= 0)
                    issues.Add("No revenue items found");

                if (totalExpenses <= 0)
                    issues.Add("No expense items found");

                if (Math.Abs(totalRevenue - totalExpenses) / Math.Max(totalRevenue, totalExpenses) > 0.5m)
                    issues.Add("Large imbalance between revenue and expenses");
            }
            catch (Exception ex)
            {
                issues.Add($"Error during consistency check: {ex.Message}");
            }

            return issues;
        }
    }
}
