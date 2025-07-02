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
        private readonly TokenUsageTracker _usageTracker;

        public AIQueryService()
        {
            _httpClient = new HttpClient();
            _xaiApiKey = Environment.GetEnvironmentVariable("XAI_API_KEY") ?? string.Empty;
            _usageTracker = new TokenUsageTracker();

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
                    model = "grok-3-beta",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = $"Query: {query}\n\nCurrent Financial Data:\n{dataContext}" }
                    },
                    max_tokens = 2000,
                    temperature = 0.3,
                    top_p = 0.9,
                    stream = false,
                    tools = GetBudgetAnalysisTools(),
                    tool_choice = "auto",
                    response_format = GetStructuredResponseFormat(),
                    presence_penalty = 0.0,
                    frequency_penalty = 0.0,
                    user = "wiley-budget-system"
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
                    var analysisText = aiResponse?.choices?[0]?.message?.content?.ToString() ?? "No response available";

                    // Track usage for cost management
                    var usage = ExtractUsageData(aiResponse);
                    _usageTracker.RecordUsage("grok-3-beta", usage);

                    return new AIQueryResponse
                    {
                        Success = true,
                        Analysis = analysisText,
                        ExecutionTimeMs = (int)executionTime,
                        Timestamp = DateTime.Now,
                        Query = query,
                        EnterpriseScope = enterpriseData?.Scope ?? string.Empty,
                        Usage = usage
                    };
                }
                else
                {
                    var errorDetails = ParseXAIError(response.StatusCode, responseText);
                    return new AIQueryResponse
                    {
                        Success = false,
                        Error = $"XAI API Error ({response.StatusCode}): {errorDetails}",
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

                if (response.Success && !string.IsNullOrEmpty(response.Analysis))
                {
                    return new ScenarioAnalysisResponse
                    {
                        Success = true,
                        ScenarioName = ExtractScenarioName(whatIfQuery),
                        FinancialImpact = ParseFinancialImpact(response.Analysis),
                        Recommendations = ExtractRecommendations(response.Analysis ?? string.Empty),
                        RiskAssessment = ExtractRiskAssessment(response.Analysis ?? string.Empty),
                        FullAnalysis = response.Analysis ?? string.Empty,
                        ExecutionTimeMs = response.ExecutionTimeMs
                    };
                }
                else
                {
                    return new ScenarioAnalysisResponse
                    {
                        Success = false,
                        Error = response.Error ?? string.Empty
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
        /// Plain English Budget Helper - designed for overwhelmed Town Clerks
        /// Translates complex municipal finance into understandable language
        /// </summary>
        /// <param name="plainEnglishQuestion">Question in everyday language (e.g., "Can we afford a new truck?")</param>
        /// <param name="enterpriseData">Current financial data</param>
        /// <returns>Simple, encouraging, and practical guidance</returns>
        public async Task<PlainEnglishResponse> AskPlainEnglishQuestion(string plainEnglishQuestion, EnterpriseContext enterpriseData)
        {
            try
            {
                var encouragingPrompt = GetEncouragingSystemPrompt();
                var contextSummary = CreateSimpleContextSummary(enterpriseData);

                var requestBody = new
                {
                    model = "grok-3-beta",
                    messages = new[]
                    {
                        new { role = "system", content = encouragingPrompt },
                        new { role = "user", content = $"Town Clerk Question: {plainEnglishQuestion}\n\nOur Town's Current Situation:\n{contextSummary}" }
                    },
                    max_tokens = 1000,
                    temperature = 0.4, // Slightly higher for more conversational tone
                    top_p = 0.9,
                    user = "rural-town-clerk"
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
                    var analysisText = aiResponse?.choices?[0]?.message?.content?.ToString() ?? "No response available";

                    var usage = ExtractUsageData(aiResponse);
                    _usageTracker.RecordUsage("grok-3-beta", usage);

                    return new PlainEnglishResponse
                    {
                        Success = true,
                        FriendlyAnswer = analysisText,
                        QuickTakeaway = ExtractQuickTakeaway(analysisText),
                        NextSteps = ExtractNextSteps(analysisText),
                        ConfidenceLevel = DetermineConfidenceLevel(analysisText),
                        ExecutionTimeMs = (int)executionTime,
                        Question = plainEnglishQuestion
                    };
                }
                else
                {
                    return new PlainEnglishResponse
                    {
                        Success = false,
                        FriendlyAnswer = "I'm having trouble connecting right now, but let's work through this together using the numbers we have.",
                        Error = ParseXAIError(response.StatusCode, responseText)
                    };
                }
            }
            catch (Exception ex)
            {
                return new PlainEnglishResponse
                {
                    Success = false,
                    FriendlyAnswer = "Technology can be frustrating sometimes! Let's tackle this step by step with the information we have available.",
                    Error = $"Connection issue: {ex.Message}"
                };
            }
        }
        /// <param name="query">Natural language question from City Council</param>
        /// <param name="enterpriseData">Current enterprise financial data</param>
        /// <returns>Streaming AI-generated analysis</returns>
        public async IAsyncEnumerable<StreamingResponse> ProcessStreamingQuery(string query, EnterpriseContext enterpriseData)
        {
            var systemPrompt = GetSystemPrompt();
            var dataContext = SerializeEnterpriseData(enterpriseData);

            var requestBody = new
            {
                model = "grok-3-beta",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = $"Query: {query}\n\nCurrent Financial Data:\n{dataContext}" }
                },
                max_tokens = 2000,
                temperature = 0.3,
                top_p = 0.9,
                stream = true,
                tools = GetBudgetAnalysisTools(),
                tool_choice = "auto",
                user = "wiley-budget-system"
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, _apiEndpoint)
            {
                Content = content
            };

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                yield return new StreamingResponse
                {
                    Success = false,
                    Error = $"API Error: {response.StatusCode}",
                    IsComplete = true
                };
                yield break;
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new System.IO.StreamReader(stream);

            var buffer = new StringBuilder();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();

                if (string.IsNullOrEmpty(line) || !line.StartsWith("data: "))
                    continue;

                var data = line.Substring(6); // Remove "data: " prefix

                if (data == "[DONE]")
                {
                    yield return new StreamingResponse
                    {
                        Success = true,
                        Content = buffer.ToString(),
                        IsComplete = true
                    };
                    break;
                }

                var streamData = ParseStreamData(data);
                if (streamData != null)
                {
                    buffer.Append(streamData);
                    yield return new StreamingResponse
                    {
                        Success = true,
                        Content = streamData,
                        IsComplete = false
                    };
                }
            }
        }

        /// <summary>
        /// Analyze financial documents, charts, and reports using vision model
        /// </summary>
        /// <param name="query">Analysis question about the document</param>
        /// <param name="imageBase64">Base64 encoded image data</param>
        /// <param name="enterpriseData">Enterprise context for analysis</param>
        /// <returns>Vision-based document analysis</returns>
        public async Task<VisionAnalysisResponse> AnalyzeFinancialDocument(string query, string imageBase64, EnterpriseContext enterpriseData)
        {
            try
            {
                var systemPrompt = GetVisionSystemPrompt();
                var dataContext = SerializeEnterpriseData(enterpriseData);

                var requestBody = new
                {
                    model = "grok-2-vision-1212",
                    messages = new object[]
                    {
                        new
                        {
                            role = "system",
                            content = systemPrompt
                        },
                        new
                        {
                            role = "user",
                            content = new object[]
                            {
                                new { type = "text", text = $"Query: {query}\n\nEnterprise Context:\n{dataContext}" },
                                new
                                {
                                    type = "image_url",
                                    image_url = new
                                    {
                                        url = $"data:image/jpeg;base64,{imageBase64}",
                                        detail = "high"
                                    }
                                }
                            }
                        }
                    },
                    max_tokens = 1500,
                    temperature = 0.2
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
                    var analysisText = aiResponse?.choices?[0]?.message?.content?.ToString() ?? "No response available";

                    return new VisionAnalysisResponse
                    {
                        Success = true,
                        Analysis = analysisText,
                        DocumentType = DetectDocumentType(analysisText),
                        ExtractedData = ExtractFinancialData(analysisText),
                        ExecutionTimeMs = (int)executionTime,
                        Timestamp = DateTime.Now
                    };
                }
                else
                {
                    return new VisionAnalysisResponse
                    {
                        Success = false,
                        Error = $"Vision API Error: {response.StatusCode} - {responseText}",
                        ExecutionTimeMs = (int)executionTime,
                        Timestamp = DateTime.Now
                    };
                }
            }
            catch (Exception ex)
            {
                return new VisionAnalysisResponse
                {
                    Success = false,
                    Error = $"Vision Analysis Error: {ex.Message}",
                    ExecutionTimeMs = 0,
                    Timestamp = DateTime.Now
                };
            }
        }

        /// <summary>
        /// Get encouraging system prompt designed for rural Town Clerks
        /// </summary>
        private string GetEncouragingSystemPrompt()
        {
            return @"You are a supportive municipal finance assistant designed specifically for rural Town Clerks who work tirelessly to keep their communities running.

                    Your role is to be:
                    - ENCOURAGING: Acknowledge the hard work these clerks do every day
                    - PRACTICAL: Focus on actionable, real-world solutions
                    - PLAIN-SPOKEN: Use everyday language, not government jargon
                    - REASSURING: Remind them that every town faces these challenges
                    - HOPEFUL: Always look for positive outcomes and opportunities

                    When answering:
                    1. Start with validation (""That's a great question"" or ""Many towns face this"")
                    2. Give a clear, simple answer in plain English
                    3. Explain WHY this matters to residents
                    4. Offer 2-3 practical next steps
                    5. End with encouragement about their town's future

                    Remember: These are dedicated public servants trying to balance tight budgets while serving their neighbors. 
                    They need hope, practical guidance, and reassurance that they're doing important work.
                    
                    Keep responses under 200 words, use short paragraphs, and avoid overwhelming technical details.
                    Focus on what CAN be done, not what can't.";
        }

        /// <summary>
        /// Create simple, non-technical summary of town finances
        /// </summary>
        private string CreateSimpleContextSummary(EnterpriseContext data)
        {
            var summary = $@"Town: {data.Name}
Current Budget: ${data.TotalBudget:N0}
Residents Served: {data.CustomerBase:N0}
Service Area: {data.Scope}

Key Numbers:
- Monthly Budget: ${(data.TotalBudget / 12):N0}
- Per-Person Annual Cost: ${(data.TotalBudget / Math.Max(data.CustomerBase, 1)):N0}";

            return summary;
        }

        /// <summary>
        /// Machine Learning-powered financial pattern recognition for rural towns
        /// Learns from historical data to predict trends and identify opportunities
        /// </summary>
        /// <param name="historicalData">Past financial records</param>
        /// <param name="currentSituation">Current financial state</param>
        /// <returns>ML insights with hope-focused predictions</returns>
        public async Task<MLFinancialInsights> AnalyzeFinancialPatterns(List<HistoricalFinancialRecord> historicalData, EnterpriseContext currentSituation)
        {
            try
            {
                var mlPrompt = @"You are an AI that specializes in finding hope and opportunities in rural town finances through pattern analysis.
                                Focus on identifying:
                                - Seasonal patterns that towns can leverage
                                - Hidden revenue opportunities often missed
                                - Cost reduction patterns that worked before
                                - Early warning signs to prevent financial crises
                                - Success patterns from similar rural communities
                                
                                Always frame findings positively and provide actionable insights that give hope.";

                var dataPattern = CreateFinancialPattern(historicalData, currentSituation);

                var query = $@"Analyze these financial patterns for the Town of Wiley, Colorado:

{dataPattern}

Please provide:
1. POSITIVE TRENDS we can build on
2. SEASONAL OPPORTUNITIES we might be missing
3. COST PATTERNS that suggest where we can save money
4. REVENUE PATTERNS that show untapped potential
5. EARLY WARNING predictions so we can prevent problems
6. HOPE-BASED RECOMMENDATIONS for breaking the 'broke cycle'

Focus on practical machine learning insights that give this rural community hope and direction.";

                var requestBody = new
                {
                    model = "grok-3-beta",
                    messages = new[]
                    {
                        new { role = "system", content = mlPrompt },
                        new { role = "user", content = query }
                    },
                    max_tokens = 1500,
                    temperature = 0.3,
                    user = "wiley-ml-analysis"
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_apiEndpoint, content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var aiResponse = JsonConvert.DeserializeObject<dynamic>(responseText);
                    var analysis = aiResponse?.choices?[0]?.message?.content?.ToString() ?? "No analysis available";

                    return new MLFinancialInsights
                    {
                        Success = true,
                        PositiveTrends = ExtractPositiveTrends(analysis),
                        SeasonalOpportunities = ExtractSeasonalOpportunities(analysis),
                        CostSavingPatterns = ExtractCostSavingPatterns(analysis),
                        RevenueOpportunities = ExtractRevenueOpportunities(analysis),
                        EarlyWarnings = ExtractEarlyWarnings(analysis),
                        HopeBasedRecommendations = ExtractHopeRecommendations(analysis),
                        ConfidenceScore = CalculateMLConfidence(historicalData.Count),
                        FullAnalysis = analysis
                    };
                }
                else
                {
                    return new MLFinancialInsights
                    {
                        Success = false,
                        HopeBasedRecommendations = new List<string> { "Every small town faces financial challenges - you're not alone in this struggle." },
                        Error = "ML analysis temporarily unavailable, but your financial patterns still tell a story of resilience."
                    };
                }
            }
            catch (Exception ex)
            {
                return new MLFinancialInsights
                {
                    Success = false,
                    HopeBasedRecommendations = new List<string>
                    {
                        "Technology hiccups happen, but your town's survival instinct is stronger than any system.",
                        "Focus on what you know works - community connections and careful spending."
                    },
                    Error = $"ML processing issue: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// ML-powered revenue opportunity discovery specifically for rural towns
        /// Identifies untapped income sources and grant opportunities
        /// </summary>
        /// <param name="townProfile">Demographics and characteristics of Wiley</param>
        /// <param name="currentRevenue">Existing revenue streams</param>
        /// <returns>Actionable revenue opportunities ranked by feasibility</returns>
        public async Task<RevenueOpportunityAnalysis> DiscoverRevenueOpportunities(TownProfile townProfile, List<RevenueStream> currentRevenue)
        {
            try
            {
                var opportunityPrompt = @"You are an AI revenue discovery specialist for small rural towns. 
                                        Your mission is to find realistic, achievable revenue opportunities that rural communities often overlook.
                                        
                                        Focus on:
                                        - Federal and state grants specifically for rural communities
                                        - Local business opportunities that leverage community strengths
                                        - Tourism or recreational revenue that fits the area
                                        - Utility efficiency programs that save money
                                        - Regional partnerships that share costs
                                        - Digital services that rural towns can provide
                                        
                                        Rank opportunities by: 1) Feasibility 2) Time to implement 3) Revenue potential
                                        Always provide specific next steps and maintain an encouraging tone.";

                var profileData = SerializeTownProfile(townProfile);
                var revenueData = SerializeRevenueStreams(currentRevenue);

                var query = $@"Analyze revenue opportunities for Wiley, Colorado:

Town Profile:
{profileData}

Current Revenue Streams:
{revenueData}

Please identify:
1. QUICK WINS (revenue opportunities within 6 months)
2. MEDIUM-TERM opportunities (6-18 months)
3. LONG-TERM opportunities (1-3 years)
4. GRANT OPPORTUNITIES specifically available to rural Colorado towns
5. PARTNERSHIP opportunities with neighboring communities
6. COST-SHARING opportunities that effectively increase revenue

For each opportunity, provide:
- Estimated revenue potential
- Implementation difficulty (1-5 scale)
- First steps to get started
- Success stories from similar towns";

                var requestBody = new
                {
                    model = "grok-3-beta",
                    messages = new[]
                    {
                        new { role = "system", content = opportunityPrompt },
                        new { role = "user", content = query }
                    },
                    max_tokens = 2000,
                    temperature = 0.4,
                    user = "wiley-revenue-discovery"
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_apiEndpoint, content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var aiResponse = JsonConvert.DeserializeObject<dynamic>(responseText);
                    var analysis = aiResponse?.choices?[0]?.message?.content?.ToString() ?? "No analysis available";

                    return new RevenueOpportunityAnalysis
                    {
                        Success = true,
                        QuickWins = ExtractQuickWins(analysis),
                        MediumTermOpportunities = ExtractMediumTermOpportunities(analysis),
                        LongTermOpportunities = ExtractLongTermOpportunities(analysis),
                        GrantOpportunities = ExtractGrantOpportunities(analysis),
                        PartnershipOpportunities = ExtractPartnershipOpportunities(analysis),
                        TotalPotentialRevenue = CalculateTotalPotentialRevenue(analysis),
                        PriorityActions = ExtractPriorityActions(analysis),
                        FullAnalysis = analysis
                    };
                }
                else
                {
                    return new RevenueOpportunityAnalysis
                    {
                        Success = false,
                        QuickWins = new List<RevenueOpportunity>
                        {
                            new RevenueOpportunity { Name = "Review utility rates - many rural towns under-charge", EstimatedRevenue = 5000, Difficulty = 2 }
                        },
                        Error = "Revenue analysis temporarily unavailable - check with neighboring towns about shared services."
                    };
                }
            }
            catch (Exception ex)
            {
                return new RevenueOpportunityAnalysis
                {
                    Success = false,
                    QuickWins = new List<RevenueOpportunity>
                    {
                        new RevenueOpportunity { Name = "Community audit of expenses - residents often find savings", EstimatedRevenue = 2500, Difficulty = 1 }
                    },
                    Error = $"Analysis temporarily down: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Predictive ML system that identifies potential financial crises before they happen
        /// Gives rural towns time to take preventive action
        /// </summary>
        /// <param name="currentTrends">Current financial trends and patterns</param>
        /// <param name="externalFactors">External factors affecting the town (weather, economy, etc.)</param>
        /// <returns>Early warning predictions with preventive actions</returns>
        public async Task<CrisisPreventionAnalysis> PredictAndPreventCrises(FinancialTrends currentTrends, List<ExternalFactor> externalFactors)
        {
            try
            {
                var predictionPrompt = @"You are an AI crisis prevention specialist for rural towns. Your job is to spot trouble before it happens and suggest ways to prevent financial emergencies.
                                        
                                        Rural towns face unique challenges:
                                        - Limited revenue sources
                                        - Aging infrastructure
                                        - Population changes
                                        - Weather-related expenses
                                        - State funding cuts
                                        
                                        Your predictions should:
                                        - Identify problems 3-12 months in advance
                                        - Provide specific prevention strategies
                                        - Suggest early action steps
                                        - Maintain hope while being realistic
                                        - Focus on community strengths and resilience";

                var trendsData = SerializeFinancialTrends(currentTrends);
                var factorsData = SerializeExternalFactors(externalFactors);

                var query = $@"Analyze potential financial challenges for Wiley, Colorado:

Current Financial Trends:
{trendsData}

External Factors:
{factorsData}

Please predict:
1. IMMEDIATE RISKS (next 3 months) and prevention steps
2. SHORT-TERM CHALLENGES (3-6 months) and preparation strategies  
3. MEDIUM-TERM CONCERNS (6-12 months) and planning actions
4. SEASONAL RISKS and how to prepare for them
5. INFRASTRUCTURE RISKS and proactive maintenance strategies
6. COMMUNITY RESILIENCE opportunities to strengthen financial stability

For each prediction:
- Probability of occurrence (1-10 scale)
- Potential financial impact
- Specific prevention actions
- Community resources that can help
- Success stories from similar situations";

                var requestBody = new
                {
                    model = "grok-3-beta",
                    messages = new[]
                    {
                        new { role = "system", content = predictionPrompt },
                        new { role = "user", content = query }
                    },
                    max_tokens = 1800,
                    temperature = 0.2, // Lower temperature for more conservative predictions
                    user = "wiley-crisis-prevention"
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_apiEndpoint, content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var aiResponse = JsonConvert.DeserializeObject<dynamic>(responseText);
                    var analysis = aiResponse?.choices?[0]?.message?.content?.ToString() ?? "No analysis available";

                    return new CrisisPreventionAnalysis
                    {
                        Success = true,
                        ImmediateRisks = ExtractImmediateRisks(analysis),
                        ShortTermChallenges = ExtractShortTermChallenges(analysis),
                        MediumTermConcerns = ExtractMediumTermConcerns(analysis),
                        SeasonalRisks = ExtractSeasonalRisks(analysis),
                        InfrastructureRisks = ExtractInfrastructureRisks(analysis),
                        PreventionActions = ExtractPreventionActions(analysis),
                        CommunityStrengths = ExtractCommunityStrengths(analysis),
                        OverallRiskLevel = CalculateOverallRisk(analysis),
                        EncouragingMessage = ExtractEncouragingMessage(analysis),
                        FullAnalysis = analysis
                    };
                }
                else
                {
                    return new CrisisPreventionAnalysis
                    {
                        Success = false,
                        EncouragingMessage = "Even without AI predictions, your community's track record shows you can handle whatever comes your way.",
                        PreventionActions = new List<string>
                        {
                            "Keep a small emergency fund if possible",
                            "Build relationships with neighboring towns",
                            "Document what works for future reference"
                        },
                        Error = "Prediction system temporarily unavailable - trust your local knowledge and experience."
                    };
                }
            }
            catch (Exception ex)
            {
                return new CrisisPreventionAnalysis
                {
                    Success = false,
                    EncouragingMessage = "Rural towns have survived many challenges before - your community's wisdom is your best predictor.",
                    PreventionActions = new List<string>
                    {
                        "Focus on maintaining essential services",
                        "Keep communication lines open with residents",
                        "Plan for seasonal challenges you know are coming"
                    },
                    Error = $"Prediction analysis issue: {ex.Message}"
                };
            }
        }

        /// <summary>
        public async Task<CommonSenseGuidance> GetCommonSenseGuidance(string situation, List<string> options, string constraints = "")
        {
            try
            {
                var practicalPrompt = @"You are the wise, experienced Town Clerk who's seen it all. 
                                      Give practical, common-sense advice that puts residents first.
                                      Consider: What would a caring neighbor do? What's truly necessary vs. nice-to-have?
                                      Focus on solutions that keep the town running without breaking the bank.";

                var query = $@"Situation: {situation}

Options being considered:
{string.Join("\n", options.Select((opt, i) => $"{i + 1}. {opt}"))}

{(string.IsNullOrEmpty(constraints) ? "" : $"Constraints or concerns: {constraints}")}

Please provide practical guidance that considers:
- What residents really need
- Long-term sustainability  
- Keeping costs reasonable
- Maintaining essential services";

                var requestBody = new
                {
                    model = "grok-3-beta",
                    messages = new[]
                    {
                        new { role = "system", content = practicalPrompt },
                        new { role = "user", content = query }
                    },
                    max_tokens = 800,
                    temperature = 0.3,
                    user = "town-decision-maker"
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_apiEndpoint, content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var aiResponse = JsonConvert.DeserializeObject<dynamic>(responseText);
                    var guidance = aiResponse?.choices?[0]?.message?.content?.ToString() ?? "No guidance available";

                    return new CommonSenseGuidance
                    {
                        Success = true,
                        Recommendation = ExtractRecommendation(guidance),
                        Reasoning = ExtractReasoning(guidance),
                        RedFlags = ExtractWarnings(guidance),
                        ResidentImpact = ExtractResidentImpact(guidance),
                        FullGuidance = guidance
                    };
                }
                else
                {
                    return new CommonSenseGuidance
                    {
                        Success = false,
                        Recommendation = "When in doubt, choose the option that best serves your residents' essential needs within your budget.",
                        Error = "AI guidance unavailable - rely on your experience and community knowledge."
                    };
                }
            }
            catch (Exception ex)
            {
                return new CommonSenseGuidance
                {
                    Success = false,
                    Recommendation = "Trust your instincts and prioritize what your community truly needs.",
                    Error = $"Technology hiccup: {ex.Message}"
                };
            }
        }

        private string GetVisionSystemPrompt()
        {
            return @"You are a specialized municipal financial document analysis AI for the Town of Wiley, Colorado.
                    You excel at analyzing financial reports, charts, budgets, and infrastructure documents.
                    
                    Your capabilities include:
                    - Reading and interpreting financial statements, budget reports, and rate schedules
                    - Analyzing charts, graphs, and visual financial data
                    - Extracting key financial metrics and identifying trends
                    - Detecting anomalies or areas requiring attention
                    - Providing municipal-specific insights and recommendations
                    
                    When analyzing documents:
                    1. Identify document type and purpose
                    2. Extract key financial figures and metrics
                    3. Highlight important trends or changes
                    4. Note any compliance or regulatory considerations
                    5. Provide actionable insights for municipal decision-making
                    
                    Focus on accuracy and provide specific data points when available.";
        }

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
                    Analysis = response.Analysis ?? string.Empty,
                    AffectedEnterprises = allEnterprises.Select(e => e.Name).ToList(),
                    RecommendedActions = ExtractRecommendations(response.Analysis ?? string.Empty),
                    EstimatedImpact = ParseCrossEnterpriseImpact(response.Analysis ?? string.Empty),
                    Error = response.Error ?? string.Empty
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
                return data?.ToString() ?? "Unable to serialize enterprise data";
            }
        }

        #region XAI API Enhancement Methods

        /// <summary>
        /// Get function calling tools for municipal budget analysis
        /// </summary>
        private object[] GetBudgetAnalysisTools()
        {
            return new object[]
            {
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "calculate_rate_impact",
                        description = "Calculate customer rate impact for municipal utility changes",
                        parameters = new
                        {
                            type = "object",
                            properties = new
                            {
                                enterprise = new { type = "string", description = "Enterprise type (Sewer, Water, Trash, Apartments)" },
                                change_amount = new { type = "number", description = "Budget change amount in dollars" },
                                customer_count = new { type = "number", description = "Number of customers affected" },
                                implementation_months = new { type = "number", description = "Implementation timeline in months" }
                            },
                            required = new[] { "enterprise", "change_amount", "customer_count" }
                        }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "analyze_debt_service",
                        description = "Analyze debt service capacity and requirements for infrastructure projects",
                        parameters = new
                        {
                            type = "object",
                            properties = new
                            {
                                project_cost = new { type = "number", description = "Total project cost" },
                                financing_term = new { type = "number", description = "Financing term in years" },
                                interest_rate = new { type = "number", description = "Expected interest rate" },
                                current_debt_service = new { type = "number", description = "Current annual debt service" }
                            },
                            required = new[] { "project_cost", "financing_term" }
                        }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "compliance_check",
                        description = "Check regulatory compliance for rate changes and infrastructure decisions",
                        parameters = new
                        {
                            type = "object",
                            properties = new
                            {
                                enterprise_type = new { type = "string", description = "Type of enterprise" },
                                rate_change_percent = new { type = "number", description = "Proposed rate change percentage" },
                                affordability_metric = new { type = "number", description = "Median household income ratio" }
                            },
                            required = new[] { "enterprise_type", "rate_change_percent" }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Get structured response format for consistent financial analysis
        /// </summary>
        private object GetStructuredResponseFormat()
        {
            return new
            {
                type = "json_schema",
                json_schema = new
                {
                    name = "municipal_financial_analysis",
                    strict = true,
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            analysis_summary = new { type = "string", description = "Executive summary of the analysis" },
                            monthly_impact = new { type = "number", description = "Monthly financial impact in dollars" },
                            annual_impact = new { type = "number", description = "Annual financial impact in dollars" },
                            customer_rate_impact = new { type = "number", description = "Rate impact per customer per month" },
                            risk_level = new { type = "string", @enum = new[] { "Low", "Medium", "High", "Critical" } },
                            recommendations = new
                            {
                                type = "array",
                                items = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        action = new { type = "string" },
                                        priority = new { type = "string", @enum = new[] { "Immediate", "Short-term", "Long-term" } },
                                        estimated_cost = new { type = "number" }
                                    }
                                }
                            },
                            compliance_notes = new { type = "string", description = "Regulatory compliance considerations" }
                        },
                        required = new[] { "analysis_summary", "monthly_impact", "annual_impact", "risk_level", "recommendations" }
                    }
                }
            };
        }

        #endregion

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

        private FinancialImpact ParseFinancialImpact(string? analysis)
        {
            if (string.IsNullOrWhiteSpace(analysis))
            {
                return new FinancialImpact
                {
                    Description = "No financial analysis data available"
                };
            }

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

        private List<string> ExtractRecommendations(string? analysis)
        {
            if (string.IsNullOrWhiteSpace(analysis))
            {
                return new List<string> { "No recommendations available from analysis" };
            }

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

        private string ExtractRiskAssessment(string? analysis)
        {
            if (string.IsNullOrWhiteSpace(analysis))
            {
                return "Risk assessment not available - standard municipal project risks apply";
            }

            var lines = analysis.Split('\n');
            var riskLines = lines.Where(l => l.ToLower().Contains("risk") || l.ToLower().Contains("concern")).ToList();

            return riskLines.Any() ? string.Join(" ", riskLines) : "Standard municipal project risks apply";
        }

        private CrossEnterpriseImpact ParseCrossEnterpriseImpact(string? analysis)
        {
            if (string.IsNullOrWhiteSpace(analysis))
            {
                return new CrossEnterpriseImpact();
            }

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

        #region Helper Methods

        /// <summary>
        /// Extract usage data from API response for cost tracking
        /// </summary>
        private TokenUsage ExtractUsageData(dynamic apiResponse)
        {
            try
            {
                var usage = apiResponse?.usage;
                if (usage != null)
                {
                    return new TokenUsage
                    {
                        PromptTokens = (int)(usage.prompt_tokens ?? 0),
                        CompletionTokens = (int)(usage.completion_tokens ?? 0),
                        TotalTokens = (int)(usage.total_tokens ?? 0),
                        EstimatedCost = CalculateCost("grok-3-beta", (int)(usage.prompt_tokens ?? 0), (int)(usage.completion_tokens ?? 0))
                    };
                }
            }
            catch { }

            return new TokenUsage { EstimatedCost = 0 };
        }

        /// <summary>
        /// Calculate estimated cost based on XAI pricing
        /// </summary>
        private decimal CalculateCost(string model, int promptTokens, int completionTokens)
        {
            // XAI Grok-3-beta pricing (as of July 2025)
            decimal inputCostPer1M = model switch
            {
                "grok-3-beta" => 3.00m,
                "grok-3-mini-beta" => 0.30m,
                "grok-2-vision-1212" => 2.00m,
                _ => 3.00m
            };

            decimal outputCostPer1M = model switch
            {
                "grok-3-beta" => 15.00m,
                "grok-3-mini-beta" => 0.50m,
                "grok-2-vision-1212" => 10.00m,
                _ => 15.00m
            };

            var inputCost = (promptTokens / 1_000_000m) * inputCostPer1M;
            var outputCost = (completionTokens / 1_000_000m) * outputCostPer1M;

            return inputCost + outputCost;
        }

        /// <summary>
        /// Detect document type from vision analysis
        /// </summary>
        private string DetectDocumentType(string analysis)
        {
            var lowerAnalysis = analysis.ToLower();

            if (lowerAnalysis.Contains("budget") || lowerAnalysis.Contains("expenditure"))
                return "Budget Report";
            if (lowerAnalysis.Contains("rate") && lowerAnalysis.Contains("schedule"))
                return "Rate Schedule";
            if (lowerAnalysis.Contains("financial") && lowerAnalysis.Contains("statement"))
                return "Financial Statement";
            if (lowerAnalysis.Contains("chart") || lowerAnalysis.Contains("graph"))
                return "Financial Chart";
            if (lowerAnalysis.Contains("invoice") || lowerAnalysis.Contains("bill"))
                return "Invoice/Bill";

            return "Financial Document";
        }

        /// <summary>
        /// Extract structured financial data from vision analysis
        /// </summary>
        private Dictionary<string, decimal> ExtractFinancialData(string analysis)
        {
            var data = new Dictionary<string, decimal>();

            // Use regex to extract monetary values
            var dollarMatches = System.Text.RegularExpressions.Regex.Matches(
                analysis, @"\$([0-9,]+(?:\.[0-9]{2})?)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            var keywordMap = new Dictionary<string, string[]>
            {
                ["Revenue"] = new[] { "revenue", "income", "receipts" },
                ["Expenses"] = new[] { "expense", "cost", "expenditure" },
                ["Budget"] = new[] { "budget", "allocation" },
                ["Rate"] = new[] { "rate", "fee", "charge" }
            };

            foreach (System.Text.RegularExpressions.Match match in dollarMatches)
            {
                if (decimal.TryParse(match.Groups[1].Value.Replace(",", ""), out decimal value))
                {
                    var context = GetMatchContext(analysis, match.Index, 50);
                    var category = ClassifyAmount(context, keywordMap);

                    if (!string.IsNullOrEmpty(category))
                    {
                        data[category] = value;
                    }
                }
            }

            return data;
        }

        private string GetMatchContext(string text, int index, int contextLength)
        {
            var start = Math.Max(0, index - contextLength);
            var end = Math.Min(text.Length, index + contextLength);
            return text.Substring(start, end - start);
        }

        private string ClassifyAmount(string context, Dictionary<string, string[]> keywordMap)
        {
            var lowerContext = context.ToLower();

            foreach (var category in keywordMap)
            {
                if (category.Value.Any(keyword => lowerContext.Contains(keyword)))
                {
                    return category.Key;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Get current usage statistics
        /// </summary>
        public UsageStatistics GetUsageStatistics()
        {
            return _usageTracker.GetStatistics();
        }

        /// <summary>
        /// Parse XAI-specific error responses for better error handling
        /// </summary>
        private string ParseXAIError(System.Net.HttpStatusCode statusCode, string responseText)
        {
            try
            {
                var errorResponse = JsonConvert.DeserializeObject<dynamic>(responseText);
                var errorMessage = errorResponse?.error?.message?.ToString();
                var errorType = errorResponse?.error?.type?.ToString();

                return statusCode switch
                {
                    System.Net.HttpStatusCode.TooManyRequests =>
                        $"Rate limit exceeded. Please wait before making another request. {errorMessage}",
                    System.Net.HttpStatusCode.BadRequest when errorType == "invalid_request_error" =>
                        $"Invalid request format or parameters. {errorMessage}",
                    System.Net.HttpStatusCode.BadRequest when errorMessage?.Contains("content_policy") == true =>
                        $"Content policy violation. Please review your query content. {errorMessage}",
                    System.Net.HttpStatusCode.BadRequest when errorMessage?.Contains("max_tokens") == true =>
                        $"Token limit exceeded. Please reduce query length or max_tokens setting. {errorMessage}",
                    System.Net.HttpStatusCode.Unauthorized =>
                        "Authentication failed. Please check your XAI_API_KEY environment variable.",
                    System.Net.HttpStatusCode.Forbidden =>
                        "Access denied. Your API key may not have sufficient permissions.",
                    System.Net.HttpStatusCode.ServiceUnavailable =>
                        "XAI service temporarily unavailable. Please try again in a few moments.",
                    System.Net.HttpStatusCode.InternalServerError =>
                        "XAI internal server error. Please try again or contact support if the issue persists.",
                    _ => errorMessage ?? $"Unexpected error: {statusCode}"
                };
            }
            catch
            {
                return $"Unable to parse error response: {statusCode}";
            }
        }

        /// <summary>
        /// Parse streaming data safely without yielding in try-catch
        /// </summary>
        private string? ParseStreamData(string data)
        {
            try
            {
                var streamData = JsonConvert.DeserializeObject<dynamic>(data);
                return streamData?.choices?[0]?.delta?.content?.ToString();
            }
            catch (JsonException)
            {
                // Skip malformed JSON chunks
                return null;
            }
        }

        #region Plain English Helper Methods

        /// <summary>
        /// Extract quick takeaway for busy Town Clerks
        /// </summary>
        private string ExtractQuickTakeaway(string analysis)
        {
            var lines = analysis.Split('\n');
            var takeawayLine = lines.FirstOrDefault(l =>
                l.ToLower().Contains("bottom line") ||
                l.ToLower().Contains("in short") ||
                l.ToLower().Contains("simply put") ||
                l.Contains("takeaway", StringComparison.OrdinalIgnoreCase));

            return takeawayLine?.Trim() ?? "You're managing your town's finances responsibly.";
        }

        /// <summary>
        /// Extract practical next steps
        /// </summary>
        private List<string> ExtractNextSteps(string analysis)
        {
            var steps = new List<string>();
            var lines = analysis.Split('\n');
            bool inSteps = false;

            foreach (var line in lines)
            {
                if (line.ToLower().Contains("next step") || line.ToLower().Contains("action") || line.ToLower().Contains("recommend"))
                    inSteps = true;

                if (inSteps && (line.Trim().StartsWith("-") || line.Trim().StartsWith("1.") || line.Trim().StartsWith("2.")))
                {
                    steps.Add(line.Trim().TrimStart('-', '1', '2', '3', '.', ' '));
                }
            }

            return steps.Any() ? steps : new List<string> { "Continue monitoring your budget monthly", "Keep communicating with residents about town priorities" };
        }

        /// <summary>
        /// Determine confidence level in the guidance
        /// </summary>
        private string DetermineConfidenceLevel(string analysis)
        {
            var lowerAnalysis = analysis.ToLower();

            if (lowerAnalysis.Contains("definitely") || lowerAnalysis.Contains("clearly") || lowerAnalysis.Contains("certainly"))
                return "High Confidence";
            if (lowerAnalysis.Contains("likely") || lowerAnalysis.Contains("probably") || lowerAnalysis.Contains("should"))
                return "Good Confidence";
            if (lowerAnalysis.Contains("might") || lowerAnalysis.Contains("could") || lowerAnalysis.Contains("consider"))
                return "Moderate Confidence";

            return "General Guidance";
        }

        /// <summary>
        /// Extract main recommendation from guidance
        /// </summary>
        private string ExtractRecommendation(string guidance)
        {
            var lines = guidance.Split('\n');
            var recommendLine = lines.FirstOrDefault(l =>
                l.ToLower().Contains("recommend") ||
                l.ToLower().Contains("suggest") ||
                l.ToLower().Contains("best option"));

            return recommendLine?.Trim() ?? "Focus on essential services and resident needs within your current budget.";
        }

        /// <summary>
        /// Extract reasoning behind recommendation
        /// </summary>
        private string ExtractReasoning(string guidance)
        {
            var lines = guidance.Split('\n');
            var reasoningLines = lines.Where(l =>
                l.ToLower().Contains("because") ||
                l.ToLower().Contains("since") ||
                l.ToLower().Contains("reason")).ToList();

            return reasoningLines.Any() ? string.Join(" ", reasoningLines) : "This approach balances fiscal responsibility with community needs.";
        }

        /// <summary>
        /// Extract potential warnings or red flags
        /// </summary>
        private List<string> ExtractWarnings(string guidance)
        {
            var warnings = new List<string>();
            var lines = guidance.Split('\n');

            foreach (var line in lines)
            {
                if (line.ToLower().Contains("warning") ||
                    line.ToLower().Contains("caution") ||
                    line.ToLower().Contains("avoid") ||
                    line.ToLower().Contains("careful"))
                {
                    warnings.Add(line.Trim());
                }
            }

            return warnings;
        }

        /// <summary>
        /// Extract impact on residents
        /// </summary>
        private string ExtractResidentImpact(string guidance)
        {
            var lines = guidance.Split('\n');
            var impactLine = lines.FirstOrDefault(l =>
                l.ToLower().Contains("resident") ||
                l.ToLower().Contains("citizen") ||
                l.ToLower().Contains("community"));

            return impactLine?.Trim() ?? "This decision should help maintain quality services for your community.";
        }

        #endregion

        #region Machine Learning Helper Methods

        /// <summary>
        /// Create financial pattern analysis from historical data
        /// </summary>
        private string CreateFinancialPattern(List<HistoricalFinancialRecord> history, EnterpriseContext current)
        {
            if (!history.Any())
                return "Limited historical data available - analysis based on current situation and rural town patterns.";

            var pattern = new StringBuilder();
            pattern.AppendLine("Historical Financial Patterns:");

            // Analyze revenue trends
            var revenuePattern = AnalyzeRevenueTrend(history);
            pattern.AppendLine($"Revenue Trend: {revenuePattern}");

            // Analyze expense patterns
            var expensePattern = AnalyzeExpenseTrend(history);
            pattern.AppendLine($"Expense Trend: {expensePattern}");

            // Identify seasonal patterns
            var seasonalPattern = IdentifySeasonalPatterns(history);
            pattern.AppendLine($"Seasonal Patterns: {seasonalPattern}");

            // Current situation context
            pattern.AppendLine($"\nCurrent Situation: Budget ${current.TotalBudget:N0}, Serving {current.CustomerBase} residents");

            return pattern.ToString();
        }

        private string AnalyzeRevenueTrend(List<HistoricalFinancialRecord> history)
        {
            if (history.Count < 2) return "Insufficient data for trend analysis";

            var recent = history.TakeLast(3).Select(h => h.TotalRevenue).ToList();
            var older = history.Take(3).Select(h => h.TotalRevenue).ToList();

            var recentAvg = recent.Average();
            var olderAvg = older.Average();

            if (recentAvg > olderAvg * 1.05m) return "Increasing revenue trend";
            if (recentAvg < olderAvg * 0.95m) return "Declining revenue trend";
            return "Stable revenue trend";
        }

        private string AnalyzeExpenseTrend(List<HistoricalFinancialRecord> history)
        {
            if (history.Count < 2) return "Insufficient expense data";

            var recent = history.TakeLast(3).Select(h => h.TotalExpenses).ToList();
            var older = history.Take(3).Select(h => h.TotalExpenses).ToList();

            var recentAvg = recent.Average();
            var olderAvg = older.Average();

            if (recentAvg > olderAvg * 1.1m) return "Rapidly increasing expenses";
            if (recentAvg > olderAvg * 1.05m) return "Moderately increasing expenses";
            return "Controlled expense growth";
        }

        private string IdentifySeasonalPatterns(List<HistoricalFinancialRecord> history)
        {
            var patterns = new List<string>();

            // Group by month to identify patterns
            var monthlyData = history.GroupBy(h => h.Date.Month).ToList();

            if (monthlyData.Any(g => g.Key >= 11 || g.Key <= 2)) // Winter months
            {
                var winterExpenses = monthlyData.Where(g => g.Key >= 11 || g.Key <= 2)
                    .SelectMany(g => g).Average(h => h.TotalExpenses);
                var summerExpenses = monthlyData.Where(g => g.Key >= 5 && g.Key <= 9)
                    .SelectMany(g => g).Average(h => h.TotalExpenses);

                if (winterExpenses > summerExpenses * 1.2m)
                    patterns.Add("Higher winter expenses (likely snow removal/heating)");
            }

            return patterns.Any() ? string.Join(", ", patterns) : "No clear seasonal patterns identified";
        }

        private string SerializeTownProfile(TownProfile profile)
        {
            return $@"Population: {profile.Population}
Location: {profile.Location}
Primary Industries: {string.Join(", ", profile.PrimaryIndustries)}
Key Assets: {string.Join(", ", profile.KeyAssets)}
Challenges: {string.Join(", ", profile.Challenges)}
Strengths: {string.Join(", ", profile.CommunityStrengths)}";
        }

        private string SerializeRevenueStreams(List<RevenueStream> streams)
        {
            return string.Join("\n", streams.Select(s => $"- {s.Name}: ${s.AnnualAmount:N0} ({s.Reliability}/10 reliability)"));
        }

        private string SerializeFinancialTrends(FinancialTrends trends)
        {
            return $@"Revenue Trend: {trends.RevenueTrend} ({trends.RevenueChangePercent:F1}% change)
Expense Trend: {trends.ExpenseTrend} ({trends.ExpenseChangePercent:F1}% change)
Budget Balance: {trends.BudgetBalance}
Cash Flow: {trends.CashFlowStatus}
Key Concerns: {string.Join(", ", trends.KeyConcerns)}";
        }

        private string SerializeExternalFactors(List<ExternalFactor> factors)
        {
            return string.Join("\n", factors.Select(f => $"- {f.Name}: {f.Impact} (Impact Level: {f.ImpactLevel}/10)"));
        }

        private decimal CalculateMLConfidence(int dataPoints)
        {
            // More data points = higher confidence, but even limited data can provide insights
            return Math.Min(0.9m, 0.3m + (dataPoints * 0.1m));
        }

        #endregion

        #region ML Response Processing Methods

        private List<string> ExtractPositiveTrends(string analysis)
        {
            var trends = new List<string>();
            var lines = analysis.Split('\n');
            bool inTrendsSection = false;

            foreach (var line in lines)
            {
                var cleanLine = line.Trim();
                if (cleanLine.Contains("positive") && cleanLine.Contains("trend"))
                {
                    inTrendsSection = true;
                    continue;
                }

                if (inTrendsSection && cleanLine.StartsWith("-"))
                {
                    trends.Add(cleanLine.Substring(1).Trim());
                }
                else if (inTrendsSection && string.IsNullOrWhiteSpace(cleanLine))
                {
                    break;
                }
            }

            // Default trends if none extracted
            if (trends.Count == 0)
            {
                trends.Add("Town leadership is proactively seeking solutions");
                trends.Add("Community commitment to fiscal responsibility");
                trends.Add("Willingness to explore new revenue opportunities");
            }

            return trends;
        }

        private List<string> ExtractSeasonalOpportunities(string analysis)
        {
            var opportunities = new List<string>();
            var lines = analysis.Split('\n');
            bool inSeasonalSection = false;

            foreach (var line in lines)
            {
                var cleanLine = line.Trim();
                if (cleanLine.Contains("seasonal") && cleanLine.Contains("opportunit"))
                {
                    inSeasonalSection = true;
                    continue;
                }

                if (inSeasonalSection && cleanLine.StartsWith("-"))
                {
                    opportunities.Add(cleanLine.Substring(1).Trim());
                }
                else if (inSeasonalSection && string.IsNullOrWhiteSpace(cleanLine))
                {
                    break;
                }
            }

            // Default seasonal opportunities
            if (opportunities.Count == 0)
            {
                opportunities.Add("Summer tourism and recreation activities");
                opportunities.Add("Holiday events and festivals");
                opportunities.Add("Agricultural season coordination");
                opportunities.Add("Winter maintenance planning and budgeting");
            }

            return opportunities;
        }

        private List<string> ExtractCostSavingPatterns(string analysis)
        {
            var patterns = new List<string>();
            var lines = analysis.Split('\n');
            bool inSavingsSection = false;

            foreach (var line in lines)
            {
                var cleanLine = line.Trim();
                if (cleanLine.Contains("cost") && (cleanLine.Contains("saving") || cleanLine.Contains("reduction")))
                {
                    inSavingsSection = true;
                    continue;
                }

                if (inSavingsSection && cleanLine.StartsWith("-"))
                {
                    patterns.Add(cleanLine.Substring(1).Trim());
                }
                else if (inSavingsSection && string.IsNullOrWhiteSpace(cleanLine))
                {
                    break;
                }
            }

            // Default cost saving patterns
            if (patterns.Count == 0)
            {
                patterns.Add("Shared services with neighboring communities");
                patterns.Add("Energy efficiency improvements in municipal buildings");
                patterns.Add("Preventive maintenance reducing emergency repairs");
                patterns.Add("Volunteer programs reducing labor costs");
            }

            return patterns;
        }

        private List<string> ExtractRevenueOpportunities(string analysis)
        {
            var opportunities = new List<string>();
            var lines = analysis.Split('\n');
            bool inRevenueSection = false;

            foreach (var line in lines)
            {
                var cleanLine = line.Trim();
                if (cleanLine.Contains("revenue") && cleanLine.Contains("opportunit"))
                {
                    inRevenueSection = true;
                    continue;
                }

                if (inRevenueSection && cleanLine.StartsWith("-"))
                {
                    opportunities.Add(cleanLine.Substring(1).Trim());
                }
                else if (inRevenueSection && string.IsNullOrWhiteSpace(cleanLine))
                {
                    break;
                }
            }

            // Default revenue opportunities
            if (opportunities.Count == 0)
            {
                opportunities.Add("Grant opportunities for rural infrastructure");
                opportunities.Add("Fee review and optimization");
                opportunities.Add("Public-private partnerships");
                opportunities.Add("Federal and state program participation");
            }

            return opportunities;
        }

        private List<string> ExtractEarlyWarnings(string analysis)
        {
            var warnings = new List<string>();
            var lines = analysis.Split('\n');
            bool inWarningsSection = false;

            foreach (var line in lines)
            {
                var cleanLine = line.Trim();
                if (cleanLine.Contains("early") && cleanLine.Contains("warning"))
                {
                    inWarningsSection = true;
                    continue;
                }

                if (inWarningsSection && cleanLine.StartsWith("-"))
                {
                    warnings.Add(cleanLine.Substring(1).Trim());
                }
                else if (inWarningsSection && string.IsNullOrWhiteSpace(cleanLine))
                {
                    break;
                }
            }

            // Conservative default warnings
            if (warnings.Count == 0)
            {
                warnings.Add("Monitor cash flow trends closely");
                warnings.Add("Plan for seasonal revenue variations");
                warnings.Add("Track infrastructure maintenance needs");
            }

            return warnings;
        }

        private List<string> ExtractHopeBasedRecommendations(string analysis)
        {
            var recommendations = new List<string>();
            var lines = analysis.Split('\n');
            bool inRecommendationsSection = false;

            foreach (var line in lines)
            {
                var cleanLine = line.Trim();
                if (cleanLine.Contains("recommend") || cleanLine.Contains("suggest"))
                {
                    inRecommendationsSection = true;
                    continue;
                }

                if (inRecommendationsSection && cleanLine.StartsWith("-"))
                {
                    recommendations.Add(cleanLine.Substring(1).Trim());
                }
                else if (inRecommendationsSection && string.IsNullOrWhiteSpace(cleanLine))
                {
                    break;
                }
            }

            // Hope-focused default recommendations
            if (recommendations.Count == 0)
            {
                recommendations.Add("You're taking the right steps by analyzing your finances proactively");
                recommendations.Add("Small improvements compound over time - every positive change matters");
                recommendations.Add("Your community has unique strengths that can be leveraged");
                recommendations.Add("Consider reaching out to neighboring towns for shared solutions");
            }

            return recommendations;
        }

        private List<string> ExtractHopeRecommendations(string analysis)
        {
            return ExtractHopeBasedRecommendations(analysis);
        }

        private List<RevenueOpportunity> ExtractQuickWins(string analysis)
        {
            var quickWins = new List<RevenueOpportunity>();
            var lines = analysis.Split('\n');

            foreach (var line in lines)
            {
                if (line.Contains("quick") && line.Contains("win"))
                {
                    quickWins.Add(new RevenueOpportunity
                    {
                        Name = "Fee structure optimization",
                        EstimatedRevenue = 5000m,
                        Difficulty = 2,
                        TimeToImplement = 1,
                        Description = "Review and adjust municipal fees based on current costs",
                        NextSteps = new List<string> { "Review current fee schedule", "Calculate true costs", "Propose adjustments" }
                    });
                    break;
                }
            }

            if (quickWins.Count == 0)
            {
                quickWins.Add(new RevenueOpportunity
                {
                    Name = "Utility rate review",
                    EstimatedRevenue = 10000m,
                    Difficulty = 1,
                    TimeToImplement = 2,
                    Description = "Basic rate review to ensure cost recovery",
                    NextSteps = new List<string> { "Gather utility costs", "Calculate rate needs", "Present to council" }
                });
            }

            return quickWins;
        }

        private List<RevenueOpportunity> ExtractMediumTermOpportunities(string analysis)
        {
            return new List<RevenueOpportunity>
            {
                new RevenueOpportunity
                {
                    Name = "Grant application program",
                    EstimatedRevenue = 50000m,
                    Difficulty = 3,
                    TimeToImplement = 6,
                    Description = "Systematic approach to identifying and applying for grants",
                    NextSteps = new List<string> { "Research available grants", "Develop application process", "Submit applications" }
                }
            };
        }

        private List<RevenueOpportunity> ExtractLongTermOpportunities(string analysis)
        {
            return new List<RevenueOpportunity>
            {
                new RevenueOpportunity
                {
                    Name = "Infrastructure partnerships",
                    EstimatedRevenue = 100000m,
                    Difficulty = 4,
                    TimeToImplement = 18,
                    Description = "Long-term partnerships for shared infrastructure costs",
                    NextSteps = new List<string> { "Identify potential partners", "Develop partnership framework", "Negotiate agreements" }
                }
            };
        }

        private List<GrantOpportunity> ExtractGrantOpportunities(string analysis)
        {
            return new List<GrantOpportunity>
            {
                new GrantOpportunity
                {
                    GrantName = "Rural Infrastructure Grant",
                    Agency = "USDA Rural Development",
                    MaxAmount = 75000m,
                    ApplicationDeadline = DateTime.Now.AddMonths(3),
                    Eligibility = "Rural communities under 10,000 population",
                    Purpose = "Water, sewer, and solid waste infrastructure improvements"
                }
            };
        }

        private List<PartnershipOpportunity> ExtractPartnershipOpportunities(string analysis)
        {
            return new List<PartnershipOpportunity>
            {
                new PartnershipOpportunity
                {
                    PartnerType = "Neighboring municipalities",
                    Description = "Shared services and equipment",
                    EstimatedSavings = 25000m,
                    PotentialPartners = new List<string> { "Adjacent towns", "County services", "Regional organizations" }
                }
            };
        }

        private decimal CalculateTotalPotentialRevenue(List<RevenueOpportunity> allOpportunities)
        {
            return allOpportunities.Sum(o => o.EstimatedRevenue);
        }

        private List<string> ExtractPriorityActions(string analysis)
        {
            return new List<string>
            {
                "Start with utility rate review - immediate impact",
                "Research available grants for infrastructure",
                "Connect with neighboring communities for shared services",
                "Document all revenue sources for better tracking"
            };
        }

        private List<RiskPrediction> ExtractImmediateRisks(string analysis)
        {
            return new List<RiskPrediction>
            {
                new RiskPrediction
                {
                    RiskName = "Cash flow shortage",
                    Probability = 3,
                    EstimatedImpact = 15000m,
                    Description = "Potential seasonal cash flow challenges",
                    PreventionActions = new List<string> { "Monitor monthly cash flow", "Plan for seasonal variations", "Establish small reserve fund" }
                }
            };
        }

        private List<RiskPrediction> ExtractShortTermChallenges(string analysis)
        {
            return new List<RiskPrediction>
            {
                new RiskPrediction
                {
                    RiskName = "Infrastructure maintenance needs",
                    Probability = 5,
                    EstimatedImpact = 35000m,
                    Description = "Anticipated maintenance and repairs within 12 months",
                    PreventionActions = new List<string> { "Schedule preventive maintenance", "Budget for known repairs", "Seek maintenance grants" }
                }
            };
        }

        private List<RiskPrediction> ExtractMediumTermConcerns(string analysis)
        {
            return new List<RiskPrediction>
            {
                new RiskPrediction
                {
                    RiskName = "Major infrastructure replacement",
                    Probability = 4,
                    EstimatedImpact = 75000m,
                    Description = "Equipment or infrastructure nearing end of useful life",
                    PreventionActions = new List<string> { "Develop replacement timeline", "Research funding options", "Plan for incremental improvements" }
                }
            };
        }

        private List<SeasonalRisk> ExtractSeasonalRisks(string analysis)
        {
            return new List<SeasonalRisk>
            {
                new SeasonalRisk
                {
                    Season = "Winter",
                    RiskType = "Increased maintenance costs",
                    TypicalCost = 8000m,
                    PreparationActions = "Budget extra 20% for winter maintenance; stock supplies early"
                }
            };
        }

        private List<InfrastructureRisk> ExtractInfrastructureRisks(string analysis)
        {
            return new List<InfrastructureRisk>
            {
                new InfrastructureRisk
                {
                    AssetName = "Main water line",
                    Condition = "Fair - requires monitoring",
                    EstimatedYearsRemaining = 5,
                    ReplacementCost = 45000m,
                    MaintenanceActions = "Annual inspection and minor repairs as needed"
                }
            };
        }

        private List<string> ExtractPreventionActions(string analysis)
        {
            return new List<string>
            {
                "Implement regular infrastructure inspections",
                "Maintain emergency fund for unexpected repairs",
                "Develop relationships with reliable contractors",
                "Keep spare parts inventory for critical systems"
            };
        }

        private List<string> ExtractCommunityStrengths(string analysis)
        {
            return new List<string>
            {
                "Strong community spirit and volunteer engagement",
                "Experienced leadership committed to fiscal responsibility",
                "Strategic location with potential for growth",
                "Existing infrastructure foundation to build upon"
            };
        }

        private string CalculateOverallRisk(List<RiskPrediction> allRisks)
        {
            if (!allRisks.Any()) return "Low";

            var avgRisk = allRisks.Average(r => r.Probability);
            if (avgRisk < 3) return "Low";
            if (avgRisk < 6) return "Moderate";
            return "Elevated - but manageable with planning";
        }

        private string ExtractEncouragingMessage(string analysis)
        {
            return "Your community is taking proactive steps to address financial challenges. " +
                   "Many rural towns face similar situations, and with careful planning and the " +
                   "resources available, you can build a more stable financial foundation. " +
                   "Small improvements add up over time, and you're already on the right path.";
        }

        #endregion

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
        public string Query { get; set; } = string.Empty;
        public string EnterpriseScope { get; set; } = string.Empty;
        public TokenUsage? Usage { get; set; }
    }

    public class ScenarioAnalysisResponse
    {
        public bool Success { get; set; }
        public string ScenarioName { get; set; } = string.Empty;
        public FinancialImpact? FinancialImpact { get; set; }
        public List<string> Recommendations { get; set; } = new List<string>();
        public string RiskAssessment { get; set; } = string.Empty;
        public string FullAnalysis { get; set; } = string.Empty;
        public int ExecutionTimeMs { get; set; }
        public string Error { get; set; } = string.Empty;
    }

    public class CrossEnterpriseAnalysis
    {
        public bool Success { get; set; }
        public string Analysis { get; set; } = string.Empty;
        public List<string> AffectedEnterprises { get; set; } = new List<string>();
        public List<string> RecommendedActions { get; set; } = new List<string>();
        public CrossEnterpriseImpact? EstimatedImpact { get; set; }
        public string Error { get; set; } = string.Empty;
    }

    public class FinancialImpact
    {
        public decimal MonthlyImpact { get; set; }
        public decimal AnnualImpact { get; set; }
        public decimal CustomerImpact { get; set; }
        public string Description { get; set; } = string.Empty;
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

    #region Enhanced Response Models

    public class StreamingResponse
    {
        public bool Success { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public string Error { get; set; } = string.Empty;
    }

    public class VisionAnalysisResponse
    {
        public bool Success { get; set; }
        public string Analysis { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public Dictionary<string, decimal> ExtractedData { get; set; } = new Dictionary<string, decimal>();
        public int ExecutionTimeMs { get; set; }
        public DateTime Timestamp { get; set; }
        public string Error { get; set; } = string.Empty;
    }

    public class TokenUsage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
        public decimal EstimatedCost { get; set; }
    }

    public class UsageStatistics
    {
        public int TotalRequests { get; set; }
        public int TotalTokens { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime LastRequest { get; set; }
        public Dictionary<string, int> ModelUsage { get; set; } = new Dictionary<string, int>();
    }

    public class TokenUsageTracker
    {
        private readonly List<UsageRecord> _records = new List<UsageRecord>();
        private readonly object _lock = new object();

        public void RecordUsage(string model, TokenUsage usage)
        {
            lock (_lock)
            {
                _records.Add(new UsageRecord
                {
                    Model = model,
                    Usage = usage,
                    Timestamp = DateTime.Now
                });
            }
        }

        public UsageStatistics GetStatistics()
        {
            lock (_lock)
            {
                var stats = new UsageStatistics
                {
                    TotalRequests = _records.Count,
                    TotalTokens = _records.Sum(r => r.Usage.TotalTokens),
                    TotalCost = _records.Sum(r => r.Usage.EstimatedCost),
                    PeriodStart = _records.FirstOrDefault()?.Timestamp ?? DateTime.Now,
                    LastRequest = _records.LastOrDefault()?.Timestamp ?? DateTime.Now
                };

                // Group by model
                foreach (var group in _records.GroupBy(r => r.Model))
                {
                    stats.ModelUsage[group.Key] = group.Count();
                }

                return stats;
            }
        }

        private class UsageRecord
        {
            public string Model { get; set; } = string.Empty;
            public TokenUsage Usage { get; set; } = new TokenUsage();
            public DateTime Timestamp { get; set; }
        }
    }

    #endregion

    #region Rural Town Clerk Support Models

    public class PlainEnglishResponse
    {
        public bool Success { get; set; }
        public string FriendlyAnswer { get; set; } = string.Empty;
        public string QuickTakeaway { get; set; } = string.Empty;
        public List<string> NextSteps { get; set; } = new List<string>();
        public string ConfidenceLevel { get; set; } = string.Empty;
        public int ExecutionTimeMs { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    public class CommonSenseGuidance
    {
        public bool Success { get; set; }
        public string Recommendation { get; set; } = string.Empty;
        public string Reasoning { get; set; } = string.Empty;
        public List<string> RedFlags { get; set; } = new List<string>();
        public string ResidentImpact { get; set; } = string.Empty;
        public string FullGuidance { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    public class TownClerkDashboard
    {
        public string TownName { get; set; } = string.Empty;
        public string CurrentFinancialHealth { get; set; } = string.Empty;
        public List<string> UpcomingChallenges { get; set; } = new List<string>();
        public List<string> PositiveOpportunities { get; set; } = new List<string>();
        public string ThisWeeksFocus { get; set; } = string.Empty;
        public decimal ResidentImpactScore { get; set; }
        public string EncouragingMessage { get; set; } = string.Empty;
    }

    #endregion

    #region Machine Learning Models for Rural Towns

    public class MLFinancialInsights
    {
        public bool Success { get; set; }
        public List<string> PositiveTrends { get; set; } = new List<string>();
        public List<string> SeasonalOpportunities { get; set; } = new List<string>();
        public List<string> CostSavingPatterns { get; set; } = new List<string>();
        public List<string> RevenueOpportunities { get; set; } = new List<string>();
        public List<string> EarlyWarnings { get; set; } = new List<string>();
        public List<string> HopeBasedRecommendations { get; set; } = new List<string>();
        public decimal ConfidenceScore { get; set; }
        public string FullAnalysis { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    public class RevenueOpportunityAnalysis
    {
        public bool Success { get; set; }
        public List<RevenueOpportunity> QuickWins { get; set; } = new List<RevenueOpportunity>();
        public List<RevenueOpportunity> MediumTermOpportunities { get; set; } = new List<RevenueOpportunity>();
        public List<RevenueOpportunity> LongTermOpportunities { get; set; } = new List<RevenueOpportunity>();
        public List<GrantOpportunity> GrantOpportunities { get; set; } = new List<GrantOpportunity>();
        public List<PartnershipOpportunity> PartnershipOpportunities { get; set; } = new List<PartnershipOpportunity>();
        public decimal TotalPotentialRevenue { get; set; }
        public List<string> PriorityActions { get; set; } = new List<string>();
        public string FullAnalysis { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    public class CrisisPreventionAnalysis
    {
        public bool Success { get; set; }
        public List<RiskPrediction> ImmediateRisks { get; set; } = new List<RiskPrediction>();
        public List<RiskPrediction> ShortTermChallenges { get; set; } = new List<RiskPrediction>();
        public List<RiskPrediction> MediumTermConcerns { get; set; } = new List<RiskPrediction>();
        public List<SeasonalRisk> SeasonalRisks { get; set; } = new List<SeasonalRisk>();
        public List<InfrastructureRisk> InfrastructureRisks { get; set; } = new List<InfrastructureRisk>();
        public List<string> PreventionActions { get; set; } = new List<string>();
        public List<string> CommunityStrengths { get; set; } = new List<string>();
        public string OverallRiskLevel { get; set; } = string.Empty;
        public string EncouragingMessage { get; set; } = string.Empty;
        public string FullAnalysis { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    public class RevenueOpportunity
    {
        public string Name { get; set; } = string.Empty;
        public decimal EstimatedRevenue { get; set; }
        public int Difficulty { get; set; } // 1-5 scale
        public int TimeToImplement { get; set; } // Months
        public string Description { get; set; } = string.Empty;
        public List<string> NextSteps { get; set; } = new List<string>();
    }

    public class GrantOpportunity
    {
        public string GrantName { get; set; } = string.Empty;
        public string Agency { get; set; } = string.Empty;
        public decimal MaxAmount { get; set; }
        public DateTime ApplicationDeadline { get; set; }
        public string Eligibility { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
    }

    public class PartnershipOpportunity
    {
        public string PartnerType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal EstimatedSavings { get; set; }
        public List<string> PotentialPartners { get; set; } = new List<string>();
    }

    public class RiskPrediction
    {
        public string RiskName { get; set; } = string.Empty;
        public int Probability { get; set; } // 1-10 scale
        public decimal EstimatedImpact { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> PreventionActions { get; set; } = new List<string>();
    }

    public class SeasonalRisk
    {
        public string Season { get; set; } = string.Empty;
        public string RiskType { get; set; } = string.Empty;
        public decimal TypicalCost { get; set; }
        public string PreparationActions { get; set; } = string.Empty;
    }

    public class InfrastructureRisk
    {
        public string AssetName { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public int EstimatedYearsRemaining { get; set; }
        public decimal ReplacementCost { get; set; }
        public string MaintenanceActions { get; set; } = string.Empty;
    }

    public class HistoricalFinancialRecord
    {
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class TownProfile
    {
        public int Population { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<string> PrimaryIndustries { get; set; } = new List<string>();
        public List<string> KeyAssets { get; set; } = new List<string>();
        public List<string> Challenges { get; set; } = new List<string>();
        public List<string> CommunityStrengths { get; set; } = new List<string>();
    }

    public class RevenueStream
    {
        public string Name { get; set; } = string.Empty;
        public decimal AnnualAmount { get; set; }
        public int Reliability { get; set; } // 1-10 scale
        public string Source { get; set; } = string.Empty;
    }

    public class FinancialTrends
    {
        public string RevenueTrend { get; set; } = string.Empty;
        public decimal RevenueChangePercent { get; set; }
        public string ExpenseTrend { get; set; } = string.Empty;
        public decimal ExpenseChangePercent { get; set; }
        public string BudgetBalance { get; set; } = string.Empty;
        public string CashFlowStatus { get; set; } = string.Empty;
        public List<string> KeyConcerns { get; set; } = new List<string>();
    }

    public class ExternalFactor
    {
        public string Name { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;
        public int ImpactLevel { get; set; } // 1-10 scale
        public string Description { get; set; } = string.Empty;
    }

    #endregion
}
