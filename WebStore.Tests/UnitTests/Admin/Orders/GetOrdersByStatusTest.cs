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
    public class GetOrdersByStatusTest
    {

        
        [TestMethod]
        public void TestGetOrders()
        {
            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();

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

            mockContext.Setup(m => m.Orders).Returns(mockOrderSet.Object);

            GetOrdersByStatus getOrders = new GetOrdersByStatus(mockContext.Object);
            var actual = getOrders.Do("shipped").Orders.FirstOrDefault();
            actual.Should().NotBeNull();
            actual.Status.Should().Be(order.Status);
            actual.PhoneNumber.Should().BeEquivalentTo(order.PhoneNumber);
            actual.Email.Should().BeEquivalentTo(order.Email);
            actual.Status.Should().BeEquivalentTo(order.Status);
            actual.Total.Should().Be(order.Total);           
            actual.Note.Should().BeEquivalentTo(order.Note);
        }
    }
}
