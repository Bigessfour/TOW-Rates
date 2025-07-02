using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace WileyBudgetManagement.Tools
{
    /// <summary>
    /// Code Analysis Tool for examining C# files in the Wiley Budget Management project
    /// Focuses on municipal utility software quality metrics
    /// </summary>
    public class CodeAnalysisTool
    {
        public class AnalysisResult
        {
            public string FileName { get; set; } = string.Empty;
            public int LineCount { get; set; }
            public int MethodCount { get; set; }
            public int ClassCount { get; set; }
            public List<string> Dependencies { get; set; } = new();
            public List<string> PublicMethods { get; set; } = new();
            public List<string> SecurityConcerns { get; set; } = new();
            public List<string> PerformanceIssues { get; set; } = new();
            public List<string> Recommendations { get; set; } = new();
            public int CyclomaticComplexity { get; set; }
            public double MaintainabilityScore { get; set; }
        }

        public static AnalysisResult AnalyzeFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var content = File.ReadAllText(filePath);
            var result = new AnalysisResult
            {
                FileName = Path.GetFileName(filePath)
            };

            // Basic metrics
            result.LineCount = content.Split('\n').Length;
            result.ClassCount = CountMatches(content, @"class\s+\w+");
            result.MethodCount = CountMatches(content, @"(public|private|protected|internal)\s+.*\s+\w+\s*\(");

            // Extract dependencies
            result.Dependencies = ExtractDependencies(content);

            // Extract public methods
            result.PublicMethods = ExtractPublicMethods(content);

            // Security analysis
            result.SecurityConcerns = AnalyzeSecurity(content);

            // Performance analysis
            result.PerformanceIssues = AnalyzePerformance(content);

            // Calculate complexity
            result.CyclomaticComplexity = CalculateCyclomaticComplexity(content);

            // Calculate maintainability
            result.MaintainabilityScore = CalculateMaintainabilityScore(result, content);

            // Generate recommendations
            result.Recommendations = GenerateRecommendations(result, content);

            return result;
        }

        private static int CountMatches(string content, string pattern)
        {
            return Regex.Matches(content, pattern, RegexOptions.IgnoreCase).Count;
        }

        private static List<string> ExtractDependencies(string content)
        {
            var dependencies = new List<string>();
            var usingMatches = Regex.Matches(content, @"using\s+([\w\.]+);");

            foreach (Match match in usingMatches)
            {
                dependencies.Add(match.Groups[1].Value);
            }

            return dependencies.Distinct().ToList();
        }

        private static List<string> ExtractPublicMethods(string content)
        {
            var methods = new List<string>();
            var methodMatches = Regex.Matches(content, @"public\s+(?:async\s+)?(?:Task<?[\w<>]*>?\s+|[\w<>]+\s+)(\w+)\s*\(");

            foreach (Match match in methodMatches)
            {
                methods.Add(match.Groups[1].Value);
            }

            return methods;
        }

        private static List<string> AnalyzeSecurity(string content)
        {
            var concerns = new List<string>();

            // Check for hardcoded secrets
            if (Regex.IsMatch(content, @"(password|secret|key|token)\s*=\s*[""'][^""']+[""']", RegexOptions.IgnoreCase))
                concerns.Add("Potential hardcoded secrets detected");

            // Check for SQL injection risks
            if (content.Contains("SqlCommand") && content.Contains("+"))
                concerns.Add("Potential SQL injection vulnerability");

            // Check for unsafe HTTP requests
            if (content.Contains("HttpClient") && !content.Contains("https://"))
                concerns.Add("HTTP requests should use HTTPS");

            // Check for environment variable usage
            if (content.Contains("Environment.GetEnvironmentVariable"))
                concerns.Add("Environment variables used - ensure proper validation");

            return concerns;
        }

        private static List<string> AnalyzePerformance(string content)
        {
            var issues = new List<string>();

            // Check for synchronous calls in async methods
            if (content.Contains("async") && content.Contains(".Result"))
                issues.Add("Blocking async calls detected (.Result usage)");

            // Check for excessive string concatenation
            if (CountMatches(content, @"\+\s*[""']") > 5)
                issues.Add("Consider using StringBuilder for multiple string concatenations");

            // Check for LINQ in loops
            if (content.Contains("foreach") && content.Contains("Where("))
                issues.Add("LINQ operations in loops may impact performance");

            // Check for HttpClient disposal
            if (content.Contains("new HttpClient()") && !content.Contains("using") && !content.Contains("IDisposable"))
                issues.Add("HttpClient should be properly disposed or reused");

            return issues;
        }

        private static int CalculateCyclomaticComplexity(string content)
        {
            // Count decision points
            int complexity = 1; // Base complexity

            complexity += CountMatches(content, @"\bif\b");
            complexity += CountMatches(content, @"\belse\b");
            complexity += CountMatches(content, @"\bwhile\b");
            complexity += CountMatches(content, @"\bfor\b");
            complexity += CountMatches(content, @"\bforeach\b");
            complexity += CountMatches(content, @"\bswitch\b");
            complexity += CountMatches(content, @"\bcase\b");
            complexity += CountMatches(content, @"\bcatch\b");
            complexity += CountMatches(content, @"\?\?");
            complexity += CountMatches(content, @"\?[^:]");

            return complexity;
        }

        private static double CalculateMaintainabilityScore(AnalysisResult result, string content)
        {
            double score = 100.0;

            // Penalize high complexity
            if (result.CyclomaticComplexity > 20) score -= 20;
            else if (result.CyclomaticComplexity > 10) score -= 10;

            // Penalize long files
            if (result.LineCount > 500) score -= 15;
            else if (result.LineCount > 300) score -= 10;

            // Penalize too many dependencies
            if (result.Dependencies.Count > 15) score -= 10;

            // Penalize lack of documentation
            var commentLines = CountMatches(content, @"///|//");
            var documentationRatio = (double)commentLines / result.LineCount;
            if (documentationRatio < 0.1) score -= 15;

            // Bonus for error handling
            if (content.Contains("try") && content.Contains("catch")) score += 5;

            // Bonus for async/await usage
            if (content.Contains("async") && content.Contains("await")) score += 5;

            return Math.Max(0, Math.Min(100, score));
        }

        private static List<string> GenerateRecommendations(AnalysisResult result, string content)
        {
            var recommendations = new List<string>();

            if (result.CyclomaticComplexity > 15)
                recommendations.Add("Consider breaking down complex methods into smaller, focused methods");

            if (result.LineCount > 400)
                recommendations.Add("File is quite large - consider splitting into multiple files");

            if (result.Dependencies.Count > 12)
                recommendations.Add("High number of dependencies - review if all are necessary");

            if (!content.Contains("ILogger"))
                recommendations.Add("Consider adding logging for better debugging and monitoring");

            if (content.Contains("string.Empty") && content.Contains("??"))
                recommendations.Add("Consistent null handling pattern is good practice");

            if (!content.Contains("ConfigureAwait(false)") && content.Contains("await"))
                recommendations.Add("Consider using ConfigureAwait(false) for library code");

            if (result.SecurityConcerns.Any())
                recommendations.Add("Address security concerns before production deployment");

            if (result.PerformanceIssues.Any())
                recommendations.Add("Review performance issues for high-traffic scenarios");

            return recommendations;
        }

        public static void PrintAnalysis(AnalysisResult result)
        {
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine($"CODE ANALYSIS REPORT: {result.FileName}");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine();

            Console.WriteLine("📊 BASIC METRICS");
            Console.WriteLine($"   Lines of Code: {result.LineCount}");
            Console.WriteLine($"   Classes: {result.ClassCount}");
            Console.WriteLine($"   Methods: {result.MethodCount}");
            Console.WriteLine($"   Dependencies: {result.Dependencies.Count}");
            Console.WriteLine();

            Console.WriteLine("🔧 QUALITY METRICS");
            Console.WriteLine($"   Cyclomatic Complexity: {result.CyclomaticComplexity}");
            Console.WriteLine($"   Maintainability Score: {result.MaintainabilityScore:F1}/100");
            Console.WriteLine();

            if (result.PublicMethods.Any())
            {
                Console.WriteLine("🔓 PUBLIC API");
                foreach (var method in result.PublicMethods)
                    Console.WriteLine($"   • {method}()");
                Console.WriteLine();
            }

            if (result.SecurityConcerns.Any())
            {
                Console.WriteLine("🔒 SECURITY CONCERNS");
                foreach (var concern in result.SecurityConcerns)
                    Console.WriteLine($"   ⚠️  {concern}");
                Console.WriteLine();
            }

            if (result.PerformanceIssues.Any())
            {
                Console.WriteLine("⚡ PERFORMANCE ISSUES");
                foreach (var issue in result.PerformanceIssues)
                    Console.WriteLine($"   🐌 {issue}");
                Console.WriteLine();
            }

            Console.WriteLine("💡 RECOMMENDATIONS");
            foreach (var recommendation in result.Recommendations)
                Console.WriteLine($"   ✅ {recommendation}");
            Console.WriteLine();

            Console.WriteLine("📦 KEY DEPENDENCIES");
            var keyDeps = result.Dependencies.Where(d =>
                !d.StartsWith("System.") ||
                d.Contains("Http") ||
                d.Contains("Threading") ||
                d.Contains("Newtonsoft")).Take(8);
            foreach (var dep in keyDeps)
                Console.WriteLine($"   📋 {dep}");

            Console.WriteLine();
            Console.WriteLine("=".PadRight(80, '='));
        }

        public static void RunAnalysis(string filePath)
        {
            try
            {
                var result = AnalyzeFile(filePath);
                PrintAnalysis(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Analysis failed: {ex.Message}");
            }
        }
    }

    // Quick runner for the analysis
    public class Program
    {
        public static void Main(string[] args)
        {
            var targetFile = @"c:\Users\steve.mckitrick\Desktop\Rate Study\WileyBudgetManagement\AIEnhancedQueryService.cs";

            Console.WriteLine("🔍 WILEY BUDGET MANAGEMENT - CODE ANALYSIS TOOL");
            Console.WriteLine($"Target: {Path.GetFileName(targetFile)}");
            Console.WriteLine();

            CodeAnalysisTool.RunAnalysis(targetFile);

            Console.WriteLine("\n⏱️  Analysis completed. Press any key to exit...");
            Console.ReadKey();
        }
    }
}
