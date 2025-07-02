using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Services;

namespace WileyBudgetManagement.Forms
{
    /// <summary>
    /// Demonstration form for AI-enhanced financial calculations
    /// Shows the integration of AI insights with basic mathematical fallbacks
    /// </summary>
    public partial class AIEnhancedDemo : Form
    {
        private WileyAIFinanceManager _financeManager;
        private EnterpriseContext _sampleEnterprise;

        // UI Controls
        private TabControl tabControl;
        private TabPage rateOptimizationTab;
        private TabPage affordabilityTab;
        private TabPage anomalyTab;
        private TabPage forecastTab;
        private TabPage complianceTab;
        private TabPage queryTab;

        // Rate Optimization Controls
        private NumericUpDown targetRevenueInput;
        private NumericUpDown maxIncreaseInput;
        private Button optimizeRatesButton;
        private RichTextBox rateResultsText;

        // Affordability Controls
        private NumericUpDown proposedRateInput;
        private Button analyzeAffordabilityButton;
        private RichTextBox affordabilityResultsText;

        // Anomaly Detection Controls
        private Button detectAnomaliesButton;
        private RichTextBox anomalyResultsText;

        // Revenue Forecast Controls
        private NumericUpDown forecastMonthsInput;
        private Button forecastRevenueButton;
        private RichTextBox forecastResultsText;

        // Compliance Controls
        private Button checkComplianceButton;
        private RichTextBox complianceResultsText;

        // AI Query Controls
        private TextBox questionInput;
        private Button askQuestionButton;
        private RichTextBox queryResultsText;

        // Status and Progress
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private ToolStripProgressBar progressBar;

        public AIEnhancedDemo()
        {
            InitializeComponent();
            InitializeServices();
            SetupSampleData();
        }

        private void InitializeComponent()
        {
            this.Text = "Wiley AI-Enhanced Financial Analysis Demo";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create tab control
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };

            // Create tabs
            CreateRateOptimizationTab();
            CreateAffordabilityTab();
            CreateAnomalyDetectionTab();
            CreateForecastTab();
            CreateComplianceTab();
            CreateQueryTab();

            // Add tabs to control
            tabControl.TabPages.Add(rateOptimizationTab);
            tabControl.TabPages.Add(affordabilityTab);
            tabControl.TabPages.Add(anomalyTab);
            tabControl.TabPages.Add(forecastTab);
            tabControl.TabPages.Add(complianceTab);
            tabControl.TabPages.Add(queryTab);

            // Create status strip
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Ready");
            progressBar = new ToolStripProgressBar { Visible = false };
            statusStrip.Items.Add(statusLabel);
            statusStrip.Items.Add(progressBar);

            // Add controls to form
            this.Controls.Add(tabControl);
            this.Controls.Add(statusStrip);
        }

        private void CreateRateOptimizationTab()
        {
            rateOptimizationTab = new TabPage("Rate Optimization");
            
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            
            // Input controls
            var inputPanel = new Panel { Height = 120, Dock = DockStyle.Top };
            
            var targetRevenueLabel = new Label 
            { 
                Text = "Target Annual Revenue:",
                Location = new Point(10, 15),
                Size = new Size(150, 23)
            };
            
            targetRevenueInput = new NumericUpDown
            {
                Location = new Point(170, 12),
                Size = new Size(120, 23),
                Maximum = 10000000,
                Value = 1500000,
                DecimalPlaces = 0,
                ThousandsSeparator = true
            };

            var maxIncreaseLabel = new Label 
            { 
                Text = "Max Rate Increase %:",
                Location = new Point(10, 45),
                Size = new Size(150, 23)
            };
            
            maxIncreaseInput = new NumericUpDown
            {
                Location = new Point(170, 42),
                Size = new Size(120, 23),
                Maximum = 100,
                Value = 15,
                DecimalPlaces = 1
            };

            optimizeRatesButton = new Button
            {
                Text = "🧠 Optimize Rates (AI + Math)",
                Location = new Point(10, 75),
                Size = new Size(200, 35),
                BackColor = Color.LightBlue
            };
            optimizeRatesButton.Click += OptimizeRatesButton_Click;

            inputPanel.Controls.Add(targetRevenueLabel);
            inputPanel.Controls.Add(targetRevenueInput);
            inputPanel.Controls.Add(maxIncreaseLabel);
            inputPanel.Controls.Add(maxIncreaseInput);
            inputPanel.Controls.Add(optimizeRatesButton);

            // Results display
            rateResultsText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9),
                ReadOnly = true
            };

            panel.Controls.Add(inputPanel);
            panel.Controls.Add(rateResultsText);
            rateOptimizationTab.Controls.Add(panel);
        }

        private void CreateAffordabilityTab()
        {
            affordabilityTab = new TabPage("Affordability Analysis");
            
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            
            var inputPanel = new Panel { Height = 90, Dock = DockStyle.Top };
            
            var proposedRateLabel = new Label 
            { 
                Text = "Proposed Monthly Rate:",
                Location = new Point(10, 15),
                Size = new Size(150, 23)
            };
            
            proposedRateInput = new NumericUpDown
            {
                Location = new Point(170, 12),
                Size = new Size(120, 23),
                Maximum = 1000,
                Value = 75,
                DecimalPlaces = 2
            };

            analyzeAffordabilityButton = new Button
            {
                Text = "🏠 Analyze Customer Impact",
                Location = new Point(10, 45),
                Size = new Size(200, 35),
                BackColor = Color.LightGreen
            };
            analyzeAffordabilityButton.Click += AnalyzeAffordabilityButton_Click;

            inputPanel.Controls.Add(proposedRateLabel);
            inputPanel.Controls.Add(proposedRateInput);
            inputPanel.Controls.Add(analyzeAffordabilityButton);

            affordabilityResultsText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9),
                ReadOnly = true
            };

            panel.Controls.Add(inputPanel);
            panel.Controls.Add(affordabilityResultsText);
            affordabilityTab.Controls.Add(panel);
        }

        private void CreateAnomalyDetectionTab()
        {
            anomalyTab = new TabPage("Anomaly Detection");
            
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            
            var inputPanel = new Panel { Height = 60, Dock = DockStyle.Top };

            detectAnomaliesButton = new Button
            {
                Text = "🔍 Detect Financial Anomalies",
                Location = new Point(10, 15),
                Size = new Size(200, 35),
                BackColor = Color.LightCoral
            };
            detectAnomaliesButton.Click += DetectAnomaliesButton_Click;

            inputPanel.Controls.Add(detectAnomaliesButton);

            anomalyResultsText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9),
                ReadOnly = true
            };

            panel.Controls.Add(inputPanel);
            panel.Controls.Add(anomalyResultsText);
            anomalyTab.Controls.Add(panel);
        }

        private void CreateForecastTab()
        {
            forecastTab = new TabPage("Revenue Forecast");
            
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            
            var inputPanel = new Panel { Height = 90, Dock = DockStyle.Top };
            
            var forecastMonthsLabel = new Label 
            { 
                Text = "Forecast Months:",
                Location = new Point(10, 15),
                Size = new Size(150, 23)
            };
            
            forecastMonthsInput = new NumericUpDown
            {
                Location = new Point(170, 12),
                Size = new Size(120, 23),
                Maximum = 60,
                Minimum = 1,
                Value = 12
            };

            forecastRevenueButton = new Button
            {
                Text = "📈 Generate Revenue Forecast",
                Location = new Point(10, 45),
                Size = new Size(200, 35),
                BackColor = Color.LightYellow
            };
            forecastRevenueButton.Click += ForecastRevenueButton_Click;

            inputPanel.Controls.Add(forecastMonthsLabel);
            inputPanel.Controls.Add(forecastMonthsInput);
            inputPanel.Controls.Add(forecastRevenueButton);

            forecastResultsText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9),
                ReadOnly = true
            };

            panel.Controls.Add(inputPanel);
            panel.Controls.Add(forecastResultsText);
            forecastTab.Controls.Add(panel);
        }

        private void CreateComplianceTab()
        {
            complianceTab = new TabPage("Compliance Check");
            
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            
            var inputPanel = new Panel { Height = 60, Dock = DockStyle.Top };

            checkComplianceButton = new Button
            {
                Text = "⚖️ Check Regulatory Compliance",
                Location = new Point(10, 15),
                Size = new Size(220, 35),
                BackColor = Color.LightSalmon
            };
            checkComplianceButton.Click += CheckComplianceButton_Click;

            inputPanel.Controls.Add(checkComplianceButton);

            complianceResultsText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9),
                ReadOnly = true
            };

            panel.Controls.Add(inputPanel);
            panel.Controls.Add(complianceResultsText);
            complianceTab.Controls.Add(panel);
        }

        private void CreateQueryTab()
        {
            queryTab = new TabPage("AI Query");
            
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            
            var inputPanel = new Panel { Height = 90, Dock = DockStyle.Top };
            
            var questionLabel = new Label 
            { 
                Text = "Ask AI about your finances:",
                Location = new Point(10, 15),
                Size = new Size(200, 23)
            };
            
            questionInput = new TextBox
            {
                Location = new Point(10, 35),
                Size = new Size(400, 23),
                Text = "What should I consider when setting utility rates?"
            };

            askQuestionButton = new Button
            {
                Text = "🤖 Ask AI Assistant",
                Location = new Point(420, 32),
                Size = new Size(150, 30),
                BackColor = Color.LightCyan
            };
            askQuestionButton.Click += AskQuestionButton_Click;

            inputPanel.Controls.Add(questionLabel);
            inputPanel.Controls.Add(questionInput);
            inputPanel.Controls.Add(askQuestionButton);

            queryResultsText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9),
                ReadOnly = true
            };

            panel.Controls.Add(inputPanel);
            panel.Controls.Add(queryResultsText);
            queryTab.Controls.Add(panel);
        }

        private void InitializeServices()
        {
            try
            {
                _financeManager = new WileyAIFinanceManager();
                statusLabel.Text = "AI services initialized successfully";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"AI unavailable - using basic calculations: {ex.Message}";
                MessageBox.Show($"AI services could not be initialized. The application will use basic mathematical calculations instead.\n\nError: {ex.Message}", 
                    "AI Service Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SetupSampleData()
        {
            _sampleEnterprise = new EnterpriseContext
            {
                Name = "Water Enterprise",
                Scope = "Municipal water utility for Town of Wiley",
                CustomerBase = 450,
                TotalBudget = 1200000m,
                TotalRevenue = 1100000m,
                TotalExpenses = 1050000m,
                YearToDateSpending = 750000m,
                PercentOfBudgetUsed = 0.625m,
                BudgetRemaining = 450000m,
                RequiredRate = 68.50m,
                CustomerAffordabilityIndex = 0.82m,
                ReserveTarget = 120000m
            };

            _sampleEnterprise.CalculateMetrics();
        }

        #region Event Handlers

        private async void OptimizeRatesButton_Click(object sender, EventArgs e)
        {
            await ExecuteWithProgress(async () =>
            {
                var goals = new RateOptimizationGoals
                {
                    TargetRevenue = targetRevenueInput.Value,
                    MaxRateIncreasePercent = maxIncreaseInput.Value / 100m,
                    CustomerRetentionTarget = 0.95m,
                    AffordabilityConstraint = 0.04m
                };

                var result = await _financeManager.OptimizeUtilityRates(_sampleEnterprise, goals);
                DisplayRateOptimizationResult(result);
            });
        }

        private async void AnalyzeAffordabilityButton_Click(object sender, EventArgs e)
        {
            await ExecuteWithProgress(async () =>
            {
                var result = await _financeManager.AnalyzeCustomerAffordability(_sampleEnterprise, proposedRateInput.Value);
                DisplayAffordabilityResult(result);
            });
        }

        private async void DetectAnomaliesButton_Click(object sender, EventArgs e)
        {
            await ExecuteWithProgress(async () =>
            {
                var result = await _financeManager.DetectFinancialAnomalies(_sampleEnterprise);
                DisplayAnomalyResult(result);
            });
        }

        private async void ForecastRevenueButton_Click(object sender, EventArgs e)
        {
            await ExecuteWithProgress(async () =>
            {
                var result = await _financeManager.ForecastRevenue(_sampleEnterprise, (int)forecastMonthsInput.Value);
                DisplayForecastResult(result);
            });
        }

        private async void CheckComplianceButton_Click(object sender, EventArgs e)
        {
            await ExecuteWithProgress(async () =>
            {
                var result = await _financeManager.CheckRegulatoryCompliance(_sampleEnterprise);
                DisplayComplianceResult(result);
            });
        }

        private async void AskQuestionButton_Click(object sender, EventArgs e)
        {
            await ExecuteWithProgress(async () =>
            {
                var answer = await _financeManager.AskFinancialQuestion(questionInput.Text, _sampleEnterprise);
                queryResultsText.Text = $"Question: {questionInput.Text}\n\nAI Response:\n{answer}";
            });
        }

        #endregion

        #region Result Display Methods

        private void DisplayRateOptimizationResult(RateOptimizationResult result)
        {
            var text = $"RATE OPTIMIZATION RESULTS\n";
            text += $"Status: {(result.Success ? "✅ Success" : "❌ Failed")}\n";
            text += $"Calculation Method: {result.CalculationMethod}\n\n";

            if (result.Success)
            {
                text += "RECOMMENDED RATES:\n";
                foreach (var rate in result.OptimizedRates)
                {
                    text += $"• {rate.ServiceType}: ${rate.RecommendedRate:F2}/month\n";
                    text += $"  Current: ${rate.CurrentRate:F2} → Change: ${rate.RateChange:+F2;-F2;0} ({rate.PercentageChange:+P1;-P1;0%})\n";
                    text += $"  Justification: {rate.Justification}\n";
                    text += $"  Confidence: {rate.ConfidenceLevel:P1}\n\n";
                }

                if (result.RevenueProjection != null)
                {
                    text += "REVENUE IMPACT:\n";
                    text += $"• Projected Annual Revenue: ${result.RevenueProjection.ProjectedAnnualRevenue:N2}\n";
                    text += $"• Revenue Change: ${result.RevenueProjection.RevenueChange:+N2;-N2;0}\n\n";
                }

                if (result.CustomerImpactAssessment != null)
                {
                    text += "CUSTOMER IMPACT:\n";
                    text += $"• Average Monthly Increase: ${result.CustomerImpactAssessment.AverageMonthlyIncrease:F2}\n";
                    text += $"• Percentage Increase: {result.CustomerImpactAssessment.PercentageIncrease:P1}\n";
                    text += $"• Estimated Customer Loss: {result.CustomerImpactAssessment.EstimatedCustomerLoss:F0} customers\n\n";
                }

                if (!string.IsNullOrEmpty(result.AIAnalysis))
                {
                    text += "AI ANALYSIS:\n";
                    text += result.AIAnalysis + "\n\n";
                }
            }
            else
            {
                text += $"Error: {result.Error}\n";
            }

            rateResultsText.Text = text;
        }

        private void DisplayAffordabilityResult(AffordabilityAnalysisResult result)
        {
            var text = $"CUSTOMER AFFORDABILITY ANALYSIS\n";
            text += $"Status: {(result.Success ? "✅ Success" : "❌ Failed")}\n";
            text += $"Calculation Method: {result.CalculationMethod}\n\n";

            if (result.Success)
            {
                text += "AFFORDABILITY METRICS:\n";
                text += $"• Overall Affordability Score: {result.OverallAffordabilityScore:P1}\n";
                text += $"• Affordability Rating: {result.AffordabilityRating}\n";
                text += $"• Vulnerable Customers: {result.VulnerableCustomerPercentage:P1}\n";
                text += $"• Average Monthly Burden: ${result.AverageMonthlyBurden:F2}\n";
                text += $"• Percentage of Income: {result.PercentageOfIncome:P2}\n\n";

                if (result.RecommendedAssistancePrograms?.Count > 0)
                {
                    text += "RECOMMENDED ASSISTANCE PROGRAMS:\n";
                    foreach (var program in result.RecommendedAssistancePrograms)
                    {
                        text += $"• {program}\n";
                    }
                    text += "\n";
                }

                if (!string.IsNullOrEmpty(result.SocioeconomicImpactAnalysis))
                {
                    text += "SOCIOECONOMIC IMPACT ANALYSIS:\n";
                    text += result.SocioeconomicImpactAnalysis + "\n";
                }
            }
            else
            {
                text += $"Error: {result.Error}\n";
            }

            affordabilityResultsText.Text = text;
        }

        private void DisplayAnomalyResult(AnomalyDetectionResult result)
        {
            var text = $"FINANCIAL ANOMALY DETECTION\n";
            text += $"Status: {(result.Success ? "✅ Success" : "❌ Failed")}\n";
            text += $"Calculation Method: {result.CalculationMethod}\n\n";

            if (result.Success)
            {
                text += $"DETECTED ANOMALIES: {result.DetectedAnomalies?.Count ?? 0}\n\n";

                if (result.DetectedAnomalies?.Count > 0)
                {
                    foreach (var anomaly in result.DetectedAnomalies)
                    {
                        text += $"• {anomaly.Date:MMM yyyy}: ${anomaly.Value:N2}\n";
                        text += $"  Expected: ${anomaly.ExpectedValue:N2}, Deviation: ${anomaly.Deviation:N2}\n";
                        text += $"  Type: {anomaly.AnomalyType}, Severity: {anomaly.SeverityScore:F1}\n";
                        text += $"  {anomaly.Description}\n\n";
                    }
                }

                if (result.SeverityAssessment != null)
                {
                    text += "SEVERITY ASSESSMENT:\n";
                    text += $"• Overall Severity: {result.SeverityAssessment.OverallSeverity}\n";
                    text += $"• Critical: {result.SeverityAssessment.CriticalAnomalies}, ";
                    text += $"High: {result.SeverityAssessment.HighSeverityAnomalies}, ";
                    text += $"Medium: {result.SeverityAssessment.MediumSeverityAnomalies}\n\n";
                }

                if (!string.IsNullOrEmpty(result.AnomalyAnalysis))
                {
                    text += "ANALYSIS:\n";
                    text += result.AnomalyAnalysis + "\n";
                }
            }
            else
            {
                text += $"Error: {result.Error}\n";
            }

            anomalyResultsText.Text = text;
        }

        private void DisplayForecastResult(RevenueForecastResult result)
        {
            var text = $"REVENUE FORECAST\n";
            text += $"Status: {(result.Success ? "✅ Success" : "❌ Failed")}\n";
            text += $"Calculation Method: {result.CalculationMethod}\n\n";

            if (result.Success)
            {
                text += "MONTHLY PROJECTIONS:\n";
                for (int i = 0; i < Math.Min(result.MonthlyProjections.Count, 12); i++)
                {
                    var month = DateTime.Now.AddMonths(i + 1);
                    text += $"• {month:MMM yyyy}: ${result.MonthlyProjections[i]:N2}\n";
                }
                text += "\n";

                if (result.ScenarioAnalysis != null)
                {
                    text += "SCENARIO ANALYSIS (Year 1 Total):\n";
                    var optimistic = result.ScenarioAnalysis.OptimisticScenario.Take(12).Sum();
                    var realistic = result.ScenarioAnalysis.RealisticScenario.Take(12).Sum();
                    var pessimistic = result.ScenarioAnalysis.PessimisticScenario.Take(12).Sum();
                    
                    text += $"• Optimistic: ${optimistic:N2}\n";
                    text += $"• Realistic: ${realistic:N2}\n";
                    text += $"• Pessimistic: ${pessimistic:N2}\n\n";
                }

                if (result.KeyAssumptions?.Count > 0)
                {
                    text += "KEY ASSUMPTIONS:\n";
                    foreach (var assumption in result.KeyAssumptions)
                    {
                        text += $"• {assumption}\n";
                    }
                }
            }
            else
            {
                text += $"Error: {result.Error}\n";
            }

            forecastResultsText.Text = text;
        }

        private void DisplayComplianceResult(ComplianceCheckResult result)
        {
            var text = $"REGULATORY COMPLIANCE CHECK\n";
            text += $"Status: {(result.Success ? "✅ Success" : "❌ Failed")}\n";
            text += $"Calculation Method: {result.CalculationMethod}\n\n";

            if (result.Success)
            {
                text += $"OVERALL COMPLIANCE: {result.OverallComplianceScore:P1}\n\n";

                if (result.ComplianceGaps?.Count > 0)
                {
                    text += "COMPLIANCE GAPS:\n";
                    foreach (var gap in result.ComplianceGaps)
                    {
                        text += $"• {gap}\n";
                    }
                    text += "\n";
                }

                if (result.RecommendedActions?.Count > 0)
                {
                    text += "RECOMMENDED ACTIONS:\n";
                    foreach (var action in result.RecommendedActions)
                    {
                        text += $"• {action}\n";
                    }
                    text += "\n";
                }

                if (!string.IsNullOrEmpty(result.RegulatoryRiskAssessment))
                {
                    text += $"RISK ASSESSMENT: {result.RegulatoryRiskAssessment}\n\n";
                }

                if (!string.IsNullOrEmpty(result.ComplianceAnalysis))
                {
                    text += "COMPLIANCE ANALYSIS:\n";
                    text += result.ComplianceAnalysis + "\n";
                }
            }
            else
            {
                text += $"Error: {result.Error}\n";
            }

            complianceResultsText.Text = text;
        }

        #endregion

        private async Task ExecuteWithProgress(Func<Task> operation)
        {
            try
            {
                statusLabel.Text = "Processing...";
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;
                
                // Disable all buttons during operation
                foreach (Control control in this.Controls)
                {
                    if (control is Button button)
                        button.Enabled = false;
                }

                await operation();

                statusLabel.Text = "Operation completed successfully";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error: {ex.Message}";
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Visible = false;
                
                // Re-enable all buttons
                foreach (Control control in this.Controls)
                {
                    if (control is Button button)
                        button.Enabled = true;
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _financeManager?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
