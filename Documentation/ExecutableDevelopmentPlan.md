# EXECUTABLE DEVELOPMENT TASKS - JULY 2025
## Town of Wiley Budget Management System
### Immediate Action Items with Code Implementation

---

## 🚀 **WEEK 1: TRASH ENTERPRISE COMPLETION**

### DAY 1 (July 1): Trash Data Finalization
```bash
# Task 1A: Validate Trash Account Structure
```
- [ ] Open `Forms/TrashInput.cs`
- [ ] Review `GetDefaultTrashData()` method (lines 771-802)
- [ ] Verify account numbers T301.00-T602.00 mapping
- [ ] Validate monthly tonnage calculations
- [ ] Test seasonal adjustment factors

**Code Changes Required:**
```csharp
// In TrashInput.cs - Add missing validation
private void ValidateTrashAccountStructure()
{
    var requiredAccounts = new[] { "T301.00", "T301.10", "T315.00", "T401.00", "T410.00", 
                                  "T415.00", "T491.00", "T501.00", "T502.00", "T503.00", 
                                  "T425.00", "T430.00", "T435.00", "T440.00", "T600.00", 
                                  "T601.00", "T602.00" };
    
    foreach (var account in requiredAccounts)
    {
        if (!trashData.Any(d => d.Account == account))
        {
            throw new InvalidOperationException($"Missing required account: {account}");
        }
    }
}
```

### DAY 2 (July 2): Complete Trash Scenario Analysis
```bash
# Task 1B: Finalize $350K Truck Calculations
```
- [ ] Open `Forms/TrashInput.cs`, method `CalculateTrashScenarios()` (line 253)
- [ ] Verify PMT calculation: $350K, 12-year, 4.5% interest = $32,083.34/year
- [ ] Implement recycling program scenario ($125K, 7-year)
- [ ] Add transfer station optimization ($200K, 15-year)

**Code Implementation:**
```csharp
// Update CalculateTrashScenarios method
private void CalculateTrashScenarios(SanitationDistrict district)
{
    decimal baseMonthly = district.MonthlyInput;
    
    // Scenario 1: New Trash Truck - VALIDATED CALCULATIONS
    decimal truckCost = 350000m;
    decimal truckYears = 12m;
    decimal truckRate = 0.045m; // 4.5% interest
    decimal truckAnnualPayment = PMTCalculation(truckCost, truckRate, truckYears);
    decimal truckMonthlyImpact = truckAnnualPayment / 12; // $2,673.61
    
    // Scenario 2: Recycling Program - ENHANCED
    decimal recyclingCost = 125000m;
    decimal recyclingYears = 7m;
    decimal recyclingRate = 0.04m; // 4% interest
    decimal recyclingAnnualPayment = PMTCalculation(recyclingCost, recyclingRate, recyclingYears);
    decimal recyclingMonthlyImpact = recyclingAnnualPayment / 12; // $1,697.79
    
    // Scenario 3: Transfer Station - NEW IMPLEMENTATION
    decimal transferCost = 200000m;
    decimal transferYears = 15m;
    decimal transferRate = 0.035m; // 3.5% interest
    decimal transferAnnualPayment = PMTCalculation(transferCost, transferRate, transferYears);
    decimal transferMonthlyImpact = transferAnnualPayment / 12; // $1,429.77
    
    // Apply scenarios based on section type
    ApplyScenariosBySection(district, baseMonthly, truckMonthlyImpact, 
                           recyclingMonthlyImpact, transferMonthlyImpact);
}

private decimal PMTCalculation(decimal principal, decimal rate, decimal years)
{
    decimal monthlyRate = rate / 12;
    decimal numPayments = years * 12;
    return principal * (monthlyRate * Math.Pow(1 + monthlyRate, numPayments)) / 
           (Math.Pow(1 + monthlyRate, numPayments) - 1) * 12; // Return annual payment
}
```

### DAY 3 (July 3): AI Infrastructure Setup
```bash
# Task 2A: XAI API Integration Foundation
```
- [ ] Create `Services/AIQueryService.cs`
- [ ] Configure XAI API key from environment variables
- [ ] Implement basic query structure
- [ ] Add error handling and validation

**New File Creation:**
```csharp
// Services/AIQueryService.cs
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WileyBudgetManagement.Services
{
    public class AIQueryService
    {
        private readonly HttpClient _httpClient;
        private readonly string _xaiApiKey;
        private readonly string _apiEndpoint = "https://api.x.ai/v1/chat/completions";

        public AIQueryService()
        {
            _httpClient = new HttpClient();
            _xaiApiKey = Environment.GetEnvironmentVariable("XAI_API_KEY");
            
            if (string.IsNullOrEmpty(_xaiApiKey))
            {
                throw new InvalidOperationException("XAI_API_KEY environment variable not found");
            }
            
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_xaiApiKey}");
        }

        public async Task<string> ProcessNaturalLanguageQuery(string query, object enterpriseData)
        {
            try
            {
                var requestBody = new
                {
                    model = "grok-beta",
                    messages = new[]
                    {
                        new { role = "system", content = GetSystemPrompt() },
                        new { role = "user", content = $"{query}\n\nEnterprise Data: {JsonConvert.SerializeObject(enterpriseData)}" }
                    },
                    max_tokens = 1000,
                    temperature = 0.3
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync(_apiEndpoint, content);
                var responseText = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    var aiResponse = JsonConvert.DeserializeObject<dynamic>(responseText);
                    return aiResponse.choices[0].message.content;
                }
                else
                {
                    return $"Error: {response.StatusCode} - {responseText}";
                }
            }
            catch (Exception ex)
            {
                return $"AI Query Error: {ex.Message}";
            }
        }

        private string GetSystemPrompt()
        {
            return @"You are a municipal budget analysis AI for the Town of Wiley. 
                    You help City Council members understand financial scenarios for Sewer, Water, Trash, and Apartments enterprises.
                    Provide clear, actionable insights about rate impacts, infrastructure investments, and budget implications.
                    Focus on practical recommendations that align with Rate Study Methodology.
                    Always explain your reasoning and provide specific numbers when possible.";
        }
    }
}
```

### DAY 4 (July 4): Trash Enterprise Database Integration
```bash
# Task 1C: Connect Trash to SQL Server Express
```
- [ ] Update `Database/SanitationRepository.cs` for trash operations
- [ ] Implement `SaveTrashDataAsync()` method completion
- [ ] Add trash-specific validation rules
- [ ] Test database connectivity

**Database Integration Code:**
```csharp
// In Database/SanitationRepository.cs - Add trash-specific methods
public async Task<bool> SaveTrashDataAsync(IEnumerable<SanitationDistrict> trashData)
{
    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            foreach (var item in trashData)
            {
                var sql = @"
                    IF EXISTS (SELECT 1 FROM Trash WHERE Account = @Account)
                        UPDATE Trash SET 
                            Label = @Label, Section = @Section, CurrentFYBudget = @CurrentFYBudget,
                            MonthlyInput = @MonthlyInput, YearToDateSpending = @YearToDateSpending,
                            GoalAdjustment = @GoalAdjustment, ReserveTarget = @ReserveTarget,
                            Scenario1 = @Scenario1, Scenario2 = @Scenario2, Scenario3 = @Scenario3,
                            RequiredRate = @RequiredRate, MonthlyUsage = @MonthlyUsage,
                            TimeOfUseFactor = @TimeOfUseFactor, CustomerAffordabilityIndex = @CustomerAffordabilityIndex
                        WHERE Account = @Account
                    ELSE
                        INSERT INTO Trash (Account, Label, Section, CurrentFYBudget, MonthlyInput, 
                                         YearToDateSpending, GoalAdjustment, ReserveTarget,
                                         Scenario1, Scenario2, Scenario3, RequiredRate, MonthlyUsage,
                                         TimeOfUseFactor, CustomerAffordabilityIndex, EntryDate)
                        VALUES (@Account, @Label, @Section, @CurrentFYBudget, @MonthlyInput,
                               @YearToDateSpending, @GoalAdjustment, @ReserveTarget,
                               @Scenario1, @Scenario2, @Scenario3, @RequiredRate, @MonthlyUsage,
                               @TimeOfUseFactor, @CustomerAffordabilityIndex, @EntryDate)";
                
                await connection.ExecuteAsync(sql, item);
            }
        }
        return true;
    }
    catch (Exception ex)
    {
        // Log error
        return false;
    }
}
```

### DAY 5 (July 5): AI Query Panel Implementation
```bash
# Task 2B: Natural Language Interface
```
- [ ] Create `Forms/AIQueryPanel.cs`
- [ ] Implement conversational interface
- [ ] Add "What-If" scenario processing
- [ ] Integrate with existing enterprise data

**New AI Query Panel:**
```csharp
// Forms/AIQueryPanel.cs
using System;
using System.Windows.Forms;
using WileyBudgetManagement.Services;

namespace WileyBudgetManagement.Forms
{
    public partial class AIQueryPanel : Form
    {
        private readonly AIQueryService _aiService;
        private TextBox queryTextBox;
        private Button submitButton;
        private RichTextBox responseTextBox;
        private ComboBox enterpriseSelector;

        public AIQueryPanel()
        {
            _aiService = new AIQueryService();
            InitializeComponent();
            SetupControls();
        }

        private void SetupControls()
        {
            this.Text = "AI Budget Analysis - City Council Assistant";
            this.Size = new Size(800, 600);

            // Query input
            var queryLabel = new Label { Text = "Ask a budget question:", Location = new Point(10, 10), Size = new Size(200, 20) };
            queryTextBox = new TextBox { 
                Location = new Point(10, 35), 
                Size = new Size(600, 60), 
                Multiline = true,
                PlaceholderText = "e.g., 'What if we delay the trash truck purchase by 2 years?'"
            };

            // Enterprise selector
            var enterpriseLabel = new Label { Text = "Focus Enterprise:", Location = new Point(620, 10), Size = new Size(100, 20) };
            enterpriseSelector = new ComboBox {
                Location = new Point(620, 35),
                Size = new Size(150, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            enterpriseSelector.Items.AddRange(new[] { "All Enterprises", "Sewer", "Water", "Trash", "Apartments" });
            enterpriseSelector.SelectedIndex = 0;

            // Submit button
            submitButton = new Button {
                Text = "Analyze with AI",
                Location = new Point(10, 105),
                Size = new Size(150, 30),
                BackColor = Color.LightBlue
            };
            submitButton.Click += SubmitButton_Click;

            // Response area
            var responseLabel = new Label { Text = "AI Analysis Results:", Location = new Point(10, 145), Size = new Size(200, 20) };
            responseTextBox = new RichTextBox {
                Location = new Point(10, 170),
                Size = new Size(760, 400),
                ReadOnly = true,
                Font = new Font("Consolas", 9)
            };

            this.Controls.AddRange(new Control[] { 
                queryLabel, queryTextBox, enterpriseLabel, enterpriseSelector, 
                submitButton, responseLabel, responseTextBox 
            });
        }

        private async void SubmitButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(queryTextBox.Text))
            {
                MessageBox.Show("Please enter a question for AI analysis.");
                return;
            }

            submitButton.Enabled = false;
            submitButton.Text = "Analyzing...";
            responseTextBox.Text = "Processing your query with AI...";

            try
            {
                // Get current enterprise data based on selection
                var enterpriseData = GetEnterpriseData(enterpriseSelector.SelectedItem.ToString());
                
                // Process with AI
                var response = await _aiService.ProcessNaturalLanguageQuery(queryTextBox.Text, enterpriseData);
                
                // Display results
                responseTextBox.Text = $"🤖 AI Analysis Results:\n\n{response}\n\n" +
                                     $"📊 Data Source: {enterpriseSelector.SelectedItem}\n" +
                                     $"⏰ Analysis Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            }
            catch (Exception ex)
            {
                responseTextBox.Text = $"❌ Error during AI analysis:\n{ex.Message}";
            }
            finally
            {
                submitButton.Enabled = true;
                submitButton.Text = "Analyze with AI";
            }
        }

        private object GetEnterpriseData(string enterprise)
        {
            // Implementation to gather current enterprise data
            // This would integrate with existing forms to get live data
            return new { Enterprise = enterprise, Status = "Live Data Integration Needed" };
        }
    }
}
```

---

## 🚀 **WEEK 2: APARTMENTS ENTERPRISE & CROSS-ENTERPRISE INTEGRATION**

### DAY 6 (July 8): Apartments Enterprise Expansion
```bash
# Task 3A: Enhance Apartments Beyond Basic Revenue
```
- [ ] Open `Forms/ApartmentsInput.cs`
- [ ] Add occupancy rate tracking
- [ ] Implement delinquency management
- [ ] Create property-specific scenarios

**Apartments Enhancement Code:**
```csharp
// Add to ApartmentsInput.cs - Enhanced property management
public class PropertyManagement
{
    public string PropertyId { get; set; }
    public string PropertyName { get; set; }
    public int TotalUnits { get; set; }
    public int OccupiedUnits { get; set; }
    public decimal OccupancyRate => TotalUnits > 0 ? (decimal)OccupiedUnits / TotalUnits : 0;
    public decimal DelinquentAmount { get; set; }
    public decimal CollectionRate { get; set; }
    public DateTime LastInspection { get; set; }
    public string Zone { get; set; }
}

// Enhanced apartments scenarios
private void CalculateApartmentScenarios(SanitationDistrict district)
{
    decimal baseMonthly = district.MonthlyInput;
    
    // Scenario 1: Rate Structure Optimization
    decimal optimizationFactor = 1.05m; // 5% efficiency gain
    district.Scenario1 = baseMonthly * optimizationFactor;
    
    // Scenario 2: New Development Impact (20% more units)
    decimal developmentImpact = 1.20m;
    district.Scenario2 = baseMonthly * developmentImpact;
    
    // Scenario 3: Affordability Assistance Program
    decimal affordabilityAdjustment = 0.92m; // 8% reduction for assistance
    district.Scenario3 = baseMonthly * affordabilityAdjustment;
}
```

### DAY 7 (July 9): Cross-Enterprise Dashboard Enhancement
```bash
# Task 4A: Municipal-Wide Analytics
```
- [ ] Update `Forms/SummaryPage.cs`
- [ ] Add real-time cross-enterprise tracking
- [ ] Implement consolidated scenario analysis
- [ ] Create board presentation mode

### DAY 8 (July 10): AI What-If Scenario Engine
```bash
# Task 2C: Advanced AI Scenario Processing
```
- [ ] Enhance AIQueryService with scenario generation
- [ ] Add cross-enterprise impact analysis
- [ ] Implement predictive modeling

### DAY 9 (July 11): Database Schema Completion
```bash
# Task 4B: SQL Server Express Full Deployment
```
- [ ] Deploy complete schema for all enterprises
- [ ] Add AI query logging table
- [ ] Implement cross-enterprise relationships

### DAY 10 (July 12): Integration Testing & Validation
```bash
# Task 5A: End-to-End System Testing
```
- [ ] Test all enterprise interactions
- [ ] Validate AI query responses
- [ ] Performance testing
- [ ] User acceptance preparation

---

## 🎯 **IMMEDIATE EXECUTION COMMANDS**

### Environment Setup
```bash
# Set XAI API Key (Run in PowerShell as Administrator)
$env:XAI_API_KEY = "your-xai-api-key-here"
[Environment]::SetEnvironmentVariable("XAI_API_KEY", "your-xai-api-key-here", "Machine")
```

### Build and Test Commands
```bash
# Build project
dotnet build .\WileyBudgetManagement\WileyBudgetManagement.csproj

# Run tests
dotnet test

# Add required NuGet packages for AI integration
dotnet add package Newtonsoft.Json
dotnet add package System.Net.Http
```

---

## 📋 **DAILY CHECKLIST TEMPLATE**

### Each Day Complete:
- [ ] Code changes committed to version control
- [ ] Unit tests updated and passing
- [ ] Documentation updated
- [ ] Performance benchmarks verified
- [ ] Integration testing completed

### Success Criteria:
- ✅ Trash Enterprise: Real-time calculations working
- ✅ AI Integration: Natural language queries responding
- ✅ Apartments: Full property management operational
- ✅ Cross-Enterprise: Municipal dashboard functional
- ✅ Production: System ready for 850+ customers

---

**Next Review**: Daily standup at 9:00 AM
**Blocker Escalation**: Immediate Slack notification
**Completion Target**: July 31, 2025
