<#
.SYNOPSIS
    Null Reference Scanner for Wiley Budget Management C# Files
.DESCRIPTION
    Scans C# files for potential null reference issues and reports them with suggestions for fixes.
    Designed for the Town of Wiley Budget Management Software project.
.PARAMETER FilePath
    Path to the C# file to scan. Can be relative or absolute.
.PARAMETER ProjectRoot
    Root directory of the project. Defaults to parent directory of script.
.PARAMETER OutputFormat
    Output format: Console, Json, or Csv. Default is Console.
.PARAMETER Severity
    Filter by severity: All, High, Medium, Low. Default is All.
.EXAMPLE
    .\NullScanTask.ps1 -FilePath "AIEnhancedQueryService.cs"
    .\NullScanTask.ps1 -FilePath "WileyBudgetManagement\*.cs" -Severity High
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$FilePath,

    [Parameter(Mandatory = $false)]
    [string]$ProjectRoot = (Split-Path $PSScriptRoot -Parent),

    [Parameter(Mandatory = $false)]
    [ValidateSet('Console', 'Json', 'Csv')]
    [string]$OutputFormat = 'Console',

    [Parameter(Mandatory = $false)]
    [ValidateSet('All', 'High', 'Medium', 'Low')]
    [string]$Severity = 'All'
)

# Null reference patterns to detect
$NullPatterns = @{
    'CS8618_NonNullableField'    = @{
        Pattern     = '(private|public|protected|internal)\s+(readonly\s+)?[^?]*\s+\w+\s*[=;]'
        Severity    = 'High'
        Description = 'Non-nullable field that may not be initialized'
        Suggestion  = "Make field nullable with '?' or initialize in constructor"
    }
    'CS8601_NullAssignment'      = @{
        Pattern     = '\w+\s*=\s*null\s*[;,]'
        Severity    = 'High'
        Description = 'Possible null assignment to non-nullable reference'
        Suggestion  = 'Use nullable type or check for null before assignment'
    }
    'CS8602_NullDereference'     = @{
        Pattern     = '\w+\.\w+(?!\?)'
        Severity    = 'Medium'
        Description = 'Potential null dereference'
        Suggestion  = "Use null-conditional operator '?.' or add null check"
    }
    'CS8625_NullLiteral'         = @{
        Pattern     = '=\s*null(?!\s*[!?])'
        Severity    = 'High'
        Description = 'Null literal assigned to non-nullable type'
        Suggestion  = 'Use nullable type or provide non-null value'
    }
    'CS8622_DelegateNullability' = @{
        Pattern     = 'EventHandler[^?]*\s+\w+\s*='
        Severity    = 'Medium'
        Description = 'Delegate nullability mismatch'
        Suggestion  = "Make parameter nullable: 'object? sender'"
    }
    'UnhandledNullReturn'        = @{
        Pattern     = 'return\s+\w+\s*[;.]'
        Severity    = 'Low'
        Description = 'Method returns value that could be null'
        Suggestion  = 'Add null check or make return type nullable'
    }
    'UncheckedCollectionAccess'  = @{
        Pattern     = '\[\s*\w+\s*\](?!\?)'
        Severity    = 'Medium'
        Description = 'Array/collection access without null check'
        Suggestion  = 'Check collection and index validity before access'
    }
    'StringOperationOnNullable'  = @{
        Pattern     = '\w+\??\.(Contains|StartsWith|EndsWith|Substring)'
        Severity    = 'Medium'
        Description = 'String operation on potentially null value'
        Suggestion  = 'Use null-conditional or check for null first'
    }
}

function Write-ColoredOutput {
    param(
        [string]$Text,
        [string]$Color = 'White'
    )

    $colorMap = @{
        'Red'     = [ConsoleColor]::Red
        'Yellow'  = [ConsoleColor]::Yellow
        'Green'   = [ConsoleColor]::Green
        'Cyan'    = [ConsoleColor]::Cyan
        'Magenta' = [ConsoleColor]::Magenta
        'White'   = [ConsoleColor]::White
        'Gray'    = [ConsoleColor]::Gray
    }

    Write-Host $Text -ForegroundColor $colorMap[$Color]
}

function Get-SeverityColor {
    param([string]$Severity)

    switch ($Severity) {
        'High' { return 'Red' }
        'Medium' { return 'Yellow' }
        'Low' { return 'Green' }
        default { return 'White' }
    }
}

function Scan-CSharpFile {
    param(
        [string]$File,
        [hashtable]$Patterns,
        [string]$SeverityFilter
    )

    if (-not (Test-Path $File)) {
        Write-ColoredOutput "File not found: $File" 'Red'
        return @()
    }

    $content = Get-Content $File -Raw
    $lines = Get-Content $File
    $issues = @()

    Write-ColoredOutput "`n🔍 Scanning: $File" 'Cyan'
    Write-ColoredOutput "📄 Lines: $($lines.Count)" 'Gray'

    foreach ($patternName in $Patterns.Keys) {
        $patternInfo = $Patterns[$patternName]

        if ($SeverityFilter -ne 'All' -and $patternInfo.Severity -ne $SeverityFilter) {
            continue
        }

        $matches = [regex]::Matches($content, $patternInfo.Pattern, [System.Text.RegularExpressions.RegexOptions]::Multiline)

        foreach ($match in $matches) {
            # Find line number
            $lineNumber = ($content.Substring(0, $match.Index) -split "`n").Count
            $lineContent = $lines[$lineNumber - 1].Trim()

            # Skip common false positives
            if ($lineContent -match '//.*null' -or
                $lineContent -match 'string\.Empty' -or
                $lineContent -match 'null!' -or
                $lineContent -match '\?\.' -or
                $lineContent -match 'nullable') {
                continue
            }

            $issue = [PSCustomObject]@{
                File        = Split-Path $File -Leaf
                Line        = $lineNumber
                Column      = $match.Index
                Pattern     = $patternName
                Severity    = $patternInfo.Severity
                Description = $patternInfo.Description
                Suggestion  = $patternInfo.Suggestion
                Code        = $lineContent
                Match       = $match.Value
            }

            $issues += $issue
        }
    }

    return $issues
}

function Format-ConsoleOutput {
    param([array]$Issues)

    if ($Issues.Count -eq 0) {
        Write-ColoredOutput '✅ No null reference issues found!' 'Green'
        return
    }

    $groupedIssues = $Issues | Group-Object File

    foreach ($fileGroup in $groupedIssues) {
        Write-ColoredOutput "`n📁 File: $($fileGroup.Name)" 'Cyan'
        Write-ColoredOutput "🚨 Issues found: $($fileGroup.Count)" 'Yellow'

        $sortedIssues = $fileGroup.Group | Sort-Object Line

        foreach ($issue in $sortedIssues) {
            $severityColor = Get-SeverityColor $issue.Severity

            Write-ColoredOutput "`n  Line $($issue.Line): [$($issue.Severity)] $($issue.Pattern)" $severityColor
            Write-ColoredOutput "  Code: $($issue.Code)" 'Gray'
            Write-ColoredOutput "  Issue: $($issue.Description)" 'White'
            Write-ColoredOutput "  Fix: $($issue.Suggestion)" 'Green'
        }
    }

    # Summary
    $totalIssues = $Issues.Count
    $highSeverity = ($Issues | Where-Object { $_.Severity -eq 'High' }).Count
    $mediumSeverity = ($Issues | Where-Object { $_.Severity -eq 'Medium' }).Count
    $lowSeverity = ($Issues | Where-Object { $_.Severity -eq 'Low' }).Count

    Write-ColoredOutput "`n📊 SUMMARY" 'Magenta'
    Write-ColoredOutput "Total Issues: $totalIssues" 'White'
    Write-ColoredOutput "High Severity: $highSeverity" 'Red'
    Write-ColoredOutput "Medium Severity: $mediumSeverity" 'Yellow'
    Write-ColoredOutput "Low Severity: $lowSeverity" 'Green'
}

function Format-JsonOutput {
    param([array]$Issues)

    $result = @{
        ScanDate    = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
        ProjectRoot = $ProjectRoot
        TotalIssues = $Issues.Count
        Issues      = $Issues
    }

    return $result | ConvertTo-Json -Depth 4
}

function Format-CsvOutput {
    param([array]$Issues)

    return $Issues | ConvertTo-Csv -NoTypeInformation
}

# Main execution
try {
    Write-ColoredOutput '🔍 Wiley Budget Management - Null Reference Scanner' 'Magenta'
    Write-ColoredOutput "📂 Project Root: $ProjectRoot" 'Gray'
    Write-ColoredOutput "🎯 Target: $FilePath" 'Gray'
    Write-ColoredOutput "⚠️  Severity Filter: $Severity" 'Gray'

    # Resolve file paths
    $targetFiles = @()

    if ($FilePath.Contains('*')) {
        $searchPath = Join-Path $ProjectRoot $FilePath
        $targetFiles = Get-ChildItem -Path $searchPath -Filter '*.cs' -Recurse | Select-Object -ExpandProperty FullName
    } else {
        $fullPath = if (Test-Path $FilePath) { $FilePath } else { Join-Path $ProjectRoot $FilePath }
        if (Test-Path $fullPath) {
            $targetFiles = @($fullPath)
        }
    }

    if ($targetFiles.Count -eq 0) {
        Write-ColoredOutput "❌ No C# files found matching: $FilePath" 'Red'
        exit 1
    }

    Write-ColoredOutput "📋 Files to scan: $($targetFiles.Count)" 'Gray'

    # Scan all files
    $allIssues = @()
    foreach ($file in $targetFiles) {
        $fileIssues = Scan-CSharpFile -File $file -Patterns $NullPatterns -SeverityFilter $Severity
        $allIssues += $fileIssues
    }

    # Output results
    switch ($OutputFormat) {
        'Console' { Format-ConsoleOutput $allIssues }
        'Json' { Format-JsonOutput $allIssues }
        'Csv' { Format-CsvOutput $allIssues }
    }

    # Set exit code based on high severity issues
    $highSeverityCount = ($allIssues | Where-Object { $_.Severity -eq 'High' }).Count
    if ($highSeverityCount -gt 0) {
        exit $highSeverityCount
    }
} catch {
    Write-ColoredOutput "❌ Error: $($_.Exception.Message)" 'Red'
    exit 1
}
