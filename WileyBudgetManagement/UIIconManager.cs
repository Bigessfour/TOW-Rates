using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WileyBudgetManagement.Forms
{
    /// <summary>
    /// UI Icon Manager for consistent icon handling across the application
    /// Provides fallback text when Unicode symbols are not supported
    /// </summary>
    public static class UIIconManager
    {
        #region Icon Definitions with Fallbacks
        
        public static readonly Dictionary<string, string> Icons = new Dictionary<string, string>
        {
            // Navigation Icons
            { "Dashboard", GetIcon("🏠", "[Home]") },
            { "Water", GetIcon("💧", "[Water]") },
            { "Sanitation", GetIcon("🚿", "[Sanitation]") },
            { "Trash", GetIcon("🗑️", "[Trash]") },
            { "Apartments", GetIcon("🏢", "[Apartments]") },
            { "AI", GetIcon("🤖", "[AI]") },
            { "Resources", GetIcon("📚", "[Resources]") },
            { "Summary", GetIcon("📈", "[Summary]") },
            { "Reports", GetIcon("📋", "[Reports]") },
            { "Settings", GetIcon("⚙️", "[Settings]") },
            
            // Action Icons
            { "Save", GetIcon("💾", "[Save]") },
            { "Add", GetIcon("➕", "[Add]") },
            { "Delete", GetIcon("🗑️", "[Delete]") },
            { "Edit", GetIcon("✏️", "[Edit]") },
            { "Refresh", GetIcon("🔄", "[Refresh]") },
            { "Export", GetIcon("📤", "[Export]") },
            { "Import", GetIcon("📥", "[Import]") },
            { "Print", GetIcon("🖨️", "[Print]") },
            { "Search", GetIcon("🔍", "[Search]") },
            { "Chart", GetIcon("📊", "[Chart]") },
            
            // Status Icons
            { "Success", GetIcon("✅", "[OK]") },
            { "Warning", GetIcon("⚠️", "[Warning]") },
            { "Error", GetIcon("❌", "[Error]") },
            { "Info", GetIcon("ℹ️", "[Info]") },
            { "Loading", GetIcon("⏳", "[Loading]") },
            
            // Business Icons
            { "Budget", GetIcon("💰", "[Budget]") },
            { "Revenue", GetIcon("💵", "[Revenue]") },
            { "Expense", GetIcon("💸", "[Expense]") },
            { "Customer", GetIcon("👤", "[Customer]") },
            { "Rate", GetIcon("📊", "[Rate]") },
            { "Analysis", GetIcon("🔬", "[Analysis]") }
        };
        
        #endregion

        #region Helper Methods
        
        /// <summary>
        /// Get icon with fallback text if Unicode is not supported
        /// </summary>
        private static string GetIcon(string unicodeIcon, string fallbackText)
        {
            try
            {
                // Test if the current system can render Unicode properly
                if (CanRenderUnicode())
                {
                    return unicodeIcon;
                }
                return fallbackText;
            }
            catch
            {
                return fallbackText;
            }
        }
        
        /// <summary>
        /// Check if the current system can render Unicode symbols properly
        /// </summary>
        private static bool CanRenderUnicode()
        {
            // Simple heuristic: check if we're on a modern Windows version
            var osVersion = Environment.OSVersion;
            if (osVersion.Platform == PlatformID.Win32NT && osVersion.Version.Major >= 10)
            {
                return true; // Windows 10+ generally supports Unicode well
            }
            
            // For older systems, use fallback text
            return false;
        }
        
        /// <summary>
        /// Get an icon by name with automatic fallback
        /// </summary>
        public static string GetIcon(string iconName)
        {
            return Icons.TryGetValue(iconName, out string? icon) ? icon : $"[{iconName}]";
        }
        
        /// <summary>
        /// Create a button text with icon
        /// </summary>
        public static string CreateButtonText(string iconName, string text)
        {
            var icon = GetIcon(iconName);
            return $"{icon} {text}";
        }
        
        /// <summary>
        /// Create a label text with status icon
        /// </summary>
        public static string CreateStatusText(string iconName, string text)
        {
            var icon = GetIcon(iconName);
            return $"{icon} {text}";
        }
        
        #endregion

        #region Font Awesome Integration (Future Enhancement)
        
        /// <summary>
        /// Integration point for Font Awesome icons if needed in the future
        /// </summary>
        public static Font GetIconFont(float size = 12f)
        {
            try
            {
                // Try to use Font Awesome if available
                return new Font("Font Awesome 5 Free", size, FontStyle.Regular);
            }
            catch
            {
                // Fallback to Segoe UI Symbol for basic icons
                try
                {
                    return new Font("Segoe UI Symbol", size, FontStyle.Regular);
                }
                catch
                {
                    // Final fallback to system font
                    return SystemFonts.DefaultFont;
                }
            }
        }
        
        #endregion

        #region Accessibility Improvements
        
        /// <summary>
        /// Get accessible text description for screen readers
        /// </summary>
        public static string GetAccessibleText(string iconName, string text)
        {
            return $"{iconName}: {text}";
        }
        
        /// <summary>
        /// Apply accessible properties to a control with icon
        /// </summary>
        public static void ApplyAccessibleProperties(Control control, string iconName, string description)
        {
            control.AccessibleName = GetAccessibleText(iconName, description);
            control.AccessibleDescription = description;
            control.AccessibleRole = AccessibleRole.PushButton;
        }
        
        #endregion
    }
}
