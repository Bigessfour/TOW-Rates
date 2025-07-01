using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using WileyBudgetManagement.Models;

namespace WileyBudgetManagement.Forms
{
    public static class ValidationHelper
    {
        // Account number validation - alphanumeric with specific prefixes
        public static bool ValidateAccount(string account, string expectedPrefix = "")
        {
            if (string.IsNullOrWhiteSpace(account))
                return false;

            // Check length (3-10 characters)
            if (account.Length < 3 || account.Length > 10)
                return false;

            // Check alphanumeric pattern
            if (!Regex.IsMatch(account, @"^[A-Z0-9]+$"))
                return false;

            // Check expected prefix if provided
            if (!string.IsNullOrEmpty(expectedPrefix) && !account.StartsWith(expectedPrefix))
                return false;

            return true;
        }

        // Budget amount validation
        public static bool ValidateBudgetAmount(decimal amount, decimal minValue = 0, decimal maxValue = 10000000)
        {
            return amount >= minValue && amount <= maxValue;
        }

        // Percentage validation (0-100% or 0-1 decimal)
        public static bool ValidatePercentage(decimal percentage, bool isDecimalFormat = true)
        {
            if (isDecimalFormat)
                return percentage >= 0 && percentage <= 1;
            else
                return percentage >= 0 && percentage <= 100;
        }

        // Monthly usage validation
        public static bool ValidateUsage(decimal usage, decimal minValue = 0, decimal maxValue = 100000000)
        {
            return usage >= minValue && usage <= maxValue;
        }

        // Rate validation
        public static bool ValidateRate(decimal rate, decimal minValue = 0, decimal maxValue = 10000)
        {
            return rate >= minValue && rate <= maxValue;
        }

        // Label/Description validation
        public static bool ValidateLabel(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
                return false;

            // Check length (3-100 characters)
            if (label.Length < 3 || label.Length > 100)
                return false;

            // Check for valid characters (letters, numbers, spaces, common punctuation)
            return Regex.IsMatch(label, @"^[A-Za-z0-9\s\-_.,()&]+$");
        }

        // Section validation
        public static bool ValidateSection(string section)
        {
            if (string.IsNullOrWhiteSpace(section))
                return false;

            // Check length (2-50 characters)
            if (section.Length < 2 || section.Length > 50)
                return false;

            // Check for valid characters
            return Regex.IsMatch(section, @"^[A-Za-z0-9\s\-_]+$");
        }

        // Customer affordability index validation (0.5 to 1.5 typical range)
        public static bool ValidateAffordabilityIndex(decimal index)
        {
            return index >= 0.1m && index <= 2.0m;
        }

        // Time of use factor validation (typically 0.5 to 2.0)
        public static bool ValidateTimeOfUseFactor(decimal factor)
        {
            return factor >= 0.1m && factor <= 3.0m;
        }

        // Comprehensive validation for SanitationDistrict
        public static ValidationResult ValidateSanitationDistrict(SanitationDistrict district, string category)
        {
            var result = new ValidationResult();

            // Account validation
            string expectedPrefix = category switch
            {
                "Water" => "W",
                "Trash" => "T", 
                "Apartments" => "APT",
                _ => ""
            };

            if (!ValidateAccount(district.Account, expectedPrefix))
            {
                result.AddError("Account", $"Invalid account number. Expected format: {expectedPrefix}### (3-10 alphanumeric characters)");
            }

            // Label validation
            if (!ValidateLabel(district.Label))
            {
                result.AddError("Label", "Label must be 3-100 characters, letters, numbers, spaces and common punctuation only");
            }

            // Section validation
            if (!ValidateSection(district.Section))
            {
                result.AddError("Section", "Section must be 2-50 characters, alphanumeric with spaces, hyphens, underscores only");
            }

            // Budget validations
            if (!ValidateBudgetAmount(district.CurrentFYBudget))
            {
                result.AddError("CurrentFYBudget", "Current FY Budget must be between $0 and $10,000,000");
            }

            if (district.MonthlyInput < 0 || district.MonthlyInput > district.CurrentFYBudget)
            {
                result.AddError("MonthlyInput", "Monthly Input must be positive and not exceed annual budget");
            }

            if (district.YearToDateSpending < 0 || district.YearToDateSpending > district.CurrentFYBudget)
            {
                result.AddError("YearToDateSpending", "Year to Date Spending must be positive and not exceed annual budget");
            }

            if (!ValidatePercentage(district.PercentOfBudget))
            {
                result.AddError("PercentOfBudget", "Percent of Budget must be between 0% and 100% (0.0 to 1.0)");
            }

            // Usage and rate validations
            if (district.MonthlyUsage < 0)
            {
                result.AddError("MonthlyUsage", "Monthly Usage cannot be negative");
            }

            if (!ValidateRate(district.RequiredRate))
            {
                result.AddError("RequiredRate", "Required Rate must be between $0 and $10,000");
            }

            // Specialized validations for apartments
            if (category == "Apartments")
            {
                if (district.CustomerAffordabilityIndex > 0 && !ValidateAffordabilityIndex(district.CustomerAffordabilityIndex))
                {
                    result.AddError("CustomerAffordabilityIndex", "Customer Affordability Index must be between 0.1 and 2.0");
                }
            }

            // Water-specific validations
            if (category == "Water")
            {
                if (district.TimeOfUseFactor > 0 && !ValidateTimeOfUseFactor(district.TimeOfUseFactor))
                {
                    result.AddError("TimeOfUseFactor", "Time of Use Factor must be between 0.1 and 3.0");
                }
            }

            // Seasonal adjustment validation for trash
            if (category == "Trash")
            {
                if (district.SeasonalAdjustment < 0 || district.SeasonalAdjustment > district.CurrentFYBudget * 0.5m)
                {
                    result.AddError("SeasonalAdjustment", "Seasonal Adjustment must be positive and not exceed 50% of annual budget");
                }
            }

            return result;
        }

        // Business logic validations
        public static ValidationResult ValidateBusinessRules(SanitationDistrict district)
        {
            var result = new ValidationResult();

            // Budget remaining should equal budget minus YTD spending
            decimal calculatedRemaining = district.CurrentFYBudget - district.YearToDateSpending;
            if (Math.Abs(district.BudgetRemaining - calculatedRemaining) > 0.01m)
            {
                result.AddWarning("BudgetRemaining", $"Budget Remaining should be {calculatedRemaining:C} based on Budget - YTD Spending");
            }

            // Percent of budget should match YTD / Budget
            if (district.CurrentFYBudget > 0)
            {
                decimal calculatedPercent = district.YearToDateSpending / district.CurrentFYBudget;
                if (Math.Abs(district.PercentOfBudget - calculatedPercent) > 0.01m)
                {
                    result.AddWarning("PercentOfBudget", $"Percent of Budget should be {calculatedPercent:P2} based on YTD/Budget");
                }
            }

            // Monthly input should be reasonable compared to annual budget
            if (district.CurrentFYBudget > 0 && district.MonthlyInput > 0)
            {
                decimal annualFromMonthly = district.MonthlyInput * 12;
                decimal variance = Math.Abs(annualFromMonthly - district.CurrentFYBudget) / district.CurrentFYBudget;
                if (variance > 0.1m) // More than 10% variance
                {
                    result.AddWarning("MonthlyInput", $"Monthly Input * 12 ({annualFromMonthly:C}) differs significantly from Annual Budget ({district.CurrentFYBudget:C})");
                }
            }

            return result;
        }
    }

    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<ValidationError> Errors { get; } = new List<ValidationError>();
        public List<ValidationError> Warnings { get; } = new List<ValidationError>();

        public void AddError(string field, string message)
        {
            Errors.Add(new ValidationError { Field = field, Message = message, Type = ValidationErrorType.Error });
        }

        public void AddWarning(string field, string message)
        {
            Warnings.Add(new ValidationError { Field = field, Message = message, Type = ValidationErrorType.Warning });
        }

        public string GetErrorSummary()
        {
            if (IsValid && Warnings.Count == 0)
                return "All validations passed.";

            var summary = "";
            if (Errors.Count > 0)
            {
                summary += $"Errors ({Errors.Count}):\n";
                summary += string.Join("\n", Errors.Select(e => $"• {e.Field}: {e.Message}"));
            }

            if (Warnings.Count > 0)
            {
                if (summary.Length > 0) summary += "\n\n";
                summary += $"Warnings ({Warnings.Count}):\n";
                summary += string.Join("\n", Warnings.Select(w => $"• {w.Field}: {w.Message}"));
            }

            return summary;
        }
    }

    public class ValidationError
    {
        public string Field { get; set; } = "";
        public string Message { get; set; } = "";
        public ValidationErrorType Type { get; set; }
    }

    public enum ValidationErrorType
    {
        Error,
        Warning
    }
}
