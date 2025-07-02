using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Database;

namespace WileyBudgetManagement.Forms
{
    public partial class SummaryPage : Form
    {
        private readonly ISanitationRepository _repository;
        private readonly DatabaseManager _databaseManager;
        private SfDataGrid summaryDataGrid = null!;
        private Button refreshButton = null!;
        private Label totalRevenueLabel = null!;
        private Label totalExpensesLabel = null!;
        private Label netSurplusLabel = null!;
        private Label statusLabel = null!;
        private Panel metricsPanel = null!;

        public SummaryPage()
        {
            _databaseManager = new DatabaseManager();
            _repository = new SanitationRepository(_databaseManager);

            InitializeComponent();
            InitializeControls();
            InitializeDataGrid();
            LoadSummaryDataAsync();
        }

        private void InitializeControls()
        {
            this.Text = "Enterprise Summary Dashboard";
            this.Size = new Size(1000, 700);

            // Create toolbar panel
            var toolbarPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top,
                BackColor = Color.DarkBlue
            };

            // Refresh button
            refreshButton = new Button
            {
                Text = "Refresh Data",
                Size = new Size(120, 30),
                Location = new Point(10, 10),
                BackColor = Color.LightBlue
            };
            refreshButton.Click += RefreshButton_Click;

            // Status label
            statusLabel = new Label
            {
                Text = "Loading summary data...",
                Location = new Point(150, 15),
                Size = new Size(300, 20),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            toolbarPanel.Controls.AddRange(new Control[] { refreshButton, statusLabel });

            // Create metrics panel for key financial indicators
            metricsPanel = new Panel
            {
                Height = 120,
                Dock = DockStyle.Top,
                BackColor = Color.LightGray
            };

            // Total Revenue Label
            totalRevenueLabel = new Label
            {
                Text = "Total Revenue: $0.00",
                Location = new Point(20, 20),
                Size = new Size(250, 30),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };

            // Total Expenses Label
            totalExpensesLabel = new Label
            {
                Text = "Total Expenses: $0.00",
                Location = new Point(280, 20),
                Size = new Size(250, 30),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };

            // Net Surplus Label
            netSurplusLabel = new Label
            {
                Text = "Net Surplus/Deficit: $0.00",
                Location = new Point(540, 20),
                Size = new Size(300, 30),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            // Add additional metrics
            var budgetUtilizationLabel = new Label
            {
                Text = "Cross-Enterprise Analysis",
                Location = new Point(20, 60),
                Size = new Size(400, 25),
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray
            };

            var rateImpactLabel = new Label
            {
                Text = "Rate Impact Analysis: View grid for detailed breakdown",
                Location = new Point(20, 85),
                Size = new Size(600, 20),
                Font = new Font("Arial", 9, FontStyle.Regular),
                ForeColor = Color.Navy
            };

            metricsPanel.Controls.AddRange(new Control[] {
                totalRevenueLabel, totalExpensesLabel, netSurplusLabel, budgetUtilizationLabel, rateImpactLabel
            });

            this.Controls.Add(metricsPanel);
            this.Controls.Add(toolbarPanel);
        }

        private void InitializeDataGrid()
        {
            summaryDataGrid = new SfDataGrid()
            {
                Dock = DockStyle.Fill,
                AllowEditing = false,
                AllowResizingColumns = true,
                AutoGenerateColumns = false,
                ShowGroupDropArea = true
            };

            // Configure columns for enterprise summary
            summaryDataGrid.Columns.Add(new GridTextColumn() { MappingName = "Enterprise", HeaderText = "Enterprise", Width = 150 });
            summaryDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "TotalRevenue", HeaderText = "Total Revenue", Width = 130, Format = "C" });
            summaryDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "TotalOperatingExpenses", HeaderText = "Operating Exp", Width = 130, Format = "C" });
            summaryDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "TotalAdminExpenses", HeaderText = "Admin Exp", Width = 120, Format = "C" });
            summaryDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "TotalExpenses", HeaderText = "Total Expenses", Width = 130, Format = "C" });
            summaryDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "NetSurplusDeficit", HeaderText = "Net Surplus", Width = 130, Format = "C" });
            summaryDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "PercentOfTotalBudget", HeaderText = "% of Total", Width = 100, Format = "P2" });
            summaryDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "BudgetUtilization", HeaderText = "Budget Used", Width = 110, Format = "P2" });
            summaryDataGrid.Columns.Add(new GridNumericColumn() { MappingName = "RequiredRateAdjustment", HeaderText = "Rate Adj %", Width = 110, Format = "P2" });

            this.Controls.Add(summaryDataGrid);
        }

        private async void LoadSummaryDataAsync()
        {
            try
            {
                statusLabel.Text = "Loading enterprise data...";
                statusLabel.ForeColor = Color.Yellow;

                // Load data from all enterprises
                var sanitationData = await _repository.GetSanitationDistrictDataAsync();
                var waterData = await _repository.GetWaterDataAsync();
                var trashData = await _repository.GetTrashDataAsync();
                var apartmentData = await _repository.GetApartmentDataAsync();

                // Calculate summary data
                var summaryData = CalculateEnterpriseSummary(sanitationData, waterData, trashData, apartmentData);

                // Update data grid
                summaryDataGrid.DataSource = summaryData;

                // Update metrics
                UpdateMetrics(summaryData);

                statusLabel.Text = "Summary data loaded successfully";
                statusLabel.ForeColor = Color.LightGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading summary data: {ex.Message}", "Data Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                statusLabel.Text = "Error loading data";
                statusLabel.ForeColor = Color.Red;
            }
        }

        private List<EnterpriseSummary> CalculateEnterpriseSummary(
            BindingList<SanitationDistrict> sanitationData,
            BindingList<SanitationDistrict> waterData,
            BindingList<SanitationDistrict> trashData,
            BindingList<SanitationDistrict> apartmentData)
        {
            var summaryList = new List<EnterpriseSummary>();

            // Sanitation District Summary
            var sanitationSummary = new EnterpriseSummary
            {
                Enterprise = "Sanitation District",
                TotalRevenue = sanitationData.Where(d => d.Section == "Revenue").Sum(d => d.CurrentFYBudget),
                TotalOperatingExpenses = sanitationData.Where(d => d.Section == "Operating").Sum(d => d.CurrentFYBudget),
                TotalAdminExpenses = sanitationData.Where(d => d.Section == "Admin").Sum(d => d.CurrentFYBudget),
                TotalYTDSpending = sanitationData.Sum(d => d.YearToDateSpending),
                TotalBudget = sanitationData.Sum(d => d.CurrentFYBudget)
            };
            sanitationSummary.TotalExpenses = sanitationSummary.TotalOperatingExpenses + sanitationSummary.TotalAdminExpenses;
            sanitationSummary.NetSurplusDeficit = sanitationSummary.TotalRevenue - sanitationSummary.TotalExpenses;
            summaryList.Add(sanitationSummary);

            // Water Summary
            var waterSummary = new EnterpriseSummary
            {
                Enterprise = "Water",
                TotalRevenue = waterData.Sum(d => d.CurrentFYBudget) * 0.75m, // Estimate 75% is revenue
                TotalOperatingExpenses = waterData.Sum(d => d.CurrentFYBudget) * 0.20m,
                TotalAdminExpenses = waterData.Sum(d => d.CurrentFYBudget) * 0.05m,
                TotalYTDSpending = waterData.Sum(d => d.YearToDateSpending),
                TotalBudget = waterData.Sum(d => d.CurrentFYBudget)
            };
            waterSummary.TotalExpenses = waterSummary.TotalOperatingExpenses + waterSummary.TotalAdminExpenses;
            waterSummary.NetSurplusDeficit = waterSummary.TotalRevenue - waterSummary.TotalExpenses;
            summaryList.Add(waterSummary);

            // Trash Summary
            var trashSummary = new EnterpriseSummary
            {
                Enterprise = "Trash",
                TotalRevenue = trashData.Sum(d => d.CurrentFYBudget) * 0.80m, // Estimate 80% is revenue
                TotalOperatingExpenses = trashData.Sum(d => d.CurrentFYBudget) * 0.15m,
                TotalAdminExpenses = trashData.Sum(d => d.CurrentFYBudget) * 0.05m,
                TotalYTDSpending = trashData.Sum(d => d.YearToDateSpending),
                TotalBudget = trashData.Sum(d => d.CurrentFYBudget)
            };
            trashSummary.TotalExpenses = trashSummary.TotalOperatingExpenses + trashSummary.TotalAdminExpenses;
            trashSummary.NetSurplusDeficit = trashSummary.TotalRevenue - trashSummary.TotalExpenses;
            summaryList.Add(trashSummary);

            // Apartments Summary
            var apartmentSummary = new EnterpriseSummary
            {
                Enterprise = "Apartments",
                TotalRevenue = apartmentData.Sum(d => d.CurrentFYBudget), // Apartments are mostly revenue
                TotalOperatingExpenses = apartmentData.Sum(d => d.CurrentFYBudget) * 0.05m,
                TotalAdminExpenses = apartmentData.Sum(d => d.CurrentFYBudget) * 0.05m,
                TotalYTDSpending = apartmentData.Sum(d => d.YearToDateSpending),
                TotalBudget = apartmentData.Sum(d => d.CurrentFYBudget)
            };
            apartmentSummary.TotalExpenses = apartmentSummary.TotalOperatingExpenses + apartmentSummary.TotalAdminExpenses;
            apartmentSummary.NetSurplusDeficit = apartmentSummary.TotalRevenue - apartmentSummary.TotalExpenses;
            summaryList.Add(apartmentSummary);

            // Calculate additional metrics
            decimal grandTotalBudget = summaryList.Sum(s => s.TotalBudget);
            foreach (var summary in summaryList)
            {
                summary.PercentOfTotalBudget = grandTotalBudget > 0 ? summary.TotalBudget / grandTotalBudget : 0;
                summary.BudgetUtilization = summary.TotalBudget > 0 ? summary.TotalYTDSpending / summary.TotalBudget : 0;

                // Calculate required rate adjustment based on surplus/deficit
                if (summary.TotalRevenue > 0 && summary.NetSurplusDeficit < 0)
                {
                    summary.RequiredRateAdjustment = Math.Abs(summary.NetSurplusDeficit) / summary.TotalRevenue;
                }
                else
                {
                    summary.RequiredRateAdjustment = 0; // No adjustment needed if surplus
                }
            }

            return summaryList;
        }

        private void UpdateMetrics(List<EnterpriseSummary> summaryData)
        {
            decimal totalRevenue = summaryData.Sum(s => s.TotalRevenue);
            decimal totalExpenses = summaryData.Sum(s => s.TotalExpenses);
            decimal netSurplus = totalRevenue - totalExpenses;

            totalRevenueLabel.Text = $"Total Revenue: {totalRevenue:C}";
            totalExpensesLabel.Text = $"Total Expenses: {totalExpenses:C}";
            netSurplusLabel.Text = $"Net Surplus/Deficit: {netSurplus:C}";
            netSurplusLabel.ForeColor = netSurplus >= 0 ? Color.DarkGreen : Color.DarkRed;
        }

        private void RefreshButton_Click(object? sender, EventArgs e)
        {
            LoadSummaryDataAsync();
        }
    }

    // Helper class for enterprise summary data
    public class EnterpriseSummary
    {
        public string Enterprise { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public decimal TotalOperatingExpenses { get; set; }
        public decimal TotalAdminExpenses { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetSurplusDeficit { get; set; }
        public decimal PercentOfTotalBudget { get; set; }
        public decimal BudgetUtilization { get; set; }
        public decimal RequiredRateAdjustment { get; set; }
        public decimal TotalYTDSpending { get; set; }
        public decimal TotalBudget { get; set; }
    }
}
