using System;
using System.Drawing;
using System.Windows.Forms;

namespace WileyBudgetManagement.Forms
{
    /// <summary>
    /// Central UI Style Manager for Town of Wiley Budget Management System
    /// Provides consistent theming, styling, and UI standards across all forms
    /// Implements modern Material Design-inspired styling for Windows Forms
    /// </summary>
    public static class UIStyleManager
    {
        #region Color Palette - Consistent Material Design Colors
        
        // Primary Colors - Town of Wiley Blue Theme
        public static readonly Color PrimaryBlue = Color.FromArgb(33, 150, 243);        // #2196F3
        public static readonly Color PrimaryBlueDark = Color.FromArgb(21, 101, 192);   // #1565C0
        public static readonly Color PrimaryBlueLight = Color.FromArgb(144, 202, 249); // #90CAF9
        
        // Secondary Colors - Professional Accent
        public static readonly Color SecondaryGreen = Color.FromArgb(76, 175, 80);      // #4CAF50
        public static readonly Color SecondaryGreenDark = Color.FromArgb(56, 142, 60);  // #388E3C
        public static readonly Color SecondaryOrange = Color.FromArgb(255, 152, 0);     // #FF9800
        
        // Neutral Colors - Professional Gray Scale
        public static readonly Color NeutralDark = Color.FromArgb(33, 33, 33);         // #212121
        public static readonly Color NeutralMedium = Color.FromArgb(97, 97, 97);       // #616161
        public static readonly Color NeutralLight = Color.FromArgb(238, 238, 238);     // #EEEEEE
        public static readonly Color NeutralOffWhite = Color.FromArgb(250, 250, 250);  // #FAFAFA
        
        // Status Colors - User Feedback
        public static readonly Color StatusSuccess = Color.FromArgb(76, 175, 80);      // #4CAF50
        public static readonly Color StatusWarning = Color.FromArgb(255, 193, 7);      // #FFC107
        public static readonly Color StatusError = Color.FromArgb(244, 67, 54);        // #F44336
        public static readonly Color StatusInfo = Color.FromArgb(33, 150, 243);        // #2196F3
        
        // Surface Colors
        public static readonly Color Surface = Color.White;
        public static readonly Color SurfaceVariant = Color.FromArgb(248, 249, 250);   // #F8F9FA
        public static readonly Color SurfaceDark = Color.FromArgb(245, 245, 245);      // #F5F5F5
        
        #endregion

        #region Typography - Modern Font System
        
        // Primary Font Family
        public static readonly string PrimaryFontFamily = "Segoe UI";
        public static readonly string FallbackFontFamily = "Arial";
        
        // Font Sizes (scaled for Windows Forms DPI)
        public static readonly float FontSizeHeading1 = 24f;  // Major headings
        public static readonly float FontSizeHeading2 = 20f;  // Section headers
        public static readonly float FontSizeHeading3 = 16f;  // Subsection headers
        public static readonly float FontSizeBody = 11f;      // Body text
        public static readonly float FontSizeCaption = 9f;    // Small text/captions
        public static readonly float FontSizeButton = 10f;    // Button text
        
        // Font Styles
        public static Font GetFont(float size, FontStyle style = FontStyle.Regular)
        {
            try
            {
                return new Font(PrimaryFontFamily, size, style);
            }
            catch
            {
                return new Font(FallbackFontFamily, size, style);
            }
        }
        
        // Pre-defined Font Sets
        public static readonly Font HeadingFont = GetFont(FontSizeHeading1, FontStyle.Bold);
        public static readonly Font SubheadingFont = GetFont(FontSizeHeading2, FontStyle.Bold);
        public static readonly Font SectionFont = GetFont(FontSizeHeading3, FontStyle.Bold);
        public static readonly Font BodyFont = GetFont(FontSizeBody);
        public static readonly Font CaptionFont = GetFont(FontSizeCaption);
        public static readonly Font ButtonFont = GetFont(FontSizeButton, FontStyle.Regular);
        
        #endregion

        #region Button Styling - Modern Flat Design
        
        /// <summary>
        /// Primary action button - Main CTA buttons
        /// </summary>
        public static void ApplyPrimaryButtonStyle(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = PrimaryBlue;
            button.ForeColor = Color.White;
            button.Font = ButtonFont;
            button.Size = new Size(Math.Max(button.Size.Width, 120), 36);
            button.Cursor = Cursors.Hand;
            
            // Enhanced padding for better touch targets
            button.Padding = new Padding(16, 8, 16, 8);
            
            // Modern flat appearance
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = PrimaryBlueDark;
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(13, 71, 161); // Even darker
            
            // Accessibility improvements
            button.UseVisualStyleBackColor = false;
            button.TabStop = true;
        }
        
        /// <summary>
        /// Secondary action button - Supporting actions
        /// </summary>
        public static void ApplySecondaryButtonStyle(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = NeutralLight;
            button.ForeColor = NeutralDark;
            button.Font = ButtonFont;
            button.Size = new Size(Math.Max(button.Size.Width, 100), 36);
            button.Cursor = Cursors.Hand;
            
            button.Padding = new Padding(14, 8, 14, 8);
            
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = NeutralMedium;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(224, 224, 224);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(189, 189, 189);
            
            button.UseVisualStyleBackColor = false;
            button.TabStop = true;
        }
        
        /// <summary>
        /// Success button - Positive actions (Save, Confirm)
        /// </summary>
        public static void ApplySuccessButtonStyle(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = StatusSuccess;
            button.ForeColor = Color.White;
            button.Font = ButtonFont;
            button.Size = new Size(Math.Max(button.Size.Width, 120), 36);
            button.Cursor = Cursors.Hand;
            
            button.Padding = new Padding(16, 8, 16, 8);
            
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = SecondaryGreenDark;
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(27, 94, 32);
            
            button.UseVisualStyleBackColor = false;
            button.TabStop = true;
        }
        
        /// <summary>
        /// Warning/Danger button - Destructive actions (Delete)
        /// </summary>
        public static void ApplyWarningButtonStyle(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = StatusError;
            button.ForeColor = Color.White;
            button.Font = ButtonFont;
            button.Size = new Size(Math.Max(button.Size.Width, 100), 36);
            button.Cursor = Cursors.Hand;
            
            button.Padding = new Padding(14, 8, 14, 8);
            
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(198, 40, 40);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 28, 28);
            
            button.UseVisualStyleBackColor = false;
            button.TabStop = true;
        }
        
        #endregion

        #region Panel and Layout Styling
        
        /// <summary>
        /// Main content panel styling
        /// </summary>
        public static void ApplyMainPanelStyle(Panel panel)
        {
            panel.BackColor = Surface;
            panel.Padding = new Padding(20);
            panel.Dock = DockStyle.Fill;
        }
        
        /// <summary>
        /// Toolbar panel styling
        /// </summary>
        public static void ApplyToolbarPanelStyle(Panel panel)
        {
            panel.BackColor = SurfaceVariant;
            panel.Height = 60;
            panel.Dock = DockStyle.Top;
            panel.Padding = new Padding(16, 12, 16, 12);
            
            // Add subtle border bottom
            panel.Paint += (sender, e) =>
            {
                using (var pen = new Pen(NeutralLight, 1))
                {
                    e.Graphics.DrawLine(pen, 0, panel.Height - 1, panel.Width, panel.Height - 1);
                }
            };
        }
        
        /// <summary>
        /// Card-style panel for data sections
        /// </summary>
        public static void ApplyCardPanelStyle(Panel panel)
        {
            panel.BackColor = Surface;
            panel.Padding = new Padding(16);
            
            // Add subtle shadow effect via border
            panel.Paint += (sender, e) =>
            {
                var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
                using (var pen = new Pen(Color.FromArgb(0, 0, 0, 20), 1))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };
        }
        
        /// <summary>
        /// Navigation panel styling
        /// </summary>
        public static void ApplyNavigationPanelStyle(Panel panel)
        {
            panel.BackColor = NeutralDark;
            panel.Width = 280;
            panel.Dock = DockStyle.Left;
            panel.Padding = new Padding(12);
        }
        
        #endregion

        #region Form Styling
        
        /// <summary>
        /// Apply consistent form styling
        /// </summary>
        public static void ApplyFormStyle(Form form, string title)
        {
            form.Font = BodyFont;
            form.BackColor = SurfaceDark;
            form.Text = title;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimumSize = new Size(800, 600);
            
            // Ensure form has proper icon if available
            try
            {
                // Add form icon logic here if you have a .ico file
                // form.Icon = new Icon("path/to/icon.ico");
            }
            catch
            {
                // Ignore icon loading errors
            }
        }
        
        #endregion

        #region Label Styling
        
        /// <summary>
        /// Primary heading label
        /// </summary>
        public static void ApplyHeadingLabelStyle(Label label)
        {
            label.Font = HeadingFont;
            label.ForeColor = NeutralDark;
            label.AutoSize = true;
        }
        
        /// <summary>
        /// Section header label
        /// </summary>
        public static void ApplySectionLabelStyle(Label label)
        {
            label.Font = SectionFont;
            label.ForeColor = NeutralMedium;
            label.AutoSize = true;
        }
        
        /// <summary>
        /// Body text label
        /// </summary>
        public static void ApplyBodyLabelStyle(Label label)
        {
            label.Font = BodyFont;
            label.ForeColor = NeutralDark;
            label.AutoSize = true;
        }
        
        /// <summary>
        /// Status label with color coding
        /// </summary>
        public static void ApplyStatusLabelStyle(Label label, StatusType status = StatusType.Info)
        {
            label.Font = CaptionFont;
            label.AutoSize = true;
            
            switch (status)
            {
                case StatusType.Success:
                    label.ForeColor = StatusSuccess;
                    break;
                case StatusType.Warning:
                    label.ForeColor = StatusWarning;
                    break;
                case StatusType.Error:
                    label.ForeColor = StatusError;
                    break;
                case StatusType.Info:
                default:
                    label.ForeColor = StatusInfo;
                    break;
            }
        }
        
        #endregion

        #region Input Control Styling
        
        /// <summary>
        /// Text box styling for consistency
        /// </summary>
        public static void ApplyTextBoxStyle(TextBox textBox)
        {
            textBox.Font = BodyFont;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = Surface;
            textBox.ForeColor = NeutralDark;
            textBox.Height = 28;
            
            // Add focus styling
            textBox.Enter += (sender, e) =>
            {
                textBox.BackColor = Color.FromArgb(240, 248, 255); // Light blue tint
            };
            
            textBox.Leave += (sender, e) =>
            {
                textBox.BackColor = Surface;
            };
        }
        
        /// <summary>
        /// ComboBox styling for consistency
        /// </summary>
        public static void ApplyComboBoxStyle(ComboBox comboBox)
        {
            comboBox.Font = BodyFont;
            comboBox.BackColor = Surface;
            comboBox.ForeColor = NeutralDark;
            comboBox.Height = 28;
            comboBox.FlatStyle = FlatStyle.Flat;
        }
        
        #endregion

        #region DataGrid Styling (Syncfusion)
        
        /// <summary>
        /// Apply consistent styling to Syncfusion DataGrid
        /// </summary>
        public static void ApplySyncfusionDataGridStyle(dynamic dataGrid)
        {
            try
            {
                // Note: Using dynamic to avoid direct Syncfusion references
                // These properties should work with SfDataGrid
                
                dataGrid.Style.HeaderStyle.BackColor = SurfaceVariant;
                dataGrid.Style.HeaderStyle.TextColor = NeutralDark;
                dataGrid.Style.HeaderStyle.Font = SectionFont;
                
                dataGrid.Style.CellStyle.BackColor = Surface;
                dataGrid.Style.CellStyle.TextColor = NeutralDark;
                dataGrid.Style.CellStyle.Font = BodyFont;
                
                dataGrid.Style.SelectionStyle.BackColor = PrimaryBlueLight;
                dataGrid.Style.SelectionStyle.TextColor = NeutralDark;
                
                // Grid lines
                dataGrid.Style.BorderStyle = BorderStyle.FixedSingle;
                dataGrid.GridLinesVisibility = true; // Assuming this property exists
            }
            catch (Exception ex)
            {
                // Log styling error but don't crash
                System.Diagnostics.Debug.WriteLine($"DataGrid styling error: {ex.Message}");
            }
        }
        
        #endregion

        #region Utility Methods
        
        /// <summary>
        /// Create a modern separator line
        /// </summary>
        public static Panel CreateSeparator(int height = 1)
        {
            return new Panel
            {
                Height = height,
                Dock = DockStyle.Top,
                BackColor = NeutralLight
            };
        }
        
        /// <summary>
        /// Create a spacer panel for consistent spacing
        /// </summary>
        public static Panel CreateSpacer(int height = 10)
        {
            return new Panel
            {
                Height = height,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };
        }
        
        /// <summary>
        /// Validate color contrast for accessibility (WCAG 2.1 AA)
        /// </summary>
        public static bool IsContrastAccessible(Color foreground, Color background)
        {
            var contrastRatio = CalculateContrastRatio(foreground, background);
            return contrastRatio >= 4.5; // WCAG AA standard
        }
        
        private static double CalculateContrastRatio(Color color1, Color color2)
        {
            var l1 = GetRelativeLuminance(color1);
            var l2 = GetRelativeLuminance(color2);
            
            var lighter = Math.Max(l1, l2);
            var darker = Math.Min(l1, l2);
            
            return (lighter + 0.05) / (darker + 0.05);
        }
        
        private static double GetRelativeLuminance(Color color)
        {
            var r = color.R / 255.0;
            var g = color.G / 255.0;
            var b = color.B / 255.0;
            
            r = r <= 0.03928 ? r / 12.92 : Math.Pow((r + 0.055) / 1.055, 2.4);
            g = g <= 0.03928 ? g / 12.92 : Math.Pow((g + 0.055) / 1.055, 2.4);
            b = b <= 0.03928 ? b / 12.92 : Math.Pow((b + 0.055) / 1.055, 2.4);
            
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }
        
        #endregion

        #region Enums
        
        public enum StatusType
        {
            Success,
            Warning,
            Error,
            Info
        }
        
        #endregion
    }
}
