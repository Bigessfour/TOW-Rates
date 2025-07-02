using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using WileyBudgetManagement.Resources;

namespace WileyBudgetManagement.Forms
{
    /// <summary>
    /// Resources Form for Town of Wiley Budget Management System
    /// Provides access to Account Library and accounting metadata
    /// </summary>
    public partial class ResourcesForm : Form
    {
        // UI Controls
        private TabControl mainTabControl = null!;
        private DataGridView accountLibraryGrid = null!;
        private ComboBox categoryFilter = null!;
        private ComboBox enterpriseFilter = null!;
        private CheckBox implementedOnlyCheckbox = null!;
        private TextBox searchTextBox = null!;
        private Button searchButton = null!;
        private Button clearFilterButton = null!;
        private Label statusLabel = null!;
        private RichTextBox accountDetailsTextBox = null!;
        private Panel summaryPanel = null!;

        // Data
        private List<AccountDefinition> currentAccounts = null!;
        private List<AccountDefinition> allAccounts = null!;

        public ResourcesForm()
        {
            InitializeComponent();
            LoadAccountLibrary();
            SetupControls();
            PopulateFilters();
            RefreshAccountDisplay();
        }

        private void SetupControls()
        {
            this.Text = "📚 Accounting Resources - Town of Wiley Budget Management";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 600);

            // Main tab control
            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            // Account Library Tab
            var accountLibraryTab = new TabPage("Account Library");
            SetupAccountLibraryTab(accountLibraryTab);
            mainTabControl.TabPages.Add(accountLibraryTab);

            // Account Summary Tab
            var summaryTab = new TabPage("Summary & Statistics");
            SetupSummaryTab(summaryTab);
            mainTabControl.TabPages.Add(summaryTab);

            // Documentation Tab
            var documentationTab = new TabPage("GASB Documentation");
            SetupDocumentationTab(documentationTab);
            mainTabControl.TabPages.Add(documentationTab);

            this.Controls.Add(mainTabControl);
        }

        private void SetupAccountLibraryTab(TabPage tab)
        {
            var mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            // Title and description
            var titleLabel = new Label
            {
                Text = "Account Library - GASB Compliant Chart of Accounts",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Location = new Point(10, 10),
                Size = new Size(600, 30)
            };

            var descLabel = new Label
            {
                Text = "Comprehensive accounting resource for all municipal enterprises. Accounts are preformatted with GASB-compliant numbering and metadata.",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Location = new Point(10, 40),
                Size = new Size(800, 40)
            };

            // Filter panel
            var filterPanel = new Panel
            {
                Location = new Point(10, 80),
                Size = new Size(1150, 60),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.AliceBlue
            };

            // Category filter
            var categoryLabel = new Label
            {
                Text = "Category:",
                Location = new Point(10, 15),
                Size = new Size(70, 20)
            };
            categoryFilter = new ComboBox
            {
                Location = new Point(80, 12),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            categoryFilter.SelectedIndexChanged += FilterChanged;

            // Enterprise filter
            var enterpriseLabel = new Label
            {
                Text = "Enterprise:",
                Location = new Point(250, 15),
                Size = new Size(70, 20)
            };
            enterpriseFilter = new ComboBox
            {
                Location = new Point(320, 12),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            enterpriseFilter.SelectedIndexChanged += FilterChanged;

            // Implemented only checkbox
            implementedOnlyCheckbox = new CheckBox
            {
                Text = "Implemented Only",
                Location = new Point(460, 15),
                Size = new Size(120, 20),
                Checked = false
            };
            implementedOnlyCheckbox.CheckedChanged += FilterChanged;

            // Search
            var searchLabel = new Label
            {
                Text = "Search:",
                Location = new Point(600, 15),
                Size = new Size(50, 20)
            };
            searchTextBox = new TextBox
            {
                Location = new Point(650, 12),
                Size = new Size(200, 25),
                PlaceholderText = "Account name, number, or description..."
            };
            searchTextBox.TextChanged += FilterChanged;

            searchButton = new Button
            {
                Text = "🔍",
                Location = new Point(860, 12),
                Size = new Size(30, 25),
                FlatStyle = FlatStyle.Flat
            };
            searchButton.Click += (s, e) => RefreshAccountDisplay();

            clearFilterButton = new Button
            {
                Text = "Clear Filters",
                Location = new Point(900, 12),
                Size = new Size(80, 25),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            clearFilterButton.Click += ClearFilters;

            filterPanel.Controls.AddRange(new Control[] {
                categoryLabel, categoryFilter, enterpriseLabel, enterpriseFilter,
                implementedOnlyCheckbox, searchLabel, searchTextBox, searchButton, clearFilterButton
            });

            // Account grid
            accountLibraryGrid = new DataGridView
            {
                Location = new Point(10, 150),
                Size = new Size(800, 400),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                GridColor = Color.LightGray
            };
            accountLibraryGrid.SelectionChanged += AccountGrid_SelectionChanged;

            // Account details panel
            var detailsLabel = new Label
            {
                Text = "Account Details:",
                Location = new Point(820, 150),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            accountDetailsTextBox = new RichTextBox
            {
                Location = new Point(820, 175),
                Size = new Size(320, 375),
                ReadOnly = true,
                BackColor = Color.WhiteSmoke,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Status label
            statusLabel = new Label
            {
                Text = "Ready - Account Library loaded",
                Location = new Point(10, 560),
                Size = new Size(400, 20),
                ForeColor = Color.Green
            };

            mainPanel.Controls.AddRange(new Control[] {
                titleLabel, descLabel, filterPanel, accountLibraryGrid, 
                detailsLabel, accountDetailsTextBox, statusLabel
            });

            tab.Controls.Add(mainPanel);
        }

        private void SetupSummaryTab(TabPage tab)
        {
            summaryPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.White
            };

            var titleLabel = new Label
            {
                Text = "Account Library Statistics & Summary",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var summaryTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.None
            };

            // Generate summary statistics
            var summary = GenerateAccountSummary();
            summaryTextBox.Text = summary;

            summaryPanel.Controls.Add(titleLabel);
            summaryPanel.Controls.Add(summaryTextBox);
            tab.Controls.Add(summaryPanel);
        }

        private void SetupDocumentationTab(TabPage tab)
        {
            var docPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.White
            };

            var titleLabel = new Label
            {
                Text = "GASB Compliance Documentation",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var docTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.None,
                Text = GetGASBDocumentation()
            };

            docPanel.Controls.Add(titleLabel);
            docPanel.Controls.Add(docTextBox);
            tab.Controls.Add(docPanel);
        }

        private void LoadAccountLibrary()
        {
            allAccounts = AccountLibrary.GetAllAccounts();
            currentAccounts = new List<AccountDefinition>(allAccounts);
        }

        private void PopulateFilters()
        {
            // Category filter
            categoryFilter.Items.Add("All Categories");
            foreach (AccountCategory category in Enum.GetValues<AccountCategory>())
            {
                categoryFilter.Items.Add(category.ToString());
            }
            categoryFilter.SelectedIndex = 0;

            // Enterprise filter
            enterpriseFilter.Items.Add("All Enterprises");
            enterpriseFilter.Items.Add("Sanitation");
            enterpriseFilter.Items.Add("Water");
            enterpriseFilter.Items.Add("Trash");
            enterpriseFilter.Items.Add("Apartments");
            enterpriseFilter.SelectedIndex = 0;
        }

        private void RefreshAccountDisplay()
        {
            try
            {
                // Apply filters
                var filteredAccounts = allAccounts.AsEnumerable();

                // Category filter
                if (categoryFilter.SelectedIndex > 0)
                {
                    var selectedCategory = (AccountCategory)Enum.Parse(typeof(AccountCategory), categoryFilter.SelectedItem?.ToString() ?? string.Empty);
                    filteredAccounts = filteredAccounts.Where(a => a.Category == selectedCategory);
                }

                // Enterprise filter
                if (enterpriseFilter.SelectedIndex > 0)
                {
                    var selectedEnterprise = (EnterpriseType)Enum.Parse(typeof(EnterpriseType), enterpriseFilter.SelectedItem?.ToString() ?? string.Empty);
                    filteredAccounts = filteredAccounts.Where(a => a.ApplicableEnterprises.HasFlag(selectedEnterprise) || 
                                                                  a.ApplicableEnterprises.HasFlag(EnterpriseType.All));
                }

                // Implemented only filter
                if (implementedOnlyCheckbox.Checked)
                {
                    filteredAccounts = filteredAccounts.Where(a => a.IsImplemented);
                }

                // Search filter
                if (!string.IsNullOrWhiteSpace(searchTextBox.Text))
                {
                    var searchTerm = searchTextBox.Text.ToLower();
                    filteredAccounts = filteredAccounts.Where(a =>
                        a.AccountNumber.ToLower().Contains(searchTerm) ||
                        a.AccountName.ToLower().Contains(searchTerm) ||
                        a.Description.ToLower().Contains(searchTerm));
                }

                currentAccounts = filteredAccounts.OrderBy(a => a.AccountNumber).ToList();

                // Update grid
                UpdateAccountGrid();

                // Update status
                statusLabel.Text = $"Displaying {currentAccounts.Count} of {allAccounts.Count} accounts";
                statusLabel.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error filtering accounts: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
            }
        }

        private void UpdateAccountGrid()
        {
            accountLibraryGrid.DataSource = null;
            accountLibraryGrid.Columns.Clear();

            if (!currentAccounts.Any()) return;

            // Create display data
            var displayData = currentAccounts.Select(a => new
            {
                AccountNumber = a.AccountNumber,
                AccountName = a.AccountName,
                Category = a.Category.ToString(),
                Implemented = a.IsImplemented ? "✓" : "○",
                Enterprises = GetEnterpriseDisplayText(a.ApplicableEnterprises),
                Description = a.Description.Length > 50 ? a.Description.Substring(0, 50) + "..." : a.Description
            }).ToList();

            accountLibraryGrid.DataSource = displayData;

            // Customize columns
            accountLibraryGrid.Columns["AccountNumber"].Width = 100;
            accountLibraryGrid.Columns["AccountName"].Width = 200;
            accountLibraryGrid.Columns["Category"].Width = 120;
            accountLibraryGrid.Columns["Implemented"].Width = 80;
            accountLibraryGrid.Columns["Enterprises"].Width = 100;
            accountLibraryGrid.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Style implemented column
            foreach (DataGridViewRow row in accountLibraryGrid.Rows)
            {
                if (row.Cells["Implemented"].Value?.ToString() == "✓")
                {
                    row.Cells["Implemented"].Style.ForeColor = Color.Green;
                    row.Cells["Implemented"].Style.Font = new Font(accountLibraryGrid.Font, FontStyle.Bold);
                }
            }
        }

        private string GetEnterpriseDisplayText(EnterpriseType enterprises)
        {
            if (enterprises.HasFlag(EnterpriseType.All)) return "All";

            var types = new List<string>();
            if (enterprises.HasFlag(EnterpriseType.Sanitation)) types.Add("S");
            if (enterprises.HasFlag(EnterpriseType.Water)) types.Add("W");
            if (enterprises.HasFlag(EnterpriseType.Trash)) types.Add("T");
            if (enterprises.HasFlag(EnterpriseType.Apartments)) types.Add("A");

            return string.Join(", ", types);
        }

        private void AccountGrid_SelectionChanged(object? sender, EventArgs e)
        {
            if (accountLibraryGrid.SelectedRows.Count > 0)
            {
                var selectedRow = accountLibraryGrid.SelectedRows[0];
                var accountNumber = selectedRow.Cells["AccountNumber"].Value?.ToString();
                
                var account = currentAccounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
                if (account != null)
                {
                    DisplayAccountDetails(account);
                }
            }
        }

        private void DisplayAccountDetails(AccountDefinition account)
        {
            var details = $@"📋 ACCOUNT DETAILS

Account Number: {account.AccountNumber}
Account Name: {account.AccountName}
Category: {account.Category}

📝 DESCRIPTION:
{account.Description}

🏢 APPLICABLE ENTERPRISES:
{GetEnterpriseFullText(account.ApplicableEnterprises)}

✅ IMPLEMENTATION STATUS:
{(account.IsImplemented ? "✓ Currently Implemented" : "○ Available for Implementation")}

📊 METADATA:
• Created: {account.DateCreated:yyyy-MM-dd}
• Created By: {account.CreatedBy}
• Active: {(account.IsActive ? "Yes" : "No")}
• Default Budget: {account.DefaultBudgetAmount:C}
• Requires Approval: {(account.RequiresApproval ? "Yes" : "No")}

💭 NOTES:
{(string.IsNullOrEmpty(account.Notes) ? "No additional notes" : account.Notes)}

🔧 GASB COMPLIANCE:
This account follows GASB (Governmental Accounting Standards Board) 
numbering conventions:
• 300-399: Revenue Accounts
• 400-459: Operating Expense Accounts  
• 460-499: Administrative & General Expense Accounts

📈 USAGE RECOMMENDATIONS:
{GetUsageRecommendations(account)}";

            accountDetailsTextBox.Text = details;
        }

        private string GetEnterpriseFullText(EnterpriseType enterprises)
        {
            if (enterprises.HasFlag(EnterpriseType.All)) return "All Municipal Enterprises";

            var types = new List<string>();
            if (enterprises.HasFlag(EnterpriseType.Sanitation)) types.Add("Sanitation District");
            if (enterprises.HasFlag(EnterpriseType.Water)) types.Add("Water Enterprise");
            if (enterprises.HasFlag(EnterpriseType.Trash)) types.Add("Trash & Recycling");
            if (enterprises.HasFlag(EnterpriseType.Apartments)) types.Add("Apartments");

            return string.Join(", ", types);
        }

        private string GetUsageRecommendations(AccountDefinition account)
        {
            return account.Category switch
            {
                AccountCategory.Revenue => "Use for tracking income sources. Monitor monthly for budget variance analysis.",
                AccountCategory.OperatingExpense => "Track operational costs. Include in rate calculations and efficiency analysis.",
                AccountCategory.AdministrativeExpense => "Allocate across enterprises using % allocation. Monitor for cost control.",
                _ => "Follow GASB guidelines for proper classification and reporting."
            };
        }

        private void FilterChanged(object? sender, EventArgs e)
        {
            RefreshAccountDisplay();
        }

        private void ClearFilters(object? sender, EventArgs e)
        {
            categoryFilter.SelectedIndex = 0;
            enterpriseFilter.SelectedIndex = 0;
            implementedOnlyCheckbox.Checked = false;
            searchTextBox.Clear();
            RefreshAccountDisplay();
        }

        private string GenerateAccountSummary()
        {
            var totalAccounts = allAccounts.Count;
            var implementedAccounts = allAccounts.Count(a => a.IsImplemented);
            var availableAccounts = totalAccounts - implementedAccounts;

            var revenueAccounts = allAccounts.Count(a => a.Category == AccountCategory.Revenue);
            var operatingAccounts = allAccounts.Count(a => a.Category == AccountCategory.OperatingExpense);
            var adminAccounts = allAccounts.Count(a => a.Category == AccountCategory.AdministrativeExpense);

            var sanitationAccounts = allAccounts.Count(a => a.ApplicableEnterprises.HasFlag(EnterpriseType.Sanitation));
            var waterAccounts = allAccounts.Count(a => a.ApplicableEnterprises.HasFlag(EnterpriseType.Water));
            var trashAccounts = allAccounts.Count(a => a.ApplicableEnterprises.HasFlag(EnterpriseType.Trash));
            var apartmentAccounts = allAccounts.Count(a => a.ApplicableEnterprises.HasFlag(EnterpriseType.Apartments));

            return $@"📊 ACCOUNT LIBRARY STATISTICS

🏛️ TOWN OF WILEY BUDGET MANAGEMENT SYSTEM
Account Library Resource | Version 1.0 | Date: July 1, 2025

═══════════════════════════════════════════════════════════════

📈 OVERALL STATISTICS
• Total Accounts Available: {totalAccounts}
• Currently Implemented: {implementedAccounts} ({(double)implementedAccounts/totalAccounts:P1})
• Available for Implementation: {availableAccounts} ({(double)availableAccounts/totalAccounts:P1})

📋 BY CATEGORY (GASB Compliant)
• Revenue Accounts (300-399): {revenueAccounts}
• Operating Expense Accounts (400-459): {operatingAccounts}  
• Administrative Expense Accounts (460-499): {adminAccounts}

🏢 BY ENTERPRISE APPLICABILITY
• Sanitation District: {sanitationAccounts} accounts
• Water Enterprise: {waterAccounts} accounts
• Trash & Recycling: {trashAccounts} accounts
• Apartments: {apartmentAccounts} accounts

═══════════════════════════════════════════════════════════════

🎯 IMPLEMENTATION STATUS

✅ COMPLETED ENTERPRISES (100% Implemented)
• Sewer/Sanitation District: Fully operational with {allAccounts.Count(a => a.IsImplemented && a.ApplicableEnterprises.HasFlag(EnterpriseType.Sanitation))} active accounts
• Water Enterprise: Production ready with $457,500 budget management

🔄 IN PROGRESS ENTERPRISES  
• Trash Enterprise: 85% Complete - Core functionality implemented
• Apartments Enterprise: 40% Complete - Basic framework exists

═══════════════════════════════════════════════════════════════

💡 ACCOUNT LIBRARY BENEFITS

🔧 STANDARDIZATION
• GASB-compliant numbering system ensures regulatory compliance
• Consistent account structure across all municipal enterprises  
• Predefined metadata reduces implementation time

📊 FLEXIBILITY
• Accounts can be easily added to any enterprise as needed
• Comprehensive search and filtering capabilities
• Future-ready account structure supports growth

✅ AUDIT READINESS
• Built-in compliance with governmental accounting standards
• Complete audit trail and documentation
• Professional account descriptions and metadata

═══════════════════════════════════════════════════════════════

📋 USAGE GUIDELINES

🚀 ADDING NEW ACCOUNTS
1. Identify Need: Determine account purpose and enterprise
2. Select from Library: Choose appropriate predefined account
3. Integrate: Add to enterprise table with proper validation
4. Update Calculations: Include in totals and scenarios
5. Validate: Test data integrity and calculations

📈 RATE STUDY COMPLIANCE
• Revenue accounts (300s): Track all income sources
• Operating expenses (400s): Monitor operational costs  
• Administrative expenses (460s): Allocate across enterprises

🔍 MONTHLY OPERATIONS
• Track actuals against budgets for variance analysis
• Support ""What-If"" scenarios for rate adjustments
• Generate audit-ready reports and documentation

═══════════════════════════════════════════════════════════════

⚡ AI INTEGRATION READY
The Account Library is fully compatible with the new AI Budget Assistant:
• Natural language queries about account usage
• Automated account recommendations for new services
• Cross-enterprise account impact analysis
• Predictive account needs based on growth patterns

═══════════════════════════════════════════════════════════════

Document Version: 1.0
Last Updated: {DateTime.Now:yyyy-MM-dd HH:mm}
Status: Production Ready";
        }

        private string GetGASBDocumentation()
        {
            return @"📚 GASB COMPLIANCE DOCUMENTATION

🏛️ GOVERNMENTAL ACCOUNTING STANDARDS BOARD (GASB) COMPLIANCE
Town of Wiley Budget Management System

═══════════════════════════════════════════════════════════════

📋 ACCOUNT NUMBERING SYSTEM

The Town of Wiley Budget Management System follows GASB Statement No. 34 
and related standards for governmental accounting and financial reporting.

🔢 NUMBERING CONVENTIONS

300-399 SERIES: REVENUE ACCOUNTS
• 301-310: Primary Service Revenue
• 311-319: Tax Revenue  
• 320-329: Other Revenue
• 330-339: Environmental and Recycling Revenue

400-459 SERIES: OPERATING EXPENSE ACCOUNTS
• 401-409: Permits and Regulatory
• 410-419: Office and Administrative Operations
• 420-429: Collection and Service Operations
• 430-439: Insurance and Risk Management
• 440-449: Financial and Administrative Services
• 450-459: Infrastructure and Maintenance

460-499 SERIES: ADMINISTRATIVE & GENERAL EXPENSE ACCOUNTS
• 460-469: Personnel Costs
• 470-479: Professional Services
• 480-489: Facilities and Operations
• 490-499: Operational Support

═══════════════════════════════════════════════════════════════

📊 ENTERPRISE FUND ACCOUNTING

Per GASB Statement No. 34, the system uses Enterprise Fund accounting for:

🚿 SANITATION DISTRICT
• Self-supporting sewage collection and treatment services
• Revenue from service charges and property taxes
• Operating and maintenance expenses tracked separately

💧 WATER ENTERPRISE  
• Municipal water utility operations
• Revenue from water sales and connection fees
• Infrastructure and treatment costs

🗑️ TRASH & RECYCLING ENTERPRISE
• Waste collection and recycling services
• Revenue from collection fees and recycling sales
• Equipment and operational costs

🏠 APARTMENTS ENTERPRISE
• Multi-family residential utility services
• Revenue from apartment service fees
• Specialized tracking for occupancy and collections

═══════════════════════════════════════════════════════════════

✅ COMPLIANCE FEATURES

🔍 AUDIT TRAIL
• Complete transaction history for all accounts
• User activity logging and approval workflows
• Automated backup and recovery procedures

📈 FINANCIAL REPORTING
• Monthly variance reports comparing actual to budget
• Annual comprehensive financial statements
• Cross-enterprise consolidation and analysis

⚖️ REGULATORY COMPLIANCE
• EPA compliance tracking for environmental regulations
• State utility commission reporting requirements
• Municipal bond covenant compliance monitoring

📊 RATE SETTING COMPLIANCE
• Cost-of-service analysis for rate justification
• Customer affordability index calculations
• Cross-subsidization analysis between enterprises

═══════════════════════════════════════════════════════════════

🎯 IMPLEMENTATION STANDARDS

💰 BUDGET MANAGEMENT
• Annual budget preparation with multi-year projections
• Monthly budget-to-actual variance analysis
• Capital improvement program integration

📋 ACCOUNT MANAGEMENT
• Standardized account coding across all enterprises
• Automated account validation and error checking
• Integration with municipal chart of accounts

🔄 SCENARIO ANALYSIS
• ""What-If"" modeling for rate and budget changes
• Infrastructure investment impact analysis
• Customer affordability and rate design studies

═══════════════════════════════════════════════════════════════

📖 REFERENCE STANDARDS

The system complies with the following standards:

• GASB Statement No. 34: Basic Financial Statements
• GASB Statement No. 37: Basic Financial Statements for State and Local Governments: Omnibus
• GASB Statement No. 62: Codification of Accounting and Financial Reporting Guidance
• EPA Guidelines for Municipal Utility Rate Setting
• Colorado Municipal Utility Regulations

═══════════════════════════════════════════════════════════════

For additional compliance information, contact:
• Municipal Auditor: Annual compliance review
• Rate Study Consultant: Rate methodology validation  
• Legal Counsel: Regulatory compliance verification
• System Administrator: Technical implementation support

Document prepared in accordance with GASB standards
Last updated: July 1, 2025";
        }
    }
}
