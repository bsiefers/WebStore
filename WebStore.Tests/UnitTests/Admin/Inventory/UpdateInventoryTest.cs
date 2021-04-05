using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebStore.Application.InventoryAdmin;
using WebStore.Database;
using WebStore.Models;
using Moq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace WebStore.Tests.UnitTests.AdminInventory
{
    [TestClass]
    public class UpdateInventoryTest
    {        
        private Mock<ApplicationDbContext> mockContext;

        [TestMethod]
        public void TestUpdateInventory()
        {
            Random random = new Random();
            mockContext = new Mock<ApplicationDbContext>();


            string description = StringUtilities.CreateRandomString(0, 5000);
            int quantity = random.Next();
            double price = random.NextDouble();
            Inventory inventory = new Inventory
            {
                Id = 1,
                Description = "Test Inventory Description",
                Price = 12.99,
                ProductId = 1,
                Quantity = 10
            };
            Product product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Description = "Test Product Description",
                Inventory = new List<Inventory>()
            };
            product.Inventory.Add(inventory);


            var request = new UpdateInventory.Request
            {
                Id = 1,
                ProductId = product.Id,
                Description = description,
                Quantity = quantity,
                Price = price
            };

            var inventoryData = new List<Inventory> { inventory }.AsQueryable();
            var mockInventorySet = new Mock<DbSet<Inventory>>();
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(inventoryData.Provider);
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventoryData.Expression);
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventoryData.ElementType);
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(() => inventoryData.GetEnumerator());


            var productData = new List<Product> { product }.AsQueryable();
            var mockProductSet = new Mock<DbSet<Product>>();
            mockProductSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productData.Provider);
            mockProductSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productData.Expression);
            mockProductSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productData.ElementType);
            mockProductSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(() => productData.GetEnumerator());

            mockContext.Setup(m => m.Inventory).Returns(mockInventorySet.Object);
            mockContext.Setup(m => m.Products).Returns(mockProductSet.Object);

            var updateInventory = new UpdateInventory(mockContext.Object);

            //valid request
            RunValidTest(updateInventory, request);

            //invalid requist
            //null request
            RunBadRequest(updateInventory, null, 400);

            //bad inventoryId
            request.Id = -1;
            RunBadRequest(updateInventory, request, 404);
            request.Id = 1;

            //bad productId
            request.ProductId = 0;
            RunBadRequest(updateInventory, request, 404);
            request.ProductId = 1;

            //bad quantity            
            request.Quantity = -1;
            RunBadRequest(updateInventory, request, 400);
            request.Quantity = 1;

            //bad price
            request.Price = -10.0;
            RunBadRequest(updateInventory, request, 400);
            request.Price = 10.0;

        }



        public void RunValidTest(UpdateInventory updateInventory, UpdateInventory.Request request)
        {
            var actual = updateInventory.Do(request).Result;

            actual.Should().NotBeNull();
            actual.Inventory.Should().NotBeNull();
            actual.Inventory.Description.Should().BeEquivalentTo(request.Description);
            actual.Inventory.Quantity.Should().Be(request.Quantity);
            actual.Inventory.Price.Should().Be(request.Price);

        }

        public void RunBadRequest(UpdateInventory updateInventory, UpdateInventory.Request request, int expectedCode)
        {
            var actual = updateInventory.Do(request).Result;

            actual.Should().NotBeNull();
            actual.Should().NotBeNull();
            actual.Inventory.Should().BeNull();            
            actual.Status.Should().Be(expectedCode);
        }
    }
}
