using System;
using System.Windows.Forms;
using System.Drawing;

/// <summary>
/// Instant Tools Launcher for Town of Wiley
/// Compile with: csc /target:winexe InstantToolsLauncher.cs
/// </summary>
public class InstantToolsLauncher
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show immediate message
            Console.WriteLine("🏛️ Opening Town of Wiley Tools Menu...");

            // Create and show the simplified tools menu
            var form = CreateToolsMenu();
            Application.Run(form);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Launch error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static Form CreateToolsMenu()
    {
        var form = new Form
        {
            Text = "🏛️ Town of Wiley - Tools Menu",
            Size = new Size(800, 600),
            StartPosition = FormStartPosition.CenterScreen,
            BackColor = Color.FromArgb(245, 248, 252)
        };

        // Header
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = Color.FromArgb(41, 128, 185)
        };

        var title = new Label
        {
            Text = "🏛️ TOWN OF WILEY DEVELOPMENT TOOLS",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill
        };

        header.Controls.Add(title);
        form.Controls.Add(header);

        // Tools panel
        var toolsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoScroll = true,
            Padding = new Padding(20)
        };

        // Add tool categories
        AddToolCategory(toolsPanel, "🔍 CODE ANALYSIS & IMPROVEMENT", new[]
        {
            "Run Task Examine - Code quality analysis",
            "Code Improvement Manager - Review improvements",
            "Quick Console Analyzer - Fast analysis",
            "Simplified Improvement Tool - Windows Forms UI"
        });

        AddToolCategory(toolsPanel, "🏛️ MUNICIPAL COMPLIANCE", new[]
        {
            "Municipal Audit Logger Test - Test logging",
            "API Key Validator - Validate xAI keys",
            "Rate Study Validator - Methodology compliance",
            "Municipal Context Checker - Verify configuration"
        });

        AddToolCategory(toolsPanel, "🤖 AI SERVICE TOOLS", new[]
        {
            "Test xAI Connection - Service connectivity",
            "Token Usage Reporter - Cost analysis",
            "AI Query Tester - Interactive testing",
            "Model Configuration Manager - AI models"
        });

        AddToolCategory(toolsPanel, "🗄️ DATABASE & INTEGRATION", new[]
        {
            "SQL Server Express Setup - Database config",
            "Database Schema Generator - Municipal schema",
            "Syncfusion License Checker - Component check",
            "Connection String Builder - Database connections"
        });

        form.Controls.Add(toolsPanel);

        // Close button
        var closeButton = new Button
        {
            Text = "Close Tools Menu",
            Size = new Size(150, 35),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            BackColor = Color.FromArgb(52, 73, 94),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        closeButton.Click += (s, e) => form.Close();

        var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 60 };
        buttonPanel.Controls.Add(closeButton);
        form.Controls.Add(buttonPanel);

        return form;
    }

    private static void AddToolCategory(FlowLayoutPanel parent, string categoryName, string[] tools)
    {
        var categoryPanel = new GroupBox
        {
            Text = categoryName,
            Width = parent.Width - 60,
            Height = 50 + (tools.Length * 30),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Margin = new Padding(0, 10, 0, 10)
        };

        int y = 25;
        foreach (var tool in tools)
        {
            var button = new Button
            {
                Text = tool,
                Size = new Size(categoryPanel.Width - 20, 25),
                Location = new Point(10, y),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8F),
                TextAlign = ContentAlignment.MiddleLeft
            };

            button.Click += (s, e) =>
            {
                MessageBox.Show($"Executing: {tool}\n\nTool would run here!",
                    "Tool Execution", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            categoryPanel.Controls.Add(button);
            y += 30;
        }

        parent.Controls.Add(categoryPanel);
    }
}
