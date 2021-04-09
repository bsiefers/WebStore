using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Application.ProductsAdmin;
using WebStore.Database;
using WebStore.Models;

namespace WebStore.Tests.UnitTests.Admin.Products
{
    [TestClass]
    public class CreateProductTest
    {                
        [TestMethod]
        public void TestCreateProduct()
        {
            Random random = new Random();
            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();

            string name = StringUtilities.CreateRandomString(0, 300);
            string description = StringUtilities.CreateRandomString(0, 5000);
            
            var request = new CreateProduct.Request
            {
                Name = name,
                Description = description
            };
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

            mockContext.Setup(m => m.Products).Returns(mockProductSet.Object);
            mockContext.Setup(m => m.Inventory).Returns(mockInventorySet.Object);

            var createProduct = new CreateProduct(mockContext.Object);
            //run valid request
            RunValidTest(createProduct, request);

            //bad requests
            request.Name = null;
            RunBadRequest(createProduct, request);
            request.Name = name;

            request.Description = null;
            RunBadRequest(createProduct, request);
            request.Description = name;
        }


        public void RunValidTest(CreateProduct createProduct, CreateProduct.Request request)
        {
            var actual = createProduct.Do(request).Result;
            actual.Should().NotBeNull();
            actual.Status.Should().Be(201);
            actual.Product.Should().NotBeNull();
            actual.Product.Name.Should().BeEquivalentTo(request.Name);
            actual.Product.Description.Should().BeEquivalentTo(request.Description);
        }

        public void RunBadRequest(CreateProduct createProduct, CreateProduct.Request request)
        {
            var actual = createProduct.Do(request).Result;
            actual.Should().NotBeNull();
            actual.Status.Should().Be(400);
            actual.Product.Should().BeNull();
        }
    }
}
