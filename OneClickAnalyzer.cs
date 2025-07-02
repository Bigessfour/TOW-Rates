using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace WileyBudgetManagement.OneClick
{
    /// <summary>
    /// One-Click Code Analyzer for Town of Wiley Budget Management
    /// Just run this and it examines everything automatically
    /// </summary>
    class OneClickAnalyzer
    {
        private static string _targetFile;
        private static string _sourceCode;

        static void Main(string[] args)
        {
            Console.WriteLine("🏛️  TOWN OF WILEY - ONE-CLICK CODE EXAMINER");
            Console.WriteLine("=" + new string('=', 55));
            Console.WriteLine("🤖 AUTO-EXAMINING YOUR CODE...");
            Console.WriteLine();

            // Auto-find the target file
            FindTargetFile();

            if (string.IsNullOrEmpty(_targetFile))
            {
                Console.WriteLine("❌ Could not find AIEnhancedQueryService.cs");
                Console.WriteLine("📁 Searching in current directory for any .cs files...");
                AutoAnalyzeAnyCSFile();
                return;
            }

            // Load and analyze
            _sourceCode = File.ReadAllText(_targetFile);
            Console.WriteLine($"📁 Examining: {Path.GetFileName(_targetFile)}");
            Console.WriteLine($"📏 File size: {_sourceCode.Length} characters");
            Console.WriteLine();

            // Run all examinations
            ExamineCodeQuality();
            ExamineSecurityIssues();
            ExaminePerformanceIssues();
            ExamineMunicipalCompliance();
            ProvideActionPlan();

            Console.WriteLine();
            Console.WriteLine("✅ EXAMINATION COMPLETE!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void FindTargetFile()
        {
            // Try multiple possible locations
            var possiblePaths = new[]
            {
                @"WileyBudgetManagement\AIEnhancedQueryService.cs",
                @"AIEnhancedQueryService.cs",
                @"..\WileyBudgetManagement\AIEnhancedQueryService.cs",
                @"..\..\WileyBudgetManagement\AIEnhancedQueryService.cs"
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    _targetFile = Path.GetFullPath(path);
                    return;
                }
            }
        }

        static void AutoAnalyzeAnyCSFile()
        {
            var csFiles = Directory.GetFiles(".", "*.cs", SearchOption.AllDirectories)
                .Where(f => !f.Contains("bin") && !f.Contains("obj") && !f.Contains("OneClickAnalyzer"))
                .ToArray();

            if (!csFiles.Any())
            {
                Console.WriteLine("❌ No C# files found to analyze");
                return;
            }

            Console.WriteLine($"📂 Found {csFiles.Length} C# files:");
            foreach (var file in csFiles.Take(5))
            {
                Console.WriteLine($"   📄 {Path.GetFileName(file)}");
            }

            _targetFile = csFiles.First();
            _sourceCode = File.ReadAllText(_targetFile);
            Console.WriteLine($"\n🔍 Auto-analyzing: {Path.GetFileName(_targetFile)}");
            Console.WriteLine();

            ExamineCodeQuality();
            ExamineSecurityIssues();
            ExaminePerformanceIssues();
        }

        static void ExamineCodeQuality()
        {
            Console.WriteLine("📊 CODE QUALITY EXAMINATION");
            Console.WriteLine("-" + new string('-', 30));

            var lines = _sourceCode.Split('\n').Length;
            var methods = CountPattern(@"(public|private|protected|internal)\s+.*\s+\w+\s*\(");
            var classes = CountPattern(@"class\s+\w+");
            var complexity = CalculateComplexity();

            Console.WriteLine($"   📏 Lines of Code: {lines}");
            Console.WriteLine($"   🏗️  Classes: {classes}");
            Console.WriteLine($"   ⚙️  Methods: {methods}");
            Console.WriteLine($"   🧩 Complexity Score: {complexity}");

            // Quality assessment
            if (complexity > 20) Console.WriteLine("   ⚠️  HIGH COMPLEXITY - Consider refactoring");
            else if (complexity > 10) Console.WriteLine("   🔶 MODERATE COMPLEXITY - Room for improvement");
            else Console.WriteLine("   ✅ GOOD COMPLEXITY - Well structured");

            if (lines > 500) Console.WriteLine("   ⚠️  LARGE FILE - Consider splitting");
            else Console.WriteLine("   ✅ REASONABLE SIZE");

            Console.WriteLine();
        }

        static void ExamineSecurityIssues()
        {
            Console.WriteLine("🔒 SECURITY EXAMINATION");
            Console.WriteLine("-" + new string('-', 25));

            var issues = new List<string>();

            // Check for environment variables without validation
            if (_sourceCode.Contains("Environment.GetEnvironmentVariable") && !_sourceCode.Contains("ValidateApiKey"))
            {
                issues.Add("🚨 CRITICAL: API key used without validation");
            }

            // Check for hardcoded secrets
            if (Regex.IsMatch(_sourceCode, @"(password|secret|key|token)\s*=\s*[""'][^""']+[""']", RegexOptions.IgnoreCase))
            {
                issues.Add("🚨 CRITICAL: Potential hardcoded secrets");
            }

            // Check for HTTPS usage
            if (_sourceCode.Contains("http://") && !_sourceCode.Contains("https://"))
            {
                issues.Add("⚠️  WARNING: Non-HTTPS endpoints detected");
            }

            // Check for SQL injection risks
            if (_sourceCode.Contains("SqlCommand") && _sourceCode.Contains("+"))
            {
                issues.Add("🚨 CRITICAL: Potential SQL injection vulnerability");
            }

            if (!issues.Any())
            {
                Console.WriteLine("   ✅ NO MAJOR SECURITY ISSUES FOUND");
            }
            else
            {
                foreach (var issue in issues)
                {
                    Console.WriteLine($"   {issue}");
                }
            }
            Console.WriteLine();
        }

        static void ExaminePerformanceIssues()
        {
            Console.WriteLine("⚡ PERFORMANCE EXAMINATION");
            Console.WriteLine("-" + new string('-', 28));

            var issues = new List<string>();

            // Check for ConfigureAwait
            if (_sourceCode.Contains("await") && !_sourceCode.Contains("ConfigureAwait(false)"))
            {
                issues.Add("🔶 IMPROVEMENT: Missing ConfigureAwait(false) in async calls");
            }

            // Check for synchronous calls in async methods
            if (_sourceCode.Contains("async") && _sourceCode.Contains(".Result"))
            {
                issues.Add("⚠️  WARNING: Blocking async calls detected");
            }

            // Check for string concatenation
            if (CountPattern(@"\+\s*[""']") > 5)
            {
                issues.Add("🔶 IMPROVEMENT: Consider StringBuilder for string operations");
            }

            // Check for proper disposal
            if (_sourceCode.Contains("new HttpClient()") && _sourceCode.Contains("IDisposable"))
            {
                Console.WriteLine("   ✅ GOOD: Proper resource disposal implemented");
            }

            if (!issues.Any() && _sourceCode.Contains("ConfigureAwait"))
            {
                Console.WriteLine("   ✅ EXCELLENT ASYNC PERFORMANCE PATTERNS");
            }
            else
            {
                foreach (var issue in issues)
                {
                    Console.WriteLine($"   {issue}");
                }
            }
            Console.WriteLine();
        }

        static void ExamineMunicipalCompliance()
        {
            Console.WriteLine("🏛️  MUNICIPAL COMPLIANCE EXAMINATION");
            Console.WriteLine("-" + new string('-', 38));

            var hasLogging = _sourceCode.Contains("ILogger") || _sourceCode.Contains("Log.");
            var hasValidation = _sourceCode.Contains("Town of Wiley") || _sourceCode.Contains("Municipality");
            var hasErrorHandling = _sourceCode.Contains("try") && _sourceCode.Contains("catch");
            var hasDocumentation = CountPattern(@"///") > 10;

            Console.WriteLine($"   📋 Audit Logging: {(hasLogging ? "✅ PRESENT" : "❌ MISSING")}");
            Console.WriteLine($"   🏛️  Municipal Context: {(hasValidation ? "✅ PRESENT" : "❌ MISSING")}");
            Console.WriteLine($"   🛡️  Error Handling: {(hasErrorHandling ? "✅ PRESENT" : "❌ MISSING")}");
            Console.WriteLine($"   📚 Documentation: {(hasDocumentation ? "✅ GOOD" : "⚠️  MINIMAL")}");

            if (!hasLogging)
            {
                Console.WriteLine("   🚨 CRITICAL: Municipal software requires audit trails");
            }

            if (!hasValidation)
            {
                Console.WriteLine("   ⚠️  WARNING: Add Town of Wiley specific validation");
            }

            Console.WriteLine();
        }

        static void ProvideActionPlan()
        {
            Console.WriteLine("🎯 RECOMMENDED ACTION PLAN");
            Console.WriteLine("-" + new string('-', 30));

            Console.WriteLine("📍 IMMEDIATE ACTIONS (Week 1):");
            if (!_sourceCode.Contains("ValidateApiKey"))
            {
                Console.WriteLine("   1. 🔒 Add API key validation (1 hour)");
            }
            if (!_sourceCode.Contains("ILogger"))
            {
                Console.WriteLine("   2. 📋 Implement audit logging (2 hours)");
            }

            Console.WriteLine();
            Console.WriteLine("📍 SHORT TERM (Week 2-3):");
            if (!_sourceCode.Contains("ConfigureAwait"))
            {
                Console.WriteLine("   3. ⚡ Add ConfigureAwait(false) to async calls (0.5 hours)");
            }
            Console.WriteLine("   4. 🏛️  Add municipal data validation (2 hours)");

            Console.WriteLine();
            Console.WriteLine("📍 LONG TERM (Month 1):");
            if (CalculateComplexity() > 15)
            {
                Console.WriteLine("   5. 🔧 Refactor complex methods (4 hours)");
            }
            Console.WriteLine("   6. 🧪 Add comprehensive testing (6 hours)");

            Console.WriteLine();
            Console.WriteLine("💰 TOTAL ESTIMATED EFFORT: 8-15 hours");
            Console.WriteLine("🎯 EXPECTED ROI: High - Municipal compliance & security");
        }

        static int CountPattern(string pattern)
        {
            return Regex.Matches(_sourceCode, pattern, RegexOptions.IgnoreCase).Count;
        }

        static int CalculateComplexity()
        {
            int complexity = 1;
            complexity += CountPattern(@"\bif\b");
            complexity += CountPattern(@"\belse\b");
            complexity += CountPattern(@"\bwhile\b");
            complexity += CountPattern(@"\bfor\b");
            complexity += CountPattern(@"\bforeach\b");
            complexity += CountPattern(@"\bswitch\b");
            complexity += CountPattern(@"\bcatch\b");
            complexity += CountPattern(@"\?\?");
            return complexity;
        }
    }
}
