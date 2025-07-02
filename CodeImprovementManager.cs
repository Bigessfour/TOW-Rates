using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WileyBudgetManagement.Tools
{
    /// <summary>
    /// Code Improvement Manager for Town of Wiley Budget Management Software
    /// Provides structured review and implementation of code improvements
    /// </summary>
    public class CodeImprovementManager
    {
        public enum ImprovementPriority
        {
            Critical = 1,    // Security, major bugs
            High = 2,        // Performance, maintainability
            Medium = 3,      // Code quality, best practices
            Low = 4          // Nice-to-have improvements
        }

        public enum ImprovementCategory
        {
            Security,
            Performance,
            Maintainability,
            BestPractices,
            MunicipalCompliance,
            ErrorHandling,
            Documentation,
            Testing
        }

        public class CodeImprovement
        {
            public string Id { get; set; } = Guid.NewGuid().ToString()[..8];
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string CurrentCode { get; set; } = string.Empty;
            public string ImprovedCode { get; set; } = string.Empty;
            public string Rationale { get; set; } = string.Empty;
            public ImprovementPriority Priority { get; set; }
            public ImprovementCategory Category { get; set; }
            public string FileName { get; set; } = string.Empty;
            public int LineNumber { get; set; }
            public bool IsSelected { get; set; }
            public decimal EstimatedHours { get; set; }
            public List<string> Dependencies { get; set; } = new();
            public string TestingNotes { get; set; } = string.Empty;
        }

        public class ImprovementPlan
        {
            public List<CodeImprovement> Improvements { get; set; } = new();
            public decimal TotalEstimatedHours => Improvements.Where(i => i.IsSelected).Sum(i => i.EstimatedHours);
            public int CriticalCount => Improvements.Count(i => i.Priority == ImprovementPriority.Critical);
            public int HighCount => Improvements.Count(i => i.Priority == ImprovementPriority.High);
            public DateTime CreatedDate { get; set; } = DateTime.Now;
        }

        private readonly string _sourceFilePath;
        private readonly string _sourceCode;

        public CodeImprovementManager(string filePath)
        {
            _sourceFilePath = filePath;
            _sourceCode = File.ReadAllText(filePath);
        }

        /// <summary>
        /// Analyze AIEnhancedQueryService and generate improvement recommendations
        /// </summary>
        public ImprovementPlan AnalyzeAndGenerateImprovements()
        {
            var plan = new ImprovementPlan();
            var improvements = new List<CodeImprovement>();

            // Security Improvements
            improvements.AddRange(AnalyzeSecurity());

            // Performance Improvements
            improvements.AddRange(AnalyzePerformance());

            // Maintainability Improvements
            improvements.AddRange(AnalyzeMaintainability());

            // Best Practices
            improvements.AddRange(AnalyzeBestPractices());

            // Municipal Finance Specific
            improvements.AddRange(AnalyzeMunicipalCompliance());

            // Error Handling
            improvements.AddRange(AnalyzeErrorHandling());

            plan.Improvements = improvements.OrderBy(i => i.Priority).ThenBy(i => i.Category).ToList();
            return plan;
        }

        private List<CodeImprovement> AnalyzeSecurity()
        {
            var improvements = new List<CodeImprovement>();

            // Environment variable validation
            if (_sourceCode.Contains("Environment.GetEnvironmentVariable") && !_sourceCode.Contains("ValidateApiKey"))
            {
                improvements.Add(new CodeImprovement
                {
                    Title = "Add API Key Validation",
                    Description = "Validate XAI API key format and accessibility before use",
                    Category = ImprovementCategory.Security,
                    Priority = ImprovementPriority.High,
                    FileName = Path.GetFileName(_sourceFilePath),
                    EstimatedHours = 1.0m,
                    CurrentCode = @"_xaiApiKey = Environment.GetEnvironmentVariable(""XAI_API_KEY"") ?? string.Empty;
if (string.IsNullOrEmpty(_xaiApiKey))
{
    throw new InvalidOperationException(""XAI_API_KEY environment variable not found."");
}",
                    ImprovedCode = @"_xaiApiKey = ValidateAndGetApiKey();

private string ValidateAndGetApiKey()
{
    var apiKey = Environment.GetEnvironmentVariable(""XAI_API_KEY"") ?? string.Empty;
    
    if (string.IsNullOrEmpty(apiKey))
        throw new InvalidOperationException(""XAI_API_KEY environment variable not found."");
    
    if (apiKey.Length < 32 || !apiKey.StartsWith(""xai-""))
        throw new InvalidOperationException(""Invalid XAI API key format."");
    
    return apiKey;
}",
                    Rationale = "Municipal software must validate all external API keys for security compliance",
                    TestingNotes = "Test with invalid keys, empty keys, and properly formatted keys"
                });
            }

            return improvements;
        }

        private List<CodeImprovement> AnalyzePerformance()
        {
            var improvements = new List<CodeImprovement>();

            // ConfigureAwait(false) missing
            if (_sourceCode.Contains("await") && !_sourceCode.Contains("ConfigureAwait(false)"))
            {
                improvements.Add(new CodeImprovement
                {
                    Title = "Add ConfigureAwait(false) to Async Calls",
                    Description = "Prevent deadlocks and improve performance in library code",
                    Category = ImprovementCategory.Performance,
                    Priority = ImprovementPriority.Medium,
                    FileName = Path.GetFileName(_sourceFilePath),
                    EstimatedHours = 0.5m,
                    CurrentCode = @"var response = await _httpClient.PostAsync(_apiEndpoint, content);
var responseText = await response.Content.ReadAsStringAsync();",
                    ImprovedCode = @"var response = await _httpClient.PostAsync(_apiEndpoint, content).ConfigureAwait(false);
var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);",
                    Rationale = "Municipal applications should avoid potential deadlocks in Windows Forms environment",
                    TestingNotes = "Verify no deadlocks occur during high-volume operations"
                });
            }

            // String concatenation optimization
            if (Regex.Matches(_sourceCode, @"\+.*[""']").Count > 3)
            {
                improvements.Add(new CodeImprovement
                {
                    Title = "Optimize String Operations",
                    Description = "Use StringBuilder for multiple string concatenations in error parsing",
                    Category = ImprovementCategory.Performance,
                    Priority = ImprovementPriority.Low,
                    FileName = Path.GetFileName(_sourceFilePath),
                    EstimatedHours = 0.75m,
                    Rationale = "Better memory efficiency for string operations in municipal reporting"
                });
            }

            return improvements;
        }

        private List<CodeImprovement> AnalyzeMaintainability()
        {
            var improvements = new List<CodeImprovement>();

            // Complex method breakdown
            if (_sourceCode.Contains("ExecuteWithRetry"))
            {
                improvements.Add(new CodeImprovement
                {
                    Title = "Extract Retry Logic to Separate Class",
                    Description = "Move complex retry logic to dedicated RetryPolicyService",
                    Category = ImprovementCategory.Maintainability,
                    Priority = ImprovementPriority.High,
                    FileName = Path.GetFileName(_sourceFilePath),
                    EstimatedHours = 2.0m,
                    CurrentCode = "ExecuteWithRetry method (62 lines)",
                    ImprovedCode = @"// New RetryPolicyService class
public class RetryPolicyService
{
    public async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation,
        int maxRetries = 3,
        TimeSpan? baseDelay = null)
    {
        // Extracted retry logic here
    }
}",
                    Rationale = "Separation of concerns improves testability and reusability across municipal services",
                    Dependencies = new List<string> { "Create RetryPolicyService", "Update DI container" },
                    TestingNotes = "Test retry scenarios independently from AI service logic"
                });
            }

            // Model configuration externalization
            if (_sourceCode.Contains("_modelConfigs = new()"))
            {
                improvements.Add(new CodeImprovement
                {
                    Title = "Move Model Configuration to External File",
                    Description = "Extract AI model configurations to appsettings.json for easier management",
                    Category = ImprovementCategory.Maintainability,
                    Priority = ImprovementPriority.Medium,
                    FileName = Path.GetFileName(_sourceFilePath),
                    EstimatedHours = 1.5m,
                    Rationale = "Municipal software should allow configuration changes without code deployment",
                    Dependencies = new List<string> { "Create ModelConfiguration.json", "Add IConfiguration injection" }
                });
            }

            return improvements;
        }

        private List<CodeImprovement> AnalyzeBestPractices()
        {
            var improvements = new List<CodeImprovement>();

            // Logging addition
            if (!_sourceCode.Contains("ILogger"))
            {
                improvements.Add(new CodeImprovement
                {
                    Title = "Add Comprehensive Logging",
                    Description = "Implement structured logging for municipal audit requirements",
                    Category = ImprovementCategory.BestPractices,
                    Priority = ImprovementPriority.High,
                    FileName = Path.GetFileName(_sourceFilePath),
                    EstimatedHours = 2.5m,
                    CurrentCode = "// No logging present",
                    ImprovedCode = @"private readonly ILogger<AIEnhancedQueryService> _logger;

// In methods:
_logger.LogInformation(""Processing AI query for Town of Wiley: {QueryType}"", query);
_logger.LogWarning(""Rate limit exceeded, retrying in {DelaySeconds} seconds"", delay);
_logger.LogError(ex, ""AI service request failed: {ErrorMessage}"", ex.Message);",
                    Rationale = "Municipal operations require detailed audit trails for transparency and compliance"
                });
            }

            return improvements;
        }

        private List<CodeImprovement> AnalyzeMunicipalCompliance()
        {
            var improvements = new List<CodeImprovement>();

            // Add municipal-specific validation
            improvements.Add(new CodeImprovement
            {
                Title = "Add Municipal Data Validation",
                Description = "Implement Town of Wiley specific business rules validation",
                Category = ImprovementCategory.MunicipalCompliance,
                Priority = ImprovementPriority.High,
                FileName = Path.GetFileName(_sourceFilePath),
                EstimatedHours = 3.0m,
                CurrentCode = "// Generic enterprise validation",
                ImprovedCode = @"private void ValidateMunicipalQuery(string query, EnterpriseContext context)
{
    // Validate query contains appropriate municipal context
    if (!context.Municipality.Equals(""Town of Wiley"", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(""This service is configured for Town of Wiley only"");
    
    // Validate rate study methodology compliance
    if (query.Contains(""rate"") && !context.HasRateStudyData)
        throw new InvalidOperationException(""Rate queries require valid rate study data"");
    
    // Add other municipal-specific validations
}",
                Rationale = "Municipal software must enforce jurisdiction-specific business rules"
            });

            return improvements;
        }

        private List<CodeImprovement> AnalyzeErrorHandling()
        {
            var improvements = new List<CodeImprovement>();

            // Enhanced exception handling
            improvements.Add(new CodeImprovement
            {
                Title = "Add Granular Exception Handling",
                Description = "Implement specific exception types for different AI service failures",
                Category = ImprovementCategory.ErrorHandling,
                Priority = ImprovementPriority.Medium,
                FileName = Path.GetFileName(_sourceFilePath),
                EstimatedHours = 2.0m,
                CurrentCode = @"catch (Exception finalEx)
{
    return new AIQueryResponse { Success = false, Error = finalEx.Message };
}",
                ImprovedCode = @"catch (HttpRequestException httpEx)
{
    _logger.LogError(httpEx, ""Network error during AI request"");
    return new AIQueryResponse { Success = false, Error = ""Network connectivity issue. Please check internet connection."" };
}
catch (TaskCanceledException timeoutEx)
{
    _logger.LogError(timeoutEx, ""AI request timeout"");
    return new AIQueryResponse { Success = false, Error = ""Request timeout. The AI service may be experiencing high load."" };
}
catch (JsonException jsonEx)
{
    _logger.LogError(jsonEx, ""JSON parsing error"");
    return new AIQueryResponse { Success = false, Error = ""Invalid response format from AI service."" };
}",
                Rationale = "Municipal users need clear, actionable error messages for troubleshooting"
            });

            return improvements;
        }

        /// <summary>
        /// Create an interactive review form for selecting improvements
        /// </summary>
        public void LaunchImprovementReview(ImprovementPlan plan)
        {
            var form = new ImprovementReviewForm(plan, this);
            form.ShowDialog();
        }

        /// <summary>
        /// Apply selected improvements to the source file
        /// </summary>
        public void ApplySelectedImprovements(ImprovementPlan plan)
        {
            var selectedImprovements = plan.Improvements.Where(i => i.IsSelected).ToList();

            if (!selectedImprovements.Any())
            {
                MessageBox.Show("No improvements selected for implementation.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Create backup
            var backupPath = _sourceFilePath + $".backup.{DateTime.Now:yyyyMMdd_HHmmss}";
            File.Copy(_sourceFilePath, backupPath);

            // Generate implementation plan
            var implementationReport = GenerateImplementationReport(selectedImprovements);

            var reportPath = Path.Combine(Path.GetDirectoryName(_sourceFilePath),
                $"ImprovementPlan_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            File.WriteAllText(reportPath, implementationReport);

            MessageBox.Show($"Implementation plan generated!\n\nBackup created: {backupPath}\nPlan saved: {reportPath}",
                "Implementation Ready", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GenerateImplementationReport(List<CodeImprovement> improvements)
        {
            var report = new StringBuilder();
            report.AppendLine("TOWN OF WILEY BUDGET MANAGEMENT - CODE IMPROVEMENT IMPLEMENTATION PLAN");
            report.AppendLine("=".PadRight(80, '='));
            report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"File: {_sourceFilePath}");
            report.AppendLine($"Total Improvements: {improvements.Count}");
            report.AppendLine($"Estimated Hours: {improvements.Sum(i => i.EstimatedHours):F1}");
            report.AppendLine();

            // Group by priority
            foreach (var priority in Enum.GetValues<ImprovementPriority>())
            {
                var priorityImprovements = improvements.Where(i => i.Priority == priority).ToList();
                if (!priorityImprovements.Any()) continue;

                report.AppendLine($"{priority.ToString().ToUpper()} PRIORITY ({priorityImprovements.Count} items)");
                report.AppendLine("-".PadRight(50, '-'));

                foreach (var improvement in priorityImprovements)
                {
                    report.AppendLine($"[{improvement.Id}] {improvement.Title}");
                    report.AppendLine($"   Category: {improvement.Category}");
                    report.AppendLine($"   Hours: {improvement.EstimatedHours:F1}");
                    report.AppendLine($"   Description: {improvement.Description}");

                    if (improvement.Dependencies.Any())
                        report.AppendLine($"   Dependencies: {string.Join(", ", improvement.Dependencies)}");

                    report.AppendLine($"   Rationale: {improvement.Rationale}");

                    if (!string.IsNullOrEmpty(improvement.TestingNotes))
                        report.AppendLine($"   Testing: {improvement.TestingNotes}");

                    report.AppendLine();
                }
                report.AppendLine();
            }

            // Implementation order recommendation
            report.AppendLine("RECOMMENDED IMPLEMENTATION ORDER");
            report.AppendLine("-".PadRight(40, '-'));
            var orderedImprovements = improvements
                .OrderBy(i => i.Priority)
                .ThenBy(i => i.Dependencies.Count)
                .ThenBy(i => i.EstimatedHours);

            int order = 1;
            foreach (var improvement in orderedImprovements)
            {
                report.AppendLine($"{order}. [{improvement.Id}] {improvement.Title} ({improvement.EstimatedHours:F1}h)");
                order++;
            }

            return report.ToString();
        }
    }
}
