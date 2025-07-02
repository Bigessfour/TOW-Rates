using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using WileyBudgetManagement.Services;
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Database;

namespace WileyBudgetManagement.Forms
{
    /// <summary>
    /// AI Query Panel for City Council natural language budget analysis
    /// Enables "What-If" scenario processing and cross-enterprise analysis
    /// </summary>
    public partial class AIQueryPanel : Form
    {
        private readonly AIQueryService _aiService;
        private readonly ISanitationRepository _repository;

        // UI Controls
        private TextBox queryTextBox;
        private Button submitButton;
        private Button clearButton;
        private RichTextBox responseTextBox;
        private ComboBox enterpriseSelector;
        private ComboBox queryTypeSelector;
        private Label statusLabel;
        private ProgressBar progressBar;
        private Panel quickQuestionsPanel;

        // Sample queries for City Council
        private readonly string[] _sampleQueries = new[]
        {
            "What if we delay the trash truck purchase by 2 years?",
            "How would a 10% rate increase affect customer affordability?",
            "What's the impact of adding 50 new water customers?",
            "Compare scenarios for water treatment plant vs pipeline replacement",
            "What if we implement a recycling program expansion?",
            "How does the $350K truck purchase affect all enterprises?",
            "What are the financial implications of EPA compliance upgrades?",
            "Analyze the impact of seasonal usage variations on revenue"
        };

        public AIQueryPanel()
        {
            try
            {
                _aiService = new AIQueryService();
                _repository = new SanitationRepository(new DatabaseManager());

                InitializeComponent();
                SetupControls();
                LoadSampleQuestions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing AI Query Panel: {ex.Message}", "Initialization Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupControls()
        {
            this.Text = "🤖 AI Budget Analysis - City Council Assistant";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 600);

            // Title and description
            var titleLabel = new Label
            {
                Text = "City Council AI Budget Assistant",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Location = new Point(15, 10),
                Size = new Size(400, 25)
            };

            var descLabel = new Label
            {
                Text = "Ask natural language questions about municipal budget scenarios and financial analysis",
                Font = new Font("Arial", 9),
                ForeColor = Color.Gray,
                Location = new Point(15, 35),
                Size = new Size(600, 20)
            };

            // Enterprise selector
            var enterpriseLabel = new Label
            {
                Text = "Focus Enterprise:",
                Location = new Point(15, 65),
                Size = new Size(120, 20)
            };

            enterpriseSelector = new ComboBox
            {
                Location = new Point(140, 63),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            enterpriseSelector.Items.AddRange(new[] {
                "All Enterprises", "Sewer District", "Water Enterprise",
                "Trash & Recycling", "Apartments"
            });
            enterpriseSelector.SelectedIndex = 0;

            // Query type selector
            var queryTypeLabel = new Label
            {
                Text = "Analysis Type:",
                Location = new Point(300, 65),
                Size = new Size(100, 20)
            };

            queryTypeSelector = new ComboBox
            {
                Location = new Point(400, 63),
                Size = new Size(180, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            queryTypeSelector.Items.AddRange(new[] {
                "General Analysis", "What-If Scenario", "Rate Impact Study",
                "Infrastructure Planning", "Cross-Enterprise Analysis"
            });
            queryTypeSelector.SelectedIndex = 0;

            // Query input area
            var queryLabel = new Label
            {
                Text = "Ask your budget question:",
                Location = new Point(15, 100),
                Size = new Size(200, 20)
            };

            queryTextBox = new TextBox
            {
                Location = new Point(15, 125),
                Size = new Size(750, 80),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Arial", 10),
                PlaceholderText = "e.g., 'What if we delay the trash truck purchase by 2 years and invest in recycling instead?'"
            };

            // Action buttons
            submitButton = new Button
            {
                Text = "🔍 Analyze with AI",
                Location = new Point(15, 215),
                Size = new Size(150, 35),
                BackColor = Color.LightBlue,
                Font = new Font("Arial", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            submitButton.Click += SubmitButton_Click;

            clearButton = new Button
            {
                Text = "🗑️ Clear",
                Location = new Point(175, 215),
                Size = new Size(80, 35),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            clearButton.Click += ClearButton_Click;

            // Progress bar
            progressBar = new ProgressBar
            {
                Location = new Point(270, 225),
                Size = new Size(200, 15),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };

            // Status label
            statusLabel = new Label
            {
                Text = "Ready for AI analysis",
                Location = new Point(480, 225),
                Size = new Size(200, 20),
                ForeColor = Color.Green
            };

            // Quick questions panel
            SetupQuickQuestionsPanel();

            // Response area
            var responseLabel = new Label
            {
                Text = "🤖 AI Analysis Results:",
                Location = new Point(15, 300),
                Size = new Size(200, 20),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            responseTextBox = new RichTextBox
            {
                Location = new Point(15, 325),
                Size = new Size(950, 320),
                ReadOnly = true,
                Font = new Font("Consolas", 9),
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Add all controls
            this.Controls.AddRange(new Control[] {
                titleLabel, descLabel, enterpriseLabel, enterpriseSelector,
                queryTypeLabel, queryTypeSelector, queryLabel, queryTextBox,
                submitButton, clearButton, progressBar, statusLabel,
                quickQuestionsPanel, responseLabel, responseTextBox
            });

            // Setup anchoring for resizing
            responseTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            queryTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void SetupQuickQuestionsPanel()
        {
            quickQuestionsPanel = new Panel
            {
                Location = new Point(780, 63),
                Size = new Size(200, 180),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.AliceBlue
            };

            var quickLabel = new Label
            {
                Text = "Quick Questions:",
                Location = new Point(5, 5),
                Size = new Size(190, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            quickQuestionsPanel.Controls.Add(quickLabel);

            // Add sample question buttons
            for (int i = 0; i < Math.Min(6, _sampleQueries.Length); i++)
            {
                var button = new Button
                {
                    Text = $"Q{i + 1}",
                    Location = new Point(5 + (i % 3) * 60, 30 + (i / 3) * 35),
                    Size = new Size(55, 30),
                    BackColor = Color.LightYellow,
                    FlatStyle = FlatStyle.Flat,
                    Tag = _sampleQueries[i],
                    Font = new Font("Arial", 8)
                };
                button.Click += QuickQuestion_Click;
                quickQuestionsPanel.Controls.Add(button);
            }

            // Add tooltip for quick questions
            var toolTip = new ToolTip();
            foreach (Control control in quickQuestionsPanel.Controls)
            {
                if (control is Button btn && btn.Tag is string question)
                {
                    toolTip.SetToolTip(btn, question.Length > 50 ? question.Substring(0, 50) + "..." : question);
                }
            }
        }

        private void LoadSampleQuestions()
        {
            // This could be enhanced to load from configuration or database
        }

        private async void SubmitButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(queryTextBox.Text))
            {
                MessageBox.Show("Please enter a question for AI analysis.", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                queryTextBox.Focus();
                return;
            }

            await ProcessAIQuery();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            queryTextBox.Clear();
            responseTextBox.Clear();
            statusLabel.Text = "Ready for AI analysis";
            statusLabel.ForeColor = Color.Green;
        }

        private void QuickQuestion_Click(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is string question)
            {
                queryTextBox.Text = question;
                queryTextBox.Focus();
            }
        }

        private async Task ProcessAIQuery()
        {
            // UI state for processing
            submitButton.Enabled = false;
            submitButton.Text = "🔄 Analyzing...";
            progressBar.Visible = true;
            statusLabel.Text = "Processing AI query...";
            statusLabel.ForeColor = Color.Blue;
            responseTextBox.Text = "🤖 Analyzing your query with AI...\n\nThis may take a few moments...";

            try
            {
                // Get current enterprise data based on selection
                var enterpriseData = await GetEnterpriseData(enterpriseSelector.SelectedItem.ToString());

                // Process based on query type
                string response;
                var queryType = queryTypeSelector.SelectedItem.ToString();

                if (queryType == "What-If Scenario")
                {
                    var scenarioResponse = await _aiService.GenerateWhatIfScenario(queryTextBox.Text, enterpriseData);
                    response = FormatScenarioResponse(scenarioResponse);
                }
                else if (queryType == "Cross-Enterprise Analysis")
                {
                    var allEnterprises = await GetAllEnterprisesData();
                    var crossResponse = await _aiService.AnalyzeCrossEnterpriseImpact(queryTextBox.Text, allEnterprises);
                    response = FormatCrossEnterpriseResponse(crossResponse);
                }
                else
                {
                    var aiResponse = await _aiService.ProcessNaturalLanguageQuery(queryTextBox.Text, enterpriseData);
                    response = FormatGeneralResponse(aiResponse);
                }

                // Display results
                responseTextBox.Text = response;
                statusLabel.Text = "Analysis complete";
                statusLabel.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                responseTextBox.Text = $"❌ Error during AI analysis:\n\n{ex.Message}\n\n" +
                                     "Please check your internet connection and try again.";
                statusLabel.Text = "Analysis failed";
                statusLabel.ForeColor = Color.Red;
            }
            finally
            {
                // Reset UI state
                submitButton.Enabled = true;
                submitButton.Text = "🔍 Analyze with AI";
                progressBar.Visible = false;
            }
        }

        private async Task<EnterpriseContext> GetEnterpriseData(string enterprise)
        {
            try
            {
                switch (enterprise)
                {
                    case "Sewer District":
                        var sewerData = await _repository.GetSanitationDataAsync();
                        return CreateEnterpriseContext("Sewer District", sewerData.ToList());

                    case "Water Enterprise":
                        var waterData = await _repository.GetWaterDataAsync();
                        return CreateEnterpriseContext("Water Enterprise", waterData.ToList());

                    case "Trash & Recycling":
                        var trashData = await _repository.GetTrashDataAsync();
                        return CreateEnterpriseContext("Trash & Recycling", trashData.ToList());

                    case "Apartments":
                        var apartmentData = await _repository.GetApartmentDataAsync();
                        return CreateEnterpriseContext("Apartments", apartmentData.ToList());

                    default: // All Enterprises
                        var allSewer = await _repository.GetSanitationDataAsync();
                        var allWater = await _repository.GetWaterDataAsync();
                        var allTrash = await _repository.GetTrashDataAsync();
                        var allApartments = await _repository.GetApartmentDataAsync();

                        var combined = allSewer.Concat(allWater).Concat(allTrash).Concat(allApartments).ToList();
                        return CreateEnterpriseContext("All Enterprises", combined);
                }
            }
            catch (Exception)
            {
                // Return basic context on error
                return new EnterpriseContext
                {
                    Name = enterprise,
                    Scope = "Limited data available",
                    TotalBudget = 0,
                    CustomerBase = 850
                };
            }
        }

        private async Task<List<EnterpriseContext>> GetAllEnterprisesData()
        {
            var enterprises = new List<EnterpriseContext>();

            try
            {
                enterprises.Add(await GetEnterpriseData("Sewer District"));
                enterprises.Add(await GetEnterpriseData("Water Enterprise"));
                enterprises.Add(await GetEnterpriseData("Trash & Recycling"));
                enterprises.Add(await GetEnterpriseData("Apartments"));
            }
            catch (Exception)
            {
                // Add minimal context on error
                enterprises.Add(new EnterpriseContext { Name = "Municipal System", Scope = "Error loading data" });
            }

            return enterprises;
        }

        private EnterpriseContext CreateEnterpriseContext(string name, List<SanitationDistrict> data)
        {
            return new EnterpriseContext
            {
                Name = name,
                Scope = $"{data.Count} accounts",
                TotalBudget = data.Sum(d => d.CurrentFYBudget),
                TotalRevenue = data.Where(d => d.Section == "Revenue").Sum(d => d.CurrentFYBudget),
                TotalExpenses = data.Where(d => d.Section != "Revenue").Sum(d => d.CurrentFYBudget),
                CustomerBase = name.Contains("Water") ? 850 : name.Contains("Trash") ? 850 :
                              name.Contains("Apartments") ? 90 : 850,
                Accounts = data
            };
        }

        #region Response Formatting

        private string FormatGeneralResponse(AIQueryResponse response)
        {
            if (!response.Success)
                return $"❌ AI Analysis Error:\n{response.Error}";

            return $"🤖 AI Budget Analysis Results\n" +
                   $"{'=' * 50}\n\n" +
                   $"📊 Query: {response.Query}\n" +
                   $"🏢 Scope: {response.EnterpriseScope}\n" +
                   $"⏱️ Analysis Time: {response.ExecutionTimeMs}ms\n" +
                   $"📅 Generated: {response.Timestamp:yyyy-MM-dd HH:mm:ss}\n\n" +
                   $"📋 ANALYSIS:\n" +
                   $"{response.Analysis}\n\n" +
                   $"{'=' * 50}\n" +
                   $"💡 This analysis was generated using AI and should be reviewed by financial staff before implementation.";
        }

        private string FormatScenarioResponse(ScenarioAnalysisResponse response)
        {
            if (!response.Success)
                return $"❌ Scenario Analysis Error:\n{response.Error}";

            var result = $"🎯 What-If Scenario Analysis\n" +
                        $"{'=' * 50}\n\n" +
                        $"📈 Scenario: {response.ScenarioName}\n" +
                        $"⏱️ Analysis Time: {response.ExecutionTimeMs}ms\n\n";

            if (response.FinancialImpact != null)
            {
                result += $"💰 FINANCIAL IMPACT:\n" +
                         $"• Monthly Impact: ${response.FinancialImpact.MonthlyImpact:N2}\n" +
                         $"• Annual Impact: ${response.FinancialImpact.AnnualImpact:N2}\n" +
                         $"• Per Customer: ${response.FinancialImpact.CustomerImpact:N2}\n\n";
            }

            if (response.Recommendations?.Any() == true)
            {
                result += $"📋 RECOMMENDATIONS:\n";
                foreach (var rec in response.Recommendations)
                {
                    result += $"• {rec}\n";
                }
                result += "\n";
            }

            result += $"⚠️ RISK ASSESSMENT:\n{response.RiskAssessment}\n\n" +
                     $"📊 DETAILED ANALYSIS:\n{response.FullAnalysis}\n\n" +
                     $"{'=' * 50}\n" +
                     $"💡 Scenario analysis should be reviewed with financial team and City Council before implementation.";

            return result;
        }

        private string FormatCrossEnterpriseResponse(CrossEnterpriseAnalysis response)
        {
            if (!response.Success)
                return $"❌ Cross-Enterprise Analysis Error:\n{response.Error}";

            var result = $"🏛️ Municipal-Wide Impact Analysis\n" +
                        $"{'=' * 50}\n\n" +
                        $"🎯 Affected Enterprises: {string.Join(", ", response.AffectedEnterprises)}\n\n";

            if (response.EstimatedImpact != null)
            {
                result += $"💰 FINANCIAL IMPACT BY ENTERPRISE:\n" +
                         $"• Sewer District: ${response.EstimatedImpact.SewerImpact:N2}\n" +
                         $"• Water Enterprise: ${response.EstimatedImpact.WaterImpact:N2}\n" +
                         $"• Trash & Recycling: ${response.EstimatedImpact.TrashImpact:N2}\n" +
                         $"• Apartments: ${response.EstimatedImpact.ApartmentsImpact:N2}\n" +
                         $"• TOTAL MUNICIPAL: ${response.EstimatedImpact.TotalMunicipalImpact:N2}\n\n";
            }

            if (response.RecommendedActions?.Any() == true)
            {
                result += $"📋 RECOMMENDED ACTIONS:\n";
                foreach (var action in response.RecommendedActions)
                {
                    result += $"• {action}\n";
                }
                result += "\n";
            }

            result += $"📊 DETAILED ANALYSIS:\n{response.Analysis}\n\n" +
                     $"{'=' * 50}\n" +
                     $"💡 Cross-enterprise impacts require coordination between all department heads and City Council approval.";

            return result;
        }

        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _aiService?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
