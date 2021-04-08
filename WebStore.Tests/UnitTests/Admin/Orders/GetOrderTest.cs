using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Application.OrdersAdmin;
using WebStore.Database;
using WebStore.Models;

namespace WebStore.Tests.UnitTests.Admin.Orders
{
    [TestClass]
    public class GetOrderTest
    {
        [TestMethod]
        public void TestGetOrder()
        {
            Random random = new Random();
            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();


            Inventory inventory = new Inventory
            {
                Id = 1,
                Description = "Test Inventory Description",
                Price = 12.99,
                ProductId = 1,
                Quantity = 10
            };
            Order order = new Order
            {
                Id = 1,
                StripeRef = "TestRef",
                FirstName = "firstName",
                LastName = "lastName",
                Email = "test@test.com",
                PhoneNumber = "373 234 3424",
                Address1 = "address 1",
                Address2 = "address 2",
                City = "test city",
                State = "test state",
                Country = "test country",
                PostCode = "test postCode",
                Status = "shipped",
                Total = 12.99,
                OrderDate = DateTime.UtcNow,
                OrderInventory = new List<OrderInventory>
                {
                    new OrderInventory
                    {
                        OrderId = 1,
                        InventoryId = 1
                    }
                },
                Note = ""
            };

            var orderData = new List<Order> { order }.AsQueryable();
            var mockOrderSet = new Mock<DbSet<Order>>();
            mockOrderSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orderData.Provider);
            mockOrderSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orderData.Expression);
            mockOrderSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orderData.ElementType);
            mockOrderSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(() => orderData.GetEnumerator());

            var inventoryData = new List<Inventory> { inventory }.AsQueryable();
            var mockInventorySet = new Mock<DbSet<Inventory>>();
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(inventoryData.Provider);
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventoryData.Expression);
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventoryData.ElementType);
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(() => inventoryData.GetEnumerator());

            mockContext.Setup(m => m.Inventory).Returns(mockInventorySet.Object);
            mockContext.Setup(m => m.Orders).Returns(mockOrderSet.Object);

            GetOrder getOrder = new GetOrder(mockContext.Object);
            var actual = getOrder.Do(1);
            actual.Should().NotBeNull();
            actual.Status.Should().Be(200);
            actual.Order.Should().NotBeNull();            
            actual.Order.PhoneNumber.Should().BeEquivalentTo(order.PhoneNumber);
            actual.Order.Email.Should().BeEquivalentTo(order.Email);
            actual.Order.Status.Should().BeEquivalentTo(order.Status);
            actual.Order.Total.Should().Be(order.Total);
            actual.Order.OrderDate.Should().NotBeNull();
            actual.Order.Note.Should().BeEquivalentTo(order.Note);            


            actual = getOrder.Do(-1);
            actual.Should().NotBeNull();
            actual.Status.Should().Be(404);
        }
    }
}
