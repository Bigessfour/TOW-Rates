using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using Syncfusion.WinForms.DataGrid.Enums;
using static WileyBudgetManagement.Tools.CodeImprovementManager;

namespace WileyBudgetManagement.Tools
{
    /// <summary>
    /// Interactive form for reviewing and selecting code improvements
    /// Uses Syncfusion controls for enhanced municipal software UI
    /// </summary>
    public partial class ImprovementReviewForm : Form
    {
        private readonly ImprovementPlan _plan;
        private readonly CodeImprovementManager _manager;
        private SfDataGrid _improvementsGrid;
        private TextBox _detailsTextBox;
        private TextBox _currentCodeTextBox;
        private TextBox _improvedCodeTextBox;
        private Label _summaryLabel;
        private Button _applyButton;
        private Button _cancelButton;
        private Button _selectAllButton;
        private Button _selectNoneButton;

        public ImprovementReviewForm(ImprovementPlan plan, CodeImprovementManager manager)
        {
            _plan = plan ?? throw new ArgumentNullException(nameof(plan));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));

            InitializeComponent();
            LoadImprovements();
            UpdateSummary();
        }

        private void InitializeComponent()
        {
            Text = "Town of Wiley - Code Improvement Review";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 600);

            // Create main layout
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3
            };

            // Set column styles (70% for grid, 30% for details)
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));

            // Set row styles
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Header
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Main content
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Buttons

            // Header section
            _summaryLabel = new Label
            {
                Text = "Loading improvements...",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.LightBlue,
                Padding = new Padding(10, 0, 0, 0)
            };
            mainLayout.Controls.Add(_summaryLabel, 0, 0);
            mainLayout.SetColumnSpan(_summaryLabel, 2);

            // Create improvements grid
            _improvementsGrid = new SfDataGrid
            {
                Dock = DockStyle.Fill,
                AllowEditing = true,
                AllowSorting = true,
                AllowFiltering = true,
                ShowGroupDropArea = true,
                AutoGenerateColumns = false
            };

            ConfigureGridColumns();
            mainLayout.Controls.Add(_improvementsGrid, 0, 1);

            // Create details panel
            var detailsPanel = CreateDetailsPanel();
            mainLayout.Controls.Add(detailsPanel, 1, 1);

            // Create button panel
            var buttonPanel = CreateButtonPanel();
            mainLayout.Controls.Add(buttonPanel, 0, 2);
            mainLayout.SetColumnSpan(buttonPanel, 2);

            Controls.Add(mainLayout);

            // Wire events
            _improvementsGrid.CurrentCellValueChanged += OnGridCellValueChanged;
            _improvementsGrid.SelectionChanged += OnGridSelectionChanged;
        }

        private void ConfigureGridColumns()
        {
            _improvementsGrid.Columns.Add(new GridCheckBoxColumn
            {
                MappingName = "IsSelected",
                HeaderText = "Select",
                Width = 60,
                AllowEditing = true
            });

            _improvementsGrid.Columns.Add(new GridTextColumn
            {
                MappingName = "Priority",
                HeaderText = "Priority",
                Width = 80
            });

            _improvementsGrid.Columns.Add(new GridTextColumn
            {
                MappingName = "Category",
                HeaderText = "Category",
                Width = 120
            });

            _improvementsGrid.Columns.Add(new GridTextColumn
            {
                MappingName = "Title",
                HeaderText = "Improvement",
                Width = 250
            });

            _improvementsGrid.Columns.Add(new GridTextColumn
            {
                MappingName = "EstimatedHours",
                HeaderText = "Hours",
                Width = 60,
                Format = "F1"
            });

            _improvementsGrid.Columns.Add(new GridTextColumn
            {
                MappingName = "Id",
                HeaderText = "ID",
                Width = 80
            });

            // Set priority column colors
            _improvementsGrid.QueryCellStyle += (sender, e) =>
            {
                if (e.Column.MappingName == "Priority" && e.RowIndex > 0)
                {
                    var improvement = _improvementsGrid.View.Records[e.RowIndex - 1].Data as CodeImprovement;
                    if (improvement != null)
                    {
                        e.Style.BackColor = improvement.Priority switch
                        {
                            ImprovementPriority.Critical => Color.FromArgb(255, 230, 230),
                            ImprovementPriority.High => Color.FromArgb(255, 245, 230),
                            ImprovementPriority.Medium => Color.FromArgb(255, 255, 230),
                            ImprovementPriority.Low => Color.FromArgb(240, 255, 240),
                            _ => Color.White
                        };
                    }
                }
            };
        }

        private Panel CreateDetailsPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill };
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));

            // Details section
            var detailsGroup = new GroupBox { Text = "Details", Dock = DockStyle.Fill };
            _detailsTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9F)
            };
            detailsGroup.Controls.Add(_detailsTextBox);
            layout.Controls.Add(detailsGroup, 0, 0);

            // Current code section
            var currentGroup = new GroupBox { Text = "Current Code", Dock = DockStyle.Fill };
            _currentCodeTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Consolas", 8F)
            };
            currentGroup.Controls.Add(_currentCodeTextBox);
            layout.Controls.Add(currentGroup, 0, 1);

            // Improved code section
            var improvedGroup = new GroupBox { Text = "Improved Code", Dock = DockStyle.Fill };
            _improvedCodeTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Consolas", 8F),
                BackColor = Color.FromArgb(240, 255, 240)
            };
            improvedGroup.Controls.Add(_improvedCodeTextBox);
            layout.Controls.Add(improvedGroup, 0, 2);

            panel.Controls.Add(layout);
            return panel;
        }

        private Panel CreateButtonPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill };

            _selectAllButton = new Button
            {
                Text = "Select All High/Critical",
                Size = new Size(150, 30),
                Location = new Point(10, 10)
            };
            _selectAllButton.Click += OnSelectAllHighPriority;

            _selectNoneButton = new Button
            {
                Text = "Clear All",
                Size = new Size(100, 30),
                Location = new Point(170, 10)
            };
            _selectNoneButton.Click += OnSelectNone;

            _applyButton = new Button
            {
                Text = "Apply Selected Improvements",
                Size = new Size(200, 30),
                Location = new Point(panel.Width - 320, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.LightGreen
            };
            _applyButton.Click += OnApplyImprovements;

            _cancelButton = new Button
            {
                Text = "Cancel",
                Size = new Size(100, 30),
                Location = new Point(panel.Width - 110, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                DialogResult = DialogResult.Cancel
            };

            panel.Controls.AddRange(new Control[] { _selectAllButton, _selectNoneButton, _applyButton, _cancelButton });
            return panel;
        }

        private void LoadImprovements()
        {
            _improvementsGrid.DataSource = _plan.Improvements;
        }

        private void OnGridCellValueChanged(object sender, EventArgs e)
        {
            UpdateSummary();
        }

        private void OnGridSelectionChanged(object sender, EventArgs e)
        {
            if (_improvementsGrid.SelectedItems.Any())
            {
                var selectedImprovement = _improvementsGrid.SelectedItems[0] as CodeImprovement;
                if (selectedImprovement != null)
                {
                    DisplayImprovementDetails(selectedImprovement);
                }
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
                                  $"Rationale:\r\n{improvement.Rationale}\r\n\r\n" +
                                  $"Testing Notes:\r\n{improvement.TestingNotes}";

            _currentCodeTextBox.Text = improvement.CurrentCode;
            _improvedCodeTextBox.Text = improvement.ImprovedCode;
        }

        private void UpdateSummary()
        {
            var selected = _plan.Improvements.Count(i => i.IsSelected);
            var totalHours = _plan.Improvements.Where(i => i.IsSelected).Sum(i => i.EstimatedHours);
            var criticalSelected = _plan.Improvements.Count(i => i.IsSelected && i.Priority == ImprovementPriority.Critical);
            var highSelected = _plan.Improvements.Count(i => i.IsSelected && i.Priority == ImprovementPriority.High);

            _summaryLabel.Text = $"Town of Wiley Code Improvements: {selected}/{_plan.Improvements.Count} selected " +
                               $"({criticalSelected} Critical, {highSelected} High Priority) - " +
                               $"Estimated Time: {totalHours:F1} hours";
        }

        private void OnSelectAllHighPriority(object sender, EventArgs e)
        {
            foreach (var improvement in _plan.Improvements)
            {
                improvement.IsSelected = improvement.Priority <= ImprovementPriority.High;
            }
            _improvementsGrid.View.Refresh();
            UpdateSummary();
        }

        private void OnSelectNone(object sender, EventArgs e)
        {
            foreach (var improvement in _plan.Improvements)
            {
                improvement.IsSelected = false;
            }
            _improvementsGrid.View.Refresh();
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
                "This will create a backup of the current file and generate an implementation plan.",
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
}
