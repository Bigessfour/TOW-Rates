-- Comprehensive Trash Enterprise Data Population Script
-- Based on Rate Study Methodology and TrashInput.cs implementation
-- Date: July 1, 2025

-- Clear existing Trash data
DELETE FROM Trash;

-- Insert comprehensive Trash Enterprise data
INSERT INTO Trash (
    Account, Label, Section, CurrentFYBudget, SeasonalAdjustment, MonthlyInput, 
    SeasonalRevenueFactor, YearToDateSpending, PercentOfBudget, BudgetRemaining, 
    GoalAdjustment, ReserveTarget, Scenario1, Scenario2, Scenario3, 
    PercentAllocation, RequiredRate, MonthlyUsage, TimeOfUseFactor, 
    CustomerAffordabilityIndex, QuarterlySummary, EntryDate, Total
) VALUES 

-- REVENUE ITEMS
('T311.00', 'Specific Ownership Taxes - Trash', 'Revenue', 22000.00, 0, 1833.33, 1.0, 10996.66, 0.50, 11003.34, 0, 0, 2215.86, 2147.13, 2092.40, 0.30, 8.63, 0, 1.0, 1.0, 5500.00, GETDATE(), 22000.00),

('T301.00', 'Residential Trash Collection Fees', 'Revenue', 320000.00, 8000, 26666.67, 1.1, 185333.34, 0.58, 134666.66, 0, 0, 326673.61, 321697.79, 319429.77, 0.85, 20.88, 850, 1.0, 0.95, 88000.00, GETDATE(), 328000.00),

('T301.10', 'Commercial Trash Collection Fees', 'Revenue', 180000.00, 5000, 15000.00, 1.2, 95000.00, 0.53, 85000.00, 0, 0, 182673.61, 181697.79, 181429.77, 0.85, 23.53, 425, 1.0, 1.0, 50000.00, GETDATE(), 185000.00),

('T320.00', 'Penalties and Interest - Trash', 'Revenue', 12000.00, 0, 1000.00, 1.0, 6000.00, 0.50, 6000.00, 0, 0, 1215.86, 1147.13, 1092.40, 0.30, 4.71, 0, 1.0, 1.0, 3000.00, GETDATE(), 12000.00),

('T315.00', 'Interest on Investments - Trash', 'Revenue', 8000.00, 0, 666.67, 1.0, 4000.00, 0.50, 4000.00, 0, 0, 815.86, 747.13, 692.40, 0.30, 3.14, 0, 1.0, 1.0, 2000.00, GETDATE(), 8000.00),

-- COLLECTION OPERATIONS
('T401.00', 'Collection Route Operations', 'Collections', 180000.00, 3000, 15000.00, 1.0, 105000.00, 0.58, 75000.00, 0, 0, 182673.61, 166697.79, 164429.77, 0, 21.18, 0, 1.1, 1.0, 48000.00, GETDATE(), 183000.00),

('T410.00', 'Collection Supplies', 'Collections', 8500.00, 500, 708.33, 1.0, 4958.33, 0.58, 3541.67, 0, 0, 11208.33, 10531.60, 10237.10, 0, 1.00, 0, 1.0, 1.0, 2625.00, GETDATE(), 9000.00),

('T415.00', 'Vehicle Maintenance - Collection', 'Collections', 25000.00, 2000, 2083.33, 1.0, 14583.33, 0.58, 10416.67, 0, 0, 27673.61, 26697.79, 26429.77, 0, 2.94, 0, 1.2, 1.0, 8000.00, GETDATE(), 27000.00),

('T491.00', 'Fuel - Collection Vehicles', 'Collections', 18000.00, 1000, 1500.00, 1.0, 10500.00, 0.58, 7500.00, 0, 0, 20673.61, 19697.79, 19429.77, 0, 2.12, 0, 1.3, 1.0, 5500.00, GETDATE(), 19000.00),

-- RECYCLING PROGRAM
('T501.00', 'Recycling Collection', 'Recycling', 45000.00, 1000, 3750.00, 1.0, 26250.00, 0.58, 18750.00, 5000, 0, 48267.32, 46848.90, 46214.89, 0, 5.29, 125, 1.0, 1.0, 12250.00, GETDATE(), 50000.00),

('T502.00', 'Recycling Processing', 'Recycling', 25000.00, 0, 2083.33, 1.0, 14583.33, 0.58, 10416.67, 3000, 0, 28083.33, 27373.52, 26929.77, 0, 2.94, 85, 1.0, 1.0, 7083.33, GETDATE(), 28000.00),

('T503.00', 'Recycling Education', 'Recycling', 8000.00, 0, 666.67, 1.0, 4666.67, 0.58, 3333.33, 1000, 0, 9666.67, 9373.52, 9229.77, 0, 0.94, 0, 1.0, 1.0, 2666.67, GETDATE(), 9000.00),

-- GENERAL OPERATIONS
('T425.00', 'Transfer Station Operations', 'Operations', 55000.00, 2500, 4583.33, 1.0, 32083.33, 0.58, 22916.67, 0, 0, 58134.71, 56726.08, 56172.10, 0, 6.47, 0, 1.1, 1.0, 15583.33, GETDATE(), 57500.00),

('T430.00', 'Trash Services Insurance', 'Operations', 12000.00, 0, 1000.00, 1.0, 7000.00, 0.58, 5000.00, 0, 0, 15134.71, 14726.08, 14472.10, 0, 1.41, 0, 1.0, 1.0, 3500.00, GETDATE(), 12000.00),

('T435.00', 'Landfill Tipping Fees', 'Operations', 75000.00, 3000, 6250.00, 1.0, 43750.00, 0.58, 31250.00, 0, 0, 78134.71, 76726.08, 76172.10, 0, 8.82, 0, 1.2, 1.0, 21250.00, GETDATE(), 78000.00),

('T440.00', 'Environmental Compliance', 'Operations', 8500.00, 0, 708.33, 1.0, 4958.33, 0.58, 3541.67, 0, 0, 11634.71, 11226.08, 10972.10, 0, 1.00, 0, 1.0, 1.0, 2625.00, GETDATE(), 8500.00),

-- EQUIPMENT & VEHICLES
('T600.00', 'Trash Collection Vehicles', 'Equipment', 50000.00, 0, 4166.67, 1.0, 29166.67, 0.58, 20833.33, 25000, 75000, 77588.95, 70206.67, 68638.44, 0, 17.65, 0, 1.0, 1.0, 25000.00, GETDATE(), 75000.00),

('T601.00', 'Collection Equipment', 'Equipment', 15000.00, 0, 1250.00, 1.0, 8750.00, 0.58, 6250.00, 5000, 20000, 18588.95, 17706.67, 17338.44, 0, 4.12, 0, 1.0, 1.0, 6250.00, GETDATE(), 20000.00),

('T602.00', 'Container Replacement', 'Equipment', 12000.00, 1000, 1000.00, 1.0, 7000.00, 0.58, 5000.00, 3000, 10000, 15588.95, 14706.67, 14338.44, 0, 2.94, 0, 1.0, 1.0, 4000.00, GETDATE(), 15000.00);

-- Update calculated Trash summary statistics
UPDATE Trash SET 
    YearToDateSpending = (MonthlyInput * 7) + SeasonalAdjustment,  -- Assuming July (month 7)
    PercentOfBudget = ((MonthlyInput * 7) + SeasonalAdjustment) / NULLIF(CurrentFYBudget, 0),
    BudgetRemaining = CurrentFYBudget - ((MonthlyInput * 7) + SeasonalAdjustment),
    QuarterlySummary = (MonthlyInput * 3) + (SeasonalAdjustment / 4),
    Total = CurrentFYBudget + SeasonalAdjustment + GoalAdjustment;

-- Trash Enterprise Scenarios Calculations:
-- Scenario 1: New Trash Truck ($350,000, 12-year lifespan, 4.5% interest) = $2,673.61 monthly impact
-- Scenario 2: Recycling Program Expansion ($125,000, 7 years, 4% interest) = $1,697.79 monthly impact  
-- Scenario 3: Transfer Station & Route Optimization ($200,000, 15 years, 3.5% interest) = $1,429.77 monthly impact

-- Calculate Scenario 1 (New Trash Truck Impact)
UPDATE Trash SET 
    Scenario1 = CASE 
        WHEN Section = 'Revenue' THEN MonthlyInput + (2673.61 * (PercentAllocation / 100.0))
        WHEN Section = 'Collections' THEN MonthlyInput + 2673.61 + GoalAdjustment + (350000.0 * 0.10 / 12) -- Include maintenance reserve
        WHEN Section = 'Recycling' THEN MonthlyInput + (2673.61 * 0.1) -- Minimal impact
        WHEN Section = 'Operations' THEN MonthlyInput + (2673.61 * 0.2) -- Operational efficiency
        WHEN Section = 'Equipment' THEN MonthlyInput + 2673.61 + (350000.0 * 0.10 / 12) -- Full impact + maintenance
        ELSE MonthlyInput
    END;

-- Calculate Scenario 2 (Recycling Program Expansion)
UPDATE Trash SET 
    Scenario2 = CASE 
        WHEN Section = 'Revenue' THEN MonthlyInput + (1697.79 * (PercentAllocation / 100.0))
        WHEN Section = 'Collections' THEN MonthlyInput + (1697.79 * 0.4) -- Collection impact
        WHEN Section = 'Recycling' THEN MonthlyInput + 1697.79 + GoalAdjustment -- Full program impact
        WHEN Section = 'Operations' THEN MonthlyInput + (1697.79 * 0.3) -- Program management
        WHEN Section = 'Equipment' THEN MonthlyInput + 1697.79 + (350000.0 * 0.10 * 0.3 / 12) -- Equipment + 30% maintenance
        ELSE MonthlyInput
    END;

-- Calculate Scenario 3 (Transfer Station & Route Optimization)
UPDATE Trash SET 
    Scenario3 = CASE 
        WHEN Section = 'Revenue' THEN MonthlyInput + (1429.77 * (PercentAllocation / 100.0))
        WHEN Section = 'Collections' THEN MonthlyInput + (1429.77 * 0.6) -- Route optimization impact
        WHEN Section = 'Recycling' THEN MonthlyInput + (1429.77 * 0.5) -- Processing efficiency
        WHEN Section = 'Operations' THEN MonthlyInput + (1429.77 * 0.8) -- Full operational impact
        WHEN Section = 'Equipment' THEN MonthlyInput + 1429.77 + (350000.0 * 0.10 * 0.5 / 12) -- Infrastructure + 50% maintenance
        ELSE MonthlyInput
    END;

-- Apply Time-of-Use and Affordability factors to scenarios
UPDATE Trash SET 
    Scenario1 = Scenario1 * TimeOfUseFactor * CustomerAffordabilityIndex,
    Scenario2 = Scenario2 * TimeOfUseFactor * CustomerAffordabilityIndex,
    Scenario3 = Scenario3 * TimeOfUseFactor * CustomerAffordabilityIndex;

-- Calculate Required Rates per customer per month (850 customers total)
UPDATE Trash SET 
    RequiredRate = CASE 
        WHEN Section = 'Revenue' THEN 
            ((SELECT SUM(CurrentFYBudget) FROM Trash WHERE Section != 'Revenue') * 
             (CurrentFYBudget / NULLIF((SELECT SUM(CurrentFYBudget) FROM Trash WHERE Section = 'Revenue'), 0))) / 850.0 / 12.0
        WHEN Section = 'Collections' THEN CurrentFYBudget / 850.0 / 12.0
        WHEN Section = 'Recycling' THEN (CurrentFYBudget + GoalAdjustment) / 850.0 / 12.0
        WHEN Section = 'Operations' THEN CurrentFYBudget / 850.0 / 12.0
        WHEN Section = 'Equipment' THEN (CurrentFYBudget + ReserveTarget) / 850.0 / 12.0
        ELSE 0
    END;

-- Apply final adjustment factors to Required Rates
UPDATE Trash SET 
    RequiredRate = RequiredRate * TimeOfUseFactor * CustomerAffordabilityIndex;

-- Verification Query - Summary Statistics
SELECT 
    Section,
    COUNT(*) as AccountCount,
    SUM(CurrentFYBudget) as TotalBudget,
    SUM(YearToDateSpending) as TotalYTDSpending,
    AVG(PercentOfBudget) as AvgBudgetUtilization,
    SUM(BudgetRemaining) as TotalBudgetRemaining,
    SUM(Scenario1) as TotalScenario1,
    SUM(Scenario2) as TotalScenario2,
    SUM(Scenario3) as TotalScenario3,
    AVG(RequiredRate) as AvgRequiredRate,
    SUM(MonthlyUsage) as TotalMonthlyTonnage
FROM Trash 
GROUP BY Section
ORDER BY Section;

-- Overall Trash Enterprise Summary
SELECT 
    'Trash Enterprise' as Enterprise,
    (SELECT SUM(CurrentFYBudget) FROM Trash WHERE Section = 'Revenue') as TotalRevenue,
    (SELECT SUM(CurrentFYBudget) FROM Trash WHERE Section != 'Revenue') as TotalExpenses,
    (SELECT SUM(CurrentFYBudget) FROM Trash WHERE Section = 'Revenue') - 
    (SELECT SUM(CurrentFYBudget) FROM Trash WHERE Section != 'Revenue') as NetIncome,
    (SELECT AVG(RequiredRate) FROM Trash) as AverageRequiredRate,
    (SELECT SUM(MonthlyUsage) FROM Trash) as TotalMonthlyTonnage,
    (SELECT SUM(CurrentFYBudget + ReserveTarget) FROM Trash WHERE Section = 'Equipment') as EquipmentInvestment,
    (SELECT SUM(CurrentFYBudget) FROM Trash WHERE Section = 'Recycling') as RecyclingInvestment;

PRINT 'Trash Enterprise database deployment completed successfully!';
PRINT 'Total Records Inserted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
PRINT 'Database is now ready for production use.';
