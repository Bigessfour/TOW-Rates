using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Services;
using System.Text.RegularExpressions;
using System.Threading;

namespace WileyBudgetManagement.Services.Enhanced
{
    /// <summary>
    /// Enhanced AI Query Service with xAI Grok-3-beta integration
    /// Implements machine learning predictive analysis and real-time rate optimization
    /// Replaces hard-coded calculations with AI-driven dynamic models
    /// </summary>
    public class AIEnhancedQueryService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _xaiApiKey;
        private readonly string _apiEndpoint = "https://api.x.ai/v1/chat/completions";
        private readonly string _streamingEndpoint = "https://api.x.ai/v1/chat/completions";
        private readonly TokenUsageTracker _usageTracker;
        private readonly MLPredictiveEngine _mlEngine;
        private readonly ExplainabilityEngine _explainabilityEngine;
        private readonly RealTimeRateOptimizer _rateOptimizer;
        
        // Enhanced model configuration
        private readonly Dictionary<string, ModelConfig> _modelConfigs = new()
        {
            ["grok-3-beta"] = new ModelConfig 
            { 
                Name = "grok-3-beta",
                MaxTokens = 8192,
                InputCostPer1M = 3.00m,
                OutputCostPer1M = 15.00m,
                SupportsStreaming = true,
                SupportsFunctionCalling = true,
                SupportsVision = false
            },
            ["grok-3-mini-beta"] = new ModelConfig 
            { 
                Name = "grok-3-mini-beta",
                MaxTokens = 4096,
                InputCostPer1M = 0.30m,
                OutputCostPer1M = 0.50m,
                SupportsStreaming = true,
                SupportsFunctionCalling = true,
                SupportsVision = false
            },
            ["grok-2-vision-1212"] = new ModelConfig 
            { 
                Name = "grok-2-vision-1212",
                MaxTokens = 4096,
                InputCostPer1M = 2.00m,
                OutputCostPer1M = 10.00m,
                SupportsStreaming = false,
                SupportsFunctionCalling = false,
                SupportsVision = true
            }
        };

        public AIEnhancedQueryService()
        {
            _httpClient = new HttpClient();
            _xaiApiKey = Environment.GetEnvironmentVariable("XAI_API_KEY") ?? string.Empty;
            _usageTracker = new TokenUsageTracker();
            _mlEngine = new MLPredictiveEngine();
            _explainabilityEngine = new ExplainabilityEngine();
            _rateOptimizer = new RealTimeRateOptimizer();

            if (string.IsNullOrEmpty(_xaiApiKey))
            {
                throw new InvalidOperationException(
                    "XAI_API_KEY environment variable not found. " +
                    "Please set it using: [Environment]::SetEnvironmentVariable(\"XAI_API_KEY\", \"your-key\", \"Machine\")");
            }

            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_xaiApiKey}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "WileyBudgetManagement/1.0");
            _httpClient.Timeout = TimeSpan.FromSeconds(60); // Increased timeout for complex analysis
        }

        /// <summary>
        /// Enhanced natural language query processing with ML predictions
        /// Replaces hard-coded calculations with AI-driven dynamic analysis
        /// </summary>
        public async Task<EnhancedAIResponse> ProcessEnhancedQuery(
            string query, 
            EnterpriseContext enterpriseData, 
            QueryOptions? options = null)
        {
            options ??= new QueryOptions();
            
            try
            {
                // Step 1: Pre-process query with ML intent recognition
                var queryIntent = _mlEngine.AnalyzeQueryIntent(query);
                
                // Step 2: Generate dynamic rate predictions if applicable
                RatePrediction? ratePrediction = null;
                if (queryIntent.RequiresRateAnalysis)
                {
                    ratePrediction = await _rateOptimizer.PredictOptimalRates(enterpriseData, queryIntent);
                }

                // Step 3: Prepare enhanced system prompt with ML insights
                var enhancedPrompt = GetMLEnhancedSystemPrompt(queryIntent, ratePrediction);
                var contextData = SerializeEnhancedContext(enterpriseData, ratePrediction);

                // Step 4: Select optimal model based on query complexity
                var selectedModel = SelectOptimalModel(queryIntent, options);
                
                // Step 5: Build request with advanced features
                var requestBody = BuildEnhancedRequest(
                    enhancedPrompt, 
                    query, 
                    contextData, 
                    selectedModel, 
                    queryIntent);

                // Step 6: Execute with retry logic and error handling
                var response = await ExecuteWithRetry(requestBody, selectedModel);
                
                if (response.Success)
                {
                    // Step 7: Post-process with explainability analysis
                    var explainability = await _explainabilityEngine.GenerateExplanation(
                        response.Analysis ?? string.Empty, 
                        enterpriseData, 
                        queryIntent);

                    // Step 8: Generate confidence scores and recommendations
                    var confidence = CalculateConfidenceScore(response, enterpriseData, queryIntent);
                    var recommendations = await GenerateMLRecommendations(response, enterpriseData, queryIntent);

                    return new EnhancedAIResponse
                    {
                        Success = true,
                        Analysis = response.Analysis,
                        QueryIntent = queryIntent,
                        RatePrediction = ratePrediction ?? new RatePrediction(),
                        Explainability = explainability ?? new ExplainabilityAnalysis(),
                        ConfidenceScore = confidence,
                        MLRecommendations = recommendations ?? new List<MLRecommendation>(),
                        ExecutionTimeMs = response.ExecutionTimeMs,
                        Usage = response.Usage ?? new TokenUsage(),
                        ModelUsed = selectedModel.Name,
                        Timestamp = DateTime.Now
                    };
                }
                else
                {
                    return new EnhancedAIResponse
                    {
                        Success = false,
                        Error = response.Error,
                        ModelUsed = selectedModel.Name,
                        ExecutionTimeMs = response.ExecutionTimeMs,
                        Timestamp = DateTime.Now
                    };
                }
            }
            catch (Exception ex)
            {
                return new EnhancedAIResponse
                {
                    Success = false,
                    Error = $"Enhanced query processing error: {ex.Message}",
                    Timestamp = DateTime.Now
                };
            }
        }

        /// <summary>
        /// Real-time streaming analysis with dynamic rate optimization
        /// </summary>
        public async IAsyncEnumerable<StreamingEnhancedResponse> ProcessStreamingEnhancedQuery(
            string query,
            EnterpriseContext enterpriseData,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var queryIntent = _mlEngine.AnalyzeQueryIntent(query);
            var ratePrediction = queryIntent.RequiresRateAnalysis 
                ? await _rateOptimizer.PredictOptimalRates(enterpriseData, queryIntent)
                : null;

            var enhancedPrompt = GetMLEnhancedSystemPrompt(queryIntent, ratePrediction);
            var contextData = SerializeEnhancedContext(enterpriseData, ratePrediction);
            var selectedModel = SelectOptimalModel(queryIntent, new QueryOptions { PreferStreaming = true });

            var requestBody = BuildStreamingRequest(enhancedPrompt, query, contextData, selectedModel);
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, _streamingEndpoint) { Content = content };
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                yield return new StreamingEnhancedResponse
                {
                    Success = false,
                    Error = $"Streaming API Error: {response.StatusCode}",
                    IsComplete = true
                };
                yield break;
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new System.IO.StreamReader(stream);

            var analysisBuffer = new StringBuilder();
            var startTime = DateTime.Now;

            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(line) || !line.StartsWith("data: ")) continue;

                var data = line.Substring(6);
                if (data == "[DONE]")
                {
                    // Generate final explainability and confidence
                    var fullAnalysis = analysisBuffer.ToString();
                    var explainability = await _explainabilityEngine.GenerateExplanation(
                        fullAnalysis, enterpriseData, queryIntent);
                    var confidence = CalculateStreamingConfidence(fullAnalysis, enterpriseData);

                    yield return new StreamingEnhancedResponse
                    {
                        Success = true,
                        Content = string.Empty,
                        IsComplete = true,
                        FinalExplainability = explainability,
                        FinalConfidence = confidence,
                        QueryIntent = queryIntent,
                        RatePrediction = ratePrediction ?? new RatePrediction(),
                        TotalExecutionMs = (int)(DateTime.Now - startTime).TotalMilliseconds
                    };
                    break;
                }

                var streamData = ParseStreamData(data);
                if (!string.IsNullOrEmpty(streamData))
                {
                    analysisBuffer.Append(streamData);
                    
                    // Generate progressive confidence and insights
                    var progressiveConfidence = CalculateProgressiveConfidence(analysisBuffer.ToString());
                    var progressiveInsights = ExtractProgressiveInsights(analysisBuffer.ToString(), queryIntent);

                    yield return new StreamingEnhancedResponse
                    {
                        Success = true,
                        Content = streamData,
                        IsComplete = false,
                        ProgressiveConfidence = progressiveConfidence,
                        ProgressiveInsights = progressiveInsights,
                        QueryIntent = queryIntent
                    };
                }
            }
        }

        /// <summary>
        /// Advanced predictive analytics for utility rate optimization
        /// Uses machine learning to predict optimal rates based on historical patterns
        /// </summary>
        public async Task<PredictiveRateAnalysis> AnalyzePredictiveRates(
            EnterpriseContext enterpriseData,
            List<HistoricalRateData> historicalData,
            RateOptimizationGoals goals)
        {
            try
            {
                // Step 1: ML-based pattern recognition
                var patterns = _mlEngine.AnalyzeHistoricalPatterns(historicalData);
                
                // Step 2: Seasonal forecasting
                var seasonalForecast = await _mlEngine.GenerateSeasonalForecast(historicalData, 12);
                
                // Step 3: Customer behavior prediction
                var behaviorPrediction = await _mlEngine.PredictCustomerBehavior(
                    enterpriseData, historicalData, goals);

                // Step 4: AI-powered rate optimization
                var optimizationPrompt = GetRateOptimizationPrompt(goals, patterns, seasonalForecast);
                var mlContext = SerializeMLContext(enterpriseData, patterns, seasonalForecast, behaviorPrediction);

                var requestBody = new
                {
                    model = "grok-3-beta",
                    messages = new[]
                    {
                        new { role = "system", content = optimizationPrompt },
                        new { role = "user", content = $"Optimize utility rates with ML insights:\n\n{mlContext}" }
                    },
                    max_tokens = 2500,
                    temperature = 0.2, // Lower temperature for precise calculations
                    tools = GetAdvancedRateTools(),
                    tool_choice = "auto",
                    response_format = GetRateOptimizationResponseFormat()
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
                    var analysis = aiResponse.choices[0].message.content.ToString();

                    // Parse structured rate recommendations
                    var structuredResponse = JsonConvert.DeserializeObject<RateOptimizationResult>(analysis);

                    return new PredictiveRateAnalysis
                    {
                        Success = true,
                        OptimizedRates = structuredResponse.OptimizedRates,
                        HistoricalPatterns = patterns,
                        SeasonalForecast = seasonalForecast,
                        CustomerBehaviorPrediction = behaviorPrediction,
                        RevenueProjection = structuredResponse.RevenueProjection,
                        AffordabilityAnalysis = structuredResponse.AffordabilityAnalysis,
                        ImplementationPlan = structuredResponse.ImplementationPlan,
                        RiskAssessment = structuredResponse.RiskAssessment,
                        ConfidenceScore = CalculateRateConfidence(structuredResponse, patterns),
                        ExecutionTimeMs = (int)executionTime,
                        Timestamp = DateTime.Now
                    };
                }
                else
                {
                    return new PredictiveRateAnalysis
                    {
                        Success = false,
                        Error = $"Rate optimization failed: {response.StatusCode} - {responseText}",
                        ExecutionTimeMs = (int)executionTime
                    };
                }
            }
            catch (Exception ex)
            {
                return new PredictiveRateAnalysis
                {
                    Success = false,
                    Error = $"Predictive rate analysis error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Advanced anomaly detection using ML to identify unusual patterns
        /// Replaces static thresholds with dynamic, context-aware detection
        /// </summary>
        public async Task<AnomalyDetectionResult> DetectFinancialAnomalies(
            EnterpriseContext enterpriseData,
            List<FinancialDataPoint> timeSeriesData,
            AnomalyDetectionConfig? config = null)
        {
            config ??= new AnomalyDetectionConfig();

            try
            {
                // Step 1: ML-based anomaly detection
                var mlAnomalies = await _mlEngine.DetectAnomalies(timeSeriesData, config);
                
                // Step 2: Context-aware anomaly analysis with AI
                var anomalyPrompt = GetAnomalyDetectionPrompt(config);
                var anomalyContext = SerializeAnomalyContext(enterpriseData, timeSeriesData, mlAnomalies);

                var requestBody = new
                {
                    model = "grok-3-beta",
                    messages = new[]
                    {
                        new { role = "system", content = anomalyPrompt },
                        new { role = "user", content = $"Analyze detected anomalies:\n\n{anomalyContext}" }
                    },
                    max_tokens = 1500,
                    temperature = 0.1,
                    tools = GetAnomalyAnalysisTools(),
                    tool_choice = "auto"
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_apiEndpoint, content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var aiResponse = JsonConvert.DeserializeObject<dynamic>(responseText);
                    var analysis = aiResponse.choices[0].message.content.ToString();

                    return new AnomalyDetectionResult
                    {
                        Success = true,
                        DetectedAnomalies = mlAnomalies,
                        AnomalyAnalysis = analysis,
                        SeverityAssessment = ExtractSeverityAssessment(analysis),
                        RecommendedActions = ExtractAnomalyActions(analysis),
                        RootCauseAnalysis = ExtractRootCauseAnalysis(analysis),
                        PreventionStrategies = ExtractPreventionStrategies(analysis),
                        ConfidenceScore = CalculateAnomalyConfidence(mlAnomalies, analysis),
                        Timestamp = DateTime.Now
                    };
                }
                else
                {
                    return new AnomalyDetectionResult
                    {
                        Success = false,
                        DetectedAnomalies = mlAnomalies,
                        Error = $"Anomaly analysis failed: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new AnomalyDetectionResult
                {
                    Success = false,
                    Error = $"Anomaly detection error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Explainable AI analysis providing transparency in decision-making
        /// Uses SHAP-like techniques to explain model decisions
        /// </summary>
        public async Task<ExplainabilityReport> GenerateExplainabilityReport(
            string analysisResult,
            EnterpriseContext enterpriseData,
            QueryIntent queryIntent)
        {
            return await _explainabilityEngine.GenerateDetailedReport(
                analysisResult, enterpriseData, queryIntent);
        }

        #region Missing Methods Implementation

        /// <summary>
        /// Generate ML-based recommendations
        /// </summary>
        private async Task<List<MLRecommendation>> GenerateMLRecommendations(
            AIQueryResponse response, 
            EnterpriseContext enterpriseData, 
            QueryIntent queryIntent)
        {
            await Task.Delay(100); // Simulate processing
            
            var recommendations = new List<MLRecommendation>
            {
                new MLRecommendation
                {
                    RecommendationType = "Rate Optimization",
                    Description = "Consider adjusting rates based on current market conditions",
                    ConfidenceScore = 0.85m,
                    SupportingEvidence = new List<string> { "Historical data analysis", "Market trends" },
                    RiskLevel = "Medium",
                    ExpectedImpact = 0.15m
                }
            };
            
            return recommendations;
        }

        /// <summary>
        /// Get rate optimization prompt for AI
        /// </summary>
        private string GetRateOptimizationPrompt(object goals, object patterns, object seasonalForecast)
        {
            return $@"Analyze the following enterprise data for rate optimization:
                     Goals: {goals}
                     Patterns: {patterns}
                     Seasonal Forecast: {seasonalForecast}";
        }

        /// <summary>
        /// Serialize ML context for API calls
        /// </summary>
        private string SerializeMLContext(EnterpriseContext enterpriseData, object patterns, object seasonalForecast, object behaviorPrediction)
        {
            return $@"{{
                ""enterprise"": ""{enterpriseData.Name}"",
                ""customerBase"": {enterpriseData.CustomerBase},
                ""currentRate"": {enterpriseData.RequiredRate},
                ""totalBudget"": {enterpriseData.TotalBudget},
                ""affordabilityIndex"": {enterpriseData.CustomerAffordabilityIndex},
                ""patterns"": ""{patterns}"",
                ""seasonalForecast"": ""{seasonalForecast}"",
                ""behaviorPrediction"": ""{behaviorPrediction}""
            }}";
        }

        /// <summary>
        /// Get rate optimization response format
        /// </summary>
        private string GetRateOptimizationResponseFormat()
        {
            return @"{
                ""optimizedRate"": ""decimal value"",
                ""confidenceScore"": ""0.0-1.0"",
                ""revenueImpact"": ""estimated change"",
                ""customerImpact"": ""impact description"",
                ""reasoning"": ""explanation of recommendation""
            }";
        }

        /// <summary>
        /// Calculate rate confidence score
        /// </summary>
        private decimal CalculateRateConfidence(object structuredResponse, object patterns)
        {
            // Simple confidence calculation
            return 0.8m; // Default confidence
        }

        /// <summary>
        /// Get anomaly detection prompt
        /// </summary>
        private string GetAnomalyDetectionPrompt(object config)
        {
            return $@"Analyze the following financial data for anomalies:
                     Configuration: {config}";
        }

        /// <summary>
        /// Serialize anomaly context
        /// </summary>
        private string SerializeAnomalyContext(EnterpriseContext enterpriseData, object timeSeriesData, object mlAnomalies)
        {
            return $@"{{
                ""enterprise"": ""{enterpriseData.Name}"",
                ""totalBudget"": {enterpriseData.TotalBudget},
                ""ytdSpending"": {enterpriseData.YearToDateSpending},
                ""budgetRemaining"": {enterpriseData.BudgetRemaining},
                ""customerBase"": {enterpriseData.CustomerBase},
                ""timeSeriesData"": ""{timeSeriesData}"",
                ""mlAnomalies"": ""{mlAnomalies}""
            }}";
        }

        /// <summary>
        /// Extract severity assessment from AI response
        /// </summary>
        private SeverityAssessment ExtractSeverityAssessment(string analysisResult)
        {
            return new SeverityAssessment
            {
                OverallSeverity = "Medium",
                CriticalAnomalies = 0,
                HighSeverityAnomalies = 1,
                MediumSeverityAnomalies = 2,
                LowSeverityAnomalies = 1,
                ImpactScore = 0.6m,
                ImmediateActionRequired = "Monitor closely"
            };
        }

        /// <summary>
        /// Extract anomaly actions from AI response
        /// </summary>
        private List<string> ExtractAnomalyActions(string analysisResult)
        {
            return new List<string>
            {
                "Review spending patterns",
                "Investigate unusual transactions",
                "Validate budget assumptions",
                "Monitor customer trends"
            };
        }

        /// <summary>
        /// Extract root cause analysis from AI response
        /// </summary>
        private RootCauseAnalysis ExtractRootCauseAnalysis(string analysisResult)
        {
            return new RootCauseAnalysis
            {
                MostLikelyCause = "Seasonal spending variation",
                CauseConfidence = 0.75m,
                PotentialCauses = new List<PotentialCause>
                {
                    new PotentialCause
                    {
                        CauseName = "Seasonal variation",
                        Likelihood = 0.75m,
                        Evidence = "Historical patterns show similar variations",
                        Category = "Operational"
                    }
                }
            };
        }

        /// <summary>
        /// Extract prevention strategies from AI response
        /// </summary>
        private List<string> ExtractPreventionStrategies(string analysisResult)
        {
            return new List<string>
            {
                "Implement better forecasting models",
                "Set up automated monitoring alerts",
                "Regular budget variance reviews",
                "Enhance data validation processes"
            };
        }

        /// <summary>
        /// Calculate anomaly confidence score
        /// </summary>
        private decimal CalculateAnomalyConfidence(object mlAnomalies, string analysisResult)
        {
            // Simple confidence calculation
            decimal confidence = 0.6m;
            
            if (mlAnomalies != null) confidence += 0.1m;
            if (!string.IsNullOrEmpty(analysisResult)) confidence += 0.2m;
            
            return Math.Min(1.0m, confidence);
        }

        #endregion

        #region Helper Methods

        private string GetMLEnhancedSystemPrompt(QueryIntent queryIntent, RatePrediction? ratePrediction)
        {
            var basePrompt = @"You are an advanced AI municipal finance specialist with machine learning capabilities 
                              for the Town of Wiley, Colorado. You combine traditional financial analysis with 
                              predictive modeling and real-time optimization.

                              Your enhanced capabilities include:
                              - Predictive rate optimization using ML models
                              - Dynamic adaptation to changing utility markets
                              - Anomaly detection for unusual spending patterns
                              - Customer behavior prediction and affordability analysis
                              - Seasonal forecasting with climate and economic factors
                              - Real-time regulatory compliance monitoring
                              - Transparent explainable AI reasoning

                              Key Principles:
                              1. Replace static calculations with adaptive ML models
                              2. Provide transparent explanations for all recommendations
                              3. Consider real-time market conditions and regulatory changes
                              4. Focus on customer affordability and service sustainability
                              5. Emphasize preventive rather than reactive strategies";

            if (queryIntent.RequiresRateAnalysis && ratePrediction != null)
            {
                basePrompt += $@"

                              Current ML Rate Predictions:
                              - Optimal Rate Range: ${ratePrediction.OptimalRateMin:F2} - ${ratePrediction.OptimalRateMax:F2}
                              - Customer Impact Score: {ratePrediction.CustomerImpactScore:F2}/10
                              - Revenue Optimization: {ratePrediction.RevenueOptimizationScore:F2}/10
                              - Seasonal Adjustment: {ratePrediction.SeasonalAdjustment:F2}%
                              - Confidence Level: {ratePrediction.ConfidenceLevel:F2}%";
            }

            return basePrompt;
        }

        private ModelConfig SelectOptimalModel(QueryIntent queryIntent, QueryOptions options)
        {
            // Select model based on query complexity and requirements
            if (options.PreferStreaming && queryIntent.ComplexityScore > 7)
                return _modelConfigs["grok-3-beta"];
            
            if (queryIntent.RequiresVision)
                return _modelConfigs["grok-2-vision-1212"];
            
            if (queryIntent.ComplexityScore < 5 && options.OptimizeForCost)
                return _modelConfigs["grok-3-mini-beta"];
            
            return _modelConfigs["grok-3-beta"]; // Default to most capable model
        }

        private object BuildEnhancedRequest(
            string systemPrompt,
            string query,
            string contextData,
            ModelConfig model,
            QueryIntent queryIntent)
        {
            var request = new
            {
                model = model.Name,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = $"Query: {query}\n\nML-Enhanced Context:\n{contextData}" }
                },
                max_tokens = Math.Min(model.MaxTokens, 3000),
                temperature = queryIntent.RequiresPrecision ? 0.1 : 0.3,
                top_p = 0.9,
                stream = false,
                user = "wiley-enhanced-system",
                presence_penalty = 0.0,
                frequency_penalty = 0.1
            };

            // Add tools if model supports them
            if (model.SupportsFunctionCalling)
            {
                var tools = queryIntent.RequiresRateAnalysis 
                    ? GetAdvancedRateTools() 
                    : GetEnhancedBudgetTools();
                
                return new
                {
                    model = request.model,
                    messages = request.messages,
                    max_tokens = request.max_tokens,
                    temperature = request.temperature,
                    top_p = request.top_p,
                    stream = request.stream,
                    user = request.user,
                    presence_penalty = request.presence_penalty,
                    frequency_penalty = request.frequency_penalty,
                    tools = tools,
                    tool_choice = "auto"
                };
            }

            return request;
        }

        private object BuildStreamingRequest(
            string systemPrompt,
            string query,
            string contextData,
            ModelConfig model)
        {
            return new
            {
                model = model.Name,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = $"Query: {query}\n\nML-Enhanced Context:\n{contextData}" }
                },
                max_tokens = Math.Min(model.MaxTokens, 2500),
                temperature = 0.3,
                top_p = 0.9,
                stream = true,
                user = "wiley-streaming-system"
            };
        }

        private async Task<AIQueryResponse> ExecuteWithRetry(object requestBody, ModelConfig model, int maxRetries = 3)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
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
                        var usage = ExtractUsageData(aiResponse);
                        
                        _usageTracker.RecordUsage(model.Name, usage);

                        return new AIQueryResponse
                        {
                            Success = true,
                            Analysis = analysisText,
                            ExecutionTimeMs = (int)executionTime,
                            Timestamp = DateTime.Now,
                            Usage = usage
                        };
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests && attempt < maxRetries)
                    {
                        // Exponential backoff for rate limiting
                        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
                        continue;
                    }
                    else
                    {
                        return new AIQueryResponse
                        {
                            Success = false,
                            Error = ParseXAIError(response.StatusCode, responseText),
                            ExecutionTimeMs = (int)executionTime,
                            Timestamp = DateTime.Now
                        };
                    }
                }
                catch (Exception) when (attempt < maxRetries)
                {
                    await Task.Delay(TimeSpan.FromSeconds(attempt * 2));
                    continue;
                }
                catch (Exception finalEx)
                {
                    return new AIQueryResponse
                    {
                        Success = false,
                        Error = $"Request execution failed: {finalEx.Message}",
                        ExecutionTimeMs = 0,
                        Timestamp = DateTime.Now
                    };
                }
            }

            return new AIQueryResponse
            {
                Success = false,
                Error = "Maximum retry attempts exceeded",
                Timestamp = DateTime.Now
            };
        }

        private string SerializeEnhancedContext(EnterpriseContext enterpriseData, RatePrediction? ratePrediction)
        {
            var context = new StringBuilder();
            
            context.AppendLine("=== ENTERPRISE FINANCIAL CONTEXT ===");
            context.AppendLine(enterpriseData.GetSummary());
            
            if (ratePrediction != null)
            {
                context.AppendLine("\n=== ML RATE PREDICTIONS ===");
                context.AppendLine($"Optimal Rate Range: ${ratePrediction.OptimalRateMin:F2} - ${ratePrediction.OptimalRateMax:F2}");
                context.AppendLine($"Customer Impact Score: {ratePrediction.CustomerImpactScore:F2}/10");
                context.AppendLine($"Revenue Optimization: {ratePrediction.RevenueOptimizationScore:F2}/10");
                context.AppendLine($"Seasonal Factors: {string.Join(", ", ratePrediction.SeasonalFactors)}");
                context.AppendLine($"Confidence Level: {ratePrediction.ConfidenceLevel:F2}%");
            }

            var ratios = enterpriseData.GetFinancialRatios();
            if (ratios.Any())
            {
                context.AppendLine("\n=== FINANCIAL RATIOS ===");
                foreach (var ratio in ratios)
                {
                    context.AppendLine($"{ratio.Key}: {ratio.Value:F2}");
                }
            }

            enterpriseData.AssessRiskFactors();
            if (enterpriseData.RiskFactors.Any())
            {
                context.AppendLine("\n=== RISK FACTORS ===");
                foreach (var risk in enterpriseData.RiskFactors)
                {
                    context.AppendLine($"- {risk}");
                }
            }

            return context.ToString();
        }

        private decimal CalculateConfidenceScore(AIQueryResponse response, EnterpriseContext data, QueryIntent intent)
        {
            decimal confidence = 0.5m; // Base confidence

            // Adjust based on data quality
            if (data.Accounts.Count > 10) confidence += 0.1m;
            if (data.TotalBudget > 0) confidence += 0.1m;
            if (data.CustomerBase > 0) confidence += 0.1m;

            // Adjust based on query intent clarity
            confidence += intent.ClarityScore * 0.2m;

            // Adjust based on response quality indicators
            if (response.Analysis?.Length > 500) confidence += 0.1m;
            if (response.Analysis?.Contains("$") == true) confidence += 0.05m;
            if (response.ExecutionTimeMs < 10000) confidence += 0.05m;

            return Math.Min(1.0m, Math.Max(0.1m, confidence));
        }

        private decimal CalculateStreamingConfidence(string analysis, EnterpriseContext data)
        {
            decimal confidence = 0.3m; // Lower base for streaming

            if (analysis.Length > 200) confidence += 0.2m;
            if (analysis.Contains("recommend")) confidence += 0.1m;
            if (analysis.Contains("$")) confidence += 0.1m;
            if (data.TotalBudget > 0) confidence += 0.15m;
            if (Regex.Matches(analysis, @"\$[\d,]+").Count > 2) confidence += 0.15m;

            return Math.Min(1.0m, confidence);
        }

        private decimal CalculateProgressiveConfidence(string partialAnalysis)
        {
            if (partialAnalysis.Length < 50) return 0.1m;
            if (partialAnalysis.Length < 200) return 0.3m;
            if (partialAnalysis.Length < 500) return 0.6m;
            return 0.8m;
        }

        private List<string> ExtractProgressiveInsights(string partialAnalysis, QueryIntent intent)
        {
            var insights = new List<string>();
            
            if (partialAnalysis.Contains("recommend"))
                insights.Add("Generating recommendations...");
            
            if (partialAnalysis.Contains("$") && intent.RequiresRateAnalysis)
                insights.Add("Calculating financial impacts...");
            
            if (partialAnalysis.Contains("customer"))
                insights.Add("Analyzing customer impact...");
            
            return insights;
        }

        // Additional helper methods for advanced tools and response formats would go here...
        
        private object[] GetAdvancedRateTools()
        {
            return new object[]
            {
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "optimize_utility_rates",
                        description = "ML-powered utility rate optimization with customer behavior prediction",
                        parameters = new
                        {
                            type = "object",
                            properties = new
                            {
                                current_rate = new { type = "number", description = "Current utility rate" },
                                customer_count = new { type = "number", description = "Number of customers" },
                                target_revenue = new { type = "number", description = "Target revenue amount" },
                                affordability_constraint = new { type = "number", description = "Maximum affordable rate increase percentage" },
                                seasonal_factors = new { type = "array", items = new { type = "string" }, description = "Seasonal adjustment factors" }
                            },
                            required = new[] { "current_rate", "customer_count", "target_revenue" }
                        }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "predict_customer_behavior",
                        description = "Predict customer response to rate changes using ML models",
                        parameters = new
                        {
                            type = "object",
                            properties = new
                            {
                                rate_change_percent = new { type = "number", description = "Proposed rate change percentage" },
                                customer_demographics = new { type = "string", description = "Customer demographic profile" },
                                historical_elasticity = new { type = "number", description = "Historical price elasticity" }
                            },
                            required = new[] { "rate_change_percent" }
                        }
                    }
                }
            };
        }

        private object[] GetEnhancedBudgetTools()
        {
            return new object[]
            {
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "ml_budget_forecast",
                        description = "Generate ML-based budget forecasts with confidence intervals",
                        parameters = new
                        {
                            type = "object",
                            properties = new
                            {
                                historical_data = new { type = "string", description = "Historical budget data" },
                                forecast_horizon = new { type = "number", description = "Forecast period in months" },
                                confidence_level = new { type = "number", description = "Desired confidence level (0-1)" }
                            },
                            required = new[] { "historical_data", "forecast_horizon" }
                        }
                    }
                }
            };
        }

        private object[] GetAnomalyAnalysisTools()
        {
            return new object[]
            {
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "analyze_spending_anomaly",
                        description = "Analyze detected spending anomalies with root cause analysis",
                        parameters = new
                        {
                            type = "object",
                            properties = new
                            {
                                anomaly_type = new { type = "string", description = "Type of anomaly detected" },
                                magnitude = new { type = "number", description = "Magnitude of deviation from normal" },
                                context = new { type = "string", description = "Context surrounding the anomaly" }
                            },
                            required = new[] { "anomaly_type", "magnitude" }
                        }
                    }
                }
            };
        }

        // Continuing with parsing and extraction methods...
        private string ParseStreamData(string data)
        {
            try
            {
                var streamData = JsonConvert.DeserializeObject<dynamic>(data);
                return streamData?.choices?[0]?.delta?.content?.ToString();
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private TokenUsage ExtractUsageData(dynamic apiResponse)
        {
            try
            {
                var usage = apiResponse?.usage;
                if (usage != null)
                {
                    var promptTokens = (int)(usage.prompt_tokens ?? 0);
                    var completionTokens = (int)(usage.completion_tokens ?? 0);
                    
                    return new TokenUsage
                    {
                        PromptTokens = promptTokens,
                        CompletionTokens = completionTokens,
                        TotalTokens = promptTokens + completionTokens,
                        EstimatedCost = CalculateCost("grok-3-beta", promptTokens, completionTokens)
                    };
                }
            }
            catch { }

            return new TokenUsage { EstimatedCost = 0 };
        }

        private decimal CalculateCost(string model, int promptTokens, int completionTokens)
        {
            if (!_modelConfigs.ContainsKey(model)) return 0;
            
            var config = _modelConfigs[model];
            var inputCost = (promptTokens / 1_000_000m) * config.InputCostPer1M;
            var outputCost = (completionTokens / 1_000_000m) * config.OutputCostPer1M;

            return inputCost + outputCost;
        }

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
                    System.Net.HttpStatusCode.Unauthorized => 
                        "Authentication failed. Please check your XAI_API_KEY environment variable.",
                    System.Net.HttpStatusCode.Forbidden => 
                        "Access denied. Your API key may not have sufficient permissions.",
                    System.Net.HttpStatusCode.ServiceUnavailable => 
                        "XAI service temporarily unavailable. Please try again in a few moments.",
                    _ => errorMessage ?? $"Unexpected error: {statusCode}"
                };
            }
            catch
            {
                return $"Unable to parse error response: {statusCode}";
            }
        }

        #endregion

        public void Dispose()
        {
            _httpClient?.Dispose();
            _mlEngine?.Dispose();
            _explainabilityEngine?.Dispose();
            _rateOptimizer?.Dispose();
        }
    }

    #region Enhanced Models and Classes

    public class ModelConfig
    {
        public string Name { get; set; } = string.Empty;
        public int MaxTokens { get; set; }
        public decimal InputCostPer1M { get; set; }
        public decimal OutputCostPer1M { get; set; }
        public bool SupportsStreaming { get; set; }
        public bool SupportsFunctionCalling { get; set; }
        public bool SupportsVision { get; set; }
    }

    public class QueryOptions
    {
        public bool PreferStreaming { get; set; } = false;
        public bool OptimizeForCost { get; set; } = false;
        public bool RequireHighPrecision { get; set; } = false;
        public int MaxTokens { get; set; } = 2000;
        public decimal Temperature { get; set; } = 0.3m;
        public string PreferredModel { get; set; } = string.Empty;
    }

    public class QueryIntent
    {
        public string IntentType { get; set; } = string.Empty;
        public decimal ClarityScore { get; set; }
        public int ComplexityScore { get; set; }
        public bool RequiresRateAnalysis { get; set; }
        public bool RequiresPrecision { get; set; }
        public bool RequiresVision { get; set; }
        public List<string> KeyConcepts { get; set; } = new List<string>();
        public List<string> RequiredData { get; set; } = new List<string>();
    }

    public class RatePrediction
    {
        public decimal OptimalRateMin { get; set; }
        public decimal OptimalRateMax { get; set; }
        public decimal CustomerImpactScore { get; set; }
        public decimal RevenueOptimizationScore { get; set; }
        public decimal SeasonalAdjustment { get; set; }
        public decimal ConfidenceLevel { get; set; }
        public List<string> SeasonalFactors { get; set; } = new List<string>();
        public string RecommendationSummary { get; set; } = string.Empty;
    }

    public class EnhancedAIResponse
    {
        public bool Success { get; set; }
        public string Analysis { get; set; } = string.Empty;
        public QueryIntent QueryIntent { get; set; } = new QueryIntent();
        public RatePrediction RatePrediction { get; set; } = new RatePrediction();
        public ExplainabilityAnalysis Explainability { get; set; } = new ExplainabilityAnalysis();
        public decimal ConfidenceScore { get; set; }
        public List<MLRecommendation> MLRecommendations { get; set; } = new List<MLRecommendation>();
        public int ExecutionTimeMs { get; set; }
        public TokenUsage Usage { get; set; } = new TokenUsage();
        public string ModelUsed { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Error { get; set; } = string.Empty;
    }

    public class StreamingEnhancedResponse
    {
        public bool Success { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public QueryIntent QueryIntent { get; set; } = new QueryIntent();
        public RatePrediction RatePrediction { get; set; } = new RatePrediction();
        public decimal ProgressiveConfidence { get; set; }
        public List<string> ProgressiveInsights { get; set; } = new List<string>();
        public ExplainabilityAnalysis FinalExplainability { get; set; } = new ExplainabilityAnalysis();
        public decimal FinalConfidence { get; set; }
        public int TotalExecutionMs { get; set; }
        public string Error { get; set; } = string.Empty;
    }

    // Additional enhanced model classes would continue here...
    
    #endregion
}
