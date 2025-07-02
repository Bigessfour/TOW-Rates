using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WileyBudgetManagement.Forms
{
    /// <summary>
    /// Extension methods and helper utilities for applying enhanced UI styling
    /// to existing Windows Forms without major code restructuring
    /// Provides quick wins for immediate UI improvements
    /// </summary>
    public static class UIEnhancementExtensions
    {
        #region Form Enhancement Extensions
        
        /// <summary>
        /// Apply modern styling to an existing form and all its controls
        /// </summary>
        public static void ApplyModernStyling(this Form form)
        {
            try
            {
                // Apply base form styling
                form.BackColor = UIStyleManager.SurfaceDark;
                form.Font = UIStyleManager.BodyFont;
                
                // Style all controls recursively
                ApplyModernStylingToControls(form.Controls);
                
                // Add form-level enhancements
                EnhanceFormAccessibility(form);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Form styling error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Quick enhancement for toolbar panels
        /// </summary>
        public static void EnhanceToolbar(this Panel toolbarPanel)
        {
            UIStyleManager.ApplyToolbarPanelStyle(toolbarPanel);
            
            // Style all buttons in toolbar
            foreach (Control control in toolbarPanel.Controls)
            {
                if (control is Button button)
                {
                    ApplySmartButtonStyling(button);
                }
            }
        }
        
        /// <summary>
        /// Enhance data entry panels with card-like styling
        /// </summary>
        public static void EnhanceAsCard(this Panel panel)
        {
            UIStyleManager.ApplyCardPanelStyle(panel);
        }
        
        #endregion

        #region Smart Control Styling
        
        private static void ApplyModernStylingToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                try
                {
                    switch (control)
                    {
                        case Button button:
                            ApplySmartButtonStyling(button);
                            break;
                            
                        case Panel panel:
                            ApplySmartPanelStyling(panel);
                            break;
                            
                        case Label label:
                            ApplySmartLabelStyling(label);
                            break;
                            
                        case TextBox textBox:
                            UIStyleManager.ApplyTextBoxStyle(textBox);
                            break;
                            
                        case ComboBox comboBox:
                            UIStyleManager.ApplyComboBoxStyle(comboBox);
                            break;
                            
                        case DataGridView dataGridView:
                            ApplyDataGridViewStyling(dataGridView);
                            break;
                            
                        case TabControl tabControl:
                            ApplyTabControlStyling(tabControl);
                            break;
                    }

                    // Recursively style child controls
                    if (control.HasChildren)
                    {
                        ApplyModernStylingToControls(control.Controls);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Control styling error for {control.GetType().Name}: {ex.Message}");
                }
            }
        }
        
        private static void ApplySmartButtonStyling(Button button)
        {
            var buttonText = button.Text.ToLower();
            
            // Apply styling based on button purpose
            if (buttonText.Contains("save") || buttonText.Contains("validate") || buttonText.Contains("confirm"))
            {
                UIStyleManager.ApplySuccessButtonStyle(button);
            }
            else if (buttonText.Contains("delete") || buttonText.Contains("remove") || buttonText.Contains("clear"))
            {
                UIStyleManager.ApplyWarningButtonStyle(button);
            }
            else if (buttonText.Contains("generate") || buttonText.Contains("export") || buttonText.Contains("refresh"))
            {
                UIStyleManager.ApplyPrimaryButtonStyle(button);
            }
            else
            {
                UIStyleManager.ApplySecondaryButtonStyle(button);
            }
            
            // Add hover enhancement
            AddButtonHoverEffect(button);
        }
        
        private static void ApplySmartPanelStyling(Panel panel)
        {
            // Determine panel type and apply appropriate styling
            if (panel.Dock == DockStyle.Top && panel.Height < 100)
            {
                // Likely a toolbar
                UIStyleManager.ApplyToolbarPanelStyle(panel);
            }
            else if (panel.Parent is Form)
            {
                // Main content panel
                UIStyleManager.ApplyMainPanelStyle(panel);
            }
            else
            {
                // General panel - apply card styling for better organization
                panel.BackColor = UIStyleManager.Surface;
            }
        }
        
        private static void ApplySmartLabelStyling(Label label)
        {
            // Determine label importance based on font and position
            if (label.Font?.Bold == true || label.Font?.Size >= 14)
            {
                UIStyleManager.ApplyHeadingLabelStyle(label);
            }
            else if (label.Dock == DockStyle.Top || label.Text.EndsWith(":"))
            {
                UIStyleManager.ApplySectionLabelStyle(label);
            }
            else
            {
                UIStyleManager.ApplyBodyLabelStyle(label);
            }
        }
        
        #endregion

        #region Specialized Control Enhancements
        
        private static void ApplyDataGridViewStyling(DataGridView gridView)
        {
            try
            {
                // Modern DataGridView styling
                gridView.BackgroundColor = UIStyleManager.Surface;
                gridView.GridColor = UIStyleManager.NeutralLight;
                gridView.BorderStyle = BorderStyle.Fixed3D;
                gridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                
                // Header styling
                gridView.EnableHeadersVisualStyles = false;
                gridView.ColumnHeadersDefaultCellStyle.BackColor = UIStyleManager.SurfaceVariant;
                gridView.ColumnHeadersDefaultCellStyle.ForeColor = UIStyleManager.NeutralDark;
                gridView.ColumnHeadersDefaultCellStyle.Font = UIStyleManager.SectionFont;
                gridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                gridView.ColumnHeadersHeight = 35;
                
                // Cell styling
                gridView.DefaultCellStyle.BackColor = UIStyleManager.Surface;
                gridView.DefaultCellStyle.ForeColor = UIStyleManager.NeutralDark;
                gridView.DefaultCellStyle.Font = UIStyleManager.BodyFont;
                gridView.DefaultCellStyle.SelectionBackColor = UIStyleManager.PrimaryBlueLight;
                gridView.DefaultCellStyle.SelectionForeColor = UIStyleManager.NeutralDark;
                gridView.RowTemplate.Height = 28;
                
                // Alternating row colors for better readability
                gridView.AlternatingRowsDefaultCellStyle.BackColor = UIStyleManager.SurfaceVariant;
                
                // Remove row headers for cleaner look
                gridView.RowHeadersVisible = false;
                
                // Selection mode
                gridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                gridView.MultiSelect = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DataGridView styling error: {ex.Message}");
            }
        }
        
        private static void ApplyTabControlStyling(TabControl tabControl)
        {
            try
            {
                tabControl.Font = UIStyleManager.BodyFont;
                
                // Style individual tab pages
                foreach (TabPage tabPage in tabControl.TabPages)
                {
                    tabPage.BackColor = UIStyleManager.Surface;
                    tabPage.Font = UIStyleManager.BodyFont;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TabControl styling error: {ex.Message}");
            }
        }
        
        #endregion

        #region Interactive Enhancements
        
        private static void AddButtonHoverEffect(Button button)
        {
            try
            {
                var originalBackColor = button.BackColor;
                
                button.MouseEnter += (sender, e) =>
                {
                    button.FlatAppearance.BorderSize = 1;
                    button.FlatAppearance.BorderColor = UIStyleManager.PrimaryBlue;
                };
                
                button.MouseLeave += (sender, e) =>
                {
                    button.FlatAppearance.BorderSize = 0;
                };
                
                // Add subtle animation effect
                button.Enter += (sender, e) =>
                {
                    // Focus styling
                    button.FlatAppearance.BorderSize = 2;
                    button.FlatAppearance.BorderColor = UIStyleManager.PrimaryBlueDark;
                };
                
                button.Leave += (sender, e) =>
                {
                    button.FlatAppearance.BorderSize = 0;
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Button hover effect error: {ex.Message}");
            }
        }
        
        #endregion

        #region Accessibility Enhancements
        
        private static void EnhanceFormAccessibility(Form form)
        {
            try
            {
                // Improve tab order
                var controls = GetAllControls(form).Where(c => c.CanSelect).ToList();
                for (int i = 0; i < controls.Count; i++)
                {
                    controls[i].TabIndex = i;
                }
                
                // Add keyboard shortcuts for common actions
                form.KeyPreview = true;
                form.KeyDown += (sender, e) =>
                {
                    // Ctrl+S for Save
                    if (e.Control && e.KeyCode == Keys.S)
                    {
                        var saveButton = GetAllControls(form)
                            .OfType<Button>()
                            .FirstOrDefault(b => b.Text.ToLower().Contains("save"));
                        saveButton?.PerformClick();
                        e.Handled = true;
                    }
                    
                    // F5 for Refresh
                    if (e.KeyCode == Keys.F5)
                    {
                        var refreshButton = GetAllControls(form)
                            .OfType<Button>()
                            .FirstOrDefault(b => b.Text.ToLower().Contains("refresh"));
                        refreshButton?.PerformClick();
                        e.Handled = true;
                    }
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Accessibility enhancement error: {ex.Message}");
            }
        }
        
        private static System.Collections.Generic.IEnumerable<Control> GetAllControls(Control container)
        {
            var controls = new System.Collections.Generic.List<Control>();
            
            foreach (Control control in container.Controls)
            {
                controls.Add(control);
                if (control.HasChildren)
                {
                    controls.AddRange(GetAllControls(control));
                }
            }
            
            return controls;
        }
        
        #endregion

        #region Quick Application Methods
        
        /// <summary>
        /// Apply quick enhancements to existing forms with minimal code changes
        /// Call this in your existing form's Load event or constructor
        /// </summary>
        public static void QuickEnhance(this Form form)
        {
            form.ApplyModernStyling();
        }
        
        /// <summary>
        /// Apply toolbar enhancements to a specific panel
        /// </summary>
        public static void QuickEnhanceToolbar(this Panel panel)
        {
            panel.EnhanceToolbar();
        }
        
        /// <summary>
        /// Enhance all buttons in a container with modern styling
        /// </summary>
        public static void EnhanceAllButtons(this Control container)
        {
            foreach (Control control in container.Controls)
            {
                if (control is Button button)
                {
                    ApplySmartButtonStyling(button);
                }
                
                if (control.HasChildren)
                {
                    control.EnhanceAllButtons();
                }
            }
        }
        
        #endregion
    }

    /// <summary>
    /// Static helper class for quick form enhancements
    /// Use this for applying styling to existing forms with single method calls
    /// </summary>
    public static class QuickStyling
    {
        /// <summary>
        /// Apply modern theme to any existing form instantly
        /// </summary>
        public static void ApplyModernTheme(Form form)
        {
            form.QuickEnhance();
        }
        
        /// <summary>
        /// Enhance specific form areas for immediate visual improvement
        /// </summary>
        public static void EnhanceFormAreas(Form form)
        {
            try
            {
                // Find and enhance toolbars
                var toolbars = form.Controls.OfType<Panel>()
                    .Where(p => p.Dock == DockStyle.Top && p.Height < 100);
                
                foreach (var toolbar in toolbars)
                {
                    toolbar.QuickEnhanceToolbar();
                }
                
                // Enhance all buttons
                form.EnhanceAllButtons();
                
                // Apply form-level styling
                form.BackColor = UIStyleManager.SurfaceDark;
                form.Font = UIStyleManager.BodyFont;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Quick enhancement error: {ex.Message}");
            }
        }
    }
}
