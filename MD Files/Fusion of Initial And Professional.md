# Gap Analysis Report: Wiley Budget Management Software vs. Professional Rate Study Methods

## Overview
The Town of Wiley Budget Management Software aims to manage budgets for the Sanitation District, Water, Trash, and Apartments, ensuring sustainable rates and informed financial decisions. The current methodology, detailed in "notes.txt" and "Rate Study Methodology.txt," includes basic input forms, validation systems, and database integration with SQL Server Express. However, critical components are missing, and the methodology lacks alignment with professional cost-of-service ratemaking standards outlined in the "Cost-of-Service Rates Manual" by the Federal Energy Regulatory Commission ([FERC Manual](https://www.ferc.gov)). This report compares the current methodology with professional practices, identifies gaps, and provides recommendations to align with industry standards.

## Current Methodology
### Implemented Features
- **Input Forms**: Basic forms for Water, Trash, and Apartments with validation systems.
- **Validation System**: Comprehensive validation with a ValidationHelper class, Save & Validate buttons, and error/warning messages.
- **Database Integration**: Completed with SQL Server Express, including mock data fallback for errors.
- **Scenario Calculations**: Basic calculations for financial scenarios (e.g., new trash truck, reserve fund).

### Missing Features
The "notes.txt" file highlights critical gaps:
- **Priority 1 (Critical)**:
  - **Sanitation District Form**: Completely missing, despite being the primary form for tracking revenues (e.g., property taxes, sewage sales) and expenses (e.g., permits, salaries).
  - **Reports System**: No implementation, including CSV exports, Sanitation Reports, Scenario Reports, Summary Reports, and Quarterly Summary Reports for regulatory compliance.
  - **Summary Page**: Incomplete, lacking aggregation of totals across enterprises and cross-enterprise rate impact analysis.
- **Priority 2 (High)**:
  - Visualization tools (e.g., trend graphs, Crystal Ball Dashboard, heat maps, bar graphs).
  - Rate calculation engine for actual rates, required rates per customer/unit, scenario-based impacts, and seasonal adjustments.
  - UI controls for Goal Adjustment, Reserve Target, Seasonal Adjustment, and Customer Affordability Index.
- **Priority 3 (Medium)**:
  - Advanced features like Time-of-Use (TOU) rates, five-year rate forecasting, administrative cost allocation, and enhanced seasonal multipliers.
  - Business logic enhancements (e.g., monthly budget targets, variance analysis, cash flow projections, reserve fund automation).
- **Current Status**: Approximately 60% of intended functionality is missing, with the Sanitation District Form being the most critical gap.

### Intended Methodology
The "Rate Study Methodology.txt" outlines the intended approach:
- **Program Intention**: Audit expenses against financial goals (e.g., surplus, $350,000 trash truck) and support trend analysis for annual rate adjustments.
- **Software Structure**: Five tables (Sanitation District, Water, Trash, Apartments, Summary) with fields for budget tracking, scenario analysis, and seasonal adjustments.
- **Scenario Analysis**: Three scenarios:
  - **Scenario 1**: New trash truck ($350,000, 2.67% rate increase).
  - **Scenario 2**: Reserve fund ($50,000 over 5 years, 0.83% rate increase).
  - **Scenario 3**: Grant repayment ($100,000, 1.83% rate increase).
- **Visualization and Reporting**: Tools like trend graphs, dashboards, and reports for transparency.
- **Inspirations**: Incorporates Colorado municipal practices (e.g., TOU rates, five-year planning) and EPA guidance (e.g., usage-based rates, affordability).

## Professional Methods in Utility Rate Studies
The "Cost-of-Service Rates Manual" ([FERC Manual](https://www.ferc.gov)) outlines a five-step cost-of-service ratemaking process for determining "just and reasonable" rates for interstate pipelines. These steps are standard in utility rate studies and provide a benchmark for comparison.

### Step 1: Computing the Cost-of-Service
- **Purpose**: Determines the revenue requirement to recover costs and earn a reasonable return.
- **Components**:
  - **Rate Base**: Gross plant, accumulated depreciation, Accumulated Deferred Income Taxes (ADIT), and working capital (e.g., cash, materials, prepayments).
  - **Return**: Calculated as Rate Base × Overall Rate of Return (cost of debt + return on equity).
  - **Expenses**: Operation and Maintenance (O&M), Administrative and General (A&G), depreciation, federal/state income taxes, non-income taxes.
  - **Revenue Credits**: Deducted from costs (e.g., penalties, leased facilities).
  - **Test Period**: 12-month base period + 9-month adjustment for known changes.
- **Formula**:
  ```
  Rate Base × Overall Rate of Return = Return
  + O&M + A&G + Depreciation + Non-Income Taxes + Income Taxes - Revenue Credits
  = Total Cost-of-Service
  ```

### Step 2: Functionalizing the Cost-of-Service
- **Purpose**: Assigns costs to specific functions (e.g., transmission, storage).
- **Method**: Uses direct assignment for O&M and the Kansas-Nebraska (K-N) Method for A&G allocation based on labor and plant ratios.

### Step 3: Cost Classification
- **Purpose**: Classifies costs as fixed (e.g., depreciation, return) or variable (e.g., materials), then as demand (reservation) or commodity (usage).
- **Methods**: Includes Volumetric, Fixed-Variable, Seaboard, United, Modified Fixed-Variable (MFV), and Straight Fixed-Variable (SFV).

### Step 4: Cost Allocation
- **Purpose**: Distributes costs among services (e.g., firm vs. interruptible), zones, and jurisdictional vs. non-jurisdictional services.
- **Methods**: Uses allocation factors like dekatherms (Dth) or Dth-miles.

### Step 5: Rate Design
- **Purpose**: Translates allocated costs into rates for customers.
- **Types**:
  - **Firm Service Rates**: Two-part rates (reservation + usage).
  - **Interruptible Service Rates**: Volumetric, often derived from firm rates.
- **Considerations**: Load factors, small customer rates, discounting adjustments, and distance-based rates (e.g., zoned rates).

### Additional Considerations
- **Load Factor**: Higher load factors lower unit rates.
- **Discounting**: Methods like Revenue Crediting or Iterative Methods.
- **Small Customer Rates**: Often subsidized volumetric rates.
- **Distance-Based Rates**: Reflect cost variations with distance.

## Gap Analysis
The following table summarizes the gaps between the Wiley methodology and professional standards:

| **Aspect** | **Wiley Methodology** | **Professional Standard (FERC)** | **Gap** |
|------------|-----------------------|----------------------------------|---------|
| **Core Forms** | Sanitation District Form missing | Requires detailed tracking of revenues and expenses | Critical gap; primary form absent |
| **Reporting** | No Reports System implemented | Detailed reports for cost-of-service, scenarios, and compliance | Missing essential reports for transparency and regulation |
| **Visualization** | No visualization tools | Trend graphs, bar graphs, heat maps standard | Lack of tools for data analysis and decision-making |
| **Rate Calculation** | No rate calculation engine | Detailed rate design for firm/interruptible services | Missing functionality for accurate rate determination |
| **Advanced Features** | TOU rates, five-year forecasting, affordability indices not implemented | Includes advanced features like TOU and affordability | Unimplemented features critical for modern rate studies |
| **Cost Classification** | Unspecified methods | Uses SFV, MFV, Seaboard, etc. | Lack of standardized classification methods |
| **Cost Allocation** | Unspecified methods | Uses K-N Method, Dth, Dth-miles | Unclear allocation processes |
| **Test Period** | Not mentioned | 12-month base + 9-month adjustment | Missing standard practice for rate adjustments |
| **Distance-Based Rates** | Not addressed | Zoned or mileage-based rates | Potential gap if services vary by distance |

### Detailed Gaps
1. **Missing Core Forms**:
   - The Sanitation District Form, critical for tracking revenues (e.g., sewage sales, taxes) and expenses (e.g., permits, salaries), is entirely absent. This is a foundational gap, as professional rate studies require detailed cost tracking.

2. **Incomplete Reporting**:
   - The Reports System is missing, including CSV exports, Sanitation Reports, Scenario Reports, Summary Reports, and Quarterly Summary Reports. Professional standards emphasize comprehensive reporting for regulatory compliance and decision support.

3. **Lack of Visualization Tools**:
   - No visualization tools (e.g., trend graphs, dashboards, heat maps) are implemented. These are standard in professional rate studies to identify trends and variances, as seen in the FERC manual’s use of Syncfusion ChartControl.

4. **Absent Rate Calculation Engine**:
   - The rate calculation engine, needed for actual rate determination, scenario-based impacts, and seasonal adjustments, is missing. Professional methods include detailed rate design for firm and interruptible services, considering load factors and customer types.

5. **Missing Advanced Features**:
   - Features like TOU rates, five-year rate forecasting, and Customer Affordability Index are not implemented. While inspired by Colorado practices and EPA guidance, these are critical for modern, sustainable rate studies.

6. **Unspecified Cost Classification and Allocation**:
   - The methodology does not specify how costs are classified (fixed vs. variable, demand vs. commodity) or allocated (e.g., between services or zones). Professional methods use standardized approaches like SFV or K-N, which are absent here.

7. **No Test Period**:
   - The methodology lacks a test period (12-month base + 9-month adjustment) for adjusting rates based on known changes, a standard practice in professional rate studies.

8. **No Distance-Based Rates**:
   - The methodology does not address distance-based rates, which are relevant for utilities with varying service distances, as outlined in the FERC manual.

## Recommendations
To align with professional standards:
1. **Implement the Sanitation District Form**:
   - Develop the form to track all revenue and expense accounts, as outlined in "Rate Study Methodology.txt."
2. **Develop the Reports System**:
   - Include CSV exports, Sanitation Reports, Scenario Reports, Summary Reports, and Quarterly Summary Reports for regulatory compliance.
3. **Add Visualization Tools**:
   - Implement trend graphs, dashboards, heat maps, and bar graphs using Syncfusion controls for data analysis.
4. **Create a Rate Calculation Engine**:
   - Develop functionality to compute rates based on expenses, revenue, scenarios, and seasonal adjustments, aligning with SFV or MFV methods.
5. **Incorporate Advanced Features**:
   - Add TOU rates, five-year forecasting, and affordability indices to enhance sustainability and compliance.
6. **Define Cost Classification and Allocation**:
   - Adopt standardized methods like SFV for classification and K-N for allocation to ensure accurate cost distribution.
7. **Adopt a Test Period**:
   - Use a 12-month base period with a 9-month adjustment for known changes to reflect current conditions.
8. **Consider Distance-Based Rates**:
   - If applicable, incorporate zoned or mileage-based rates for services with varying distances.

## Citation
The "Cost-of-Service Rates Manual" ([FERC Manual](https://www.ferc.gov)) was used as a key resource to understand professional standards in utility rate studies. It provides a comprehensive guide to cost-of-service ratemaking, detailing steps for computing costs, functionalizing, classifying, allocating, and designing rates, which informed the identification of gaps and recommendations.