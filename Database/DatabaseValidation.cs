using System;
using System.Threading.Tasks;
using WileyBudgetManagement.Database;

namespace WileyBudgetManagement.Database
{
    public class DatabaseValidation
    {
        public static async Task<bool> ValidateDatabaseConnection()
        {
            try
            {
                var databaseManager = new DatabaseManager();

                Console.WriteLine("Testing database connection...");
                bool connectionTest = await databaseManager.TestConnectionAsync();

                if (!connectionTest)
                {
                    Console.WriteLine("❌ Database connection failed!");
                    return false;
                }

                Console.WriteLine("✅ Database connection successful!");

                // Test repository
                var repository = new SanitationRepository(databaseManager);

                Console.WriteLine("Testing Sewer enterprise data retrieval...");
                var sewerData = await repository.GetSanitationDistrictDataAsync();
                Console.WriteLine($"✅ Retrieved {sewerData.Count} Sewer records");

                Console.WriteLine("Testing Water enterprise data retrieval...");
                var waterData = await repository.GetWaterDataAsync();
                Console.WriteLine($"✅ Retrieved {waterData.Count} Water records");

                Console.WriteLine("Testing Trash enterprise data retrieval...");
                var trashData = await repository.GetTrashDataAsync();
                Console.WriteLine($"✅ Retrieved {trashData.Count} Trash records");

                Console.WriteLine("Testing Apartments enterprise data retrieval...");
                var apartmentData = await repository.GetApartmentDataAsync();
                Console.WriteLine($"✅ Retrieved {apartmentData.Count} Apartment records");

                Console.WriteLine("\n🎯 Database Status Summary:");
                Console.WriteLine($"   • Sewer Enterprise: {sewerData.Count} records (ACTIVE)");
                Console.WriteLine($"   • Water Enterprise: {waterData.Count} records (Ready for implementation)");
                Console.WriteLine($"   • Trash Enterprise: {trashData.Count} records (Ready for implementation)");
                Console.WriteLine($"   • Apartments Enterprise: {apartmentData.Count} records (Ready for implementation)");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database validation failed: {ex.Message}");
                return false;
            }
        }
    }
}
