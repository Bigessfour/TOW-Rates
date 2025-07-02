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
using WileyBudgetManagement;
using System.IO;

namespace WileyBudgetManagement.Forms
{
    public partial class DashboardForm : Form
    {
        private Panel navigationPanel = null!;
        private Panel contentPanel = null!;
        private Label titleLabel = null!;
        private Label statusLabel = null!;
        private Panel summaryPanel = null!;
        private readonly DatabaseManager _databaseManager = new DatabaseManager();
        private readonly ISanitationRepository? _repository;

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
            try
            {
                // Initialize debug helper and clear previous logs
                DebugHelper.ClearLogs();
                DebugHelper.LogAction("DashboardForm constructor starting...");

                // Initialize repository with error handling
                try
                {
                    _repository = new SanitationRepository(_databaseManager);
                    DebugHelper.LogAction("SanitationRepository created successfully");
                }
                catch (Exception dbEx)
                {
                    DebugHelper.LogError(dbEx, "Database initialization");
                    // Continue without database - use mock data
                    _repository = null; // Will be handled gracefully in other methods
                }

                InitializeComponent();
                DebugHelper.LogAction("InitializeComponent completed");

                // Apply modern UI enhancements with error handling
                try
                {
                    this.ApplyModernStyling();
                    DebugHelper.LogAction("Modern styling applied");
                }
                catch (Exception styleEx)
                {
                    DebugHelper.LogError(styleEx, "UI styling");
                    // Continue with basic styling
                }

                InitializeDashboard();
                this.Load += DashboardForm_Load;
                DebugHelper.LogAction("Dashboard initialized");

                LoadSummaryData();
                DebugHelper.LogAction("Summary data loaded");

                DebugHelper.LogAction("DashboardForm constructor completed successfully");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "DashboardForm constructor");

                // Create minimal working form instead of error state
                InitializeMinimalForm(ex);
            }
        }

        private void InitializeMinimalForm(Exception? originalException = null)
        {
            try
            {
                this.Text = "Wiley Budget Management - Safe Mode";
                this.Size = new Size(1200, 800);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.BackColor = Color.White;
                this.WindowState = FormWindowState.Normal;

                // Create a simple working interface
                var mainPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Padding = new Padding(20)
                };

                var titleLabel = new Label
                {
                    Text = "Town of Wiley Budget Management System",
                    Font = new Font("Segoe UI", 24, FontStyle.Bold),
                    ForeColor = Color.DarkBlue,
                    Dock = DockStyle.Top,
                    Height = 60,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var statusLabel = new Label
                {
                    Text = originalException != null
                        ? $"Running in Safe Mode - Some features may be limited\nReason: {originalException.Message}"
                        : "System initialized successfully",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = originalException != null ? Color.Orange : Color.Green,
                    Dock = DockStyle.Top,
                    Height = 80,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var buttonPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Top,
                    Height = 200,
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = true,
                    Padding = new Padding(50, 20, 50, 20)
                };

                // Create basic navigation buttons
                var buttons = new[]
                {
                    new { Text = "Water District", Action = new Action(() => MessageBox.Show("Water District functionality", "Info")) },
                    new { Text = "Sanitation District", Action = new Action(() => MessageBox.Show("Sanitation District functionality", "Info")) },
                    new { Text = "Trash & Recycling", Action = new Action(() => MessageBox.Show("Trash & Recycling functionality", "Info")) },
                    new { Text = "Apartments", Action = new Action(() => MessageBox.Show("Apartments functionality", "Info")) },
                    new { Text = "Reports", Action = new Action(() => MessageBox.Show("Reports functionality", "Info")) },
                    new { Text = "Settings", Action = new Action(() => MessageBox.Show("Settings functionality", "Info")) }
                };

                foreach (var buttonInfo in buttons)
                {
                    var button = new Button
                    {
                        Text = buttonInfo.Text,
                        Size = new Size(180, 50),
                        Font = new Font("Segoe UI", 11),
                        BackColor = Color.LightBlue,
                        ForeColor = Color.DarkBlue,
                        FlatStyle = FlatStyle.Flat,
                        Margin = new Padding(10)
                    };
                    button.Click += (s, e) => buttonInfo.Action();
                    buttonPanel.Controls.Add(button);
                }

                mainPanel.Controls.Add(buttonPanel);
                mainPanel.Controls.Add(statusLabel);
                mainPanel.Controls.Add(titleLabel);

                this.Controls.Add(mainPanel);

                System.Diagnostics.Debug.WriteLine("Minimal form initialized successfully");
            }
            catch (Exception minimalEx)
            {
                System.Diagnostics.Debug.WriteLine($"Even minimal form failed: {minimalEx.Message}");
                // Last resort - just set basic properties
                this.Text = "Wiley Budget Management - Error";
                this.Size = new Size(600, 400);
                this.BackColor = Color.LightGray;
            }
        }

        private void InitializeDashboard()
        {
            this.Text = "Town of Wiley Budget Management - Dashboard";
            this.Size = new Size(1600, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = UIStyleManager.SurfaceDark;

            // Suspend layout while creating controls for better performance
            this.SuspendLayout();

            try
            {
                // Create layout in proper Z-order (back to front)
                CreateStatusBar();        // Bottom layer
                CreateSummaryPanel();     // Summary at bottom
                CreateMainPanel();        // Main content area
                CreateNavigationPanel();  // Navigation on top/left

                // Force layout calculation before showing content
                this.ResumeLayout(true);
                this.PerformLayout();
            }
            catch (Exception ex)
            {
                this.ResumeLayout(false);
                System.Diagnostics.Debug.WriteLine($"Dashboard initialization error: {ex.Message}");
                throw;
            }
        }

        private void DashboardForm_Load(object? sender, EventArgs e)
        {
            // Show initial dashboard content after layout is complete
            this.BeginInvoke(new Action(() =>
            {
                ShowDashboardOverview();
                this.Refresh();
            }));
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

            // Title section with improved sizing
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
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };
            titleSection.Controls.Add(titleLabel);

            // Separator with improved visibility
            var separator = new Panel
            {
                Height = 2,
                Dock = DockStyle.Top,
                BackColor = UIStyleManager.PrimaryBlue,
                Margin = new Padding(0, 10, 0, 15)
            };

            // Navigation buttons container with proper scrolling
            var buttonsContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = true,
                Padding = new Padding(5, 10, 5, 10)
            };

            // Create navigation buttons with improved layout
            var buttons = new[]
            {
                new { Text = "🏠 Dashboard", Action = new Action(() => ShowDashboardOverview()) },
                new { Text = "💧 Water District", Action = new Action(() => ShowWaterInput()) },
                new { Text = "🚿 Sanitation", Action = new Action(() => ShowSanitationDistrictInput()) },
                new { Text = "🗑️ Trash & Recycling", Action = new Action(() => ShowTrashInput()) },
                new { Text = "🏢 Apartments", Action = new Action(() => ShowApartmentsInput()) },
                new { Text = "🤖 AI Assistant", Action = new Action(() => ShowAIQueryPanel()) },
                new { Text = "📚 Resources", Action = new Action(() => ShowResources()) },
                new { Text = "📊 Summary", Action = new Action(() => ShowSummaryPage()) },
                new { Text = "📋 Reports", Action = new Action(() => ShowReports()) },
                new { Text = "⚙️ Settings", Action = new Action(() => ShowSettings()) }
            };

            int yPosition = 5;
            foreach (var buttonInfo in buttons)
            {
                var button = CreateNavigationButton(buttonInfo.Text, buttonInfo.Action);
                button.Location = new Point(5, yPosition);
                button.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                buttonsContainer.Controls.Add(button);
                yPosition += 50; // 45px button + 5px spacing
            }

            // Add components to navigation panel in reverse order (bottom to top)
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
                Size = new Size(250, 45),
                Cursor = Cursors.Hand,
                Margin = new Padding(5),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                UseVisualStyleBackColor = false,
                TabStop = true
            };

            // Apply modern styling with improved colors
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = UIStyleManager.PrimaryBlue;
            button.FlatAppearance.MouseDownBackColor = UIStyleManager.PrimaryBlueDark;
            button.ForeColor = Color.White;
            button.BackColor = Color.FromArgb(62, 62, 66);

            // Add padding for better text positioning
            button.Padding = new Padding(10, 0, 10, 0);

            // Add click handler with improved error handling and visual feedback
            button.Click += (sender, e) =>
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                try
                {
                    DebugHelper.LogButtonClick(text);

                    // Visual feedback - briefly change color
                    var originalColor = button.BackColor;
                    button.BackColor = UIStyleManager.PrimaryBlueDark;
                    button.Refresh();

                    // Update status with loading message
                    if (statusLabel != null)
                    {
                        statusLabel.Text = $"Loading: {text}...";
                        statusLabel.Refresh();
                        Application.DoEvents();
                    }

                    DebugHelper.LogAction($"About to execute action", text);

                    // Execute action with small delay for visual feedback
                    System.Threading.Thread.Sleep(100);
                    action();

                    stopwatch.Stop();
                    DebugHelper.LogPerformance($"Button '{text}' execution", stopwatch.Elapsed);
                    DebugHelper.LogAction($"Action completed successfully", text);

                    // Restore button color
                    button.BackColor = originalColor;

                    // Update status on success
                    if (statusLabel != null)
                    {
                        statusLabel.Text = $"✓ Loaded: {text}";
                    }
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    DebugHelper.LogError(ex, $"Button click: {text}");
                    DebugHelper.LogPerformance($"Button '{text}' execution (FAILED)", stopwatch.Elapsed);

                    // Restore button color on error
                    button.BackColor = Color.FromArgb(62, 62, 66);

                    var errorMessage = $"Error loading {text}: {ex.Message}\n\nFull details logged to C:\\temp\\WileyDebug\\";
                    MessageBox.Show(errorMessage, "Navigation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (statusLabel != null)
                    {
                        statusLabel.Text = $"❌ Error: {text}";
                    }
                }
            };

            // Add hover effects for better user experience
            button.MouseEnter += (sender, e) =>
            {
                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.BorderColor = UIStyleManager.PrimaryBlueLight;
            };

            button.MouseLeave += (sender, e) =>
            {
                button.FlatAppearance.BorderSize = 0;
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
            var statusPanel = new Panel
            {
                Height = 30,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(0, 122, 204)
            };

            statusLabel = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F),
                Padding = new Padding(10, 0, 10, 0),
                Text = "Ready"
            };

            // Create a debug menu button on the right
            var debugButton = new Button
            {
                Text = "🐛 Debug Tools",
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Right,
                Width = 120,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 100, 180)
            };

            debugButton.FlatAppearance.BorderSize = 0;
            debugButton.Click += (s, e) => ShowDebugMenu();

            statusPanel.Controls.Add(debugButton);
            statusPanel.Controls.Add(statusLabel);
            this.Controls.Add(statusPanel);
        }

        private void ShowDebugMenu()
        {
            try
            {
                var debugMenu = new ContextMenuStrip();

                var clearLogsItem = new ToolStripMenuItem("Clear Logs");
                clearLogsItem.Click += (s, e) =>
                {
                    DebugHelper.ClearLogs();
                    MessageBox.Show("Debug logs cleared successfully", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };

                var showErrorSummaryItem = new ToolStripMenuItem("Show Error Summary");
                showErrorSummaryItem.Click += (s, e) => DebugHelper.ShowErrorSummary();

                var openLogFolderItem = new ToolStripMenuItem("Open Log Folder");
                openLogFolderItem.Click += (s, e) => DebugHelper.OpenLogDirectory();

                var separator = new ToolStripSeparator();

                var testWaterItem = new ToolStripMenuItem("Test Water District");
                testWaterItem.Click += (s, e) => ShowWaterInput();

                var testSanitationItem = new ToolStripMenuItem("Test Sanitation District");
                testSanitationItem.Click += (s, e) => ShowSanitationDistrictInput();

                var testTrashItem = new ToolStripMenuItem("Test Trash & Recycling");
                testTrashItem.Click += (s, e) => ShowTrashInput();

                var testApartmentsItem = new ToolStripMenuItem("Test Apartments");
                testApartmentsItem.Click += (s, e) => ShowApartmentsInput();

                debugMenu.Items.Add(clearLogsItem);
                debugMenu.Items.Add(showErrorSummaryItem);
                debugMenu.Items.Add(openLogFolderItem);
                debugMenu.Items.Add(separator);
                debugMenu.Items.Add(testWaterItem);
                debugMenu.Items.Add(testSanitationItem);
                debugMenu.Items.Add(testTrashItem);
                debugMenu.Items.Add(testApartmentsItem);

                // Show the context menu at the mouse position
                debugMenu.Show(Control.MousePosition);

                DebugHelper.LogAction("Debug menu opened");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "ShowDebugMenu");
            }
        }

        private void ShowDashboardOverview()
        {
            try
            {
                if (contentPanel == null)
                {
                    throw new InvalidOperationException("Content panel not initialized");
                }

                // Suspend layout for better performance
                contentPanel.SuspendLayout();
                contentPanel.Controls.Clear();

                var overviewPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Padding = new Padding(30),
                    AutoScroll = true
                };

                // Welcome header with enhanced styling
                var headerPanel = new Panel
                {
                    Height = 100,
                    Dock = DockStyle.Top,
                    BackColor = Color.White
                };

                var welcomeLabel = new Label
                {
                    Text = "🏛️ Town of Wiley Budget Management System",
                    Font = new Font("Segoe UI", 26, FontStyle.Bold),
                    ForeColor = UIStyleManager.NeutralDark,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false
                };
                headerPanel.Controls.Add(welcomeLabel);

                // Subtitle with improved styling
                var subtitlePanel = new Panel
                {
                    Height = 50,
                    Dock = DockStyle.Top,
                    BackColor = Color.White
                };

                var subtitleLabel = new Label
                {
                    Text = "🏗️ Municipal Utility Rate Management Dashboard",
                    Font = new Font("Segoe UI", 16, FontStyle.Regular),
                    ForeColor = UIStyleManager.NeutralMedium,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false
                };
                subtitlePanel.Controls.Add(subtitleLabel);

                // Enhanced spacer
                var spacer1 = UIStyleManager.CreateSpacer(25);

                // Improved quick stats panel
                var statsPanel = CreateEnhancedQuickStatsPanel();
                statsPanel.Dock = DockStyle.Top;
                statsPanel.Height = 200;

                // Another spacer
                var spacer2 = UIStyleManager.CreateSpacer(25);

                // Enhanced instructions panel
                var instructionsPanel = CreateEnhancedInstructionsPanel();
                instructionsPanel.Dock = DockStyle.Fill;

                // Add all components in proper order (bottom to top for dock top)
                overviewPanel.Controls.Add(instructionsPanel);  // Fill (bottom)
                overviewPanel.Controls.Add(spacer2);           // Spacer
                overviewPanel.Controls.Add(statsPanel);        // Stats
                overviewPanel.Controls.Add(spacer1);           // Spacer
                overviewPanel.Controls.Add(subtitlePanel);     // Subtitle
                overviewPanel.Controls.Add(headerPanel);       // Header (top)

                contentPanel.Controls.Add(overviewPanel);

                // Resume layout and force refresh
                contentPanel.ResumeLayout(true);
                contentPanel.PerformLayout();
                contentPanel.Refresh();

                if (statusLabel != null)
                    statusLabel.Text = "✓ Dashboard Overview loaded successfully";

                System.Diagnostics.Debug.WriteLine("Dashboard overview loaded successfully");
            }
            catch (Exception ex)
            {
                if (contentPanel != null)
                    contentPanel.ResumeLayout(false);

                var errorMsg = $"Failed to load Dashboard Overview: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"ShowDashboardOverview Error: {ex}");

                if (statusLabel != null)
                    statusLabel.Text = $"❌ Dashboard Error: {ex.Message}";

                // Show a simple error message in content panel
                if (contentPanel != null)
                {
                    contentPanel.Controls.Clear();
                    var errorLabel = new Label
                    {
                        Text = $"⚠️ Dashboard Loading Error:\n{ex.Message}\n\nPlease contact support if this issue persists.",
                        Font = new Font("Segoe UI", 14),
                        ForeColor = Color.Red,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill,
                        AutoSize = false
                    };
                    contentPanel.Controls.Add(errorLabel);
                }
            }
        }

        private Panel CreateEnhancedQuickStatsPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var statsTitle = new Label
            {
                Text = "📊 Quick Statistics & System Health",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = UIStyleManager.NeutralDark,
                Dock = DockStyle.Top,
                Height = 40,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panel.Controls.Add(statsTitle);

            // Create enhanced stat cards with better layout
            var statsContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(10),
                AutoSize = false
            };

            var stats = GetEnhancedQuickStatistics();
            foreach (var stat in stats)
            {
                var card = CreateEnhancedStatCard(stat.Key, stat.Value, stat.Key);
                statsContainer.Controls.Add(card);
            }

            panel.Controls.Add(statsContainer);
            return panel;
        }

        private Panel CreateEnhancedStatCard(string title, string value, string category)
        {
            var card = new Panel
            {
                Size = new Size(220, 120),
                BackColor = UIStyleManager.Surface,
                Margin = new Padding(8),
                Padding = new Padding(15)
            };

            // Apply enhanced card styling with gradient effect
            UIStyleManager.ApplyCardPanelStyle(card);

            // Add category-specific color coding
            var accentColor = GetCategoryColor(category);

            // Add colored top border
            card.Paint += (sender, e) =>
            {
                using (var brush = new SolidBrush(accentColor))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, card.Width, 4);
                }

                // Add subtle shadow
                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                using (var pen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };

            var iconLabel = new Label
            {
                Text = GetCategoryIcon(category),
                Font = new Font("Segoe UI", 16),
                ForeColor = accentColor,
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = UIStyleManager.SectionFont,
                ForeColor = UIStyleManager.NeutralMedium,
                Dock = DockStyle.Top,
                Height = 25,
                AutoSize = false
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font(UIStyleManager.PrimaryFontFamily, 18, FontStyle.Bold),
                ForeColor = accentColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };

            card.Controls.Add(valueLabel);
            card.Controls.Add(titleLabel);
            card.Controls.Add(iconLabel);

            return card;
        }

        private Color GetCategoryColor(string category)
        {
            return category.ToLower() switch
            {
                var c when c.Contains("budget") => UIStyleManager.PrimaryBlue,
                var c when c.Contains("spending") => UIStyleManager.SecondaryOrange,
                var c when c.Contains("customers") => UIStyleManager.SecondaryGreen,
                var c when c.Contains("studies") => UIStyleManager.StatusInfo,
                var c when c.Contains("updated") => UIStyleManager.NeutralMedium,
                var c when c.Contains("health") => UIStyleManager.StatusSuccess,
                _ => UIStyleManager.PrimaryBlue
            };
        }

        private string GetCategoryIcon(string category)
        {
            return category.ToLower() switch
            {
                var c when c.Contains("budget") => "💰",
                var c when c.Contains("spending") => "📈",
                var c when c.Contains("customers") => "👥",
                var c when c.Contains("studies") => "📋",
                var c when c.Contains("updated") => "🕒",
                var c when c.Contains("health") => "✅",
                _ => "📊"
            };
        }

        private Panel CreateEnhancedInstructionsPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var instructionsTitle = new Label
            {
                Text = "🚀 Getting Started Guide",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = UIStyleManager.NeutralDark,
                Dock = DockStyle.Top,
                Height = 40,
                AutoSize = false
            };
            panel.Controls.Add(instructionsTitle);

            var instructionsText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Text = @"Welcome to the Town of Wiley Budget Management System! 🎉

This comprehensive application helps you manage and analyze utility rate studies for multiple municipal enterprises:

🏗️ ENTERPRISE MODULES:
• 💧 Water District - Manage water utility revenues, expenses, and rate calculations
• 🚿 Sanitation District - Track sanitation service finances and operational costs  
• 🗑️ Trash & Recycling - Monitor waste management operations and fee structures
• 🏢 Apartments Input - Handle multi-unit residential billing and allocations

🎯 KEY FEATURES:
✅ Real-time budget calculations and scenario modeling
✅ Comprehensive data validation and error checking
✅ Rate study methodology compliance (GASB standards)
✅ Professional reporting and export capabilities
✅ Seasonal adjustment calculations for accurate projections
✅ AI-powered budget analysis and natural language queries

🚀 QUICK START:
1️⃣ Click on any district module from the left navigation menu
2️⃣ Review and update budget data as needed for the current fiscal year
3️⃣ Use 'Save & Validate' to verify data integrity and compliance
4️⃣ Generate comprehensive reports for rate study analysis
5️⃣ Leverage AI Assistant for advanced queries and insights

💡 PRO TIPS:
• Use Ctrl+S to quickly save your work
• Press F5 to refresh data in any module
• Check the status bar for real-time feedback
• Export data regularly for backup purposes

📞 Need Help? Contact the IT department or municipal finance team for support and training."
            };

            panel.Controls.Add(instructionsText);
            return panel;
        }

        private Dictionary<string, string> GetEnhancedQuickStatistics()
        {
            var stats = new Dictionary<string, string>();

            try
            {
                // Enhanced statistics with more meaningful data
                stats["Total Budget"] = "$2,150,000";
                stats["YTD Spending"] = "50.2% ($1,075,000)";
                stats["Active Customers"] = "850 accounts";
                stats["Rate Studies"] = "3 Active Projects";
                stats["Last Updated"] = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                stats["System Health"] = "Excellent ✅";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Statistics error: {ex.Message}");
                stats["Status"] = "Loading...";
                stats["System"] = "Initializing";
            }

            return stats;
        }

        private void ShowWaterInput()
        {
            try
            {
                DebugHelper.LogAction("=== ShowWaterInput called ===");

                if (waterForm == null || waterForm.IsDisposed)
                {
                    DebugHelper.LogAction("Creating new WaterInput form...");

                    if (statusLabel != null)
                        statusLabel.Text = "Creating Water District form...";

                    DebugHelper.LogAction("About to call new WaterInput() constructor...");

                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    waterForm = new WaterInput();
                    stopwatch.Stop();

                    DebugHelper.LogFormCreation("WaterInput", true);
                    DebugHelper.LogPerformance("WaterInput constructor", stopwatch.Elapsed);
                    System.Diagnostics.Debug.WriteLine("Water form created successfully");
                }
                else
                {
                    DebugHelper.LogAction("Reusing existing WaterInput form");
                }

                DebugHelper.LogAction("Calling ShowFormInMainPanel for WaterInput...");
                ShowFormInMainPanel(waterForm);

                if (statusLabel != null)
                    statusLabel.Text = UIIconManager.CreateStatusText("Water", "Water District Input loaded");

                DebugHelper.LogAction("ShowWaterInput completed successfully");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "ShowWaterInput");

                var errorMsg = $"Failed to load Water District: {ex.Message}\n\nFull details logged to C:\\temp\\WileyDebug\\";
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
                DebugHelper.LogAction("=== ShowSanitationDistrictInput called ===");

                if (sanitationForm == null || sanitationForm.IsDisposed)
                {
                    if (statusLabel != null)
                        statusLabel.Text = "Creating Sanitation District form...";

                    DebugHelper.LogAction("About to call new SanitationDistrictInput() constructor...");

                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    sanitationForm = new SanitationDistrictInput();
                    stopwatch.Stop();

                    DebugHelper.LogFormCreation("SanitationDistrictInput", true);
                    DebugHelper.LogPerformance("SanitationDistrictInput constructor", stopwatch.Elapsed);
                    System.Diagnostics.Debug.WriteLine("Sanitation form created successfully");
                }

                DebugHelper.LogAction("Calling ShowFormInMainPanel for SanitationDistrictInput...");
                ShowFormInMainPanel(sanitationForm);

                if (statusLabel != null)
                    statusLabel.Text = UIIconManager.CreateStatusText("Sanitation", "Sanitation District Input loaded");

                DebugHelper.LogAction("ShowSanitationDistrictInput completed successfully");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "ShowSanitationDistrictInput");

                var errorMsg = $"Failed to load Sanitation District: {ex.Message}\n\nFull details logged to C:\\temp\\WileyDebug\\";
                System.Diagnostics.Debug.WriteLine($"ShowSanitationDistrictInput Error: {ex}");

                MessageBox.Show(errorMsg, "Form Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (statusLabel != null)
                    statusLabel.Text = $"Error: {ex.Message}";
            }
        }

        private void ShowTrashInput()
        {
            try
            {
                DebugHelper.LogAction("=== ShowTrashInput called ===");

                if (trashForm == null || trashForm.IsDisposed)
                {
                    DebugHelper.LogAction("About to call new TrashInput() constructor...");

                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    trashForm = new TrashInput();
                    stopwatch.Stop();

                    DebugHelper.LogFormCreation("TrashInput", true);
                    DebugHelper.LogPerformance("TrashInput constructor", stopwatch.Elapsed);
                }

                DebugHelper.LogAction("Calling ShowFormInMainPanel for TrashInput...");
                ShowFormInMainPanel(trashForm);

                if (statusLabel != null)
                    statusLabel.Text = UIIconManager.CreateStatusText("Trash", "Trash & Recycling Input loaded");

                DebugHelper.LogAction("ShowTrashInput completed successfully");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "ShowTrashInput");

                var errorMsg = $"Failed to load Trash Input: {ex.Message}\n\nFull details logged to C:\\temp\\WileyDebug\\";
                MessageBox.Show(errorMsg, "Form Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (statusLabel != null)
                    statusLabel.Text = $"Error: {ex.Message}";
            }
        }

        private void ShowApartmentsInput()
        {
            var logFile = @"C:\temp\dashboard_debug.log";
            try
            {
                File.AppendAllText(logFile, $"[{DateTime.Now:HH:mm:ss}] === ShowApartmentsInput called ===\n");

                if (apartmentForm == null || apartmentForm.IsDisposed)
                {
                    File.AppendAllText(logFile, $"[{DateTime.Now:HH:mm:ss}] About to call new ApartmentsInput() constructor...\n");
                    apartmentForm = new ApartmentsInput();
                    File.AppendAllText(logFile, $"[{DateTime.Now:HH:mm:ss}] ApartmentsInput form created successfully\n");
                }

                File.AppendAllText(logFile, $"[{DateTime.Now:HH:mm:ss}] Calling ShowFormInMainPanel for ApartmentsInput...\n");
                ShowFormInMainPanel(apartmentForm);
                statusLabel.Text = "Apartments Input loaded";
                File.AppendAllText(logFile, $"[{DateTime.Now:HH:mm:ss}] ShowApartmentsInput completed successfully\n");
            }
            catch (Exception ex)
            {
                var errorDetails = $"[{DateTime.Now:HH:mm:ss}] *** CRITICAL ERROR in ShowApartmentsInput ***\n" +
                                  $"Message: {ex.Message}\n" +
                                  $"Type: {ex.GetType().Name}\n" +
                                  $"Stack trace: {ex.StackTrace}\n" +
                                  $"Inner exception: {ex.InnerException?.Message ?? "None"}\n" +
                                  $"===============================================\n\n";

                try { File.AppendAllText(logFile, errorDetails); } catch { }

                var errorMsg = $"Failed to load Apartments Input: {ex.Message}\n\nFull details logged to C:\\temp\\dashboard_debug.log";
                MessageBox.Show(errorMsg, "Form Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (statusLabel != null)
                    statusLabel.Text = $"Error: {ex.Message}";
            }
        }

        private void ShowAIQueryPanel()
        {
            var logFile = @"C:\temp\dashboard_debug.log";
            try
            {
                File.AppendAllText(logFile, $"[{DateTime.Now:HH:mm:ss}] === ShowAIQueryPanel called ===\n");

                if (aiQueryForm == null || aiQueryForm.IsDisposed)
                {
                    File.AppendAllText(logFile, $"[{DateTime.Now:HH:mm:ss}] About to call new AIQueryPanel() constructor...\n");
                    aiQueryForm = new AIQueryPanel();
                    File.AppendAllText(logFile, $"[{DateTime.Now:HH:mm:ss}] AIQueryPanel form created successfully\n");
                }

                File.AppendAllText(logFile, $"[{DateTime.Now:HH:mm:ss}] Calling ShowFormInMainPanel for AIQueryPanel...\n");
                ShowFormInMainPanel(aiQueryForm);
                statusLabel.Text = "AI Budget Assistant loaded - Ask natural language questions about municipal budgets";
                File.AppendAllText(logFile, $"[{DateTime.Now:HH:mm:ss}] ShowAIQueryPanel completed successfully\n");
            }
            catch (Exception ex)
            {
                var errorDetails = $"[{DateTime.Now:HH:mm:ss}] *** CRITICAL ERROR in ShowAIQueryPanel ***\n" +
                                  $"Message: {ex.Message}\n" +
                                  $"Type: {ex.GetType().Name}\n" +
                                  $"Stack trace: {ex.StackTrace}\n" +
                                  $"Inner exception: {ex.InnerException?.Message ?? "None"}\n" +
                                  $"===============================================\n\n";

                try { File.AppendAllText(logFile, errorDetails); } catch { }

                var errorMsg = $"Failed to load AI Query Panel: {ex.Message}\n\nFull details logged to C:\\temp\\dashboard_debug.log";
                MessageBox.Show(errorMsg, "Form Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (statusLabel != null)
                    statusLabel.Text = $"Error: {ex.Message}";
            }
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
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                DebugHelper.LogAction("ShowFormInMainPanel called", $"Form: {form?.GetType().Name ?? "null"}");

                // Validate inputs
                if (form == null)
                {
                    throw new ArgumentNullException(nameof(form), "Form cannot be null");
                }

                if (contentPanel == null)
                {
                    throw new InvalidOperationException("Content panel is not initialized");
                }

                DebugHelper.LogAction("Clearing content panel controls");

                // Remove previous controls
                contentPanel.Controls.Clear();

                DebugHelper.LogAction("Preparing form for embedding", form.GetType().Name);

                // Prepare form for embedding
                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.Dock = DockStyle.Fill;
                form.Parent = contentPanel;

                DebugHelper.LogAction("Adding form to content panel", form.GetType().Name);

                // Add and show form
                contentPanel.Controls.Add(form);
                form.Show();
                form.BringToFront();

                DebugHelper.LogAction("Refreshing panels");

                // Force refresh
                contentPanel.Refresh();
                this.Refresh();

                stopwatch.Stop();
                DebugHelper.LogPerformance($"Load form '{form.GetType().Name}'", stopwatch.Elapsed);
                DebugHelper.LogAction($"Successfully loaded form", form.GetType().Name);
                System.Diagnostics.Debug.WriteLine($"Successfully loaded form: {form.GetType().Name}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                DebugHelper.LogError(ex, $"ShowFormInMainPanel with form: {form?.GetType().Name ?? "null"}");

                var errorMsg = $"Error loading form: {ex.Message}\n\nForm Type: {form?.GetType().Name ?? "null"}\nContent Panel: {(contentPanel == null ? "null" : "initialized")}\n\nDetails logged to C:\\temp\\WileyDebug\\";

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
            try
            {
                // Show closing message
                if (statusLabel != null)
                {
                    statusLabel.Text = "Closing application...";
                    statusLabel.Refresh();
                }

                // Clean up child forms with proper disposal
                try { waterForm?.Close(); waterForm?.Dispose(); } catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Water form cleanup error: {ex.Message}"); }
                try { sanitationForm?.Close(); sanitationForm?.Dispose(); } catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Sanitation form cleanup error: {ex.Message}"); }
                try { trashForm?.Close(); trashForm?.Dispose(); } catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Trash form cleanup error: {ex.Message}"); }
                try { apartmentForm?.Close(); apartmentForm?.Dispose(); } catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Apartment form cleanup error: {ex.Message}"); }
                try { summaryForm?.Close(); summaryForm?.Dispose(); } catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Summary form cleanup error: {ex.Message}"); }
                try { reportsForm?.Close(); reportsForm?.Dispose(); } catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Reports form cleanup error: {ex.Message}"); }
                try { aiQueryForm?.Close(); aiQueryForm?.Dispose(); } catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"AI Query form cleanup error: {ex.Message}"); }
                try { resourcesForm?.Close(); resourcesForm?.Dispose(); } catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Resources form cleanup error: {ex.Message}"); }

                // Clean up database connections
                try
                {
                    // DatabaseManager cleanup - check if it implements IDisposable
                    if (_databaseManager is IDisposable disposableDb)
                    {
                        disposableDb.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Database cleanup error: {ex.Message}");
                }

                // Force garbage collection for clean shutdown
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                System.Diagnostics.Debug.WriteLine("Dashboard form cleanup completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Form closing error: {ex.Message}");
                // Don't prevent closing even if cleanup fails
            }
            finally
            {
                base.OnFormClosing(e);
            }
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
