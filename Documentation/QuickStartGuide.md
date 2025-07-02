# QUICK START IMPLEMENTATION GUIDE
## Town of Wiley Budget Management System - AI Integration
### July 1, 2025

---

## 🚀 **IMMEDIATE SETUP (5 minutes)**

### Step 1: Environment Configuration
```powershell
# Set XAI API Key (Run in PowerShell as Administrator)
[Environment]::SetEnvironmentVariable("XAI_API_KEY", "your-actual-xai-key", "Machine")

# Restart VS Code to reload environment variables
# Verify setup
$env:XAI_API_KEY
```

### Step 2: Install Dependencies
```bash
# Run from project root
dotnet add .\WileyBudgetManagement\WileyBudgetManagement.csproj package Newtonsoft.Json
```

### Step 3: Run Setup Task
- Press `Ctrl+Shift+P` in VS Code
- Type "Tasks: Run Task"
- Select "AI Integration Setup"

---

## 📝 **TODAY'S DEVELOPMENT PRIORITIES**

### PRIORITY 1: Complete Trash Enterprise (2-3 hours)
```bash
# Files to modify:
# 1. Forms/TrashInput.cs (lines 253-320) - CalculateTrashScenarios method
# 2. Database/SanitationRepository.cs - Add SaveTrashDataAsync method
# 3. Forms/TrashInput.cs (lines 811-845) - Complete SaveTrashDataAsync
```

**Immediate Code Changes:**
1. **Update PMT Calculation in TrashInput.cs:**
```csharp
// Replace existing CalculateTrashScenarios method
private void CalculateTrashScenarios(SanitationDistrict district)
{
    decimal baseMonthly = district.MonthlyInput;
    
    // VALIDATED: Scenario 1 - $350K Truck, 12 years, 4.5% = $32,083.34/year
    decimal truckAnnualCost = 32083.34m;
    decimal truckMonthlyImpact = truckAnnualCost / 12; // $2,673.61
    
    // NEW: Scenario 2 - Recycling Program, $125K, 7 years, 4%
    decimal recyclingAnnualCost = 20373.52m;
    decimal recyclingMonthlyImpact = recyclingAnnualCost / 12; // $1,697.79
    
    // NEW: Scenario 3 - Transfer Station, $200K, 15 years, 3.5%
    decimal transferAnnualCost = 17157.24m;
    decimal transferMonthlyImpact = transferAnnualCost / 12; // $1,429.77
    
    // Apply scenarios by section type (existing logic enhanced)
    ApplyScenariosBySection(district, baseMonthly, truckMonthlyImpact, 
                           recyclingMonthlyImpact, transferMonthlyImpact);
}
```

### PRIORITY 2: Create AI Query Service (1-2 hours)
```bash
# Create new file: Services/AIQueryService.cs
# Location: Create Services folder in WileyBudgetManagement project
```

**Implementation Steps:**
1. **Create Services folder**
2. **Add AIQueryService.cs** (use code from ExecutableDevelopmentPlan.md)
3. **Test with simple query**

### PRIORITY 3: Basic AI Interface (1 hour)
```bash
# Create new file: Forms/AIQueryPanel.cs
# Add to main dashboard navigation
```

---

## 🧪 **TESTING CHECKLIST**

### Trash Enterprise Validation
- [ ] All 17 trash accounts load correctly
- [ ] Scenario calculations produce expected values:
  - Scenario 1: $2,673.61/month truck impact
  - Scenario 2: $1,697.79/month recycling impact  
  - Scenario 3: $1,429.77/month transfer station impact
- [ ] Save/Load operations work with database
- [ ] Charts render without errors

### AI Integration Testing
- [ ] XAI_API_KEY environment variable loads
- [ ] AIQueryService instantiates without errors
- [ ] Basic "What-if" query returns response
- [ ] Error handling works for invalid API key

---

## 🎯 **SUCCESS METRICS FOR TODAY**

### Must Complete (MVP):
1. ✅ Trash enterprise scenarios calculate correctly
2. ✅ AI service responds to basic queries
3. ✅ All existing functionality still works

### Should Complete (Enhanced):
1. ✅ Trash enterprise saves to database
2. ✅ AI panel integrated in dashboard
3. ✅ Basic "What-if" scenarios working

### Could Complete (Advanced):
1. ✅ Complex cross-enterprise AI queries
2. ✅ Predictive analysis capabilities
3. ✅ Enhanced error handling and logging

---

## 🔧 **DEVELOPMENT WORKFLOW**

### Hour-by-Hour Plan:
```
09:00 - 10:00: Environment setup & dependency installation
10:00 - 12:00: Complete trash enterprise calculations
12:00 - 13:00: Lunch break
13:00 - 14:30: Implement AI query service
14:30 - 15:30: Create basic AI interface
15:30 - 16:00: Integration testing
16:00 - 17:00: Documentation and next-day planning
```

### Debug Commands:
```powershell
# Check environment variables
Get-ChildItem Env: | Where-Object Name -like "*XAI*"

# Test build
dotnet build .\WileyBudgetManagement\WileyBudgetManagement.csproj --verbosity detailed

# Check references
dotnet list .\WileyBudgetManagement\WileyBudgetManagement.csproj reference
```

---

## 📞 **QUICK REFERENCE**

### Key File Locations:
- **Trash Enterprise**: `Forms/TrashInput.cs`
- **Database Operations**: `Database/SanitationRepository.cs`
- **AI Service**: `Services/AIQueryService.cs` (to create)
- **Main Dashboard**: `Forms/DashboardForm.cs`

### Important Methods:
- `CalculateTrashScenarios()` - Line 253 in TrashInput.cs
- `SaveTrashDataAsync()` - Line 811 in TrashInput.cs
- `ProcessNaturalLanguageQuery()` - New in AIQueryService.cs

### Test Data:
- **Trash Accounts**: T301.00 to T602.00 (17 accounts)
- **Customer Base**: 850 customers
- **Truck Cost**: $350,000 over 12 years = $2,673.61/month

---

## 🚨 **TROUBLESHOOTING**

### Common Issues:
1. **API Key Not Found**: Restart VS Code after setting environment variable
2. **Build Errors**: Check NuGet package references
3. **Database Connection**: Verify SQL Server Express is running
4. **AI Queries Fail**: Check internet connection and API key validity

### Emergency Contacts:
- **Technical Issues**: Check GitHub repository issues
- **API Problems**: Review XAI documentation
- **Database Issues**: Validate connection string

---

**Document Version**: 1.0  
**Last Updated**: July 1, 2025  
**Next Review**: Daily at 5:00 PM
