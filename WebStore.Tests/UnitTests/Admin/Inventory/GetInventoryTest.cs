using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Application.InventoryAdmin;
using WebStore.Database;
using WebStore.Models;

namespace WebStore.Tests.UnitTests.AdminInventory
{
    [TestClass]
    public class GetInventoryTest
    {

        [TestMethod]
        public void TestGetInventory()
        {
            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();

            Product product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Description = "Test Product Description",
                Inventory = new List<Inventory>()
            };

            Inventory inventory = new Inventory
            {
                Id = 1,
                Description = "Test Inventory Description",
                Price = 12.99,
                ProductId = 1,
                Quantity = 10,
                Product = product
            };
            product.Inventory.Add(inventory);

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

            var getInventory = new GetInventory(mockContext.Object);
            //valid request
            var response = getInventory.Do();
            response.Should().NotBeNull();
            var pvm = response.FirstOrDefault();
            pvm.Id.Should().Be(product.Id);            
            pvm.ProductName.Should().BeEquivalentTo(product.Name);
            pvm.Description.Should().BeEquivalentTo(product.Description);
            
            var ivm = pvm.Inventory.FirstOrDefault();
            ivm.Id.Should().Be(inventory.Id);
            ivm.Description.Should().Be(inventory.Description);
            ivm.Price.Should().Be(inventory.Price);
            ivm.ProductId.Should().Be(product.Id);
            ivm.Quantity.Should().Be(inventory.Quantity);

        }

    }
}
