using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using WileyBudgetManagement.Models;

namespace WileyBudgetManagement.Reports
{
    public partial class ReportsForm : Form
    {
        private SfDataGrid reportsDataGrid = null!;
        private ComboBox reportTypeCombo = null!;
        private Button generateButton = null!;
        private Button exportButton = null!;
        private Button printButton = null!;
        private Panel chartPlaceholder = null!;
        private Label statusLabel = null!;

        private List<SanitationDistrict> sanitationData = new List<SanitationDistrict>();

        public ReportsForm()
        {
            InitializeComponent();
            InitializeControls();
            InitializeReportsDataGrid();
        }

        private void InitializeControls()
        {
            this.Text = "Reports and Analytics";
            this.Size = new Size(1400, 800);
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
                "Budget vs Actual",
                "Revenue Analysis",
                "Expense Analysis",
                "Scenario Comparison",
                "Quarterly Summary",
                "Rate Analysis"
            });
            reportTypeCombo.SelectedIndex = 0;

            // Generate button
            generateButton = new Button
            {
                Text = "Generate Report",
                Size = new Size(120, 30),
                Location = new Point(320, 15),
                BackColor = Color.LightBlue
            };
            generateButton.Click += GenerateButton_Click;

            // Export button
            exportButton = new Button
            {
                Text = "Export to CSV",
                Size = new Size(100, 30),
                Location = new Point(450, 15),
                BackColor = Color.LightGreen
            };
            exportButton.Click += ExportButton_Click;

            // Print button
            printButton = new Button
            {
                Text = "Print",
                Size = new Size(80, 30),
                Location = new Point(560, 15),
                BackColor = Color.LightYellow
            };
            printButton.Click += PrintButton_Click;

            // Status label
            statusLabel = new Label
            {
                Text = "Ready",
                Location = new Point(670, 20),
                Size = new Size(200, 20),
                ForeColor = Color.DarkGreen
            };

            toolbarPanel.Controls.AddRange(new Control[] {
                reportLabel, reportTypeCombo, generateButton, exportButton, printButton, statusLabel
            });

            this.Controls.Add(toolbarPanel);

            // Create chart placeholder panel
            chartPlaceholder = new Panel
            {
                Height = 200,
                Dock = DockStyle.Top,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var chartLabel = new Label
            {
                Text = "Chart visualization will be added here",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10, FontStyle.Italic),
                ForeColor = Color.Gray
            };

            chartPlaceholder.Controls.Add(chartLabel);
            this.Controls.Add(chartPlaceholder);
        }

        private void InitializeReportsDataGrid()
        {
            reportsDataGrid = new SfDataGrid()
            {
                Dock = DockStyle.Fill,
                AllowEditing = false,
                AllowResizingColumns = true,
                AutoGenerateColumns = false,
                ShowGroupDropArea = true,
                SelectionMode = Syncfusion.WinForms.DataGrid.Enums.GridSelectionMode.Single
            };

            this.Controls.Add(reportsDataGrid);
        }

        public void LoadSanitationData(List<SanitationDistrict> data)
        {
            sanitationData = data ?? new List<SanitationDistrict>();
            GenerateCurrentReport();
        }

        private void GenerateButton_Click(object? sender, EventArgs e)
        {
            GenerateCurrentReport();
        }

        private void GenerateCurrentReport()
        {
            string reportType = reportTypeCombo.SelectedItem?.ToString() ?? "Budget vs Actual";

            try
            {
                switch (reportType)
                {
                    case "Budget vs Actual":
                        GenerateBudgetVsActualReport();
                        break;
                    case "Revenue Analysis":
                        GenerateRevenueAnalysisReport();
                        break;
                    case "Expense Analysis":
                        GenerateExpenseAnalysisReport();
                        break;
                    case "Scenario Comparison":
                        GenerateScenarioComparisonReport();
                        break;
                    case "Quarterly Summary":
                        GenerateQuarterlySummaryReport();
                        break;
                    case "Rate Analysis":
                        GenerateRateAnalysisReport();
                        break;
                    default:
                        GenerateBudgetVsActualReport();
                        break;
                }

                statusLabel.Text = $"{reportType} generated successfully";
                statusLabel.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error generating report: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
            }
        }

        private void GenerateBudgetVsActualReport()
        {
            reportsDataGrid.Columns.Clear();

            reportsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account", Width = 100 });
            reportsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 200 });
            reportsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Section", HeaderText = "Section", Width = 100 });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Budget", HeaderText = "Budget", Width = 120, Format = "C" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Actual", HeaderText = "YTD Actual", Width = 120, Format = "C" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Variance", HeaderText = "Variance", Width = 120, Format = "C" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "PercentUsed", HeaderText = "% Used", Width = 100, Format = "P2" });

            var reportData = sanitationData.Select(d => new
            {
                Account = d.Account,
                Label = d.Label,
                Section = d.Section,
                Budget = d.CurrentFYBudget,
                Actual = d.YearToDateSpending,
                Variance = d.BudgetRemaining,
                PercentUsed = d.PercentOfBudget
            }).ToList();

            reportsDataGrid.DataSource = reportData;
        }

        private void GenerateRevenueAnalysisReport()
        {
            reportsDataGrid.Columns.Clear();

            reportsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account", Width = 100 });
            reportsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 200 });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "CurrentFYBudget", HeaderText = "Budget", Width = 120, Format = "C" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "MonthlyInput", HeaderText = "Monthly", Width = 100, Format = "C" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "SeasonalFactor", HeaderText = "Seasonal Factor", Width = 120, Format = "N2" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "RequiredRate", HeaderText = "Required Rate", Width = 120, Format = "C" });

            var revenueData = sanitationData.Where(d => d.Section == "Revenue").Select(d => new
            {
                Account = d.Account,
                Label = d.Label,
                CurrentFYBudget = d.CurrentFYBudget,
                MonthlyInput = d.MonthlyInput,
                SeasonalFactor = d.SeasonalRevenueFactor,
                RequiredRate = d.RequiredRate
            }).ToList();

            reportsDataGrid.DataSource = revenueData;
        }

        private void GenerateExpenseAnalysisReport()
        {
            reportsDataGrid.Columns.Clear();

            reportsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account", Width = 100 });
            reportsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 200 });
            reportsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Section", HeaderText = "Type", Width = 100 });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Budget", HeaderText = "Budget", Width = 120, Format = "C" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "YTDSpending", HeaderText = "YTD Spending", Width = 120, Format = "C" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Remaining", HeaderText = "Remaining", Width = 120, Format = "C" });

            var expenseData = sanitationData.Where(d => d.Section == "Operating" || d.Section == "Admin").Select(d => new
            {
                Account = d.Account,
                Label = d.Label,
                Section = d.Section,
                Budget = d.CurrentFYBudget,
                YTDSpending = d.YearToDateSpending,
                Remaining = d.BudgetRemaining
            }).ToList();

            reportsDataGrid.DataSource = expenseData;
        }

        private void GenerateScenarioComparisonReport()
        {
            reportsDataGrid.Columns.Clear();

            reportsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account", Width = 100 });
            reportsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 200 });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Current", HeaderText = "Current", Width = 100, Format = "C" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Scenario1", HeaderText = "Trash Truck", Width = 100, Format = "C" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Scenario2", HeaderText = "Reserve Fund", Width = 100, Format = "C" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Scenario3", HeaderText = "Grant Repay", Width = 100, Format = "C" });

            var scenarioData = sanitationData.Select(d => new
            {
                Account = d.Account,
                Label = d.Label,
                Current = d.MonthlyInput,
                Scenario1 = d.Scenario1,
                Scenario2 = d.Scenario2,
                Scenario3 = d.Scenario3
            }).ToList();

            reportsDataGrid.DataSource = scenarioData;
        }

        private void GenerateQuarterlySummaryReport()
        {
            reportsDataGrid.Columns.Clear();

            reportsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Section", HeaderText = "Section", Width = 100 });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Q1", HeaderText = "Q1", Width = 100, Format = "C" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Q2", HeaderText = "Q2", Width = 100, Format = "C" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Q3", HeaderText = "Q3", Width = 100, Format = "C" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "Q4", HeaderText = "Q4", Width = 100, Format = "C" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "YearTotal", HeaderText = "Year Total", Width = 120, Format = "C" });

            var quarterlyData = sanitationData.GroupBy(d => d.Section).Select(g => new
            {
                Section = g.Key,
                Q1 = g.Sum(d => d.MonthlyInput * 3),
                Q2 = g.Sum(d => d.MonthlyInput * 3),
                Q3 = g.Sum(d => d.MonthlyInput * 3),
                Q4 = g.Sum(d => d.MonthlyInput * 3),
                YearTotal = g.Sum(d => d.CurrentFYBudget)
            }).ToList();

            reportsDataGrid.DataSource = quarterlyData;
        }

        private void GenerateRateAnalysisReport()
        {
            reportsDataGrid.Columns.Clear();

            reportsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Account", HeaderText = "Account", Width = 100 });
            reportsDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Label", HeaderText = "Description", Width = 200 });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "RequiredRate", HeaderText = "Required Rate", Width = 120, Format = "C" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "AffordabilityFactor", HeaderText = "Affordability", Width = 120, Format = "N2" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "TOUFactor", HeaderText = "TOU Factor", Width = 100, Format = "N2" });
            reportsDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "AdjustedRate", HeaderText = "Adjusted Rate", Width = 120, Format = "C" });

            var rateData = sanitationData.Select(d => new
            {
                Account = d.Account,
                Label = d.Label,
                RequiredRate = d.RequiredRate,
                AffordabilityFactor = d.CustomerAffordabilityIndex,
                TOUFactor = d.TimeOfUseFactor,
                AdjustedRate = d.RequiredRate * d.CustomerAffordabilityIndex * d.TimeOfUseFactor
            }).ToList();

            reportsDataGrid.DataSource = rateData;
        }

        private void ExportButton_Click(object? sender, EventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    DefaultExt = "csv",
                    FileName = $"Report_{reportTypeCombo.SelectedItem}_{DateTime.Now:yyyyMMdd}.csv"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCSV(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToCSV(string fileName)
        {
            var dataSource = reportsDataGrid.DataSource;
            if (dataSource == null) return;

            using (var writer = new StreamWriter(fileName))
            {
                // Write headers
                var headers = reportsDataGrid.Columns.Select(c => c.HeaderText).ToArray();
                writer.WriteLine(string.Join(",", headers));

                // Write data rows
                if (dataSource is IEnumerable<object> items)
                {
                    foreach (var item in items)
                    {
                        var values = new List<string>();
                        foreach (var column in reportsDataGrid.Columns)
                        {
                            var prop = item.GetType().GetProperty(column.MappingName);
                            var value = prop?.GetValue(item)?.ToString() ?? "";
                            values.Add($"\"{value}\"");
                        }
                        writer.WriteLine(string.Join(",", values));
                    }
                }
            }

            statusLabel.Text = "Export completed successfully";
            statusLabel.ForeColor = Color.DarkGreen;
        }

        private void PrintButton_Click(object? sender, EventArgs e)
        {
            // Simple print implementation
            MessageBox.Show("Print functionality will be implemented in a future version.",
                "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
