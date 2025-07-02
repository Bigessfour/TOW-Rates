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
    public partial class TrashInput : Form
    {
        private SfDataGrid trashDataGrid = null!;
        private BindingList<SanitationDistrict> trashData = null!;
        private readonly ISanitationRepository _repository;
        private readonly DatabaseManager _databaseManager;
        private Button saveButton = null!;
        private Button addRowButton = null!;
        private Button deleteRowButton = null!;
        private ComboBox sectionFilterCombo = null!;
        private Label statusLabel = null!;

        public TrashInput()
        {
            _databaseManager = new DatabaseManager();
            _repository = new SanitationRepository(_databaseManager);

            InitializeComponent();
            InitializeControls();
            InitializeTrashDataGrid();
            LoadTrashDataAsync();
            SetupValidation();
        }

        private void InitializeControls()
        {
            this.Text = "Trash & Sanitation District - Revenue & Expenses";
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
            sectionFilterCombo.Items.AddRange(new[] { "All", "Revenue", "Collections", "Recycling", "Operations", "Equipment" });
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

        private void InitializeTrashDataGrid()
        {
            trashDataGrid = new SfDataGrid()
            {
                Dock = DockStyle.Fill,
                AllowEditing = true,
                AllowResizingColumns = true,
                AutoGenerateColumns = false,
                ShowGroupDropArea = true,
                SelectionMode = Syncfusion.WinForms.DataGrid.Enums.GridSelectionMode.Single
            };

            // Configure columns for Trash/Sanitation data following Rate Study Methodology
            trashDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account #", Width = 80 });
            trashDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 250 });
            trashDataGrid.Columns.Add(new GridComboBoxColumn()
            {
                MappingName = "Section",
                HeaderText = "Section",
                Width = 100,
                DataSource = new[] { "Revenue", "Collections", "Recycling", "Operations", "Equipment" }
            });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CurrentFYBudget", HeaderText = "Current FY Budget", Width = 120, Format = "C" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "SeasonalAdjustment", HeaderText = "Seasonal Adj", Width = 100, Format = "C" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "MonthlyInput", HeaderText = "Monthly Input", Width = 110, Format = "C" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "SeasonalRevenueFactor", HeaderText = "Seasonal Factor", Width = 100, Format = "N2" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "YearToDateSpending", HeaderText = "YTD Spending", Width = 110, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightGray } });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "PercentOfBudget", HeaderText = "% of Budget", Width = 90, Format = "P2", AllowEditing = false, CellStyle = { BackColor = Color.LightGray } });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "BudgetRemaining", HeaderText = "Budget Remaining", Width = 120, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightGray } });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "GoalAdjustment", HeaderText = "Goal Adjustment", Width = 110, Format = "C" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "ReserveTarget", HeaderText = "Reserve Target", Width = 110, Format = "C" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "TimeOfUseFactor", HeaderText = "TOU Factor", Width = 90, Format = "N2" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CustomerAffordabilityIndex", HeaderText = "Affordability", Width = 100, Format = "N2" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "PercentAllocation", HeaderText = "% Allocation", Width = 100, Format = "P2" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "MonthlyUsage", HeaderText = "Tonnage/Month", Width = 110, Format = "N1" });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Scenario1", HeaderText = "Scenario 1", Width = 90, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightYellow } });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Scenario2", HeaderText = "Scenario 2", Width = 90, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightYellow } });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Scenario3", HeaderText = "Scenario 3", Width = 90, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightYellow } });
            trashDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "RequiredRate", HeaderText = "Required Rate", Width = 100, Format = "C", AllowEditing = false, CellStyle = { BackColor = Color.LightBlue } });

            // Handle cell value changes for real-time calculations
            trashDataGrid.CurrentCellEndEdit += TrashDataGrid_CurrentCellEndEdit;

            this.Controls.Add(trashDataGrid);
        }

        private void TrashDataGrid_CurrentCellEndEdit(object? sender, CurrentCellEndEditEventArgs e)
        {
            if (trashData != null && trashDataGrid.CurrentCell != null)
            {
                var rowIndex = trashDataGrid.CurrentCell.RowIndex;
                if (rowIndex >= 0 && rowIndex < trashData.Count)
                {
                    var district = trashData[rowIndex];
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

                // Calculate Trash-specific Scenarios
                CalculateTrashScenarios(district);

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

            // Apply seasonal adjustments for trash collection patterns
            decimal seasonalImpact = 0;

            if (district.Section == "Revenue")
            {
                // Revenue may vary seasonally (holiday waste, summer activities)
                seasonalImpact = district.SeasonalAdjustment * district.SeasonalRevenueFactor;
            }
            else if (district.Section == "Collections")
            {
                // Collection costs may increase during peak seasons
                seasonalImpact = district.SeasonalAdjustment * 1.2m;
            }
            else
            {
                seasonalImpact = district.SeasonalAdjustment;
            }

            district.YearToDateSpending = Math.Max(0, baseYTD + seasonalImpact);

            // Cap at 120% of budget to prevent unrealistic values
            if (district.YearToDateSpending > district.CurrentFYBudget * 1.2m)
            {
                district.YearToDateSpending = district.CurrentFYBudget * 1.2m;
            }
        }

        private void CalculateTrashScenarios(SanitationDistrict district)
        {
            decimal baseMonthly = district.MonthlyInput;

            // Trash Management Scenarios based on Rate Study Methodology
            // Scenario 1: New Trash Truck ($350,000, 12-year lifespan, 4.5% interest) - CRITICAL PRIORITY
            decimal trashTruckAnnualCost = 32083.34m; // From rate study: $29,166.67 depreciation + $2,916.67 maintenance
            decimal trashTruckMonthlyImpact = trashTruckAnnualCost / 12; // $2,673.61

            // Scenario 2: Recycling Program Expansion ($125,000, 7 years, 4% interest)
            decimal recyclingProgramAnnualCost = 20373.52m; // Enhanced program with processing equipment
            decimal recyclingProgramMonthlyImpact = recyclingProgramAnnualCost / 12; // $1,697.79

            // Scenario 3: Transfer Station & Route Optimization ($200,000, 15 years, 3.5% interest)
            decimal transferStationAnnualCost = 17157.24m; // Infrastructure + route efficiency
            decimal transferStationMonthlyImpact = transferStationAnnualCost / 12; // $1,429.77

            // Add fleet maintenance reserves (10% of equipment value annually)
            decimal maintenanceReserve = (district.Section == "Equipment") ?
                (350000m * 0.10m / 12) : 0; // $2,916.67 monthly for equipment accounts

            switch (district.Section)
            {
                case "Revenue":
                    // Revenue needs to cover equipment and program costs with 2.67% rate increase
                    // Sewage Sales base ($100,000) * 2.67% = $2,670 monthly impact
                    district.Scenario1 = baseMonthly + (trashTruckMonthlyImpact * (district.PercentAllocation / 100m));
                    district.Scenario2 = baseMonthly + (recyclingProgramMonthlyImpact * (district.PercentAllocation / 100m));
                    district.Scenario3 = baseMonthly + (transferStationMonthlyImpact * (district.PercentAllocation / 100m));
                    break;

                case "Collections":
                    // Collection services directly affected by new truck and route efficiency
                    district.Scenario1 = baseMonthly + trashTruckMonthlyImpact + district.GoalAdjustment + maintenanceReserve;
                    district.Scenario2 = baseMonthly + (recyclingProgramMonthlyImpact * 0.4m); // Collection impact for recycling
                    district.Scenario3 = baseMonthly + (transferStationMonthlyImpact * 0.6m); // Route optimization savings
                    break;

                case "Recycling":
                    // Recycling programs enhanced with new equipment and processing
                    district.Scenario1 = baseMonthly + (trashTruckMonthlyImpact * 0.1m); // Minimal direct impact
                    district.Scenario2 = baseMonthly + recyclingProgramMonthlyImpact + district.GoalAdjustment;
                    district.Scenario3 = baseMonthly + (transferStationMonthlyImpact * 0.5m); // Processing efficiency
                    break;

                case "Operations":
                    // General operations including fuel, disposal, and administrative costs
                    district.Scenario1 = baseMonthly + (trashTruckMonthlyImpact * 0.2m); // Operational efficiency
                    district.Scenario2 = baseMonthly + (recyclingProgramMonthlyImpact * 0.3m); // Program management
                    district.Scenario3 = baseMonthly + (transferStationMonthlyImpact * 0.8m); // Full operational impact
                    break;

                case "Equipment":
                    // Equipment depreciation and maintenance - full impact scenarios
                    district.Scenario1 = baseMonthly + trashTruckMonthlyImpact + maintenanceReserve;
                    district.Scenario2 = baseMonthly + recyclingProgramMonthlyImpact + (maintenanceReserve * 0.3m);
                    district.Scenario3 = baseMonthly + transferStationMonthlyImpact + (maintenanceReserve * 0.5m);
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
                    case "Collections":
                        calculatedRate = district.CurrentFYBudget / customerBase / 12;
                        break;
                    case "Recycling":
                        calculatedRate = (district.CurrentFYBudget + district.GoalAdjustment) / customerBase / 12;
                        break;
                    case "Operations":
                        calculatedRate = district.CurrentFYBudget / customerBase / 12;
                        break;
                    case "Equipment":
                        calculatedRate = (district.CurrentFYBudget + district.ReserveTarget) / customerBase / 12;
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
            return trashData?.Where(d => d.Section != "Revenue").Sum(d => d.CurrentFYBudget) ?? 0;
        }

        private decimal GetTotalRevenue()
        {
            return trashData?.Where(d => d.Section == "Revenue").Sum(d => d.CurrentFYBudget) ?? 0;
        }

        private decimal GetTotalCustomerBase()
        {
            return 850; // Town of Wiley approximate trash customer base
        }

        private void RefreshGrid()
        {
            trashDataGrid.View?.Refresh();
        }

        private void SetupValidation()
        {
            // Additional setup for grid behavior
            trashDataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            ValidateAllData();
            SaveTrashDataAsync();
        }

        private void AddRowButton_Click(object? sender, EventArgs e)
        {
            var newDistrict = new SanitationDistrict
            {
                Account = $"T{trashData.Count + 1:000}",
                Label = "New Trash Item",
                Section = "Revenue",
                EntryDate = DateTime.Now,
                TimeOfUseFactor = 1.0m,
                CustomerAffordabilityIndex = 1.0m,
                SeasonalRevenueFactor = 1.0m
            };

            trashData.Add(newDistrict);
            statusLabel.Text = "New row added";
            statusLabel.ForeColor = Color.DarkGreen;
        }

        private void DeleteRowButton_Click(object? sender, EventArgs e)
        {
            if (trashDataGrid.SelectedIndex >= 0 && trashDataGrid.SelectedIndex < trashData.Count)
            {
                var result = MessageBox.Show("Are you sure you want to delete this row?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    trashData.RemoveAt(trashDataGrid.SelectedIndex);
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
                trashDataGrid.View.Filter = null;
            }
            else
            {
                trashDataGrid.View.Filter = item =>
                {
                    var district = item as SanitationDistrict;
                    return district?.Section == selectedSection;
                };
            }

            trashDataGrid.View.RefreshFilter();
            statusLabel.Text = $"Filtered by: {selectedSection}";
            statusLabel.ForeColor = Color.Blue;
        }

        private void ValidateAllData()
        {
            var allErrors = new List<string>();
            var allWarnings = new List<string>();

            for (int i = 0; i < trashData.Count; i++)
            {
                var district = trashData[i];
                var rowPrefix = $"Row {i + 1} ({district.Account}): ";

                // Use comprehensive validation helper
                var fieldValidation = ValidationHelper.ValidateSanitationDistrict(district, "Trash");
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

                // Trash-specific validations
                ValidateTrashSpecificRules(district, rowPrefix, allErrors, allWarnings);
            }

            // Global validations
            ValidateTrashGlobalRules(allWarnings);

            // Display validation results
            DisplayValidationResults(allErrors, allWarnings);
        }

        private void ValidateTrashSpecificRules(SanitationDistrict district, string rowPrefix, List<string> errors, List<string> warnings)
        {
            // Trash-specific account validation
            if (!district.Account.StartsWith("T") && !string.IsNullOrWhiteSpace(district.Account))
            {
                warnings.Add($"{rowPrefix}Account should start with 'T' for Trash enterprise");
            }

            // Collection route efficiency validation
            if (district.Section == "Collections" && district.RequiredRate > 30m)
            {
                warnings.Add($"{rowPrefix}Collection rate ${district.RequiredRate:F2} exceeds typical range ($15-$30)");
            }

            // Recycling program cost-effectiveness validation
            if (district.Section == "Recycling" && district.MonthlyInput > 0)
            {
                decimal recyclingEfficiency = district.CurrentFYBudget / Math.Max(1, district.MonthlyInput * 12);
                if (recyclingEfficiency > 2.0m)
                {
                    warnings.Add($"{rowPrefix}Recycling program efficiency ratio {recyclingEfficiency:F2} may indicate high costs");
                }
            }

            // Equipment depreciation validation
            if (district.Section == "Equipment")
            {
                // Validate truck depreciation - $350K truck over 12 years = $29,166.67 annual
                decimal expectedAnnualDepreciation = 29166.67m;
                decimal actualAnnualBudget = district.CurrentFYBudget;

                if (Math.Abs(actualAnnualBudget - expectedAnnualDepreciation) > 5000m)
                {
                    warnings.Add($"{rowPrefix}Equipment budget ${actualAnnualBudget:F0} differs significantly from expected truck depreciation ${expectedAnnualDepreciation:F0}");
                }

                // Maintenance reserve validation (should be ~10% of asset value)
                decimal expectedMaintenanceReserve = 35000m; // 10% of $350K truck
                if (district.ReserveTarget < expectedMaintenanceReserve * 0.5m)
                {
                    warnings.Add($"{rowPrefix}Maintenance reserve ${district.ReserveTarget:F0} may be insufficient (recommended: ${expectedMaintenanceReserve:F0})");
                }
            }

            // Seasonal adjustment validation for waste collection
            if (district.Section == "Collections" || district.Section == "Operations")
            {
                // Seasonal variations typically 15-25% for waste collection
                decimal seasonalPercentage = Math.Abs(district.SeasonalAdjustment) / Math.Max(1, district.MonthlyInput) * 100;
                if (seasonalPercentage > 30m)
                {
                    warnings.Add($"{rowPrefix}Seasonal adjustment {seasonalPercentage:F1}% seems high for waste operations");
                }
            }

            // Rate impact validation for Scenario 1 (New Truck)
            if (district.Section == "Revenue" && district.Scenario1 > 0)
            {
                decimal rateIncrease = ((district.Scenario1 - district.MonthlyInput) / Math.Max(1, district.MonthlyInput)) * 100;
                if (Math.Abs(rateIncrease - 2.67m) > 0.5m) // Expected 2.67% increase per rate study
                {
                    warnings.Add($"{rowPrefix}Scenario 1 rate increase {rateIncrease:F2}% differs from expected 2.67% for new truck");
                }
            }

            // Section-specific validations
            var validSections = new[] { "Revenue", "Collections", "Recycling", "Operations", "Equipment" };
            if (!validSections.Contains(district.Section))
            {
                errors.Add($"{rowPrefix}Section must be one of: {string.Join(", ", validSections)}");
            }

            // Tonnage validation (using MonthlyUsage field)
            if (district.MonthlyUsage < 0)
            {
                errors.Add($"{rowPrefix}Monthly Tonnage cannot be negative");
            }

            if (district.MonthlyUsage > 10000) // 10,000 tons seems excessive for a small town
            {
                warnings.Add($"{rowPrefix}Monthly Tonnage ({district.MonthlyUsage:N1} tons) seems unusually high for a municipal operation");
            }

            // Collection-specific validations
            if (district.Section == "Collections" && district.CurrentFYBudget < 50000)
            {
                warnings.Add($"{rowPrefix}Collection budget may be insufficient for adequate service");
            }

            // Equipment replacement validation
            if (district.Section == "Equipment")
            {
                if (district.ReserveTarget < district.CurrentFYBudget * 0.1m)
                {
                    warnings.Add($"{rowPrefix}Equipment reserve target should be at least 10% of annual budget");
                }

                if (district.Scenario1 > district.CurrentFYBudget * 2)
                {
                    warnings.Add($"{rowPrefix}Scenario 1 (New Truck) impact seems very high");
                }
            }

            // Recycling program validation
            if (district.Section == "Recycling")
            {
                if (district.CurrentFYBudget > 0 && district.MonthlyUsage == 0)
                {
                    warnings.Add($"{rowPrefix}Recycling program should track tonnage processed");
                }
            }
        }

        private void ValidateTrashGlobalRules(List<string> warnings)
        {
            decimal totalRevenue = GetTotalRevenue();
            decimal totalExpenses = GetTotalExpenses();

            // Revenue vs Expense balance validation
            if (totalExpenses > totalRevenue)
            {
                decimal deficit = totalExpenses - totalRevenue;
                warnings.Add($"BUDGET IMBALANCE: Total Expenses ({totalExpenses:C}) exceed Total Revenue ({totalRevenue:C}) by {deficit:C}");
            }

            // Check equipment allocation
            decimal equipmentCosts = trashData?.Where(d => d.Section == "Equipment").Sum(d => d.CurrentFYBudget) ?? 0;
            decimal totalBudget = totalRevenue + totalExpenses;

            if (equipmentCosts < totalBudget * 0.20m)
            {
                warnings.Add("Equipment allocation may be too low (recommended: at least 20% of total budget for replacement reserves)");
            }

            // Check recycling allocation
            decimal recyclingCosts = trashData?.Where(d => d.Section == "Recycling").Sum(d => d.CurrentFYBudget) ?? 0;

            if (recyclingCosts < totalBudget * 0.10m)
            {
                warnings.Add("Recycling allocation may be too low (recommended: at least 10% of total budget)");
            }

            // Check tonnage vs budget alignment
            decimal totalTonnage = trashData?.Sum(d => d.MonthlyUsage) ?? 0;
            if (totalTonnage > 0 && totalBudget > 0)
            {
                decimal costPerTon = totalBudget / (totalTonnage * 12);
                if (costPerTon > 150)
                {
                    warnings.Add($"Cost per ton ({costPerTon:C}) seems high - consider efficiency improvements");
                }
                else if (costPerTon < 50)
                {
                    warnings.Add($"Cost per ton ({costPerTon:C}) seems low - verify tonnage data accuracy");
                }
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

        private void LoadTrashDataAsync()
        {
            try
            {
                // Initialize with predefined trash district data
                trashData = GetDefaultTrashData();
                trashDataGrid.DataSource = trashData;

                statusLabel.Text = "Default trash data loaded";
                statusLabel.ForeColor = Color.Blue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading trash data: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                trashData = new BindingList<SanitationDistrict>();
                trashDataGrid.DataSource = trashData;

                statusLabel.Text = "Error loading data";
                statusLabel.ForeColor = Color.Red;
            }
        }

        private BindingList<SanitationDistrict> GetDefaultTrashData()
        {
            return new BindingList<SanitationDistrict>
            {
                // Revenue Items for Trash District
                new SanitationDistrict { Account = "T311.00", Label = "Specific Ownership Taxes - Trash", Section = "Revenue", CurrentFYBudget = 22000.00m, MonthlyInput = 1833.33m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, PercentAllocation = 0.30m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "T301.00", Label = "Residential Trash Collection Fees", Section = "Revenue", CurrentFYBudget = 320000.00m, MonthlyInput = 26666.67m, SeasonalAdjustment = 8000, SeasonalRevenueFactor = 1.1m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 0.95m, PercentAllocation = 0.85m, MonthlyUsage = 850, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "T301.10", Label = "Commercial Trash Collection Fees", Section = "Revenue", CurrentFYBudget = 180000.00m, MonthlyInput = 15000.00m, SeasonalAdjustment = 5000, SeasonalRevenueFactor = 1.2m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, PercentAllocation = 0.85m, MonthlyUsage = 425, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "T320.00", Label = "Penalties and Interest - Trash", Section = "Revenue", CurrentFYBudget = 12000.00m, MonthlyInput = 1000.00m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, PercentAllocation = 0.30m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "T315.00", Label = "Interest on Investments - Trash", Section = "Revenue", CurrentFYBudget = 8000.00m, MonthlyInput = 666.67m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, PercentAllocation = 0.30m, EntryDate = DateTime.Now },

                // Collection Operations
                new SanitationDistrict { Account = "T401.00", Label = "Collection Route Operations", Section = "Collections", CurrentFYBudget = 180000.00m, MonthlyInput = 15000.00m, SeasonalAdjustment = 3000, TimeOfUseFactor = 1.1m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "T410.00", Label = "Collection Supplies", Section = "Collections", CurrentFYBudget = 8500.00m, MonthlyInput = 708.33m, SeasonalAdjustment = 500, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "T415.00", Label = "Vehicle Maintenance - Collection", Section = "Collections", CurrentFYBudget = 25000.00m, MonthlyInput = 2083.33m, SeasonalAdjustment = 2000, TimeOfUseFactor = 1.2m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "T491.00", Label = "Fuel - Collection Vehicles", Section = "Collections", CurrentFYBudget = 18000.00m, MonthlyInput = 1500.00m, SeasonalAdjustment = 1000, TimeOfUseFactor = 1.3m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },

                // Recycling Program
                new SanitationDistrict { Account = "T501.00", Label = "Recycling Collection", Section = "Recycling", CurrentFYBudget = 45000.00m, MonthlyInput = 3750.00m, SeasonalAdjustment = 1000, GoalAdjustment = 5000, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, MonthlyUsage = 125, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "T502.00", Label = "Recycling Processing", Section = "Recycling", CurrentFYBudget = 25000.00m, MonthlyInput = 2083.33m, SeasonalAdjustment = 0, GoalAdjustment = 3000, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, MonthlyUsage = 85, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "T503.00", Label = "Recycling Education", Section = "Recycling", CurrentFYBudget = 8000.00m, MonthlyInput = 666.67m, SeasonalAdjustment = 0, GoalAdjustment = 1000, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },

                // General Operations
                new SanitationDistrict { Account = "T425.00", Label = "Transfer Station Operations", Section = "Operations", CurrentFYBudget = 55000.00m, MonthlyInput = 4583.33m, SeasonalAdjustment = 2500, TimeOfUseFactor = 1.1m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "T430.00", Label = "Trash Services Insurance", Section = "Operations", CurrentFYBudget = 12000.00m, MonthlyInput = 1000.00m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "T435.00", Label = "Landfill Tipping Fees", Section = "Operations", CurrentFYBudget = 75000.00m, MonthlyInput = 6250.00m, SeasonalAdjustment = 3000, TimeOfUseFactor = 1.2m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "T440.00", Label = "Environmental Compliance", Section = "Operations", CurrentFYBudget = 8500.00m, MonthlyInput = 708.33m, SeasonalAdjustment = 0, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },

                // Equipment & Vehicles
                new SanitationDistrict { Account = "T600.00", Label = "Trash Collection Vehicles", Section = "Equipment", CurrentFYBudget = 50000.00m, MonthlyInput = 4166.67m, SeasonalAdjustment = 0, GoalAdjustment = 25000, ReserveTarget = 75000, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "T601.00", Label = "Collection Equipment", Section = "Equipment", CurrentFYBudget = 15000.00m, MonthlyInput = 1250.00m, SeasonalAdjustment = 0, GoalAdjustment = 5000, ReserveTarget = 20000, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now },
                new SanitationDistrict { Account = "T602.00", Label = "Container Replacement", Section = "Equipment", CurrentFYBudget = 12000.00m, MonthlyInput = 1000.00m, SeasonalAdjustment = 1000, GoalAdjustment = 3000, ReserveTarget = 10000, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m, EntryDate = DateTime.Now }
            };
        }

        public BindingList<SanitationDistrict> GetTrashData()
        {
            return trashData;
        }

        public async void SaveTrashDataAsync()
        {
            try
            {
                // Calculate all fields before saving
                foreach (var district in trashData)
                {
                    CalculateFields(district);
                }

                bool success = await _repository.SaveAllAsync(trashData, "Trash");
                if (success)
                {
                    MessageBox.Show("Trash District data saved successfully!", "Save Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    statusLabel.Text = "Data saved successfully";
                    statusLabel.ForeColor = Color.DarkGreen;
                }
                else
                {
                    MessageBox.Show("Failed to save trash district data.", "Save Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    statusLabel.Text = "Save failed";
                    statusLabel.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving trash district data: {ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                statusLabel.Text = "Save error";
                statusLabel.ForeColor = Color.Red;
            }
        }

        // Additional methods for comprehensive functionality
        public Dictionary<string, decimal> GetTrashSummaryStatistics()
        {
            var stats = new Dictionary<string, decimal>();

            try
            {
                stats["TotalRevenue"] = GetTotalRevenue();
                stats["TotalExpenses"] = GetTotalExpenses();
                stats["NetIncome"] = stats["TotalRevenue"] - stats["TotalExpenses"];
                stats["AverageRequiredRate"] = trashData.Average(d => d.RequiredRate);
                stats["TotalYTDSpending"] = trashData.Sum(d => d.YearToDateSpending);
                stats["TotalMonthlyTonnage"] = trashData.Sum(d => d.MonthlyUsage);
                stats["EquipmentInvestment"] = trashData.Where(d => d.Section == "Equipment").Sum(d => d.CurrentFYBudget + d.ReserveTarget);
                stats["RecyclingInvestment"] = trashData.Where(d => d.Section == "Recycling").Sum(d => d.CurrentFYBudget);
            }
            catch (Exception)
            {
                stats.Clear();
            }

            return stats;
        }

        public void ExportTrashData(string filePath)
        {
            try
            {
                using (var writer = new System.IO.StreamWriter(filePath))
                {
                    writer.WriteLine("Account,Label,Section,CurrentFYBudget,MonthlyInput,YearToDateSpending,PercentOfBudget,BudgetRemaining,MonthlyTonnage,Scenario1,Scenario2,Scenario3,RequiredRate");

                    foreach (var district in trashData)
                    {
                        writer.WriteLine($"{district.Account},{district.Label},{district.Section},{district.CurrentFYBudget:F2},{district.MonthlyInput:F2},{district.YearToDateSpending:F2},{district.PercentOfBudget:F4},{district.BudgetRemaining:F2},{district.MonthlyUsage:F1},{district.Scenario1:F2},{district.Scenario2:F2},{district.Scenario3:F2},{district.RequiredRate:F2}");
                    }
                }

                statusLabel.Text = "Trash data exported successfully";
                statusLabel.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting trash data: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void RecalculateAllTrash()
        {
            try
            {
                foreach (var district in trashData)
                {
                    CalculateFields(district);
                }

                RefreshGrid();
                statusLabel.Text = "All trash calculations refreshed";
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
