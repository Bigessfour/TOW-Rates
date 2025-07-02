using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using WileyBudgetManagement.Tools;
using static WileyBudgetManagement.Tools.CodeImprovementManager;

namespace WileyBudgetManagement
{
    /// <summary>
    /// Simplified Code Improvement Tool for Town of Wiley Budget Management
    /// Works without Syncfusion - uses standard Windows Forms controls
    /// </summary>
    public partial class SimplifiedImprovementForm : Form
    {
        private readonly ImprovementPlan _plan;
        private readonly CodeImprovementManager _manager;
        private ListView _improvementsListView;
        private TextBox _detailsTextBox;
        private TextBox _currentCodeTextBox;
        private TextBox _improvedCodeTextBox;
        private Label _summaryLabel;
        private Button _applyButton;
        private Button _selectAllButton;

        public SimplifiedImprovementForm(ImprovementPlan plan, CodeImprovementManager manager)
        {
            _plan = plan ?? throw new ArgumentNullException(nameof(plan));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));

            InitializeComponent();
            LoadImprovements();
            UpdateSummary();
        }

        private void InitializeComponent()
        {
            Text = "Town of Wiley - Code Improvement Review (Simplified)";
            Size = new System.Drawing.Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;

            var mainPanel = new Panel { Dock = DockStyle.Fill };

            // Summary label at top
            _summaryLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 40,
                Text = "Loading improvements...",
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.LightBlue,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            // Create split container for main content
            var mainSplitter = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 600,
                Orientation = Orientation.Vertical
            };

            // Left panel: Improvements list
            var leftPanel = new Panel { Dock = DockStyle.Fill };
            _improvementsListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                CheckBoxes = true,
                MultiSelect = false
            };

            _improvementsListView.Columns.Add("Priority", 80);
            _improvementsListView.Columns.Add("Category", 120);
            _improvementsListView.Columns.Add("Title", 300);
            _improvementsListView.Columns.Add("Hours", 60);
            _improvementsListView.Columns.Add("ID", 80);

            _improvementsListView.ItemSelectionChanged += OnListViewSelectionChanged;
            _improvementsListView.ItemChecked += OnItemChecked;

            leftPanel.Controls.Add(_improvementsListView);
            mainSplitter.Panel1.Controls.Add(leftPanel);

            // Right panel: Details
            var rightSplitter = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 150
            };

            // Details section
            var detailsGroup = new GroupBox { Text = "Details", Dock = DockStyle.Fill };
            _detailsTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
                Font = new System.Drawing.Font("Consolas", 9F)
            };
            detailsGroup.Controls.Add(_detailsTextBox);
            rightSplitter.Panel1.Controls.Add(detailsGroup);

            // Code comparison section
            var codeSplitter = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal
            };

            var currentGroup = new GroupBox { Text = "Current Code", Dock = DockStyle.Fill };
            _currentCodeTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Both,
                Font = new System.Drawing.Font("Consolas", 8F)
            };
            currentGroup.Controls.Add(_currentCodeTextBox);
            codeSplitter.Panel1.Controls.Add(currentGroup);

            var improvedGroup = new GroupBox { Text = "Improved Code", Dock = DockStyle.Fill };
            _improvedCodeTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Both,
                Font = new System.Drawing.Font("Consolas", 8F),
                BackColor = System.Drawing.Color.FromArgb(240, 255, 240)
            };
            improvedGroup.Controls.Add(_improvedCodeTextBox);
            codeSplitter.Panel2.Controls.Add(improvedGroup);

            rightSplitter.Panel2.Controls.Add(codeSplitter);
            mainSplitter.Panel2.Controls.Add(rightSplitter);

            // Button panel at bottom
            var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 50 };

            _selectAllButton = new Button
            {
                Text = "Select All High/Critical",
                Size = new System.Drawing.Size(150, 30),
                Location = new System.Drawing.Point(10, 10)
            };
            _selectAllButton.Click += OnSelectAllHighPriority;

            _applyButton = new Button
            {
                Text = "Apply Selected Improvements",
                Size = new System.Drawing.Size(200, 30),
                Location = new System.Drawing.Point(170, 10),
                BackColor = System.Drawing.Color.LightGreen
            };
            _applyButton.Click += OnApplyImprovements;

            var cancelButton = new Button
            {
                Text = "Cancel",
                Size = new System.Drawing.Size(100, 30),
                Location = new System.Drawing.Point(380, 10),
                DialogResult = DialogResult.Cancel
            };

            buttonPanel.Controls.AddRange(new Control[] { _selectAllButton, _applyButton, cancelButton });

            // Add all to main panel
            mainPanel.Controls.Add(mainSplitter);
            mainPanel.Controls.Add(buttonPanel);
            mainPanel.Controls.Add(_summaryLabel);

            Controls.Add(mainPanel);
        }

        private void LoadImprovements()
        {
            _improvementsListView.Items.Clear();

            foreach (var improvement in _plan.Improvements)
            {
                var item = new ListViewItem(improvement.Priority.ToString());
                item.SubItems.Add(improvement.Category.ToString());
                item.SubItems.Add(improvement.Title);
                item.SubItems.Add(improvement.EstimatedHours.ToString("F1"));
                item.SubItems.Add(improvement.Id);
                item.Tag = improvement;
                item.Checked = improvement.IsSelected;

                // Color coding based on priority
                switch (improvement.Priority)
                {
                    case ImprovementPriority.Critical:
                        item.BackColor = System.Drawing.Color.FromArgb(255, 230, 230);
                        break;
                    case ImprovementPriority.High:
                        item.BackColor = System.Drawing.Color.FromArgb(255, 245, 230);
                        break;
                    case ImprovementPriority.Medium:
                        item.BackColor = System.Drawing.Color.FromArgb(255, 255, 230);
                        break;
                    case ImprovementPriority.Low:
                        item.BackColor = System.Drawing.Color.FromArgb(240, 255, 240);
                        break;
                }

                _improvementsListView.Items.Add(item);
            }
        }

        private void OnListViewSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected && e.Item.Tag is CodeImprovement improvement)
            {
                DisplayImprovementDetails(improvement);
            }
        }

        private void OnItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Tag is CodeImprovement improvement)
            {
                improvement.IsSelected = e.Item.Checked;
                UpdateSummary();
            }
        }

        private void DisplayImprovementDetails(CodeImprovement improvement)
        {
            _detailsTextBox.Text = $"ID: {improvement.Id}\r\n" +
                                  $"Title: {improvement.Title}\r\n" +
                                  $"Priority: {improvement.Priority}\r\n" +
                                  $"Category: {improvement.Category}\r\n" +
                                  $"Estimated Hours: {improvement.EstimatedHours:F1}\r\n\r\n" +
                                  $"Description:\r\n{improvement.Description}\r\n\r\n" +
                                  $"Rationale:\r\n{improvement.Rationale}";

            _currentCodeTextBox.Text = improvement.CurrentCode;
            _improvedCodeTextBox.Text = improvement.ImprovedCode;
        }

        private void UpdateSummary()
        {
            var selected = _plan.Improvements.Count(i => i.IsSelected);
            var totalHours = _plan.Improvements.Where(i => i.IsSelected).Sum(i => i.EstimatedHours);

            _summaryLabel.Text = $"Town of Wiley Code Improvements: {selected}/{_plan.Improvements.Count} selected - " +
                               $"Estimated Time: {totalHours:F1} hours";
        }

        private void OnSelectAllHighPriority(object sender, EventArgs e)
        {
            foreach (ListViewItem item in _improvementsListView.Items)
            {
                if (item.Tag is CodeImprovement improvement)
                {
                    var shouldSelect = improvement.Priority <= ImprovementPriority.High;
                    item.Checked = shouldSelect;
                    improvement.IsSelected = shouldSelect;
                }
            }
            UpdateSummary();
        }

        private void OnApplyImprovements(object sender, EventArgs e)
        {
            var selectedCount = _plan.Improvements.Count(i => i.IsSelected);
            if (selectedCount == 0)
            {
                MessageBox.Show("Please select at least one improvement to apply.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Apply {selectedCount} selected improvements?\n\n" +
                "This will create a backup and generate an implementation plan.",
                "Confirm Implementation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _manager.ApplySelectedImprovements(_plan);
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }

    /// <summary>
    /// Main program entry point for the simplified improvement tool
    /// </summary>
    class SimplifiedToolProgram
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Check for command-line task commands
            if (args.Length > 0)
            {
                var command = string.Join(" ", args).ToLower();
                if (command.Contains("run task examine") || command.Contains("examine"))
                {
                    RunQuickExamine();
                    return;
                }
            }

            var targetFile = @"c:\Users\steve.mckitrick\Desktop\Rate Study\WileyBudgetManagement\AIEnhancedQueryService.cs";

            if (args.Length > 0 && File.Exists(args[0]))
            {
                targetFile = args[0];
            }

            // Check if target file exists before proceeding
            if (!File.Exists(targetFile))
            {
                MessageBox.Show($"Target file not found:\n{targetFile}\n\nPlease ensure the AIEnhancedQueryService.cs file exists.",
                    "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Try to let user select a file
                using var openDialog = new OpenFileDialog
                {
                    Title = "Select C# File to Analyze",
                    Filter = "C# Files (*.cs)|*.cs|All Files (*.*)|*.*",
                    InitialDirectory = Path.GetDirectoryName(targetFile) ?? Environment.CurrentDirectory
                };

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    targetFile = openDialog.FileName;
                }
                else
                {
                    return; // User cancelled
                }
            }

            try
            {
                Console.WriteLine("🏛️  TOWN OF WILEY - SIMPLIFIED CODE IMPROVEMENT TOOL");
                Console.WriteLine($"📁 Analyzing: {Path.GetFileName(targetFile)}");

                var manager = new CodeImprovementManager(targetFile);
                var plan = manager.AnalyzeAndGenerateImprovements();

                Console.WriteLine($"✅ Found {plan.Improvements.Count} improvements");
                Console.WriteLine("🖥️  Opening review interface...");

                using var form = new SimplifiedImprovementForm(plan, manager);
                Application.Run(form);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"File not found: {ex.Message}\n\nPlease check the file path and try again.",
                    "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Analysis Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void RunQuickExamine()
        {
            Console.WriteLine("🤖 EXECUTING: RUN TASK EXAMINE");
            Console.WriteLine("=" + new string('=', 40));

            try
            {
                // Try to run the one-click analyzer
                var examinerPath = "OneClickAnalyzer.cs";
                if (File.Exists(examinerPath))
                {
                    System.Diagnostics.Process.Start("RunTaskExamine.bat");
                }
                else
                {
                    Console.WriteLine("❌ OneClickAnalyzer.cs not found");
                    Console.WriteLine("💡 Please ensure OneClickAnalyzer.cs is in the current directory");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Command execution failed: {ex.Message}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
