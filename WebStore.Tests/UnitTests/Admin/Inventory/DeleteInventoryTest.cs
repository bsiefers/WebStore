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
    public class DeleteInventoryTest
    {
        private Mock<ApplicationDbContext> mockContext;
        
        [TestMethod]
        public void TestDeleteInventory()
        {
            mockContext = new Mock<ApplicationDbContext>();

            Inventory inventory = new Inventory
            {
                Id = 1,
                Description = "Test Inventory Description",
                Price = 12.99,
                ProductId = 1,
                Quantity = 10
            };            

            var inventoryData = new List<Inventory> { inventory }.AsQueryable();
            var mockInventorySet = new Mock<DbSet<Inventory>>();
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(inventoryData.Provider);
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventoryData.Expression);
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventoryData.ElementType);
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(() => inventoryData.GetEnumerator());

            mockContext.Setup(m => m.Inventory).Returns(mockInventorySet.Object);
            var deleteInventory = new DeleteInventory(mockContext.Object);
            
            //invalid request
            RunInvalidRequest(deleteInventory, -1);
            //valid request
            RunValidRequest(deleteInventory, 1);

        }

        public void RunValidRequest(DeleteInventory deleteInventory, int id)
        {
            var response = deleteInventory.Do(id).Result;
            response.Should().NotBeNull();
            response.Status.Should().Be(200);
        }

        public void RunInvalidRequest(DeleteInventory deleteInventory, int id)
        {            
            var response = deleteInventory.Do(id).Result;
            response.Should().NotBeNull();
            response.Status.Should().Be(404);            
        }
    }
}
