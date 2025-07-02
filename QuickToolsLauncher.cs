using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Quick launcher for Town of Wiley Tools Menu
/// Type "launch tools" to open the dynamic tools menu
/// </summary>
public class QuickToolsLauncher
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Parse command line arguments
        var command = string.Join(" ", args).ToLower();

        if (command.Contains("launch tools") ||
            command.Contains("tools") ||
            command.Contains("menu") ||
            args.Length == 0)
        {
            LaunchToolsMenu();
        }
        else
        {
            Console.WriteLine("🏛️ Town of Wiley Quick Tools Launcher");
            Console.WriteLine("Usage: QuickToolsLauncher.exe [launch tools]");
            Console.WriteLine("Or simply run without arguments to open tools menu");
        }
    }

    private static void LaunchToolsMenu()
    {
        try
        {
            Console.WriteLine("🚀 Launching Town of Wiley Tools Menu...");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create and show the simple tools menu
            Application.Run(new SimpleToolsForm());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error launching tools menu: {ex.Message}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}

/// <summary>
/// Simple Tools Form that works with basic C# compiler
/// </summary>
public partial class SimpleToolsForm : Form
{
    private FlowLayoutPanel toolsPanel;

    public SimpleToolsForm()
    {
        InitializeComponent();
        ScanAndLoadTools();
    }

    private void InitializeComponent()
    {
        Text = "🏛️ Town of Wiley - Development Tools";
        Size = new Size(600, 500);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Color.FromArgb(245, 248, 252);

        // Header
        var headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = Color.FromArgb(41, 128, 185)
        };

        var titleLabel = new Label
        {
            Text = "🏛️ TOWN OF WILEY DEVELOPMENT TOOLS",
            Font = new Font("Arial", 12F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };

        headerPanel.Controls.Add(titleLabel);
        Controls.Add(headerPanel);

        // Tools panel
        toolsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(20)
        };

        Controls.Add(toolsPanel);

        // Close button
        var buttonPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 50
        };

        var closeButton = new Button
        {
            Text = "Close",
            Size = new Size(100, 30),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            Location = new Point(480, 10)
        };
        closeButton.Click += (s, e) => Close();

        buttonPanel.Controls.Add(closeButton);
        Controls.Add(buttonPanel);
    }

    private void ScanAndLoadTools()
    {
        var rootPath = @"c:\Users\steve.mckitrick\Desktop\Rate Study";

        // Add category headers and tools
        AddCategoryHeader("📜 PowerShell Scripts");
        ScanPowerShellScripts(rootPath);

        AddCategoryHeader("🔧 C# Tools");
        ScanCSharpTools(rootPath);

        AddCategoryHeader("🏛️ Municipal Tools");
        AddBuiltInTools();
    }

    private void AddCategoryHeader(string categoryName)
    {
        var headerLabel = new Label
        {
            Text = categoryName,
            Font = new Font("Arial", 10F, FontStyle.Bold),
            Size = new Size(540, 25),
            BackColor = Color.FromArgb(236, 240, 245),
            ForeColor = Color.FromArgb(52, 73, 94),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(10, 5, 10, 5),
            Margin = new Padding(0, 10, 0, 5)
        };

        toolsPanel.Controls.Add(headerLabel);
    }

    private void ScanPowerShellScripts(string rootPath)
    {
        try
        {
            var scriptFiles = Directory.GetFiles(rootPath, "*.ps1", SearchOption.AllDirectories);

            foreach (var scriptFile in scriptFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(scriptFile);
                AddToolButton(fileName, $"PowerShell: {fileName}", () => ExecutePowerShellScript(scriptFile));
            }
        }
        catch (Exception ex)
        {
            AddToolButton("Error scanning scripts", ex.Message, () => { });
        }
    }

    private void ScanCSharpTools(string rootPath)
    {
        try
        {
            var toolPatterns = new[] { "*Tool*.cs", "*Launch*.cs", "*Analyzer*.cs" };
            var toolFiles = new List<string>();

            foreach (var pattern in toolPatterns)
            {
                toolFiles.AddRange(Directory.GetFiles(rootPath, pattern, SearchOption.TopDirectoryOnly));
            }

            foreach (var toolFile in toolFiles.Distinct())
            {
                var fileName = Path.GetFileNameWithoutExtension(toolFile);
                AddToolButton(fileName, $"C# Tool: {fileName}", () => ExecuteCSharpTool(toolFile));
            }
        }
        catch (Exception ex)
        {
            AddToolButton("Error scanning tools", ex.Message, () => { });
        }
    }

    private void AddBuiltInTools()
    {
        AddToolButton("View Project Folder", "Open project folder in explorer", () =>
            Process.Start("explorer.exe", @"c:\Users\steve.mckitrick\Desktop\Rate Study"));

        AddToolButton("API Key Status", "Check xAI API key status", CheckApiKeyStatus);

        AddToolButton("Build Project", "Build WileyBudgetManagement project", BuildProject);
    }

    private void AddToolButton(string name, string description, Action action)
    {
        var button = new Button
        {
            Text = name,
            Size = new Size(540, 35),
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 2, 0, 2),
            Tag = action
        };

        button.FlatAppearance.BorderSize = 0;
        button.Click += OnToolButtonClick;

        // Add tooltip
        var tooltip = new ToolTip();
        tooltip.SetToolTip(button, description);

        toolsPanel.Controls.Add(button);
    }

    private void OnToolButtonClick(object sender, EventArgs e)
    {
        var button = sender as Button;
        var action = button?.Tag as Action;

        if (action != null)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing tool: {ex.Message}", "Tool Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    private void ExecutePowerShellScript(string scriptPath)
    {
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"",
                UseShellExecute = true
            };

            Process.Start(processInfo);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to execute script: {ex.Message}", "Script Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ExecuteCSharpTool(string toolPath)
    {
        try
        {
            var workingDir = Path.GetDirectoryName(toolPath);
            var fileName = Path.GetFileNameWithoutExtension(toolPath);

            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c cd \"{workingDir}\" && csc /target:exe /out:\"{fileName}.exe\" \"{toolPath}\" && \"{fileName}.exe\" && del \"{fileName}.exe\"",
                UseShellExecute = true
            };

            Process.Start(processInfo);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to execute tool: {ex.Message}", "Tool Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CheckApiKeyStatus()
    {
        var apiKey = Environment.GetEnvironmentVariable("XAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            MessageBox.Show("XAI_API_KEY environment variable not found.", "API Key Status",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        else
        {
            var status = apiKey.StartsWith("xai-") && apiKey.Length >= 32 ? "Valid" : "Invalid";
            MessageBox.Show($"API Key Status: {status}\nLength: {apiKey.Length} characters", "API Key Status",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void BuildProject()
    {
        try
        {
            var projectPath = @"c:\Users\steve.mckitrick\Desktop\Rate Study\WileyBudgetManagement";
            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c cd \"{projectPath}\" && csc /target:winexe /r:System.Windows.Forms.dll /r:System.Drawing.dll *.cs",
                UseShellExecute = true
            };

            Process.Start(processInfo);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to build project: {ex.Message}", "Build Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
