using GithubActionNetCoreAPI;
using GithubActionNetCoreAPI.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GithubActionsNetCoreAPI.Tests
{
    public class DemoTests
    {
        [Fact]
        public void Test1()
        {
            Assert.True(1 == 1);
        }

        [Fact]
        public async Task CustomerIntegrationTest()
        {
            // Create a DB context
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
            var optionsBuilder = new DbContextOptionsBuilder<CustomerContext>();
            optionsBuilder.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
            var context =  new CustomerContext(optionsBuilder.Options);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();


            // Just to make sure: Delete all existing customers in th DB
            /*context.Customers.RemoveRange(await context.Customers.ToArrayAsync());
            await context.SaveChangesAsync(); */

            // Create controller
            var controller = new CustomersController(context);

            // Add customer
            controller.Add(new Customer() { CustomerName = "Foobar" });

            // Check: Does GetAll return the added customer
            var result = (await controller.GetAll()).ToArray();
            Assert.Single(result);
            Assert.Equal("Foobar", result[0].CustomerName);
        }
    }
}