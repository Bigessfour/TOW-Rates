
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using WileyBudgetManagement.Forms;

namespace WileyBudgetManagement.Forms
{
    public class WaterTestRunner : Form
    {
        private Button runTestButton = null!;
        private Button logAndContinueButton = null!;
        private TextBox resultsTextBox = null!;
        private ProgressBar progressBar = null!;
        private Label progressLabel = null!;
        private WaterInput waterInputForm = null!;
        private BackgroundWorker validationWorker = null!;

        public WaterTestRunner()
        {
            InitializeComponent();
            waterInputForm = new WaterInput();
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
            this.Text = "Water Enterprise Test Runner";
            this.Size = new System.Drawing.Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            progressBar = new ProgressBar
            {
                Dock = DockStyle.Top,
                Style = ProgressBarStyle.Continuous,
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
                Text = "Run Water Validation Test",
                Dock = DockStyle.Top,
                Height = 40
            };
            runTestButton.Click += RunTestButton_Click;

            logAndContinueButton = new Button
            {
                Text = "Log and Continue",
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
            this.Controls.Add(progressLabel);
            this.Controls.Add(progressBar);
        }

        private void RunTestButton_Click(object? sender, EventArgs e)
        {
            resultsTextBox.Text = "Starting water validation test...\r\n";
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
            try
            {
                // This will need to be implemented in WaterInput.cs
                // e.Result = waterInputForm.GetValidationResults(); 
                e.Result = new object(); // Placeholder
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        private void ValidationWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Visible = true;
            runTestButton.Enabled = true;

            if (e.Error != null)
            {
                resultsTextBox.AppendText($"FAILURE! An unhandled exception occurred during the test.\r\n");
                resultsTextBox.AppendText($"Message: {e.Error.Message}\r\n");
                resultsTextBox.AppendText($"Stack Trace:\r\n{e.Error.StackTrace}");
            }
            else if (e.Result is Exception ex)
            {
                resultsTextBox.AppendText($"FAILURE! An exception was caught by the validation logic.\r\n");
                resultsTextBox.AppendText($"Message: {ex.Message}\r\n");
                resultsTextBox.AppendText($"Stack Trace:\r\n{ex.StackTrace}");
            }
            else
            {
                resultsTextBox.AppendText("SUCCESS: Water validation test completed (mock).");
                progressBar.Value = 100;
                progressLabel.Text = "100% Complete";
            }

            logAndContinueButton.Visible = true;
        }

        private void LogAndContinueButton_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}
