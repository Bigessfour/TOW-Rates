using System;
using System.Drawing;
using System.Windows.Forms;

namespace WileyBudgetManagement.Forms
{
    /// <summary>
    /// UI Enhancement Demonstration Form
    /// Shows side-by-side comparison of old vs new styling
    /// Demonstrates the comprehensive UI improvements implemented
    /// </summary>
    public partial class UIEnhancementDemo : Form
    {
        public UIEnhancementDemo()
        {
            InitializeComponent();
            SetupDemo();
        }

        private void SetupDemo()
        {
            this.Text = "UI Enhancement Demonstration - Town of Wiley";
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = UIStyleManager.SurfaceDark;

            // Create split container for side-by-side comparison
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 700
            };

            // Left panel - OLD STYLING
            var oldPanel = CreateOldStylePanel();
            splitContainer.Panel1.Controls.Add(oldPanel);

            // Right panel - NEW STYLING
            var newPanel = CreateNewStylePanel();
            splitContainer.Panel2.Controls.Add(newPanel);

            this.Controls.Add(splitContainer);
        }

        private Panel CreateOldStylePanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SystemColors.Control,
                Padding = new Padding(10)
            };

            var titleLabel = new Label
            {
                Text = "BEFORE: Old Styling",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(300, 30),
                ForeColor = Color.Black
            };

            // Old-style toolbar
            var oldToolbar = new Panel
            {
                Location = new Point(10, 50),
                Size = new Size(660, 50),
                BackColor = Color.LightGray
            };

            var oldSaveButton = new Button
            {
                Text = "Save & Validate",
                Location = new Point(10, 10),
                Size = new Size(120, 30),
                BackColor = Color.LightBlue
            };

            var oldDeleteButton = new Button
            {
                Text = "Delete Row",
                Location = new Point(140, 10),
                Size = new Size(90, 30),
                BackColor = Color.LightCoral
            };

            var oldComboBox = new ComboBox
            {
                Location = new Point(240, 10),
                Size = new Size(150, 25),
                Items = { "All", "Revenue", "Operating" }
            };
            oldComboBox.SelectedIndex = 0;

            var oldStatusLabel = new Label
            {
                Text = "Ready",
                Location = new Point(400, 15),
                Size = new Size(100, 20),
                ForeColor = Color.DarkGreen
            };

            oldToolbar.Controls.AddRange(new Control[] { oldSaveButton, oldDeleteButton, oldComboBox, oldStatusLabel });

            // Old-style data area
            var oldDataPanel = new Panel
            {
                Location = new Point(10, 110),
                Size = new Size(660, 400),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var oldDataLabel = new Label
            {
                Text = "Data Grid Area\n\nCharacteristics:\n• Basic system colors\n• Inconsistent button styling\n• Poor visual hierarchy\n• Limited accessibility\n• No hover effects\n• Outdated typography",
                Location = new Point(20, 20),
                Size = new Size(620, 360),
                Font = new Font("Arial", 10),
                ForeColor = Color.Black
            };

            oldDataPanel.Controls.Add(oldDataLabel);

            // Old-style summary cards
            var oldCard1 = new Panel
            {
                Location = new Point(10, 520),
                Size = new Size(150, 80),
                BackColor = Color.FromArgb(0, 122, 204)
            };

            var oldCard1Label = new Label
            {
                Text = "Revenue\n$890,000",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            oldCard1.Controls.Add(oldCard1Label);

            var oldCard2 = new Panel
            {
                Location = new Point(170, 520),
                Size = new Size(150, 80),
                BackColor = Color.FromArgb(0, 122, 204)
            };

            var oldCard2Label = new Label
            {
                Text = "Expenses\n$750,000",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            oldCard2.Controls.Add(oldCard2Label);

            panel.Controls.AddRange(new Control[] { titleLabel, oldToolbar, oldDataPanel, oldCard1, oldCard2 });

            return panel;
        }

        private Panel CreateNewStylePanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UIStyleManager.Surface,
                Padding = new Padding(10)
            };

            var titleLabel = new Label
            {
                Text = "AFTER: Enhanced Styling",
                Location = new Point(10, 10),
                Size = new Size(300, 35)
            };
            UIStyleManager.ApplyHeadingLabelStyle(titleLabel);
            titleLabel.Font = new Font(UIStyleManager.PrimaryFontFamily, 14, FontStyle.Bold);
            titleLabel.ForeColor = UIStyleManager.PrimaryBlue;

            // New-style toolbar
            var newToolbar = new Panel
            {
                Location = new Point(10, 55),
                Size = new Size(660, 60)
            };
            UIStyleManager.ApplyToolbarPanelStyle(newToolbar);

            var newSaveButton = new Button
            {
                Text = "💾 Save & Validate",
                Location = new Point(16, 12)
            };
            UIStyleManager.ApplySuccessButtonStyle(newSaveButton);

            var newDeleteButton = new Button
            {
                Text = "🗑️ Delete Row",
                Location = new Point(152, 12)
            };
            UIStyleManager.ApplyWarningButtonStyle(newDeleteButton);

            var newAddButton = new Button
            {
                Text = "➕ Add Row",
                Location = new Point(268, 12)
            };
            UIStyleManager.ApplyPrimaryButtonStyle(newAddButton);

            var newComboBox = new ComboBox
            {
                Location = new Point(384, 15),
                Size = new Size(150, 28),
                Items = { "All", "Revenue", "Operating" }
            };
            UIStyleManager.ApplyComboBoxStyle(newComboBox);
            newComboBox.SelectedIndex = 0;

            var newStatusLabel = new Label
            {
                Text = "💧 System Ready",
                Location = new Point(550, 18),
                Size = new Size(100, 20)
            };
            UIStyleManager.ApplyStatusLabelStyle(newStatusLabel, UIStyleManager.StatusType.Success);

            newToolbar.Controls.AddRange(new Control[] { newSaveButton, newDeleteButton, newAddButton, newComboBox, newStatusLabel });

            // New-style data area
            var newDataPanel = new Panel
            {
                Location = new Point(10, 125),
                Size = new Size(660, 400),
                BackColor = UIStyleManager.Surface
            };
            UIStyleManager.ApplyCardPanelStyle(newDataPanel);

            var newDataLabel = new Label
            {
                Text = "Enhanced Data Grid Area\n\nImprovements:\n✅ Modern Material Design colors\n✅ Consistent button styling with icons\n✅ Clear visual hierarchy\n✅ Enhanced accessibility (WCAG 2.1)\n✅ Smooth hover effects\n✅ Professional typography (Segoe UI)\n✅ Improved spacing and padding\n✅ Status indicators with color coding",
                Location = new Point(20, 20),
                Size = new Size(620, 360),
                Font = UIStyleManager.BodyFont,
                ForeColor = UIStyleManager.NeutralDark
            };

            newDataPanel.Controls.Add(newDataLabel);

            // New-style enhanced summary cards
            var newCard1 = CreateEnhancedCard("💰", "Revenue", "$890,000", new Point(10, 535));
            var newCard2 = CreateEnhancedCard("📊", "Expenses", "$750,000", new Point(190, 535));
            var newCard3 = CreateEnhancedCard("💹", "Net Income", "$140,000", new Point(370, 535));

            panel.Controls.AddRange(new Control[] { titleLabel, newToolbar, newDataPanel, newCard1, newCard2, newCard3 });

            return panel;
        }

        private Panel CreateEnhancedCard(string icon, string title, string value, Point location)
        {
            var card = new Panel
            {
                Location = location,
                Size = new Size(170, 100),
                BackColor = UIStyleManager.Surface,
                Padding = new Padding(12)
            };

            // Add modern card styling
            UIStyleManager.ApplyCardPanelStyle(card);

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.Transparent
            };

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font(UIStyleManager.PrimaryFontFamily, 12),
                Size = new Size(25, 25),
                Location = new Point(0, 2),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var titleLabel = new Label
            {
                Text = title,
                Location = new Point(30, 5),
                Size = new Size(card.Width - 42, 20),
                Font = new Font(UIStyleManager.PrimaryFontFamily, 9, FontStyle.Bold),
                ForeColor = UIStyleManager.NeutralMedium
            };

            headerPanel.Controls.Add(iconLabel);
            headerPanel.Controls.Add(titleLabel);
            card.Controls.Add(headerPanel);

            var valueLabel = new Label
            {
                Text = value,
                Dock = DockStyle.Fill,
                Font = new Font(UIStyleManager.PrimaryFontFamily, 14, FontStyle.Bold),
                ForeColor = UIStyleManager.PrimaryBlue,
                TextAlign = ContentAlignment.MiddleCenter
            };

            card.Controls.Add(valueLabel);

            return card;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Name = "UIEnhancementDemo";
            this.ResumeLayout(false);
        }
    }
}
