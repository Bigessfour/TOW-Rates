using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
// using Syncfusion.WinForms.Charts;  // Commented out until chart package is available
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Database;

namespace WileyBudgetManagement.Reports
{
    public partial class ReportsForm : Form
    {
        private readonly ISanitationRepository _repository;
        private readonly DatabaseManager _databaseManager;
        private SfDataGrid reportDataGrid = null!;
        // private SfCartesianChart trendChart = null!;  // Commented out until chart package is available
        private Panel chartPlaceholder = null!;  // Temporary placeholder for chart
        private ComboBox reportTypeCombo = null!;
        private Button generateReportButton = null!;
        private Button exportCsvButton = null!;
        private Button exportPdfButton = null!;
        private Label statusLabel = null!;
        private Panel chartPanel = null!;
        private Panel gridPanel = null!;

        public ReportsForm()
        {
            _databaseManager = new DatabaseManager();
            _repository = new SanitationRepository(_databaseManager);

            InitializeComponent();
            InitializeControls();
            InitializeDataGrid();
            InitializeChart();
        }

        private void InitializeControls()
        {
            this.Text = "Reports - Export & Analysis";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create toolbar panel
            var toolbarPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.LightGray
            };

            // Report type selection
            var reportLabel = new Label
            {
                Text = "Report Type:",
                Location = new Point(10, 20),
                Size = new Size(80, 20)
            };

            reportTypeCombo = new ComboBox
            {
                Size = new Size(200, 25),
                Location = new Point(100, 17),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            reportTypeCombo.Items.AddRange(new[] {
                "Sanitation District Report",
                "Water Report",
                "Trash Report",
                "Apartments Report",
                "Summary Report",
                "Scenario Comparison Report",
                "Quarterly Summary Report"
            });
            reportTypeCombo.SelectedIndex = 0;

            // Generate button
            generateReportButton = new Button
            {
                Text = "Generate Report",
                Size = new Size(120, 30),
                Location = new Point(320, 15),
                BackColor = Color.LightBlue
            };
            generateReportButton.Click += GenerateReportButton_Click;

            // Export CSV button
            exportCsvButton = new Button
            {
                Text = "Export CSV",
                Size = new Size(100, 30),
                Location = new Point(450, 15),
                BackColor = Color.LightGreen
            };
            exportCsvButton.Click += ExportCsvButton_Click;

            // Export PDF button (placeholder for future enhancement)
            exportPdfButton = new Button
            {
                Text = "Export PDF",
                Size = new Size(100, 30),
                Location = new Point(560, 15),
                BackColor = Color.LightSalmon,
                Enabled = false // Will enable in future
            };

            // Status label
            statusLabel = new Label
            {
                Text = "Ready to generate reports",
                Location = new Point(680, 20),
                Size = new Size(300, 20),
                ForeColor = Color.DarkGreen
            };

            toolbarPanel.Controls.AddRange(new Control[] {
                reportLabel, reportTypeCombo, generateReportButton, exportCsvButton, exportPdfButton, statusLabel
            });

            // Create main layout panels
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60F)); // Data grid gets 60%
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40F)); // Chart gets 40%

            gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke
            };

            mainPanel.Controls.Add(gridPanel, 0, 0);
            mainPanel.Controls.Add(chartPanel, 0, 1);

            this.Controls.Add(mainPanel);
            this.Controls.Add(toolbarPanel);
        }

        private void InitializeDataGrid()
        {
            reportDataGrid = new SfDataGrid()
            {
                Dock = DockStyle.Fill,
                AllowEditing = false,
                AllowResizingColumns = true,
                AutoGenerateColumns = false,
                ShowGroupDropArea = true,
                SelectionMode = Syncfusion.WinForms.DataGrid.Enums.GridSelectionMode.Row
            };

            gridPanel.Controls.Add(reportDataGrid);
        }

        private void InitializeChart()
        {
            trendChart = new SfCartesianChart()
            {
                Dock = DockStyle.Fill
            };

            // Configure chart appearance
            trendChart.Header.Text = "Trend Analysis";
            trendChart.Legend.Visibility = Visibility.Visible;
            trendChart.Legend.Position = LegendPosition.Right;

            // Add primary axes
            trendChart.PrimaryAxis = new CategoryAxis()
            {
                Header = "Account",
                LabelRotationAngle = -45
            };

            trendChart.SecondaryAxis = new NumericalAxis()
            {
                Header = "Amount ($)",
                LabelFormat = "C0"
            };

            chartPanel.Controls.Add(trendChart);
        }

        private async void GenerateReportButton_Click(object? sender, EventArgs e)
        {
            try
            {
                statusLabel.Text = "Generating report...";
                statusLabel.ForeColor = Color.Blue;

                string selectedReport = reportTypeCombo.SelectedItem?.ToString() ?? "Sanitation District Report";

                switch (selectedReport)
                {
                    case "Sanitation District Report":
                        await GenerateSanitationDistrictReport();
                        break;
                    case "Water Report":
                        await GenerateWaterReport();
                        break;
                    case "Trash Report":
                        await GenerateTrashReport();
                        break;
                    case "Apartments Report":
                        await GenerateApartmentsReport();
                        break;
                    case "Summary Report":
                        await GenerateSummaryReport();
                        break;
                    case "Scenario Comparison Report":
                        await GenerateScenarioComparisonReport();
                        break;
                    case "Quarterly Summary Report":
                        await GenerateQuarterlySummaryReport();
                        break;
                }

                statusLabel.Text = "Report generated successfully";
                statusLabel.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Report Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                statusLabel.Text = "Report generation failed";
                statusLabel.ForeColor = Color.Red;
            }
        }

        private async Task GenerateSanitationDistrictReport()
        {
            var data = await _repository.GetSanitationDistrictDataAsync();

            // Configure columns for sanitation district report
            reportDataGrid.Columns.Clear();
            reportDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account", Width = 80 });
            reportDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 250 });
            reportDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Section", HeaderText = "Section", Width = 100 });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CurrentFYBudget", HeaderText = "Budget", Width = 120, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "YearToDateSpending", HeaderText = "YTD Spending", Width = 120, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "PercentOfBudget", HeaderText = "% of Budget", Width = 100, Format = "P2" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "BudgetRemaining", HeaderText = "Remaining", Width = 120, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "RequiredRate", HeaderText = "Req. Rate", Width = 100, Format = "C" });

            reportDataGrid.DataSource = data;

            // Update chart with budget vs actual data
            UpdateBudgetChart(data, "Sanitation District Budget vs Actual");
        }

        private async Task GenerateWaterReport()
        {
            var data = await _repository.GetWaterDataAsync();

            reportDataGrid.Columns.Clear();
            reportDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account", Width = 80 });
            reportDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 250 });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CurrentFYBudget", HeaderText = "Budget", Width = 120, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "YearToDateSpending", HeaderText = "YTD Spending", Width = 120, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "MonthlyUsage", HeaderText = "Usage", Width = 100 });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "RequiredRate", HeaderText = "Rate", Width = 100, Format = "C" });

            reportDataGrid.DataSource = data;
            UpdateBudgetChart(data, "Water Department Analysis");
        }

        private async Task GenerateTrashReport()
        {
            var data = await _repository.GetTrashDataAsync();

            reportDataGrid.Columns.Clear();
            reportDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account", Width = 80 });
            reportDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 250 });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CurrentFYBudget", HeaderText = "Budget", Width = 120, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "YearToDateSpending", HeaderText = "YTD Spending", Width = 120, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "SeasonalAdjustment", HeaderText = "Seasonal Adj", Width = 100, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "RequiredRate", HeaderText = "Rate", Width = 100, Format = "C" });

            reportDataGrid.DataSource = data;
            UpdateBudgetChart(data, "Trash Collection Analysis");
        }

        private async Task GenerateApartmentsReport()
        {
            var data = await _repository.GetApartmentDataAsync();

            reportDataGrid.Columns.Clear();
            reportDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account", Width = 80 });
            reportDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 250 });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CurrentFYBudget", HeaderText = "Budget", Width = 120, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "MonthlyUsage", HeaderText = "Units", Width = 80 });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "RequiredRate", HeaderText = "Rate/Unit", Width = 100, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CustomerAffordabilityIndex", HeaderText = "Affordability", Width = 100, Format = "N2" });

            reportDataGrid.DataSource = data;
            UpdateBudgetChart(data, "Apartment Complex Analysis");
        }

        private async Task GenerateSummaryReport()
        {
            // Get data from all sources
            var sanitationData = await _repository.GetSanitationDistrictDataAsync();
            var waterData = await _repository.GetWaterDataAsync();
            var trashData = await _repository.GetTrashDataAsync();
            var apartmentData = await _repository.GetApartmentDataAsync();

            // Create summary data
            var summaryData = new List<dynamic>
            {
                new {
                    Enterprise = "Sanitation District",
                    TotalBudget = sanitationData.Sum(d => d.CurrentFYBudget),
                    TotalYTD = sanitationData.Sum(d => d.YearToDateSpending),
                    TotalRemaining = sanitationData.Sum(d => d.BudgetRemaining),
                    PercentUsed = sanitationData.Sum(d => d.CurrentFYBudget) > 0 ?
                        sanitationData.Sum(d => d.YearToDateSpending) / sanitationData.Sum(d => d.CurrentFYBudget) : 0,
                    Revenue = sanitationData.Where(d => d.Section == "Revenue").Sum(d => d.CurrentFYBudget),
                    Expenses = sanitationData.Where(d => d.Section != "Revenue").Sum(d => d.CurrentFYBudget),
                    NetSurplus = sanitationData.Where(d => d.Section == "Revenue").Sum(d => d.CurrentFYBudget) -
                                 sanitationData.Where(d => d.Section != "Revenue").Sum(d => d.CurrentFYBudget)
                },
                new {
                    Enterprise = "Water",
                    TotalBudget = waterData.Sum(d => d.CurrentFYBudget),
                    TotalYTD = waterData.Sum(d => d.YearToDateSpending),
                    TotalRemaining = waterData.Sum(d => d.BudgetRemaining),
                    PercentUsed = waterData.Sum(d => d.CurrentFYBudget) > 0 ?
                        waterData.Sum(d => d.YearToDateSpending) / waterData.Sum(d => d.CurrentFYBudget) : 0,
                    Revenue = waterData.Sum(d => d.CurrentFYBudget) * 0.7m, // Estimate
                    Expenses = waterData.Sum(d => d.CurrentFYBudget) * 0.3m, // Estimate
                    NetSurplus = waterData.Sum(d => d.CurrentFYBudget) * 0.4m // Estimate
                },
                new {
                    Enterprise = "Trash",
                    TotalBudget = trashData.Sum(d => d.CurrentFYBudget),
                    TotalYTD = trashData.Sum(d => d.YearToDateSpending),
                    TotalRemaining = trashData.Sum(d => d.BudgetRemaining),
                    PercentUsed = trashData.Sum(d => d.CurrentFYBudget) > 0 ?
                        trashData.Sum(d => d.YearToDateSpending) / trashData.Sum(d => d.CurrentFYBudget) : 0,
                    Revenue = trashData.Sum(d => d.CurrentFYBudget) * 0.8m, // Estimate
                    Expenses = trashData.Sum(d => d.CurrentFYBudget) * 0.2m, // Estimate
                    NetSurplus = trashData.Sum(d => d.CurrentFYBudget) * 0.6m // Estimate
                },
                new {
                    Enterprise = "Apartments",
                    TotalBudget = apartmentData.Sum(d => d.CurrentFYBudget),
                    TotalYTD = apartmentData.Sum(d => d.YearToDateSpending),
                    TotalRemaining = apartmentData.Sum(d => d.BudgetRemaining),
                    PercentUsed = apartmentData.Sum(d => d.CurrentFYBudget) > 0 ?
                        apartmentData.Sum(d => d.YearToDateSpending) / apartmentData.Sum(d => d.CurrentFYBudget) : 0,
                    Revenue = apartmentData.Sum(d => d.CurrentFYBudget),
                    Expenses = apartmentData.Sum(d => d.CurrentFYBudget) * 0.1m, // Estimate
                    NetSurplus = apartmentData.Sum(d => d.CurrentFYBudget) * 0.9m // Estimate
                }
            };

            reportDataGrid.Columns.Clear();
            reportDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Enterprise", HeaderText = "Enterprise", Width = 150 });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "TotalBudget", HeaderText = "Total Budget", Width = 120, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "TotalYTD", HeaderText = "YTD Spending", Width = 120, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "PercentUsed", HeaderText = "% Used", Width = 80, Format = "P2" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Revenue", HeaderText = "Revenue", Width = 120, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Expenses", HeaderText = "Expenses", Width = 120, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "NetSurplus", HeaderText = "Net Surplus", Width = 120, Format = "C" });

            reportDataGrid.DataSource = summaryData;
            UpdateSummaryChart(summaryData);
        }

        private async Task GenerateScenarioComparisonReport()
        {
            var data = await _repository.GetSanitationDistrictDataAsync();

            reportDataGrid.Columns.Clear();
            reportDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account", Width = 80 });
            reportDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 200 });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "MonthlyInput", HeaderText = "Current", Width = 100, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Scenario1", HeaderText = "Scenario 1\n(Trash Truck)", Width = 120, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Scenario2", HeaderText = "Scenario 2\n(Reserve Fund)", Width = 120, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Scenario3", HeaderText = "Scenario 3\n(Grant Repay)", Width = 120, Format = "C" });

            reportDataGrid.DataSource = data;
            UpdateScenarioChart(data);
        }

        private async Task GenerateQuarterlySummaryReport()
        {
            var data = await _repository.GetSanitationDistrictDataAsync();

            // Generate quarterly summary based on monthly inputs
            var quarterlyData = data.Select(d => new
            {
                d.Account,
                d.Label,
                d.Section,
                Q1 = d.MonthlyInput * 3,
                Q2 = d.MonthlyInput * 3,
                Q3 = d.MonthlyInput * 3,
                Q4 = d.MonthlyInput * 3,
                YearTotal = d.MonthlyInput * 12,
                d.CurrentFYBudget,
                Variance = d.CurrentFYBudget - (d.MonthlyInput * 12)
            }).ToList();

            reportDataGrid.Columns.Clear();
            reportDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account", Width = 80 });
            reportDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 200 });
            reportDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Section", HeaderText = "Section", Width = 80 });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Q1", HeaderText = "Q1", Width = 100, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Q2", HeaderText = "Q2", Width = 100, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Q3", HeaderText = "Q3", Width = 100, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Q4", HeaderText = "Q4", Width = 100, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "YearTotal", HeaderText = "Year Total", Width = 120, Format = "C" });
            reportDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Variance", HeaderText = "Variance", Width = 100, Format = "C" });

            reportDataGrid.DataSource = quarterlyData;
            UpdateQuarterlyChart(quarterlyData);
        }

        private void UpdateBudgetChart(BindingList<SanitationDistrict> data, string title)
        {
            trendChart.Series.Clear();
            trendChart.Header.Text = title;

            var budgetSeries = new ColumnSeries()
            {
                ItemsSource = data.Take(10), // Limit to first 10 items for readability
                XBindingPath = "Account",
                YBindingPath = "CurrentFYBudget",
                Label = "Budget",
                LegendIcon = LegendIcon.Rectangle
            };

            var actualSeries = new ColumnSeries()
            {
                ItemsSource = data.Take(10),
                XBindingPath = "Account",
                YBindingPath = "YearToDateSpending",
                Label = "YTD Spending",
                LegendIcon = LegendIcon.Rectangle
            };

            trendChart.Series.Add(budgetSeries);
            trendChart.Series.Add(actualSeries);
        }

        private void UpdateSummaryChart(List<dynamic> summaryData)
        {
            trendChart.Series.Clear();
            trendChart.Header.Text = "Enterprise Summary Comparison";

            var revenueSeries = new ColumnSeries()
            {
                ItemsSource = summaryData,
                XBindingPath = "Enterprise",
                YBindingPath = "Revenue",
                Label = "Revenue",
                LegendIcon = LegendIcon.Rectangle
            };

            var expenseSeries = new ColumnSeries()
            {
                ItemsSource = summaryData,
                XBindingPath = "Enterprise",
                YBindingPath = "Expenses",
                Label = "Expenses",
                LegendIcon = LegendIcon.Rectangle
            };

            trendChart.Series.Add(revenueSeries);
            trendChart.Series.Add(expenseSeries);
        }

        private void UpdateScenarioChart(BindingList<SanitationDistrict> data)
        {
            trendChart.Series.Clear();
            trendChart.Header.Text = "Scenario Comparison";

            var currentSeries = new LineSeries()
            {
                ItemsSource = data.Take(8),
                XBindingPath = "Account",
                YBindingPath = "MonthlyInput",
                Label = "Current",
                LegendIcon = LegendIcon.Line
            };

            var scenario1Series = new LineSeries()
            {
                ItemsSource = data.Take(8),
                XBindingPath = "Account",
                YBindingPath = "Scenario1",
                Label = "Scenario 1",
                LegendIcon = LegendIcon.Line
            };

            var scenario2Series = new LineSeries()
            {
                ItemsSource = data.Take(8),
                XBindingPath = "Account",
                YBindingPath = "Scenario2",
                Label = "Scenario 2",
                LegendIcon = LegendIcon.Line
            };

            trendChart.Series.Add(currentSeries);
            trendChart.Series.Add(scenario1Series);
            trendChart.Series.Add(scenario2Series);
        }

        private void UpdateQuarterlyChart(List<dynamic> quarterlyData)
        {
            trendChart.Series.Clear();
            trendChart.Header.Text = "Quarterly Trend Analysis";

            var q1Series = new ColumnSeries()
            {
                ItemsSource = quarterlyData.Take(8),
                XBindingPath = "Account",
                YBindingPath = "Q1",
                Label = "Q1",
                LegendIcon = LegendIcon.Rectangle
            };

            var q2Series = new ColumnSeries()
            {
                ItemsSource = quarterlyData.Take(8),
                XBindingPath = "Account",
                YBindingPath = "Q2",
                Label = "Q2",
                LegendIcon = LegendIcon.Rectangle
            };

            var q3Series = new ColumnSeries()
            {
                ItemsSource = quarterlyData.Take(8),
                XBindingPath = "Account",
                YBindingPath = "Q3",
                Label = "Q3",
                LegendIcon = LegendIcon.Rectangle
            };

            var q4Series = new ColumnSeries()
            {
                ItemsSource = quarterlyData.Take(8),
                XBindingPath = "Account",
                YBindingPath = "Q4",
                Label = "Q4",
                LegendIcon = LegendIcon.Rectangle
            };

            trendChart.Series.Add(q1Series);
            trendChart.Series.Add(q2Series);
            trendChart.Series.Add(q3Series);
            trendChart.Series.Add(q4Series);
        }

        private void ExportCsvButton_Click(object? sender, EventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv",
                    DefaultExt = "csv",
                    FileName = $"{reportTypeCombo.SelectedItem}_{DateTime.Now:yyyy-MM-dd}.csv"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCsv(saveFileDialog.FileName);
                    MessageBox.Show($"Report exported successfully to {saveFileDialog.FileName}",
                        "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    statusLabel.Text = "CSV export completed";
                    statusLabel.ForeColor = Color.DarkGreen;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting CSV: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                statusLabel.Text = "CSV export failed";
                statusLabel.ForeColor = Color.Red;
            }
        }

        private void ExportToCsv(string fileName)
        {
            var csv = new StringBuilder();

            // Add headers
            var headers = reportDataGrid.Columns.Select(c => c.HeaderText).ToArray();
            csv.AppendLine(string.Join(",", headers.Select(h => $"\"{h}\"")));

            // Add data rows
            var dataSource = reportDataGrid.DataSource;
            if (dataSource is BindingList<SanitationDistrict> sanitationList)
            {
                foreach (var item in sanitationList)
                {
                    var values = new List<string>();
                    foreach (var column in reportDataGrid.Columns)
                    {
                        var property = typeof(SanitationDistrict).GetProperty(column.MappingName);
                        var value = property?.GetValue(item)?.ToString() ?? "";
                        values.Add($"\"{value}\"");
                    }
                    csv.AppendLine(string.Join(",", values));
                }
            }
            else if (dataSource is System.Collections.IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    var values = new List<string>();
                    foreach (var column in reportDataGrid.Columns)
                    {
                        var property = item.GetType().GetProperty(column.MappingName);
                        var value = property?.GetValue(item)?.ToString() ?? "";
                        values.Add($"\"{value}\"");
                    }
                    csv.AppendLine(string.Join(",", values));
                }
            }

            File.WriteAllText(fileName, csv.ToString());
        }
    }
}
