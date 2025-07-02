using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WileyBudgetManagement.Models;

namespace WileyBudgetManagement.Database
{
    public interface ISanitationRepository
    {
        Task<BindingList<SanitationDistrict>> GetSanitationDataAsync();
        Task<BindingList<SanitationDistrict>> GetSanitationDistrictDataAsync();
        Task<BindingList<SanitationDistrict>> GetWaterDataAsync();
        Task<BindingList<SanitationDistrict>> GetTrashDataAsync();
        Task<BindingList<SanitationDistrict>> GetApartmentDataAsync();
        Task<List<SanitationDistrict>> GetAllDataAsync();
        Task<SanitationDistrict?> GetByAccountAsync(string account, string category);
        Task<bool> SaveAsync(SanitationDistrict district, string category);
        Task<bool> UpdateAsync(SanitationDistrict district, string category);
        Task<bool> DeleteAsync(string account, string category);
        Task<bool> SaveAllAsync(BindingList<SanitationDistrict> districts, string category);
    }

    public class SanitationRepository : ISanitationRepository
    {
        private readonly DatabaseManager _databaseManager;

        public SanitationRepository(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
        }

        public async Task<BindingList<SanitationDistrict>> GetSanitationDataAsync()
        {
            return await GetSanitationDistrictDataAsync();
        }

        public async Task<BindingList<SanitationDistrict>> GetSanitationDistrictDataAsync()
        {
            if (_databaseManager.IsMockDataEnabled)
            {
                return GetMockSanitationDistrictData();
            }

            return await GetDataFromTableAsync("SanitationDistrict");
        }

        public async Task<BindingList<SanitationDistrict>> GetWaterDataAsync()
        {
            if (_databaseManager.IsMockDataEnabled)
            {
                return GetMockWaterData();
            }

            return await GetDataFromTableAsync("Water");
        }

        public async Task<BindingList<SanitationDistrict>> GetTrashDataAsync()
        {
            if (_databaseManager.IsMockDataEnabled)
            {
                return GetMockTrashData();
            }

            return await GetDataFromTableAsync("Trash");
        }

        public async Task<BindingList<SanitationDistrict>> GetApartmentDataAsync()
        {
            if (_databaseManager.IsMockDataEnabled)
            {
                return GetMockApartmentData();
            }

            return await GetDataFromTableAsync("Apartments");
        }

        public async Task<List<SanitationDistrict>> GetAllDataAsync()
        {
            var allData = new List<SanitationDistrict>();

            var waterData = await GetWaterDataAsync();
            var trashData = await GetTrashDataAsync();
            var apartmentData = await GetApartmentDataAsync();

            allData.AddRange(waterData);
            allData.AddRange(trashData);
            allData.AddRange(apartmentData);

            return allData;
        }

        private async Task<BindingList<SanitationDistrict>> GetDataFromTableAsync(string tableName)
        {
            var data = new BindingList<SanitationDistrict>();

            try
            {
                using var connection = await _databaseManager.GetConnectionAsync();
                string query = $@"
                    SELECT Account, Label, Section, CurrentFYBudget, SeasonalAdjustment, MonthlyInput, 
                           SeasonalRevenueFactor, YearToDateSpending, PercentOfBudget, BudgetRemaining,
                           GoalAdjustment, ReserveTarget, Scenario1, Scenario2, Scenario3, PercentAllocation,
                           RequiredRate, MonthlyUsage, TimeOfUseFactor, CustomerAffordabilityIndex,
                           QuarterlySummary, EntryDate, Total
                    FROM {tableName}
                    ORDER BY Account";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var district = new SanitationDistrict
                    {
                        Account = reader["Account"]?.ToString() ?? string.Empty,
                        Label = reader["Label"]?.ToString() ?? string.Empty,
                        Section = reader["Section"]?.ToString() ?? string.Empty,
                        CurrentFYBudget = reader["CurrentFYBudget"] != DBNull.Value ? Convert.ToDecimal(reader["CurrentFYBudget"]) : 0,
                        SeasonalAdjustment = reader["SeasonalAdjustment"] != DBNull.Value ? Convert.ToDecimal(reader["SeasonalAdjustment"]) : 0,
                        MonthlyInput = reader["MonthlyInput"] != DBNull.Value ? Convert.ToDecimal(reader["MonthlyInput"]) : 0,
                        SeasonalRevenueFactor = reader["SeasonalRevenueFactor"] != DBNull.Value ? Convert.ToDecimal(reader["SeasonalRevenueFactor"]) : 0,
                        YearToDateSpending = reader["YearToDateSpending"] != DBNull.Value ? Convert.ToDecimal(reader["YearToDateSpending"]) : 0,
                        PercentOfBudget = reader["PercentOfBudget"] != DBNull.Value ? Convert.ToDecimal(reader["PercentOfBudget"]) : 0,
                        BudgetRemaining = reader["BudgetRemaining"] != DBNull.Value ? Convert.ToDecimal(reader["BudgetRemaining"]) : 0,
                        GoalAdjustment = reader["GoalAdjustment"] != DBNull.Value ? Convert.ToDecimal(reader["GoalAdjustment"]) : 0,
                        ReserveTarget = reader["ReserveTarget"] != DBNull.Value ? Convert.ToDecimal(reader["ReserveTarget"]) : 0,
                        Scenario1 = reader["Scenario1"] != DBNull.Value ? Convert.ToDecimal(reader["Scenario1"]) : 0,
                        Scenario2 = reader["Scenario2"] != DBNull.Value ? Convert.ToDecimal(reader["Scenario2"]) : 0,
                        Scenario3 = reader["Scenario3"] != DBNull.Value ? Convert.ToDecimal(reader["Scenario3"]) : 0,
                        PercentAllocation = reader["PercentAllocation"] != DBNull.Value ? Convert.ToDecimal(reader["PercentAllocation"]) : 0,
                        RequiredRate = reader["RequiredRate"] != DBNull.Value ? Convert.ToDecimal(reader["RequiredRate"]) : 0,
                        MonthlyUsage = reader["MonthlyUsage"] != DBNull.Value ? Convert.ToDecimal(reader["MonthlyUsage"]) : 0,
                        TimeOfUseFactor = reader["TimeOfUseFactor"] != DBNull.Value ? Convert.ToDecimal(reader["TimeOfUseFactor"]) : 0,
                        CustomerAffordabilityIndex = reader["CustomerAffordabilityIndex"] != DBNull.Value ? Convert.ToDecimal(reader["CustomerAffordabilityIndex"]) : 0,
                        QuarterlySummary = reader["QuarterlySummary"] != DBNull.Value ? Convert.ToDecimal(reader["QuarterlySummary"]) : 0,
                        EntryDate = reader["EntryDate"] != DBNull.Value ? Convert.ToDateTime(reader["EntryDate"]) : DateTime.Now,
                        Total = reader["Total"] != DBNull.Value ? Convert.ToDecimal(reader["Total"]) : 0
                    };

                    data.Add(district);
                }
            }
            catch (Exception ex)
            {
                // Log error and fall back to mock data
                System.Diagnostics.Debug.WriteLine($"Database error, falling back to mock data: {ex.Message}");
                return GetMockDataByCategory(tableName);
            }

            return data;
        }

        public async Task<SanitationDistrict?> GetByAccountAsync(string account, string category)
        {
            if (_databaseManager.IsMockDataEnabled)
            {
                var mockData = GetMockDataByCategory(category);
                return mockData.FirstOrDefault(d => d.Account == account);
            }

            try
            {
                using var connection = await _databaseManager.GetConnectionAsync();
                string query = $@"
                    SELECT Account, Label, Section, CurrentFYBudget, SeasonalAdjustment, MonthlyInput, 
                           SeasonalRevenueFactor, YearToDateSpending, PercentOfBudget, BudgetRemaining,
                           GoalAdjustment, ReserveTarget, Scenario1, Scenario2, Scenario3, PercentAllocation,
                           RequiredRate, MonthlyUsage, TimeOfUseFactor, CustomerAffordabilityIndex,
                           QuarterlySummary, EntryDate, Total
                    FROM {GetTableName(category)}
                    WHERE Account = @Account";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Account", account);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return MapReaderToDistrict(reader);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
            }

            return null;
        }

        public async Task<bool> SaveAsync(SanitationDistrict district, string category)
        {
            if (_databaseManager.IsMockDataEnabled)
            {
                return true; // Mock save always succeeds
            }

            try
            {
                using var connection = await _databaseManager.GetConnectionAsync();
                string tableName = GetTableName(category);

                string query = $@"
                    INSERT INTO {tableName} 
                    (Account, Label, Section, CurrentFYBudget, SeasonalAdjustment, MonthlyInput, 
                     SeasonalRevenueFactor, YearToDateSpending, PercentOfBudget, BudgetRemaining,
                     GoalAdjustment, ReserveTarget, Scenario1, Scenario2, Scenario3, PercentAllocation,
                     RequiredRate, MonthlyUsage, TimeOfUseFactor, CustomerAffordabilityIndex,
                     QuarterlySummary, Total)
                    VALUES 
                    (@Account, @Label, @Section, @CurrentFYBudget, @SeasonalAdjustment, @MonthlyInput,
                     @SeasonalRevenueFactor, @YearToDateSpending, @PercentOfBudget, @BudgetRemaining,
                     @GoalAdjustment, @ReserveTarget, @Scenario1, @Scenario2, @Scenario3, @PercentAllocation,
                     @RequiredRate, @MonthlyUsage, @TimeOfUseFactor, @CustomerAffordabilityIndex,
                     @QuarterlySummary, @Total)";

                using var command = new SqlCommand(query, connection);
                AddDistrictParameters(command, district);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Save error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(SanitationDistrict district, string category)
        {
            if (_databaseManager.IsMockDataEnabled)
            {
                return true; // Mock update always succeeds
            }

            try
            {
                using var connection = await _databaseManager.GetConnectionAsync();
                string tableName = GetTableName(category);

                string query = $@"
                    UPDATE {tableName} SET 
                        Label = @Label, Section = @Section, CurrentFYBudget = @CurrentFYBudget,
                        SeasonalAdjustment = @SeasonalAdjustment, MonthlyInput = @MonthlyInput,
                        SeasonalRevenueFactor = @SeasonalRevenueFactor, YearToDateSpending = @YearToDateSpending,
                        PercentOfBudget = @PercentOfBudget, BudgetRemaining = @BudgetRemaining,
                        GoalAdjustment = @GoalAdjustment, ReserveTarget = @ReserveTarget,
                        Scenario1 = @Scenario1, Scenario2 = @Scenario2, Scenario3 = @Scenario3,
                        PercentAllocation = @PercentAllocation, RequiredRate = @RequiredRate,
                        MonthlyUsage = @MonthlyUsage, TimeOfUseFactor = @TimeOfUseFactor,
                        CustomerAffordabilityIndex = @CustomerAffordabilityIndex,
                        QuarterlySummary = @QuarterlySummary, Total = @Total
                    WHERE Account = @Account";

                using var command = new SqlCommand(query, connection);
                AddDistrictParameters(command, district);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string account, string category)
        {
            if (_databaseManager.IsMockDataEnabled)
            {
                return true; // Mock delete always succeeds
            }

            try
            {
                using var connection = await _databaseManager.GetConnectionAsync();
                string tableName = GetTableName(category);

                string query = $"DELETE FROM {tableName} WHERE Account = @Account";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Account", account);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Delete error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SaveAllAsync(BindingList<SanitationDistrict> districts, string category)
        {
            if (_databaseManager.IsMockDataEnabled)
            {
                return true; // Mock save always succeeds
            }

            try
            {
                using var connection = await _databaseManager.GetConnectionAsync();
                using var transaction = connection.BeginTransaction();

                try
                {
                    foreach (var district in districts)
                    {
                        // Try update first, then insert if doesn't exist
                        var existing = await GetByAccountAsync(district.Account, category);
                        if (existing != null)
                        {
                            await UpdateAsync(district, category);
                        }
                        else
                        {
                            await SaveAsync(district, category);
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveAll error: {ex.Message}");
                return false;
            }
        }

        private string GetTableName(string category)
        {
            return category switch
            {
                "SanitationDistrict" => "SanitationDistrict",
                "Water" => "Water",
                "Trash" => "Trash",
                "Apartments" => "Apartments",
                _ => throw new ArgumentException($"Unknown category: {category}")
            };
        }

        private void AddDistrictParameters(SqlCommand command, SanitationDistrict district)
        {
            command.Parameters.AddWithValue("@Account", district.Account);
            command.Parameters.AddWithValue("@Label", district.Label);
            command.Parameters.AddWithValue("@Section", district.Section ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CurrentFYBudget", district.CurrentFYBudget);
            command.Parameters.AddWithValue("@SeasonalAdjustment", district.SeasonalAdjustment);
            command.Parameters.AddWithValue("@MonthlyInput", district.MonthlyInput);
            command.Parameters.AddWithValue("@SeasonalRevenueFactor", district.SeasonalRevenueFactor);
            command.Parameters.AddWithValue("@YearToDateSpending", district.YearToDateSpending);
            command.Parameters.AddWithValue("@PercentOfBudget", district.PercentOfBudget);
            command.Parameters.AddWithValue("@BudgetRemaining", district.BudgetRemaining);
            command.Parameters.AddWithValue("@GoalAdjustment", district.GoalAdjustment);
            command.Parameters.AddWithValue("@ReserveTarget", district.ReserveTarget);
            command.Parameters.AddWithValue("@Scenario1", district.Scenario1);
            command.Parameters.AddWithValue("@Scenario2", district.Scenario2);
            command.Parameters.AddWithValue("@Scenario3", district.Scenario3);
            command.Parameters.AddWithValue("@PercentAllocation", district.PercentAllocation);
            command.Parameters.AddWithValue("@RequiredRate", district.RequiredRate);
            command.Parameters.AddWithValue("@MonthlyUsage", district.MonthlyUsage);
            command.Parameters.AddWithValue("@TimeOfUseFactor", district.TimeOfUseFactor);
            command.Parameters.AddWithValue("@CustomerAffordabilityIndex", district.CustomerAffordabilityIndex);
            command.Parameters.AddWithValue("@QuarterlySummary", district.QuarterlySummary);
            command.Parameters.AddWithValue("@Total", district.Total);
        }

        private SanitationDistrict MapReaderToDistrict(SqlDataReader reader)
        {
            return new SanitationDistrict
            {
                Account = reader["Account"]?.ToString() ?? string.Empty,
                Label = reader["Label"]?.ToString() ?? string.Empty,
                Section = reader["Section"]?.ToString() ?? string.Empty,
                CurrentFYBudget = reader["CurrentFYBudget"] != DBNull.Value ? Convert.ToDecimal(reader["CurrentFYBudget"]) : 0,
                SeasonalAdjustment = reader["SeasonalAdjustment"] != DBNull.Value ? Convert.ToDecimal(reader["SeasonalAdjustment"]) : 0,
                MonthlyInput = reader["MonthlyInput"] != DBNull.Value ? Convert.ToDecimal(reader["MonthlyInput"]) : 0,
                SeasonalRevenueFactor = reader["SeasonalRevenueFactor"] != DBNull.Value ? Convert.ToDecimal(reader["SeasonalRevenueFactor"]) : 0,
                YearToDateSpending = reader["YearToDateSpending"] != DBNull.Value ? Convert.ToDecimal(reader["YearToDateSpending"]) : 0,
                PercentOfBudget = reader["PercentOfBudget"] != DBNull.Value ? Convert.ToDecimal(reader["PercentOfBudget"]) : 0,
                BudgetRemaining = reader["BudgetRemaining"] != DBNull.Value ? Convert.ToDecimal(reader["BudgetRemaining"]) : 0,
                GoalAdjustment = reader["GoalAdjustment"] != DBNull.Value ? Convert.ToDecimal(reader["GoalAdjustment"]) : 0,
                ReserveTarget = reader["ReserveTarget"] != DBNull.Value ? Convert.ToDecimal(reader["ReserveTarget"]) : 0,
                Scenario1 = reader["Scenario1"] != DBNull.Value ? Convert.ToDecimal(reader["Scenario1"]) : 0,
                Scenario2 = reader["Scenario2"] != DBNull.Value ? Convert.ToDecimal(reader["Scenario2"]) : 0,
                Scenario3 = reader["Scenario3"] != DBNull.Value ? Convert.ToDecimal(reader["Scenario3"]) : 0,
                PercentAllocation = reader["PercentAllocation"] != DBNull.Value ? Convert.ToDecimal(reader["PercentAllocation"]) : 0,
                RequiredRate = reader["RequiredRate"] != DBNull.Value ? Convert.ToDecimal(reader["RequiredRate"]) : 0,
                MonthlyUsage = reader["MonthlyUsage"] != DBNull.Value ? Convert.ToDecimal(reader["MonthlyUsage"]) : 0,
                TimeOfUseFactor = reader["TimeOfUseFactor"] != DBNull.Value ? Convert.ToDecimal(reader["TimeOfUseFactor"]) : 0,
                CustomerAffordabilityIndex = reader["CustomerAffordabilityIndex"] != DBNull.Value ? Convert.ToDecimal(reader["CustomerAffordabilityIndex"]) : 0,
                QuarterlySummary = reader["QuarterlySummary"] != DBNull.Value ? Convert.ToDecimal(reader["QuarterlySummary"]) : 0,
                EntryDate = reader["EntryDate"] != DBNull.Value ? Convert.ToDateTime(reader["EntryDate"]) : DateTime.Now,
                Total = reader["Total"] != DBNull.Value ? Convert.ToDecimal(reader["Total"]) : 0
            };
        }

        // Mock data methods for fallback
        private BindingList<SanitationDistrict> GetMockDataByCategory(string category)
        {
            return category switch
            {
                "SanitationDistrict" => GetMockSanitationDistrictData(),
                "Water" => GetMockWaterData(),
                "Trash" => GetMockTrashData(),
                "Apartments" => GetMockApartmentData(),
                _ => new BindingList<SanitationDistrict>()
            };
        }

        private BindingList<SanitationDistrict> GetMockSanitationDistrictData()
        {
            return new BindingList<SanitationDistrict>
            {
                // Revenue Items
                new SanitationDistrict { Account = "311.00", Label = "Specific Ownership Taxes", Section = "Revenue", CurrentFYBudget = 15500.00m, MonthlyInput = 1291.67m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, YearToDateSpending = 7750.00m, PercentOfBudget = 0.50m, BudgetRemaining = 7750.00m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m },
                new SanitationDistrict { Account = "301.00", Label = "Sewage Sales", Section = "Revenue", CurrentFYBudget = 100000.00m, MonthlyInput = 8333.33m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.2m, YearToDateSpending = 50000.00m, PercentOfBudget = 0.50m, BudgetRemaining = 50000.00m, TimeOfUseFactor = 1.1m, CustomerAffordabilityIndex = 0.9m },
                new SanitationDistrict { Account = "310.10", Label = "Delinquent Taxes", Section = "Revenue", CurrentFYBudget = 2500.00m, MonthlyInput = 208.33m, SeasonalAdjustment = 0, SeasonalRevenueFactor = 1.0m, YearToDateSpending = 1250.00m, PercentOfBudget = 0.50m, BudgetRemaining = 1250.00m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m },
                
                // Operating Expenses
                new SanitationDistrict { Account = "401.00", Label = "Permits and Assessments", Section = "Operating", CurrentFYBudget = 976.00m, MonthlyInput = 81.33m, SeasonalAdjustment = 0, YearToDateSpending = 488.00m, PercentOfBudget = 0.50m, BudgetRemaining = 488.00m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m },
                new SanitationDistrict { Account = "418.00", Label = "Lift Station Utilities", Section = "Operating", CurrentFYBudget = 15000.00m, MonthlyInput = 1250.00m, SeasonalAdjustment = 500, YearToDateSpending = 7500.00m, PercentOfBudget = 0.50m, BudgetRemaining = 7500.00m, TimeOfUseFactor = 1.2m, CustomerAffordabilityIndex = 1.0m },
                new SanitationDistrict { Account = "432.53", Label = "Sewer Cleaning", Section = "Operating", CurrentFYBudget = 7600.00m, MonthlyInput = 633.33m, SeasonalAdjustment = 1500, YearToDateSpending = 3800.00m, PercentOfBudget = 0.50m, BudgetRemaining = 3800.00m, TimeOfUseFactor = 1.5m, CustomerAffordabilityIndex = 1.0m },
                
                // Admin Expenses
                new SanitationDistrict { Account = "460.00", Label = "Supt Salaries", Section = "Admin", CurrentFYBudget = 26000.00m, MonthlyInput = 2166.67m, SeasonalAdjustment = 0, YearToDateSpending = 13000.00m, PercentOfBudget = 0.50m, BudgetRemaining = 13000.00m, PercentAllocation = 0.40m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m },
                new SanitationDistrict { Account = "460.10", Label = "Clerk Salaries", Section = "Admin", CurrentFYBudget = 26000.00m, MonthlyInput = 2166.67m, SeasonalAdjustment = 0, YearToDateSpending = 13000.00m, PercentOfBudget = 0.50m, BudgetRemaining = 13000.00m, PercentAllocation = 0.40m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m },
                new SanitationDistrict { Account = "491.11", Label = "Employee Benefits", Section = "Admin", CurrentFYBudget = 16000.00m, MonthlyInput = 1333.33m, SeasonalAdjustment = 0, YearToDateSpending = 8000.00m, PercentOfBudget = 0.50m, BudgetRemaining = 8000.00m, PercentAllocation = 0.40m, TimeOfUseFactor = 1.0m, CustomerAffordabilityIndex = 1.0m }
            };
        }

        private BindingList<SanitationDistrict> GetMockWaterData()
        {
            return new BindingList<SanitationDistrict>
            {
                new SanitationDistrict { Account = "W001", Label = "Water Treatment Plant Operations", Section = "Operations", CurrentFYBudget = 250000, MonthlyInput = 20833.33m, YearToDateSpending = 125000, PercentOfBudget = 0.50m, BudgetRemaining = 125000, MonthlyUsage = 1500000, RequiredRate = 0.167m },
                new SanitationDistrict { Account = "W002", Label = "Water Distribution Maintenance", Section = "Maintenance", CurrentFYBudget = 180000, MonthlyInput = 15000, YearToDateSpending = 90000, PercentOfBudget = 0.50m, BudgetRemaining = 90000, MonthlyUsage = 1200000, RequiredRate = 0.125m },
                new SanitationDistrict { Account = "W003", Label = "Water Quality Testing", Section = "Quality", CurrentFYBudget = 45000, MonthlyInput = 3750, YearToDateSpending = 22500, PercentOfBudget = 0.50m, BudgetRemaining = 22500, MonthlyUsage = 0, RequiredRate = 0.0375m }
            };
        }

        private BindingList<SanitationDistrict> GetMockTrashData()
        {
            return new BindingList<SanitationDistrict>
            {
                new SanitationDistrict { Account = "T001", Label = "Residential Trash Collection", Section = "Collections", CurrentFYBudget = 320000, MonthlyInput = 26666.67m, YearToDateSpending = 160000, PercentOfBudget = 0.50m, BudgetRemaining = 160000, SeasonalAdjustment = 5000, RequiredRate = 22.50m },
                new SanitationDistrict { Account = "T002", Label = "Commercial Trash Collection", Section = "Collections", CurrentFYBudget = 180000, MonthlyInput = 15000, YearToDateSpending = 90000, PercentOfBudget = 0.50m, BudgetRemaining = 90000, SeasonalAdjustment = 2500, RequiredRate = 45.00m },
                new SanitationDistrict { Account = "T003", Label = "Recycling Program", Section = "Recycling", CurrentFYBudget = 95000, MonthlyInput = 7916.67m, YearToDateSpending = 47500, PercentOfBudget = 0.50m, BudgetRemaining = 47500, SeasonalAdjustment = 1500, RequiredRate = 8.75m },
                new SanitationDistrict { Account = "T004", Label = "Landfill Operations", Section = "Operations", CurrentFYBudget = 125000, MonthlyInput = 10416.67m, YearToDateSpending = 62500, PercentOfBudget = 0.50m, BudgetRemaining = 62500, SeasonalAdjustment = 3000, RequiredRate = 15.25m }
            };
        }

        private BindingList<SanitationDistrict> GetMockApartmentData()
        {
            return new BindingList<SanitationDistrict>
            {
                new SanitationDistrict { Account = "APT001", Label = "Meadowbrook Apartments (24 units)", Section = "Zone A", CurrentFYBudget = 43200, MonthlyInput = 3600, YearToDateSpending = 21600, PercentOfBudget = 0.50m, BudgetRemaining = 21600, MonthlyUsage = 24, RequiredRate = 150.00m, CustomerAffordabilityIndex = 0.85m },
                new SanitationDistrict { Account = "APT002", Label = "Sunset Manor (36 units)", Section = "Zone B", CurrentFYBudget = 64800, MonthlyInput = 5400, YearToDateSpending = 32400, PercentOfBudget = 0.50m, BudgetRemaining = 32400, MonthlyUsage = 36, RequiredRate = 150.00m, CustomerAffordabilityIndex = 0.78m },
                new SanitationDistrict { Account = "APT003", Label = "Riverside Condos (18 units)", Section = "Zone A", CurrentFYBudget = 32400, MonthlyInput = 2700, YearToDateSpending = 16200, PercentOfBudget = 0.50m, BudgetRemaining = 16200, MonthlyUsage = 18, RequiredRate = 150.00m, CustomerAffordabilityIndex = 0.92m },
                new SanitationDistrict { Account = "APT004", Label = "Oak Street Townhomes (12 units)", Section = "Zone C", CurrentFYBudget = 21600, MonthlyInput = 1800, YearToDateSpending = 10800, PercentOfBudget = 0.50m, BudgetRemaining = 10800, MonthlyUsage = 12, RequiredRate = 150.00m, CustomerAffordabilityIndex = 0.88m }
            };
        }
    }
}
