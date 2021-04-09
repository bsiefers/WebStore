using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Application.Products;
using WebStore.Database;
using WebStore.Models;

namespace WebStore.Tests.UnitTests.Products
{
    [TestClass]
    public class GetProductsTest
    {
        [TestMethod]
        public void TestGetProducts()
        {
            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();
            Product product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Description = "Test Product Description",
                Inventory = new List<Inventory>(),

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

            var getProducts = new GetProducts(mockContext.Object);

            var response = getProducts.Do();
            response.Should().NotBeNull();
            response.Status.Should().Be(200);
            var pvm = response.Products.FirstOrDefault();            
            pvm.Name.Should().BeEquivalentTo(product.Name);
            pvm.Description.Should().BeEquivalentTo(product.Description);            
        }
    }
}
