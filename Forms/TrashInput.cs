using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using Syncfusion.WinForms.DataGrid.Events;
using Syncfusion.Data;
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Database;
using WileyBudgetManagement;

namespace WileyBudgetManagement.Forms
{
    public class TrashValidationResult
    {
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public int TotalTests { get; set; }
        public int PassedTests { get; set; }
    }

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
        private Panel chartPanel = null!;
        private string currentChartType = "Budget";
        private ComboBox chartTypeCombo = null!;
        private SplitContainer mainSplitContainer = null!;

        public TrashInput()
        {
            try
            {
                DebugHelper.LogAction("TrashInput constructor starting");
                _databaseManager = new DatabaseManager();
                _repository = new SanitationRepository(_databaseManager);

                InitializeComponent();
                DebugHelper.LogAction("InitializeComponent completed");

                InitializeControls();
                DebugHelper.LogAction("InitializeControls completed");

                this.Load += TrashInput_Load;

                InitializeTrashDataGrid();
                DebugHelper.LogAction("InitializeTrashDataGrid completed");

                LoadTrashDataAsync();
                DebugHelper.LogAction("LoadTrashDataAsync completed");

                SetupValidation();
                DebugHelper.LogAction("SetupValidation completed");

                DebugHelper.LogAction("TrashInput constructor completed successfully");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "TrashInput constructor");
                // Re-throw the exception to be caught by the TestRunner
                // This prevents a blocking MessageBox from freezing the script.
                throw;
            }
        }

        private void InitializeControls()
        {
            try
            {
                DebugHelper.LogAction("TrashInput.InitializeControls starting");

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

                // Chart type selector
                var chartLabel = new Label
                {
                    Text = "Chart Type:",
                    Location = new Point(620, 15),
                    Size = new Size(80, 20)
                };

                chartTypeCombo = new ComboBox
                {
                    Size = new Size(150, 25),
                    Location = new Point(700, 12),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                chartTypeCombo.Items.AddRange(new[] { "Budget", "YTD", "Scenarios", "Rates", "Tonnage" });
                chartTypeCombo.SelectedIndex = 0;
                chartTypeCombo.SelectedIndexChanged += ChartTypeCombo_SelectedIndexChanged;

                // Status label
                statusLabel = new Label
                {
                    Text = "Ready",
                    Location = new Point(870, 15),
                    Size = new Size(300, 20),
                    ForeColor = Color.DarkGreen
                };

                // Add recalculate button
                var recalculateButton = new Button
                {
                    Text = "Recalculate All",
                    Size = new Size(120, 30),
                    Location = new Point(1180, 10),
                    BackColor = Color.LightSkyBlue
                };
                recalculateButton.Click += (s, e) =>
                {
                    try
                    {
                        DebugHelper.LogAction("RecalculateAll button clicked");
                        RecalculateAllTrash();
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.LogError(ex, "RecalculateAll button click");
                    }
                };

                toolbarPanel.Controls.AddRange(new Control[] {
                    saveButton, addRowButton, deleteRowButton, filterLabel, sectionFilterCombo,
                    chartLabel, chartTypeCombo, statusLabel, recalculateButton
                });

                // Create main split container
                mainSplitContainer = new SplitContainer
                {
                    Dock = DockStyle.Fill,
                    Orientation = Orientation.Vertical
                };

                // Add chart panel to bottom split
                chartPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                mainSplitContainer.Panel2.Controls.Add(chartPanel);

                // Add controls to form
                this.Controls.Add(mainSplitContainer);
                this.Controls.Add(toolbarPanel);

                DebugHelper.LogAction("TrashInput.InitializeControls completed successfully");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "TrashInput.InitializeControls");
                throw; // Re-throw to let the constructor handle it
            }
        }

        private void TrashInput_Load(object? sender, EventArgs e)
        {
            if (mainSplitContainer != null)
            {
                mainSplitContainer.Panel1MinSize = 200;
                mainSplitContainer.Panel2MinSize = 200;
                if (mainSplitContainer.Width > mainSplitContainer.Panel1MinSize + mainSplitContainer.Panel2MinSize)
                {
                    mainSplitContainer.SplitterDistance = 500;
                }
            }
        }

        private void ChartTypeCombo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            try
            {
                if (chartTypeCombo.SelectedItem != null)
                {
                    DebugHelper.LogAction($"Changing chart type to: {chartTypeCombo.SelectedItem}");
                    currentChartType = chartTypeCombo.SelectedItem.ToString() ?? "Budget";
                    UpdateChartVisualization();
                    statusLabel.Text = $"Chart updated: {chartTypeCombo.SelectedItem}";
                    statusLabel.ForeColor = Color.DarkGreen;
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "ChartTypeCombo_SelectedIndexChanged");
                statusLabel.Text = $"Chart error: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
            }
        }

        private void InitializeChartVisualization()
        {
            // Clear any existing controls from the chart panel
            chartPanel.Controls.Clear();

            // Create a simple visualization label for now
            Label chartLabel = new Label
            {
                Text = $"Trash Enterprise {currentChartType} Chart\n\nChart Visualization Placeholder\n\nActive Data Records: {trashData.Count}\n\nTotals:\nRevenue: ${GetTotalRevenue():N0}\nExpenses: ${GetTotalExpenses():N0}",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.DarkBlue
            };

            chartPanel.Controls.Add(chartLabel);
            DebugHelper.LogAction("Basic chart visualization initialized");
        }

        private void UpdateChartVisualization()
        {
            // Update the chart based on the selected chart type
            InitializeChartVisualization();
            DebugHelper.LogAction($"Chart visualization updated to: {currentChartType}");
        }

        private void InitializeTrashDataGrid()
        {
            try
            {
                DebugHelper.LogAction("TrashInput.InitializeTrashDataGrid starting");

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

                // Add to panel1 of split container
                mainSplitContainer.Panel1.Controls.Add(trashDataGrid);

                DebugHelper.LogAction("TrashInput.InitializeTrashDataGrid completed successfully");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "TrashInput.InitializeTrashDataGrid");
                throw; // Re-throw to let the constructor handle it
            }
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
                DebugHelper.LogError(ex, $"CalculateFields for {district.Account}");
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
                    // The PercentAllocation is already a decimal (e.g., 0.30 for 30%), so no need to divide by 100.
                    district.Scenario1 = baseMonthly * 1.0267m;
                    district.Scenario2 = baseMonthly + (recyclingProgramMonthlyImpact * district.PercentAllocation);
                    district.Scenario3 = baseMonthly + (transferStationMonthlyImpact * district.PercentAllocation);
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
            if (district.Section != "Revenue")
            {
                ApplyFactors(district);
            }

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
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, $"CalculateRequiredRate for {district.Account}");
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

        public int CountTotalValidationRules()
        {
            // This method provides a static count of all possible validation checks
            // for a single row and the global checks.
            int perRowRules = 0;

            // From ValidationHelper.ValidateSanitationDistrict("Trash")
            perRowRules += 10; // Account, Label, Section, Budget, MonthlyInput, YTD, %Budget, Usage, Rate, SeasonalAdj

            // From ValidationHelper.ValidateBusinessRules
            perRowRules += 3; // BudgetRemaining, %Budget calc, MonthlyInput variance

            // From ValidateTrashSpecificRules
            perRowRules += 14; // Account prefix, Collection rate, Recycling efficiency, Depreciation, Maintenance reserve, Seasonal %, Scenario rate, Section name, Tonnage negative, Tonnage high, Collection budget, Equipment reserve, Scenario impact, Recycling tonnage tracking

            int globalRules = 0;
            // From ValidateTrashGlobalRules
            globalRules += 5; // Budget Imbalance, Equipment allocation, Recycling allocation, Cost per ton high, Cost per ton low

            // Total is per-row checks multiplied by row count, plus global checks.
            // We will calculate the final total inside GetValidationResults.
            // This method's purpose is to centralize the rule count for a single entity.
            return perRowRules;
        }

        public TrashValidationResult GetValidationResults()
        {
            var result = new TrashValidationResult();
            try
            {
                DebugHelper.LogAction("GetValidationResults started");

                int perRowRuleCount = CountTotalValidationRules();
                int totalPossibleTests = trashData.Count * perRowRuleCount + 5; // 5 global rules
                int passedTests = 0;

                for (int i = 0; i < trashData.Count; i++)
                {
                    var district = trashData[i];
                    // FIX: Ensure all fields are calculated before they are validated.
                    CalculateFields(district);

                    var rowPrefix = $"Row {i + 1} ({district.Account}): ";
                    int initialErrorCount = result.Errors.Count;
                    int initialWarningCount = result.Warnings.Count;

                    // Use comprehensive validation helper
                    var fieldValidation = ValidationHelper.ValidateSanitationDistrict(district, "Trash");
                    var businessValidation = ValidationHelper.ValidateBusinessRules(district);

                    // Collect validation errors and warnings
                    foreach (var error in fieldValidation.Errors)
                    {
                        result.Errors.Add($"{rowPrefix}{error.Message}");
                    }

                    foreach (var warning in fieldValidation.Warnings)
                    {
                        result.Warnings.Add($"{rowPrefix}{warning.Message}");
                    }

                    foreach (var error in businessValidation.Errors)
                    {
                        result.Errors.Add($"{rowPrefix}{error.Message}");
                    }

                    foreach (var warning in businessValidation.Warnings)
                    {
                        result.Warnings.Add($"{rowPrefix}{warning.Message}");
                    }

                    // Trash-specific validations
                    ValidateTrashSpecificRules(district, rowPrefix, result.Errors, result.Warnings);

                    int finalErrorCount = result.Errors.Count;
                    int finalWarningCount = result.Warnings.Count;
                    int rulesFailedThisRow = (finalErrorCount - initialErrorCount) + (finalWarningCount - initialWarningCount);
                    passedTests += (perRowRuleCount - rulesFailedThisRow);
                }

                // Global validations
                int initialGlobalWarningCount = result.Warnings.Count;
                ValidateTrashGlobalRules(result.Warnings);
                int finalGlobalWarningCount = result.Warnings.Count;
                int globalRulesFailed = finalGlobalWarningCount - initialGlobalWarningCount;
                passedTests += (5 - globalRulesFailed); // 5 global rules

                result.TotalTests = totalPossibleTests;
                result.PassedTests = passedTests;

                DebugHelper.LogAction("GetValidationResults completed");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "GetValidationResults");
                result.Errors.Add($"An unexpected error occurred during validation: {ex.Message}");
            }
            return result;
        }

        public void ValidateAllData()
        {
            try
            {
                DebugHelper.LogAction("ValidateAllData started");
                var validationResult = GetValidationResults();
                DisplayValidationResults(validationResult.Errors, validationResult.Warnings);
                DebugHelper.LogAction("ValidateAllData completed");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "ValidateAllData");
                // Re-throw to be caught by the TestRunner, preventing a blocking MessageBox.
                throw new InvalidOperationException($"An unexpected error occurred during validation: {ex.Message}", ex);
            }
        }

        private void ValidateTrashSpecificRules(SanitationDistrict district, string rowPrefix, List<string> errors, List<string> warnings)
        {
            DebugHelper.LogAction($"ValidateTrashSpecificRules started for {district.Account}");
            try
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
                if (district.Section == "Equipment" && district.Account == "T600.00")
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
                DebugHelper.LogAction($"ValidateTrashSpecificRules completed for {district.Account}");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, $"ValidateTrashSpecificRules for {district.Account}");
                throw;
            }
        }

        private void ValidateTrashGlobalRules(List<string> warnings)
        {
            DebugHelper.LogAction("ValidateTrashGlobalRules started");
            try
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
                // FIX: The total budget for percentage calculations should be based on total expenses, not the sum of revenue and expenses.
                decimal totalBudget = totalExpenses;

                if (totalBudget > 0 && (equipmentCosts / totalBudget) < 0.20m)
                {
                    warnings.Add("Equipment allocation may be too low (recommended: at least 20% of total budget for replacement reserves)");
                }

                // Check recycling allocation
                decimal recyclingCosts = trashData?.Where(d => d.Section == "Recycling").Sum(d => d.CurrentFYBudget) ?? 0;

                if (totalBudget > 0 && (recyclingCosts / totalBudget) < 0.10m)
                {
                    warnings.Add("Recycling allocation may be too low (recommended: at least 10% of total budget)");
                }

                // Check tonnage vs budget alignment
                decimal totalTonnage = trashData?.Sum(d => d.MonthlyUsage) ?? 0;
                if (totalTonnage > 0 && totalExpenses > 0)
                {
                    decimal costPerTon = totalExpenses / (totalTonnage * 12);
                    if (costPerTon > 150)
                    {
                        warnings.Add($"Cost per ton ({costPerTon:C}) seems high - consider efficiency improvements");
                    }
                    else if (costPerTon < 50)
                    {
                        warnings.Add($"Cost per ton ({costPerTon:C}) seems low - verify tonnage data accuracy");
                    }
                }
                DebugHelper.LogAction("ValidateTrashGlobalRules completed");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "ValidateTrashGlobalRules");
                throw;
            }
        }

        private void DisplayValidationResults(List<string> errors, List<string> warnings)
        {
            string results = FormatValidationResults(errors, warnings);

            if (!string.IsNullOrEmpty(results))
            {
                var icon = errors.Any() ? MessageBoxIcon.Error : MessageBoxIcon.Warning;
                var title = errors.Any() ? "Validation Errors" : "Validation Warnings";

                MessageBox.Show(results, title, MessageBoxButtons.OK, icon);

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

        public string FormatValidationResults(List<string> errors, List<string> warnings)
        {
            if (!errors.Any() && !warnings.Any())
            {
                return string.Empty; // Indicates success
            }

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

            return message;
        }

        private void LoadTrashDataAsync()
        {
            try
            {
                DebugHelper.LogAction("TrashInput.LoadTrashDataAsync starting");

                // Initialize with predefined trash district data
                trashData = GetDefaultTrashData();
                trashDataGrid.DataSource = trashData;

                // Initialize a simple chart visualization
                try
                {
                    DebugHelper.LogAction("Initializing chart visualization");
                    InitializeChartVisualization();
                    DebugHelper.LogAction("Chart visualization initialized successfully");
                }
                catch (Exception chartEx)
                {
                    DebugHelper.LogError(chartEx, "Chart visualization initialization");
                    statusLabel.Text = $"Chart initialization error: {chartEx.Message}";
                    statusLabel.ForeColor = Color.Red;
                }

                statusLabel.Text = "Default trash data loaded";
                statusLabel.ForeColor = Color.Blue;

                DebugHelper.LogAction("TrashInput.LoadTrashDataAsync completed successfully");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "TrashInput.LoadTrashDataAsync");

                // Re-throw to be caught by the TestRunner, preventing a blocking MessageBox.
                throw new InvalidOperationException($"Error loading trash data: {ex.Message}", ex);
            }
        }

        private BindingList<SanitationDistrict> GetDefaultTrashData()
        {
            return new BindingList<SanitationDistrict>
            {
                // Revenue
                new SanitationDistrict { Account = "T301.00", Label = "Residential Trash Collection Fees", Section = "Revenue", CurrentFYBudget = 1150000.00m, MonthlyInput = 95833.33m },
                new SanitationDistrict { Account = "T302.00", Label = "Commercial Trash Collection Fees", Section = "Revenue", CurrentFYBudget = 245000.00m, MonthlyInput = 20416.67m },
                new SanitationDistrict { Account = "T303.00", Label = "Other Revenue", Section = "Revenue", CurrentFYBudget = 0.00m, MonthlyInput = 0.00m },

                // Expenses
                new SanitationDistrict { Account = "T401.00", Label = "Salaries and Wages", Section = "Operations", CurrentFYBudget = 500000.00m, MonthlyInput = 41666.67m },
                new SanitationDistrict { Account = "T402.00", Label = "Fuel", Section = "Operations", CurrentFYBudget = 150000.00m, MonthlyInput = 12500.00m },
                new SanitationDistrict { Account = "T403.00", Label = "Maintenance", Section = "Operations", CurrentFYBudget = 100000.00m, MonthlyInput = 8333.33m },
                new SanitationDistrict { Account = "T404.00", Label = "Landfill Fees", Section = "Operations", CurrentFYBudget = 200000.00m, MonthlyInput = 16666.67m, MonthlyUsage = 1100m },
                new SanitationDistrict { Account = "T501.00", Label = "Recycling Collection", Section = "Recycling", CurrentFYBudget = 145000.00m, MonthlyInput = 12083.33m, MonthlyUsage = 62.5m },
                new SanitationDistrict { Account = "T601.00", Label = "Collection Equipment", Section = "Equipment", CurrentFYBudget = 300000.00m, MonthlyInput = 25000.00m, ReserveTarget = 30000m },
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
                // Calculation is now handled by ValidateAllData, which is called before this method.
                // foreach (var district in trashData)
                // {
                //     CalculateFields(district);
                // }

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
                DebugHelper.LogError(ex, "SaveTrashDataAsync");
                // Re-throw to be caught by the TestRunner, preventing a blocking MessageBox.
                throw new InvalidOperationException($"Error saving trash district data: {ex.Message}", ex);
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
                // Re-throw to be caught by the TestRunner, preventing a blocking MessageBox.
                throw new InvalidOperationException($"Error exporting trash data: {ex.Message}", ex);
            }
        }

        private void RecalculateButton_Click(object? sender, EventArgs e)
        {
            try
            {
                DebugHelper.LogAction("TrashInput.RecalculateButton_Click starting");

                // Recalculate all items
                RecalculateAllTrash();

                // Update the chart
                DebugHelper.LogAction("Refreshing chart after recalculation");
                UpdateChartVisualization();

                statusLabel.Text = "All calculations and chart updated";
                statusLabel.ForeColor = Color.DarkGreen;

                DebugHelper.LogAction("TrashInput.RecalculateButton_Click completed successfully");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "TrashInput.RecalculateButton_Click");
                statusLabel.Text = $"Recalculation error: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
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
