using Autofac.Extras.Moq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Application.Orders;
using WebStore.Application.Services.Payment;
using WebStore.Database;
using WebStore.Models;

namespace WebStore.Tests.UnitTests.Orders
{
    [TestClass]
    public class CreateOrderTest
    {
        [TestMethod]
        public void TestCreateOrder()
        {
            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();
            
            
            var request = new CreateOrder.Request
            {                
                CustomerInformation = new CreateOrder.CustomerInformation
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
                },
                Cart = new List<CreateOrder.CartItem>
                {
                    new CreateOrder.CartItem
                    {
                        Id = 1,
                        Quantity = 1
                    }
                },
                Token = "Test Token"
            };

            Inventory inventory = new Inventory
            {
                Id = 1,
                Description = "Test Inventory Description",
                Price = 12.99,
                ProductId = 1,
                Quantity = 10
            };

            var options = new ChargeCreateOptions
            {
                Amount = 0,
                Currency = "usd",
                Description = "Purchase at Webshop.",
                Source = "test token",
            };


            var inventoryData = new List<Inventory> { inventory }.AsQueryable();
            var mockInventorySet = new Mock<DbSet<Inventory>>();
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(inventoryData.Provider);
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventoryData.Expression);
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventoryData.ElementType);
            mockInventorySet.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(() => inventoryData.GetEnumerator());
            mockContext.Setup(m => m.Inventory).Returns(mockInventorySet.Object);

            var orderData = new List<Models.Order> { }.AsQueryable();
            var mockOrderSet = new Mock<DbSet<Models.Order>>();
            mockOrderSet.As<IQueryable<Models.Order>>().Setup(m => m.Provider).Returns(orderData.Provider);
            mockOrderSet.As<IQueryable<Models.Order>>().Setup(m => m.Expression).Returns(orderData.Expression);
            mockOrderSet.As<IQueryable<Models.Order>>().Setup(m => m.ElementType).Returns(orderData.ElementType);
            mockOrderSet.As<IQueryable<Models.Order>>().Setup(m => m.GetEnumerator()).Returns(() => orderData.GetEnumerator());
            mockContext.Setup(m => m.Orders).Returns(mockOrderSet.Object);


            var mockChargeService = new Mock<PaymentService>();
            CreateOrder createOrder = new CreateOrder(mockContext.Object, mockChargeService.Object);
            var charge = new Charge();
            charge.Id = "Test Stripe Ref";
            mockChargeService
            .Setup(x => x.CreatePayment(It.IsAny<ChargeCreateOptions>()))
            .Returns(charge);

            mockChargeService.Object.CreatePayment(options);




            var response = createOrder.Do(request);
            response.Status.Should().Be(201);
            request.Cart.First().Quantity = 11;
            response = createOrder.Do(request);
            response.Status.Should().Be(409);
            

        }
    }
}
