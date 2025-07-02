using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Reports;
using WileyBudgetManagement.Database;

namespace WileyBudgetManagement.Forms
{
    public partial class DashboardForm : Form
    {
        private Panel navigationPanel = null!;
        private Panel contentPanel = null!;
        private Label titleLabel = null!;
        private Label statusLabel = null!;
        private Panel summaryPanel = null!;
        private readonly DatabaseManager _databaseManager;
        private readonly ISanitationRepository _repository;

        // Child forms
        private WaterInput? waterForm;
        private SanitationDistrictInput? sanitationForm;
        private TrashInput? trashForm;
        private ApartmentsInput? apartmentForm;
        private SummaryPage? summaryForm;
        private ReportsForm? reportsForm;
        private AIQueryPanel? aiQueryForm;
        private ResourcesForm? resourcesForm;

        public DashboardForm()
        {
            _databaseManager = new DatabaseManager();
            _repository = new SanitationRepository(_databaseManager);

            InitializeComponent();
            InitializeDashboard();
            LoadSummaryData();
        }

        private void InitializeDashboard()
        {
            this.Text = "Town of Wiley Budget Management - Dashboard";
            this.Size = new Size(1600, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            // Create main layout
            CreateNavigationPanel();
            CreateMainPanel();
            CreateSummaryPanel();
            CreateStatusBar();
        }

        private void CreateNavigationPanel()
        {
            navigationPanel = new Panel
            {
                Width = 250,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(45, 45, 48),
                Padding = new Padding(10)
            };

            // Title
            titleLabel = new Label
            {
                Text = "Budget Management",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };
            navigationPanel.Controls.Add(titleLabel);

            // Separator
            var separator1 = new Panel
            {
                Height = 2,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(0, 122, 204)
            };
            navigationPanel.Controls.Add(separator1);

            // Navigation buttons
            var buttonsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            var buttons = new[]
            {
                new { Text = "📊 Dashboard Overview", Action = new Action(() => ShowDashboardOverview()) },
                new { Text = "💧 Water District", Action = new Action(() => ShowWaterInput()) },
                new { Text = "🚿 Sanitation District", Action = new Action(() => ShowSanitationDistrictInput()) },
                new { Text = "🗑️ Trash & Recycling", Action = new Action(() => ShowTrashInput()) },
                new { Text = "🏠 Apartments Input", Action = new Action(() => ShowApartmentsInput()) },
                new { Text = "🤖 AI Budget Assistant", Action = new Action(() => ShowAIQueryPanel()) },
                new { Text = "� Resources", Action = new Action(() => ShowResources()) },
                new { Text = "�📈 Summary & Analysis", Action = new Action(() => ShowSummaryPage()) },
                new { Text = "📋 Reports", Action = new Action(() => ShowReports()) },
                new { Text = "⚙️ Settings", Action = new Action(() => ShowSettings()) }
            };

            for (int i = 0; i < buttons.Length; i++)
            {
                var button = CreateNavigationButton(buttons[i].Text, buttons[i].Action);
                button.Location = new Point(10, 10 + (i * 50));
                buttonsPanel.Controls.Add(button);
            }

            navigationPanel.Controls.Add(buttonsPanel);
            this.Controls.Add(navigationPanel);
        }

        private Button CreateNavigationButton(string text, Action action)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(220, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(62, 62, 66),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 122, 204);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 100, 180);

            button.Click += (sender, e) => action();

            return button;
        }

        private void CreateMainPanel()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };
            this.Controls.Add(contentPanel);
        }

        private void CreateSummaryPanel()
        {
            summaryPanel = new Panel
            {
                Height = 150,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(10)
            };

            var summaryTitle = new Label
            {
                Text = "Quick Summary",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 30
            };
            summaryPanel.Controls.Add(summaryTitle);

            this.Controls.Add(summaryPanel);
        }

        private void CreateStatusBar()
        {
            statusLabel = new Label
            {
                Text = "Ready",
                Height = 25,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            this.Controls.Add(statusLabel);
        }

        private void ShowDashboardOverview()
        {
            contentPanel.Controls.Clear();

            var overviewPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            // Welcome message
            var welcomeLabel = new Label
            {
                Text = "Town of Wiley Budget Management System",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48),
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter
            };
            overviewPanel.Controls.Add(welcomeLabel);

            // Quick stats panel
            var statsPanel = CreateQuickStatsPanel();
            statsPanel.Dock = DockStyle.Top;
            statsPanel.Height = 200;
            overviewPanel.Controls.Add(statsPanel);

            // Recent activity or instructions
            var instructionsPanel = CreateInstructionsPanel();
            instructionsPanel.Dock = DockStyle.Fill;
            overviewPanel.Controls.Add(instructionsPanel);

            contentPanel.Controls.Add(overviewPanel);
            statusLabel.Text = "Dashboard Overview loaded";
        }

        private Panel CreateQuickStatsPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var statsTitle = new Label
            {
                Text = "Quick Statistics",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40
            };
            panel.Controls.Add(statsTitle);

            // Create stat cards
            var statsContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(5)
            };

            var stats = GetQuickStatistics();
            foreach (var stat in stats)
            {
                var card = CreateStatCard(stat.Key, stat.Value);
                statsContainer.Controls.Add(card);
            }

            panel.Controls.Add(statsContainer);
            return panel;
        }

        private Panel CreateStatCard(string title, string value)
        {
            var card = new Panel
            {
                Size = new Size(200, 120),
                BackColor = Color.FromArgb(0, 122, 204),
                Margin = new Padding(5),
                Padding = new Padding(15)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 30
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);

            return card;
        }

        private Panel CreateInstructionsPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var instructionsTitle = new Label
            {
                Text = "Getting Started",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40
            };
            panel.Controls.Add(instructionsTitle);

            var instructionsText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.None,
                Text = @"Welcome to the Town of Wiley Budget Management System!

This application helps you manage and analyze utility rate studies for:

• Water District - Manage water utility revenues and expenses
• Sanitation District - Track sanitation service finances  
• Trash & Recycling - Monitor waste management operations
• Apartments Input - Handle multi-unit residential data

Key Features:
✓ Real-time budget calculations and scenarios
✓ Comprehensive validation and error checking
✓ Rate study methodology compliance
✓ Export capabilities for reporting
✓ Seasonal adjustment calculations

To get started:
1. Click on any district from the navigation menu
2. Review and update budget data as needed
3. Use Save & Validate to check your data
4. Generate reports for rate study analysis

For questions or support, contact the IT department."
            };

            panel.Controls.Add(instructionsText);
            return panel;
        }

        private Dictionary<string, string> GetQuickStatistics()
        {
            var stats = new Dictionary<string, string>();

            try
            {
                // These would typically come from actual data
                stats["Total Budget"] = "$2,150,000";
                stats["YTD Spending"] = "$1,075,000";
                stats["Customers"] = "850";
                stats["Rate Studies"] = "3 Active";
                stats["Last Updated"] = DateTime.Now.ToString("MM/dd/yyyy");
                stats["Budget Health"] = "Good";
            }
            catch (Exception)
            {
                stats["Status"] = "Loading...";
            }

            return stats;
        }

        private void ShowWaterInput()
        {
            if (waterForm == null || waterForm.IsDisposed)
            {
                waterForm = new WaterInput();
            }
            ShowFormInMainPanel(waterForm);
            statusLabel.Text = "Water District Input loaded";
        }

        private void ShowSanitationDistrictInput()
        {
            if (sanitationForm == null || sanitationForm.IsDisposed)
            {
                sanitationForm = new SanitationDistrictInput();
            }
            ShowFormInMainPanel(sanitationForm);
            statusLabel.Text = "Sanitation District Input loaded";
        }

        private void ShowTrashInput()
        {
            if (trashForm == null || trashForm.IsDisposed)
            {
                trashForm = new TrashInput();
            }
            ShowFormInMainPanel(trashForm);
            statusLabel.Text = "Trash & Recycling Input loaded";
        }

        private void ShowApartmentsInput()
        {
            if (apartmentForm == null || apartmentForm.IsDisposed)
            {
                apartmentForm = new ApartmentsInput();
            }
            ShowFormInMainPanel(apartmentForm);
            statusLabel.Text = "Apartments Input loaded";
        }

        private void ShowAIQueryPanel()
        {
            if (aiQueryForm == null || aiQueryForm.IsDisposed)
            {
                aiQueryForm = new AIQueryPanel();
            }
            ShowFormInMainPanel(aiQueryForm);
            statusLabel.Text = "AI Budget Assistant loaded - Ask natural language questions about municipal budgets";
        }

        private void ShowResources()
        {
            if (resourcesForm == null || resourcesForm.IsDisposed)
            {
                resourcesForm = new ResourcesForm();
            }
            ShowFormInMainPanel(resourcesForm);
            statusLabel.Text = "Accounting Resources loaded - GASB-compliant account library available";
        }

        private void ShowSummaryPage()
        {
            if (summaryForm == null || summaryForm.IsDisposed)
            {
                summaryForm = new SummaryPage();
            }
            ShowFormInMainPanel(summaryForm);
            statusLabel.Text = "Summary & Analysis loaded";
        }

        private void ShowReports()
        {
            if (reportsForm == null || reportsForm.IsDisposed)
            {
                reportsForm = new ReportsForm();
            }
            ShowFormInMainPanel(reportsForm);
            statusLabel.Text = "Reports loaded";
        }

        private void ShowSettings()
        {
            MessageBox.Show("Settings functionality will be implemented in a future version.",
                "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "Settings - Coming Soon";
        }

        private void ShowFormInMainPanel(Form form)
        {
            try
            {
                // Remove previous controls
                contentPanel.Controls.Clear();

                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.Dock = DockStyle.Fill;
                form.Parent = contentPanel;

                contentPanel.Controls.Add(form);
                form.Show();
                form.BringToFront();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Error loading form";
            }
        }

        private void LoadSummaryData()
        {
            try
            {
                // Update summary panel with current data
                UpdateSummaryPanel();
                statusLabel.Text = "Dashboard ready";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error loading summary: {ex.Message}";
            }
        }

        private void UpdateSummaryPanel()
        {
            // Clear existing summary except title
            var title = summaryPanel.Controls.OfType<Label>().FirstOrDefault();
            summaryPanel.Controls.Clear();
            if (title != null)
            {
                summaryPanel.Controls.Add(title);
            }

            // Add summary information
            var summaryText = new Label
            {
                Text = GetSummaryText(),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(45, 45, 48),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(10)
            };

            summaryPanel.Controls.Add(summaryText);
        }

        private string GetSummaryText()
        {
            try
            {
                var summary = $"Rate Study Status: Active | ";
                summary += $"Last Update: {DateTime.Now:MM/dd/yyyy HH:mm} | ";
                summary += $"User: {Environment.UserName} | ";
                summary += $"Validation: Ready for Review";

                return summary;
            }
            catch (Exception)
            {
                return "Summary information loading...";
            }
        }

        // Event handlers for legacy compatibility
        private void btnWaterInput_Click(object sender, EventArgs e)
        {
            ShowWaterInput();
        }

        private void btnSanitationDistrictInput_Click(object sender, EventArgs e)
        {
            ShowSanitationDistrictInput();
        }

        private void btnApartmentsInput_Click(object sender, EventArgs e)
        {
            ShowApartmentsInput();
        }

        private void btnTrashInput_Click(object sender, EventArgs e)
        {
            ShowTrashInput();
        }

        private void btnSummaryPage_Click(object sender, EventArgs e)
        {
            ShowSummaryPage();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            ShowReports();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Clean up child forms
            waterForm?.Dispose();
            sanitationForm?.Dispose();
            trashForm?.Dispose();
            apartmentForm?.Dispose();
            summaryForm?.Dispose();
            reportsForm?.Dispose();
            aiQueryForm?.Dispose();
            resourcesForm?.Dispose();

            base.OnFormClosing(e);
        }

        // Public methods for external access
        public void RefreshSummary()
        {
            LoadSummaryData();
        }

        public void ShowSpecificForm(string formName)
        {
            switch (formName?.ToLower())
            {
                case "water":
                    ShowWaterInput();
                    break;
                case "sanitation":
                    ShowSanitationDistrictInput();
                    break;
                case "trash":
                    ShowTrashInput();
                    break;
                case "apartments":
                    ShowApartmentsInput();
                    break;
                case "ai":
                case "aiquery":
                    ShowAIQueryPanel();
                    break;
                case "resources":
                case "accounting":
                    ShowResources();
                    break;
                case "summary":
                    ShowSummaryPage();
                    break;
                case "reports":
                    ShowReports();
                    break;
                default:
                    ShowDashboardOverview();
                    break;
            }
        }
    }
}
