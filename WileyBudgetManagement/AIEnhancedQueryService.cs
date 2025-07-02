using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Services;

namespace WileyBudgetManagement.Services.Enhanced
{
    /// <summary>
    /// Simplified AI Query Service with xAI Grok-3-beta integration
    /// Focused on practical utility rate analysis for Town of Wiley
    /// Complex calculations delegated to AIEnhancedCalculations service
    /// </summary>
    public class AIEnhancedQueryService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _xaiApiKey;
        private readonly string _apiEndpoint = "https://api.x.ai/v1/chat/completions";
        private readonly TokenUsageTracker _usageTracker;
        // Model configuration for different AI models
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
            }
        };

        public AIEnhancedQueryService()
        {
            _httpClient = new HttpClient();
            _xaiApiKey = Environment.GetEnvironmentVariable("XAI_API_KEY") ?? string.Empty;
            _usageTracker = new TokenUsageTracker();

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
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
        }

        /// <summary>
        /// Simple AI query for rate optimization - delegates complex calculations to AIEnhancedCalculations
        /// </summary>
        public async Task<AIQueryResponse> QueryRateOptimization(
            EnterpriseContext enterpriseData,
            string query)
        {
            var systemPrompt = @"You are a municipal finance AI assistant for the Town of Wiley, Colorado.
                               Provide clear, practical advice on utility rate optimization.
                               Focus on customer affordability, revenue adequacy, and regulatory compliance.";

            var contextData = enterpriseData.GetSummary();
            
            var requestBody = new
            {
                model = "grok-3-beta",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = $"Query: {query}\n\nContext:\n{contextData}" }
                },
                max_tokens = 1500,
                temperature = 0.3
            };

            return await ExecuteWithRetry(requestBody, _modelConfigs["grok-3-beta"]);
        }

        /// <summary>
        /// Simple anomaly detection query - delegates complex ML to AIEnhancedCalculations
        /// </summary>
        public async Task<AIQueryResponse> QueryAnomalyDetection(
            EnterpriseContext enterpriseData,
            string analysisQuery)
        {
            var systemPrompt = @"You are a financial anomaly detection specialist for municipal utilities.
                               Analyze spending patterns and identify unusual financial activities.
                               Provide practical recommendations for investigation and prevention.";

            var contextData = enterpriseData.GetSummary();
            
            var requestBody = new
            {
                model = "grok-3-beta",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = $"Analysis Request: {analysisQuery}\n\nFinancial Context:\n{contextData}" }
                },
                max_tokens = 1200,
                temperature = 0.1 // Low temperature for precise analysis
            };

            return await ExecuteWithRetry(requestBody, _modelConfigs["grok-3-beta"]);
        }

        /// <summary>
        /// Simple revenue forecasting query - delegates complex analysis to AIEnhancedCalculations
        /// </summary>
        public async Task<AIQueryResponse> QueryRevenueForecast(
            EnterpriseContext enterpriseData,
            string forecastQuery)
        {
            var systemPrompt = @"You are a municipal revenue forecasting specialist.
                               Analyze revenue trends and provide realistic projections.
                               Consider seasonal patterns, economic conditions, and regulatory changes.";

            var contextData = enterpriseData.GetSummary();
            
            var requestBody = new
            {
                model = "grok-3-beta",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = $"Forecast Request: {forecastQuery}\n\nRevenue Context:\n{contextData}" }
                },
                max_tokens = 1500,
                temperature = 0.2
            };

            return await ExecuteWithRetry(requestBody, _modelConfigs["grok-3-beta"]);
        }

        #region Core AI Query Methods

        /// <summary>
        /// General purpose AI query with basic response handling
        /// </summary>
        public async Task<AIQueryResponse> ProcessGeneralQuery(
            string query,
            EnterpriseContext enterpriseData)
        {
            var systemPrompt = @"You are a municipal finance AI assistant for the Town of Wiley, Colorado.
                               Provide clear, actionable advice on utility management, budgeting, and financial planning.
                               Always consider customer affordability and regulatory compliance.";

            var contextData = enterpriseData.GetSummary();
            
            var requestBody = new
            {
                model = "grok-3-beta",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = $"Question: {query}\n\nContext:\n{contextData}" }
                },
                max_tokens = 2000,
                temperature = 0.3
            };

            return await ExecuteWithRetry(requestBody, _modelConfigs["grok-3-beta"]);
        }

        #endregion

        #region Helper Methods

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
        }
    }

    #region Simplified Models and Classes

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

    public class AIQueryResponse
    {
        public bool Success { get; set; }
        public string Analysis { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public int ExecutionTimeMs { get; set; }
        public DateTime Timestamp { get; set; }
        public TokenUsage Usage { get; set; } = new TokenUsage();
    }

    public class TokenUsageTracker
    {
        private readonly Dictionary<string, List<TokenUsage>> _usageHistory = new();

        public void RecordUsage(string model, TokenUsage usage)
        {
            if (!_usageHistory.ContainsKey(model))
                _usageHistory[model] = new List<TokenUsage>();
            
            _usageHistory[model].Add(usage);
        }

        public decimal GetTotalCost(string model = null)
        {
            if (model != null)
                return _usageHistory.ContainsKey(model) 
                    ? _usageHistory[model].Sum(u => u.EstimatedCost) 
                    : 0;
            
            return _usageHistory.SelectMany(kvp => kvp.Value).Sum(u => u.EstimatedCost);
        }
    }

    #endregion
}
