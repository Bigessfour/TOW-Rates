# 🏛️ Town of Wiley - Quick Tools Launcher

## Quick Start

When you type "launch tools", you now have multiple ways to open the dynamic tools menu:

### Method 1: Batch File (Recommended)
```bash
./launch-tools.bat
```
- Double-click `launch-tools.bat` in Windows Explorer
- Or run from command line: `launch-tools.bat`

### Method 2: PowerShell Menu (Direct)
```powershell
pwsh -ExecutionPolicy Bypass -File "quick-tools-menu.ps1"
```

### Method 3: Command Line Shortcut
Add this to your PATH or create a batch file in a folder that's in your PATH:
```batch
@echo off
cd "c:\Users\steve.mckitrick\Desktop\Rate Study"
pwsh -ExecutionPolicy Bypass -File "quick-tools-menu.ps1"
```

## Features

The dynamic tools menu will automatically scan and display:

### 📜 PowerShell Scripts
- `debug-all-enterprises.ps1` - Debug all enterprise modules
- `launch-tools.ps1` - Alternative launcher script
- Any other `.ps1` files in the workspace

### 🔧 C# Tools  
- `CodeAnalysisTool.cs` - Code analysis and review
- `OneClickAnalyzer.cs` - Quick code analyzer
- `QuickConsoleAnalyzer.cs` - Console-based analyzer
- `SimplifiedImprovementTool.cs` - Code improvement tool
- All other `*Tool*.cs`, `*Launch*.cs`, `*Analyzer*.cs` files

### 🏛️ Municipal Tools
- **Open Project Folder** - Open workspace in Windows Explorer
- **Check API Key Status** - Verify xAI API key configuration
- **Run Debug All Enterprises** - Execute enterprise debugging
- **View Available VS Code Tasks** - Show configured VS Code tasks

## VS Code Integration

The following VS Code tasks are available and can be executed from the tools menu:
- Build WileyBudgetManagement
- Debug All Enterprises  
- Validate All Enterprises
- AI Integration Setup
- Deploy Production Build

## Requirements

- Windows PowerShell 5.1+ or PowerShell Core 7+
- .NET Framework (for C# compilation)
- Optional: Visual Studio or .NET SDK for advanced C# tool compilation

## Usage

1. **Quick Launch**: Double-click `launch-tools.bat`
2. **Select Tool**: Click any tool button to execute
3. **PowerShell Scripts**: Execute directly with proper permissions
4. **C# Tools**: Automatically compiled and executed (if compiler available)
5. **Municipal Tools**: Built-in utilities for common tasks

## Troubleshooting

### PowerShell Execution Policy
If scripts fail to run:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### C# Compiler Not Found
- Install Visual Studio 2022 Community (free)
- Or install .NET 8 SDK
- Or use PowerShell scripts instead

### API Key Issues
Set your xAI API key:
```batch
setx XAI_API_KEY "your-xai-api-key-here"
```

## File Structure

```
Rate Study/
├── launch-tools.bat              # Main launcher (click this!)
├── quick-tools-menu.ps1          # PowerShell GUI menu
├── QuickToolsLauncher.cs          # C# launcher (if compiler available)
├── debug-all-enterprises.ps1     # Enterprise debugging script
├── CodeAnalysisTool.cs           # Code analysis tool
├── OneClickAnalyzer.cs           # Quick analyzer
└── [other tools...]             # Additional development tools
```

## Next Steps

1. **Bookmark**: Add `launch-tools.bat` to your taskbar or desktop
2. **Customize**: Add your own tools by creating `.ps1` or `.cs` files
3. **Integrate**: Use with VS Code tasks and build processes
4. **Extend**: Add new tool categories in `quick-tools-menu.ps1`

---

**Town of Wiley Budget Management Software**  
*Municipal Excellence Through Technology*
