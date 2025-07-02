using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using WileyBudgetManagement.Database;
using WileyBudgetManagement.Models;

namespace WileyBudgetManagement.Forms
{
    /// <summary>
    /// Enhanced Dashboard Form with Modern UI Styling
    /// Implements comprehensive UI improvements including:
    /// - Material Design-inspired styling
    /// - Improved accessibility and usability
    /// - Consistent theme and typography
    /// - Enhanced user feedback and navigation
    /// </summary>
    public partial class DashboardFormEnhanced : Form
    {
        #region Private Fields
        
        private Panel navigationPanel = null!;
        private Panel contentPanel = null!;
        private Label titleLabel = null!;
        private Label statusLabel = null!;
        private Panel summaryPanel = null!;
        private Panel quickStatsPanel = null!;
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
        
        #endregion

        #region Constructor and Initialization
        
        public DashboardFormEnhanced()
        {
            _databaseManager = new DatabaseManager();
            _repository = new SanitationRepository(_databaseManager);

            InitializeComponent();
            InitializeEnhancedDashboard();
            LoadSummaryDataAsync();
        }

        private void InitializeEnhancedDashboard()
        {
            // Apply modern form styling
            UIStyleManager.ApplyFormStyle(this, "Town of Wiley Budget Management - Dashboard");
            this.Size = new Size(1600, 1000);
            this.WindowState = FormWindowState.Maximized;
            this.Icon = LoadApplicationIcon();

            // Create enhanced layout with improved spacing and organization
            CreateEnhancedNavigationPanel();
            CreateEnhancedMainPanel();
            CreateEnhancedSummaryPanel();
            CreateEnhancedStatusBar();
            
            // Initialize with dashboard overview
            ShowEnhancedDashboardOverview();
        }

        private Icon? LoadApplicationIcon()
        {
            try
            {
                // Placeholder for application icon
                // return new Icon("Resources/wiley-icon.ico");
                return null;
            }
            catch
            {
                return null;
            }
        }
        
        #endregion

        #region Enhanced UI Creation
        
        private void CreateEnhancedNavigationPanel()
        {
            navigationPanel = new Panel();
            UIStyleManager.ApplyNavigationPanelStyle(navigationPanel);

            // Enhanced title with better typography
            titleLabel = new Label
            {
                Text = "Budget Management",
                Font = new Font(UIStyleManager.PrimaryFontFamily, 16, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 20, 0, 0)
            };
            navigationPanel.Controls.Add(titleLabel);

            // Modern separator
            var separator = UIStyleManager.CreateSeparator(2);
            separator.BackColor = UIStyleManager.PrimaryBlue;
            navigationPanel.Controls.Add(separator);

            // Spacer for better visual hierarchy
            navigationPanel.Controls.Add(UIStyleManager.CreateSpacer(10));

            // Enhanced navigation buttons with icons and improved styling
            var buttonsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(8)
            };

            var navigationButtons = new[]
            {
                new { Text = "🏠 Dashboard Overview", Action = new Action(() => ShowEnhancedDashboardOverview()), Description = "Main dashboard with key metrics" },
                new { Text = "💧 Water District", Action = new Action(() => ShowWaterInput()), Description = "Water utility management" },
                new { Text = "🚿 Sanitation District", Action = new Action(() => ShowSanitationDistrictInput()), Description = "Sanitation services management" },
                new { Text = "🗑️ Trash & Recycling", Action = new Action(() => ShowTrashInput()), Description = "Waste management operations" },
                new { Text = "🏠 Apartments Input", Action = new Action(() => ShowApartmentsInput()), Description = "Multi-unit residential data" },
                new { Text = "🤖 AI Budget Assistant", Action = new Action(() => ShowAIQueryPanel()), Description = "Natural language budget analysis" },
                new { Text = "📚 Resources", Action = new Action(() => ShowResources()), Description = "Accounting resources and documentation" },
                new { Text = "📊 Summary & Analysis", Action = new Action(() => ShowSummaryPage()), Description = "Comprehensive financial analysis" },
                new { Text = "📋 Reports", Action = new Action(() => ShowReports()), Description = "Generate and export reports" },
                new { Text = "⚙️ Settings", Action = new Action(() => ShowSettings()), Description = "System configuration" }
            };

            for (int i = 0; i < navigationButtons.Length; i++)
            {
                var buttonInfo = navigationButtons[i];
                var button = CreateEnhancedNavigationButton(buttonInfo.Text, buttonInfo.Action, buttonInfo.Description);
                button.Location = new Point(8, 8 + (i * 50));
                buttonsPanel.Controls.Add(button);
            }

            navigationPanel.Controls.Add(buttonsPanel);
            this.Controls.Add(navigationPanel);
        }

        private Button CreateEnhancedNavigationButton(string text, Action action, string description)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(264, 42), // Slightly larger for better touch targets
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(55, 55, 58), // Slightly lighter than panel
                ForeColor = Color.White,
                Font = new Font(UIStyleManager.PrimaryFontFamily, 10, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand,
                Padding = new Padding(12, 0, 8, 0)
            };

            // Enhanced flat appearance with better hover effects
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = UIStyleManager.PrimaryBlue;
            button.FlatAppearance.MouseDownBackColor = UIStyleManager.PrimaryBlueDark;

            // Add tooltip for better UX
            var tooltip = new ToolTip();
            tooltip.SetToolTip(button, description);

            // Enhanced click handling with visual feedback
            button.Click += (sender, e) =>
            {
                // Briefly change color to show interaction
                button.BackColor = UIStyleManager.PrimaryBlueDark;
                var timer = new System.Windows.Forms.Timer { Interval = 100 };
                timer.Tick += (s, args) =>
                {
                    button.BackColor = Color.FromArgb(55, 55, 58);
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
                
                action();
            };

            return button;
        }

        private void CreateEnhancedMainPanel()
        {
            contentPanel = new Panel();
            UIStyleManager.ApplyMainPanelStyle(contentPanel);
            this.Controls.Add(contentPanel);
        }

        private void CreateEnhancedSummaryPanel()
        {
            summaryPanel = new Panel
            {
                Height = 180, // Increased height for better content
                Dock = DockStyle.Bottom,
                BackColor = UIStyleManager.SurfaceVariant,
                Padding = new Padding(20)
            };

            // Add subtle shadow at top
            summaryPanel.Paint += (sender, e) =>
            {
                using (var brush = new LinearGradientBrush(
                    new Point(0, 0), new Point(0, 5),
                    Color.FromArgb(20, 0, 0, 0), Color.Transparent))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, summaryPanel.Width, 5);
                }
            };

            var summaryTitle = new Label
            {
                Text = "Quick Summary",
                Dock = DockStyle.Top,
                Height = 35
            };
            UIStyleManager.ApplyHeadingLabelStyle(summaryTitle);
            summaryTitle.Font = new Font(UIStyleManager.PrimaryFontFamily, 14, FontStyle.Bold);
            summaryPanel.Controls.Add(summaryTitle);

            // Create quick stats panel
            CreateQuickStatsPanel();

            this.Controls.Add(summaryPanel);
        }

        private void CreateQuickStatsPanel()
        {
            quickStatsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 0)
            };

            var statsContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(0),
                AutoScroll = true
            };

            // Create enhanced stat cards
            var stats = GetQuickStatistics();
            foreach (var stat in stats)
            {
                var card = CreateEnhancedStatCard(stat.Key, stat.Value, GetStatIcon(stat.Key));
                statsContainer.Controls.Add(card);
            }

            quickStatsPanel.Controls.Add(statsContainer);
            summaryPanel.Controls.Add(quickStatsPanel);
        }

        private Panel CreateEnhancedStatCard(string title, string value, string icon)
        {
            var card = new Panel
            {
                Size = new Size(220, 120),
                BackColor = UIStyleManager.Surface,
                Margin = new Padding(0, 0, 16, 0),
                Padding = new Padding(16)
            };

            // Add subtle border and shadow effect
            card.Paint += (sender, e) =>
            {
                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                using (var pen = new Pen(UIStyleManager.NeutralLight, 1))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
                
                // Add subtle shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 1, 1, card.Width - 1, card.Height - 1);
                }
            };

            // Icon and title container
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.Transparent
            };

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font(UIStyleManager.PrimaryFontFamily, 14),
                Size = new Size(30, 30),
                Location = new Point(0, 5),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var titleLabel = new Label
            {
                Text = title,
                Location = new Point(35, 8),
                Size = new Size(card.Width - 51, 24),
                ForeColor = UIStyleManager.NeutralMedium
            };
            UIStyleManager.ApplyBodyLabelStyle(titleLabel);
            titleLabel.Font = new Font(UIStyleManager.PrimaryFontFamily, 9, FontStyle.Bold);

            headerPanel.Controls.Add(iconLabel);
            headerPanel.Controls.Add(titleLabel);
            card.Controls.Add(headerPanel);

            // Value label
            var valueLabel = new Label
            {
                Text = value,
                Dock = DockStyle.Fill,
                ForeColor = UIStyleManager.PrimaryBlue,
                TextAlign = ContentAlignment.MiddleCenter
            };
            UIStyleManager.ApplyHeadingLabelStyle(valueLabel);
            valueLabel.Font = new Font(UIStyleManager.PrimaryFontFamily, 18, FontStyle.Bold);

            card.Controls.Add(valueLabel);

            return card;
        }

        private string GetStatIcon(string statType)
        {
            return statType.ToLower() switch
            {
                var s when s.Contains("revenue") => "💰",
                var s when s.Contains("expense") => "📊",
                var s when s.Contains("water") => "💧",
                var s when s.Contains("sanitation") => "🚿",
                var s when s.Contains("trash") => "🗑️",
                var s when s.Contains("budget") => "📈",
                _ => "📋"
            };
        }

        private void CreateEnhancedStatusBar()
        {
            statusLabel = new Label
            {
                Text = "Ready - Town of Wiley Budget Management System",
                Height = 32,
                Dock = DockStyle.Bottom,
                BackColor = UIStyleManager.PrimaryBlue,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            UIStyleManager.ApplyBodyLabelStyle(statusLabel);
            statusLabel.ForeColor = Color.White; // Override for status bar
            
            this.Controls.Add(statusLabel);
        }
        
        #endregion

        #region Enhanced Dashboard Overview
        
        private void ShowEnhancedDashboardOverview()
        {
            contentPanel.Controls.Clear();

            var overviewPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UIStyleManager.Surface,
                AutoScroll = true
            };

            // Welcome section with enhanced typography
            var welcomePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 20, 0, 20)
            };

            var welcomeLabel = new Label
            {
                Text = "Town of Wiley Budget Management System",
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = UIStyleManager.NeutralDark
            };
            UIStyleManager.ApplyHeadingLabelStyle(welcomeLabel);
            welcomeLabel.Font = new Font(UIStyleManager.PrimaryFontFamily, 22, FontStyle.Bold);

            var subtitleLabel = new Label
            {
                Text = "Comprehensive Municipal Utility Rate Management & Analysis",
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = UIStyleManager.NeutralMedium
            };
            UIStyleManager.ApplySectionLabelStyle(subtitleLabel);

            welcomePanel.Controls.Add(welcomeLabel);
            welcomePanel.Controls.Add(subtitleLabel);
            overviewPanel.Controls.Add(welcomePanel);

            // Quick actions panel
            var quickActionsPanel = CreateQuickActionsPanel();
            quickActionsPanel.Dock = DockStyle.Top;
            quickActionsPanel.Height = 200;
            overviewPanel.Controls.Add(quickActionsPanel);

            // Instructions panel with enhanced layout
            var instructionsPanel = CreateEnhancedInstructionsPanel();
            instructionsPanel.Dock = DockStyle.Fill;
            overviewPanel.Controls.Add(instructionsPanel);

            contentPanel.Controls.Add(overviewPanel);
            UpdateStatusLabel("Dashboard Overview loaded - Welcome to Wiley Budget Management", UIStyleManager.StatusType.Success);
        }

        private Panel CreateQuickActionsPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.Transparent,
                Padding = new Padding(0, 20, 0, 20)
            };

            var titleLabel = new Label
            {
                Text = "Quick Actions",
                Dock = DockStyle.Top,
                Height = 30
            };
            UIStyleManager.ApplySectionLabelStyle(titleLabel);
            panel.Controls.Add(titleLabel);

            var actionsContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(0, 10, 0, 0)
            };

            // Quick action buttons
            var quickActions = new[]
            {
                new { Text = "💧 Water Management", Action = new Action(() => ShowWaterInput()) },
                new { Text = "🚿 Sanitation Data", Action = new Action(() => ShowSanitationDistrictInput()) },
                new { Text = "🗑️ Trash Services", Action = new Action(() => ShowTrashInput()) },
                new { Text = "📊 Generate Reports", Action = new Action(() => ShowReports()) },
                new { Text = "🤖 AI Assistant", Action = new Action(() => ShowAIQueryPanel()) }
            };

            foreach (var action in quickActions)
            {
                var button = new Button
                {
                    Text = action.Text,
                    Size = new Size(160, 50),
                    Margin = new Padding(0, 0, 12, 8)
                };
                UIStyleManager.ApplyPrimaryButtonStyle(button);
                button.Click += (s, e) => action.Action();
                actionsContainer.Controls.Add(button);
            }

            panel.Controls.Add(actionsContainer);
            return panel;
        }

        private Panel CreateEnhancedInstructionsPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.Transparent,
                Padding = new Padding(0, 20, 0, 0)
            };

            var instructionsTitle = new Label
            {
                Text = "Getting Started",
                Dock = DockStyle.Top,
                Height = 35
            };
            UIStyleManager.ApplySectionLabelStyle(instructionsTitle);
            panel.Controls.Add(instructionsTitle);

            var instructionsText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = UIStyleManager.Surface,
                BorderStyle = BorderStyle.None,
                Font = UIStyleManager.BodyFont,
                Text = @"Welcome to the Town of Wiley Budget Management System!

This comprehensive application helps you manage and analyze utility rate studies for all municipal services:

🏢 ENTERPRISE MANAGEMENT
• Water District - Manage water utility revenues, expenses, and rate calculations
• Sanitation District - Track sanitation service finances and operational costs  
• Trash & Recycling - Monitor waste management operations and collection services
• Apartments Input - Handle multi-unit residential billing and rate structures

🔧 KEY FEATURES
✓ Real-time budget calculations and scenario modeling
✓ Comprehensive validation and error checking with detailed reporting
✓ Rate study methodology compliance (GASB standards)
✓ Professional export capabilities for council presentations
✓ Seasonal adjustment calculations and trend analysis
✓ AI-powered budget analysis and natural language queries

🚀 GETTING STARTED
1. Select any enterprise from the navigation menu to begin data entry
2. Review and update budget data using the intuitive grid interfaces
3. Use 'Save & Validate' to check data integrity and compliance
4. Generate professional reports for rate study analysis and presentations
5. Utilize the AI Assistant for complex budget questions and scenario planning

📞 SUPPORT
For technical assistance or questions about rate study methodology, contact the IT department or refer to the Resources section for comprehensive documentation and account definitions.

Built with ❤️ for the Town of Wiley"
            };

            panel.Controls.Add(instructionsText);
            return panel;
        }
        
        #endregion

        #region Enhanced Form Navigation
        
        private void ShowWaterInput()
        {
            if (waterForm == null || waterForm.IsDisposed)
            {
                waterForm = new WaterInput();
                ApplyEnhancedStylingToChildForm(waterForm);
            }
            ShowFormInMainPanel(waterForm);
            UpdateStatusLabel("Water District Input loaded - Manage water utility data", UIStyleManager.StatusType.Info);
        }

        private void ShowSanitationDistrictInput()
        {
            if (sanitationForm == null || sanitationForm.IsDisposed)
            {
                sanitationForm = new SanitationDistrictInput();
                ApplyEnhancedStylingToChildForm(sanitationForm);
            }
            ShowFormInMainPanel(sanitationForm);
            UpdateStatusLabel("Sanitation District Input loaded - Track sanitation services", UIStyleManager.StatusType.Info);
        }

        private void ShowTrashInput()
        {
            if (trashForm == null || trashForm.IsDisposed)
            {
                trashForm = new TrashInput();
                ApplyEnhancedStylingToChildForm(trashForm);
            }
            ShowFormInMainPanel(trashForm);
            UpdateStatusLabel("Trash & Recycling Input loaded - Manage waste services", UIStyleManager.StatusType.Info);
        }

        private void ShowApartmentsInput()
        {
            if (apartmentForm == null || apartmentForm.IsDisposed)
            {
                apartmentForm = new ApartmentsInput();
                ApplyEnhancedStylingToChildForm(apartmentForm);
            }
            ShowFormInMainPanel(apartmentForm);
            UpdateStatusLabel("Apartments Input loaded - Multi-unit residential management", UIStyleManager.StatusType.Info);
        }

        private void ShowAIQueryPanel()
        {
            if (aiQueryForm == null || aiQueryForm.IsDisposed)
            {
                aiQueryForm = new AIQueryPanel();
                ApplyEnhancedStylingToChildForm(aiQueryForm);
            }
            ShowFormInMainPanel(aiQueryForm);
            UpdateStatusLabel("AI Budget Assistant loaded - Ask natural language questions", UIStyleManager.StatusType.Success);
        }

        private void ShowResources()
        {
            if (resourcesForm == null || resourcesForm.IsDisposed)
            {
                resourcesForm = new ResourcesForm();
                ApplyEnhancedStylingToChildForm(resourcesForm);
            }
            ShowFormInMainPanel(resourcesForm);
            UpdateStatusLabel("Accounting Resources loaded - GASB-compliant documentation", UIStyleManager.StatusType.Info);
        }

        private void ShowSummaryPage()
        {
            if (summaryForm == null || summaryForm.IsDisposed)
            {
                summaryForm = new SummaryPage();
                ApplyEnhancedStylingToChildForm(summaryForm);
            }
            ShowFormInMainPanel(summaryForm);
            UpdateStatusLabel("Summary & Analysis loaded - Comprehensive financial overview", UIStyleManager.StatusType.Info);
        }

        private void ShowReports()
        {
            if (reportsForm == null || reportsForm.IsDisposed)
            {
                reportsForm = new ReportsForm();
                ApplyEnhancedStylingToChildForm(reportsForm);
            }
            ShowFormInMainPanel(reportsForm);
            UpdateStatusLabel("Reports loaded - Generate professional presentations", UIStyleManager.StatusType.Info);
        }

        private void ShowSettings()
        {
            UpdateStatusLabel("Settings panel - Coming in next update", UIStyleManager.StatusType.Warning);
            // Placeholder for settings form
        }

        private void ApplyEnhancedStylingToChildForm(Form childForm)
        {
            try
            {
                // Apply consistent styling to child forms
                childForm.BackColor = UIStyleManager.SurfaceDark;
                childForm.Font = UIStyleManager.BodyFont;
                
                // Style all buttons in the child form
                StyleChildFormControls(childForm.Controls);
            }
            catch (Exception ex)
            {
                UpdateStatusLabel($"Styling error: {ex.Message}", UIStyleManager.StatusType.Warning);
            }
        }

        private void StyleChildFormControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                switch (control)
                {
                    case Button button when button.Text.Contains("Save"):
                        UIStyleManager.ApplySuccessButtonStyle(button);
                        break;
                    case Button button when button.Text.Contains("Delete"):
                        UIStyleManager.ApplyWarningButtonStyle(button);
                        break;
                    case Button button:
                        UIStyleManager.ApplySecondaryButtonStyle(button);
                        break;
                    case Panel panel when panel.Dock == DockStyle.Top && panel.Height < 100:
                        UIStyleManager.ApplyToolbarPanelStyle(panel);
                        break;
                    case Label label when label.Font?.Bold == true:
                        UIStyleManager.ApplySectionLabelStyle(label);
                        break;
                    case Label label:
                        UIStyleManager.ApplyBodyLabelStyle(label);
                        break;
                    case TextBox textBox:
                        UIStyleManager.ApplyTextBoxStyle(textBox);
                        break;
                    case ComboBox comboBox:
                        UIStyleManager.ApplyComboBoxStyle(comboBox);
                        break;
                }

                // Recursively style child controls
                if (control.HasChildren)
                {
                    StyleChildFormControls(control.Controls);
                }
            }
        }
        
        #endregion

        #region Utility Methods
        
        private void ShowFormInMainPanel(Form form)
        {
            try
            {
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
                UpdateStatusLabel($"Error loading form: {ex.Message}", UIStyleManager.StatusType.Error);
            }
        }

        private async void LoadSummaryDataAsync()
        {
            try
            {
                // Simulate async data loading
                UpdateStatusLabel("Loading dashboard data...", UIStyleManager.StatusType.Info);
                
                await System.Threading.Tasks.Task.Delay(500); // Simulate loading time
                
                UpdateQuickStatsPanel();
                UpdateStatusLabel("Dashboard ready - All systems operational", UIStyleManager.StatusType.Success);
            }
            catch (Exception ex)
            {
                UpdateStatusLabel($"Error loading summary: {ex.Message}", UIStyleManager.StatusType.Error);
            }
        }

        private void UpdateQuickStatsPanel()
        {
            try
            {
                if (quickStatsPanel != null)
                {
                    var statsContainer = quickStatsPanel.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();
                    if (statsContainer != null)
                    {
                        statsContainer.Controls.Clear();
                        
                        var stats = GetQuickStatistics();
                        foreach (var stat in stats)
                        {
                            var card = CreateEnhancedStatCard(stat.Key, stat.Value, GetStatIcon(stat.Key));
                            statsContainer.Controls.Add(card);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatusLabel($"Error updating stats: {ex.Message}", UIStyleManager.StatusType.Warning);
            }
        }

        private Dictionary<string, string> GetQuickStatistics()
        {
            try
            {
                // Mock data - replace with actual database queries
                return new Dictionary<string, string>
                {
                    { "Total Revenue", "$2,450,000" },
                    { "Total Expenses", "$2,180,000" },
                    { "Water District", "$890,000" },
                    { "Sanitation District", "$650,000" },
                    { "Trash Services", "$520,000" },
                    { "Net Income", "$270,000" }
                };
            }
            catch
            {
                return new Dictionary<string, string>
                {
                    { "Status", "Loading..." }
                };
            }
        }

        private void UpdateStatusLabel(string message, UIStyleManager.StatusType statusType = UIStyleManager.StatusType.Info)
        {
            if (statusLabel != null)
            {
                statusLabel.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
                
                // Update status bar color based on status type
                statusLabel.BackColor = statusType switch
                {
                    UIStyleManager.StatusType.Success => UIStyleManager.StatusSuccess,
                    UIStyleManager.StatusType.Warning => UIStyleManager.StatusWarning,
                    UIStyleManager.StatusType.Error => UIStyleManager.StatusError,
                    _ => UIStyleManager.PrimaryBlue
                };
            }
        }

        // Legacy button click handlers for compatibility
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
                    ShowEnhancedDashboardOverview();
                    break;
            }
        }
        
        #endregion

        #region Form Events and Cleanup
        
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                // Clean up resources
                _databaseManager?.Dispose();
                
                // Dispose child forms
                waterForm?.Dispose();
                sanitationForm?.Dispose();
                trashForm?.Dispose();
                apartmentForm?.Dispose();
                summaryForm?.Dispose();
                reportsForm?.Dispose();
                aiQueryForm?.Dispose();
                resourcesForm?.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cleanup error: {ex.Message}");
            }
            finally
            {
                base.OnFormClosed(e);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            // Update quick stats layout on resize
            if (WindowState == FormWindowState.Maximized || WindowState == FormWindowState.Normal)
            {
                UpdateQuickStatsPanel();
            }
        }
        
        #endregion
    }
}
