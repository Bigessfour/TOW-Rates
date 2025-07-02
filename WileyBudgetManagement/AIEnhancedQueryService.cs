using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Services;

namespace WileyBudgetManagement.Services.Enhanced
{
    /// <summary>
    /// Simplified AI Query Service with xAI Grok-3-beta integration
    /// Focused on practical utility rate analysis for Town of Wiley
    /// Complex calculations delegated to AIEnhancedCalculations service
    /// Enhanced with municipal security validation and audit logging
    /// </summary>
    public class AIEnhancedQueryService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _xaiApiKey;
        private readonly string _apiEndpoint = "https://api.x.ai/v1/chat/completions";
        private readonly TokenUsageTracker _usageTracker;
        private readonly IMunicipalAuditLogger _logger;

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

        public AIEnhancedQueryService(IMunicipalAuditLogger? logger = null)
        {
            _httpClient = new HttpClient();
            _xaiApiKey = ValidateAndGetApiKey();
            _usageTracker = new TokenUsageTracker();
            _logger = logger ?? new MunicipalAuditLogger();

            _logger.LogInformation("AIEnhancedQueryService",
                "Initializing AI service for Town of Wiley",
                new { Municipality = "Town of Wiley", Service = "xAI Grok-3-beta" });

            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_xaiApiKey}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "WileyBudgetManagement/1.0");
            _httpClient.Timeout = TimeSpan.FromSeconds(60);

            _logger.LogInformation("AIEnhancedQueryService",
                "HTTP client configured for Town of Wiley municipal operations");
        }

        /// <summary>
        /// Validates XAI API key for Town of Wiley municipal security requirements
        /// Ensures proper format, length, and accessibility before use
        /// </summary>
        private string ValidateAndGetApiKey()
        {
            var apiKey = Environment.GetEnvironmentVariable("XAI_API_KEY") ?? string.Empty;

            // Check if API key exists
            if (string.IsNullOrEmpty(apiKey))
            {
                var error = "XAI_API_KEY environment variable not found. Municipal software requires secure API key configuration.";
                _logger?.LogError("APIKeyValidation", error);
                throw new InvalidOperationException($"{error} Please set it using: [Environment]::SetEnvironmentVariable(\"XAI_API_KEY\", \"your-key\", \"Machine\")");
            }

            // Validate API key format for xAI service
            if (!IsValidXAIApiKeyFormat(apiKey))
            {
                var error = "Invalid XAI API key format detected. Town of Wiley security policy requires properly formatted API keys.";
                _logger?.LogWarning("APIKeyValidation", error);
                throw new InvalidOperationException($"{error} Expected format: xai-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            }

            // Additional security validation
            if (apiKey.Length < 32)
            {
                var error = "XAI API key appears to be too short for security requirements.";
                _logger?.LogWarning("APIKeyValidation", error);
                throw new InvalidOperationException($"{error} Municipal software requires full-length API keys for compliance.");
            }

            // Test basic accessibility
            ValidateApiKeyAccessibility(apiKey);

            _logger?.LogInformation("APIKeyValidation",
                "XAI API key validated successfully for Town of Wiley operations",
                new { KeyLength = apiKey.Length, Municipality = "Town of Wiley" });

            return apiKey;
        }

        /// <summary>
        /// Validates XAI API key format according to service specifications
        /// </summary>
        private bool IsValidXAIApiKeyFormat(string apiKey)
        {
            // xAI API keys typically start with "xai-" followed by alphanumeric characters
            if (!apiKey.StartsWith("xai-", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Check for reasonable length (xAI keys are typically 43+ characters)
            if (apiKey.Length < 32)
            {
                return false;
            }

            // Ensure no whitespace or invalid characters
            if (apiKey.Contains(" ") || apiKey.Contains("\t") || apiKey.Contains("\n"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates API key accessibility for municipal compliance
        /// Prepares for future enhancement with actual API validation
        /// </summary>
        private void ValidateApiKeyAccessibility(string apiKey)
        {
            // Basic validation - in production this could make a test API call
            // For now, ensure the key meets basic security requirements

            if (apiKey.Contains("test") || apiKey.Contains("demo") || apiKey.Contains("example"))
            {
                throw new InvalidOperationException(
                    "Test or demo API keys are not permitted in Town of Wiley production systems. " +
                    "Please use a valid production API key for municipal operations.");
            }

            // TODO: Future enhancement - make actual validation call to xAI API
            // This would be implemented when we want to verify the key works
            // await TestApiKeyWithService(apiKey);
        }

        /// <summary>
        /// Simple AI query for rate optimization - delegates complex calculations to AIEnhancedCalculations
        /// </summary>
        public async Task<AIQueryResponse> QueryRateOptimization(
            EnterpriseContext enterpriseData,
            string query)
        {
            var operationId = Guid.NewGuid().ToString()[..8];
            _logger.LogInformation("RateOptimization",
                "Starting rate optimization query for Town of Wiley",
                new { OperationId = operationId, Municipality = enterpriseData.Municipality, QueryLength = query.Length });

            try
            {
                // Validate municipal context before processing
                ValidateMunicipalContext(enterpriseData, "rate optimization");

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

                _logger.LogInformation("RateOptimization",
                    "Sending rate optimization request to xAI service",
                    new { OperationId = operationId, Model = "grok-3-beta", MaxTokens = 1500 });

                var result = await ExecuteWithRetry(requestBody, _modelConfigs["grok-3-beta"]).ConfigureAwait(false);

                // Log the result for municipal audit
                _logger.LogInformation("RateOptimization",
                    "Rate optimization query completed",
                    new
                    {
                        OperationId = operationId,
                        Success = result.Success,
                        ExecutionTimeMs = result.ExecutionTimeMs,
                        EstimatedCost = result.Usage?.EstimatedCost ?? 0,
                        TokensUsed = result.Usage?.TotalTokens ?? 0
                    });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("RateOptimization",
                    "Rate optimization query failed",
                    new { OperationId = operationId, Error = ex.Message, Municipality = enterpriseData.Municipality });
                throw;
            }
        }

        /// <summary>
        /// Simple anomaly detection query - delegates complex ML to AIEnhancedCalculations
        /// </summary>
        public async Task<AIQueryResponse> QueryAnomalyDetection(
            EnterpriseContext enterpriseData,
            string analysisQuery)
        {
            var operationId = Guid.NewGuid().ToString()[..8];
            _logger.LogInformation("AnomalyDetection",
                "Starting anomaly detection analysis for Town of Wiley",
                new { OperationId = operationId, Municipality = enterpriseData.Municipality });

            try
            {
                ValidateMunicipalContext(enterpriseData, "anomaly detection");

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
                    temperature = 0.1
                };

                var result = await ExecuteWithRetry(requestBody, _modelConfigs["grok-3-beta"]).ConfigureAwait(false);

                _logger.LogInformation("AnomalyDetection",
                    "Anomaly detection analysis completed",
                    new
                    {
                        OperationId = operationId,
                        Success = result.Success,
                        EstimatedCost = result.Usage?.EstimatedCost ?? 0
                    });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("AnomalyDetection",
                    "Anomaly detection analysis failed",
                    new { OperationId = operationId, Error = ex.Message });
                throw;
            }
        }

        /// <summary>
        /// Simple revenue forecasting query - delegates complex analysis to AIEnhancedCalculations
        /// </summary>
        public async Task<AIQueryResponse> QueryRevenueForecast(
            EnterpriseContext enterpriseData,
            string forecastQuery)
        {
            var operationId = Guid.NewGuid().ToString()[..8];
            _logger.LogInformation("RevenueForecast",
                "Starting revenue forecasting for Town of Wiley",
                new { OperationId = operationId, Municipality = enterpriseData.Municipality });

            try
            {
                ValidateMunicipalContext(enterpriseData, "revenue forecast");

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

                var result = await ExecuteWithRetry(requestBody, _modelConfigs["grok-3-beta"]).ConfigureAwait(false);

                _logger.LogInformation("RevenueForecast",
                    "Revenue forecasting completed",
                    new
                    {
                        OperationId = operationId,
                        Success = result.Success,
                        EstimatedCost = result.Usage?.EstimatedCost ?? 0
                    });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("RevenueForecast",
                    "Revenue forecasting failed",
                    new { OperationId = operationId, Error = ex.Message });
                throw;
            }
        }

        #region Core AI Query Methods

        /// <summary>
        /// General purpose AI query with basic response handling
        /// </summary>
        public async Task<AIQueryResponse> ProcessGeneralQuery(
            string query,
            EnterpriseContext enterpriseData)
        {
            var operationId = Guid.NewGuid().ToString()[..8];
            _logger.LogInformation("GeneralQuery",
                "Processing general AI query for Town of Wiley",
                new { OperationId = operationId, Municipality = enterpriseData.Municipality });

            try
            {
                ValidateMunicipalContext(enterpriseData, "general query");

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

                var result = await ExecuteWithRetry(requestBody, _modelConfigs["grok-3-beta"]).ConfigureAwait(false);

                _logger.LogInformation("GeneralQuery",
                    "General query completed",
                    new
                    {
                        OperationId = operationId,
                        Success = result.Success,
                        EstimatedCost = result.Usage?.EstimatedCost ?? 0
                    });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("GeneralQuery",
                    "General query failed",
                    new { OperationId = operationId, Error = ex.Message });
                throw;
            }
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
                    var response = await _httpClient.PostAsync(_apiEndpoint, content).ConfigureAwait(false);
                    var executionTime = (DateTime.Now - startTime).TotalMilliseconds;

                    var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var aiResponse = JsonConvert.DeserializeObject<dynamic>(responseText);
                        var analysisText = aiResponse?.choices?[0]?.message?.content?.ToString() ?? string.Empty;
                        var usage = ExtractUsageData(aiResponse);

                        _usageTracker.RecordUsage(model.Name, usage);

                        // Log successful API call for municipal audit
                        _logger.LogInformation("AIServiceCall",
                            "Successful API call to xAI service",
                            new
                            {
                                Model = model.Name,
                                ExecutionTimeMs = (int)executionTime,
                                PromptTokens = usage.PromptTokens,
                                CompletionTokens = usage.CompletionTokens,
                                EstimatedCost = usage.EstimatedCost,
                                Municipality = "Town of Wiley"
                            });

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
                        _logger.LogWarning("AIServiceCall",
                            $"Rate limit exceeded, retrying in {Math.Pow(2, attempt)} seconds",
                            new { Attempt = attempt, MaxRetries = maxRetries });

                        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt))).ConfigureAwait(false);
                        continue;
                    }
                    else
                    {
                        var error = ParseXAIError(response.StatusCode, responseText);
                        _logger.LogError("AIServiceCall",
                            "API call failed",
                            new { StatusCode = response.StatusCode, Error = error, Attempt = attempt });

                        return new AIQueryResponse
                        {
                            Success = false,
                            Error = error,
                            ExecutionTimeMs = (int)executionTime,
                            Timestamp = DateTime.Now
                        };
                    }
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    _logger.LogWarning("AIServiceCall",
                        $"API call attempt {attempt} failed, retrying",
                        new { Attempt = attempt, Error = ex.Message });

                    await Task.Delay(TimeSpan.FromSeconds(attempt * 2)).ConfigureAwait(false);
                    continue;
                }
                catch (Exception finalEx)
                {
                    _logger.LogError("AIServiceCall",
                        "All API call attempts failed",
                        new { MaxRetries = maxRetries, FinalError = finalEx.Message });

                    return new AIQueryResponse
                    {
                        Success = false,
                        Error = $"Request execution failed: {finalEx.Message}",
                        ExecutionTimeMs = 0,
                        Timestamp = DateTime.Now
                    };
                }
            }

            _logger.LogError("AIServiceCall",
                "Maximum retry attempts exceeded",
                new { MaxRetries = maxRetries });

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
                var errorMessage = errorResponse?.error?.message?.ToString() ?? string.Empty;
                var errorType = errorResponse?.error?.type?.ToString() ?? string.Empty;

                return statusCode switch
                {
                    System.Net.HttpStatusCode.TooManyRequests =>
                        $"Rate limit exceeded. Please wait before making another request. {errorMessage}",
                    System.Net.HttpStatusCode.BadRequest when errorType == "invalid_request_error" =>
                        $"Invalid request format or parameters. {errorMessage}",
                    System.Net.HttpStatusCode.BadRequest when errorMessage.Contains("content_policy") =>
                        $"Content policy violation. Please review your query content. {errorMessage}",
                    System.Net.HttpStatusCode.Unauthorized =>
                        "Authentication failed. Please check your XAI_API_KEY environment variable.",
                    System.Net.HttpStatusCode.Forbidden =>
                        "Access denied. Your API key may not have sufficient permissions.",
                    System.Net.HttpStatusCode.ServiceUnavailable =>
                        "XAI service temporarily unavailable. Please try again in a few moments.",
                    _ => string.IsNullOrEmpty(errorMessage) ? $"Unexpected error: {statusCode}" : errorMessage
                };
            }
            catch
            {
                return $"Unable to parse error response: {statusCode}";
            }
        }

        /// <summary>
        /// Validates municipal context for Town of Wiley specific requirements
        /// </summary>
        private void ValidateMunicipalContext(EnterpriseContext context, string operationType)
        {
            if (context == null)
            {
                _logger.LogError("MunicipalValidation",
                    "Municipal context validation failed - null context",
                    new { OperationType = operationType });
                throw new ArgumentNullException(nameof(context),
                    "Municipal context is required for Town of Wiley operations");
            }

            // Ensure this service is being used for Town of Wiley only
            if (!context.Municipality.Equals("Town of Wiley", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError("MunicipalValidation",
                    "Municipal validation failed - incorrect municipality",
                    new { OperationType = operationType, ProvidedMunicipality = context.Municipality });
                throw new InvalidOperationException(
                    $"This AI service is configured exclusively for Town of Wiley operations. " +
                    $"Current context municipality: {context.Municipality}");
            }

            // Validate rate study data for rate optimization queries
            if (operationType.Contains("rate") && !context.HasRateStudyData)
            {
                _logger.LogWarning("MunicipalValidation",
                    "Rate optimization requested without rate study data",
                    new { OperationType = operationType, HasRateStudyData = context.HasRateStudyData });
                throw new InvalidOperationException(
                    "Rate optimization queries require valid rate study data. " +
                    "Please ensure rate study methodology data is available before proceeding.");
            }

            _logger.LogInformation("MunicipalValidation",
                "Municipal context validation successful",
                new
                {
                    OperationType = operationType,
                    Municipality = context.Municipality,
                    HasRateStudyData = context.HasRateStudyData
                });
        }

        public void Dispose()
        {
            _logger?.LogInformation("AIEnhancedQueryService",
                "Disposing AI service for Town of Wiley",
                new { TotalCost = _usageTracker.GetTotalCost() });
            _httpClient?.Dispose();
        }

        #endregion

        #region Municipal Audit Logging

        /// <summary>
        /// Municipal audit logging interface for Town of Wiley compliance
        /// </summary>
        public interface IMunicipalAuditLogger
        {
            void LogInformation(string category, string message, object? data = null);
            void LogWarning(string category, string message, object? data = null);
            void LogError(string category, string message, object? data = null);
        }

        /// <summary>
        /// Municipal audit logger implementation for Town of Wiley
        /// Ensures compliance with municipal transparency and audit requirements
        /// </summary>
        public class MunicipalAuditLogger : IMunicipalAuditLogger
        {
            private readonly string _logFile;

            public MunicipalAuditLogger()
            {
                var logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "TownOfWiley", "Logs");
                Directory.CreateDirectory(logDirectory);
                _logFile = Path.Combine(logDirectory, $"AI_Audit_{DateTime.Now:yyyyMMdd}.log");
            }

            public void LogInformation(string category, string message, object? data = null)
            {
                WriteLog("INFO", category, message, data);
            }

            public void LogWarning(string category, string message, object? data = null)
            {
                WriteLog("WARN", category, message, data);
            }

            public void LogError(string category, string message, object? data = null)
            {
                WriteLog("ERROR", category, message, data);
            }

            private void WriteLog(string level, string category, string message, object? data)
            {
                try
                {
                    var logEntry = new
                    {
                        Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        Level = level,
                        Category = category,
                        Message = message,
                        Data = data,
                        Municipality = "Town of Wiley",
                        Application = "Budget Management System"
                    };

                    var json = JsonConvert.SerializeObject(logEntry, Formatting.None);
                    File.AppendAllText(_logFile, json + Environment.NewLine);

                    // Also write to console for development
                    Console.WriteLine($"[{logEntry.Timestamp}] [{level}] {category}: {message}");
                }
                catch
                {
                    // Fail silently to avoid breaking the application
                    // In production, this might write to Windows Event Log as fallback
                }
            }
        }

        #endregion
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

        public decimal GetTotalCost(string? model = null)
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
