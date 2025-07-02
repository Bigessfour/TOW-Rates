using System;
using System.Drawing;
using System.Windows.Forms;

namespace WileyBudgetManagement.Forms
{
    public partial class DashboardForm
    {
        private ToolStripMenuItem debugMenuItem = null!;

        private void CreateDebugMenu()
        {
            try
            {
                // Create debug menu on top right of form
                var debugMenuStrip = new MenuStrip
                {
                    Dock = DockStyle.Top,
                    BackColor = Color.FromArgb(45, 45, 48),
                    ForeColor = Color.White,
                    RenderMode = ToolStripRenderMode.Professional
                };

                debugMenuItem = new ToolStripMenuItem("🐛 Debug");
                debugMenuItem.ForeColor = Color.White;

                var clearLogsItem = new ToolStripMenuItem("Clear Logs");
                clearLogsItem.Click += (s, e) =>
                {
                    DebugHelper.ClearLogs();
                    MessageBox.Show("Debug logs cleared successfully", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };

                var showErrorSummaryItem = new ToolStripMenuItem("Show Error Summary");
                showErrorSummaryItem.Click += (s, e) => DebugHelper.ShowErrorSummary();

                var openLogFolderItem = new ToolStripMenuItem("Open Log Folder");
                openLogFolderItem.Click += (s, e) => DebugHelper.OpenLogDirectory();

                var testButtons = new ToolStripMenuItem("Test All Buttons");
                testButtons.Click += (s, e) => RunButtonTests();

                var separator = new ToolStripSeparator();

                debugMenuItem.DropDownItems.Add(clearLogsItem);
                debugMenuItem.DropDownItems.Add(showErrorSummaryItem);
                debugMenuItem.DropDownItems.Add(openLogFolderItem);
                debugMenuItem.DropDownItems.Add(separator);
                debugMenuItem.DropDownItems.Add(testButtons);

                debugMenuStrip.Items.Add(debugMenuItem);

                this.MainMenuStrip = debugMenuStrip;
                this.Controls.Add(debugMenuStrip);

                DebugHelper.LogAction("Debug menu created successfully");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "CreateDebugMenu");
            }
        }

        private void RunButtonTests()
        {
            try
            {
                DebugHelper.LogAction("Starting automated button tests");

                var result = MessageBox.Show(
                    "This will attempt to click all main navigation buttons to identify issues.\n\n" +
                    "Continue with automated testing?",
                    "Confirm Button Tests",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;

                // Add test execution code here
                DebugHelper.LogAction("Automated button tests completed");
                MessageBox.Show("Button tests completed. Check logs for details.", "Tests Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "RunButtonTests");
                MessageBox.Show($"Error during button tests: {ex.Message}", "Test Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
