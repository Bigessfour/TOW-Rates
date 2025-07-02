using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace WileyBudgetManagement.Database
{
    public class DatabaseManager
    {
        private readonly string _connectionString;
        private readonly bool _enableMockData;

        public DatabaseManager()
        {
            _connectionString = GetConnectionString();
            _enableMockData = bool.Parse(ConfigurationManager.AppSettings["EnableMockData"] ?? "true");
        }

        private string GetConnectionString()
        {
            string provider = ConfigurationManager.AppSettings["DatabaseProvider"] ?? "SqlServerExpress";

            return provider switch
            {
                "LocalDB" => ConfigurationManager.ConnectionStrings["WileyBudgetLocalDB"]?.ConnectionString
                           ?? throw new InvalidOperationException("LocalDB connection string not found"),
                "SqlServerExpress" => ConfigurationManager.ConnectionStrings["WileyBudgetDB"]?.ConnectionString
                                    ?? throw new InvalidOperationException("SQL Server Express connection string not found"),
                _ => throw new InvalidOperationException($"Unknown database provider: {provider}")
            };
        }

        public async Task<SqlConnection> GetConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public bool IsMockDataEnabled => _enableMockData;

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var connection = await GetConnectionAsync();
                return connection.State == ConnectionState.Open;
            }
            catch
            {
                return false;
            }
        }

        public async Task InitializeDatabaseAsync()
        {
            if (_enableMockData)
            {
                // Skip database initialization when using mock data
                return;
            }

            try
            {
                using var connection = await GetConnectionAsync();

                // Check if database exists and create tables if needed
                await EnsureTablesExistAsync(connection);
                await SeedInitialDataAsync(connection);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize database: {ex.Message}", ex);
            }
        }

        private async Task EnsureTablesExistAsync(SqlConnection connection)
        {
            string[] tableNames = { "SanitationDistrict", "Water", "Trash", "Apartments", "Summary" };

            foreach (string tableName in tableNames)
            {
                string checkTableQuery = $@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{tableName}' AND xtype='U')
                    BEGIN
                        {GetCreateTableScript(tableName)}
                    END";

                using var command = new SqlCommand(checkTableQuery, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        private string GetCreateTableScript(string tableName)
        {
            return tableName switch
            {
                "SanitationDistrict" or "Water" or "Trash" or "Apartments" => $@"
                    CREATE TABLE {tableName} (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Account NVARCHAR(50) NOT NULL,
                        Label NVARCHAR(100) NOT NULL,
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
                        EntryDate DATETIME DEFAULT GETDATE(),
                        Total DECIMAL(18,2),
                        UNIQUE(Account)
                    )",
                "Summary" => @"
                    CREATE TABLE Summary (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Enterprise NVARCHAR(50) NOT NULL,
                        TotalOperatingIncome DECIMAL(18,2),
                        TotalOMExpenses DECIMAL(18,2),
                        TotalAdminExpenses DECIMAL(18,2),
                        NetSurplusDeficit DECIMAL(18,2),
                        PercentOfTotalBudget DECIMAL(5,2),
                        RequiredRate DECIMAL(18,2),
                        TrendData NVARCHAR(MAX),
                        EntryDate DATETIME DEFAULT GETDATE()
                    )",
                _ => throw new ArgumentException($"Unknown table name: {tableName}")
            };
        }

        private async Task SeedInitialDataAsync(SqlConnection connection)
        {
            // Check if data already exists
            string checkDataQuery = "SELECT COUNT(*) FROM Water UNION ALL SELECT COUNT(*) FROM Trash UNION ALL SELECT COUNT(*) FROM Apartments";
            using var checkCommand = new SqlCommand(checkDataQuery, connection);

            // Only seed if tables are empty
            var reader = await checkCommand.ExecuteReaderAsync();
            bool hasData = false;
            while (await reader.ReadAsync())
            {
                if (reader.GetInt32(0) > 0)
                {
                    hasData = true;
                    break;
                }
            }
            reader.Close();

            if (!hasData)
            {
                await SeedWaterDataAsync(connection);
                await SeedTrashDataAsync(connection);
                await SeedApartmentDataAsync(connection);
            }
        }

        private async Task SeedWaterDataAsync(SqlConnection connection)
        {
            string insertQuery = @"
                INSERT INTO Water (Account, Label, Section, CurrentFYBudget, MonthlyInput, YearToDateSpending, PercentOfBudget, BudgetRemaining, MonthlyUsage, RequiredRate)
                VALUES 
                (@Account1, @Label1, @Section1, @Budget1, @Monthly1, @YTD1, @Percent1, @Remaining1, @Usage1, @Rate1),
                (@Account2, @Label2, @Section2, @Budget2, @Monthly2, @YTD2, @Percent2, @Remaining2, @Usage2, @Rate2),
                (@Account3, @Label3, @Section3, @Budget3, @Monthly3, @YTD3, @Percent3, @Remaining3, @Usage3, @Rate3)";

            using var command = new SqlCommand(insertQuery, connection);

            // Water data parameters
            command.Parameters.AddWithValue("@Account1", "W001");
            command.Parameters.AddWithValue("@Label1", "Water Treatment Plant Operations");
            command.Parameters.AddWithValue("@Section1", "Operations");
            command.Parameters.AddWithValue("@Budget1", 250000);
            command.Parameters.AddWithValue("@Monthly1", 20833.33m);
            command.Parameters.AddWithValue("@YTD1", 125000);
            command.Parameters.AddWithValue("@Percent1", 0.50m);
            command.Parameters.AddWithValue("@Remaining1", 125000);
            command.Parameters.AddWithValue("@Usage1", 1500000);
            command.Parameters.AddWithValue("@Rate1", 0.167m);

            command.Parameters.AddWithValue("@Account2", "W002");
            command.Parameters.AddWithValue("@Label2", "Water Distribution Maintenance");
            command.Parameters.AddWithValue("@Section2", "Maintenance");
            command.Parameters.AddWithValue("@Budget2", 180000);
            command.Parameters.AddWithValue("@Monthly2", 15000);
            command.Parameters.AddWithValue("@YTD2", 90000);
            command.Parameters.AddWithValue("@Percent2", 0.50m);
            command.Parameters.AddWithValue("@Remaining2", 90000);
            command.Parameters.AddWithValue("@Usage2", 1200000);
            command.Parameters.AddWithValue("@Rate2", 0.125m);

            command.Parameters.AddWithValue("@Account3", "W003");
            command.Parameters.AddWithValue("@Label3", "Water Quality Testing");
            command.Parameters.AddWithValue("@Section3", "Quality");
            command.Parameters.AddWithValue("@Budget3", 45000);
            command.Parameters.AddWithValue("@Monthly3", 3750);
            command.Parameters.AddWithValue("@YTD3", 22500);
            command.Parameters.AddWithValue("@Percent3", 0.50m);
            command.Parameters.AddWithValue("@Remaining3", 22500);
            command.Parameters.AddWithValue("@Usage3", 0);
            command.Parameters.AddWithValue("@Rate3", 0.0375m);

            await command.ExecuteNonQueryAsync();
        }

        private async Task SeedTrashDataAsync(SqlConnection connection)
        {
            // Execute comprehensive Trash data population script
            string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "PopulateTrashData.sql");
            
            if (File.Exists(scriptPath))
            {
                string scriptContent = await File.ReadAllTextAsync(scriptPath);
                
                // Split the script into individual commands (by GO statements)
                var commands = scriptContent.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (string commandText in commands)
                {
                    string trimmedCommand = commandText.Trim();
                    if (!string.IsNullOrEmpty(trimmedCommand) && !trimmedCommand.StartsWith("--") && !trimmedCommand.StartsWith("PRINT"))
                    {
                        try
                        {
                            using var command = new SqlCommand(trimmedCommand, connection);
                            command.CommandTimeout = 300; // 5 minutes timeout for complex operations
                            await command.ExecuteNonQueryAsync();
                        }
                        catch (Exception ex)
                        {
                            // Log but continue with next command
                            Console.WriteLine($"Warning: Error executing Trash data command: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                // Fallback to basic Trash data if script file not found
                await SeedBasicTrashDataAsync(connection);
            }
        }

        private async Task SeedBasicTrashDataAsync(SqlConnection connection)
        {
            string insertQuery = @"
                INSERT INTO Trash (Account, Label, Section, CurrentFYBudget, MonthlyInput, YearToDateSpending, PercentOfBudget, BudgetRemaining, SeasonalAdjustment, RequiredRate, EntryDate, MonthlyUsage, TimeOfUseFactor, CustomerAffordabilityIndex)
                VALUES 
                (@Account1, @Label1, @Section1, @Budget1, @Monthly1, @YTD1, @Percent1, @Remaining1, @Seasonal1, @Rate1, GETDATE(), @Usage1, 1.0, 1.0),
                (@Account2, @Label2, @Section2, @Budget2, @Monthly2, @YTD2, @Percent2, @Remaining2, @Seasonal2, @Rate2, GETDATE(), @Usage2, 1.0, 1.0),
                (@Account3, @Label3, @Section3, @Budget3, @Monthly3, @YTD3, @Percent3, @Remaining3, @Seasonal3, @Rate3, GETDATE(), @Usage3, 1.0, 1.0),
                (@Account4, @Label4, @Section4, @Budget4, @Monthly4, @YTD4, @Percent4, @Remaining4, @Seasonal4, @Rate4, GETDATE(), @Usage4, 1.0, 1.0)";

            using var command = new SqlCommand(insertQuery, connection);

            // Enhanced Trash data parameters
            command.Parameters.AddWithValue("@Account1", "T301.00");
            command.Parameters.AddWithValue("@Label1", "Residential Trash Collection Fees");
            command.Parameters.AddWithValue("@Section1", "Revenue");
            command.Parameters.AddWithValue("@Budget1", 320000);
            command.Parameters.AddWithValue("@Monthly1", 26666.67m);
            command.Parameters.AddWithValue("@YTD1", 185333.34m);
            command.Parameters.AddWithValue("@Percent1", 0.58m);
            command.Parameters.AddWithValue("@Remaining1", 134666.66m);
            command.Parameters.AddWithValue("@Seasonal1", 8000);
            command.Parameters.AddWithValue("@Rate1", 20.88m);
            command.Parameters.AddWithValue("@Usage1", 850);

            command.Parameters.AddWithValue("@Account2", "T401.00");
            command.Parameters.AddWithValue("@Label2", "Collection Route Operations");
            command.Parameters.AddWithValue("@Section2", "Collections");
            command.Parameters.AddWithValue("@Budget2", 180000);
            command.Parameters.AddWithValue("@Monthly2", 15000);
            command.Parameters.AddWithValue("@YTD2", 105000);
            command.Parameters.AddWithValue("@Percent2", 0.58m);
            command.Parameters.AddWithValue("@Remaining2", 75000);
            command.Parameters.AddWithValue("@Seasonal2", 3000);
            command.Parameters.AddWithValue("@Rate2", 21.18m);
            command.Parameters.AddWithValue("@Usage2", 0);

            command.Parameters.AddWithValue("@Account3", "T501.00");
            command.Parameters.AddWithValue("@Label3", "Recycling Collection");
            command.Parameters.AddWithValue("@Section3", "Recycling");
            command.Parameters.AddWithValue("@Budget3", 45000);
            command.Parameters.AddWithValue("@Monthly3", 3750);
            command.Parameters.AddWithValue("@YTD3", 26250);
            command.Parameters.AddWithValue("@Percent3", 0.58m);
            command.Parameters.AddWithValue("@Remaining3", 18750);
            command.Parameters.AddWithValue("@Seasonal3", 1000);
            command.Parameters.AddWithValue("@Rate3", 5.29m);
            command.Parameters.AddWithValue("@Usage3", 125);

            command.Parameters.AddWithValue("@Account4", "T600.00");
            command.Parameters.AddWithValue("@Label4", "Trash Collection Vehicles");
            command.Parameters.AddWithValue("@Section4", "Equipment");
            command.Parameters.AddWithValue("@Budget4", 50000);
            command.Parameters.AddWithValue("@Monthly4", 4166.67m);
            command.Parameters.AddWithValue("@YTD4", 29166.67m);
            command.Parameters.AddWithValue("@Percent4", 0.58m);
            command.Parameters.AddWithValue("@Remaining4", 20833.33m);
            command.Parameters.AddWithValue("@Seasonal4", 0);
            command.Parameters.AddWithValue("@Rate4", 17.65m);
            command.Parameters.AddWithValue("@Usage4", 0);

            await command.ExecuteNonQueryAsync();
        }

        private async Task SeedApartmentDataAsync(SqlConnection connection)
        {
            string insertQuery = @"
                INSERT INTO Apartments (Account, Label, Section, CurrentFYBudget, MonthlyInput, YearToDateSpending, PercentOfBudget, BudgetRemaining, MonthlyUsage, RequiredRate, CustomerAffordabilityIndex)
                VALUES 
                (@Account1, @Label1, @Section1, @Budget1, @Monthly1, @YTD1, @Percent1, @Remaining1, @Usage1, @Rate1, @Afford1),
                (@Account2, @Label2, @Section2, @Budget2, @Monthly2, @YTD2, @Percent2, @Remaining2, @Usage2, @Rate2, @Afford2),
                (@Account3, @Label3, @Section3, @Budget3, @Monthly3, @YTD3, @Percent3, @Remaining3, @Usage3, @Rate3, @Afford3),
                (@Account4, @Label4, @Section4, @Budget4, @Monthly4, @YTD4, @Percent4, @Remaining4, @Usage4, @Rate4, @Afford4)";

            using var command = new SqlCommand(insertQuery, connection);

            // Apartment data parameters
            command.Parameters.AddWithValue("@Account1", "APT001");
            command.Parameters.AddWithValue("@Label1", "Meadowbrook Apartments (24 units)");
            command.Parameters.AddWithValue("@Section1", "Zone A");
            command.Parameters.AddWithValue("@Budget1", 43200);
            command.Parameters.AddWithValue("@Monthly1", 3600);
            command.Parameters.AddWithValue("@YTD1", 21600);
            command.Parameters.AddWithValue("@Percent1", 0.50m);
            command.Parameters.AddWithValue("@Remaining1", 21600);
            command.Parameters.AddWithValue("@Usage1", 24);
            command.Parameters.AddWithValue("@Rate1", 150.00m);
            command.Parameters.AddWithValue("@Afford1", 0.85m);

            command.Parameters.AddWithValue("@Account2", "APT002");
            command.Parameters.AddWithValue("@Label2", "Sunset Manor (36 units)");
            command.Parameters.AddWithValue("@Section2", "Zone B");
            command.Parameters.AddWithValue("@Budget2", 64800);
            command.Parameters.AddWithValue("@Monthly2", 5400);
            command.Parameters.AddWithValue("@YTD2", 32400);
            command.Parameters.AddWithValue("@Percent2", 0.50m);
            command.Parameters.AddWithValue("@Remaining2", 32400);
            command.Parameters.AddWithValue("@Usage2", 36);
            command.Parameters.AddWithValue("@Rate2", 150.00m);
            command.Parameters.AddWithValue("@Afford2", 0.78m);

            command.Parameters.AddWithValue("@Account3", "APT003");
            command.Parameters.AddWithValue("@Label3", "Riverside Condos (18 units)");
            command.Parameters.AddWithValue("@Section3", "Zone A");
            command.Parameters.AddWithValue("@Budget3", 32400);
            command.Parameters.AddWithValue("@Monthly3", 2700);
            command.Parameters.AddWithValue("@YTD3", 16200);
            command.Parameters.AddWithValue("@Percent3", 0.50m);
            command.Parameters.AddWithValue("@Remaining3", 16200);
            command.Parameters.AddWithValue("@Usage3", 18);
            command.Parameters.AddWithValue("@Rate3", 150.00m);
            command.Parameters.AddWithValue("@Afford3", 0.92m);

            command.Parameters.AddWithValue("@Account4", "APT004");
            command.Parameters.AddWithValue("@Label4", "Oak Street Townhomes (12 units)");
            command.Parameters.AddWithValue("@Section4", "Zone C");
            command.Parameters.AddWithValue("@Budget4", 21600);
            command.Parameters.AddWithValue("@Monthly4", 1800);
            command.Parameters.AddWithValue("@YTD4", 10800);
            command.Parameters.AddWithValue("@Percent4", 0.50m);
            command.Parameters.AddWithValue("@Remaining4", 10800);
            command.Parameters.AddWithValue("@Usage4", 12);
            command.Parameters.AddWithValue("@Rate4", 150.00m);
            command.Parameters.AddWithValue("@Afford4", 0.88m);

            await command.ExecuteNonQueryAsync();
        }
    }
}
