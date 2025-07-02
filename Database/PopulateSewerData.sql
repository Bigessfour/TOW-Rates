-- Populate Sewer Enterprise Data (Fully Implemented)
-- Date: July 1, 2025

USE [WileyBudgetManagement];
GO

-- Insert Sewer (Sanitation District) Revenue Data
INSERT INTO [SanitationDistrict] (
    Account, Label, Section, CurrentFYBudget, SeasonalAdjustment, MonthlyInput, 
    SeasonalRevenueFactor, YearToDateSpending, PercentOfBudget, BudgetRemaining,
    GoalAdjustment, ReserveTarget, Scenario1, Scenario2, Scenario3, 
    PercentAllocation, RequiredRate, TimeOfUseFactor, CustomerAffordabilityIndex
) VALUES
-- Revenue Items
('311.00', 'Specific Ownership Taxes', 'Revenue', 15500.00, 0, 1291.67, 1.0, 7750.00, 0.50, 7750.00, 0, 0, 1291.67, 1291.67, 1291.67, 0, 1.83, 1.0, 1.0),
('301.00', 'Sewage Sales', 'Revenue', 100000.00, 0, 8333.33, 1.2, 50000.00, 0.50, 50000.00, 0, 0, 8556.33, 8402.22, 8452.89, 0, 11.76, 1.1, 0.9),
('310.10', 'Delinquent Taxes', 'Revenue', 2500.00, 0, 208.33, 1.0, 1250.00, 0.50, 1250.00, 0, 0, 208.33, 208.33, 208.33, 0, 0.29, 1.0, 1.0),
('334.00', 'Permits and Assessments', 'Revenue', 976.00, 0, 81.33, 1.0, 488.00, 0.50, 488.00, 0, 0, 81.33, 81.33, 81.33, 0, 0.11, 1.0, 1.0),
('361.00', 'Interest on Investments', 'Revenue', 48500.00, 0, 4041.67, 1.1, 24250.00, 0.50, 24250.00, 0, 0, 4041.67, 4041.67, 4041.67, 0, 5.71, 1.0, 1.0),
('390.00', 'Other Revenues', 'Revenue', 16624.00, 0, 1385.33, 0.8, 8312.00, 0.50, 8312.00, 0, 0, 1385.33, 1385.33, 1385.33, 0, 1.96, 1.0, 1.0);

-- Operating Expenses
INSERT INTO [SanitationDistrict] (
    Account, Label, Section, CurrentFYBudget, SeasonalAdjustment, MonthlyInput,
    YearToDateSpending, PercentOfBudget, BudgetRemaining, GoalAdjustment,
    ReserveTarget, Scenario1, Scenario2, Scenario3, TimeOfUseFactor, CustomerAffordabilityIndex
) VALUES
('418.00', 'Lift Station Utilities', 'Operating', 15000.00, 500, 1250.00, 7500.00, 0.50, 7500.00, 0, 1500, 1250.00, 1250.00, 1250.00, 1.2, 1.0),
('432.33', 'Sewer Line Maintenance', 'Operating', 12000.00, 2000, 1000.00, 6000.00, 0.50, 6000.00, 0, 1200, 1000.00, 1000.00, 1000.00, 1.5, 1.0),
('432.53', 'Sewer Cleaning', 'Operating', 7600.00, 1500, 633.33, 3800.00, 0.50, 3800.00, 0, 760, 633.33, 633.33, 633.33, 1.5, 1.0),
('432.55', 'Equipment Repairs', 'Operating', 6000.00, 800, 500.00, 3000.00, 0.50, 3000.00, 0, 600, 500.00, 500.00, 500.00, 1.3, 1.0),
('433.00', 'Professional Services', 'Operating', 17036.00, 0, 1419.67, 8518.00, 0.50, 8518.00, 0, 1704, 1419.67, 1419.67, 1419.67, 1.0, 1.0);

-- Administrative & General Expenses  
INSERT INTO [SanitationDistrict] (
    Account, Label, Section, CurrentFYBudget, SeasonalAdjustment, MonthlyInput,
    YearToDateSpending, PercentOfBudget, BudgetRemaining, GoalAdjustment,
    ReserveTarget, Scenario1, Scenario2, Scenario3, PercentAllocation, TimeOfUseFactor, CustomerAffordabilityIndex
) VALUES
('510.00', 'Supt Salaries', 'Admin', 26000.00, 0, 2166.67, 13000.00, 0.50, 13000.00, 0, 2600, 2166.67, 2166.67, 2166.67, 25.0, 1.0, 1.0),
('511.00', 'Clerk Salaries', 'Admin', 26000.00, 0, 2166.67, 13000.00, 0.50, 13000.00, 0, 2600, 2166.67, 2166.67, 2166.67, 25.0, 1.0, 1.0),
('512.00', 'FICA', 'Admin', 3978.00, 0, 331.50, 1989.00, 0.50, 1989.00, 0, 398, 331.50, 331.50, 331.50, 25.0, 1.0, 1.0),
('513.00', 'Benefits', 'Admin', 10400.00, 0, 866.67, 5200.00, 0.50, 5200.00, 0, 1040, 866.67, 866.67, 866.67, 25.0, 1.0, 1.0),
('521.00', 'Accounting', 'Admin', 8722.00, 0, 726.83, 4361.00, 0.50, 4361.00, 0, 872, 726.83, 726.83, 726.83, 100.0, 1.0, 1.0),
('531.00', 'General Insurance', 'Admin', 15000.00, 0, 1250.00, 7500.00, 0.50, 7500.00, 0, 1500, 1250.00, 1250.00, 1250.00, 50.0, 1.0, 1.0);

PRINT 'Sewer Enterprise data populated successfully.';
PRINT 'Ready for Water, Trash, and Apartments implementation.';
GO
