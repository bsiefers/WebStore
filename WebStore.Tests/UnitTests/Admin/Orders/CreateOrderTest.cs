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
    public class CreateOrderTest
    {
        [TestMethod]
        public void TestCreateOrder()
        {
            Random random = new Random();
            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();

            var request = new CreateOrder.Request
            {
                Total = random.NextDouble(),
                Status = "shipped",
                Note = StringUtilities.CreateRandomString(0, 5000),
                Cart = new List<CreateOrder.CartItem> { new CreateOrder.CartItem { Id = 1, Quantity = 1 } },
                CustomerInformation = new CreateOrder.CustomerInformation
                {
                    Email = "test@test.com",
                    FirstName = StringUtilities.CreateRandomString(0,50),
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
            var inventoryData = new List<Inventory> { inventory }.AsQueryable();
            var mockInventorySet = new Mock<DbSet<Inventory>>();
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(inventoryData.Provider);
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventoryData.Expression);
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventoryData.ElementType);
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(() => inventoryData.GetEnumerator());

            mockContext.Setup(m => m.Inventory).Returns(mockInventorySet.Object);

            var createOrder = new CreateOrder(mockContext.Object);

        }

        public void RunValidTest(CreateOrder createOrder, CreateOrder.Request request)
        {
            var actual = createOrder.Do(request).Result;
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

        public void RunBadRequest(CreateOrder createOrder, CreateOrder.Request request)
        {
            var actual = createOrder.Do(request).Result;
            actual.Should().NotBeNull();
            actual.Status.Should().NotBe(201);
            Assert.IsTrue(actual.Status == 400 || actual.Status == 409);
            actual.Order.Should().BeNull();
        }
    }
}
