# Quick Launch Tools Menu for Town of Wiley Budget Management
# Usage: ./launch-tools.ps1 or just type "launch tools"

Write-Host '🏛️ TOWN OF WILEY - QUICK TOOLS LAUNCHER' -ForegroundColor Cyan
Write-Host '=========================================' -ForegroundColor Cyan

$rootPath = 'c:\Users\steve.mckitrick\Desktop\Rate Study'
$toolsMenuPath = "$rootPath\WileyBudgetManagement\ToolMenuForm.cs"
$quickLauncherPath = "$rootPath\QuickToolsLauncher.cs"

Write-Host '🔍 Checking for tools menu...' -ForegroundColor Yellow

if (Test-Path $toolsMenuPath) {
    Write-Host "✅ Found tools menu at: $toolsMenuPath" -ForegroundColor Green

    try {
        Write-Host '🚀 Compiling and launching tools menu...' -ForegroundColor Cyan

        # Compile the QuickToolsLauncher
        $compilePath = "$rootPath\QuickToolsLauncher.exe"

        # Compile both the launcher and the tools form
        csc /target:winexe /r:System.Windows.Forms.dll /r:System.Drawing.dll /out:"$compilePath" "$quickLauncherPath" "$toolsMenuPath"

        if (Test-Path $compilePath) {
            Write-Host '✅ Compilation successful!' -ForegroundColor Green
            Write-Host '🏛️ Opening Town of Wiley Tools Menu...' -ForegroundColor Cyan

            # Launch the tools menu
            Start-Process -FilePath $compilePath -Wait

            # Clean up executable
            Remove-Item $compilePath -ErrorAction SilentlyContinue
        } else {
            Write-Host '❌ Compilation failed. Trying alternative method...' -ForegroundColor Red

            # Alternative: Try to launch directly via dotnet if available
            $projectPath = "$rootPath\WileyBudgetManagement\WileyBudgetManagement.csproj"
            if (Test-Path $projectPath) {
                Write-Host '🔄 Using dotnet to launch...' -ForegroundColor Yellow
                dotnet run --project $projectPath
            } else {
                Write-Host '❌ Could not find project file. Please check installation.' -ForegroundColor Red
            }
        }
    } catch {
        Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host '📋 Available tools at root level:' -ForegroundColor Yellow

        # Show available tools as fallback
        Get-ChildItem $rootPath -Filter '*.cs' | Where-Object { $_.Name -like '*Tool*' -or $_.Name -like '*Launch*' -or $_.Name -like '*Analyzer*' } | ForEach-Object {
            Write-Host "  🔧 $($_.Name)" -ForegroundColor White
        }

        Get-ChildItem $rootPath -Filter '*.ps1' | ForEach-Object {
            Write-Host "  📜 $($_.Name)" -ForegroundColor White
        }
    }
} else {
    Write-Host '❌ Tools menu not found at expected location.' -ForegroundColor Red
    Write-Host '📁 Searching for available tools...' -ForegroundColor Yellow

    # Search for tools
    $tools = @()
    $tools += Get-ChildItem $rootPath -Filter '*.cs' | Where-Object { $_.Name -like '*Tool*' -or $_.Name -like '*Launch*' -or $_.Name -like '*Analyzer*' }
    $tools += Get-ChildItem $rootPath -Filter '*.ps1'

    if ($tools.Count -gt 0) {
        Write-Host "📋 Found $($tools.Count) available tools:" -ForegroundColor Green
        $tools | ForEach-Object {
            Write-Host "  🔧 $($_.Name)" -ForegroundColor White
        }
    } else {
        Write-Host '❌ No tools found in workspace.' -ForegroundColor Red
    }
}

Write-Host ''
Write-Host 'Press any key to exit...' -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
