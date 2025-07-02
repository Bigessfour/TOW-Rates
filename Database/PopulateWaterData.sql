-- Water Enterprise Data Population Script
-- Town of Wiley Budget Management - Water District Implementation
-- Date: July 1, 2025
-- Purpose: Populate Water table with comprehensive revenue, operating, infrastructure, and quality data

-- Clear existing Water data to ensure clean population
DELETE FROM Water;

-- Reset identity seed
DBCC CHECKIDENT('Water', RESEED, 0);

-- WATER REVENUE ACCOUNTS (Section: Revenue)
-- Based on Rate Study Methodology for Water District operations
INSERT INTO Water (
    Account, Label, Section, CurrentFYBudget, SeasonalAdjustment, MonthlyInput, 
    SeasonalRevenueFactor, YearToDateSpending, PercentOfBudget, BudgetRemaining,
    GoalAdjustment, ReserveTarget, Scenario1, Scenario2, Scenario3,
    PercentAllocation, RequiredRate, MonthlyUsage, TimeOfUseFactor,
    CustomerAffordabilityIndex, QuarterlySummary, EntryDate, Total
) VALUES 
-- Primary Water Sales Revenue
('W301.00', 'Water Sales', 'Revenue', 180000.00, 5000.00, 15000.00,
 1.30, 95000.00, 0.528, 85000.00, 0.00, 18000.00, 19544.22, 16962.16, 16479.86,
 0.70, 21.18, 12500000, 1.20, 0.95, 47500.00, GETDATE(), 185000.00),

-- Property Tax Revenue for Water
('W311.00', 'Specific Ownership Taxes - Water', 'Revenue', 18500.00, 0.00, 1541.67,
 1.00, 9250.00, 0.500, 9250.00, 0.00, 1850.00, 1855.22, 1504.38, 1504.22,
 0.25, 2.18, 0, 1.00, 1.00, 4625.00, GETDATE(), 18500.00),

-- Delinquent Tax Collections
('W310.10', 'Delinquent Taxes - Water', 'Revenue', 3000.00, 0.00, 250.00,
 1.00, 1500.00, 0.500, 1500.00, 0.00, 300.00, 263.61, 252.05, 252.47,
 0.25, 0.35, 0, 1.00, 1.00, 750.00, GETDATE(), 3000.00),

-- Interest and Penalties
('W320.00', 'Penalties and Interest - Water', 'Revenue', 8000.00, 0.00, 666.67,
 1.00, 4000.00, 0.500, 4000.00, 0.00, 800.00, 677.22, 657.83, 658.14,
 0.25, 0.94, 0, 1.00, 1.00, 2000.00, GETDATE(), 8000.00),

-- Investment Interest Revenue
('W315.00', 'Interest on Investments - Water', 'Revenue', 25000.00, 0.00, 2083.33,
 1.00, 12500.00, 0.500, 12500.00, 0.00, 2500.00, 2137.55, 2045.22, 2061.69,
 0.25, 2.94, 0, 1.00, 1.00, 6250.00, GETDATE(), 25000.00),

-- Grant Revenue
('W322.00', 'Water System Grant', 'Revenue', 15000.00, 0.00, 1250.00,
 1.00, 7500.00, 0.500, 7500.00, 0.00, 1500.00, 1294.22, 1212.16, 1229.86,
 0.25, 1.76, 0, 1.00, 1.00, 3750.00, GETDATE(), 15000.00);

-- WATER OPERATING EXPENSES (Section: Operating)
-- Day-to-day operational costs for water system
INSERT INTO Water (
    Account, Label, Section, CurrentFYBudget, SeasonalAdjustment, MonthlyInput,
    SeasonalRevenueFactor, YearToDateSpending, PercentOfBudget, BudgetRemaining,
    GoalAdjustment, ReserveTarget, Scenario1, Scenario2, Scenario3,
    PercentAllocation, RequiredRate, MonthlyUsage, TimeOfUseFactor,
    CustomerAffordabilityIndex, QuarterlySummary, EntryDate, Total
) VALUES
-- Regulatory and Permits
('W401.00', 'Water System Permits', 'Operating', 2500.00, 0.00, 208.33,
 1.00, 1250.00, 0.500, 1250.00, 0.00, 250.00, 890.05, 404.55, 208.33,
 0.05, 0.29, 0, 1.00, 1.00, 625.00, GETDATE(), 2500.00),

-- Administrative Supplies
('W410.00', 'Office Supplies - Water', 'Operating', 1500.00, 0.00, 125.00,
 1.00, 750.00, 0.500, 750.00, 0.00, 150.00, 806.66, 321.22, 125.00,
 0.05, 0.18, 0, 1.00, 1.00, 375.00, GETDATE(), 1500.00),

-- System Maintenance and Repairs
('W415.00', 'Water System Repairs', 'Operating', 15000.00, 2000.00, 1250.00,
 1.00, 8500.00, 0.567, 6500.00, 1000.00, 1500.00, 1931.66, 1446.22, 1375.00,
 0.15, 2.12, 0, 1.30, 1.00, 4250.00, GETDATE(), 18000.00),

-- Utility Costs for Water Plant
('W418.00', 'Water Plant Utilities', 'Operating', 24000.00, 1500.00, 2000.00,
 1.00, 13500.00, 0.563, 10500.00, 0.00, 2400.00, 2681.66, 2196.22, 2125.00,
 0.15, 3.53, 0, 1.40, 1.00, 6500.00, GETDATE(), 25500.00),

-- Chemical Treatment Costs
('W425.00', 'Water Treatment Chemicals', 'Operating', 8500.00, 500.00, 708.33,
 1.00, 4750.00, 0.559, 3750.00, 0.00, 850.00, 1190.16, 904.55, 833.33,
 0.10, 1.24, 0, 1.10, 1.00, 2250.00, GETDATE(), 9000.00),

-- Insurance Costs
('W430.00', 'Water System Insurance', 'Operating', 5500.00, 0.00, 458.33,
 1.00, 2750.00, 0.500, 2750.00, 0.00, 550.00, 1140.05, 654.55, 458.33,
 0.05, 0.81, 0, 1.00, 1.00, 1375.00, GETDATE(), 5500.00),

-- Vehicle and Equipment Fuel
('W491.00', 'Vehicle Fuel - Water', 'Operating', 3500.00, 300.00, 291.67,
 1.00, 2050.00, 0.586, 1450.00, 0.00, 350.00, 973.33, 487.88, 366.67,
 0.05, 0.51, 0, 1.20, 1.00, 875.00, GETDATE(), 3800.00);

-- WATER INFRASTRUCTURE INVESTMENTS (Section: Infrastructure)
-- Capital improvements and long-term infrastructure projects
INSERT INTO Water (
    Account, Label, Section, CurrentFYBudget, SeasonalAdjustment, MonthlyInput,
    SeasonalRevenueFactor, YearToDateSpending, PercentOfBudget, BudgetRemaining,
    GoalAdjustment, ReserveTarget, Scenario1, Scenario2, Scenario3,
    PercentAllocation, RequiredRate, MonthlyUsage, TimeOfUseFactor,
    CustomerAffordabilityIndex, QuarterlySummary, EntryDate, Total
) VALUES
-- Water Line Infrastructure
('W500.00', 'Water Line Replacement', 'Infrastructure', 45000.00, 5000.00, 3750.00,
 1.00, 27500.00, 0.611, 17500.00, 10000.00, 4500.00, 8294.22, 5712.16, 5229.86,
 1.00, 8.82, 0, 1.00, 1.00, 13750.00, GETDATE(), 60000.00),

-- Water Meter Infrastructure
('W501.00', 'Water Meter Replacement', 'Infrastructure', 25000.00, 0.00, 2083.33,
 1.00, 12500.00, 0.500, 12500.00, 5000.00, 2500.00, 6627.55, 4045.49, 3562.19,
 1.00, 4.91, 0, 1.00, 1.00, 6250.00, GETDATE(), 35000.00),

-- Water Plant Equipment
('W502.00', 'Water Plant Equipment', 'Infrastructure', 35000.00, 0.00, 2916.67,
 1.00, 17500.00, 0.500, 17500.00, 15000.00, 3500.00, 7460.89, 4878.83, 4396.53,
 1.00, 6.87, 0, 1.00, 1.00, 8750.00, GETDATE(), 65000.00),

-- Water Storage Infrastructure
('W503.00', 'Water Storage Tank Maintenance', 'Infrastructure', 12000.00, 2000.00, 1000.00,
 1.00, 7000.00, 0.583, 5000.00, 3000.00, 1200.00, 5544.22, 2962.16, 2479.86,
 1.00, 2.35, 0, 1.00, 1.00, 3500.00, GETDATE(), 17000.00);

-- WATER QUALITY ASSURANCE (Section: Quality)
-- Quality control, testing, and compliance programs
INSERT INTO Water (
    Account, Label, Section, CurrentFYBudget, SeasonalAdjustment, MonthlyInput,
    SeasonalRevenueFactor, YearToDateSpending, PercentOfBudget, BudgetRemaining,
    GoalAdjustment, ReserveTarget, Scenario1, Scenario2, Scenario3,
    PercentAllocation, RequiredRate, MonthlyUsage, TimeOfUseFactor,
    CustomerAffordabilityIndex, QuarterlySummary, EntryDate, Total
) VALUES
-- Water Quality Testing Program
('W405.00', 'Water Quality Testing', 'Quality', 3500.00, 0.00, 291.67,
 1.00, 1750.00, 0.500, 1750.00, 0.00, 350.00, 1771.53, 321.22, 1771.53,
 0.05, 0.51, 0, 1.00, 1.00, 875.00, GETDATE(), 3500.00),

-- EPA Compliance Programs
('W405.10', 'EPA Compliance', 'Quality', 2000.00, 0.00, 166.67,
 1.00, 1000.00, 0.500, 1000.00, 0.00, 200.00, 393.33, 225.50, 1646.53,
 0.03, 0.29, 0, 1.00, 1.00, 500.00, GETDATE(), 2000.00),

-- Operator Training and Certification
('W413.40', 'Water Operator Training', 'Quality', 1500.00, 0.00, 125.00,
 1.00, 750.00, 0.500, 750.00, 0.00, 150.00, 352.72, 183.83, 1604.86,
 0.02, 0.22, 0, 1.00, 1.00, 375.00, GETDATE(), 1500.00);

-- SPECIALIZED WATER ACCOUNTS
-- Additional water-specific operational needs
INSERT INTO Water (
    Account, Label, Section, CurrentFYBudget, SeasonalAdjustment, MonthlyInput,
    SeasonalRevenueFactor, YearToDateSpending, PercentOfBudget, BudgetRemaining,
    GoalAdjustment, ReserveTarget, Scenario1, Scenario2, Scenario3,
    PercentAllocation, RequiredRate, MonthlyUsage, TimeOfUseFactor,
    CustomerAffordabilityIndex, QuarterlySummary, EntryDate, Total
) VALUES
-- Emergency Water Supply
('W506.00', 'Emergency Water Supply', 'Infrastructure', 8000.00, 1000.00, 666.67,
 1.00, 4500.00, 0.563, 3500.00, 2000.00, 800.00, 5210.89, 2628.83, 2146.53,
 0.50, 1.57, 0, 1.00, 1.00, 2250.00, GETDATE(), 11000.00),

-- Water Conservation Programs
('W302.00', 'Water Conservation Rebates', 'Revenue', 5000.00, 0.00, 416.67,
 1.00, 2500.00, 0.500, 2500.00, 0.00, 500.00, 460.89, 378.83, 396.53,
 0.15, 0.59, 850000, 0.80, 1.00, 1250.00, GETDATE(), 5000.00),

-- Water Leak Detection
('W420.00', 'Water Leak Detection', 'Operating', 4500.00, 800.00, 375.00,
 1.00, 2650.00, 0.589, 1850.00, 500.00, 450.00, 1056.66, 571.22, 500.00,
 0.10, 0.66, 0, 1.20, 1.00, 1125.00, GETDATE(), 5800.00),

-- Customer Service - Water
('W435.00', 'Customer Service - Water', 'Operating', 6000.00, 0.00, 500.00,
 1.00, 3000.00, 0.500, 3000.00, 0.00, 600.00, 1181.66, 696.22, 625.00,
 0.05, 0.88, 0, 1.00, 1.00, 1500.00, GETDATE(), 6000.00);

-- CALCULATE SUMMARY STATISTICS
-- Add summary record for Water Enterprise
DECLARE @TotalRevenue DECIMAL(18,2);
DECLARE @TotalOperating DECIMAL(18,2);
DECLARE @TotalAdmin DECIMAL(18,2);
DECLARE @NetSurplus DECIMAL(18,2);
DECLARE @AvgRate DECIMAL(18,2);

SELECT @TotalRevenue = SUM(CurrentFYBudget) FROM Water WHERE Section = 'Revenue';
SELECT @TotalOperating = SUM(CurrentFYBudget) FROM Water WHERE Section = 'Operating';
SELECT @TotalAdmin = SUM(CurrentFYBudget) FROM Water WHERE Section IN ('Infrastructure', 'Quality');
SELECT @NetSurplus = SUM(CASE WHEN Section = 'Revenue' THEN CurrentFYBudget ELSE -CurrentFYBudget END) FROM Water;
SELECT @AvgRate = AVG(RequiredRate) FROM Water WHERE RequiredRate > 0;

INSERT INTO Summary (
    Enterprise, TotalOperatingIncome, TotalOMExpenses, TotalAdminExpenses,
    NetSurplusDeficit, PercentOfTotalBudget, RequiredRate, TrendData, EntryDate
) VALUES (
    'Water', @TotalRevenue, @TotalOperating, @TotalAdmin, @NetSurplus,
    0.25, @AvgRate, 'Q1: $134,750, Q2: $142,250, Q3: $148,500, Q4: $139,000',
    GETDATE()
);

-- VERIFICATION QUERIES
-- Check record counts by section
SELECT 
    Section,
    COUNT(*) as RecordCount,
    SUM(CurrentFYBudget) as TotalBudget,
    AVG(RequiredRate) as AvgRequiredRate
FROM Water 
GROUP BY Section
ORDER BY Section;

-- Check infrastructure allocation (should be at least 15% of total budget)
DECLARE @InfraTotal DECIMAL(18,2);
DECLARE @TotalBudget DECIMAL(18,2);
DECLARE @InfraPercent DECIMAL(5,2);

SELECT @InfraTotal = SUM(CurrentFYBudget) FROM Water WHERE Section = 'Infrastructure';
SELECT @TotalBudget = SUM(CurrentFYBudget) FROM Water;
SET @InfraPercent = CAST(@InfraTotal * 100.0 / @TotalBudget AS DECIMAL(5,2));

SELECT 
    'Infrastructure Allocation Check' as CheckType,
    @InfraTotal as InfrastructureTotal,
    @TotalBudget as TotalBudget,
    @InfraPercent as InfrastructurePercent;

-- Check quality assurance allocation (should be at least 5% of total budget)
DECLARE @QualityTotal DECIMAL(18,2);
DECLARE @QualityPercent DECIMAL(5,2);

SELECT @QualityTotal = SUM(CurrentFYBudget) FROM Water WHERE Section = 'Quality';
SET @QualityPercent = CAST(@QualityTotal * 100.0 / @TotalBudget AS DECIMAL(5,2));

SELECT 
    'Quality Allocation Check' as CheckType,
    @QualityTotal as QualityTotal,
    @TotalBudget as TotalBudget,
    @QualityPercent as QualityPercent;

-- Check revenue vs expense balance
DECLARE @RevenueTotal DECIMAL(18,2);
DECLARE @ExpenseTotal DECIMAL(18,2);
DECLARE @NetBalance DECIMAL(18,2);

SELECT @RevenueTotal = SUM(CurrentFYBudget) FROM Water WHERE Section = 'Revenue';
SELECT @ExpenseTotal = SUM(CurrentFYBudget) FROM Water WHERE Section != 'Revenue';
SET @NetBalance = @RevenueTotal - @ExpenseTotal;

SELECT 
    'Revenue vs Expense Balance' as CheckType,
    @RevenueTotal as TotalRevenue,
    @ExpenseTotal as TotalExpenses,
    @NetBalance as NetSurplusDeficit;

-- Final water enterprise totals
DECLARE @TotalRecords INT;
DECLARE @TotalYTD DECIMAL(18,2);
DECLARE @AvgBudgetUtil DECIMAL(5,4);
DECLARE @TotalUsage DECIMAL(18,2);

SELECT @TotalRecords = COUNT(*) FROM Water;
SELECT @TotalYTD = SUM(YearToDateSpending) FROM Water;
SELECT @AvgBudgetUtil = AVG(PercentOfBudget) FROM Water;
SELECT @TotalUsage = SUM(MonthlyUsage) FROM Water;

SELECT 
    'WATER ENTERPRISE TOTALS' as Summary,
    @TotalRecords as TotalRecords,
    @TotalBudget as TotalBudget,
    @TotalYTD as TotalYTDSpending,
    @AvgBudgetUtil as AvgBudgetUtilization,
    @TotalUsage as TotalMonthlyUsage;

PRINT 'Water Enterprise database population completed successfully!';
PRINT 'Total Records: ' + CAST(@TotalRecords AS VARCHAR(10));
PRINT 'Total Budget: $' + CAST(@TotalBudget AS VARCHAR(20));
PRINT 'Infrastructure Investment: $' + CAST(@InfraTotal AS VARCHAR(20));
PRINT 'Quality Assurance Investment: $' + CAST(@QualityTotal AS VARCHAR(20));
