using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WileyBudgetManagement.Models;
using WileyBudgetManagement.Services;

namespace WileyBudgetManagement.Services.Enhanced
{
    /// <summary>
    /// Machine Learning Predictive Engine for municipal financial analysis
    /// Replaces hard-coded calculations with adaptive ML models
    /// Implements predictive modeling for rate optimization and financial forecasting
    /// </summary>
    public class MLPredictiveEngine : IDisposable
    {
        private readonly Dictionary<string, MLModel> _trainedModels;
        private readonly FeatureExtractor _featureExtractor;
        private readonly TimeSeriesAnalyzer _timeSeriesAnalyzer;

        public MLPredictiveEngine()
        {
            _trainedModels = new Dictionary<string, MLModel>();
            _featureExtractor = new FeatureExtractor();
            _timeSeriesAnalyzer = new TimeSeriesAnalyzer();
            
            InitializeModels();
        }

        private void InitializeModels()
        {
            // Initialize pre-trained models for different analysis types
            _trainedModels["rate_optimization"] = new RateOptimizationModel();
            _trainedModels["customer_behavior"] = new CustomerBehaviorModel();
            _trainedModels["anomaly_detection"] = new AnomalyDetectionModel();
            _trainedModels["seasonal_forecast"] = new SeasonalForecastModel();
            _trainedModels["revenue_prediction"] = new RevenuePredictionModel();
        }

        /// <summary>
        /// Analyze query intent using natural language processing
        /// </summary>
        public QueryIntent AnalyzeQueryIntent(string query)
        {
            var intent = new QueryIntent();
            
            try
            {
                var lowerQuery = query.ToLower();
                
                // Intent classification
                intent.IntentType = ClassifyIntent(lowerQuery);
                intent.ClarityScore = CalculateClarityScore(query);
                intent.ComplexityScore = CalculateComplexityScore(query);
                
                // Feature detection
                intent.RequiresRateAnalysis = ContainsRateKeywords(lowerQuery);
                intent.RequiresPrecision = ContainsPrecisionKeywords(lowerQuery);
                intent.RequiresVision = ContainsVisionKeywords(lowerQuery);
                
                // Extract key concepts and required data
                intent.KeyConcepts = ExtractKeyConcepts(query);
                intent.RequiredData = DetermineRequiredData(intent.KeyConcepts);
                
                return intent;
            }
            catch (Exception)
            {
                // Return default intent on error
                var lowerQuery = query.ToLower();
                return new QueryIntent
                {
                    IntentType = "general_analysis",
                    ClarityScore = 0.5m,
                    ComplexityScore = 5,
                    RequiresRateAnalysis = lowerQuery.Contains("rate") || lowerQuery.Contains("price"),
                    KeyConcepts = new List<string> { "general_inquiry" }
                };
            }
        }

        /// <summary>
        /// Predict optimal rates using ML models and historical patterns
        /// </summary>
        public async Task<RatePrediction> PredictOptimalRates(
            EnterpriseContext enterpriseData, 
            QueryIntent queryIntent)
        {
            try
            {
                if (!_trainedModels.ContainsKey("rate_optimization"))
                    throw new InvalidOperationException("Rate optimization model not available");

                var model = _trainedModels["rate_optimization"] as RateOptimizationModel;
                var features = _featureExtractor.ExtractRateFeatures(enterpriseData, queryIntent);
                
                var prediction = await model.PredictOptimalRates(features);
                
                return new RatePrediction
                {
                    OptimalRateMin = prediction.MinRate,
                    OptimalRateMax = prediction.MaxRate,
                    CustomerImpactScore = CalculateCustomerImpact(prediction, enterpriseData),
                    RevenueOptimizationScore = CalculateRevenueOptimization(prediction, enterpriseData),
                    SeasonalAdjustment = prediction.SeasonalAdjustment,
                    ConfidenceLevel = prediction.Confidence,
                    SeasonalFactors = prediction.SeasonalFactors,
                    RecommendationSummary = GenerateRateRecommendationSummary(prediction)
                };
            }
            catch (Exception ex)
            {
                // Return conservative prediction on error
                return new RatePrediction
                {
                    OptimalRateMin = enterpriseData.RequiredRate * 0.95m,
                    OptimalRateMax = enterpriseData.RequiredRate * 1.05m,
                    CustomerImpactScore = 5.0m,
                    RevenueOptimizationScore = 5.0m,
                    SeasonalAdjustment = 0.0m,
                    ConfidenceLevel = 30.0m,
                    RecommendationSummary = $"Conservative rate adjustment recommended due to analysis error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Analyze historical patterns using ML algorithms
        /// Replaces static historical analysis with dynamic pattern recognition
        /// </summary>
        public List<HistoricalPattern> AnalyzeHistoricalPatterns(
            List<HistoricalRateData> historicalData)
        {
            var patterns = new List<HistoricalPattern>();
            
            if (!historicalData.Any()) 
            {
                patterns.Add(new HistoricalPattern
                {
                    PatternType = "insufficient_data",
                    Description = "Limited historical data available for pattern analysis",
                    Confidence = 0.1m,
                    Recommendations = new List<string> { "Collect more historical data for better predictions" }
                });
                return patterns;
            }

            try
            {
                // Trend analysis
                var trendPattern = _timeSeriesAnalyzer.AnalyzeTrends(historicalData);
                if (trendPattern != null) patterns.Add(trendPattern);

                // Seasonal pattern detection
                var seasonalPatterns = _timeSeriesAnalyzer.DetectSeasonalPatterns(historicalData);
                patterns.AddRange(seasonalPatterns);

                // Volatility analysis
                var volatilityPattern = _timeSeriesAnalyzer.AnalyzeVolatility(historicalData);
                if (volatilityPattern != null) patterns.Add(volatilityPattern);

                // Growth pattern analysis
                var growthPattern = _timeSeriesAnalyzer.AnalyzeGrowthPatterns(historicalData);
                if (growthPattern != null) patterns.Add(growthPattern);

                return patterns;
            }
            catch (Exception ex)
            {
                patterns.Add(new HistoricalPattern
                {
                    PatternType = "analysis_error",
                    Description = $"Pattern analysis encountered an error: {ex.Message}",
                    Confidence = 0.0m,
                    Recommendations = new List<string> { "Review data quality and try analysis again" }
                });
                return patterns;
            }
        }

        /// <summary>
        /// Generate seasonal forecasts using advanced time series ML models
        /// </summary>
        public async Task<SeasonalForecast> GenerateSeasonalForecast(
            List<HistoricalRateData> historicalData, 
            int forecastMonths)
        {
            try
            {
                if (!_trainedModels.ContainsKey("seasonal_forecast"))
                    throw new InvalidOperationException("Seasonal forecast model not available");

                var model = _trainedModels["seasonal_forecast"] as SeasonalForecastModel;
                var timeSeriesFeatures = _featureExtractor.ExtractTimeSeriesFeatures(historicalData);
                
                var forecast = await model.GenerateForecast(timeSeriesFeatures, forecastMonths);
                
                return new SeasonalForecast
                {
                    ForecastPeriodMonths = forecastMonths,
                    MonthlyForecasts = forecast.MonthlyPredictions,
                    SeasonalFactors = forecast.SeasonalAdjustments,
                    ConfidenceIntervals = forecast.ConfidenceIntervals,
                    TrendComponent = forecast.TrendComponent,
                    SeasonalComponent = forecast.SeasonalComponent,
                    IrregularComponent = forecast.IrregularComponent,
                    OverallConfidence = forecast.ModelConfidence,
                    KeyDrivers = forecast.InfluentialFactors,
                    RiskFactors = IdentifyForecastRisks(forecast)
                };
            }
            catch (Exception ex)
            {
                // Return simple seasonal forecast on error
                return GenerateSimpleSeasonalForecast(historicalData, forecastMonths, ex.Message);
            }
        }

        /// <summary>
        /// Predict customer behavior changes in response to rate modifications
        /// Uses behavioral economics ML models
        /// </summary>
        public async Task<CustomerBehaviorPrediction> PredictCustomerBehavior(
            EnterpriseContext enterpriseData,
            List<HistoricalRateData> historicalData,
            RateOptimizationGoals goals)
        {
            try
            {
                if (!_trainedModels.ContainsKey("customer_behavior"))
                    throw new InvalidOperationException("Customer behavior model not available");

                var model = _trainedModels["customer_behavior"] as CustomerBehaviorModel;
                var behaviorFeatures = _featureExtractor.ExtractBehaviorFeatures(
                    enterpriseData, historicalData, goals);
                
                var prediction = await model.PredictBehavior(behaviorFeatures);
                
                return new CustomerBehaviorPrediction
                {
                    ExpectedUsageChange = prediction.UsageChangePercent,
                    CustomerRetentionRate = prediction.RetentionRate,
                    PriceElasticity = prediction.PriceElasticity,
                    AffordabilityIndex = prediction.AffordabilityIndex,
                    CustomerSegmentImpacts = prediction.SegmentImpacts,
                    ChurnRisk = prediction.ChurnRisk,
                    RevenueImpact = prediction.RevenueImpact,
                    ConfidenceLevel = prediction.Confidence,
                    BehaviorDrivers = prediction.KeyDrivers,
                    RecommendedMitigations = GenerateMitigationStrategies(prediction)
                };
            }
            catch (Exception ex)
            {
                // Return conservative behavior prediction
                return new CustomerBehaviorPrediction
                {
                    ExpectedUsageChange = -2.0m, // Conservative 2% usage decrease
                    CustomerRetentionRate = 95.0m,
                    PriceElasticity = -0.5m,
                    AffordabilityIndex = 0.8m,
                    ChurnRisk = 5.0m,
                    ConfidenceLevel = 40.0m,
                    BehaviorDrivers = new List<string> { "Rate sensitivity", "Economic conditions" },
                    RecommendedMitigations = new List<string> { 
                        "Gradual rate implementation", 
                        "Customer communication plan",
                        $"Note: Prediction based on conservative estimates due to analysis error: {ex.Message}"
                    }
                };
            }
        }

        /// <summary>
        /// Detect financial anomalies using ensemble ML methods
        /// Replaces static threshold-based detection with adaptive models
        /// </summary>
        public async Task<List<FinancialAnomaly>> DetectAnomalies(
            List<FinancialDataPoint> timeSeriesData,
            AnomalyDetectionConfig config)
        {
            var anomalies = new List<FinancialAnomaly>();
            
            try
            {
                if (!_trainedModels.ContainsKey("anomaly_detection"))
                    throw new InvalidOperationException("Anomaly detection model not available");

                var model = _trainedModels["anomaly_detection"] as AnomalyDetectionModel;
                var anomalyFeatures = _featureExtractor.ExtractAnomalyFeatures(timeSeriesData, config);
                
                var detectedAnomalies = await model.DetectAnomalies(anomalyFeatures, config);
                
                foreach (var anomaly in detectedAnomalies)
                {
                    anomalies.Add(new FinancialAnomaly
                    {
                        AnomalyId = Guid.NewGuid().ToString(),
                        Timestamp = anomaly.Timestamp,
                        AnomalyType = anomaly.Type,
                        Severity = anomaly.Severity,
                        AnomalyScore = anomaly.Score,
                        ExpectedValue = anomaly.ExpectedValue,
                        ActualValue = anomaly.ActualValue,
                        Deviation = anomaly.Deviation,
                        Description = anomaly.Description,
                        PotentialCauses = anomaly.PotentialCauses,
                        RecommendedActions = anomaly.RecommendedActions,
                        ConfidenceLevel = anomaly.Confidence
                    });
                }
                
                return anomalies;
            }
            catch (Exception ex)
            {
                // Return error anomaly
                anomalies.Add(new FinancialAnomaly
                {
                    AnomalyId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.Now,
                    AnomalyType = "detection_error",
                    Severity = "Low",
                    Description = $"Anomaly detection failed: {ex.Message}",
                    RecommendedActions = new List<string> { "Review data quality", "Retry detection with different parameters" },
                    ConfidenceLevel = 0.0m
                });
                
                return anomalies;
            }
        }

        #region Helper Methods

        private string ClassifyIntent(string lowerQuery)
        {
            if (lowerQuery.Contains("what if") || lowerQuery.Contains("scenario"))
                return "scenario_analysis";
            
            if (lowerQuery.Contains("rate") || lowerQuery.Contains("price") || lowerQuery.Contains("cost"))
                return "rate_analysis";
            
            if (lowerQuery.Contains("predict") || lowerQuery.Contains("forecast"))
                return "predictive_analysis";
            
            if (lowerQuery.Contains("compare") || lowerQuery.Contains("versus"))
                return "comparative_analysis";
            
            if (lowerQuery.Contains("optimize") || lowerQuery.Contains("improve"))
                return "optimization_analysis";
                
            return "general_analysis";
        }

        private decimal CalculateClarityScore(string query)
        {
            decimal score = 0.5m; // Base score
            
            // Length penalty/bonus
            if (query.Length > 20 && query.Length < 200) score += 0.2m;
            if (query.Length < 10 || query.Length > 500) score -= 0.2m;
            
            // Question word bonus
            var questionWords = new[] { "what", "how", "when", "where", "why", "which", "who" };
            if (questionWords.Any(w => query.ToLower().Contains(w))) score += 0.1m;
            
            // Specific terminology bonus
            var specificTerms = new[] { "rate", "budget", "revenue", "expense", "customer", "utility" };
            score += specificTerms.Count(t => query.ToLower().Contains(t)) * 0.05m;
            
            // Grammar and structure
            if (query.Contains("?")) score += 0.1m;
            if (query.Split(' ').Length > 3) score += 0.05m;
            
            return Math.Min(1.0m, Math.Max(0.1m, score));
        }

        private int CalculateComplexityScore(string query)
        {
            int complexity = 3; // Base complexity
            
            // Word count influence
            var wordCount = query.Split(' ').Length;
            complexity += wordCount / 10;
            
            // Complex concepts
            var complexConcepts = new[] { "optimization", "prediction", "analysis", "comparison", "modeling" };
            complexity += complexConcepts.Count(c => query.ToLower().Contains(c));
            
            // Multiple questions/scenarios
            if (query.Count(c => c == '?') > 1) complexity += 2;
            if (query.ToLower().Contains("and")) complexity += 1;
            
            return Math.Min(10, Math.Max(1, complexity));
        }

        private bool ContainsRateKeywords(string lowerQuery)
        {
            var rateKeywords = new[] { "rate", "price", "cost", "fee", "charge", "tariff", "billing" };
            return rateKeywords.Any(k => lowerQuery.Contains(k));
        }

        private bool ContainsPrecisionKeywords(string lowerQuery)
        {
            var precisionKeywords = new[] { "exact", "precise", "calculate", "compute", "specific", "dollar" };
            return precisionKeywords.Any(k => lowerQuery.Contains(k));
        }

        private bool ContainsVisionKeywords(string lowerQuery)
        {
            var visionKeywords = new[] { "chart", "graph", "image", "picture", "document", "report" };
            return visionKeywords.Any(k => lowerQuery.Contains(k));
        }

        private List<string> ExtractKeyConcepts(string query)
        {
            var concepts = new List<string>();
            var lowerQuery = query.ToLower();
            
            var conceptMap = new Dictionary<string, string[]>
            {
                ["financial"] = new[] { "budget", "revenue", "expense", "cost", "money", "financial" },
                ["customer"] = new[] { "customer", "resident", "user", "client", "account" },
                ["utility"] = new[] { "water", "sewer", "trash", "utility", "service" },
                ["temporal"] = new[] { "monthly", "annual", "seasonal", "year", "month", "time" },
                ["performance"] = new[] { "efficiency", "performance", "optimization", "improvement" }
            };
            
            foreach (var category in conceptMap)
            {
                if (category.Value.Any(term => lowerQuery.Contains(term)))
                {
                    concepts.Add(category.Key);
                }
            }
            
            return concepts.Any() ? concepts : new List<string> { "general" };
        }

        private List<string> DetermineRequiredData(List<string> keyConcepts)
        {
            var requiredData = new List<string>();
            
            if (keyConcepts.Contains("financial"))
                requiredData.AddRange(new[] { "budget_data", "revenue_data", "expense_data" });
            
            if (keyConcepts.Contains("customer"))
                requiredData.AddRange(new[] { "customer_count", "usage_data", "demographic_data" });
            
            if (keyConcepts.Contains("utility"))
                requiredData.AddRange(new[] { "service_data", "infrastructure_data", "operational_data" });
            
            if (keyConcepts.Contains("temporal"))
                requiredData.AddRange(new[] { "historical_data", "trend_data", "seasonal_data" });
            
            return requiredData.Distinct().ToList();
        }

        private decimal CalculateCustomerImpact(RateOptimizationResult prediction, EnterpriseContext enterpriseData)
        {
            // Calculate customer impact score (1-10 scale)
            var currentRate = enterpriseData.RequiredRate;
            var proposedRate = (prediction.MinRate + prediction.MaxRate) / 2;
            
            if (currentRate <= 0) return 5.0m; // Neutral impact if no current rate
            
            var changePercent = Math.Abs((proposedRate - currentRate) / currentRate);
            
            // Impact scoring: lower change = lower impact (better for customers)
            if (changePercent < 0.05m) return 9.0m; // Very low impact
            if (changePercent < 0.10m) return 7.0m; // Low impact  
            if (changePercent < 0.20m) return 5.0m; // Moderate impact
            if (changePercent < 0.30m) return 3.0m; // High impact
            return 1.0m; // Very high impact
        }

        private decimal CalculateRevenueOptimization(RateOptimizationResult prediction, EnterpriseContext enterpriseData)
        {
            // Calculate revenue optimization score (1-10 scale)
            var targetRevenue = enterpriseData.TotalExpenses; // Break-even target
            var proposedRate = (prediction.MinRate + prediction.MaxRate) / 2;
            var projectedRevenue = proposedRate * enterpriseData.CustomerBase * 12; // Annual revenue
            
            if (targetRevenue <= 0) return 5.0m;
            
            var revenueRatio = projectedRevenue / targetRevenue;
            
            // Optimization scoring: closer to optimal revenue = higher score
            if (revenueRatio >= 1.20m) return 9.0m; // Strong revenue optimization
            if (revenueRatio >= 1.10m) return 8.0m; // Good revenue optimization
            if (revenueRatio >= 1.05m) return 7.0m; // Moderate revenue optimization
            if (revenueRatio >= 1.00m) return 6.0m; // Break-even
            if (revenueRatio >= 0.95m) return 4.0m; // Slight revenue shortfall
            return 2.0m; // Significant revenue shortfall
        }

        private string GenerateRateRecommendationSummary(RateOptimizationResult prediction)
        {
            var avgRate = (prediction.MinRate + prediction.MaxRate) / 2;
            var summary = new System.Text.StringBuilder();
            
            summary.AppendLine($"Optimal rate range: ${prediction.MinRate:F2} - ${prediction.MaxRate:F2}");
            summary.AppendLine($"Recommended rate: ${avgRate:F2}");
            
            if (prediction.SeasonalAdjustment != 0)
            {
                summary.AppendLine($"Seasonal adjustment: {prediction.SeasonalAdjustment:F1}%");
            }
            
            if (prediction.Confidence > 0.8m)
            {
                summary.AppendLine("High confidence in optimization results");
            }
            else if (prediction.Confidence > 0.6m)
            {
                summary.AppendLine("Moderate confidence - consider additional data");
            }
            else
            {
                summary.AppendLine("Low confidence - recommend careful monitoring");
            }
            
            return summary.ToString();
        }

        private SeasonalForecast GenerateSimpleSeasonalForecast(
            List<HistoricalRateData> historicalData, 
            int forecastMonths, 
            string errorMessage)
        {
            // Generate simple trend-based forecast when ML fails
            var monthlyForecasts = new List<MonthlyForecast>();
            
            decimal baseValue = historicalData.Any() 
                ? historicalData.Average(d => d.Revenue) 
                : 10000m; // Default base value
            
            for (int i = 1; i <= forecastMonths; i++)
            {
                var seasonalFactor = 1.0m + (decimal)(0.1 * Math.Sin(2 * Math.PI * i / 12)); // Simple seasonal pattern
                var forecastValue = baseValue * seasonalFactor;
                
                monthlyForecasts.Add(new MonthlyForecast
                {
                    Month = DateTime.Now.AddMonths(i),
                    ForecastValue = forecastValue,
                    LowerBound = forecastValue * 0.9m,
                    UpperBound = forecastValue * 1.1m,
                    Confidence = 0.5m
                });
            }
            
            return new SeasonalForecast
            {
                ForecastPeriodMonths = forecastMonths,
                MonthlyForecasts = monthlyForecasts,
                OverallConfidence = 0.4m,
                KeyDrivers = new List<string> { "Simple trend analysis", $"Note: {errorMessage}" },
                RiskFactors = new List<string> { "Limited forecasting capability due to technical issues" }
            };
        }

        private List<string> IdentifyForecastRisks(SeasonalForecastResult forecast)
        {
            var risks = new List<string>();
            
            if (forecast.ModelConfidence < 0.6m)
                risks.Add("Low model confidence - forecast uncertainty high");
            
            if (forecast.TrendComponent < -0.1m)
                risks.Add("Declining trend detected - revenue may decrease");
            
            if (forecast.IrregularComponent > 0.2m)
                risks.Add("High irregular variation - unpredictable fluctuations possible");
            
            var highVariabilityMonths = forecast.MonthlyPredictions
                .Where(p => Math.Abs(p.UpperBound - p.LowerBound) / p.ForecastValue > 0.3m)
                .Count();
            
            if (highVariabilityMonths > forecast.MonthlyPredictions.Count / 2)
                risks.Add("High forecast variability - consider additional data sources");
            
            return risks;
        }

        private List<string> GenerateMitigationStrategies(CustomerBehaviorResult prediction)
        {
            var strategies = new List<string>();
            
            if (prediction.ChurnRisk > 10.0m)
                strategies.Add("Implement customer retention program");
            
            if (prediction.AffordabilityIndex < 0.7m)
                strategies.Add("Consider payment assistance programs");
            
            if (prediction.UsageChangePercent < -10.0m)
                strategies.Add("Plan for reduced usage revenue impact");
            
            if (prediction.PriceElasticity < -1.0m)
                strategies.Add("Rate changes will significantly impact usage - phase implementation");
            
            return strategies.Any() ? strategies : new List<string> { "Monitor customer response closely" };
        }

        #endregion

        public void Dispose()
        {
            _featureExtractor?.Dispose();
            _timeSeriesAnalyzer?.Dispose();
            
            foreach (var model in _trainedModels.Values)
            {
                model?.Dispose();
            }
        }
    }

    #region Supporting Classes

    public class FeatureExtractor : IDisposable
    {
        public RateOptimizationFeatures ExtractRateFeatures(EnterpriseContext data, QueryIntent intent)
        {
            return new RateOptimizationFeatures
            {
                CurrentRate = data.RequiredRate,
                CustomerCount = data.CustomerBase,
                TotalRevenue = data.TotalRevenue,
                TotalExpenses = data.TotalExpenses,
                AffordabilityIndex = data.CustomerAffordabilityIndex,
                SeasonalAdjustment = data.SeasonalAdjustment,
                QueryComplexity = intent.ComplexityScore,
                RequiresPrecision = intent.RequiresPrecision
            };
        }

        public TimeSeriesFeatures ExtractTimeSeriesFeatures(List<HistoricalRateData> data)
        {
            if (!data.Any()) return new TimeSeriesFeatures();
            
            return new TimeSeriesFeatures
            {
                DataPoints = data.Count,
                TimeSpan = data.Max(d => d.Date) - data.Min(d => d.Date),
                AverageValue = data.Average(d => d.Revenue),
                Variance = CalculateVariance(data.Select(d => d.Revenue).ToList()),
                TrendDirection = CalculateTrend(data),
                SeasonalIndicators = ExtractSeasonalIndicators(data)
            };
        }

        public CustomerBehaviorFeatures ExtractBehaviorFeatures(
            EnterpriseContext data, 
            List<HistoricalRateData> history, 
            RateOptimizationGoals goals)
        {
            return new CustomerBehaviorFeatures
            {
                CustomerBase = data.CustomerBase,
                CurrentAffordability = data.CustomerAffordabilityIndex,
                HistoricalElasticity = CalculateHistoricalElasticity(history),
                SocioeconomicFactors = ExtractSocioeconomicFactors(data),
                ServiceQualityIndex = EstimateServiceQuality(data),
                CompetitiveLandscape = AssessCompetitiveLandscape(data)
            };
        }

        public AnomalyDetectionFeatures ExtractAnomalyFeatures(
            List<FinancialDataPoint> data, 
            AnomalyDetectionConfig config)
        {
            return new AnomalyDetectionFeatures
            {
                TimeSeriesLength = data.Count,
                SamplingFrequency = DetermineSamplingFrequency(data),
                BaselineWindow = config.BaselineWindow,
                SensitivityLevel = config.SensitivityLevel,
                StatisticalMoments = CalculateStatisticalMoments(data)
            };
        }

        #region Helper Methods

        private decimal CalculateVariance(List<decimal> values)
        {
            if (values.Count < 2) return 0;
            
            var mean = values.Average();
            var sumSquaredDeviations = values.Sum(x => (x - mean) * (x - mean));
            return sumSquaredDeviations / (values.Count - 1);
        }

        private decimal CalculateTrend(List<HistoricalRateData> data)
        {
            if (data.Count < 2) return 0;
            
            var orderedData = data.OrderBy(d => d.Date).ToList();
            var firstHalf = orderedData.Take(orderedData.Count / 2).Average(d => d.Revenue);
            var secondHalf = orderedData.Skip(orderedData.Count / 2).Average(d => d.Revenue);
            
            return firstHalf != 0 ? (secondHalf - firstHalf) / firstHalf : 0;
        }

        private List<decimal> ExtractSeasonalIndicators(List<HistoricalRateData> data)
        {
            var monthlyAverages = new decimal[12];
            var monthlyCounts = new int[12];
            
            foreach (var point in data)
            {
                var month = point.Date.Month - 1; // 0-based indexing
                monthlyAverages[month] += point.Revenue;
                monthlyCounts[month]++;
            }
            
            for (int i = 0; i < 12; i++)
            {
                if (monthlyCounts[i] > 0)
                    monthlyAverages[i] /= monthlyCounts[i];
            }
            
            return monthlyAverages.ToList();
        }

        private decimal CalculateHistoricalElasticity(List<HistoricalRateData> history)
        {
            // Simple elasticity estimation
            if (history.Count < 2) return -0.5m; // Default elasticity
            
            var rateChanges = new List<decimal>();
            var usageChanges = new List<decimal>();
            
            for (int i = 1; i < history.Count; i++)
            {
                var prevData = history[i - 1];
                var currData = history[i];
                
                if (prevData.Rate > 0 && prevData.Usage > 0)
                {
                    var rateChange = (currData.Rate - prevData.Rate) / prevData.Rate;
                    var usageChange = (currData.Usage - prevData.Usage) / prevData.Usage;
                    
                    rateChanges.Add(rateChange);
                    usageChanges.Add(usageChange);
                }
            }
            
            if (rateChanges.Count < 2) return -0.5m;
            
            // Simple correlation-based elasticity
            var avgRateChange = rateChanges.Average();
            var avgUsageChange = usageChanges.Average();
            
            return avgRateChange != 0 ? avgUsageChange / avgRateChange : -0.5m;
        }

        private Dictionary<string, decimal> ExtractSocioeconomicFactors(EnterpriseContext data)
        {
            return new Dictionary<string, decimal>
            {
                ["AffordabilityIndex"] = data.CustomerAffordabilityIndex,
                ["ServiceAreaSize"] = data.CustomerBase,
                ["UtilityBurden"] = data.RequiredRate > 0 ? data.RequiredRate / 100 : 0.1m // Simplified burden calculation
            };
        }

        private decimal EstimateServiceQuality(EnterpriseContext data)
        {
            // Simple service quality estimation based on financial health
            var operationalEfficiency = data.TotalRevenue > 0 ? data.TotalExpenses / data.TotalRevenue : 1.0m;
            
            if (operationalEfficiency < 0.8m) return 9.0m; // Highly efficient = high quality
            if (operationalEfficiency < 1.0m) return 7.0m; // Efficient = good quality
            if (operationalEfficiency < 1.2m) return 5.0m; // Adequate = average quality
            return 3.0m; // Inefficient = lower quality
        }

        private decimal AssessCompetitiveLandscape(EnterpriseContext data)
        {
            // For municipal utilities, competition is typically limited
            // Score based on market characteristics
            if (data.Name.Contains("Water") || data.Name.Contains("Sewer"))
                return 2.0m; // Low competition (natural monopoly)
            if (data.Name.Contains("Trash"))
                return 6.0m; // Moderate competition possible
            return 4.0m; // Default moderate competition
        }

        private string DetermineSamplingFrequency(List<FinancialDataPoint> data)
        {
            if (data.Count < 2) return "unknown";
            
            var intervals = new List<TimeSpan>();
            for (int i = 1; i < Math.Min(data.Count, 10); i++) // Sample first 10 intervals
            {
                intervals.Add(data[i].Timestamp - data[i - 1].Timestamp);
            }
            
            var avgInterval = intervals.Average(i => i.TotalDays);
            
            if (avgInterval <= 1.5) return "daily";
            if (avgInterval <= 7.5) return "weekly";
            if (avgInterval <= 31) return "monthly";
            if (avgInterval <= 366) return "annual";
            return "irregular";
        }

        private Dictionary<string, decimal> CalculateStatisticalMoments(List<FinancialDataPoint> data)
        {
            if (!data.Any()) return new Dictionary<string, decimal>();
            
            var values = data.Select(d => d.Value).ToList();
            var mean = values.Average();
            var variance = CalculateVariance(values);
            var stdDev = (decimal)Math.Sqrt((double)variance);
            
            // Calculate skewness and kurtosis
            var skewness = CalculateSkewness(values, mean, stdDev);
            var kurtosis = CalculateKurtosis(values, mean, stdDev);
            
            return new Dictionary<string, decimal>
            {
                ["Mean"] = mean,
                ["Variance"] = variance,
                ["StandardDeviation"] = stdDev,
                ["Skewness"] = skewness,
                ["Kurtosis"] = kurtosis
            };
        }

        private decimal CalculateSkewness(List<decimal> values, decimal mean, decimal stdDev)
        {
            if (stdDev == 0 || values.Count < 3) return 0;
            
            var n = values.Count;
            var sumCubedDeviations = values.Sum(x => (decimal)Math.Pow((double)((x - mean) / stdDev), 3));
            
            return (n * sumCubedDeviations) / ((n - 1) * (n - 2));
        }

        private decimal CalculateKurtosis(List<decimal> values, decimal mean, decimal stdDev)
        {
            if (stdDev == 0 || values.Count < 4) return 0;
            
            var n = values.Count;
            var sumQuarticDeviations = values.Sum(x => (decimal)Math.Pow((double)((x - mean) / stdDev), 4));
            
            var kurtosis = (n * (n + 1) * sumQuarticDeviations) / ((n - 1) * (n - 2) * (n - 3));
            var adjustment = (3 * (n - 1) * (n - 1)) / ((n - 2) * (n - 3));
            
            return kurtosis - adjustment; // Excess kurtosis
        }

        #endregion

        public void Dispose()
        {
            // Cleanup resources if needed
        }
    }

    public class TimeSeriesAnalyzer : IDisposable
    {
        public HistoricalPattern AnalyzeTrends(List<HistoricalRateData> data)
        {
            if (data.Count < 3) return null;
            
            var orderedData = data.OrderBy(d => d.Date).ToList();
            var trendStrength = CalculateTrendStrength(orderedData);
            var trendDirection = CalculateTrendDirection(orderedData);
            
            return new HistoricalPattern
            {
                PatternType = "trend",
                Description = $"{GetTrendDescription(trendDirection, trendStrength)}",
                Confidence = Math.Abs(trendStrength),
                Strength = Math.Abs(trendStrength),
                Direction = trendDirection > 0 ? "increasing" : "decreasing",
                Recommendations = GenerateTrendRecommendations(trendDirection, trendStrength)
            };
        }

        public List<HistoricalPattern> DetectSeasonalPatterns(List<HistoricalRateData> data)
        {
            var patterns = new List<HistoricalPattern>();
            
            if (data.Count < 12) return patterns; // Need at least a year of data
            
            var monthlyAnalysis = AnalyzeMonthlyPatterns(data);
            if (monthlyAnalysis != null) patterns.Add(monthlyAnalysis);
            
            var quarterlyAnalysis = AnalyzeQuarterlyPatterns(data);
            if (quarterlyAnalysis != null) patterns.Add(quarterlyAnalysis);
            
            return patterns;
        }

        public HistoricalPattern AnalyzeVolatility(List<HistoricalRateData> data)
        {
            if (data.Count < 5) return null;
            
            var values = data.Select(d => d.Revenue).ToList();
            var volatility = CalculateVolatility(values);
            var volatilityLevel = ClassifyVolatility(volatility);
            
            return new HistoricalPattern
            {
                PatternType = "volatility",
                Description = $"Revenue volatility is {volatilityLevel}",
                Confidence = 0.8m,
                Strength = volatility,
                Recommendations = GenerateVolatilityRecommendations(volatilityLevel, volatility)
            };
        }

        public HistoricalPattern AnalyzeGrowthPatterns(List<HistoricalRateData> data)
        {
            if (data.Count < 6) return null;
            
            var growthRates = CalculateGrowthRates(data);
            var avgGrowthRate = growthRates.Average();
            var growthStability = CalculateGrowthStability(growthRates);
            
            return new HistoricalPattern
            {
                PatternType = "growth",
                Description = $"Average growth rate: {avgGrowthRate:P2}, Stability: {GetStabilityDescription(growthStability)}",
                Confidence = growthStability,
                Strength = Math.Abs(avgGrowthRate),
                Direction = avgGrowthRate > 0 ? "positive" : "negative",
                Recommendations = GenerateGrowthRecommendations(avgGrowthRate, growthStability)
            };
        }

        #region Helper Methods

        private decimal CalculateTrendStrength(List<HistoricalRateData> orderedData)
        {
            // Linear regression slope as trend strength
            var n = orderedData.Count;
            var sumX = 0m;
            var sumY = 0m;
            var sumXY = 0m;
            var sumX2 = 0m;
            
            for (int i = 0; i < n; i++)
            {
                var x = i; // Time index
                var y = orderedData[i].Revenue;
                
                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
            }
            
            var slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            return slope / (sumY / n); // Normalized slope
        }

        private decimal CalculateTrendDirection(List<HistoricalRateData> orderedData)
        {
            var firstThird = orderedData.Take(orderedData.Count / 3).Average(d => d.Revenue);
            var lastThird = orderedData.Skip(2 * orderedData.Count / 3).Average(d => d.Revenue);
            
            return lastThird - firstThird;
        }

        private string GetTrendDescription(decimal direction, decimal strength)
        {
            var directionText = direction > 0 ? "increasing" : "decreasing";
            var strengthText = Math.Abs(strength) > 0.1m ? "strong" : 
                              Math.Abs(strength) > 0.05m ? "moderate" : "weak";
            
            return $"{strengthText} {directionText} trend detected";
        }

        private List<string> GenerateTrendRecommendations(decimal direction, decimal strength)
        {
            var recommendations = new List<string>();
            
            if (direction > 0 && Math.Abs(strength) > 0.1m)
            {
                recommendations.Add("Strong positive trend - consider rate stabilization strategies");
                recommendations.Add("Monitor for sustainability of growth trend");
            }
            else if (direction < 0 && Math.Abs(strength) > 0.1m)
            {
                recommendations.Add("Strong negative trend - investigate underlying causes");
                recommendations.Add("Consider revenue enhancement strategies");
            }
            else
            {
                recommendations.Add("Stable trend - maintain current strategies");
            }
            
            return recommendations;
        }

        private HistoricalPattern AnalyzeMonthlyPatterns(List<HistoricalRateData> data)
        {
            var monthlyAvgs = new decimal[12];
            var monthlyCounts = new int[12];
            
            foreach (var point in data)
            {
                var month = point.Date.Month - 1;
                monthlyAvgs[month] += point.Revenue;
                monthlyCounts[month]++;
            }
            
            for (int i = 0; i < 12; i++)
            {
                if (monthlyCounts[i] > 0)
                    monthlyAvgs[i] /= monthlyCounts[i];
            }
            
            var overallAvg = monthlyAvgs.Where(a => a > 0).Average();
            var seasonalStrength = monthlyAvgs.Where(a => a > 0).Max() / overallAvg - 1;
            
            if (seasonalStrength > 0.1m) // 10% seasonal variation threshold
            {
                return new HistoricalPattern
                {
                    PatternType = "monthly_seasonal",
                    Description = $"Monthly seasonal pattern detected with {seasonalStrength:P1} variation",
                    Confidence = Math.Min(0.9m, seasonalStrength * 5), // Higher variation = higher confidence
                    Strength = seasonalStrength,
                    Recommendations = new List<string> 
                    { 
                        "Account for seasonal revenue variations in budgeting",
                        "Consider seasonal rate adjustments if appropriate"
                    }
                };
            }
            
            return null;
        }

        private HistoricalPattern AnalyzeQuarterlyPatterns(List<HistoricalRateData> data)
        {
            var quarterlyAvgs = new decimal[4];
            var quarterlyCounts = new int[4];
            
            foreach (var point in data)
            {
                var quarter = (point.Date.Month - 1) / 3;
                quarterlyAvgs[quarter] += point.Revenue;
                quarterlyCounts[quarter]++;
            }
            
            for (int i = 0; i < 4; i++)
            {
                if (quarterlyCounts[i] > 0)
                    quarterlyAvgs[i] /= quarterlyCounts[i];
            }
            
            var overallAvg = quarterlyAvgs.Where(a => a > 0).Average();
            var seasonalStrength = quarterlyAvgs.Where(a => a > 0).Max() / overallAvg - 1;
            
            if (seasonalStrength > 0.15m) // 15% quarterly variation threshold
            {
                return new HistoricalPattern
                {
                    PatternType = "quarterly_seasonal",
                    Description = $"Quarterly seasonal pattern detected with {seasonalStrength:P1} variation",
                    Confidence = Math.Min(0.8m, seasonalStrength * 3),
                    Strength = seasonalStrength,
                    Recommendations = new List<string> 
                    { 
                        "Plan for quarterly revenue fluctuations",
                        "Align expense timing with revenue patterns"
                    }
                };
            }
            
            return null;
        }

        private decimal CalculateVolatility(List<decimal> values)
        {
            if (values.Count < 2) return 0;
            
            var mean = values.Average();
            var variance = values.Sum(v => (v - mean) * (v - mean)) / values.Count;
            var stdDev = (decimal)Math.Sqrt((double)variance);
            
            return mean > 0 ? stdDev / mean : 0; // Coefficient of variation
        }

        private string ClassifyVolatility(decimal volatility)
        {
            if (volatility < 0.05m) return "very low";
            if (volatility < 0.10m) return "low";
            if (volatility < 0.20m) return "moderate";
            if (volatility < 0.30m) return "high";
            return "very high";
        }

        private List<string> GenerateVolatilityRecommendations(string volatilityLevel, decimal volatility)
        {
            return volatilityLevel switch
            {
                "very low" => new List<string> { "Revenue is very stable - maintain current approach" },
                "low" => new List<string> { "Revenue is stable - good foundation for planning" },
                "moderate" => new List<string> { "Some revenue variation - monitor trends closely" },
                "high" => new List<string> { "High revenue volatility - implement risk management strategies", "Consider reserve fund for revenue fluctuations" },
                "very high" => new List<string> { "Very high volatility - urgent need for stabilization strategies", "Investigate causes of revenue instability" },
                _ => new List<string> { "Monitor revenue patterns for stability" }
            };
        }

        private List<decimal> CalculateGrowthRates(List<HistoricalRateData> data)
        {
            var growthRates = new List<decimal>();
            var orderedData = data.OrderBy(d => d.Date).ToList();
            
            for (int i = 1; i < orderedData.Count; i++)
            {
                var prevRevenue = orderedData[i - 1].Revenue;
                var currRevenue = orderedData[i].Revenue;
                
                if (prevRevenue > 0)
                {
                    var growthRate = (currRevenue - prevRevenue) / prevRevenue;
                    growthRates.Add(growthRate);
                }
            }
            
            return growthRates;
        }

        private decimal CalculateGrowthStability(List<decimal> growthRates)
        {
            if (growthRates.Count < 2) return 0.5m;
            
            var volatility = CalculateVolatility(growthRates);
            return Math.Max(0.1m, 1.0m - volatility * 5); // Higher volatility = lower stability
        }

        private string GetStabilityDescription(decimal stability)
        {
            if (stability > 0.8m) return "very stable";
            if (stability > 0.6m) return "stable";
            if (stability > 0.4m) return "moderate";
            if (stability > 0.2m) return "unstable";
            return "very unstable";
        }

        private List<string> GenerateGrowthRecommendations(decimal avgGrowthRate, decimal stability)
        {
            var recommendations = new List<string>();
            
            if (avgGrowthRate > 0.05m && stability > 0.6m)
            {
                recommendations.Add("Consistent positive growth - good financial trajectory");
                recommendations.Add("Consider infrastructure investments to support growth");
            }
            else if (avgGrowthRate < -0.05m)
            {
                recommendations.Add("Declining growth trend - investigate root causes");
                recommendations.Add("Implement revenue recovery strategies");
            }
            else if (stability < 0.4m)
            {
                recommendations.Add("Unstable growth pattern - focus on stabilization");
                recommendations.Add("Identify factors causing growth volatility");
            }
            else
            {
                recommendations.Add("Stable moderate growth - maintain current approach");
            }
            
            return recommendations;
        }

        #endregion

        public void Dispose()
        {
            // Cleanup resources if needed
        }
    }

    #endregion
}
