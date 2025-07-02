# PowerShell-Native Debug Script for Wiley Budget Management

# --- Configuration ---
$config = @{
    ProjectFile        = '.\WileyBudgetManagement\WileyBudgetManagement.csproj'
    LogDirectory       = 'C:\temp\WileyDebug'
    BuildLogName       = 'build.log'
    AppLogName         = 'dashboard_debug.log'
    ErrorLogName       = 'errors.log'
    ExitTimeoutSeconds = 15
}

$config.BuildLogPath = Join-Path $config.LogDirectory $config.BuildLogName
$config.AppLogPath = Join-Path $config.LogDirectory $config.AppLogName
$config.ErrorLogPath = Join-Path $config.LogDirectory $config.ErrorLogName

# --- Helper Functions ---
function Write-Log {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Message,
        [Parameter(Mandatory = $true)]
        [string]$Path
    )
    $timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
    "$timestamp - $Message" | Out-File -FilePath $Path -Append
}

# --- Main Script ---

# 1. Initialize Environment
Write-Progress -Activity 'Debugging Wiley Budget Management' -Status 'Initializing...' -PercentComplete 0
Write-Host 'Step 1: Initializing debug environment...' -ForegroundColor Cyan
try {
    if (Test-Path $config.LogDirectory) {
        Remove-Item -Path $config.LogDirectory -Recurse -Force -ErrorAction Stop
    }
    New-Item -Path $config.LogDirectory -ItemType Directory -ErrorAction Stop | Out-Null
    Write-Host "Log directory cleared and recreated at $($config.LogDirectory)" -ForegroundColor Green
} catch {
    Write-Host "Error initializing environment: $_" -ForegroundColor Red
    exit 1
}

# 2. Build Project
Write-Progress -Activity 'Debugging Wiley Budget Management' -Status 'Building project...' -PercentComplete 25
Write-Host 'Step 2: Building project with Start-Process for better error handling...' -ForegroundColor Cyan
$buildResult = Start-Process -FilePath 'dotnet' -ArgumentList "build $($config.ProjectFile)" -NoNewWindow -Wait -PassThru
if ($buildResult.ExitCode -ne 0) {
    Write-Host "Build failed with exit code $($buildResult.ExitCode). Check logs for details." -ForegroundColor Red
    exit 1
}
Write-Host 'Build completed successfully.' -ForegroundColor Green

# 3. Run Application
Write-Progress -Activity 'Debugging Wiley Budget Management' -Status 'Running application...' -PercentComplete 50
Write-Host 'Step 3: Running the application...' -ForegroundColor Yellow
$runResult = Start-Process -FilePath 'dotnet' -ArgumentList "run --project $($config.ProjectFile)" -NoNewWindow -Wait -PassThru
if ($runResult.ExitCode -ne 0) {
    Write-Host "Application exited with code $($runResult.ExitCode)." -ForegroundColor Yellow
}

# 4. Analyze Logs in Parallel
Write-Progress -Activity 'Debugging Wiley Budget Management' -Status 'Analyzing logs...' -PercentComplete 75
Write-Host 'Step 4: Inspecting logs for errors and warnings...' -ForegroundColor Yellow

$logFiles = @($config.AppLogPath, $config.ErrorLogPath)
$logResults = foreach ($log in $logFiles) {
    if (Test-Path $log) {
        Get-Content $log | Select-String 'error|fail|exception|critical' | ForEach-Object {
            [pscustomobject]@{
                LogFile    = (Get-Item $log).Name
                LineNumber = $_.LineNumber
                Message    = "'" + $_.Line + "'" # Quote the message to prevent execution
            }
        }
    }
}

if ($logResults) {
    Write-Host 'Found potential issues in logs:' -ForegroundColor Red
    $logResults | Format-Table -AutoSize
} else {
    Write-Host 'No critical issues found in logs.' -ForegroundColor Green
    # NEW: Check for unlogged error scenario
    if (Test-Path $config.AppLogPath) {
        Write-Host '-----------------------------------------------------------------' -ForegroundColor Yellow
        Write-Host 'ATTENTION: The application ran, but no errors were logged.' -ForegroundColor Yellow
        Write-Host 'If you saw an error message box, it means an exception was caught but not logged.' -ForegroundColor Yellow
        Write-Host "This prevents automated analysis. To fix this, a logging call must be added to the correct 'catch' block." -ForegroundColor Yellow
        Write-Host 'The most likely unlogged exception is in a UI event handler.' -ForegroundColor Yellow
        Write-Host '-----------------------------------------------------------------' -ForegroundColor Yellow
    }
}

# 5. Completion
Write-Progress -Activity 'Debugging Wiley Budget Management' -Status 'Completed.' -PercentComplete 100
Write-Host 'Debug process completed.' -ForegroundColor Green

# 6. Verify File Save
Write-Host 'Step 6: Verifying recent changes to TrashInput.cs...' -ForegroundColor Cyan
$fileToVerify = '.\Forms\TrashInput.cs'
$stringToFind = 'mainSplitContainer.Panel1MinSize = 200;'
try {
    $content = Get-Content $fileToVerify -Raw -ErrorAction Stop
    if ($content -match $stringToFind) {
        Write-Host "Verification successful: Changes have been saved to $($fileToVerify)." -ForegroundColor Green
    } else {
        Write-Host "Verification FAILED: Could not find expected changes in $($fileToVerify)." -ForegroundColor Red
    }
} catch {
    Write-Host "Error reading file $($fileToVerify): $_" -ForegroundColor Red
}

Write-Host "Script will exit in $($config.ExitTimeoutSeconds) seconds. Press any key to exit immediately..." -ForegroundColor Green
$timeout = $config.ExitTimeoutSeconds
while ($timeout -gt 0 -and -not $Host.UI.RawUI.KeyAvailable) {
    Start-Sleep -Seconds 1
    $timeout--
}
