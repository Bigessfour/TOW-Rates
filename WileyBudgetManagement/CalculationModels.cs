using System;
using System.Collections.Generic;

namespace WileyBudgetManagement.Models
{
    #region Calculation Goals and Parameters

    /// <summary>
    /// Goals and constraints for rate optimization
    /// </summary>
    public class RateOptimizationGoals
    {
        public decimal TargetRevenue { get; set; }
        public decimal MaxRateIncreasePercent { get; set; } = 0.15m; // 15% max increase
        public decimal CustomerRetentionTarget { get; set; } = 0.95m; // 95% retention
        public decimal AffordabilityConstraint { get; set; } = 0.04m; // 4% of income max
        public List<string> PriorityObjectives { get; set; } = new List<string>();
        public bool ConsiderSeasonalFactors { get; set; } = true;
        public bool IncludeCompetitiveAnalysis { get; set; } = true;
    }

    /// <summary>
    /// Customer demographic data for affordability analysis
    /// </summary>
    public class CustomerDemographics
    {
        public string CustomerSegment { get; set; } = string.Empty;
        public decimal AverageHouseholdIncome { get; set; }
        public int NumberOfCustomers { get; set; }
        public decimal UtilityBurdenPercentage { get; set; }
        public string GeographicArea { get; set; } = string.Empty;
        public List<string> VulnerabilityFactors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Configuration for anomaly detection
    /// </summary>
    public class AnomalyDetectionConfig
    {
        public decimal SensitivityLevel { get; set; } = 2.0m; // Standard deviations
        public int MinimumDataPoints { get; set; } = 12; // Minimum months of data
        public bool DetectSeasonalAnomalies { get; set; } = true;
        public bool DetectTrendAnomalies { get; set; } = true;
        public List<string> IgnoreCategories { get; set; } = new List<string>();
    }

    /// <summary>
    /// Financial data point for time series analysis
    /// </summary>
    public class FinancialDataPoint
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Historical revenue data for forecasting
    /// </summary>
    public class HistoricalRevenueData
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int CustomerCount { get; set; }
        public decimal AverageRate { get; set; }
        public string RevenueType { get; set; } = string.Empty; // Base, Usage, Fees, etc.
        public Dictionary<string, decimal> RevenueBreakdown { get; set; } = new Dictionary<string, decimal>();
    }

    /// <summary>
    /// Regulatory requirement definition
    /// </summary>
    public class RegulatoryRequirement
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Budget, Revenue, Reserves, etc.
        public string Description { get; set; } = string.Empty;
        public decimal Threshold { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public bool IsMandatory { get; set; } = true;
        public string Authority { get; set; } = string.Empty; // State, Federal, Local
        public DateTime EffectiveDate { get; set; }
        public string RecommendedAction { get; set; } = string.Empty;
    }

    #endregion

    #region Calculation Results

    /// <summary>
    /// Rate optimization result with AI insights and basic calculations
    /// </summary>
    public class RateOptimizationResult
    {
        public bool Success { get; set; }
        public string Error { get; set; } = string.Empty;
        public List<OptimizedRate> OptimizedRates { get; set; } = new List<OptimizedRate>();
        public decimal ConfidenceScore { get; set; }
        public string AIAnalysis { get; set; } = string.Empty;
        public RevenueProjection RevenueProjection { get; set; } = new RevenueProjection();
        public CustomerImpactAssessment CustomerImpactAssessment { get; set; } = new CustomerImpactAssessment();
        public List<string> RiskFactors { get; set; } = new List<string>();
        public ImplementationPlan ImplementationTimeline { get; set; } = new ImplementationPlan();
        public string CalculationMethod { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Individual rate recommendation
    /// </summary>
    public class OptimizedRate
    {
        public string ServiceType { get; set; } = string.Empty;
        public decimal CurrentRate { get; set; }
        public decimal RecommendedRate { get; set; }
        public decimal RateChange { get; set; }
        public decimal PercentageChange => CurrentRate > 0 ? RateChange / CurrentRate : 0;
        public string Justification { get; set; } = string.Empty;
        public decimal ConfidenceLevel { get; set; }
        public DateTime EffectiveDate { get; set; }
        public List<string> ImplementationSteps { get; set; } = new List<string>();
    }

    /// <summary>
    /// Revenue projection analysis
    /// </summary>
    public class RevenueProjection
    {
        public decimal ProjectedAnnualRevenue { get; set; }
        public decimal RevenueChange { get; set; }
        public decimal RevenueChangePercentage { get; set; }
        public List<decimal> MonthlyProjections { get; set; } = new List<decimal>();
        public decimal ConfidenceInterval { get; set; }
        public List<string> KeyAssumptions { get; set; } = new List<string>();
    }

    /// <summary>
    /// Customer impact assessment
    /// </summary>
    public class CustomerImpactAssessment
    {
        public decimal AverageMonthlyIncrease { get; set; }
        public decimal PercentageIncrease { get; set; }
        public decimal EstimatedCustomerLoss { get; set; }
        public decimal AffordabilityImpact { get; set; }
        public List<string> VulnerableSegments { get; set; } = new List<string>();
        public List<string> MitigationStrategies { get; set; } = new List<string>();
    }

    /// <summary>
    /// Implementation plan for rate changes
    /// </summary>
    public class ImplementationPlan
    {
        public DateTime ProposedStartDate { get; set; }
        public List<ImplementationStep> Steps { get; set; } = new List<ImplementationStep>();
        public int EstimatedDurationDays { get; set; }
        public List<string> RequiredApprovals { get; set; } = new List<string>();
        public List<string> CommunicationPlan { get; set; } = new List<string>();
    }

    public class ImplementationStep
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime TargetDate { get; set; }
        public bool IsCompleted { get; set; }
        public List<string> Dependencies { get; set; } = new List<string>();
    }

    /// <summary>
    /// Customer affordability analysis result
    /// </summary>
    public class AffordabilityAnalysisResult
    {
        public bool Success { get; set; }
        public string Error { get; set; } = string.Empty;
        public decimal OverallAffordabilityScore { get; set; }
        public decimal VulnerableCustomerPercentage { get; set; }
        public string AffordabilityRating { get; set; } = string.Empty;
        public decimal AverageMonthlyBurden { get; set; }
        public decimal PercentageOfIncome { get; set; }
        public List<string> RecommendedAssistancePrograms { get; set; } = new List<string>();
        public List<string> AlternativeRateStructures { get; set; } = new List<string>();
        public string SocioeconomicImpactAnalysis { get; set; } = string.Empty;
        public decimal ConfidenceScore { get; set; }
        public string CalculationMethod { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Anomaly detection result
    /// </summary>
    public class AnomalyDetectionResult
    {
        public bool Success { get; set; }
        public string Error { get; set; } = string.Empty;
        public List<DetectedAnomaly> DetectedAnomalies { get; set; } = new List<DetectedAnomaly>();
        public string AnomalyAnalysis { get; set; } = string.Empty;
        public SeverityAssessment SeverityAssessment { get; set; } = new SeverityAssessment();
        public List<string> RecommendedActions { get; set; } = new List<string>();
        public RootCauseAnalysis RootCauseAnalysis { get; set; } = new RootCauseAnalysis();
        public List<string> PreventionStrategies { get; set; } = new List<string>();
        public decimal ConfidenceScore { get; set; }
        public string CalculationMethod { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Individual anomaly detection
    /// </summary>
    public class DetectedAnomaly
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public decimal ExpectedValue { get; set; }
        public decimal Deviation { get; set; }
        public decimal SeverityScore { get; set; }
        public string AnomalyType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> PotentialCauses { get; set; } = new List<string>();
    }

    /// <summary>
    /// Revenue forecasting result
    /// </summary>
    public class RevenueForecastResult
    {
        public bool Success { get; set; }
        public string Error { get; set; } = string.Empty;
        public List<decimal> MonthlyProjections { get; set; } = new List<decimal>();
        public List<ConfidenceInterval> ConfidenceIntervals { get; set; } = new List<ConfidenceInterval>();
        public ScenarioAnalysis ScenarioAnalysis { get; set; } = new ScenarioAnalysis();
        public List<string> KeyAssumptions { get; set; } = new List<string>();
        public decimal ForecastAccuracy { get; set; }
        public string CalculationMethod { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Confidence interval for forecasts
    /// </summary>
    public class ConfidenceInterval
    {
        public decimal LowerBound { get; set; }
        public decimal UpperBound { get; set; }
        public decimal ConfidenceLevel { get; set; }
        public string Interpretation { get; set; } = string.Empty;
    }

    /// <summary>
    /// Scenario analysis for forecasting
    /// </summary>
    public class ScenarioAnalysis
    {
        public List<decimal> OptimisticScenario { get; set; } = new List<decimal>();
        public List<decimal> RealisticScenario { get; set; } = new List<decimal>();
        public List<decimal> PessimisticScenario { get; set; } = new List<decimal>();
        public decimal OptimisticProbability { get; set; } = 0.2m;
        public decimal RealisticProbability { get; set; } = 0.6m;
        public decimal PessimisticProbability { get; set; } = 0.2m;
        public List<string> ScenarioDrivers { get; set; } = new List<string>();
    }

    /// <summary>
    /// Regulatory compliance check result
    /// </summary>
    public class ComplianceCheckResult
    {
        public bool Success { get; set; }
        public string Error { get; set; } = string.Empty;
        public decimal OverallComplianceScore { get; set; }
        public decimal CompliancePercentage { get; set; }
        public List<string> ComplianceGaps { get; set; } = new List<string>();
        public List<string> RecommendedActions { get; set; } = new List<string>();
        public string RegulatoryRiskAssessment { get; set; } = string.Empty;
        public string ComplianceAnalysis { get; set; } = string.Empty;
        public List<ComplianceDetail> DetailedResults { get; set; } = new List<ComplianceDetail>();
        public string CalculationMethod { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Detailed compliance check result
    /// </summary>
    public class ComplianceDetail
    {
        public string RequirementName { get; set; } = string.Empty;
        public bool IsCompliant { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string RecommendedAction { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
    }

    #endregion

    #region Supporting Classes

    /// <summary>
    /// Machine learning recommendation
    /// </summary>
    public class MLRecommendation
    {
        public string RecommendationType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal ConfidenceScore { get; set; }
        public List<string> SupportingEvidence { get; set; } = new List<string>();
        public string RiskLevel { get; set; } = string.Empty;
        public decimal ExpectedImpact { get; set; }
        public string ImplementationComplexity { get; set; } = string.Empty;
        public List<string> Prerequisites { get; set; } = new List<string>();
    }

    /// <summary>
    /// Explainability analysis for AI decisions
    /// </summary>
    public class ExplainabilityAnalysis
    {
        public string MainReasoning { get; set; } = string.Empty;
        public List<ExplanationFactor> KeyFactors { get; set; } = new List<ExplanationFactor>();
        public List<string> Assumptions { get; set; } = new List<string>();
        public List<string> Limitations { get; set; } = new List<string>();
        public decimal TransparencyScore { get; set; }
        public string MethodologyExplanation { get; set; } = string.Empty;
    }

    public class ExplanationFactor
    {
        public string Name { get; set; } = string.Empty;
        public decimal ImportanceWeight { get; set; }
        public string Influence { get; set; } = string.Empty; // Positive, Negative, Neutral
        public string Description { get; set; } = string.Empty;
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
        public string MostLikelyCause { get; set; } = string.Empty;
        public decimal CauseConfidence { get; set; }
        public List<PotentialCause> PotentialCauses { get; set; } = new List<PotentialCause>();
        public string AnalysisMethod { get; set; } = string.Empty;
        public List<string> SupportingEvidence { get; set; } = new List<string>();
    }

    public class PotentialCause
    {
        public string CauseName { get; set; } = string.Empty;
        public decimal Likelihood { get; set; }
        public string Evidence { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Operational, Financial, External, etc.
        public List<string> RecommendedInvestigation { get; set; } = new List<string>();
    }

    /// <summary>
    /// Token usage tracking for AI services
    /// </summary>
    public class TokenUsage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
        public decimal EstimatedCost { get; set; }
        public string Model { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Historical rate data for analysis
    /// </summary>
    public class HistoricalRateData
    {
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
        public string RateType { get; set; } = string.Empty;
        public int CustomerCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal CustomerSatisfaction { get; set; }
        public List<string> MarketConditions { get; set; } = new List<string>();
    }

    #endregion
}
