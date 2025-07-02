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
    public partial class WaterInput : Form
    {
        private SfDataGrid waterDataGrid = null!;
        private BindingList<SanitationDistrict> waterData = null!;
        private readonly ISanitationRepository _repository;
        private readonly DatabaseManager _databaseManager;
        private Button saveButton = null!;
        private Button addRowButton = null!;
        private Button deleteRowButton = null!;
        private ComboBox sectionFilterCombo = null!;
        private Label statusLabel = null!;

        public WaterInput()
        {
            _databaseManager = new DatabaseManager();
            _repository = new SanitationRepository(_databaseManager);

            InitializeComponent();
            InitializeControls();
            InitializeWaterDataGrid();
            LoadWaterDataAsync();
            SetupValidation();
        }

        private void InitializeControls()
        {
            this.Text = "Water District - Revenue & Expenses";
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
            sectionFilterCombo.Items.AddRange(new[] { "All", "Revenue", "Operating", "Infrastructure", "Quality" });
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

        private void InitializeWaterDataGrid()
        {
            waterDataGrid = new SfDataGrid()
            {
                Dock = DockStyle.Fill,
                AllowEditing = true,
                AllowResizingColumns = true,
                AutoGenerateColumns = false,
                ShowGroupDropArea = true,
                SelectionMode = Syncfusion.WinForms.DataGrid.Enums.GridSelectionMode.Single
            };

            // Configure columns for Water District data following Rate Study Methodology
            waterDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account #", Width = 80 });
            waterDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 250 });
            waterDataGrid.Columns.Add(new GridComboBoxColumn()
            {
                MappingName = "Section",
                HeaderText = "Section",
                Width = 100,
                DataSource = new[] { "Revenue", "Operating", "Infrastructure", "Quality" }
            });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CurrentFYBudget", HeaderText = "Current FY Budget", Width = 120, Format = "C" });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "SeasonalAdjustment", HeaderText = "Seasonal Adj", Width = 100, Format = "C" });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "MonthlyInput", HeaderText = "Monthly Input", Width = 110, Format = "C" });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "SeasonalRevenueFactor", HeaderText = "Seasonal Factor", Width = 100, Format = "N2" });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "YearToDateSpending", HeaderText = "YTD Spending", Width = 110, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightGray } });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "PercentOfBudget", HeaderText = "% of Budget", Width = 90, Format = "P2", AllowEditing = false, CellStyle = { BackColor = Color.LightGray } });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "BudgetRemaining", HeaderText = "Budget Remaining", Width = 120, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightGray } });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "GoalAdjustment", HeaderText = "Goal Adjustment", Width = 110, Format = "C" });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "ReserveTarget", HeaderText = "Reserve Target", Width = 110, Format = "C" });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "TimeOfUseFactor", HeaderText = "TOU Factor", Width = 90, Format = "N2" });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CustomerAffordabilityIndex", HeaderText = "Affordability", Width = 100, Format = "N2" });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "PercentAllocation", HeaderText = "% Allocation", Width = 100, Format = "P2" });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "MonthlyUsage", HeaderText = "Monthly Usage (Gal)", Width = 120, Format = "N0" });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Scenario1", HeaderText = "Scenario 1", Width = 90, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightYellow } });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Scenario2", HeaderText = "Scenario 2", Width = 90, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightYellow } });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Scenario3", HeaderText = "Scenario 3", Width = 90, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightYellow } });
            waterDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "RequiredRate", HeaderText = "Required Rate", Width = 100, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightBlue } });

            // Handle cell value changes for real-time calculations
            waterDataGrid.CurrentCellEndEdit += WaterDataGrid_CurrentCellEndEdit;

            this.Controls.Add(waterDataGrid);
        }

        private void WaterDataGrid_CurrentCellEndEdit(object? sender, CurrentCellEndEditEventArgs e)
        {
            if (waterData != null && waterDataGrid.CurrentCell != null)
            {
                var rowIndex = waterDataGrid.CurrentCell.RowIndex;
                if (rowIndex >= 0 && rowIndex < waterData.Count)
                {
                    var district = waterData[rowIndex];
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

                // Calculate Water-specific Scenarios
                CalculateWaterScenarios(district);

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

            // Apply seasonal adjustments for water usage patterns
            decimal seasonalImpact = 0;

            if (district.Section == "Revenue")
            {
                // Water revenue has summer peak usage
                seasonalImpact = district.SeasonalAdjustment * district.SeasonalRevenueFactor;
            }
            else
            {
                // Operating and Infrastructure expenses
                seasonalImpact = district.SeasonalAdjustment;
            }

            district.YearToDateSpending = Math.Max(0, baseYTD + seasonalImpact);

            // Cap at 120% of budget to prevent unrealistic values
            if (district.YearToDateSpending > district.CurrentFYBudget * 1.2m)
            {
                district.YearToDateSpending = district.CurrentFYBudget * 1.2m;
            }
        }

        private void CalculateWaterScenarios(SanitationDistrict district)
        {
            decimal baseMonthly = district.MonthlyInput;

            // Water Infrastructure Scenarios based on Rate Study Methodology
            // Scenario 1: New Water Treatment Plant ($750,000, 20-year lifespan, 4% interest)
            decimal treatmentPlantMonthlyImpact = 4544.22m; // PMT calculation

            // Scenario 2: Pipeline Replacement Program ($200,000 over 10 years, 3.5% interest)
            decimal pipelineMonthlyImpact = 1962.16m;

            // Scenario 3: Water Quality Upgrade ($125,000, 8 years, 3% interest)
            decimal qualityUpgradeMonthlyImpact = 1479.86m;

            switch (district.Section)
            {
                case "Revenue":
                    // Revenue needs to cover infrastructure costs
                    district.Scenario1 = baseMonthly + (treatmentPlantMonthlyImpact * district.PercentAllocation);
                    district.Scenario2 = baseMonthly + (pipelineMonthlyImpact * district.PercentAllocation);
                    district.Scenario3 = baseMonthly + (qualityUpgradeMonthlyImpact * district.PercentAllocation);
                    break;

                case "Operating":
                    // Operating expenses directly impacted
                    district.Scenario1 = baseMonthly + (treatmentPlantMonthlyImpact * 0.15m); // 15% operational impact
                    district.Scenario2 = baseMonthly + (pipelineMonthlyImpact * 0.10m);
                    district.Scenario3 = baseMonthly + district.GoalAdjustment;
                    break;

                case "Infrastructure":
                    // Infrastructure directly affected
                    district.Scenario1 = baseMonthly + treatmentPlantMonthlyImpact;
                    district.Scenario2 = baseMonthly + pipelineMonthlyImpact;
                    district.Scenario3 = baseMonthly + qualityUpgradeMonthlyImpact;
                    break;

                case "Quality":
                    // Quality assurance costs
                    district.Scenario1 = baseMonthly + (treatmentPlantMonthlyImpact * 0.05m);
                    district.Scenario2 = baseMonthly + (pipelineMonthlyImpact * 0.03m);
                    district.Scenario3 = baseMonthly + qualityUpgradeMonthlyImpact;
                    break;

                default:
                    district.Scenario1 = baseMonthly;
                    district.Scenario2 = baseMonthly;
                    district.Scenario3 = baseMonthly;
                    break;
            }

            // Apply time-of-use and affordability adjustments
            ApplyFactors(district);

            // Ensure scenarios are not negative
            district.Scenario1 = Math.Max(0, district.Scenario1);
            district.Scenario2 = Math.Max(0, district.Scenario2);
            district.Scenario3 = Math.Max(0, district.Scenario3);
        }

        private void ApplyFactors(SanitationDistrict district)
        {
            if (district.TimeOfUseFactor > 0 && district.TimeOfUseFactor != 1.0m)
            {
                district.Scenario1 *= district.TimeOfUseFactor;
                district.Scenario2 *= district.TimeOfUseFactor;
                district.Scenario3 *= district.TimeOfUseFactor;
            }

            if (district.CustomerAffordabilityIndex > 0 && district.CustomerAffordabilityIndex != 1.0m)
            {
                district.Scenario1 *= district.CustomerAffordabilityIndex;
                district.Scenario2 *= district.CustomerAffordabilityIndex;
                district.Scenario3 *= district.CustomerAffordabilityIndex;
            }
        }

        private void CalculateRequiredRate(SanitationDistrict district)
        {
            try
            {
                decimal calculatedRate = 0;
                decimal customerBase = GetTotalCustomerBase();

                if (customerBase <= 0) 
                {
                    district.RequiredRate = 0;
                    return;
                }

                switch (district.Section)
                {
                    case "Revenue":
                        calculatedRate = CalculateRevenueRequiredRate(district, customerBase);
                        break;
                    case "Operating":
                        calculatedRate = district.CurrentFYBudget / customerBase / 12;
                        break;
                    case "Infrastructure":
                        calculatedRate = (district.CurrentFYBudget + district.GoalAdjustment) / customerBase / 12;
                        break;
                    case "Quality":
                        calculatedRate = district.CurrentFYBudget / customerBase / 12;
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

                district.RequiredRate = Math.Max(0, calculatedRate);
            }
            catch (Exception)
            {
                district.RequiredRate = 0;
            }
        }

        private decimal CalculateRevenueRequiredRate(SanitationDistrict district, decimal customerBase)
        {
            decimal totalExpenses = GetTotalExpenses();
            decimal totalRevenue = GetTotalRevenue();

            if (totalRevenue <= 0) return 0;

            decimal revenueShare = district.CurrentFYBudget / totalRevenue;
            return (totalExpenses * revenueShare) / customerBase / 12;
        }

        private decimal GetTotalExpenses()
        {
            return waterData?.Where(d => d.Section != "Revenue").Sum(d => d.CurrentFYBudget) ?? 0;
        }

        private decimal GetTotalRevenue()
        {
            return waterData?.Where(d => d.Section == "Revenue").Sum(d => d.CurrentFYBudget) ?? 0;
        }

        private decimal GetTotalCustomerBase()
        {
            return 850; // Town of Wiley approximate water customer base
        }

        private void RefreshGrid()
        {
            waterDataGrid.View?.Refresh();
        }

        private void AddRowButton_Click(object? sender, EventArgs e)
        {
            var newDistrict = new SanitationDistrict
            {
                Account = $"W{waterData.Count + 1:000}",
                Label = "New Water Item",
                Section = "Revenue",
                EntryDate = DateTime.Now,
                TimeOfUseFactor = 1.0m,
                CustomerAffordabilityIndex = 1.0m,
                SeasonalRevenueFactor = 1.0m
            };

            waterData.Add(newDistrict);
            statusLabel.Text = "New row added";
            statusLabel.ForeColor = Color.DarkGreen;
        }

        private void DeleteRowButton_Click(object? sender, EventArgs e)
        {
            if (waterDataGrid.SelectedIndex >= 0 && waterDataGrid.SelectedIndex < waterData.Count)
            {
                var result = MessageBox.Show("Are you sure you want to delete this row?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    waterData.RemoveAt(waterDataGrid.SelectedIndex);
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
                waterDataGrid.View.Filter = null;
            }
            else
            {
                waterDataGrid.View.Filter = item =>
                {
                    var district = item as SanitationDistrict;
                    return district?.Section == selectedSection;
                };
            }

            waterDataGrid.View.RefreshFilter();
            statusLabel.Text = $"Filtered by: {selectedSection}";
            statusLabel.ForeColor = Color.Blue;
        }

        private void SetupValidation()
        {
            // Additional setup for grid behavior
            waterDataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            ValidateAllData();
            SaveWaterDataAsync();
        }

        private void ValidateAllData()
        {
            var allErrors = new List<string>();
            var allWarnings = new List<string>();

            for (int i = 0; i < waterData.Count; i++)
            {
                var district = waterData[i];
                var rowPrefix = $"Row {i + 1} ({district.Account}): ";

                // Use comprehensive validation helper
                var fieldValidation = ValidationHelper.ValidateSanitationDistrict(district, "Water");
                var businessValidation = ValidationHelper.ValidateBusinessRules(district);

                // Collect validation errors and warnings
                foreach (var error in fieldValidation.Errors)
                {
                    allErrors.Add($"{rowPrefix}{error.Message}");
                }

                foreach (var warning in fieldValidation.Warnings)
                {
                    allWarnings.Add($"{rowPrefix}{warning.Message}");
                }

                foreach (var error in businessValidation.Errors)
                {
                    allErrors.Add($"{rowPrefix}{error.Message}");
                }

                foreach (var warning in businessValidation.Warnings)
                {
                    allWarnings.Add($"{rowPrefix}{warning.Message}");
                }

                // Water-specific validations
                ValidateWaterSpecificRules(district, rowPrefix, allErrors, allWarnings);
            }

            // Global validations
            ValidateWaterGlobalRules(allWarnings);

            // Display validation results
            DisplayValidationResults(allErrors, allWarnings);
        }

        private void ValidateWaterSpecificRules(SanitationDistrict district, string rowPrefix, List<string> errors, List<string> warnings)
        {
            // Water-specific account validation
            if (!district.Account.StartsWith("W") && !string.IsNullOrWhiteSpace(district.Account))
            {
                errors.Add($"{rowPrefix}Water accounts should start with 'W' prefix");
            }

            // Section-specific validations
            var validSections = new[] { "Revenue", "Operating", "Infrastructure", "Quality" };
            if (!validSections.Contains(district.Section))
            {
                errors.Add($"{rowPrefix}Section must be one of: {string.Join(", ", validSections)}");
            }

            // Water usage validation
            if (district.MonthlyUsage < 0)
            {
                errors.Add($"{rowPrefix}Monthly Usage cannot be negative");
            }

            if (district.MonthlyUsage > 100000000) // 100 million gallons seems excessive
            {
                warnings.Add($"{rowPrefix}Monthly Usage ({district.MonthlyUsage:N0} gallons) seems unusually high");
            }

            // Seasonal factor validation for water
            if (district.SeasonalRevenueFactor < 0.8m || district.SeasonalRevenueFactor > 1.5m)
            {
                warnings.Add($"{rowPrefix}Seasonal Revenue Factor should typically be between 0.8 and 1.5 for water");
            }

            // Infrastructure scenario validation
            if (district.Section == "Infrastructure")
            {
                if (district.Scenario1 > district.CurrentFYBudget * 3)
                {
                    warnings.Add($"{rowPrefix}Scenario 1 (Treatment Plant) impact seems very high");
                }
            }

            // Quality assurance validation
            if (district.Section == "Quality" && district.CurrentFYBudget < 1000)
            {
                warnings.Add($"{rowPrefix}Quality assurance budget may be insufficient");
            }
        }

        private void ValidateWaterGlobalRules(List<string> warnings)
        {
            decimal totalRevenue = GetTotalRevenue();
            decimal totalExpenses = GetTotalExpenses();

            // Revenue vs Expense balance validation
            if (totalExpenses > totalRevenue)
            {
                decimal deficit = totalExpenses - totalRevenue;
                warnings.Add($"BUDGET IMBALANCE: Total Expenses ({totalExpenses:C}) exceed Total Revenue ({totalRevenue:C}) by {deficit:C}");
            }

            // Check infrastructure allocation
            decimal infrastructureCosts = waterData?.Where(d => d.Section == "Infrastructure").Sum(d => d.CurrentFYBudget) ?? 0;
            decimal totalBudget = totalRevenue + totalExpenses;

            if (infrastructureCosts < totalBudget * 0.15m)
            {
                warnings.Add("Infrastructure allocation may be too low (recommended: at least 15% of total budget)");
            }

            // Check quality assurance allocation
            decimal qualityCosts = waterData?.Where(d => d.Section == "Quality").Sum(d => d.CurrentFYBudget) ?? 0;
            
            if (qualityCosts < totalBudget * 0.05m)
            {
                warnings.Add("Quality assurance allocation may be too low (recommended: at least 5% of total budget)");
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

        private void LoadWaterDataAsync()
        {
            try
            {
                // Initialize with predefined water district data
                waterData = GetDefaultWaterData();
                waterDataGrid.DataSource = waterData;

                statusLabel.Text = "Default water data loaded";
                statusLabel.ForeColor = Color.Blue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading water data: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                waterData = new BindingList<SanitationDistrict>();
                waterDataGrid.DataSource = waterData;

                statusLabel.Text = "Error loading data";
                statusLabel.ForeColor = Color.Red;
            }
        }

        private BindingList<SanitationDistrict> GetDefaultWaterData()
        {
            return new BindingList<SanitationDistrict>
            {
                // Revenue Items for Water District
                new SanitationDistrict { Account = "W311.00", Label = "Specific Ownership Taxes - Water", Section = "Revenue", CurrentFYBudget = 18500.00m, MonthlyInput = 1541.67m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, PercentAllocation = 0.25m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W301.00", Label = "Water Sales", Section = "Revenue", CurrentFYBudget = 180000.00m, MonthlyInput = 15000.00m, SeasonalAdjustment = 5000, SeasonalRevenueFactor = 1.3m, TimeOfUseFactor = 1.2m, CustomerAffordabilityIndex = 0.95m, PercentAllocation = 0.70m, MonthlyUsage = 12500000, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W310.10", Label = "Delinquent Taxes - Water", Section = "Revenue", CurrentFYBudget = 3000.00m, MonthlyInput = 250.00m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, PercentAllocation = 0.25m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W320.00", Label = "Penalties and Interest - Water", Section = "Revenue", CurrentFYBudget = 8000.00m, MonthlyInput = 666.67m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, PercentAllocation = 0.25m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W315.00", Label = "Interest on Investments - Water", Section = "Revenue", CurrentFYBudget = 25000.00m, MonthlyInput = 2083.33m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, PercentAllocation = 0.25m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W322.00", Label = "Water System Grant", Section = "Revenue", CurrentFYBudget = 15000.00m, MonthlyInput = 1250.00m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, PercentAllocation = 0.25m, EntryDate = DateTime.Now },

                // Operating Expenses
                new SanitationDistrict { Account = "W401.00", Label = "Water System Permits", Section = "Operating", CurrentFYBudget = 2500.00m, MonthlyInput = 208.33m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W410.00", Label = "Office Supplies - Water", Section = "Operating", CurrentFYBudget = 1500.00m, MonthlyInput = 125.00m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W415.00", Label = "Water System Repairs", Section = "Operating", CurrentFYBudget = 15000.00m, MonthlyInput = 1250.00m, SeasonalAdjustment = 2000, TimeOfUseFactor = 1.3m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W418.00", Label = "Water Plant Utilities", Section = "Operating", CurrentFYBudget = 24000.00m, MonthlyInput = 2000.00m, SeasonalAdjustment = 1500, TimeOfUseFactor = 1.4m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W425.00", Label = "Water Treatment Chemicals", Section = "Operating", CurrentFYBudget = 8500.00m, MonthlyInput = 708.33m, SeasonalAdjustment = 500, TimeOfUseFactor = 1.1m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W430.00", Label = "Water System Insurance", Section = "Operating", CurrentFYBudget = 5500.00m, MonthlyInput = 458.33m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W491.00", Label = "Vehicle Fuel - Water", Section = "Operating", CurrentFYBudget = 3500.00m, MonthlyInput = 291.67m, SeasonalAdjustment = 300, TimeOfUseFactor = 1.2m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },

                // Infrastructure
                new SanitationDistrict { Account = "W500.00", Label = "Water Line Replacement", Section = "Infrastructure", CurrentFYBudget = 45000.00m, MonthlyInput = 3750.00m, SeasonalAdjustment = 5000, GoalAdjustment = 10000, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W501.00", Label = "Water Meter Replacement", Section = "Infrastructure", CurrentFYBudget = 25000.00m, MonthlyInput = 2083.33m, SeasonalAdjustment = 0, GoalAdjustment = 5000, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W502.00", Label = "Water Plant Equipment", Section = "Infrastructure", CurrentFYBudget = 35000.00m, MonthlyInput = 2916.67m, SeasonalAdjustment = 0, GoalAdjustment = 15000, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W503.00", Label = "Water Storage Tank Maintenance", Section = "Infrastructure", CurrentFYBudget = 12000.00m, MonthlyInput = 1000.00m, SeasonalAdjustment = 2000, GoalAdjustment = 3000, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },

                // Quality Assurance
                new SanitationDistrict { Account = "W405.00", Label = "Water Quality Testing", Section = "Quality", CurrentFYBudget = 3500.00m, MonthlyInput = 291.67m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W405.10", Label = "EPA Compliance", Section = "Quality", CurrentFYBudget = 2000.00m, MonthlyInput = 166.67m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "W413.40", Label = "Water Operator Training", Section = "Quality", CurrentFYBudget = 1500.00m, MonthlyInput = 125.00m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now }
            };
        }

        public BindingList<SanitationDistrict> GetWaterData()
        {
            return waterData;
        }

        public async void SaveWaterDataAsync()
        {
            try
            {
                // Calculate all fields before saving
                foreach (var district in waterData)
                {
                    CalculateFields(district);
                }

                bool success = await _repository.SaveAllAsync(waterData, "Water");
                if (success)
                {
                    MessageBox.Show("Water District data saved successfully!", "Save Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    statusLabel.Text = "Data saved successfully";
                    statusLabel.ForeColor = Color.DarkGreen;
                }
                else
                {
                    MessageBox.Show("Failed to save water district data.", "Save Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    statusLabel.Text = "Save failed";
                    statusLabel.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving water district data: {ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                statusLabel.Text = "Save error";
                statusLabel.ForeColor = Color.Red;
            }
        }

        // Additional methods for comprehensive functionality
        public Dictionary<string, decimal> GetWaterSummaryStatistics()
        {
            var stats = new Dictionary<string, decimal>();

            try
            {
                stats["TotalRevenue"] = GetTotalRevenue();
                stats["TotalExpenses"] = GetTotalExpenses();
                stats["NetIncome"] = stats["TotalRevenue"] - stats["TotalExpenses"];
                stats["AverageRequiredRate"] = waterData.Average(d => d.RequiredRate);
                stats["TotalYTDSpending"] = waterData.Sum(d => d.YearToDateSpending);
                stats["TotalMonthlyUsage"] = waterData.Sum(d => d.MonthlyUsage);
                stats["InfrastructureInvestment"] = waterData.Where(d => d.Section == "Infrastructure").Sum(d => d.CurrentFYBudget);
                stats["QualityInvestment"] = waterData.Where(d => d.Section == "Quality").Sum(d => d.CurrentFYBudget);
            }
            catch (Exception)
            {
                stats.Clear();
            }

            return stats;
        }

        public void ExportWaterData(string filePath)
        {
            try
            {
                using (var writer = new System.IO.StreamWriter(filePath))
                {
                    writer.WriteLine("Account,Label,Section,CurrentFYBudget,MonthlyInput,YearToDateSpending,PercentOfBudget,BudgetRemaining,MonthlyUsage,Scenario1,Scenario2,Scenario3,RequiredRate");

                    foreach (var district in waterData)
                    {
                        writer.WriteLine($"{district.Account},{district.Label},{district.Section},{district.CurrentFYBudget:F2},{district.MonthlyInput:F2},{district.YearToDateSpending:F2},{district.PercentOfBudget:F4},{district.BudgetRemaining:F2},{district.MonthlyUsage:F0},{district.Scenario1:F2},{district.Scenario2:F2},{district.Scenario3:F2},{district.RequiredRate:F2}");
                    }
                }

                statusLabel.Text = "Water data exported successfully";
                statusLabel.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting water data: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void RecalculateAllWater()
        {
            try
            {
                foreach (var district in waterData)
                {
                    CalculateFields(district);
                }

                RefreshGrid();
                statusLabel.Text = "All water calculations refreshed";
                statusLabel.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Recalculation error: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
            }
        }
    }
}
