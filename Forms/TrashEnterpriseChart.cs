using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Chart;
using WileyBudgetManagement.Models;

namespace WileyBudgetManagement.Forms
{
    public class TrashEnterpriseChart
    {
        private ChartControl budgetChart;
        private readonly Panel containerPanel;
        private readonly BindingList<SanitationDistrict> trashData;
        private string currentChartType = "Budget";

        public TrashEnterpriseChart(Panel container, BindingList<SanitationDistrict> data)
        {
            containerPanel = container;
            trashData = data;
            InitializeChart();
        }

        private void InitializeChart()
        {
            try
            {
                DebugHelper.LogAction("Initializing Trash Enterprise Chart");

                budgetChart = new ChartControl();
                budgetChart.Dock = DockStyle.Fill;
                budgetChart.ChartArea.BorderColor = Color.LightGray;
                budgetChart.ChartArea.BorderWidth = 1;

                budgetChart.PrimaryXAxis.Title = "Accounts";
                budgetChart.PrimaryXAxis.TitleFont = new Font("Segoe UI", 10, FontStyle.Regular);
                budgetChart.PrimaryXAxis.LabelFont = new Font("Segoe UI", 9, FontStyle.Regular);
                budgetChart.PrimaryXAxis.LabelRotationAngle = -45;

                budgetChart.PrimaryYAxis.Title = "Amount ($)";
                budgetChart.PrimaryYAxis.TitleFont = new Font("Segoe UI", 10, FontStyle.Regular);
                budgetChart.PrimaryYAxis.LabelFont = new Font("Segoe UI", 9, FontStyle.Regular);
                budgetChart.PrimaryYAxis.LabelFormat = "C0";

                budgetChart.Legend.Visible = true;
                budgetChart.Legend.Position = ChartDock.Bottom;
                budgetChart.Legend.Font = new Font("Segoe UI", 9, FontStyle.Regular);

                budgetChart.Title.Text = "Trash Enterprise Budget Analysis";
                budgetChart.Title.Font = new Font("Segoe UI", 12, FontStyle.Bold);

                // Create default chart
                UpdateChartData("Budget");

                containerPanel.Controls.Add(budgetChart);
                DebugHelper.LogAction("Trash Enterprise Chart initialized successfully");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "InitializeChart in TrashEnterpriseChart");

                // Create a placeholder with error message
                var errorLabel = new Label
                {
                    Text = $"Chart error: {ex.Message}\nPlease check that Syncfusion controls are properly installed.",
                    Dock = DockStyle.Fill,
                    ForeColor = Color.Red,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular)
                };
                containerPanel.Controls.Add(errorLabel);
            }
        }

        public void UpdateChartData(string chartType)
        {
            try
            {
                DebugHelper.LogAction($"Updating Trash Enterprise Chart: {chartType}");
                currentChartType = chartType;
                budgetChart.Series.Clear();

                switch (chartType)
                {
                    case "Budget":
                        CreateBudgetChart();
                        break;
                    case "YTD":
                        CreateYTDChart();
                        break;
                    case "Scenarios":
                        CreateScenariosChart();
                        break;
                    case "Rates":
                        CreateRatesChart();
                        break;
                    case "Tonnage":
                        CreateTonnageChart();
                        break;
                    default:
                        CreateBudgetChart();
                        break;
                }

                budgetChart.Refresh();
                DebugHelper.LogAction($"Trash Enterprise Chart updated successfully: {chartType}");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, $"UpdateChartData({chartType}) in TrashEnterpriseChart");
            }
        }

        private void CreateBudgetChart()
        {
            // Group data by section for better visualization
            var sectionGroups = trashData
                .GroupBy(d => d.Section)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .OrderBy(g => g.Key)
                .ToList();

            // Create column series for current budget
            var budgetSeries = new ChartSeries();
            budgetSeries.Name = "Current FY Budget";
            budgetSeries.Type = ChartSeriesType.Column;
            budgetSeries.Style.Interior = new BrushInfo(Color.FromArgb(41, 121, 255));

            // Create column series for YTD spending
            var ytdSeries = new ChartSeries();
            ytdSeries.Name = "YTD Spending";
            ytdSeries.Type = ChartSeriesType.Column;
            ytdSeries.Style.Interior = new BrushInfo(Color.FromArgb(237, 125, 49));

            // Create column series for remaining budget
            var remainingSeries = new ChartSeries();
            remainingSeries.Name = "Budget Remaining";
            remainingSeries.Type = ChartSeriesType.Column;
            remainingSeries.Style.Interior = new BrushInfo(Color.FromArgb(91, 155, 213));

            // Add data points for each section's total
            foreach (var group in sectionGroups)
            {
                decimal totalBudget = group.Sum(d => d.CurrentFYBudget);
                decimal totalYTD = group.Sum(d => d.YearToDateSpending);
                decimal totalRemaining = group.Sum(d => d.BudgetRemaining);

                budgetSeries.Points.Add(group.Key, (double)totalBudget);
                ytdSeries.Points.Add(group.Key, (double)totalYTD);
                remainingSeries.Points.Add(group.Key, (double)totalRemaining);
            }

            budgetChart.Series.Add(budgetSeries);
            budgetChart.Series.Add(ytdSeries);
            budgetChart.Series.Add(remainingSeries);

            budgetChart.Title.Text = "Trash Enterprise Budget by Section";
        }

        private void CreateYTDChart()
        {
            // Create pie chart for YTD spending by section
            var ytdSeries = new ChartSeries();
            ytdSeries.Name = "YTD Spending";
            ytdSeries.Type = ChartSeriesType.Pie;
            ytdSeries.Style.DisplayText = true;
            ytdSeries.Style.TextFormat = "{0}: {2:P1}";

            // Group data by section
            var sectionGroups = trashData
                .GroupBy(d => d.Section)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .OrderBy(g => g.Key)
                .ToList();

            // Color palette for sections
            Color[] colors = new Color[] {
                Color.FromArgb(41, 121, 255),    // Blue
                Color.FromArgb(237, 125, 49),    // Orange
                Color.FromArgb(91, 155, 213),    // Light Blue
                Color.FromArgb(112, 173, 71),    // Green
                Color.FromArgb(255, 192, 0)      // Yellow
            };

            int colorIndex = 0;
            foreach (var group in sectionGroups)
            {
                decimal totalYTD = group.Sum(d => d.YearToDateSpending);

                ChartPoint point = new ChartPoint(group.Key, (double)totalYTD);
                point.Interior = new BrushInfo(colors[colorIndex % colors.Length]);
                ytdSeries.Points.Add(point);

                colorIndex++;
            }

            budgetChart.Series.Add(ytdSeries);
            budgetChart.Title.Text = "Trash Enterprise YTD Spending by Section";
        }

        private void CreateScenariosChart()
        {
            // Group data by section for scenarios
            var sectionGroups = trashData
                .GroupBy(d => d.Section)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .OrderBy(g => g.Key)
                .ToList();

            // Create column series for each scenario
            var scenario1Series = new ChartSeries();
            scenario1Series.Name = "Scenario 1 (New Truck)";
            scenario1Series.Type = ChartSeriesType.Column;
            scenario1Series.Style.Interior = new BrushInfo(Color.FromArgb(41, 121, 255));

            var scenario2Series = new ChartSeries();
            scenario2Series.Name = "Scenario 2 (Recycling)";
            scenario2Series.Type = ChartSeriesType.Column;
            scenario2Series.Style.Interior = new BrushInfo(Color.FromArgb(112, 173, 71));

            var scenario3Series = new ChartSeries();
            scenario3Series.Name = "Scenario 3 (Route Opt)";
            scenario3Series.Type = ChartSeriesType.Column;
            scenario3Series.Style.Interior = new BrushInfo(Color.FromArgb(237, 125, 49));

            // Add data points for each section's scenario totals
            foreach (var group in sectionGroups)
            {
                decimal totalScenario1 = group.Sum(d => d.Scenario1);
                decimal totalScenario2 = group.Sum(d => d.Scenario2);
                decimal totalScenario3 = group.Sum(d => d.Scenario3);

                scenario1Series.Points.Add(group.Key, (double)totalScenario1);
                scenario2Series.Points.Add(group.Key, (double)totalScenario2);
                scenario3Series.Points.Add(group.Key, (double)totalScenario3);
            }

            budgetChart.Series.Add(scenario1Series);
            budgetChart.Series.Add(scenario2Series);
            budgetChart.Series.Add(scenario3Series);

            budgetChart.Title.Text = "Trash Enterprise Scenarios by Section";
        }

        private void CreateRatesChart()
        {
            // Only include items with required rates
            var rateItems = trashData
                .Where(d => d.RequiredRate > 0)
                .OrderBy(d => d.Section)
                .ThenBy(d => d.RequiredRate)
                .Take(10) // Limit to top 10 for readability
                .ToList();

            // Create column series for required rates
            var ratesSeries = new ChartSeries();
            ratesSeries.Name = "Required Rate";
            ratesSeries.Type = ChartSeriesType.Column;
            ratesSeries.Style.Interior = new BrushInfo(Color.FromArgb(112, 173, 71));

            // Add data points for each item
            foreach (var item in rateItems)
            {
                string label = item.Label.Length > 15 ? item.Label.Substring(0, 15) + "..." : item.Label;
                ratesSeries.Points.Add(label, (double)item.RequiredRate);
            }

            budgetChart.Series.Add(ratesSeries);
            budgetChart.Title.Text = "Trash Enterprise Required Rates";
        }

        private void CreateTonnageChart()
        {
            // Only include items with monthly usage (tonnage)
            var tonnageItems = trashData
                .Where(d => d.MonthlyUsage > 0)
                .OrderByDescending(d => d.MonthlyUsage)
                .Take(10) // Limit to top 10 for readability
                .ToList();

            // Create column series for tonnage
            var tonnageSeries = new ChartSeries();
            tonnageSeries.Name = "Monthly Tonnage";
            tonnageSeries.Type = ChartSeriesType.Column;
            tonnageSeries.Style.Interior = new BrushInfo(Color.FromArgb(91, 155, 213));

            // Add data points for each item
            foreach (var item in tonnageItems)
            {
                string label = item.Label.Length > 15 ? item.Label.Substring(0, 15) + "..." : item.Label;
                tonnageSeries.Points.Add(label, (double)item.MonthlyUsage);
            }

            // Create line series for cost per ton
            var costPerTonSeries = new ChartSeries();
            costPerTonSeries.Name = "Cost Per Ton";
            costPerTonSeries.Type = ChartSeriesType.Line;
            costPerTonSeries.Style.Interior = new BrushInfo(Color.FromArgb(237, 125, 49));

            // Add cost per ton data points
            foreach (var item in tonnageItems)
            {
                string label = item.Label.Length > 15 ? item.Label.Substring(0, 15) + "..." : item.Label;
                decimal costPerTon = item.MonthlyUsage > 0 ? item.MonthlyInput / item.MonthlyUsage : 0;
                costPerTonSeries.Points.Add(label, (double)costPerTon);
            }

            budgetChart.Series.Add(tonnageSeries);
            budgetChart.Series.Add(costPerTonSeries);
            budgetChart.Title.Text = "Trash Enterprise Monthly Tonnage";
        }

        public void Refresh()
        {
            UpdateChartData(currentChartType);
        }
    }
}
