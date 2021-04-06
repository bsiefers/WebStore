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
    public class DeleteProductTest
    {
        [TestMethod]
        public void TestDeleteProduct()
        {
            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();
            Product product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Description = "Test Product Description",
                Inventory = new List<Inventory>()
            };

            var productData = new List<Product> { product }.AsQueryable();
            var mockProductSet = new Mock<DbSet<Product>>();
            mockProductSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productData.Provider);
            mockProductSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productData.Expression);
            mockProductSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productData.ElementType);
            mockProductSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(() => productData.GetEnumerator());

            mockContext.Setup(m => m.Products).Returns(mockProductSet.Object);
            var deleteProduct = new DeleteProduct(mockContext.Object);

            //invalid request
            RunInvalidRequest(deleteProduct, -1);
            //valid request
            RunValidRequest(deleteProduct, 1);
        }

        public void RunValidRequest(DeleteProduct deleteInventory, int id)
        {
            var response = deleteInventory.Do(id).Result;
            response.Should().NotBeNull();
            response.Status.Should().Be(200);
        }

        public void RunInvalidRequest(DeleteProduct deleteInventory, int id)
        {
            var response = deleteInventory.Do(id).Result;
            response.Should().NotBeNull();
            response.Status.Should().Be(404);
        }
    }


}
