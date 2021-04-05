using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using WebStore.Application.InventoryAdmin;
using WebStore.Database;
using WebStore.Models;
using Moq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace WebStore.Tests.UnitTests.AdminInventory
{
    [TestClass]
    public class CreateInventoryTest
    {
        private Mock<DbSet<Inventory>> mockInventorySet;
        private Mock<ApplicationDbContext> mockContext;
        [TestMethod]
        public void TestCreateInventory()
        {
            Random random = new Random();
            mockContext = new Mock<ApplicationDbContext>();

            string description = StringUtilities.CreateRandomString(0, 5000);
            int quantity = random.Next();
            double price = random.NextDouble();

            Product product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Description = "Test Product Description",
                Price = 12.00,
                Inventory = new List<Inventory>()
            };
            var request = new CreateInventory.Request
            {
                ProductId = product.Id,
                Description = description,
                Quantity = quantity,
                Price = price
            };

            mockInventorySet = new Mock<DbSet<Inventory>>();


            var productData = new List<Product>{ product }.AsQueryable();
            
            var mockProductSet = new Mock<DbSet<Product>>();
            mockProductSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productData.Provider);
            mockProductSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productData.Expression);
            mockProductSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productData.ElementType);
            mockProductSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(() => productData.GetEnumerator());
            

            mockContext.Setup(m => m.Inventory).Returns(mockInventorySet.Object);
            mockContext.Setup(m => m.Products).Returns(mockProductSet.Object);
            

            var createInventory = new CreateInventory(mockContext.Object);

            //valid request
            RunValidTest(createInventory, request);

            //invalid requests
            //null request
            RunBadRequest(createInventory, null);

            //bad productId
            request.ProductId = 2;
            RunBadRequest(createInventory, request);

            //neg productId
            request.ProductId = -1;
            RunBadRequest(createInventory, request);
            request.ProductId = 1;

            //neg price
            request.Price = -12.00;
            RunBadRequest(createInventory, request);
            request.Price = 12.00;

            //neg quantity
            request.Quantity = -1;
            RunBadRequest(createInventory, request);

        }

        public void RunValidTest(CreateInventory createInventory, CreateInventory.Request request)
        {
            var actual = createInventory.Do(request).Result;

            actual.Should().NotBeNull();
            actual.Status.Should().Be(201);
            actual.Inventory.Should().NotBeNull();
            actual.Inventory.Description.Should().BeEquivalentTo(request.Description);
            actual.Inventory.Quantity.Should().Be(request.Quantity);
            actual.Inventory.Price.Should().Be(request.Price);

            mockInventorySet.Verify(m => m.Add(It.IsAny<Inventory>()), Times.Once());            
        }

        public void RunBadRequest(CreateInventory createInventory, CreateInventory.Request request)
        {
            var actual = createInventory.Do(request).Result;
            actual.Should().NotBeNull();
            actual.Status.Should().Be(400);
            actual.Inventory.Should().BeNull();
        }

    }
}
