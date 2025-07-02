# Water Input Form - Charts Implementation

## Overview
The WaterInput.cs form has been enhanced with comprehensive charting capabilities using LiveChartsCore for financial data visualization in the Town of Wiley Rate Study application.

## Charts Implemented

### 1. Budget vs Actual Chart (Column Chart)
- **Purpose**: Compare current fiscal year budget against year-to-date spending
- **Data Source**: `CurrentFYBudget` vs `YearToDateSpending` fields
- **Visualization**: Side-by-side column chart
- **Colors**: Steel Blue (Budget), Orange (YTD Spending)

### 2. Scenario Comparison Chart (Column Chart)
- **Purpose**: Compare three infrastructure scenarios for water district planning
- **Data Source**: `Scenario1`, `Scenario2`, `Scenario3` fields
- **Scenarios**:
  - Scenario 1: New Water Treatment Plant ($750,000, 20-year)
  - Scenario 2: Pipeline Replacement Program ($200,000, 10-year)
  - Scenario 3: Water Quality Upgrade ($125,000, 8-year)
- **Colors**: Green, Blue, Purple

### 3. Section Distribution Chart (Pie Chart)
- **Purpose**: Show budget distribution across water district sections
- **Data Source**: Grouped by `Section` field (Revenue, Operating, Infrastructure, Quality)
- **Visualization**: Pie chart with percentage distribution
- **Colors**: Light Green (Revenue), Light Blue (Operating), Orange (Infrastructure), Red (Quality)

### 4. Monthly Trends Chart (Line Chart)
- **Purpose**: Track monthly input trends vs required rates
- **Data Source**: `MonthlyInput` vs `RequiredRate` fields
- **Visualization**: Multi-line chart with points
- **Colors**: Blue (Monthly Input), Red (Required Rate)

### 5. Usage vs Revenue Chart (Scatter Plot)
- **Purpose**: Analyze correlation between water usage and revenue
- **Data Source**: `MonthlyUsage` (X-axis) vs `MonthlyInput` (Y-axis)
- **Filter**: Only Revenue section items with usage > 0
- **Visualization**: Scatter plot with trend analysis

## Features

### Interactive Controls
- **Refresh Charts Button**: Manual chart refresh
- **Tab Navigation**: Switch between different chart views
- **Real-time Updates**: Charts automatically refresh when data is edited
- **Section Filtering**: Filter charts by water district section

### Data Integration
- **Automatic Calculations**: Charts use calculated fields (scenarios, required rates)
- **Live Data Binding**: Charts update when grid data changes
- **Validation Integration**: Chart data reflects validated and calculated values

### Export Capabilities
- **Chart Summary Export**: Export comprehensive chart data and statistics
- **Financial Summary**: Key performance indicators and totals
- **Section Breakdown**: Detailed analysis by section
- **CSV Data Export**: Raw chart data for external analysis

## Technical Implementation

### Chart Library
- **Framework**: LiveChartsCore with SkiaSharp rendering
- **Platform**: Windows Forms (.NET 8)
- **Performance**: Optimized for real-time data updates

### Chart Configuration
- **Colors**: Professional color scheme using SKColors
- **Formatting**: Currency and percentage formatters
- **Responsive**: Charts resize with form
- **Accessibility**: Clear labels and legends

### Data Handling
- **Type Conversion**: Decimal to double conversion for chart compatibility
- **Error Handling**: Graceful handling of invalid data
- **Performance**: Efficient data binding and refresh

## Usage Instructions

1. **Viewing Charts**: Click on chart tabs to view different visualizations
2. **Refreshing Data**: Click "Refresh Charts" button or edit grid data
3. **Filtering**: Use section filter to focus on specific areas
4. **Exporting**: Use export methods for reporting and analysis

## Water-Specific Features

### Seasonal Adjustments
- Charts reflect seasonal revenue factors for water usage patterns
- Summer peak usage variations included in calculations

### Infrastructure Planning
- Three-scenario analysis for major water infrastructure investments
- Financial impact visualization across 8-20 year periods

### Regulatory Compliance
- Quality assurance budget tracking
- EPA compliance cost visualization

### Customer Impact Analysis
- Time-of-use factor integration
- Customer affordability index considerations
- Rate structure impact visualization

## Future Enhancements
- Chart export to image formats
- Advanced trend analysis
- Predictive modeling visualization
- Multi-year comparison charts
- Drill-down capabilities for detailed analysis

## Dependencies
- LiveChartsCore.SkiaSharpView.WinForms (v2.0.0-rc5.4)
- SkiaSharp.Views.WindowsForms (v3.116.1)
- Syncfusion.SfDataGrid.WinForms (v26.1.42)

The chart implementation provides comprehensive financial visualization capabilities specifically tailored for water district rate study analysis and planning.
