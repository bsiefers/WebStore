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
    public class UpdateOrderTest
    {
        [TestMethod]
        public void TestUpdateOrder()
        {
            Random random = new Random();
            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();

            var request = new UpdateOrder.Request
            {
                Total = random.NextDouble(),
                Status = "shipped",
                Note = StringUtilities.CreateRandomString(0, 5000),
                Cart = new List<UpdateOrder.CartItem> { new UpdateOrder.CartItem { Id = 1, Quantity = 1 } },
                CustomerInformation = new UpdateOrder.CustomerInformation
                {
                    Email = "test@test.com",
                    FirstName = StringUtilities.CreateRandomString(0, 50),
                    LastName = StringUtilities.CreateRandomString(0, 50),
                    Address1 = StringUtilities.CreateRandomString(0, 500),
                    Address2 = StringUtilities.CreateRandomString(0, 500),
                    City = StringUtilities.CreateRandomString(0, 500),
                    Country = StringUtilities.CreateRandomString(0, 500),
                    PostCode = "23432",
                    Phone = "342 342 3244",
                    State = StringUtilities.CreateRandomString(0, 100)
                }
            };

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
                StripeRef  = "TestRef",
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

            mockContext.Setup(m => m.Orders).Returns(mockOrderSet.Object);
            mockContext.Setup(m => m.Inventory).Returns(mockInventorySet.Object);

            var UpdateOrder = new UpdateOrder(mockContext.Object);

        }

        public void RunValidTest(UpdateOrder UpdateOrder, UpdateOrder.Request request)
        {
            var actual = UpdateOrder.Do(request).Result;
            actual.Should().NotBeNull();
            actual.Status.Should().Be(201);
            actual.Order.Should().NotBeNull();
            actual.Status.Should().Be(201);
            actual.Order.Phone.Should().BeEquivalentTo(request.CustomerInformation.Phone);
            actual.Order.Email.Should().BeEquivalentTo(request.CustomerInformation.Email);
            actual.Order.Status.Should().BeEquivalentTo(request.Status);
            actual.Order.Total.Should().Be(request.Total);
            actual.Order.OrderDate.Should().NotBeNull();
            actual.Order.Note.Should().BeEquivalentTo(request.Note);

        }

        public void RunBadRequest(UpdateOrder UpdateOrder, UpdateOrder.Request request)
        {
            var actual = UpdateOrder.Do(request).Result;
            actual.Should().NotBeNull();
            actual.Status.Should().NotBe(201);
            Assert.IsTrue(actual.Status == 400 || actual.Status == 409);
            actual.Order.Should().BeNull();
        }
    }
}
