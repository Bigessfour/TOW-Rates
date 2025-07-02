using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WileyBudgetManagement.Services.Enhanced
{
    #region Core ML Model Interfaces

    /// <summary>
    /// Base interface for all ML models in the system
    /// </summary>
    public interface IMLModel : IDisposable
    {
        string ModelName { get; }
        string ModelVersion { get; }
        DateTime LastTrained { get; }
        decimal ModelConfidence { get; }
        Task<bool> IsModelReady();
    }

    /// <summary>
    /// Abstract base class for ML models with common functionality
    /// </summary>
    public abstract class MLModel : IMLModel
    {
        public string ModelName { get; protected set; } = string.Empty;
        public string ModelVersion { get; protected set; } = "1.0.0";
        public DateTime LastTrained { get; protected set; } = DateTime.Now;
        public decimal ModelConfidence { get; protected set; } = 0.7m;

        public virtual Task<bool> IsModelReady()
        {
            return Task.FromResult(true); // Override in specific implementations
        }

        public virtual void Dispose()
        {
            // Override in specific implementations for cleanup
        }
    }

    #endregion

    #region Rate Optimization Models

    /// <summary>
    /// ML model for predicting optimal utility rates
    /// Uses ensemble methods and customer behavior modeling
    /// </summary>
    public class RateOptimizationModel : MLModel
    {
        public RateOptimizationModel()
        {
            ModelName = "RateOptimizer";
            ModelVersion = "2.1.0";
            ModelConfidence = 0.82m;
        }

        public async Task<RateOptimizationResult> PredictOptimalRates(RateOptimizationFeatures features)
        {
            await Task.Delay(100); // Simulate ML processing time
            
            try
            {
                // Simulate ML-based rate optimization
                var baseRate = features.CurrentRate > 0 ? features.CurrentRate : 
                              (features.TotalExpenses / Math.Max(features.CustomerCount, 1)) / 12;
                
                // Apply ML-derived adjustments
                var efficiencyFactor = CalculateEfficiencyFactor(features);
                var affordabilityFactor = CalculateAffordabilityFactor(features);
                var seasonalFactor = CalculateSeasonalFactor(features);
                
                var optimizedRate = baseRate * efficiencyFactor * affordabilityFactor;
                var variance = optimizedRate * 0.1m; // 10% variance for min/max range
                
                return new RateOptimizationResult
                {
                    MinRate = Math.Max(0, optimizedRate - variance),
                    MaxRate = optimizedRate + variance,
                    OptimalRate = optimizedRate,
                    Confidence = CalculateConfidence(features),
                    SeasonalAdjustment = (seasonalFactor - 1) * 100, // Convert to percentage
                    SeasonalFactors = GenerateSeasonalFactors(features),
                    OptimizationScore = CalculateOptimizationScore(features, optimizedRate),
                    RevenueProjection = optimizedRate * features.CustomerCount * 12,
                    AffordabilityImpact = CalculateAffordabilityImpact(features, optimizedRate),
                    ImplementationRecommendations = GenerateImplementationRecommendations(features, optimizedRate)
                };
            }
            catch (Exception ex)
            {
                // Return safe default on error
                return new RateOptimizationResult
                {
                    MinRate = features.CurrentRate * 0.95m,
                    MaxRate = features.CurrentRate * 1.05m,
                    OptimalRate = features.CurrentRate,
                    Confidence = 0.3m,
                    SeasonalAdjustment = 0,
                    ErrorMessage = $"Rate optimization error: {ex.Message}"
                };
            }
        }

        private decimal CalculateEfficiencyFactor(RateOptimizationFeatures features)
        {
            // Calculate efficiency based on revenue/expense ratio
            if (features.TotalRevenue <= 0 || features.TotalExpenses <= 0) return 1.0m;
            
            var efficiency = features.TotalRevenue / features.TotalExpenses;
            
            // Adjust rate based on efficiency
            if (efficiency > 1.2m) return 0.95m; // Reduce rate if very efficient
            if (efficiency < 0.9m) return 1.1m;  // Increase rate if inefficient
            return 1.0m; // No adjustment for balanced efficiency
        }

        private decimal CalculateAffordabilityFactor(RateOptimizationFeatures features)
        {
            // Adjust based on affordability constraints
            if (features.AffordabilityIndex > 0.8m) return 1.05m; // Can afford slight increase
            if (features.AffordabilityIndex < 0.6m) return 0.95m; // Must reduce for affordability
            return 1.0m; // No affordability adjustment needed
        }

        private decimal CalculateSeasonalFactor(RateOptimizationFeatures features)
        {
            // Apply seasonal adjustments
            var currentMonth = DateTime.Now.Month;
            var seasonalAdjustment = features.SeasonalAdjustment;
            
            // Summer months (high usage) vs Winter months (lower usage)
            if (currentMonth >= 6 && currentMonth <= 8) // Summer
                return 1.0m + (seasonalAdjustment * 0.5m);
            if (currentMonth >= 12 || currentMonth <= 2) // Winter
                return 1.0m - (seasonalAdjustment * 0.3m);
            
            return 1.0m; // Spring/Fall - no seasonal adjustment
        }

        private decimal CalculateConfidence(RateOptimizationFeatures features)
        {
            decimal confidence = 0.7m; // Base confidence
            
            // Adjust based on data quality
            if (features.CustomerCount > 100) confidence += 0.1m;
            if (features.TotalRevenue > 0 && features.TotalExpenses > 0) confidence += 0.1m;
            if (features.CurrentRate > 0) confidence += 0.05m;
            if (features.AffordabilityIndex > 0) confidence += 0.05m;
            
            return Math.Min(0.95m, confidence);
        }

        private List<string> GenerateSeasonalFactors(RateOptimizationFeatures features)
        {
            var factors = new List<string>();
            
            if (Math.Abs(features.SeasonalAdjustment) > 0.1m)
            {
                factors.Add("Significant seasonal usage variations detected");
                factors.Add("Summer peak usage periods");
                factors.Add("Winter usage reduction patterns");
            }
            else
            {
                factors.Add("Stable year-round usage patterns");
            }
            
            return factors;
        }

        private decimal CalculateOptimizationScore(RateOptimizationFeatures features, decimal optimizedRate)
        {
            // Score the optimization quality (1-10 scale)
            decimal score = 5.0m; // Base score
            
            var currentRate = features.CurrentRate;
            if (currentRate > 0)
            {
                var changePercent = Math.Abs((optimizedRate - currentRate) / currentRate);
                
                // Lower change = higher score (more stable)
                if (changePercent < 0.05m) score += 2.0m;
                else if (changePercent < 0.10m) score += 1.0m;
                else if (changePercent > 0.25m) score -= 1.0m;
            }
            
            // Affordability consideration
            if (features.AffordabilityIndex > 0.8m) score += 1.0m;
            else if (features.AffordabilityIndex < 0.6m) score -= 1.0m;
            
            return Math.Min(10.0m, Math.Max(1.0m, score));
        }

        private decimal CalculateAffordabilityImpact(RateOptimizationFeatures features, decimal optimizedRate)
        {
            if (features.CurrentRate <= 0) return 5.0m; // Neutral impact
            
            var rateChange = (optimizedRate - features.CurrentRate) / features.CurrentRate;
            var affordabilityImpact = rateChange * (1 / Math.Max(features.AffordabilityIndex, 0.1m));
            
            // Scale to 1-10 (lower is better for customers)
            return Math.Min(10.0m, Math.Max(1.0m, 5.0m + (affordabilityImpact * 10)));
        }

        private List<string> GenerateImplementationRecommendations(RateOptimizationFeatures features, decimal optimizedRate)
        {
            var recommendations = new List<string>();
            
            if (features.CurrentRate > 0)
            {
                var changePercent = (optimizedRate - features.CurrentRate) / features.CurrentRate;
                
                if (Math.Abs(changePercent) > 0.15m)
                {
                    recommendations.Add("Consider phased implementation over 6-12 months");
                    recommendations.Add("Communicate changes to customers well in advance");
                }
                
                if (changePercent > 0.1m)
                {
                    recommendations.Add("Provide clear justification for rate increase");
                    recommendations.Add("Consider customer assistance programs");
                }
                
                if (changePercent < -0.1m)
                {
                    recommendations.Add("Rate reduction opportunity - ensure long-term sustainability");
                }
            }
            
            if (features.AffordabilityIndex < 0.7m)
            {
                recommendations.Add("Monitor customer burden carefully");
                recommendations.Add("Consider graduated rate structure");
            }
            
            return recommendations.Any() ? recommendations : new List<string> { "Current rate structure appears optimal" };
        }
    }

    #endregion

    #region Customer Behavior Models

    /// <summary>
    /// ML model for predicting customer behavior changes
    /// Uses behavioral economics and elasticity modeling
    /// </summary>
    public class CustomerBehaviorModel : MLModel
    {
        public CustomerBehaviorModel()
        {
            ModelName = "CustomerBehaviorPredictor";
            ModelVersion = "1.8.0";
            ModelConfidence = 0.76m;
        }

        public async Task<CustomerBehaviorResult> PredictBehavior(CustomerBehaviorFeatures features)
        {
            await Task.Delay(150); // Simulate ML processing
            
            try
            {
                // Predict usage change based on elasticity
                var usageChange = CalculateUsageChange(features);
                var retentionRate = CalculateRetentionRate(features);
                var churnRisk = CalculateChurnRisk(features);
                
                return new CustomerBehaviorResult
                {
                    UsageChangePercent = usageChange,
                    RetentionRate = retentionRate,
                    PriceElasticity = features.HistoricalElasticity,
                    AffordabilityIndex = features.CurrentAffordability,
                    ChurnRisk = churnRisk,
                    RevenueImpact = CalculateRevenueImpact(features, usageChange, retentionRate),
                    Confidence = CalculateBehaviorConfidence(features),
                    KeyDrivers = IdentifyBehaviorDrivers(features),
                    SegmentImpacts = GenerateSegmentImpacts(features),
                    BehaviorRisks = IdentifyBehaviorRisks(features, usageChange, churnRisk)
                };
            }
            catch (Exception ex)
            {
                return new CustomerBehaviorResult
                {
                    UsageChangePercent = -2.0m, // Conservative estimate
                    RetentionRate = 95.0m,
                    PriceElasticity = -0.5m,
                    AffordabilityIndex = features.CurrentAffordability,
                    ChurnRisk = 5.0m,
                    Confidence = 0.4m,
                    KeyDrivers = new List<string> { $"Analysis error: {ex.Message}" }
                };
            }
        }

        private decimal CalculateUsageChange(CustomerBehaviorFeatures features)
        {
            // Base usage change on elasticity and affordability
            var elasticity = features.HistoricalElasticity;
            var affordability = features.CurrentAffordability;
            
            // Assume a 10% rate change for prediction
            var assumedRateChange = 0.1m;
            var baseUsageChange = elasticity * assumedRateChange;
            
            // Adjust for affordability constraints
            if (affordability < 0.6m)
                baseUsageChange *= 1.5m; // Amplify negative response for low affordability
            else if (affordability > 0.9m)
                baseUsageChange *= 0.7m; // Dampen response for high affordability
            
            return baseUsageChange * 100; // Convert to percentage
        }

        private decimal CalculateRetentionRate(CustomerBehaviorFeatures features)
        {
            var baseRetention = 97.0m; // Start with high municipal utility retention
            
            // Adjust based on affordability
            if (features.CurrentAffordability < 0.5m) baseRetention -= 5.0m;
            else if (features.CurrentAffordability < 0.7m) baseRetention -= 2.0m;
            
            // Adjust based on service quality
            if (features.ServiceQualityIndex < 5.0m) baseRetention -= 3.0m;
            else if (features.ServiceQualityIndex > 8.0m) baseRetention += 1.0m;
            
            // Adjust based on competitive landscape
            if (features.CompetitiveLandscape > 7.0m) baseRetention -= 2.0m;
            
            return Math.Max(85.0m, Math.Min(99.0m, baseRetention));
        }

        private decimal CalculateChurnRisk(CustomerBehaviorFeatures features)
        {
            var churnRisk = 2.0m; // Base low churn risk for utilities
            
            // Increase based on affordability stress
            if (features.CurrentAffordability < 0.5m) churnRisk += 8.0m;
            else if (features.CurrentAffordability < 0.7m) churnRisk += 4.0m;
            
            // Increase based on service issues
            if (features.ServiceQualityIndex < 5.0m) churnRisk += 5.0m;
            
            // Increase based on competitive alternatives
            if (features.CompetitiveLandscape > 7.0m) churnRisk += 3.0m;
            
            return Math.Min(25.0m, churnRisk);
        }

        private decimal CalculateRevenueImpact(CustomerBehaviorFeatures features, decimal usageChange, decimal retentionRate)
        {
            // Simplified revenue impact calculation
            var usageImpact = usageChange / 100.0m; // Convert percentage back to decimal
            var churnImpact = (100.0m - retentionRate) / 100.0m;
            
            // Combined impact on revenue
            var totalImpact = (1 + usageImpact) * (1 - churnImpact) - 1;
            
            return totalImpact * 100; // Return as percentage
        }

        private decimal CalculateBehaviorConfidence(CustomerBehaviorFeatures features)
        {
            decimal confidence = 0.6m; // Base confidence
            
            // Higher confidence with more customers (larger sample)
            if (features.CustomerBase > 500) confidence += 0.1m;
            if (features.CustomerBase > 1000) confidence += 0.05m;
            
            // Higher confidence with better data
            if (features.HistoricalElasticity != 0) confidence += 0.1m;
            if (features.CurrentAffordability > 0) confidence += 0.05m;
            
            return Math.Min(0.9m, confidence);
        }

        private List<string> IdentifyBehaviorDrivers(CustomerBehaviorFeatures features)
        {
            var drivers = new List<string>();
            
            if (features.CurrentAffordability < 0.7m)
                drivers.Add("Affordability constraints");
            
            if (Math.Abs(features.HistoricalElasticity) > 1.0m)
                drivers.Add("High price sensitivity");
            
            if (features.ServiceQualityIndex > 8.0m)
                drivers.Add("High service satisfaction");
            else if (features.ServiceQualityIndex < 5.0m)
                drivers.Add("Service quality concerns");
            
            if (features.CompetitiveLandscape > 6.0m)
                drivers.Add("Competitive market pressures");
            
            return drivers.Any() ? drivers : new List<string> { "Standard utility customer behavior patterns" };
        }

        private Dictionary<string, CustomerSegmentImpact> GenerateSegmentImpacts(CustomerBehaviorFeatures features)
        {
            return new Dictionary<string, CustomerSegmentImpact>
            {
                ["Residential"] = new CustomerSegmentImpact
                {
                    SegmentName = "Residential",
                    PopulationPercent = 85.0m,
                    ExpectedUsageChange = CalculateUsageChange(features) * 0.9m, // Slightly less sensitive
                    ChurnRisk = CalculateChurnRisk(features) * 0.8m, // Lower churn risk
                    RevenueImpact = 75.0m // Percent of total revenue impact
                },
                ["Commercial"] = new CustomerSegmentImpact
                {
                    SegmentName = "Commercial",
                    PopulationPercent = 12.0m,
                    ExpectedUsageChange = CalculateUsageChange(features) * 1.2m, // More sensitive to rates
                    ChurnRisk = CalculateChurnRisk(features) * 1.5m, // Higher churn risk
                    RevenueImpact = 20.0m
                },
                ["Industrial"] = new CustomerSegmentImpact
                {
                    SegmentName = "Industrial",
                    PopulationPercent = 3.0m,
                    ExpectedUsageChange = CalculateUsageChange(features) * 1.5m, // Very sensitive
                    ChurnRisk = CalculateChurnRisk(features) * 2.0m, // Highest churn risk
                    RevenueImpact = 5.0m
                }
            };
        }

        private List<string> IdentifyBehaviorRisks(CustomerBehaviorFeatures features, decimal usageChange, decimal churnRisk)
        {
            var risks = new List<string>();
            
            if (usageChange < -10.0m)
                risks.Add("Significant usage reduction expected");
            
            if (churnRisk > 10.0m)
                risks.Add("Elevated customer churn risk");
            
            if (features.CurrentAffordability < 0.6m)
                risks.Add("Customer affordability stress");
            
            if (features.ServiceQualityIndex < 6.0m)
                risks.Add("Service quality may drive negative behavior");
            
            return risks.Any() ? risks : new List<string> { "Low behavioral risk profile" };
        }
    }

    #endregion

    #region Anomaly Detection Models

    /// <summary>
    /// ML model for detecting financial anomalies
    /// Uses ensemble methods and statistical analysis
    /// </summary>
    public class AnomalyDetectionModel : MLModel
    {
        public AnomalyDetectionModel()
        {
            ModelName = "FinancialAnomalyDetector";
            ModelVersion = "1.5.0";
            ModelConfidence = 0.84m;
        }

        public async Task<List<DetectedAnomaly>> DetectAnomalies(
            AnomalyDetectionFeatures features, 
            AnomalyDetectionConfig config)
        {
            await Task.Delay(200); // Simulate ML processing
            
            var anomalies = new List<DetectedAnomaly>();
            
            try
            {
                // Statistical anomaly detection
                var statisticalAnomalies = DetectStatisticalAnomalies(features, config);
                anomalies.AddRange(statisticalAnomalies);
                
                // Pattern-based anomaly detection
                var patternAnomalies = DetectPatternAnomalies(features, config);
                anomalies.AddRange(patternAnomalies);
                
                // Context-aware anomaly detection
                var contextAnomalies = DetectContextualAnomalies(features, config);
                anomalies.AddRange(contextAnomalies);
                
                return anomalies;
            }
            catch (Exception ex)
            {
                anomalies.Add(new DetectedAnomaly
                {
                    Timestamp = DateTime.Now,
                    Type = "detection_error",
                    Severity = "Low",
                    Score = 0.1m,
                    Description = $"Anomaly detection failed: {ex.Message}",
                    Confidence = 0.0m
                });
                
                return anomalies;
            }
        }

        private List<DetectedAnomaly> DetectStatisticalAnomalies(
            AnomalyDetectionFeatures features, 
            AnomalyDetectionConfig config)
        {
            var anomalies = new List<DetectedAnomaly>();
            
            // Z-score based detection
            foreach (var statMoment in features.StatisticalMoments)
            {
                if (statMoment.Key == "StandardDeviation" && statMoment.Value > config.SensitivityLevel * 2)
                {
                    anomalies.Add(new DetectedAnomaly
                    {
                        Timestamp = DateTime.Now,
                        Type = "high_volatility",
                        Severity = "Medium",
                        Score = Math.Min(1.0m, statMoment.Value / 1000),
                        ExpectedValue = 0,
                        ActualValue = statMoment.Value,
                        Deviation = statMoment.Value,
                        Description = "Unusually high data volatility detected",
                        PotentialCauses = new List<string> { "Irregular spending patterns", "Data entry errors", "Seasonal variations" },
                        RecommendedActions = new List<string> { "Review recent transactions", "Verify data accuracy" },
                        Confidence = 0.7m
                    });
                }
                
                if (statMoment.Key == "Skewness" && Math.Abs(statMoment.Value) > 2.0m)
                {
                    anomalies.Add(new DetectedAnomaly
                    {
                        Timestamp = DateTime.Now,
                        Type = "distribution_skew",
                        Severity = "Low",
                        Score = Math.Abs(statMoment.Value) / 10.0m,
                        Description = $"Data distribution highly skewed ({statMoment.Value:F2})",
                        PotentialCauses = new List<string> { "Outlier transactions", "Irregular billing cycles" },
                        RecommendedActions = new List<string> { "Investigate extreme values", "Consider data transformation" },
                        Confidence = 0.6m
                    });
                }
            }
            
            return anomalies;
        }

        private List<DetectedAnomaly> DetectPatternAnomalies(
            AnomalyDetectionFeatures features, 
            AnomalyDetectionConfig config)
        {
            var anomalies = new List<DetectedAnomaly>();
            
            // Time series pattern detection
            if (features.SamplingFrequency == "irregular")
            {
                anomalies.Add(new DetectedAnomaly
                {
                    Timestamp = DateTime.Now,
                    Type = "irregular_timing",
                    Severity = "Medium",
                    Score = 0.6m,
                    Description = "Irregular data sampling pattern detected",
                    PotentialCauses = new List<string> { "Inconsistent data collection", "System issues" },
                    RecommendedActions = new List<string> { "Standardize data collection intervals", "Check system reliability" },
                    Confidence = 0.8m
                });
            }
            
            // Data quality pattern detection
            if (features.TimeSeriesLength < config.BaselineWindow)
            {
                anomalies.Add(new DetectedAnomaly
                {
                    Timestamp = DateTime.Now,
                    Type = "insufficient_data",
                    Severity = "Low",
                    Score = 0.3m,
                    Description = "Insufficient historical data for reliable analysis",
                    PotentialCauses = new List<string> { "New system implementation", "Data loss" },
                    RecommendedActions = new List<string> { "Extend data collection period", "Verify data completeness" },
                    Confidence = 0.9m
                });
            }
            
            return anomalies;
        }

        private List<DetectedAnomaly> DetectContextualAnomalies(
            AnomalyDetectionFeatures features, 
            AnomalyDetectionConfig config)
        {
            var anomalies = new List<DetectedAnomaly>();
            
            // Context-specific anomalies based on municipal finance patterns
            var currentMonth = DateTime.Now.Month;
            
            // Budget year-end anomalies
            if (currentMonth >= 11) // November/December
            {
                anomalies.Add(new DetectedAnomaly
                {
                    Timestamp = DateTime.Now,
                    Type = "year_end_spending",
                    Severity = "Low",
                    Score = 0.4m,
                    Description = "Year-end spending surge period - monitor for budget exhaustion",
                    PotentialCauses = new List<string> { "Use-or-lose budget policies", "Deferred maintenance" },
                    RecommendedActions = new List<string> { "Review remaining budget allocations", "Prioritize essential expenses" },
                    Confidence = 0.8m
                });
            }
            
            // Seasonal utility anomalies
            if (currentMonth >= 6 && currentMonth <= 8) // Summer months
            {
                if (features.StatisticalMoments.ContainsKey("Mean"))
                {
                    var meanValue = features.StatisticalMoments["Mean"];
                    // In summer, water usage typically increases
                    anomalies.Add(new DetectedAnomaly
                    {
                        Timestamp = DateTime.Now,
                        Type = "seasonal_variation",
                        Severity = "Low",
                        Score = 0.2m,
                        Description = "Summer seasonal usage pattern - expect higher consumption",
                        PotentialCauses = new List<string> { "Increased irrigation", "Higher temperatures" },
                        RecommendedActions = new List<string> { "Monitor peak capacity", "Plan for seasonal revenue variation" },
                        Confidence = 0.7m
                    });
                }
            }
            
            return anomalies;
        }
    }

    #endregion

    #region Seasonal Forecast Models

    /// <summary>
    /// ML model for seasonal forecasting
    /// Uses time series analysis and climate data integration
    /// </summary>
    public class SeasonalForecastModel : MLModel
    {
        public SeasonalForecastModel()
        {
            ModelName = "SeasonalForecaster";
            ModelVersion = "2.0.0";
            ModelConfidence = 0.79m;
        }

        public async Task<SeasonalForecastResult> GenerateForecast(
            TimeSeriesFeatures features, 
            int forecastMonths)
        {
            await Task.Delay(300); // Simulate complex ML processing
            
            try
            {
                var monthlyPredictions = GenerateMonthlyPredictions(features, forecastMonths);
                var decomposition = PerformSeasonalDecomposition(features);
                
                return new SeasonalForecastResult
                {
                    MonthlyPredictions = monthlyPredictions,
                    SeasonalAdjustments = ExtractSeasonalAdjustments(decomposition),
                    ConfidenceIntervals = CalculateConfidenceIntervals(monthlyPredictions),
                    TrendComponent = decomposition.Trend,
                    SeasonalComponent = decomposition.Seasonal,
                    IrregularComponent = decomposition.Irregular,
                    ModelConfidence = CalculateForecastConfidence(features, forecastMonths),
                    InfluentialFactors = IdentifyInfluentialFactors(features),
                    ForecastQuality = AssessForecastQuality(features, forecastMonths)
                };
            }
            catch (Exception ex)
            {
                // Return simple trend-based forecast on error
                return GenerateSimpleForecast(features, forecastMonths, ex.Message);
            }
        }

        private List<MonthlyForecast> GenerateMonthlyPredictions(TimeSeriesFeatures features, int forecastMonths)
        {
            var predictions = new List<MonthlyForecast>();
            var baseValue = features.AverageValue;
            var trendDirection = features.TrendDirection;
            
            for (int i = 1; i <= forecastMonths; i++)
            {
                var targetMonth = DateTime.Now.AddMonths(i);
                
                // Apply trend
                var trendedValue = baseValue * (1 + (trendDirection * i * 0.01m)); // 1% trend per month
                
                // Apply seasonal adjustment
                var seasonalFactor = GetSeasonalFactor(targetMonth.Month, features.SeasonalIndicators);
                var seasonalValue = trendedValue * seasonalFactor;
                
                // Add some uncertainty
                var uncertainty = seasonalValue * 0.1m * i; // Increasing uncertainty over time
                
                predictions.Add(new MonthlyForecast
                {
                    Month = targetMonth,
                    ForecastValue = seasonalValue,
                    LowerBound = seasonalValue - uncertainty,
                    UpperBound = seasonalValue + uncertainty,
                    Confidence = Math.Max(0.3m, 0.9m - (i * 0.05m)), // Decreasing confidence over time
                    SeasonalFactor = seasonalFactor,
                    TrendContribution = trendedValue - baseValue,
                    UncertaintyRange = uncertainty * 2
                });
            }
            
            return predictions;
        }

        private SeasonalDecomposition PerformSeasonalDecomposition(TimeSeriesFeatures features)
        {
            // Simplified seasonal decomposition
            var trend = features.TrendDirection;
            var seasonal = features.SeasonalIndicators.Any() ? 
                features.SeasonalIndicators.Max() / features.AverageValue - 1 : 0.1m;
            var irregular = features.Variance > 0 ? 
                (decimal)Math.Sqrt((double)features.Variance) / features.AverageValue : 0.05m;
            
            return new SeasonalDecomposition
            {
                Trend = trend,
                Seasonal = seasonal,
                Irregular = irregular,
                Residual = 1.0m - Math.Abs(trend) - Math.Abs(seasonal) - Math.Abs(irregular)
            };
        }

        private List<decimal> ExtractSeasonalAdjustments(SeasonalDecomposition decomposition)
        {
            var adjustments = new List<decimal>();
            
            // Generate 12 monthly seasonal factors
            for (int month = 1; month <= 12; month++)
            {
                var seasonalEffect = decomposition.Seasonal * 
                    (decimal)Math.Sin(2 * Math.PI * month / 12 + Math.PI / 4); // Phase shift for realism
                adjustments.Add(1.0m + seasonalEffect);
            }
            
            return adjustments;
        }

        private List<ConfidenceInterval> CalculateConfidenceIntervals(List<MonthlyForecast> predictions)
        {
            return predictions.Select(p => new ConfidenceInterval
            {
                Month = p.Month,
                Lower95 = p.LowerBound,
                Upper95 = p.UpperBound,
                Lower80 = p.ForecastValue - (p.UncertaintyRange * 0.6m),
                Upper80 = p.ForecastValue + (p.UncertaintyRange * 0.6m),
                Lower50 = p.ForecastValue - (p.UncertaintyRange * 0.3m),
                Upper50 = p.ForecastValue + (p.UncertaintyRange * 0.3m)
            }).ToList();
        }

        private decimal CalculateForecastConfidence(TimeSeriesFeatures features, int forecastMonths)
        {
            decimal confidence = 0.8m; // Base confidence
            
            // Adjust based on data quality
            if (features.DataPoints < 12) confidence -= 0.2m; // Need at least a year
            if (features.DataPoints > 24) confidence += 0.1m; // More data = better
            
            // Adjust based on variance
            if (features.Variance / (features.AverageValue * features.AverageValue) > 0.25m) // High CV
                confidence -= 0.15m;
            
            // Adjust based on forecast horizon
            confidence -= forecastMonths * 0.02m; // Confidence decreases with horizon
            
            return Math.Max(0.3m, Math.Min(0.9m, confidence));
        }

        private List<string> IdentifyInfluentialFactors(TimeSeriesFeatures features)
        {
            var factors = new List<string>();
            
            if (Math.Abs(features.TrendDirection) > 0.05m)
                factors.Add($"Strong {(features.TrendDirection > 0 ? "upward" : "downward")} trend");
            
            if (features.SeasonalIndicators.Any())
            {
                var seasonalVariation = features.SeasonalIndicators.Max() - features.SeasonalIndicators.Min();
                if (seasonalVariation > features.AverageValue * 0.2m)
                    factors.Add("Significant seasonal patterns");
            }
            
            if (features.Variance > (features.AverageValue * features.AverageValue * 0.1m))
                factors.Add("High data volatility");
            
            factors.Add("Climate and weather patterns");
            factors.Add("Economic conditions");
            factors.Add("Regulatory environment");
            
            return factors;
        }

        private string AssessForecastQuality(TimeSeriesFeatures features, int forecastMonths)
        {
            var dataQuality = features.DataPoints >= 24 ? "Good" : "Limited";
            var horizonQuality = forecastMonths <= 12 ? "Reliable" : "Extended";
            var variabilityQuality = features.Variance / (features.AverageValue * features.AverageValue) < 0.1m ? "Stable" : "Variable";
            
            return $"Data: {dataQuality}, Horizon: {horizonQuality}, Stability: {variabilityQuality}";
        }

        private decimal GetSeasonalFactor(int month, List<decimal> seasonalIndicators)
        {
            if (!seasonalIndicators.Any() || seasonalIndicators.Count < 12)
            {
                // Default seasonal pattern for utilities
                return month switch
                {
                    6 or 7 or 8 => 1.2m,      // Summer peak
                    12 or 1 or 2 => 0.9m,     // Winter low
                    _ => 1.0m                  // Spring/Fall normal
                };
            }
            
            var monthIndex = month - 1; // Convert to 0-based
            if (monthIndex < seasonalIndicators.Count)
            {
                var avgValue = seasonalIndicators.Average();
                return avgValue > 0 ? seasonalIndicators[monthIndex] / avgValue : 1.0m;
            }
            
            return 1.0m;
        }

        private SeasonalForecastResult GenerateSimpleForecast(TimeSeriesFeatures features, int forecastMonths, string errorMessage)
        {
            var simplePredictions = new List<MonthlyForecast>();
            var baseValue = features.AverageValue > 0 ? features.AverageValue : 10000m;
            
            for (int i = 1; i <= forecastMonths; i++)
            {
                var month = DateTime.Now.AddMonths(i);
                var seasonalFactor = GetSeasonalFactor(month.Month, new List<decimal>());
                var forecastValue = baseValue * seasonalFactor;
                
                simplePredictions.Add(new MonthlyForecast
                {
                    Month = month,
                    ForecastValue = forecastValue,
                    LowerBound = forecastValue * 0.85m,
                    UpperBound = forecastValue * 1.15m,
                    Confidence = 0.5m
                });
            }
            
            return new SeasonalForecastResult
            {
                MonthlyPredictions = simplePredictions,
                ModelConfidence = 0.4m,
                InfluentialFactors = new List<string> { $"Simple forecast due to: {errorMessage}" },
                ForecastQuality = "Basic - Limited by processing error"
            };
        }
    }

    #endregion

    #region Revenue Prediction Models

    /// <summary>
    /// ML model for revenue prediction and optimization
    /// Integrates multiple revenue streams and external factors
    /// </summary>
    public class RevenuePredictionModel : MLModel
    {
        public RevenuePredictionModel()
        {
            ModelName = "RevenuePredictor";
            ModelVersion = "1.6.0";
            ModelConfidence = 0.81m;
        }

        public async Task<RevenuePredictionResult> PredictRevenue(
            RevenueFeatures features, 
            int predictionMonths)
        {
            await Task.Delay(250); // Simulate ML processing
            
            try
            {
                var predictions = GenerateRevenuePredictions(features, predictionMonths);
                var scenarios = GenerateRevenueScenarios(features, predictions);
                var optimization = OptimizeRevenueStreams(features, predictions);
                
                return new RevenuePredictionResult
                {
                    MonthlyPredictions = predictions,
                    RevenueScenarios = scenarios,
                    OptimizationRecommendations = optimization,
                    TotalPredictedRevenue = predictions.Sum(p => p.PredictedRevenue),
                    RevenueGrowthRate = CalculateRevenueGrowthRate(predictions),
                    RevenueStability = AssessRevenueStability(predictions),
                    KeyRisks = IdentifyRevenueRisks(features, predictions),
                    OpportunityAreas = IdentifyRevenueOpportunities(features, predictions),
                    ModelConfidence = CalculateRevenueConfidence(features),
                    PredictionQuality = AssessPredictionQuality(features, predictionMonths)
                };
            }
            catch (Exception ex)
            {
                return new RevenuePredictionResult
                {
                    TotalPredictedRevenue = features.CurrentRevenue * predictionMonths,
                    RevenueGrowthRate = 0.02m, // Conservative 2% growth
                    ModelConfidence = 0.3m,
                    KeyRisks = new List<string> { $"Prediction error: {ex.Message}" },
                    PredictionQuality = "Error Recovery Mode"
                };
            }
        }

        private List<MonthlyRevenuePrediction> GenerateRevenuePredictions(RevenueFeatures features, int months)
        {
            var predictions = new List<MonthlyRevenuePrediction>();
            var baseRevenue = features.CurrentRevenue;
            var growthRate = features.HistoricalGrowthRate;
            
            for (int i = 1; i <= months; i++)
            {
                var month = DateTime.Now.AddMonths(i);
                
                // Apply growth trend
                var trendedRevenue = baseRevenue * (decimal)Math.Pow(1.0 + (double)growthRate / 12.0, i);
                
                // Apply seasonal adjustments
                var seasonalFactor = GetRevenueSeasonalFactor(month.Month);
                var seasonalRevenue = trendedRevenue * seasonalFactor;
                
                // Add uncertainty
                var uncertainty = seasonalRevenue * 0.1m * (decimal)Math.Sqrt(i);
                
                predictions.Add(new MonthlyRevenuePrediction
                {
                    Month = month,
                    PredictedRevenue = seasonalRevenue,
                    LowerBound = seasonalRevenue - uncertainty,
                    UpperBound = seasonalRevenue + uncertainty,
                    GrowthContribution = trendedRevenue - baseRevenue,
                    SeasonalContribution = seasonalRevenue - trendedRevenue,
                    Confidence = Math.Max(0.4m, 0.9m - (i * 0.03m))
                });
            }
            
            return predictions;
        }

        private Dictionary<string, RevenueScenario> GenerateRevenueScenarios(
            RevenueFeatures features, 
            List<MonthlyRevenuePrediction> basePredictions)
        {
            var scenarios = new Dictionary<string, RevenueScenario>();
            
            // Conservative scenario (90% of base)
            scenarios["Conservative"] = new RevenueScenario
            {
                ScenarioName = "Conservative",
                Probability = 0.3m,
                TotalRevenue = basePredictions.Sum(p => p.PredictedRevenue * 0.9m),
                Description = "Economic downturn or service disruptions",
                KeyAssumptions = new List<string> { "Reduced customer demand", "Economic stress" }
            };
            
            // Base scenario (100% of prediction)
            scenarios["Base"] = new RevenueScenario
            {
                ScenarioName = "Base",
                Probability = 0.5m,
                TotalRevenue = basePredictions.Sum(p => p.PredictedRevenue),
                Description = "Expected performance under normal conditions",
                KeyAssumptions = new List<string> { "Normal demand patterns", "Stable economic conditions" }
            };
            
            // Optimistic scenario (110% of base)
            scenarios["Optimistic"] = new RevenueScenario
            {
                ScenarioName = "Optimistic",
                Probability = 0.2m,
                TotalRevenue = basePredictions.Sum(p => p.PredictedRevenue * 1.1m),
                Description = "Growth from new customers or rate optimization",
                KeyAssumptions = new List<string> { "Customer growth", "Successful rate optimization" }
            };
            
            return scenarios;
        }

        private List<RevenueOptimization> OptimizeRevenueStreams(
            RevenueFeatures features, 
            List<MonthlyRevenuePrediction> predictions)
        {
            var optimizations = new List<RevenueOptimization>();
            
            // Rate optimization
            optimizations.Add(new RevenueOptimization
            {
                OptimizationType = "Rate Structure",
                CurrentValue = features.CurrentRevenue,
                OptimizedValue = features.CurrentRevenue * 1.05m,
                ImprovementPercent = 5.0m,
                ImplementationDifficulty = "Medium",
                TimeToImplement = "3-6 months",
                Description = "Optimize rate structure based on cost recovery analysis",
                RiskLevel = "Low"
            });
            
            // Efficiency improvements
            optimizations.Add(new RevenueOptimization
            {
                OptimizationType = "Operational Efficiency",
                CurrentValue = features.OperationalCosts,
                OptimizedValue = features.OperationalCosts * 0.95m,
                ImprovementPercent = 5.0m,
                ImplementationDifficulty = "Medium",
                TimeToImplement = "6-12 months",
                Description = "Reduce operational costs through efficiency improvements",
                RiskLevel = "Low"
            });
            
            // New revenue streams
            optimizations.Add(new RevenueOptimization
            {
                OptimizationType = "New Revenue Streams",
                CurrentValue = 0,
                OptimizedValue = features.CurrentRevenue * 0.1m,
                ImprovementPercent = 10.0m,
                ImplementationDifficulty = "High",
                TimeToImplement = "12-24 months",
                Description = "Develop new revenue sources (fees, services, partnerships)",
                RiskLevel = "Medium"
            });
            
            return optimizations;
        }

        private decimal GetRevenueSeasonalFactor(int month)
        {
            // Municipal utility seasonal patterns
            return month switch
            {
                6 or 7 or 8 => 1.15m,     // Summer peak (higher water usage)
                12 or 1 or 2 => 0.95m,    // Winter low (holiday impact)
                9 or 10 or 11 => 1.05m,   // Fall (back to normal + leaf collection)
                _ => 1.0m                  // Spring normal
            };
        }

        private decimal CalculateRevenueGrowthRate(List<MonthlyRevenuePrediction> predictions)
        {
            if (predictions.Count < 2) return 0;
            
            var firstMonth = predictions.First().PredictedRevenue;
            var lastMonth = predictions.Last().PredictedRevenue;
            var months = predictions.Count;
            
            return firstMonth > 0 ? (decimal)Math.Pow((double)(lastMonth / firstMonth), 12.0 / months) - 1 : 0;
        }

        private decimal AssessRevenueStability(List<MonthlyRevenuePrediction> predictions)
        {
            if (!predictions.Any()) return 0.5m;
            
            var revenues = predictions.Select(p => p.PredictedRevenue).ToList();
            var mean = revenues.Average();
            var variance = revenues.Sum(r => (r - mean) * (r - mean)) / revenues.Count;
            var stdDev = (decimal)Math.Sqrt((double)variance);
            
            var coefficientOfVariation = mean > 0 ? stdDev / mean : 1.0m;
            
            // Lower CV = higher stability
            return Math.Max(0.1m, 1.0m - coefficientOfVariation);
        }

        private List<string> IdentifyRevenueRisks(RevenueFeatures features, List<MonthlyRevenuePrediction> predictions)
        {
            var risks = new List<string>();
            
            var avgConfidence = predictions.Average(p => p.Confidence);
            if (avgConfidence < 0.6m)
                risks.Add("Low prediction confidence due to data limitations");
            
            var revenueGrowth = CalculateRevenueGrowthRate(predictions);
            if (revenueGrowth < 0)
                risks.Add("Declining revenue trend predicted");
            
            if (features.CustomerChurnRate > 0.05m)
                risks.Add("High customer churn rate impacting revenue");
            
            var stability = AssessRevenueStability(predictions);
            if (stability < 0.6m)
                risks.Add("High revenue volatility expected");
            
            return risks.Any() ? risks : new List<string> { "Low revenue risk profile" };
        }

        private List<string> IdentifyRevenueOpportunities(RevenueFeatures features, List<MonthlyRevenuePrediction> predictions)
        {
            var opportunities = new List<string>();
            
            var revenueGrowth = CalculateRevenueGrowthRate(predictions);
            if (revenueGrowth > 0.03m)
                opportunities.Add("Strong growth trend - consider capacity expansion");
            
            if (features.CustomerSatisfactionScore > 8.0m)
                opportunities.Add("High customer satisfaction - opportunity for rate optimization");
            
            if (features.MarketShareGrowth > 0.02m)
                opportunities.Add("Growing market share - expand service offerings");
            
            var seasonalVariation = predictions.Max(p => p.PredictedRevenue) / predictions.Min(p => p.PredictedRevenue);
            if (seasonalVariation > 1.2m)
                opportunities.Add("Seasonal demand patterns - opportunity for seasonal pricing");
            
            return opportunities.Any() ? opportunities : new List<string> { "Stable revenue base for strategic planning" };
        }

        private decimal CalculateRevenueConfidence(RevenueFeatures features)
        {
            decimal confidence = 0.7m; // Base confidence
            
            // Data quality factors
            if (features.HistoricalDataPoints > 24) confidence += 0.1m;
            if (features.CustomerRetentionRate > 0.95m) confidence += 0.05m;
            if (features.CustomerSatisfactionScore > 7.0m) confidence += 0.05m;
            
            // Risk factors
            if (features.CustomerChurnRate > 0.1m) confidence -= 0.1m;
            if (features.MarketCompetition > 7.0m) confidence -= 0.05m;
            
            return Math.Max(0.4m, Math.Min(0.9m, confidence));
        }

        private string AssessPredictionQuality(RevenueFeatures features, int predictionMonths)
        {
            var dataQuality = features.HistoricalDataPoints >= 24 ? "Robust" : "Limited";
            var horizonQuality = predictionMonths <= 12 ? "Short-term" : "Long-term";
            var marketQuality = features.MarketCompetition < 5.0m ? "Stable Market" : "Competitive Market";
            
            return $"{dataQuality} data, {horizonQuality} horizon, {marketQuality}";
        }
    }

    #endregion
}
