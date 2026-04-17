using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ExoticBackend.Data;
using ExoticBackend.Models;
using ExoticBackend.DTOs;

namespace ExoticBackend.Tests
{
    public class BackendUnitTests
    {
        private ExoticDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ExoticDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ExoticDbContext(options);
        }

        [Fact]
        public void RegisterAndHashTest()
        {
            string plainPassword = "SecretPassword123!";

            string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword, salt);

            Assert.NotEqual(plainPassword, passwordHash);
            Assert.True(BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash));
        }

        
        [Fact]
        public async Task GetDriversTest()
        {
            var db = GetInMemoryDbContext("TestDb_Drivers");
            db.Users.Add(new User { Id = 1, Username = "SimaUtas", isDriver = false });
            db.Users.Add(new User { Id = 2, Username = "ProfiSofőr", isDriver = true });
            await db.SaveChangesAsync();

            var drivers = await db.Users.Where(u => u.isDriver == true).ToListAsync();

            Assert.Single(drivers);
            Assert.Equal(2, drivers[0].Id);
        }

        [Fact]
        public async Task GetVehiclesTest()
        {
            var db = GetInMemoryDbContext("TestDb_FilterVehicles");
            db.Vehicles.Add(new Vehicle { Id = 1, Category = "Sport", Fuel = "Benzin" });
            db.Vehicles.Add(new Vehicle { Id = 2, Category = "SUV", Fuel = "Dízel" });
            await db.SaveChangesAsync();
            string categoryToSearch = "Sport";

            var query = db.Vehicles.AsQueryable();
            if (!string.IsNullOrWhiteSpace(categoryToSearch))
            {
                query = query.Where(v => v.Category == categoryToSearch);
            }
            var result = await query.ToListAsync();

            Assert.Single(result);
            Assert.Equal("Sport", result[0].Category);
        }

        [Fact]
        public async Task VerifyTest()
        {
            var db = GetInMemoryDbContext("TestDb_License");
            
            var user = new User { Id = 1, Is_Verified = 1, Clearance = 1 }; 
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var dbUser = await db.Users.FindAsync(1);
            dbUser.LicenseNumber = "ABC-123456";
            if (dbUser.Is_Verified == 1)
            {
                dbUser.Clearance = 2;
            }
            await db.SaveChangesAsync();

            Assert.Equal(2, dbUser.Clearance);
            Assert.Equal("ABC-123456", dbUser.LicenseNumber);
        }

       
        [Fact]
        public async Task OrderLogicTest()
        {
            
            var db = GetInMemoryDbContext("TestDb_VerifyOrder");
           
            var vehicle = new Vehicle { Id = 1, Status = 1, Times_Rented = 5, Fuel = "Benzin" };
            var order = new Order { Id = 1, VerificationToken = "token123", Status = 1, Vehicle = vehicle };
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            var dbOrder = await db.Orders.Include(o => o.Vehicle).FirstOrDefaultAsync(o => o.VerificationToken == "token123");
            dbOrder.Status = 2;
            dbOrder.VerificationToken = null;
            dbOrder.Vehicle.Status = 2;
            dbOrder.Vehicle.Times_Rented += 1;
            await db.SaveChangesAsync();

            var updatedVehicle = await db.Vehicles.FindAsync(1);
            Assert.Equal(2, dbOrder.Status);
            Assert.Null(dbOrder.VerificationToken);
            Assert.Equal(2, updatedVehicle.Status);
            Assert.Equal(6, updatedVehicle.Times_Rented);
        }

        [Fact]
        public async Task FinishOrderTest()
        {
            
            var db = GetInMemoryDbContext("TestDb_FinishOrder");
            
            var vehicle = new Vehicle { Id = 1, Status = 2, Fuel = "Benzin" };
            var order = new Order { Id = 1, Status = 2, Vehicle = vehicle };
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            
            var dbOrder = await db.Orders.Include(o => o.Vehicle).FirstOrDefaultAsync(o => o.Id == 1);
            dbOrder.Status = 3; 
            dbOrder.Vehicle.Status = 1;
            await db.SaveChangesAsync();

            Assert.Equal(3, dbOrder.Status);
            Assert.Equal(1, dbOrder.Vehicle.Status);
        }

        
        [Fact]
        public async Task TaxiOrderTest()
        {
            
            var db = GetInMemoryDbContext("TestDb_TaxiAccept");
            var taxiOrder = new TaxiOrder { Id = 1, Status = 1 };
            db.TaxiOrders.Add(taxiOrder);
            await db.SaveChangesAsync();

           
            var dbOrder = await db.TaxiOrders.FindAsync(1);
            dbOrder.Status = 2;
            await db.SaveChangesAsync();

            Assert.Equal(2, dbOrder.Status);
        }
    }
}