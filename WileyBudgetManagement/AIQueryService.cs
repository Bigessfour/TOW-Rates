using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WileyBudgetManagement.Models;

namespace WileyBudgetManagement.Services
{
    /// <summary>
    /// AI Query Service for natural language budget analysis using XAI API
    /// Enables City Council "What-If" scenario processing and predictive analysis
    /// </summary>
    public class AIQueryService
    {
        private readonly HttpClient _httpClient;
        private readonly string _xaiApiKey;
        private readonly string _apiEndpoint = "https://api.x.ai/v1/chat/completions";

        public AIQueryService()
        {
            _httpClient = new HttpClient();
            _xaiApiKey = Environment.GetEnvironmentVariable("XAI_API_KEY");

            if (string.IsNullOrEmpty(_xaiApiKey))
            {
                throw new InvalidOperationException("XAI_API_KEY environment variable not found. Please set it using: [Environment]::SetEnvironmentVariable(\"XAI_API_KEY\", \"your-key\", \"Machine\")");
            }

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_xaiApiKey}");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Process natural language queries for municipal budget analysis
        /// </summary>
        /// <param name="query">Natural language question from City Council</param>
        /// <param name="enterpriseData">Current enterprise financial data</param>
        /// <returns>AI-generated analysis and recommendations</returns>
        public async Task<AIQueryResponse> ProcessNaturalLanguageQuery(string query, EnterpriseContext enterpriseData)
        {
            try
            {
                var systemPrompt = GetSystemPrompt();
                var dataContext = SerializeEnterpriseData(enterpriseData);

                var requestBody = new
                {
                    model = "grok-beta",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = $"Query: {query}\n\nCurrent Financial Data:\n{dataContext}" }
                    },
                    max_tokens = 2000,
                    temperature = 0.3,
                    top_p = 0.9
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var startTime = DateTime.Now;
                var response = await _httpClient.PostAsync(_apiEndpoint, content);
                var executionTime = (DateTime.Now - startTime).TotalMilliseconds;

                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var aiResponse = JsonConvert.DeserializeObject<dynamic>(responseText);
                    var analysisText = aiResponse.choices[0].message.content.ToString();

                    return new AIQueryResponse
                    {
                        Success = true,
                        Analysis = analysisText,
                        ExecutionTimeMs = (int)executionTime,
                        Timestamp = DateTime.Now,
                        Query = query,
                        EnterpriseScope = enterpriseData.Scope
                    };
                }
                else
                {
                    return new AIQueryResponse
                    {
                        Success = false,
                        Error = $"API Error: {response.StatusCode} - {responseText}",
                        ExecutionTimeMs = (int)executionTime,
                        Timestamp = DateTime.Now,
                        Query = query
                    };
                }
            }
            catch (Exception ex)
            {
                return new AIQueryResponse
                {
                    Success = false,
                    Error = $"AI Query Error: {ex.Message}",
                    ExecutionTimeMs = 0,
                    Timestamp = DateTime.Now,
                    Query = query
                };
            }
        }

        /// <summary>
        /// Generate specific "What-If" scenarios based on natural language input
        /// </summary>
        /// <param name="whatIfQuery">What-if question (e.g., "What if we delay truck purchase by 2 years?")</param>
        /// <param name="enterpriseData">Current enterprise data for analysis</param>
        /// <returns>Scenario analysis with financial impacts</returns>
        public async Task<ScenarioAnalysisResponse> GenerateWhatIfScenario(string whatIfQuery, EnterpriseContext enterpriseData)
        {
            try
            {
                var enhancedQuery = $"Generate a detailed financial scenario analysis for: {whatIfQuery}\n\n" +
                                  "Please provide:\n" +
                                  "1. Monthly financial impact\n" +
                                  "2. Annual budget implications\n" +
                                  "3. Customer rate impact (per customer)\n" +
                                  "4. Implementation timeline\n" +
                                  "5. Risk assessment\n" +
                                  "6. Alternative recommendations\n\n" +
                                  "Format as structured analysis with specific dollar amounts.";

                var response = await ProcessNaturalLanguageQuery(enhancedQuery, enterpriseData);

                if (response.Success)
                {
                    return new ScenarioAnalysisResponse
                    {
                        Success = true,
                        ScenarioName = ExtractScenarioName(whatIfQuery),
                        FinancialImpact = ParseFinancialImpact(response.Analysis),
                        Recommendations = ExtractRecommendations(response.Analysis),
                        RiskAssessment = ExtractRiskAssessment(response.Analysis),
                        FullAnalysis = response.Analysis,
                        ExecutionTimeMs = response.ExecutionTimeMs
                    };
                }
                else
                {
                    return new ScenarioAnalysisResponse
                    {
                        Success = false,
                        Error = response.Error
                    };
                }
            }
            catch (Exception ex)
            {
                return new ScenarioAnalysisResponse
                {
                    Success = false,
                    Error = $"Scenario generation error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Analyze cross-enterprise impact of proposed changes
        /// </summary>
        public async Task<CrossEnterpriseAnalysis> AnalyzeCrossEnterpriseImpact(string scenario,
            List<EnterpriseContext> allEnterprises)
        {
            try
            {
                var consolidatedData = allEnterprises.ToDictionary(e => e.Name, e => e);
                var analysisQuery = $"Analyze the cross-enterprise impact of: {scenario}\n\n" +
                                  "Consider effects on:\n" +
                                  "- Sewer Enterprise operations and rates\n" +
                                  "- Water Enterprise infrastructure and costs\n" +
                                  "- Trash Enterprise collection and processing\n" +
                                  "- Apartments Enterprise revenue and compliance\n\n" +
                                  "Provide municipal-wide financial implications and rate coordination recommendations.";

                var combinedContext = new EnterpriseContext
                {
                    Name = "Municipal-Wide Analysis",
                    Scope = "All Enterprises",
                    TotalBudget = allEnterprises.Sum(e => e.TotalBudget),
                    CustomerBase = allEnterprises.Max(e => e.CustomerBase) // Assuming overlapping customers
                };

                var response = await ProcessNaturalLanguageQuery(analysisQuery, combinedContext);

                return new CrossEnterpriseAnalysis
                {
                    Success = response.Success,
                    Analysis = response.Analysis,
                    AffectedEnterprises = allEnterprises.Select(e => e.Name).ToList(),
                    RecommendedActions = ExtractRecommendations(response.Analysis),
                    EstimatedImpact = ParseCrossEnterpriseImpact(response.Analysis),
                    Error = response.Error
                };
            }
            catch (Exception ex)
            {
                return new CrossEnterpriseAnalysis
                {
                    Success = false,
                    Error = $"Cross-enterprise analysis error: {ex.Message}"
                };
            }
        }

        private string GetSystemPrompt()
        {
            return @"You are a specialized municipal budget analysis AI for the Town of Wiley, Colorado. 
                    You help City Council members understand financial scenarios for Sewer, Water, Trash, and Apartments enterprises.
                    
                    Your expertise includes:
                    - Municipal utility rate setting according to EPA and Colorado state regulations
                    - Infrastructure investment analysis and debt service calculations
                    - Customer affordability analysis and rate impact assessments
                    - Cross-enterprise budget coordination and optimization
                    - Rate Study Methodology compliance and best practices
                    
                    Always provide:
                    1. Specific dollar amounts and percentages when possible
                    2. Clear explanations of financial reasoning
                    3. Risk assessments and alternative recommendations
                    4. Customer impact analysis (rate changes per customer)
                    5. Implementation timelines and considerations
                    6. Regulatory compliance implications
                    
                    Focus on practical, actionable insights that support informed municipal decision-making.
                    When analyzing scenarios, consider seasonal variations, affordability factors, and long-term sustainability.";
        }

        private string SerializeEnterpriseData(EnterpriseContext data)
        {
            try
            {
                return JsonConvert.SerializeObject(data, Formatting.Indented);
            }
            catch
            {
                return data.ToString();
            }
        }

        #region Response Parsing Methods

        private string ExtractScenarioName(string query)
        {
            // Extract meaningful scenario name from query
            if (query.ToLower().Contains("truck"))
                return "Trash Truck Investment Analysis";
            if (query.ToLower().Contains("water") && query.ToLower().Contains("plant"))
                return "Water Treatment Plant Scenario";
            if (query.ToLower().Contains("rate") && query.ToLower().Contains("increase"))
                return "Rate Adjustment Analysis";

            return "Custom Scenario Analysis";
        }

        private FinancialImpact ParseFinancialImpact(string analysis)
        {
            // Parse AI response for financial data
            // This is a simplified parser - could be enhanced with regex patterns
            return new FinancialImpact
            {
                MonthlyImpact = ExtractMonetaryValue(analysis, "monthly"),
                AnnualImpact = ExtractMonetaryValue(analysis, "annual"),
                CustomerImpact = ExtractMonetaryValue(analysis, "customer"),
                Description = "Parsed from AI analysis"
            };
        }

        private decimal ExtractMonetaryValue(string text, string context)
        {
            // Simplified monetary value extraction
            // In production, would use more sophisticated parsing
            try
            {
                var lines = text.Split('\n');
                var contextLines = lines.Where(l => l.ToLower().Contains(context) && l.Contains("$")).ToList();

                if (contextLines.Any())
                {
                    var line = contextLines.First();
                    var dollarIndex = line.IndexOf('$');
                    if (dollarIndex >= 0)
                    {
                        var valueText = line.Substring(dollarIndex + 1).Split(' ').First().Replace(",", "");
                        if (decimal.TryParse(valueText, out decimal value))
                            return value;
                    }
                }
            }
            catch { }

            return 0;
        }

        private List<string> ExtractRecommendations(string analysis)
        {
            var recommendations = new List<string>();
            var lines = analysis.Split('\n');
            bool inRecommendations = false;

            foreach (var line in lines)
            {
                if (line.ToLower().Contains("recommend") || line.ToLower().Contains("suggest"))
                    inRecommendations = true;

                if (inRecommendations && line.Trim().StartsWith("-"))
                    recommendations.Add(line.Trim().Substring(1).Trim());
            }

            return recommendations.Any() ? recommendations : new List<string> { "See full analysis for recommendations" };
        }

        private string ExtractRiskAssessment(string analysis)
        {
            var lines = analysis.Split('\n');
            var riskLines = lines.Where(l => l.ToLower().Contains("risk") || l.ToLower().Contains("concern")).ToList();

            return riskLines.Any() ? string.Join(" ", riskLines) : "Standard municipal project risks apply";
        }

        private CrossEnterpriseImpact ParseCrossEnterpriseImpact(string analysis)
        {
            return new CrossEnterpriseImpact
            {
                SewerImpact = ExtractMonetaryValue(analysis, "sewer"),
                WaterImpact = ExtractMonetaryValue(analysis, "water"),
                TrashImpact = ExtractMonetaryValue(analysis, "trash"),
                ApartmentsImpact = ExtractMonetaryValue(analysis, "apartment"),
                TotalMunicipalImpact = ExtractMonetaryValue(analysis, "total")
            };
        }

        #endregion

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    #region Response Models

    public class AIQueryResponse
    {
        public bool Success { get; set; }
        public string? Analysis { get; set; }
        public string? Error { get; set; }
        public int ExecutionTimeMs { get; set; }
        public DateTime Timestamp { get; set; }
        public string Query { get; set; }
        public string EnterpriseScope { get; set; }
    }

    public class ScenarioAnalysisResponse
    {
        public bool Success { get; set; }
        public string ScenarioName { get; set; }
        public FinancialImpact FinancialImpact { get; set; }
        public List<string> Recommendations { get; set; }
        public string RiskAssessment { get; set; }
        public string FullAnalysis { get; set; }
        public int ExecutionTimeMs { get; set; }
        public string Error { get; set; }
    }

    public class CrossEnterpriseAnalysis
    {
        public bool Success { get; set; }
        public string Analysis { get; set; }
        public List<string> AffectedEnterprises { get; set; }
        public List<string> RecommendedActions { get; set; }
        public CrossEnterpriseImpact EstimatedImpact { get; set; }
        public string Error { get; set; }
    }

    public class FinancialImpact
    {
        public decimal MonthlyImpact { get; set; }
        public decimal AnnualImpact { get; set; }
        public decimal CustomerImpact { get; set; }
        public string Description { get; set; }
    }

    public class CrossEnterpriseImpact
    {
        public decimal SewerImpact { get; set; }
        public decimal WaterImpact { get; set; }
        public decimal TrashImpact { get; set; }
        public decimal ApartmentsImpact { get; set; }
        public decimal TotalMunicipalImpact { get; set; }
    }

    #endregion
}
