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
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace WileyBudgetManagement.Forms
{
    public class WaterValidationResult
    {
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public int TotalTests { get; set; }
        public int PassedTests { get; set; }
        public bool IsValid => !Errors.Any();
    }

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

        // Chart controls
        private TabControl chartTabControl = null!;
        private CartesianChart budgetVsActualChart = null!;
        private CartesianChart scenarioComparisonChart = null!;
        private PieChart sectionDistributionChart = null!;
        private CartesianChart monthlyTrendChart = null!;
        private CartesianChart usageVsRevenueChart = null!;
        private Button refreshChartsButton = null!;
        private Panel chartPanel = null!;

        public WaterInput()
        {
            _databaseManager = new DatabaseManager();
            _repository = new SanitationRepository(_databaseManager);

            InitializeComponent();

            // Apply modern UI styling
            this.ApplyModernStyling();

            InitializeControls();
            InitializeWaterDataGrid();
            InitializeCharts();
            LoadWaterData();
            SetupValidation();
        }

        public int CountTotalValidationRules()
        {
            // This method provides a static count of all possible validation checks
            // for a single row and the global checks.
            int perRowRules = 0;

            // From ValidationHelper.ValidateSanitationDistrict("Water")
            perRowRules += 10; // Account, Label, Section, Budget, MonthlyInput, YTD, %Budget, Usage, Rate, SeasonalAdj

            // From ValidationHelper.ValidateBusinessRules
            perRowRules += 3; // BudgetRemaining, %Budget calc, MonthlyInput variance

            // From ValidateWaterSpecificRules
            perRowRules += 6; // Account prefix, Section name, Usage negative, Usage high, Seasonal factor, Quality budget

            return perRowRules;
        }

        public WaterValidationResult GetValidationResults()
        {
            var result = new WaterValidationResult();
            try
            {
                int perRowRuleCount = CountTotalValidationRules();
                int globalRuleCount = 3; // Budget Imbalance, Infrastructure allocation, Quality allocation
                int totalPossibleTests = waterData.Count * perRowRuleCount + globalRuleCount;
                int passedTests = 0;

                for (int i = 0; i < waterData.Count; i++)
                {
                    var district = waterData[i];
                    CalculateFields(district);

                    var rowPrefix = $"Row {i + 1} ({district.Account}): ";
                    int initialErrorCount = result.Errors.Count;
                    int initialWarningCount = result.Warnings.Count;

                    // Use comprehensive validation helper
                    var fieldValidation = ValidationHelper.ValidateSanitationDistrict(district, "Water");
                    var businessValidation = ValidationHelper.ValidateBusinessRules(district);

                    result.Errors.AddRange(fieldValidation.Errors.Select(e => $"{rowPrefix}{e.Message}"));
                    result.Warnings.AddRange(fieldValidation.Warnings.Select(w => $"{rowPrefix}{w.Message}"));
                    result.Errors.AddRange(businessValidation.Errors.Select(e => $"{rowPrefix}{e.Message}"));
                    result.Warnings.AddRange(businessValidation.Warnings.Select(w => $"{rowPrefix}{w.Message}"));

                    // Water-specific validations
                    ValidateWaterSpecificRules(district, rowPrefix, result.Errors, result.Warnings);

                    int finalErrorCount = result.Errors.Count;
                    int finalWarningCount = result.Warnings.Count;
                    int rulesFailedThisRow = (finalErrorCount - initialErrorCount) + (finalWarningCount - initialWarningCount);
                    passedTests += (perRowRuleCount - rulesFailedThisRow);
                }

                // Global validations
                int initialGlobalWarningCount = result.Warnings.Count;
                ValidateWaterGlobalRules(result.Warnings);
                int finalGlobalWarningCount = result.Warnings.Count;
                int globalRulesFailed = finalGlobalWarningCount - initialGlobalWarningCount;
                passedTests += (globalRuleCount - globalRulesFailed);

                result.TotalTests = totalPossibleTests;
                result.PassedTests = passedTests;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"An unexpected error occurred during validation: {ex.Message}");
            }
            return result;
        }

        private void InitializeControls()
        {
            UIStyleManager.ApplyFormStyle(this, "Water District - Revenue & Expenses");
            this.Size = new Size(1600, 1000); // Increased size for charts

            // Create modern toolbar panel
            var toolbarPanel = new Panel();
            UIStyleManager.ApplyToolbarPanelStyle(toolbarPanel);

            // Enhanced Save button with success styling
            saveButton = new Button
            {
                Text = "💾 Save & Validate",
                Location = new Point(16, 12)
            };
            UIStyleManager.ApplySuccessButtonStyle(saveButton);
            saveButton.Click += SaveButton_Click;

            // Enhanced Add Row button
            addRowButton = new Button
            {
                Text = "➕ Add Row",
                Location = new Point(152, 12)
            };
            UIStyleManager.ApplyPrimaryButtonStyle(addRowButton);
            addRowButton.Click += AddRowButton_Click;

            // Enhanced Delete Row button
            deleteRowButton = new Button
            {
                Text = "🗑️ Delete Row",
                Location = new Point(268, 12)
            };
            UIStyleManager.ApplyWarningButtonStyle(deleteRowButton);
            deleteRowButton.Click += DeleteRowButton_Click;

            // Enhanced Refresh Charts button
            refreshChartsButton = new Button
            {
                Text = "📊 Refresh Charts",
                Location = new Point(384, 12)
            };
            UIStyleManager.ApplySecondaryButtonStyle(refreshChartsButton);
            refreshChartsButton.Click += RefreshChartsButton_Click;

            // Modern section filter
            var filterLabel = new Label
            {
                Text = "Filter by Section:",
                Location = new Point(520, 18),
                Size = new Size(100, 20)
            };
            UIStyleManager.ApplyBodyLabelStyle(filterLabel);

            sectionFilterCombo = new ComboBox
            {
                Size = new Size(150, 28),
                Location = new Point(630, 15),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            UIStyleManager.ApplyComboBoxStyle(sectionFilterCombo);
            sectionFilterCombo.Items.AddRange(new[] { "All", "Revenue", "Operating", "Infrastructure", "Quality" });
            sectionFilterCombo.SelectedIndex = 0;
            sectionFilterCombo.SelectedIndexChanged += SectionFilterCombo_SelectedIndexChanged;

            // Enhanced status label
            statusLabel = new Label
            {
                Text = "💧 Water District Ready",
                Location = new Point(800, 18),
                Size = new Size(300, 20)
            };
            UIStyleManager.ApplyStatusLabelStyle(statusLabel, UIStyleManager.StatusType.Success);

            toolbarPanel.Controls.AddRange(new Control[] {
                saveButton, addRowButton, deleteRowButton, refreshChartsButton, filterLabel, sectionFilterCombo, statusLabel
            });

            this.Controls.Add(toolbarPanel);

            // Create main split container for data grid and charts
            var mainSplitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                Panel1MinSize = 200,
                Panel2MinSize = 150,
                // Set SplitterDistance after the container is added and sized
                SplitterWidth = 4
            };

            this.Controls.Add(mainSplitContainer);

            // Set splitter distance safely after the container is added
            this.Load += (s, e) =>
            {
                // Calculate a safe splitter distance: 60% of available height, but respect min sizes
                var availableHeight = mainSplitContainer.Height;
                var minTotalHeight = mainSplitContainer.Panel1MinSize + mainSplitContainer.Panel2MinSize + mainSplitContainer.SplitterWidth;

                if (availableHeight > minTotalHeight)
                {
                    var proposedDistance = (int)(availableHeight * 0.6);
                    var maxDistance = availableHeight - mainSplitContainer.Panel2MinSize - mainSplitContainer.SplitterWidth;
                    var minDistance = mainSplitContainer.Panel1MinSize;

                    // Ensure the distance is within valid bounds
                    proposedDistance = Math.Max(minDistance, Math.Min(proposedDistance, maxDistance));
                    mainSplitContainer.SplitterDistance = proposedDistance;
                }
                else
                {
                    // Fallback: set to minimum for Panel1
                    mainSplitContainer.SplitterDistance = mainSplitContainer.Panel1MinSize;
                }
            };

            // Chart panel for the bottom section
            chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            mainSplitContainer.Panel2.Controls.Add(chartPanel);
        }

        private void InitializeWaterDataGrid()
        {
            // Get the split container from the parent controls
            var mainSplitContainer = this.Controls.OfType<SplitContainer>().FirstOrDefault();
            if (mainSplitContainer == null) return;

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

            mainSplitContainer.Panel1.Controls.Add(waterDataGrid);
        }

        private void InitializeCharts()
        {
            // Create tab control for different chart views
            chartTabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };

            // Budget vs Actual Chart Tab
            var budgetTab = new TabPage("Budget vs Actual");
            budgetVsActualChart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            budgetTab.Controls.Add(budgetVsActualChart);
            chartTabControl.TabPages.Add(budgetTab);

            // Scenario Comparison Chart Tab
            var scenarioTab = new TabPage("Scenario Analysis");
            scenarioComparisonChart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            scenarioTab.Controls.Add(scenarioComparisonChart);
            chartTabControl.TabPages.Add(scenarioTab);

            // Section Distribution Chart Tab
            var distributionTab = new TabPage("Section Distribution");
            sectionDistributionChart = new PieChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            distributionTab.Controls.Add(sectionDistributionChart);
            chartTabControl.TabPages.Add(distributionTab);

            // Monthly Trend Chart Tab
            var trendTab = new TabPage("Monthly Trends");
            monthlyTrendChart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            trendTab.Controls.Add(monthlyTrendChart);
            chartTabControl.TabPages.Add(trendTab);

            // Usage vs Revenue Chart Tab
            var usageTab = new TabPage("Usage vs Revenue");
            usageVsRevenueChart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            usageTab.Controls.Add(usageVsRevenueChart);
            chartTabControl.TabPages.Add(usageTab);

            chartPanel.Controls.Add(chartTabControl);

            // Initial chart setup
            SetupBudgetVsActualChart();
            SetupScenarioComparisonChart();
            SetupSectionDistributionChart();
            SetupMonthlyTrendChart();
            SetupUsageVsRevenueChart();
        }

        private void SetupBudgetVsActualChart()
        {
            var budgetSeries = new ColumnSeries<decimal>
            {
                Name = "Budget",
                Values = new List<decimal>(),
                Fill = new SolidColorPaint(SKColors.SteelBlue)
            };

            var actualSeries = new ColumnSeries<decimal>
            {
                Name = "YTD Spending",
                Values = new List<decimal>(),
                Fill = new SolidColorPaint(SKColors.Orange)
            };

            budgetVsActualChart.Series = new ISeries[] { budgetSeries, actualSeries };

            budgetVsActualChart.XAxes = new List<LiveChartsCore.Kernel.Sketches.ICartesianAxis>
            {
                new Axis
                {
                    Name = "Account",
                    Labels = new List<string>()
                }
            };

            budgetVsActualChart.YAxes = new List<LiveChartsCore.Kernel.Sketches.ICartesianAxis>
            {
                new Axis
                {
                    Name = "Amount ($)",
                    Labeler = value => value.ToString("C0")
                }
            };
        }

        private void SetupScenarioComparisonChart()
        {
            var scenario1Series = new ColumnSeries<decimal>
            {
                Name = "Scenario 1 (Treatment Plant)",
                Values = new List<decimal>(),
                Fill = new SolidColorPaint(SkiaSharp.SKColors.Green)
            };

            var scenario2Series = new ColumnSeries<decimal>
            {
                Name = "Scenario 2 (Pipeline)",
                Values = new List<decimal>(),
                Fill = new SolidColorPaint(SkiaSharp.SKColors.Blue)
            };

            var scenario3Series = new ColumnSeries<decimal>
            {
                Name = "Scenario 3 (Quality)",
                Values = new List<decimal>(),
                Fill = new SolidColorPaint(SkiaSharp.SKColors.Purple)
            };

            scenarioComparisonChart.Series = new ISeries[] { scenario1Series, scenario2Series, scenario3Series };

            scenarioComparisonChart.XAxes = new List<LiveChartsCore.Kernel.Sketches.ICartesianAxis>
            {
                new Axis
                {
                    Name = "Account",
                    Labels = new List<string>()
                }
            };

            scenarioComparisonChart.YAxes = new List<LiveChartsCore.Kernel.Sketches.ICartesianAxis>
            {
                new Axis
                {
                    Name = "Monthly Cost ($)",
                    Labeler = value => value.ToString("C0")
                }
            };
        }

        private void SetupSectionDistributionChart()
        {
            var revenueSeries = new PieSeries<decimal>
            {
                Name = "Revenue",
                Values = new List<decimal> { 0 },
                Fill = new SolidColorPaint(SkiaSharp.SKColors.LightGreen)
            };

            var operatingSeries = new PieSeries<decimal>
            {
                Name = "Operating",
                Values = new List<decimal> { 0 },
                Fill = new SolidColorPaint(SkiaSharp.SKColors.LightBlue)
            };

            var infrastructureSeries = new PieSeries<decimal>
            {
                Name = "Infrastructure",
                Values = new List<decimal> { 0 },
                Fill = new SolidColorPaint(SkiaSharp.SKColors.Orange)
            };

            var qualitySeries = new PieSeries<decimal>
            {
                Name = "Quality",
                Values = new List<decimal> { 0 },
                Fill = new SolidColorPaint(SkiaSharp.SKColors.Red)
            };

            sectionDistributionChart.Series = new ISeries[] { revenueSeries, operatingSeries, infrastructureSeries, qualitySeries };
        }

        private void SetupMonthlyTrendChart()
        {
            var monthlyInputSeries = new LineSeries<decimal>
            {
                Name = "Monthly Input",
                Values = new List<decimal>(),
                Fill = null,
                Stroke = new SolidColorPaint(SkiaSharp.SKColors.Blue) { StrokeThickness = 3 },
                GeometryFill = new SolidColorPaint(SkiaSharp.SKColors.Blue),
                GeometryStroke = new SolidColorPaint(SkiaSharp.SKColors.White) { StrokeThickness = 2 }
            };

            var requiredRateSeries = new LineSeries<decimal>
            {
                Name = "Required Rate",
                Values = new List<decimal>(),
                Fill = null,
                Stroke = new SolidColorPaint(SkiaSharp.SKColors.Red) { StrokeThickness = 3 },
                GeometryFill = new SolidColorPaint(SkiaSharp.SKColors.Red),
                GeometryStroke = new SolidColorPaint(SkiaSharp.SKColors.White) { StrokeThickness = 2 }
            };

            monthlyTrendChart.Series = new ISeries[] { monthlyInputSeries, requiredRateSeries };

            monthlyTrendChart.XAxes = new List<LiveChartsCore.Kernel.Sketches.ICartesianAxis>
            {
                new Axis
                {
                    Name = "Account",
                    Labels = new List<string>()
                }
            };

            monthlyTrendChart.YAxes = new List<LiveChartsCore.Kernel.Sketches.ICartesianAxis>
            {
                new Axis
                {
                    Name = "Amount ($)",
                    Labeler = value => value.ToString("C0")
                }
            };
        }

        private void SetupUsageVsRevenueChart()
        {
            var usagePoints = new List<ObservablePoint>();
            var scatterSeries = new ScatterSeries<ObservablePoint>
            {
                Name = "Usage vs Revenue",
                Values = usagePoints,
                Fill = new SolidColorPaint(SkiaSharp.SKColors.SteelBlue),
                Stroke = new SolidColorPaint(SkiaSharp.SKColors.DarkBlue) { StrokeThickness = 2 }
            };

            usageVsRevenueChart.Series = new ISeries[] { scatterSeries };

            usageVsRevenueChart.XAxes = new List<LiveChartsCore.Kernel.Sketches.ICartesianAxis>
            {
                new Axis
                {
                    Name = "Monthly Usage (Gallons)",
                    Labeler = value => (value / 1000000).ToString("N1") + "M"
                }
            };

            usageVsRevenueChart.YAxes = new List<LiveChartsCore.Kernel.Sketches.ICartesianAxis>
            {
                new Axis
                {
                    Name = "Monthly Revenue ($)",
                    Labeler = value => value.ToString("C0")
                }
            };
        }

        private void RefreshChartsButton_Click(object? sender, EventArgs e)
        {
            RefreshAllCharts();
            statusLabel.Text = "Charts refreshed";
            statusLabel.ForeColor = Color.DarkGreen;
        }

        private void RefreshAllCharts()
        {
            if (waterData == null || !waterData.Any()) return;

            try
            {
                RefreshBudgetVsActualChart();
                RefreshScenarioComparisonChart();
                RefreshSectionDistributionChart();
                RefreshMonthlyTrendChart();
                RefreshUsageVsRevenueChart();
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Chart refresh error: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
            }
        }

        private void RefreshBudgetVsActualChart()
        {
            var budgetValues = waterData.Select(d => d.CurrentFYBudget).ToList();
            var actualValues = waterData.Select(d => d.YearToDateSpending).ToList();
            var labels = waterData.Select(d => d.Account).ToList();

            var budgetSeries = new ColumnSeries<decimal>
            {
                Name = "Budget",
                Values = budgetValues,
                Fill = new SolidColorPaint(SkiaSharp.SKColors.SteelBlue)
            };

            var actualSeries = new ColumnSeries<decimal>
            {
                Name = "YTD Spending",
                Values = actualValues,
                Fill = new SolidColorPaint(SkiaSharp.SKColors.Orange)
            };

            budgetVsActualChart.Series = new ISeries[] { budgetSeries, actualSeries };

            if (budgetVsActualChart.XAxes.Any())
            {
                ((Axis)budgetVsActualChart.XAxes.First()).Labels = labels;
            }
        }

        private void RefreshScenarioComparisonChart()
        {
            var scenario1Values = waterData.Select(d => d.Scenario1).ToList();
            var scenario2Values = waterData.Select(d => d.Scenario2).ToList();
            var scenario3Values = waterData.Select(d => d.Scenario3).ToList();
            var labels = waterData.Select(d => d.Account).ToList();

            var scenario1Series = new ColumnSeries<decimal>
            {
                Name = "Scenario 1 (Treatment Plant)",
                Values = scenario1Values,
                Fill = new SolidColorPaint(SkiaSharp.SKColors.Green)
            };

            var scenario2Series = new ColumnSeries<decimal>
            {
                Name = "Scenario 2 (Pipeline)",
                Values = scenario2Values,
                Fill = new SolidColorPaint(SkiaSharp.SKColors.Blue)
            };

            var scenario3Series = new ColumnSeries<decimal>
            {
                Name = "Scenario 3 (Quality)",
                Values = scenario3Values,
                Fill = new SolidColorPaint(SkiaSharp.SKColors.Purple)
            };

            scenarioComparisonChart.Series = new ISeries[] { scenario1Series, scenario2Series, scenario3Series };

            if (scenarioComparisonChart.XAxes.Any())
            {
                ((Axis)scenarioComparisonChart.XAxes.First()).Labels = labels;
            }
        }

        private void RefreshSectionDistributionChart()
        {
            var sectionTotals = waterData
                .GroupBy(d => d.Section)
                .ToDictionary(g => g.Key, g => g.Sum(d => Math.Abs(d.CurrentFYBudget)));

            var revenueSeries = new PieSeries<decimal>
            {
                Name = "Revenue",
                Values = new List<decimal> { sectionTotals.GetValueOrDefault("Revenue", 0) },
                Fill = new SolidColorPaint(SkiaSharp.SKColors.LightGreen)
            };

            var operatingSeries = new PieSeries<decimal>
            {
                Name = "Operating",
                Values = new List<decimal> { sectionTotals.GetValueOrDefault("Operating", 0) },
                Fill = new SolidColorPaint(SkiaSharp.SKColors.LightBlue)
            };

            var infrastructureSeries = new PieSeries<decimal>
            {
                Name = "Infrastructure",
                Values = new List<decimal> { sectionTotals.GetValueOrDefault("Infrastructure", 0) },
                Fill = new SolidColorPaint(SkiaSharp.SKColors.Orange)
            };

            var qualitySeries = new PieSeries<decimal>
            {
                Name = "Quality",
                Values = new List<decimal> { sectionTotals.GetValueOrDefault("Quality", 0) },
                Fill = new SolidColorPaint(SkiaSharp.SKColors.Red)
            };

            sectionDistributionChart.Series = new ISeries[] { revenueSeries, operatingSeries, infrastructureSeries, qualitySeries };
        }

        private void RefreshMonthlyTrendChart()
        {
            var monthlyInputValues = waterData.Select(d => d.MonthlyInput).ToList();
            var requiredRateValues = waterData.Select(d => d.RequiredRate).ToList();
            var labels = waterData.Select(d => d.Account).ToList();

            var monthlyInputSeries = new LineSeries<decimal>
            {
                Name = "Monthly Input",
                Values = monthlyInputValues,
                Fill = null,
                Stroke = new SolidColorPaint(SkiaSharp.SKColors.Blue) { StrokeThickness = 3 },
                GeometryFill = new SolidColorPaint(SkiaSharp.SKColors.Blue),
                GeometryStroke = new SolidColorPaint(SkiaSharp.SKColors.White) { StrokeThickness = 2 }
            };

            var requiredRateSeries = new LineSeries<decimal>
            {
                Name = "Required Rate",
                Values = requiredRateValues,
                Fill = null,
                Stroke = new SolidColorPaint(SkiaSharp.SKColors.Red) { StrokeThickness = 3 },
                GeometryFill = new SolidColorPaint(SkiaSharp.SKColors.Red),
                GeometryStroke = new SolidColorPaint(SkiaSharp.SKColors.White) { StrokeThickness = 2 }
            };

            monthlyTrendChart.Series = new ISeries[] { monthlyInputSeries, requiredRateSeries };

            if (monthlyTrendChart.XAxes.Any())
            {
                ((Axis)monthlyTrendChart.XAxes.First()).Labels = labels;
            }
        }

        private void RefreshUsageVsRevenueChart()
        {
            var usagePoints = waterData
                .Where(d => d.Section == "Revenue" && d.MonthlyUsage > 0)
                .Select(d => new ObservablePoint((double)d.MonthlyUsage, (double)d.MonthlyInput))
                .ToList();

            var scatterSeries = new ScatterSeries<ObservablePoint>
            {
                Name = "Usage vs Revenue",
                Values = usagePoints,
                Fill = new SolidColorPaint(SkiaSharp.SKColors.SteelBlue),
                Stroke = new SolidColorPaint(SkiaSharp.SKColors.DarkBlue) { StrokeThickness = 2 }
            };

            usageVsRevenueChart.Series = new ISeries[] { scatterSeries };
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
                    RefreshAllCharts(); // Update charts when data changes
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
            // Use the enhanced Water Scenario Calculator
            var waterList = new List<SanitationDistrict> { district };
            WaterScenarioCalculator.CalculateWaterScenarios(waterList);

            // The scenarios are now calculated by the dedicated calculator
            // Apply any additional local adjustments if needed

            // Ensure minimum values
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
            // Use the enhanced Water Scenario Calculator for rate calculations
            var totalExpenses = GetTotalExpenses();
            var totalRevenue = GetTotalRevenue();

            district.RequiredRate = WaterScenarioCalculator.CalculateWaterRequiredRate(district, totalExpenses, totalRevenue);
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
            decimal totalBudget = totalExpenses; // Use total expenses as the base for allocation percentage

            if (totalBudget > 0 && (infrastructureCosts / totalBudget) < 0.15m)
            {
                warnings.Add("Infrastructure allocation may be too low (recommended: at least 15% of total budget)");
            }

            // Check quality assurance allocation
            decimal qualityCosts = waterData?.Where(d => d.Section == "Quality").Sum(d => d.CurrentFYBudget) ?? 0;

            if (totalBudget > 0 && (qualityCosts / totalBudget) < 0.05m)
            {
                warnings.Add("Quality assurance allocation may be too low (recommended: at least 5% of total budget)");
            }
        }

        private void ValidateAllData()
        {
            var validationResult = GetValidationResults();
            DisplayValidationResults(validationResult.Errors, validationResult.Warnings);
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

        private void LoadWaterData()
        {
            try
            {
                // Initialize with predefined water district data
                waterData = GetDefaultWaterData();
                waterDataGrid.DataSource = waterData;

                // Calculate all fields to ensure charts have data
                foreach (var district in waterData)
                {
                    CalculateFields(district);
                }

                // Initial chart refresh
                RefreshAllCharts();

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
                stats["AverageRequiredRate"] = waterData?.Any() == true ? waterData.Average(d => d.RequiredRate) : 0;
                stats["TotalYTDSpending"] = waterData?.Sum(d => d.YearToDateSpending) ?? 0;
                stats["TotalMonthlyUsage"] = waterData?.Sum(d => d.MonthlyUsage) ?? 0;
                stats["InfrastructureInvestment"] = waterData?.Where(d => d.Section == "Infrastructure").Sum(d => d.CurrentFYBudget) ?? 0;
                stats["QualityInvestment"] = waterData?.Where(d => d.Section == "Quality").Sum(d => d.CurrentFYBudget) ?? 0;
            }
            catch (Exception)
            {
                stats.Clear();
            }

            return stats;
        }

        public void ExportChartSummary(string filePath)
        {
            try
            {
                using (var writer = new System.IO.StreamWriter(filePath))
                {
                    writer.WriteLine("=== Water District Chart Data Summary ===");
                    writer.WriteLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    writer.WriteLine();

                    // Summary statistics
                    var stats = GetWaterSummaryStatistics();
                    writer.WriteLine("FINANCIAL SUMMARY:");
                    foreach (var stat in stats)
                    {
                        writer.WriteLine($"{stat.Key}: {stat.Value:C}");
                    }
                    writer.WriteLine();

                    // Section breakdown
                    writer.WriteLine("SECTION BREAKDOWN:");
                    var sectionSummary = waterData.GroupBy(d => d.Section)
                        .Select(g => new
                        {
                            Section = g.Key,
                            Count = g.Count(),
                            TotalBudget = g.Sum(d => d.CurrentFYBudget),
                            TotalYTD = g.Sum(d => d.YearToDateSpending),
                            AvgScenario1 = g.Average(d => d.Scenario1),
                            AvgScenario2 = g.Average(d => d.Scenario2),
                            AvgScenario3 = g.Average(d => d.Scenario3)
                        });

                    foreach (var section in sectionSummary)
                    {
                        writer.WriteLine($"{section.Section}:");
                        writer.WriteLine($"  Items: {section.Count}");
                        writer.WriteLine($"  Total Budget: {section.TotalBudget:C}");
                        writer.WriteLine($"  YTD Spending: {section.TotalYTD:C}");
                        writer.WriteLine($"  Avg Scenario 1: {section.AvgScenario1:C}");
                        writer.WriteLine($"  Avg Scenario 2: {section.AvgScenario2:C}");
                        writer.WriteLine($"  Avg Scenario 3: {section.AvgScenario3:C}");
                        writer.WriteLine();
                    }

                    // Chart data points
                    writer.WriteLine("DETAILED CHART DATA:");
                    writer.WriteLine("Account,Section,Budget,YTD,Scenario1,Scenario2,Scenario3,RequiredRate,MonthlyUsage");
                    foreach (var district in waterData)
                    {
                        writer.WriteLine($"{district.Account},{district.Section},{district.CurrentFYBudget:F2},{district.YearToDateSpending:F2},{district.Scenario1:F2},{district.Scenario2:F2},{district.Scenario3:F2},{district.RequiredRate:F2},{district.MonthlyUsage:F0}");
                    }
                }

                statusLabel.Text = "Chart summary exported successfully";
                statusLabel.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting chart summary: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
