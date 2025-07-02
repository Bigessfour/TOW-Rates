@echo offecho 🔍 Discovering and testing tools...

REM Quick discovery test
set TOOL_COUNT=0
for %%f in ("%TOOLS_PATH%\*.ps1") do set /a TOOL_COUNT+=1
for %%f in ("%TOOLS_PATH%\*Tool*.cs" "%TOOLS_PATH%\*Launch*.cs" "%TOOLS_PATH%\*Analyzer*.cs") do if exist "%%f" set /a TOOL_COUNT+=1

echo ✅ Discovered %TOOL_COUNT% available tools

REM Test essential components
echo 🧪 Running quick validation tests...

if exist "%TOOLS_PATH%\WileyBudgetManagement\Program.cs" (
    echo ✅ Main program found
) else (
    echo ❌ Main program missing
)

if exist "%TOOLS_PATH%\Forms\MainForm.cs" (
    echo ✅ Main form found
) else (
    echo ❌ Main form missing
)

if exist "%TOOLS_PATH%\Database\DatabaseManager.cs" (
    echo ✅ Database manager found
) else (
    echo ❌ Database manager missing
)

echo 🚀 Launching Town of Wiley Tools Menu...echo.
echo ============================================
echo   🏛️ TOWN OF WILEY - QUICK TOOLS LAUNCHER
echo ============================================
echo.

set "TOOLS_PATH=c:\Users\steve.mckitrick\Desktop\Rate Study"
set "PS_MENU=%TOOLS_PATH%\quick-tools-menu.ps1"

echo � Launching Town of Wiley Tools Menu...

if exist "%PS_MENU%" (
    echo ✅ Found PowerShell tools menu
    pwsh -ExecutionPolicy Bypass -File "%PS_MENU%"
    if errorlevel 1 (
        echo ❌ PowerShell Core not available, trying Windows PowerShell...
        if errorlevel 1 (
            echo ❌ PowerShell execution failed, showing fallback menu...
            goto :fallback_menu
        )
    )
) else (
    goto :fallback_menu
)
goto :end

:fallback_menu
echo ❌ Tools menu not found
echo 📁 Searching for available tools...
echo.
echo 📋 Available Tools:
for %%f in ("%TOOLS_PATH%\*.ps1") do echo   📜 %%~nxf
for %%f in ("%TOOLS_PATH%\*Tool*.cs" "%TOOLS_PATH%\*Launch*.cs" "%TOOLS_PATH%\*Analyzer*.cs") do if exist "%%f" echo   🔧 %%~nxf
echo.
echo 🧪 Quick Test Results:
echo   Tools discovered: %TOOL_COUNT%
echo   Essential files: Checked above
echo.
echo Press any key to exit...
pause >nul

:end
