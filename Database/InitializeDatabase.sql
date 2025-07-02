-- Wiley Sanitation District Budget Management Database Initialization
-- Date: July 1, 2025
-- Purpose: Complete database setup for all enterprises

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'WileyBudgetManagement')
BEGIN
    CREATE DATABASE [WileyBudgetManagement];
END
GO

USE [WileyBudgetManagement];
GO

-- Drop tables if they exist (for clean setup)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Summary]') AND type in (N'U'))
DROP TABLE [dbo].[Summary];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Apartments]') AND type in (N'U'))
DROP TABLE [dbo].[Apartments];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Trash]') AND type in (N'U'))
DROP TABLE [dbo].[Trash];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Water]') AND type in (N'U'))
DROP TABLE [dbo].[Water];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SanitationDistrict]') AND type in (N'U'))
DROP TABLE [dbo].[SanitationDistrict];

-- Create SanitationDistrict table (fully implemented)
CREATE TABLE [dbo].[SanitationDistrict] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Account] NVARCHAR(50) NOT NULL UNIQUE,
    [Label] NVARCHAR(100) NOT NULL,
    [Section] NVARCHAR(50),
    [CurrentFYBudget] DECIMAL(18,2) DEFAULT 0,
    [SeasonalAdjustment] DECIMAL(18,2) DEFAULT 0,
    [MonthlyInput] DECIMAL(18,2) DEFAULT 0,
    [SeasonalRevenueFactor] DECIMAL(18,4) DEFAULT 1.0,
    [YearToDateSpending] DECIMAL(18,2) DEFAULT 0,
    [PercentOfBudget] DECIMAL(5,2) DEFAULT 0,
    [BudgetRemaining] DECIMAL(18,2) DEFAULT 0,
    [GoalAdjustment] DECIMAL(18,2) DEFAULT 0,
    [ReserveTarget] DECIMAL(18,2) DEFAULT 0,
    [Scenario1] DECIMAL(18,2) DEFAULT 0,
    [Scenario2] DECIMAL(18,2) DEFAULT 0,
    [Scenario3] DECIMAL(18,2) DEFAULT 0,
    [PercentAllocation] DECIMAL(5,2) DEFAULT 0,
    [RequiredRate] DECIMAL(18,2) DEFAULT 0,
    [MonthlyUsage] DECIMAL(18,2) DEFAULT 0,
    [TimeOfUseFactor] DECIMAL(5,2) DEFAULT 1.0,
    [CustomerAffordabilityIndex] DECIMAL(5,2) DEFAULT 1.0,
    [QuarterlySummary] DECIMAL(18,2) DEFAULT 0,
    [EntryDate] DATETIME DEFAULT GETDATE(),
    [Total] DECIMAL(18,2) DEFAULT 0
);

-- Create Water table
CREATE TABLE [dbo].[Water] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Account] NVARCHAR(50) NOT NULL UNIQUE,
    [Label] NVARCHAR(100) NOT NULL,
    [Section] NVARCHAR(50),
    [CurrentFYBudget] DECIMAL(18,2) DEFAULT 0,
    [SeasonalAdjustment] DECIMAL(18,2) DEFAULT 0,
    [MonthlyInput] DECIMAL(18,2) DEFAULT 0,
    [SeasonalRevenueFactor] DECIMAL(18,4) DEFAULT 1.0,
    [YearToDateSpending] DECIMAL(18,2) DEFAULT 0,
    [PercentOfBudget] DECIMAL(5,2) DEFAULT 0,
    [BudgetRemaining] DECIMAL(18,2) DEFAULT 0,
    [GoalAdjustment] DECIMAL(18,2) DEFAULT 0,
    [ReserveTarget] DECIMAL(18,2) DEFAULT 0,
    [Scenario1] DECIMAL(18,2) DEFAULT 0,
    [Scenario2] DECIMAL(18,2) DEFAULT 0,
    [Scenario3] DECIMAL(18,2) DEFAULT 0,
    [PercentAllocation] DECIMAL(5,2) DEFAULT 0,
    [RequiredRate] DECIMAL(18,2) DEFAULT 0,
    [MonthlyUsage] DECIMAL(18,2) DEFAULT 0,
    [TimeOfUseFactor] DECIMAL(5,2) DEFAULT 1.0,
    [CustomerAffordabilityIndex] DECIMAL(5,2) DEFAULT 1.0,
    [QuarterlySummary] DECIMAL(18,2) DEFAULT 0,
    [EntryDate] DATETIME DEFAULT GETDATE(),
    [Total] DECIMAL(18,2) DEFAULT 0
);

-- Create Trash table
CREATE TABLE [dbo].[Trash] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Account] NVARCHAR(50) NOT NULL UNIQUE,
    [Label] NVARCHAR(100) NOT NULL,
    [Section] NVARCHAR(50),
    [CurrentFYBudget] DECIMAL(18,2) DEFAULT 0,
    [SeasonalAdjustment] DECIMAL(18,2) DEFAULT 0,
    [MonthlyInput] DECIMAL(18,2) DEFAULT 0,
    [SeasonalRevenueFactor] DECIMAL(18,4) DEFAULT 1.0,
    [YearToDateSpending] DECIMAL(18,2) DEFAULT 0,
    [PercentOfBudget] DECIMAL(5,2) DEFAULT 0,
    [BudgetRemaining] DECIMAL(18,2) DEFAULT 0,
    [GoalAdjustment] DECIMAL(18,2) DEFAULT 0,
    [ReserveTarget] DECIMAL(18,2) DEFAULT 0,
    [Scenario1] DECIMAL(18,2) DEFAULT 0,
    [Scenario2] DECIMAL(18,2) DEFAULT 0,
    [Scenario3] DECIMAL(18,2) DEFAULT 0,
    [PercentAllocation] DECIMAL(5,2) DEFAULT 0,
    [RequiredRate] DECIMAL(18,2) DEFAULT 0,
    [MonthlyUsage] DECIMAL(18,2) DEFAULT 0,
    [TimeOfUseFactor] DECIMAL(5,2) DEFAULT 1.0,
    [CustomerAffordabilityIndex] DECIMAL(5,2) DEFAULT 1.0,
    [QuarterlySummary] DECIMAL(18,2) DEFAULT 0,
    [EntryDate] DATETIME DEFAULT GETDATE(),
    [Total] DECIMAL(18,2) DEFAULT 0
);

-- Create Apartments table
CREATE TABLE [dbo].[Apartments] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Account] NVARCHAR(50) NOT NULL UNIQUE,
    [Label] NVARCHAR(100) NOT NULL,
    [Section] NVARCHAR(50),
    [CurrentFYBudget] DECIMAL(18,2) DEFAULT 0,
    [SeasonalAdjustment] DECIMAL(18,2) DEFAULT 0,
    [MonthlyInput] DECIMAL(18,2) DEFAULT 0,
    [SeasonalRevenueFactor] DECIMAL(18,4) DEFAULT 1.0,
    [YearToDateSpending] DECIMAL(18,2) DEFAULT 0,
    [PercentOfBudget] DECIMAL(5,2) DEFAULT 0,
    [BudgetRemaining] DECIMAL(18,2) DEFAULT 0,
    [GoalAdjustment] DECIMAL(18,2) DEFAULT 0,
    [ReserveTarget] DECIMAL(18,2) DEFAULT 0,
    [Scenario1] DECIMAL(18,2) DEFAULT 0,
    [Scenario2] DECIMAL(18,2) DEFAULT 0,
    [Scenario3] DECIMAL(18,2) DEFAULT 0,
    [PercentAllocation] DECIMAL(5,2) DEFAULT 0,
    [RequiredRate] DECIMAL(18,2) DEFAULT 0,
    [MonthlyUsage] DECIMAL(18,2) DEFAULT 0,
    [TimeOfUseFactor] DECIMAL(5,2) DEFAULT 1.0,
    [CustomerAffordabilityIndex] DECIMAL(5,2) DEFAULT 1.0,
    [QuarterlySummary] DECIMAL(18,2) DEFAULT 0,
    [EntryDate] DATETIME DEFAULT GETDATE(),
    [Total] DECIMAL(18,2) DEFAULT 0
);

-- Create Summary table
CREATE TABLE [dbo].[Summary] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Enterprise] NVARCHAR(50) NOT NULL,
    [TotalOperatingIncome] DECIMAL(18,2) DEFAULT 0,
    [TotalOMExpenses] DECIMAL(18,2) DEFAULT 0,
    [TotalAdminExpenses] DECIMAL(18,2) DEFAULT 0,
    [NetSurplusDeficit] DECIMAL(18,2) DEFAULT 0,
    [PercentOfTotalBudget] DECIMAL(5,2) DEFAULT 0,
    [RequiredRate] DECIMAL(18,2) DEFAULT 0,
    [TrendData] NVARCHAR(MAX),
    [EntryDate] DATETIME DEFAULT GETDATE()
);

PRINT 'Database tables created successfully.';
PRINT 'Ready for data population and enterprise implementation.';
