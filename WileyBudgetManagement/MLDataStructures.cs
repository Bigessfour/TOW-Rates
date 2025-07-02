using System;
using System.Collections.Generic;
using System.Linq;

namespace WileyBudgetManagement.Services.Enhanced
{
    #region Feature Classes for ML Models

    /// <summary>
    /// Features for rate optimization ML model
    /// </summary>
    public class RateOptimizationFeatures
    {
        public decimal CurrentRate { get; set; }
        public int CustomerCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal AffordabilityIndex { get; set; }
        public decimal SeasonalAdjustment { get; set; }
        public int QueryComplexity { get; set; }
        public bool RequiresPrecision { get; set; }
        public decimal HistoricalVariance { get; set; }
        public List<decimal> UsagePatterns { get; set; } = new List<decimal>();
        public Dictionary<string, decimal> RegionalFactors { get; set; } = new Dictionary<string, decimal>();
        // Additional properties needed by RealTimeRateOptimizer
        public decimal TargetRevenue { get; set; }
        public string ServiceType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Features for time series analysis
    /// </summary>
    public class TimeSeriesFeatures
    {
        public int DataPoints { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public decimal AverageValue { get; set; }
        public decimal Variance { get; set; }
        public decimal TrendDirection { get; set; }
        public List<decimal> SeasonalIndicators { get; set; } = new List<decimal>();
        public decimal Autocorrelation { get; set; }
        public decimal Stationarity { get; set; }
        public string DataQuality { get; set; } = string.Empty;
    }

    /// <summary>
    /// Features for customer behavior prediction
    /// </summary>
    public class CustomerBehaviorFeatures
    {
        public int CustomerBase { get; set; }
        public decimal CurrentAffordability { get; set; }
        public decimal HistoricalElasticity { get; set; }
        public Dictionary<string, decimal> SocioeconomicFactors { get; set; } = new Dictionary<string, decimal>();
        public decimal ServiceQualityIndex { get; set; }
        public decimal CompetitiveLandscape { get; set; }
        public List<CustomerSegment> CustomerSegments { get; set; } = new List<CustomerSegment>();
        public decimal CustomerSatisfactionScore { get; set; }
        public decimal CustomerRetentionRate { get; set; }
    }

    /// <summary>
    /// Features for anomaly detection
    /// </summary>
    public class AnomalyDetectionFeatures
    {
        public int TimeSeriesLength { get; set; }
        public string SamplingFrequency { get; set; } = string.Empty;
        public int BaselineWindow { get; set; }
        public decimal SensitivityLevel { get; set; }
        public Dictionary<string, decimal> StatisticalMoments { get; set; } = new Dictionary<string, decimal>();
        public List<decimal> ControlLimits { get; set; } = new List<decimal>();
        public decimal NoiseLevel { get; set; }
        public string DataDistribution { get; set; } = string.Empty;
    }

    /// <summary>
    /// Features for revenue prediction
    /// </summary>
    public class RevenueFeatures
    {
        public decimal CurrentRevenue { get; set; }
        public decimal HistoricalGrowthRate { get; set; }
        public decimal OperationalCosts { get; set; }
        public decimal CustomerChurnRate { get; set; }
        public decimal CustomerSatisfactionScore { get; set; }
        public decimal MarketShareGrowth { get; set; }
        public decimal MarketCompetition { get; set; }
        public int HistoricalDataPoints { get; set; }
        public decimal CustomerRetentionRate { get; set; }
        public List<RevenueStream> RevenueStreams { get; set; } = new List<RevenueStream>();
        public Dictionary<string, decimal> EconomicIndicators { get; set; } = new Dictionary<string, decimal>();
    }

    #endregion

    #region Configuration Classes

    /// <summary>
    /// Configuration for anomaly detection
    /// </summary>
    public class AnomalyDetectionConfig
    {
        public decimal SensitivityLevel { get; set; } = 0.05m; // 5% significance level
        public int BaselineWindow { get; set; } = 30; // 30 data points for baseline
        public string DetectionMethod { get; set; } = "ensemble"; // statistical, ml, ensemble
        public bool EnableSeasonalAdjustment { get; set; } = true;
        public decimal OutlierThreshold { get; set; } = 3.0m; // Z-score threshold
        public List<string> IgnorePatterns { get; set; } = new List<string>();
        public bool EnableContextualDetection { get; set; } = true;
    }

    /// <summary>
    /// Goals for rate optimization
    /// </summary>
    public class RateOptimizationGoals
    {
        public decimal TargetRevenue { get; set; }
        public decimal MaxRateIncrease { get; set; } = 0.15m; // 15% max increase
        public decimal MinAffordabilityIndex { get; set; } = 0.7m; // 70% affordability threshold
        public bool OptimizeForEquity { get; set; } = true;
        public bool ConsiderSeasonality { get; set; } = true;
        public List<string> Constraints { get; set; } = new List<string>();
        public string OptimizationMethod { get; set; } = "balanced"; // revenue_max, cost_min, balanced
    }

    #endregion

    #region Result Classes

    /// <summary>
    /// Result from rate optimization model
    /// </summary>
    public class RateOptimizationResult
    {
        public decimal MinRate { get; set; }
        public decimal MaxRate { get; set; }
        public decimal OptimalRate { get; set; }
        public decimal Confidence { get; set; }
        public decimal SeasonalAdjustment { get; set; }
        public List<string> SeasonalFactors { get; set; } = new List<string>();
        public decimal OptimizationScore { get; set; }
        public decimal RevenueProjection { get; set; }
        public decimal AffordabilityImpact { get; set; }
        public List<string> ImplementationRecommendations { get; set; } = new List<string>();
        public string ErrorMessage { get; set; } = string.Empty;
        public Dictionary<string, decimal> SensitivityAnalysis { get; set; } = new Dictionary<string, decimal>();
        // Additional properties needed by RealTimeRateOptimizer
        public OptimizationScenario OptimalScenario { get; set; } = new OptimizationScenario();
        public List<OptimizationScenario> AlternativeScenarios { get; set; } = new List<OptimizationScenario>();
        public ImplementationPlan ImplementationPlan { get; set; } = new ImplementationPlan();
        public OptimizationConfidence ConfidenceMetrics { get; set; } = new OptimizationConfidence();
        public MonitoringPlan MonitoringPlan { get; set; } = new MonitoringPlan();
        public DateTime OptimizationTimestamp { get; set; } = DateTime.Now;
        public string ValidityPeriod { get; set; } = string.Empty;
        public RiskAssessment RiskAssessment { get; set; } = new RiskAssessment();
        public ExpectedOutcomes ExpectedOutcomes { get; set; } = new ExpectedOutcomes();
        public bool ComplianceValidation { get; set; }
    }

    /// <summary>
    /// Result from customer behavior prediction
    /// </summary>
    public class CustomerBehaviorResult
    {
        public decimal UsageChangePercent { get; set; }
        public decimal RetentionRate { get; set; }
        public decimal PriceElasticity { get; set; }
        public decimal AffordabilityIndex { get; set; }
        public decimal ChurnRisk { get; set; }
        public decimal RevenueImpact { get; set; }
        public decimal Confidence { get; set; }
        public List<string> KeyDrivers { get; set; } = new List<string>();
        public Dictionary<string, CustomerSegmentImpact> SegmentImpacts { get; set; } = new Dictionary<string, CustomerSegmentImpact>();
        public List<string> BehaviorRisks { get; set; } = new List<string>();
        public List<MitigationStrategy> RecommendedMitigations { get; set; } = new List<MitigationStrategy>();
    }

    /// <summary>
    /// Detected anomaly from ML model
    /// </summary>
    public class DetectedAnomaly
    {
        public DateTime Timestamp { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
        public decimal Score { get; set; } // 0-1 anomaly score
        public decimal ExpectedValue { get; set; }
        public decimal ActualValue { get; set; }
        public decimal Deviation { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> PotentialCauses { get; set; } = new List<string>();
        public List<string> RecommendedActions { get; set; } = new List<string>();
        public decimal Confidence { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Result from seasonal forecast model
    /// </summary>
    public class SeasonalForecastResult
    {
        public List<MonthlyForecast> MonthlyPredictions { get; set; } = new List<MonthlyForecast>();
        public List<decimal> SeasonalAdjustments { get; set; } = new List<decimal>();
        public List<ConfidenceInterval> ConfidenceIntervals { get; set; } = new List<ConfidenceInterval>();
        public decimal TrendComponent { get; set; }
        public decimal SeasonalComponent { get; set; }
        public decimal IrregularComponent { get; set; }
        public decimal ModelConfidence { get; set; }
        public List<string> InfluentialFactors { get; set; } = new List<string>();
        public string ForecastQuality { get; set; } = string.Empty;
        public Dictionary<string, decimal> ModelMetrics { get; set; } = new Dictionary<string, decimal>();
    }

    /// <summary>
    /// Result from revenue prediction model
    /// </summary>
    public class RevenuePredictionResult
    {
        public List<MonthlyRevenuePrediction> MonthlyPredictions { get; set; } = new List<MonthlyRevenuePrediction>();
        public Dictionary<string, RevenueScenario> RevenueScenarios { get; set; } = new Dictionary<string, RevenueScenario>();
        public List<RevenueOptimization> OptimizationRecommendations { get; set; } = new List<RevenueOptimization>();
        public decimal TotalPredictedRevenue { get; set; }
        public decimal RevenueGrowthRate { get; set; }
        public decimal RevenueStability { get; set; }
        public List<string> KeyRisks { get; set; } = new List<string>();
        public List<string> OpportunityAreas { get; set; } = new List<string>();
        public decimal ModelConfidence { get; set; }
        public string PredictionQuality { get; set; } = string.Empty;
    }

    #endregion

    #region Supporting Data Structures

    /// <summary>
    /// Customer segment definition
    /// </summary>
    public class CustomerSegment
    {
        public string SegmentName { get; set; } = string.Empty;
        public int CustomerCount { get; set; }
        public decimal AverageUsage { get; set; }
        public decimal AverageRevenue { get; set; }
        public decimal PriceElasticity { get; set; }
        public decimal AffordabilityIndex { get; set; }
        public List<string> Characteristics { get; set; } = new List<string>();
    }

    /// <summary>
    /// Impact on customer segment
    /// </summary>
    public class CustomerSegmentImpact
    {
        public string SegmentName { get; set; } = string.Empty;
        public decimal PopulationPercent { get; set; }
        public decimal ExpectedUsageChange { get; set; }
        public decimal ChurnRisk { get; set; }
        public decimal RevenueImpact { get; set; }
        public List<string> SpecificConcerns { get; set; } = new List<string>();
        public List<string> MitigationOpportunities { get; set; } = new List<string>();
    }

    /// <summary>
    /// Mitigation strategy for customer behavior risks
    /// </summary>
    public class MitigationStrategy
    {
        public string StrategyName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal EffectivenessScore { get; set; } // 1-10 scale
        public decimal ImplementationCost { get; set; }
        public string TimeToImplement { get; set; } = string.Empty;
        public List<string> RequiredResources { get; set; } = new List<string>();
        public decimal RiskReduction { get; set; } // Percentage risk reduction expected
    }

    /// <summary>
    /// Monthly forecast data point
    /// </summary>
    public class MonthlyForecast
    {
        public DateTime Month { get; set; }
        public decimal ForecastValue { get; set; }
        public decimal LowerBound { get; set; }
        public decimal UpperBound { get; set; }
        public decimal Confidence { get; set; }
        public decimal SeasonalFactor { get; set; }
        public decimal TrendContribution { get; set; }
        public decimal UncertaintyRange { get; set; }
        public Dictionary<string, decimal> ComponentBreakdown { get; set; } = new Dictionary<string, decimal>();
    }

    /// <summary>
    /// Confidence interval for forecasts
    /// </summary>
    public class ConfidenceInterval
    {
        public DateTime Month { get; set; }
        public decimal Lower95 { get; set; }
        public decimal Upper95 { get; set; }
        public decimal Lower80 { get; set; }
        public decimal Upper80 { get; set; }
        public decimal Lower50 { get; set; }
        public decimal Upper50 { get; set; }
        public decimal MedianForecast { get; set; }
    }

    /// <summary>
    /// Seasonal decomposition components
    /// </summary>
    public class SeasonalDecomposition
    {
        public decimal Trend { get; set; }
        public decimal Seasonal { get; set; }
        public decimal Irregular { get; set; }
        public decimal Residual { get; set; }
        public decimal R2 { get; set; } // Goodness of fit
        public List<decimal> TrendSeries { get; set; } = new List<decimal>();
        public List<decimal> SeasonalSeries { get; set; } = new List<decimal>();
    }

    /// <summary>
    /// Monthly revenue prediction
    /// </summary>
    public class MonthlyRevenuePrediction
    {
        public DateTime Month { get; set; }
        public decimal PredictedRevenue { get; set; }
        public decimal LowerBound { get; set; }
        public decimal UpperBound { get; set; }
        public decimal GrowthContribution { get; set; }
        public decimal SeasonalContribution { get; set; }
        public decimal Confidence { get; set; }
        public Dictionary<string, decimal> RevenueBySource { get; set; } = new Dictionary<string, decimal>();
    }

    /// <summary>
    /// Revenue scenario analysis
    /// </summary>
    public class RevenueScenario
    {
        public string ScenarioName { get; set; } = string.Empty;
        public decimal Probability { get; set; }
        public decimal TotalRevenue { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> KeyAssumptions { get; set; } = new List<string>();
        public Dictionary<string, decimal> ImpactFactors { get; set; } = new Dictionary<string, decimal>();
        public List<string> RiskMitigations { get; set; } = new List<string>();
    }

    /// <summary>
    /// Revenue optimization recommendation
    /// </summary>
    public class RevenueOptimization
    {
        public string OptimizationType { get; set; } = string.Empty;
        public decimal CurrentValue { get; set; }
        public decimal OptimizedValue { get; set; }
        public decimal ImprovementPercent { get; set; }
        public string ImplementationDifficulty { get; set; } = string.Empty;
        public string TimeToImplement { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RiskLevel { get; set; } = string.Empty;
        public List<string> Prerequisites { get; set; } = new List<string>();
        public decimal ROI { get; set; }
    }

    /// <summary>
    /// Revenue stream definition
    /// </summary>
    public class RevenueStream
    {
        public string Name { get; set; } = string.Empty;
        public decimal MonthlyAmount { get; set; }
        public decimal GrowthRate { get; set; }
        public decimal Volatility { get; set; }
        public string StreamType { get; set; } = string.Empty; // Fixed, Variable, One-time
        public decimal SeasonalFactor { get; set; } = 1.0m;
        public List<string> RiskFactors { get; set; } = new List<string>();
    }

    #endregion

    #region Historical Data Classes

    /// <summary>
    /// Historical rate data for analysis
    /// </summary>
    public class HistoricalRateData
    {
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
        public decimal Revenue { get; set; }
        public decimal Usage { get; set; }
        public int CustomerCount { get; set; }
        public decimal CustomerSatisfaction { get; set; }
        public string Notes { get; set; } = string.Empty;
        public Dictionary<string, decimal> AdditionalMetrics { get; set; } = new Dictionary<string, decimal>();
    }

    /// <summary>
    /// Historical pattern identified by ML
    /// </summary>
    public class HistoricalPattern
    {
        public string PatternType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Confidence { get; set; }
        public decimal Strength { get; set; }
        public string Direction { get; set; } = string.Empty;
        public List<string> Recommendations { get; set; } = new List<string>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Dictionary<string, decimal> PatternMetrics { get; set; } = new Dictionary<string, decimal>();
    }

    /// <summary>
    /// Financial data point for time series analysis
    /// </summary>
    public class FinancialDataPoint
    {
        public DateTime Timestamp { get; set; }
        public decimal Value { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
        public bool IsVerified { get; set; } = true;
        public decimal QualityScore { get; set; } = 1.0m;
    }

    #endregion

    #region Enhanced Response Classes

    /// <summary>
    /// Financial anomaly detected by the system
    /// </summary>
    public class FinancialAnomaly
    {
        public string AnomalyId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string AnomalyType { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public decimal AnomalyScore { get; set; }
        public decimal ExpectedValue { get; set; }
        public decimal ActualValue { get; set; }
        public decimal Deviation { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> PotentialCauses { get; set; } = new List<string>();
        public List<string> RecommendedActions { get; set; } = new List<string>();
        public decimal ConfidenceLevel { get; set; }
        public string Status { get; set; } = "New";
        public DateTime DetectedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Seasonal forecast for municipal planning
    /// </summary>
    public class SeasonalForecast
    {
        public int ForecastPeriodMonths { get; set; }
        public List<MonthlyForecast> MonthlyForecasts { get; set; } = new List<MonthlyForecast>();
        public List<decimal> SeasonalFactors { get; set; } = new List<decimal>();
        public List<ConfidenceInterval> ConfidenceIntervals { get; set; } = new List<ConfidenceInterval>();
        public decimal TrendComponent { get; set; }
        public decimal SeasonalComponent { get; set; }
        public decimal IrregularComponent { get; set; }
        public decimal OverallConfidence { get; set; }
        public List<string> KeyDrivers { get; set; } = new List<string>();
        public List<string> RiskFactors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Customer behavior prediction results
    /// </summary>
    public class CustomerBehaviorPrediction
    {
        public decimal ExpectedUsageChange { get; set; }
        public decimal CustomerRetentionRate { get; set; }
        public decimal PriceElasticity { get; set; }
        public decimal AffordabilityIndex { get; set; }
        public Dictionary<string, CustomerSegmentImpact> CustomerSegmentImpacts { get; set; } = new Dictionary<string, CustomerSegmentImpact>();
        public decimal ChurnRisk { get; set; }
        public decimal RevenueImpact { get; set; }
        public decimal ConfidenceLevel { get; set; }
        public List<string> BehaviorDrivers { get; set; } = new List<string>();
        public List<string> RecommendedMitigations { get; set; } = new List<string>();
    }

    #endregion

    #region Comprehensive Analysis Results

    /// <summary>
    /// Comprehensive predictive rate analysis
    /// </summary>
    public class PredictiveRateAnalysis
    {
        public bool Success { get; set; }
        public List<OptimizedRate> OptimizedRates { get; set; } = new List<OptimizedRate>();
        public List<HistoricalPattern> HistoricalPatterns { get; set; } = new List<HistoricalPattern>();
        public SeasonalForecast SeasonalForecast { get; set; } = new SeasonalForecast();
        public CustomerBehaviorPrediction CustomerBehaviorPrediction { get; set; } = new CustomerBehaviorPrediction();
        public RevenueProjection RevenueProjection { get; set; } = new RevenueProjection();
        public AffordabilityAnalysis AffordabilityAnalysis { get; set; } = new AffordabilityAnalysis();
        public ImplementationPlan ImplementationPlan { get; set; } = new ImplementationPlan();
        public RiskAssessment RiskAssessment { get; set; } = new RiskAssessment();
        public decimal ConfidenceScore { get; set; }
        public int ExecutionTimeMs { get; set; }
        public DateTime Timestamp { get; set; }
        public string Error { get; set; } = string.Empty;
    }

    /// <summary>
    /// Anomaly detection comprehensive result
    /// </summary>
    public class AnomalyDetectionResult
    {
        public bool Success { get; set; }
        public List<FinancialAnomaly> DetectedAnomalies { get; set; } = new List<FinancialAnomaly>();
        public string AnomalyAnalysis { get; set; } = string.Empty;
        public SeverityAssessment SeverityAssessment { get; set; } = new SeverityAssessment();
        public List<string> RecommendedActions { get; set; } = new List<string>();
        public RootCauseAnalysis RootCauseAnalysis { get; set; } = new RootCauseAnalysis();
        public List<string> PreventionStrategies { get; set; } = new List<string>();
        public decimal ConfidenceScore { get; set; }
        public DateTime Timestamp { get; set; }
        public string Error { get; set; } = string.Empty;
    }

    #endregion

    #region Supporting Analysis Classes

    /// <summary>
    /// Optimized rate recommendation
    /// </summary>
    public class OptimizedRate
    {
        public string RateType { get; set; } = string.Empty; // Base, Peak, Off-Peak, etc.
        public decimal CurrentRate { get; set; }
        public decimal OptimalRate { get; set; }
        public decimal ChangePercent { get; set; }
        public string Justification { get; set; } = string.Empty;
        public decimal CustomerImpact { get; set; }
        public decimal RevenueImpact { get; set; }
        public List<string> ImplementationSteps { get; set; } = new List<string>();
    }

    /// <summary>
    /// Revenue projection analysis
    /// </summary>
    public class RevenueProjection
    {
        public decimal CurrentAnnualRevenue { get; set; }
        public decimal ProjectedAnnualRevenue { get; set; }
        public decimal RevenueChange { get; set; }
        public decimal TotalRevenueChange { get; set; }
        public List<MonthlyRevenuePrediction> MonthlyProjections { get; set; } = new List<MonthlyRevenuePrediction>();
        public List<RevenueScenario> Scenarios { get; set; } = new List<RevenueScenario>();
        public decimal ConfidenceLevel { get; set; }
    }

    /// <summary>
    /// Affordability analysis for rate changes
    /// </summary>
    public class AffordabilityAnalysis
    {
        public decimal MedianHouseholdIncome { get; set; }
        public decimal CurrentAffordabilityRatio { get; set; }
        public decimal ProjectedAffordabilityRatio { get; set; }
        public Dictionary<string, decimal> AffordabilityBySegment { get; set; } = new Dictionary<string, decimal>();
        public List<string> AffordabilityConcerns { get; set; } = new List<string>();
        public List<string> MitigationOptions { get; set; } = new List<string>();
    }

    /// <summary>
    /// Implementation plan for rate changes
    /// </summary>
    public class ImplementationPlan
    {
        public DateTime ProposedStartDate { get; set; }
        public TimeSpan EstimatedDuration { get; set; } = TimeSpan.FromDays(30);
        public List<string> CriticalSuccessFactors { get; set; } = new List<string>();
        public List<string> ResourceRequirements { get; set; } = new List<string>();
        public List<string> RiskMitigationSteps { get; set; } = new List<string>();
        public List<string> MonitoringCheckpoints { get; set; } = new List<string>();
        public List<string> RollbackProcedures { get; set; } = new List<string>();
        public List<ImplementationPhase> Phases { get; set; } = new List<ImplementationPhase>();
        public List<string> Prerequisites { get; set; } = new List<string>();
        public string CommunicationPlan { get; set; } = string.Empty;
        public List<RegulatoryRequirement> RegulatoryRequirements { get; set; } = new List<RegulatoryRequirement>();
        public decimal EstimatedCost { get; set; }
    }

    /// <summary>
    /// Risk assessment for financial decisions
    /// </summary>
    public class RiskAssessment
    {
        public string OverallRiskLevel { get; set; } = string.Empty;
        public decimal OverallRiskScore { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public List<IdentifiedRisk> IdentifiedRisks { get; set; } = new List<IdentifiedRisk>();
        public List<string> MitigationStrategies { get; set; } = new List<string>();
        public decimal FinancialRiskScore { get; set; }
        public decimal CustomerRiskScore { get; set; }
        public decimal RegulatoryRiskScore { get; set; }
        public string RiskSummary { get; set; } = string.Empty;
    }

    /// <summary>
    /// Severity assessment for anomalies
    /// </summary>
    public class SeverityAssessment
    {
        public string OverallSeverity { get; set; } = string.Empty;
        public int CriticalAnomalies { get; set; }
        public int HighSeverityAnomalies { get; set; }
        public int MediumSeverityAnomalies { get; set; }
        public int LowSeverityAnomalies { get; set; }
        public decimal ImpactScore { get; set; }
        public string ImmediateActionRequired { get; set; } = string.Empty;
    }

    /// <summary>
    /// Root cause analysis for anomalies
    /// </summary>
    public class RootCauseAnalysis
    {
        public List<PotentialCause> PotentialCauses { get; set; } = new List<PotentialCause>();
        public string MostLikelyCause { get; set; } = string.Empty;
        public decimal CauseConfidence { get; set; }
        public List<string> InvestigationSteps { get; set; } = new List<string>();
        public string CausalChain { get; set; } = string.Empty;
    }

    /// <summary>
    /// Implementation phase details
    /// </summary>
    public class ImplementationPhase
    {
        public string PhaseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; } = TimeSpan.FromDays(7);
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> Activities { get; set; } = new List<string>();
        public List<string> Deliverables { get; set; } = new List<string>();
        public List<string> Stakeholders { get; set; } = new List<string>();
        public List<string> Dependencies { get; set; } = new List<string>();
        public List<string> SuccessCriteria { get; set; } = new List<string>();
        public decimal Budget { get; set; }
    }

    /// <summary>
    /// Regulatory requirement for implementation
    /// </summary>
    public class RegulatoryRequirement
    {
        public string RequirementType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ComplianceDeadline { get; set; }
        public string ResponsibleParty { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<string> RequiredDocuments { get; set; } = new List<string>();
    }

    /// <summary>
    /// Identified risk in analysis
    /// </summary>
    public class IdentifiedRisk
    {
        public string RiskName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Probability { get; set; }
        public decimal Impact { get; set; }
        public decimal RiskScore { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> MitigationActions { get; set; } = new List<string>();
    }

    /// <summary>
    /// Potential cause for anomalies
    /// </summary>
    public class PotentialCause
    {
        public string CauseName { get; set; } = string.Empty;
        public decimal Likelihood { get; set; }
        public string Evidence { get; set; } = string.Empty;
        public List<string> VerificationSteps { get; set; } = new List<string>();
        public string Category { get; set; } = string.Empty;
    }

    #endregion

    #region Optimization-Specific Classes

    /// <summary>
    /// Optimization scenario for rate analysis
    /// </summary>
    public class OptimizationScenario
    {
        public string ScenarioId { get; set; } = string.Empty;
        public string ScenarioName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal OptimizedRate { get; set; }
        public decimal CurrentRate { get; set; }
        public decimal RevenueImpact { get; set; }
        public decimal CustomerImpact { get; set; }
        public decimal ConfidenceScore { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public List<string> Assumptions { get; set; } = new List<string>();
        public List<string> Constraints { get; set; } = new List<string>();
        public Dictionary<string, decimal> Metrics { get; set; } = new Dictionary<string, decimal>();
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsRecommended { get; set; }
        public string Status { get; set; } = "Draft";
        // Additional properties needed by RealTimeRateOptimizer
        public List<RateAdjustment> RateAdjustments { get; set; } = new List<RateAdjustment>();
        public TimeSpan ExpectedImplementationTime { get; set; } = TimeSpan.FromDays(30);
        public string CustomerImpactLevel { get; set; } = string.Empty;
        public ScenarioEvaluation EvaluationResults { get; set; } = new ScenarioEvaluation();
    }

    /// <summary>
    /// Parameters for optimization algorithms
    /// </summary>
    public class OptimizationParameters
    {
        public decimal TargetRevenue { get; set; }
        public decimal MaxRateIncrease { get; set; } = 0.15m; // 15% max increase
        public decimal MinAffordabilityIndex { get; set; } = 0.7m; // 70% affordability threshold
        public bool OptimizeForEquity { get; set; } = true;
        public bool ConsiderSeasonality { get; set; } = true;
        public List<string> Constraints { get; set; } = new List<string>();
        public string OptimizationMethod { get; set; } = "balanced"; // revenue_max, cost_min, balanced
        public decimal ConvergenceThreshold { get; set; } = 0.001m;
        public int MaxIterations { get; set; } = 1000;
        public Dictionary<string, decimal> WeightingFactors { get; set; } = new Dictionary<string, decimal>();
        public bool EnableRealTimeUpdates { get; set; } = true;
        public string PriorityLevel { get; set; } = "Standard";
        // Additional properties needed by RealTimeRateOptimizer
        public Dictionary<string, object> CustomOptimizationTargets { get; set; } = new Dictionary<string, object>();
        public decimal RevenueOptimizationWeight { get; set; } = 0.4m;
        public decimal AffordabilityWeight { get; set; } = 0.3m;
        public decimal RiskTolerance { get; set; } = 0.3m;
    }

    /// <summary>
    /// Real-time optimization update structure
    /// </summary>
    public class RealTimeOptimizationUpdate
    {
        public string UpdateId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string UpdateType { get; set; } = string.Empty; // DataUpdate, ParameterChange, ResultUpdate
        public string Message { get; set; } = string.Empty;
        public decimal Progress { get; set; }
        public Dictionary<string, object> UpdatedValues { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> IntermediateResults { get; set; } = new Dictionary<string, object>();
        public object FinalResults { get; set; } = new object();
        public decimal ConfidenceLevel { get; set; }
        public string Source { get; set; } = string.Empty;
        public bool RequiresRecomputation { get; set; }
        public string Priority { get; set; } = "Normal"; // Low, Normal, High, Critical
        public List<string> AffectedScenarios { get; set; } = new List<string>();
        public string Status { get; set; } = "Pending";
    }

    /// <summary>
    /// Rate adjustment recommendation
    /// </summary>
    public class RateAdjustment
    {
        public string AdjustmentId { get; set; } = string.Empty;
        public DateTime EffectiveDate { get; set; }
        public decimal CurrentRate { get; set; }
        public decimal ProposedRate { get; set; }
        public decimal AdjustmentPercent { get; set; }
        public decimal AdjustmentPercentage { get; set; } // Additional property needed by RealTimeRateOptimizer
        public string RateType { get; set; } = string.Empty; // Base, Peak, Off-Peak, etc.
        public string ServiceType { get; set; } = string.Empty; // Additional property needed by RealTimeRateOptimizer
        public string Justification { get; set; } = string.Empty;
        public decimal ExpectedRevenueImpact { get; set; }
        public decimal CustomerImpact { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public List<string> Prerequisites { get; set; } = new List<string>();
        public List<string> ImplementationSteps { get; set; } = new List<string>();
        public string ApprovalStatus { get; set; } = "Pending";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Scenario evaluation results
    /// </summary>
    public class ScenarioEvaluation
    {
        public string EvaluationId { get; set; } = string.Empty;
        public string ScenarioId { get; set; } = string.Empty;
        public DateTime EvaluatedAt { get; set; } = DateTime.Now;
        public decimal OverallScore { get; set; }
        public decimal RevenueScore { get; set; }
        public decimal AffordabilityScore { get; set; }
        public decimal RiskScore { get; set; }
        public decimal ImplementationScore { get; set; }
        public decimal ProjectedRevenueChange { get; set; }
        public decimal CustomerAffordabilityImpact { get; set; }
        public decimal CustomerAffordabilityScore { get; set; }
        public string CustomerImpactLevel { get; set; } = string.Empty;
        public decimal ImplementationComplexity { get; set; }
        public bool RegulatoryCompliance { get; set; }
        public string EvaluationDetails { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public List<string> Strengths { get; set; } = new List<string>();
        public List<string> Weaknesses { get; set; } = new List<string>();
        public List<string> RiskFactors { get; set; } = new List<string>();
        public Dictionary<string, decimal> DetailedMetrics { get; set; } = new Dictionary<string, decimal>();
        public string EvaluatorComments { get; set; } = string.Empty;
    }

    /// <summary>
    /// Optimization confidence analysis
    /// </summary>
    public class OptimizationConfidence
    {
        public decimal OverallConfidence { get; set; }
        public decimal ModelConfidence { get; set; }
        public decimal ImplementationConfidence { get; set; }
        public decimal DataQualityConfidence { get; set; }
        public decimal ModelAccuracyConfidence { get; set; }
        public decimal AssumptionValidityConfidence { get; set; }
        public decimal ImplementationFeasibilityConfidence { get; set; }
        public List<string> ConfidenceFactors { get; set; } = new List<string>();
        public List<string> UncertaintyFactors { get; set; } = new List<string>();
        public string ConfidenceLevel { get; set; } = string.Empty; // Low, Medium, High
        public Dictionary<string, decimal> ComponentConfidences { get; set; } = new Dictionary<string, decimal>();
        public string RecommendedValidation { get; set; } = string.Empty;
        public DateTime AssessedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Monitoring plan for optimization implementation
    /// </summary>
    public class MonitoringPlan
    {
        public string PlanId { get; set; } = string.Empty;
        public string ScenarioId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> MonitoringMetrics { get; set; } = new List<string>();
        public List<string> KeyPerformanceIndicators { get; set; } = new List<string>();
        public List<string> KeyMetrics { get; set; } = new List<string>();
        public List<DateTime> ReviewSchedule { get; set; } = new List<DateTime>();
        public Dictionary<string, decimal> BaselineValues { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, decimal> TargetValues { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, decimal> AlertThresholds { get; set; } = new Dictionary<string, decimal>();
        public TimeSpan MonitoringFrequency { get; set; } = TimeSpan.FromDays(7);
        public List<string> ResponsibleParties { get; set; } = new List<string>();
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Expected outcomes from optimization scenario
    /// </summary>
    public class ExpectedOutcomes
    {
        public string OutcomeId { get; set; } = string.Empty;
        public string ScenarioId { get; set; } = string.Empty;
        public decimal ExpectedRevenue { get; set; }
        public decimal ExpectedCostSavings { get; set; }
        public decimal ExpectedCustomerImpact { get; set; }
        public decimal ExpectedAffordabilityChange { get; set; }
        public decimal RevenueIncrease { get; set; }
        public string CustomerImpact { get; set; } = string.Empty;
        public string TimeToFullImplementation { get; set; } = string.Empty;
        public decimal SuccessProbability { get; set; }
        public List<string> ExpectedBenefits { get; set; } = new List<string>();
        public List<string> ExpectedChallenges { get; set; } = new List<string>();
        public Dictionary<string, decimal> ExpectedMetrics { get; set; } = new Dictionary<string, decimal>();
        public string TimeFrame { get; set; } = string.Empty;
        public decimal ConfidenceLevel { get; set; }
        public string SuccessCriteria { get; set; } = string.Empty;
        public List<string> MitigationStrategies { get; set; } = new List<string>();
        public DateTime ProjectedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Rate optimization summary for final results
    /// </summary>
    public class RateOptimizationSummary
    {
        public string SummaryId { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public decimal OptimalRate { get; set; }
        public decimal ProjectedRevenueChange { get; set; }
        public string CustomerImpactLevel { get; set; } = string.Empty;
        public string EstimatedDuration { get; set; } = string.Empty;
        public decimal ConfidenceScore { get; set; }
        public string RecommendationLevel { get; set; } = string.Empty;
        public List<string> KeyBenefits { get; set; } = new List<string>();
        public List<string> KeyRisks { get; set; } = new List<string>();
        public List<string> NextSteps { get; set; } = new List<string>();
        public string ExecutiveSummary { get; set; } = string.Empty;
        // Additional properties needed by RealTimeRateOptimizer
        public OptimizationScenario OptimalScenario { get; set; } = new OptimizationScenario();
        public decimal ExpectedRevenueIncrease { get; set; }
        public string ImplementationTimeline { get; set; } = string.Empty;
        public decimal ConfidenceLevel { get; set; }
        public DateTime NextReviewDate { get; set; }
    }

    #endregion

    #region Additional Supporting Data Structures

    /// <summary>
    /// Explainability analysis result structure
    /// </summary>
    public class ExplainabilityAnalysis
    {
        public FeatureImportanceResult FeatureImportance { get; set; } = new FeatureImportanceResult();
        public DecisionPathResult DecisionPath { get; set; } = new DecisionPathResult();
        public decimal TransparencyScore { get; set; }
        public string ExplanationNarrative { get; set; } = string.Empty;
        public List<string> KeyDecisionFactors { get; set; } = new List<string>();
        public List<AlternativeScenario> AlternativeScenarios { get; set; } = new List<AlternativeScenario>();
        public ConfidenceFactorAnalysis ConfidenceFactors { get; set; } = new ConfidenceFactorAnalysis();
        public BiasAssessment BiasAssessment { get; set; } = new BiasAssessment();
        public List<string> Limitations { get; set; } = new List<string>();
        public List<string> RecommendedValidations { get; set; } = new List<string>();
    }

    /// <summary>
    /// Feature importance analysis result
    /// </summary>
    public class FeatureImportanceResult
    {
        public Dictionary<string, decimal> ImportanceScores { get; set; } = new Dictionary<string, decimal>();
        public decimal FeatureCoverage { get; set; }
        public decimal DataCompleteness { get; set; }
        public decimal CrossValidationScore { get; set; }
        public decimal FeatureStability { get; set; }
        public List<string> TopFeatures { get; set; } = new List<string>();
    }

    /// <summary>
    /// Decision path analysis result
    /// </summary>
    public class DecisionPathResult
    {
        public decimal ModelAccuracy { get; set; }
        public decimal PredictionStability { get; set; }
        public int HistoricalDataMonths { get; set; }
        public List<string> KeyInsights { get; set; } = new List<string>();
        public List<string> Uncertainties { get; set; } = new List<string>();
        public List<string> ConfidenceIndicators { get; set; } = new List<string>();
        public string DecisionLogic { get; set; } = string.Empty;
    }

    /// <summary>
    /// Transparency metrics
    /// </summary>
    public class TransparencyMetrics
    {
        public decimal OverallTransparencyScore { get; set; }
        public decimal DataQualityScore { get; set; }
        public decimal ModelReliabilityScore { get; set; }
        public decimal FeatureCompleteness { get; set; }
        public decimal ExplanationDepth { get; set; }
        public decimal AuditabilityScore { get; set; }
    }

    /// <summary>
    /// Alternative scenario description
    /// </summary>
    public class AlternativeScenario
    {
        public string ScenarioName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> KeyChanges { get; set; } = new List<string>();
        public string ExpectedOutcome { get; set; } = string.Empty;
        public string RiskLevel { get; set; } = string.Empty;
    }

    /// <summary>
    /// Confidence factor analysis
    /// </summary>
    public class ConfidenceFactorAnalysis
    {
        public List<string> DataQualityFactors { get; set; } = new List<string>();
        public List<string> ModelQualityFactors { get; set; } = new List<string>();
        public List<string> ExternalFactors { get; set; } = new List<string>();
        public decimal OverallConfidence { get; set; }
    }

    /// <summary>
    /// Bias assessment analysis
    /// </summary>
    public class BiasAssessment
    {
        public List<string> IdentifiedBiases { get; set; } = new List<string>();
        public List<string> MitigationRecommendations { get; set; } = new List<string>();
        public string BiasRiskLevel { get; set; } = string.Empty;
    }

    /// <summary>
    /// Feature distribution analysis
    /// </summary>
    public class FeatureDistribution
    {
        public decimal Mean { get; set; }
        public decimal StandardDeviation { get; set; }
        public List<decimal> Distribution { get; set; } = new List<decimal>();
    }

    /// <summary>
    /// Explainability report
    /// </summary>
    public class ExplainabilityReport
    {
        public string ExecutiveSummary { get; set; } = string.Empty;
        public ExplainabilityAnalysis DetailedAnalysis { get; set; } = new ExplainabilityAnalysis();
        public AuditTrail AuditTrail { get; set; } = new AuditTrail();
        public RegulatoryCompliance RegulatoryCompliance { get; set; } = new RegulatoryCompliance();
        public List<string> RecommendedActions { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; }
        public string ReportVersion { get; set; } = string.Empty;
    }

    /// <summary>
    /// Audit trail structure
    /// </summary>
    public class AuditTrail
    {
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string QueryType { get; set; } = string.Empty;
        public List<string> DataSources { get; set; } = new List<string>();
        public List<string> DecisionFactors { get; set; } = new List<string>();
        public List<string> ValidationSteps { get; set; } = new List<string>();
        public string BiasAssessment { get; set; } = string.Empty;
        public string ComplianceNotes { get; set; } = string.Empty;
    }

    /// <summary>
    /// Regulatory compliance assessment
    /// </summary>
    public class RegulatoryCompliance
    {
        public bool TransparencyCompliant { get; set; }
        public bool ExplainabilityCompliant { get; set; }
        public bool AuditabilityCompliant { get; set; }
        public bool BiasAssessmentCompliant { get; set; }
        public decimal ComplianceScore { get; set; }
        public string ComplianceNotes { get; set; } = string.Empty;
    }

    /// <summary>
    /// ML recommendation structure
    /// </summary>
    public class MLRecommendation
    {
        public string RecommendationType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal ConfidenceScore { get; set; }
        public List<string> SupportingEvidence { get; set; } = new List<string>();
        public List<string> ImplementationSteps { get; set; } = new List<string>();
        public string RiskLevel { get; set; } = string.Empty;
        public decimal ExpectedImpact { get; set; }
    }

    /// <summary>
    /// Customer impact analysis structure
    /// </summary>
    public class CustomerImpactAnalysis
    {
        public decimal AffordabilityImpact { get; set; }
        public decimal AffordabilityScore { get; set; }
        public string ImpactLevel { get; set; } = string.Empty;
    }

    #endregion
}
