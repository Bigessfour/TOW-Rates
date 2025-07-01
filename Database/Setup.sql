-- SQL Server Express schema for WileyBudgetManagement
CREATE TABLE SanitationDistrict (
    Account NVARCHAR(50),
    Label NVARCHAR(100),
    Section NVARCHAR(50),
    CurrentFYBudget DECIMAL(18,2),
    SeasonalAdjustment DECIMAL(18,2),
    MonthlyInput DECIMAL(18,2),
    SeasonalRevenueFactor DECIMAL(18,4),
    YearToDateSpending DECIMAL(18,2),
    PercentOfBudget DECIMAL(5,2),
    BudgetRemaining DECIMAL(18,2),
    GoalAdjustment DECIMAL(18,2),
    ReserveTarget DECIMAL(18,2),
    Scenario1 DECIMAL(18,2),
    Scenario2 DECIMAL(18,2),
    Scenario3 DECIMAL(18,2),
    PercentAllocation DECIMAL(5,2),
    RequiredRate DECIMAL(18,2),
    MonthlyUsage DECIMAL(18,2),
    TimeOfUseFactor DECIMAL(5,2),
    CustomerAffordabilityIndex DECIMAL(5,2),
    QuarterlySummary DECIMAL(18,2),
    EntryDate DATETIME,
    Total DECIMAL(18,2)
);

CREATE TABLE Water (
    -- Same fields as SanitationDistrict
    Account NVARCHAR(50),
    Label NVARCHAR(100),
    Section NVARCHAR(50),
    CurrentFYBudget DECIMAL(18,2),
    SeasonalAdjustment DECIMAL(18,2),
    MonthlyInput DECIMAL(18,2),
    SeasonalRevenueFactor DECIMAL(18,4),
    YearToDateSpending DECIMAL(18,2),
    PercentOfBudget DECIMAL(5,2),
    BudgetRemaining DECIMAL(18,2),
    GoalAdjustment DECIMAL(18,2),
    ReserveTarget DECIMAL(18,2),
    Scenario1 DECIMAL(18,2),
    Scenario2 DECIMAL(18,2),
    Scenario3 DECIMAL(18,2),
    PercentAllocation DECIMAL(5,2),
    RequiredRate DECIMAL(18,2),
    MonthlyUsage DECIMAL(18,2),
    TimeOfUseFactor DECIMAL(5,2),
    CustomerAffordabilityIndex DECIMAL(5,2),
    QuarterlySummary DECIMAL(18,2),
    EntryDate DATETIME,
    Total DECIMAL(18,2)
);

CREATE TABLE Trash (
    -- Same fields as SanitationDistrict
    Account NVARCHAR(50),
    Label NVARCHAR(100),
    Section NVARCHAR(50),
    CurrentFYBudget DECIMAL(18,2),
    SeasonalAdjustment DECIMAL(18,2),
    MonthlyInput DECIMAL(18,2),
    SeasonalRevenueFactor DECIMAL(18,4),
    YearToDateSpending DECIMAL(18,2),
    PercentOfBudget DECIMAL(5,2),
    BudgetRemaining DECIMAL(18,2),
    GoalAdjustment DECIMAL(18,2),
    ReserveTarget DECIMAL(18,2),
    Scenario1 DECIMAL(18,2),
    Scenario2 DECIMAL(18,2),
    Scenario3 DECIMAL(18,2),
    PercentAllocation DECIMAL(5,2),
    RequiredRate DECIMAL(18,2),
    MonthlyUsage DECIMAL(18,2),
    TimeOfUseFactor DECIMAL(5,2),
    CustomerAffordabilityIndex DECIMAL(5,2),
    QuarterlySummary DECIMAL(18,2),
    EntryDate DATETIME,
    Total DECIMAL(18,2)
);

CREATE TABLE Apartments (
    -- Same fields as SanitationDistrict
    Account NVARCHAR(50),
    Label NVARCHAR(100),
    Section NVARCHAR(50),
    CurrentFYBudget DECIMAL(18,2),
    SeasonalAdjustment DECIMAL(18,2),
    MonthlyInput DECIMAL(18,2),
    SeasonalRevenueFactor DECIMAL(18,4),
    YearToDateSpending DECIMAL(18,2),
    PercentOfBudget DECIMAL(5,2),
    BudgetRemaining DECIMAL(18,2),
    GoalAdjustment DECIMAL(18,2),
    ReserveTarget DECIMAL(18,2),
    Scenario1 DECIMAL(18,2),
    Scenario2 DECIMAL(18,2),
    Scenario3 DECIMAL(18,2),
    PercentAllocation DECIMAL(5,2),
    RequiredRate DECIMAL(18,2),
    MonthlyUsage DECIMAL(18,2),
    TimeOfUseFactor DECIMAL(5,2),
    CustomerAffordabilityIndex DECIMAL(5,2),
    QuarterlySummary DECIMAL(18,2),
    EntryDate DATETIME,
    Total DECIMAL(18,2)
);

CREATE TABLE Summary (
    Enterprise NVARCHAR(50),
    TotalOperatingIncome DECIMAL(18,2),
    TotalOMExpenses DECIMAL(18,2),
    TotalAdminExpenses DECIMAL(18,2),
    NetSurplusDeficit DECIMAL(18,2),
    PercentOfTotalBudget DECIMAL(5,2),
    RequiredRate DECIMAL(18,2),
    TrendData NVARCHAR(MAX)
);
