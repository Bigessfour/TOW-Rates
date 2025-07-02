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
            
            // Apply modern UI enhancements
            this.ApplyModernStyling();
            
            InitializeDashboard();
            LoadSummaryData();
        }

        private void InitializeDashboard()
        {
            this.Text = "Town of Wiley Budget Management - Dashboard";
            this.Size = new Size(1600, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = UIStyleManager.SurfaceDark;

            // Create layout in proper Z-order (back to front)
            CreateStatusBar();        // Bottom layer
            CreateSummaryPanel();     // Summary at bottom
            CreateMainPanel();        // Main content area
            CreateNavigationPanel();  // Navigation on top/left
            
            // Show initial dashboard content
            ShowDashboardOverview();
            
            // Force layout update
            this.PerformLayout();
            this.Refresh();
        }

        private void CreateNavigationPanel()
        {
            navigationPanel = new Panel
            {
                Width = 280,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(45, 45, 48),
                Padding = new Padding(15)
            };

            // Title section
            var titleSection = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };

            titleLabel = new Label
            {
                Text = "Budget Management",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            titleSection.Controls.Add(titleLabel);

            // Separator
            var separator = new Panel
            {
                Height = 2,
                Dock = DockStyle.Top,
                BackColor = UIStyleManager.PrimaryBlue,
                Margin = new Padding(0, 10, 0, 20)
            };

            // Navigation buttons container
            var buttonsContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = true
            };

            // Create navigation buttons
            var buttons = new[]
            {
                new { Text = UIIconManager.CreateButtonText("Dashboard", "Dashboard Overview"), Action = new Action(() => ShowDashboardOverview()) },
                new { Text = UIIconManager.CreateButtonText("Water", "Water District"), Action = new Action(() => ShowWaterInput()) },
                new { Text = UIIconManager.CreateButtonText("Sanitation", "Sanitation District"), Action = new Action(() => ShowSanitationDistrictInput()) },
                new { Text = UIIconManager.CreateButtonText("Trash", "Trash & Recycling"), Action = new Action(() => ShowTrashInput()) },
                new { Text = UIIconManager.CreateButtonText("Apartments", "Apartments Input"), Action = new Action(() => ShowApartmentsInput()) },
                new { Text = UIIconManager.CreateButtonText("AI", "AI Budget Assistant"), Action = new Action(() => ShowAIQueryPanel()) },
                new { Text = UIIconManager.CreateButtonText("Resources", "Resources"), Action = new Action(() => ShowResources()) },
                new { Text = UIIconManager.CreateButtonText("Summary", "Summary & Analysis"), Action = new Action(() => ShowSummaryPage()) },
                new { Text = UIIconManager.CreateButtonText("Reports", "Reports"), Action = new Action(() => ShowReports()) },
                new { Text = UIIconManager.CreateButtonText("Settings", "Settings"), Action = new Action(() => ShowSettings()) }
            };

            int yPosition = 10;
            foreach (var buttonInfo in buttons)
            {
                var button = CreateNavigationButton(buttonInfo.Text, buttonInfo.Action);
                button.Location = new Point(10, yPosition);
                buttonsContainer.Controls.Add(button);
                yPosition += 55; // 45px button + 10px spacing
            }

            // Add components to navigation panel
            navigationPanel.Controls.Add(buttonsContainer);
            navigationPanel.Controls.Add(separator);
            navigationPanel.Controls.Add(titleSection);

            this.Controls.Add(navigationPanel);
        }

        private Button CreateNavigationButton(string text, Action action)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(220, 45),
                Cursor = Cursors.Hand,
                Margin = new Padding(5),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };

            // Apply modern styling
            UIStyleManager.ApplySecondaryButtonStyle(button);
            button.FlatAppearance.MouseOverBackColor = UIStyleManager.PrimaryBlue;
            button.FlatAppearance.MouseDownBackColor = UIStyleManager.PrimaryBlueDark;
            button.ForeColor = Color.White;
            button.BackColor = Color.FromArgb(62, 62, 66);

            // Add click handler with error handling
            button.Click += (sender, e) => 
            {
                try
                {
                    // Update status
                    if (statusLabel != null)
                    {
                        statusLabel.Text = $"Loading: {text}";
                        statusLabel.Refresh();
                    }
                    
                    // Execute action
                    action();
                    
                    // Update status on success
                    if (statusLabel != null)
                    {
                        statusLabel.Text = $"Loaded: {text}";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading {text}: {ex.Message}", "Navigation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    if (statusLabel != null)
                    {
                        statusLabel.Text = $"Error loading: {text}";
                    }
                }
            };

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
            try
            {
                if (contentPanel == null)
                {
                    throw new InvalidOperationException("Content panel not initialized");
                }

                contentPanel.Controls.Clear();

                var overviewPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Padding = new Padding(30),
                    AutoScroll = true
                };

                // Welcome header with better styling
                var headerPanel = new Panel
                {
                    Height = 80,
                    Dock = DockStyle.Top,
                    BackColor = Color.White
                };

                var welcomeLabel = new Label
                {
                    Text = "Town of Wiley Budget Management System",
                    Font = new Font("Segoe UI", 24, FontStyle.Bold),
                    ForeColor = UIStyleManager.NeutralDark,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                headerPanel.Controls.Add(welcomeLabel);

                // Subtitle
                var subtitlePanel = new Panel
                {
                    Height = 40,
                    Dock = DockStyle.Top,
                    BackColor = Color.White
                };

                var subtitleLabel = new Label
                {
                    Text = "Municipal Utility Rate Management Dashboard",
                    Font = new Font("Segoe UI", 14, FontStyle.Regular),
                    ForeColor = UIStyleManager.NeutralMedium,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                subtitlePanel.Controls.Add(subtitleLabel);

                // Spacer
                var spacer1 = UIStyleManager.CreateSpacer(20);

                // Quick stats panel with improved layout
                var statsPanel = CreateQuickStatsPanel();
                statsPanel.Dock = DockStyle.Top;
                statsPanel.Height = 180;

                // Another spacer
                var spacer2 = UIStyleManager.CreateSpacer(20);

                // Instructions panel with better formatting
                var instructionsPanel = CreateInstructionsPanel();
                instructionsPanel.Dock = DockStyle.Fill;

                // Add all components in order
                overviewPanel.Controls.Add(instructionsPanel);  // Fill (bottom)
                overviewPanel.Controls.Add(spacer2);           // Spacer
                overviewPanel.Controls.Add(statsPanel);        // Stats
                overviewPanel.Controls.Add(spacer1);           // Spacer
                overviewPanel.Controls.Add(subtitlePanel);     // Subtitle
                overviewPanel.Controls.Add(headerPanel);       // Header (top)

                contentPanel.Controls.Add(overviewPanel);
                
                if (statusLabel != null)
                    statusLabel.Text = UIIconManager.CreateStatusText("Dashboard", "Dashboard Overview loaded");
                
                System.Diagnostics.Debug.WriteLine("Dashboard overview loaded successfully");
            }
            catch (Exception ex)
            {
                var errorMsg = $"Failed to load Dashboard Overview: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"ShowDashboardOverview Error: {ex}");
                
                if (statusLabel != null)
                    statusLabel.Text = $"Dashboard Error: {ex.Message}";
                
                // Show a simple error message in content panel
                if (contentPanel != null)
                {
                    contentPanel.Controls.Clear();
                    var errorLabel = new Label
                    {
                        Text = $"Dashboard Loading Error:\n{ex.Message}",
                        Font = new Font("Segoe UI", 12),
                        ForeColor = Color.Red,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill
                    };
                    contentPanel.Controls.Add(errorLabel);
                }
            }
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
                BackColor = UIStyleManager.Surface,
                Margin = new Padding(5),
                Padding = new Padding(15)
            };

            // Apply modern card styling
            UIStyleManager.ApplyCardPanelStyle(card);

            var titleLabel = new Label
            {
                Text = title,
                Font = UIStyleManager.SectionFont,
                ForeColor = UIStyleManager.NeutralMedium,
                Dock = DockStyle.Top,
                Height = 30
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font(UIStyleManager.PrimaryFontFamily, 16, FontStyle.Bold),
                ForeColor = UIStyleManager.PrimaryBlue,
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
            try
            {
                if (waterForm == null || waterForm.IsDisposed)
                {
                    if (statusLabel != null)
                        statusLabel.Text = "Creating Water District form...";
                    
                    waterForm = new WaterInput();
                    System.Diagnostics.Debug.WriteLine("Water form created successfully");
                }
                
                ShowFormInMainPanel(waterForm);
                
                if (statusLabel != null)
                    statusLabel.Text = UIIconManager.CreateStatusText("Water", "Water District Input loaded");
            }
            catch (Exception ex)
            {
                var errorMsg = $"Failed to load Water District: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"ShowWaterInput Error: {ex}");
                
                MessageBox.Show(errorMsg, "Form Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                if (statusLabel != null)
                    statusLabel.Text = $"Error: {ex.Message}";
            }
        }

        private void ShowSanitationDistrictInput()
        {
            try
            {
                if (sanitationForm == null || sanitationForm.IsDisposed)
                {
                    if (statusLabel != null)
                        statusLabel.Text = "Creating Sanitation District form...";
                    
                    sanitationForm = new SanitationDistrictInput();
                    System.Diagnostics.Debug.WriteLine("Sanitation form created successfully");
                }
                
                ShowFormInMainPanel(sanitationForm);
                
                if (statusLabel != null)
                    statusLabel.Text = UIIconManager.CreateStatusText("Sanitation", "Sanitation District Input loaded");
            }
            catch (Exception ex)
            {
                var errorMsg = $"Failed to load Sanitation District: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"ShowSanitationDistrictInput Error: {ex}");
                
                MessageBox.Show(errorMsg, "Form Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                if (statusLabel != null)
                    statusLabel.Text = $"Error: {ex.Message}";
            }
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
                // Validate inputs
                if (form == null)
                {
                    throw new ArgumentNullException(nameof(form), "Form cannot be null");
                }

                if (contentPanel == null)
                {
                    throw new InvalidOperationException("Content panel is not initialized");
                }

                // Remove previous controls
                contentPanel.Controls.Clear();

                // Prepare form for embedding
                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.Dock = DockStyle.Fill;
                form.Parent = contentPanel;

                // Add and show form
                contentPanel.Controls.Add(form);
                form.Show();
                form.BringToFront();
                
                // Force refresh
                contentPanel.Refresh();
                this.Refresh();
                
                System.Diagnostics.Debug.WriteLine($"Successfully loaded form: {form.GetType().Name}");
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error loading form: {ex.Message}\n\nForm Type: {form?.GetType().Name ?? "null"}\nContent Panel: {(contentPanel == null ? "null" : "initialized")}";
                
                MessageBox.Show(errorMsg, "Form Loading Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                if (statusLabel != null)
                {
                    statusLabel.Text = $"Error: {ex.Message}";
                }
                
                System.Diagnostics.Debug.WriteLine($"ShowFormInMainPanel Error: {ex}");
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

        // Event handlers for legacy compatibility (empty implementations)
        private void btnWaterInput_Click(object? sender, EventArgs e) => ShowWaterInput();
        private void btnSanitationDistrictInput_Click(object? sender, EventArgs e) => ShowSanitationDistrictInput();
        private void btnApartmentsInput_Click(object? sender, EventArgs e) => ShowApartmentsInput();
        private void btnTrashInput_Click(object? sender, EventArgs e) => ShowTrashInput();
        private void btnSummaryPage_Click(object? sender, EventArgs e) => ShowSummaryPage();
        private void btnReports_Click(object? sender, EventArgs e) => ShowReports();

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
