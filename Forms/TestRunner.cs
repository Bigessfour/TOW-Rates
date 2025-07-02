using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using WileyBudgetManagement.Forms;

namespace WileyBudgetManagement.Forms
{
    public class TestRunner : Form
    {
        private Button runTestButton = null!;
        private Button logAndContinueButton = null!;
        private TextBox resultsTextBox = null!;
        private ProgressBar progressBar = null!;
        private Label progressLabel = null!; // Added for percentage text
        private TrashInput trashInputForm = null!;
        private BackgroundWorker validationWorker = null!;

        public TestRunner()
        {
            InitializeComponent();
            // Instantiate the form we want to test in the background
            trashInputForm = new TrashInput();
            InitializeBackgroundWorker();
        }

        private void InitializeBackgroundWorker()
        {
            validationWorker = new BackgroundWorker();
            validationWorker.DoWork += ValidationWorker_DoWork;
            validationWorker.RunWorkerCompleted += ValidationWorker_RunWorkerCompleted;
        }

        private void InitializeComponent()
        {
            this.Text = "Micro-Test Runner";
            this.Size = new System.Drawing.Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            progressBar = new ProgressBar
            {
                Dock = DockStyle.Top,
                Style = ProgressBarStyle.Continuous, // Changed from Marquee
                Height = 20,
                Visible = false
            };

            progressLabel = new Label
            {
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Visible = false,
                Text = "0% Complete"
            };

            runTestButton = new Button
            {
                Text = "Run ValidateAllData Test",
                Dock = DockStyle.Top,
                Height = 40
            };
            runTestButton.Click += RunTestButton_Click;

            logAndContinueButton = new Button
            {
                Text = "Discover Complete, Continue?",
                Dock = DockStyle.Bottom,
                Height = 40,
                Visible = false
            };
            logAndContinueButton.Click += LogAndContinueButton_Click;

            resultsTextBox = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new System.Drawing.Font("Consolas", 10)
            };

            this.Controls.Add(resultsTextBox);
            this.Controls.Add(logAndContinueButton);
            this.Controls.Add(runTestButton);
            this.Controls.Add(progressLabel); // Add the label
            this.Controls.Add(progressBar);
        }

        private void RunTestButton_Click(object? sender, EventArgs e)
        {
            resultsTextBox.Text = "Starting test...\r\n";
            runTestButton.Enabled = false;
            progressBar.Value = 0;
            progressBar.Visible = true;
            progressLabel.Text = "0% Complete";
            progressLabel.Visible = true;
            logAndContinueButton.Visible = false;

            if (!validationWorker.IsBusy)
            {
                validationWorker.RunWorkerAsync();
            }
        }

        private void ValidationWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            // This runs on a background thread
            try
            {
                // The result is now the entire validation result object
                e.Result = trashInputForm.GetValidationResults();
            }
            catch (Exception ex)
            {
                e.Result = ex; // Pass the exception to the completed event
            }
        }

        private void ValidationWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            // This runs on the UI thread
            progressBar.Visible = true; // Keep it visible to show final progress
            runTestButton.Enabled = true;

            if (e.Error != null)
            {
                resultsTextBox.AppendText($"FAILURE! An unhandled exception occurred during the test.\r\n");
                resultsTextBox.AppendText($"---\r\n");
                resultsTextBox.AppendText($"Exception Type: {e.Error.GetType().Name}\r\n");
                resultsTextBox.AppendText($"Message: {e.Error.Message}\r\n");
                resultsTextBox.AppendText($"---\r\n");
                resultsTextBox.AppendText($"Stack Trace:\r\n{e.Error.StackTrace}");
                DebugHelper.LogError(e.Error, "TestRunner");
            }
            else if (e.Result is Exception ex)
            {
                resultsTextBox.AppendText($"FAILURE! An exception was caught by the validation logic.\r\n");
                resultsTextBox.AppendText($"---\r\n");
                resultsTextBox.AppendText($"Exception Type: {ex.GetType().Name}\r\n");
                resultsTextBox.AppendText($"Message: {ex.Message}\r\n");
                resultsTextBox.AppendText($"---\r\n");
                resultsTextBox.AppendText($"Stack Trace:\r\n{ex.StackTrace}");
                DebugHelper.LogError(ex, "TestRunner");
            }
            else if (e.Result is TrashValidationResult validationResult)
            {
                string formattedResults = trashInputForm.FormatValidationResults(validationResult.Errors, validationResult.Warnings);
                if (string.IsNullOrEmpty(formattedResults))
                {
                    resultsTextBox.AppendText("SUCCESS: All validations passed.\r\n");
                }
                else
                {
                    resultsTextBox.AppendText("COMPLETED: Validation ran, issues found:\r\n");
                    resultsTextBox.AppendText("---\r\n");
                    resultsTextBox.AppendText(formattedResults);
                    resultsTextBox.AppendText("\r\n---\r\n");
                }

                // Update progress bar and label
                if (validationResult.TotalTests > 0)
                {
                    int percentage = (int)Math.Round((double)validationResult.PassedTests / validationResult.TotalTests * 100);
                    progressBar.Value = percentage;
                    progressLabel.Text = $"Debugging Progress: {percentage}% ({validationResult.PassedTests} / {validationResult.TotalTests} rules passed)";
                    resultsTextBox.AppendText($"\r\nProgress: {percentage}% ({validationResult.PassedTests} passed out of {validationResult.TotalTests} total checks).");
                }
                else
                {
                    progressLabel.Text = "No validation rules were executed.";
                }

                logAndContinueButton.Visible = true;
            }
        }

        private void LogAndContinueButton_Click(object? sender, EventArgs e)
        {
            try
            {
                string logPath = "C:\\temp\\WileyDebug\\test_runner_results.log";
                File.WriteAllText(logPath, resultsTextBox.Text);
                DebugHelper.LogAction($"Test runner results saved to {logPath}");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(ex, "LogAndContinueButton_Click");
            }
            finally
            {
                this.Close();
            }
        }
    }
}
