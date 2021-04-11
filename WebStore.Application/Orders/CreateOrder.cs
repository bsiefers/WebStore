using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe;
using WebStore.Database;
using WebStore.Models;
using WebStore.Application.Services.Payment;
namespace WebStore.Application.Orders
{
    public class CreateOrder
    {
        private static CreateOrder instance = null;

        private ApplicationDbContext _context;
        private PaymentService _paymentService;
        public CreateOrder(ApplicationDbContext context, PaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        private int CalculateOrderAmount(IEnumerable<CartItem> items)
        {
            int amount = 0;
            IEnumerable<int> inventoryIds = items.Select(item => item.Id);
            var inventory = _context.Inventory.Include(x => x.Product).Where(x => inventoryIds.Contains(x.Id));

            foreach (var item in inventory)
            {
                int quantity = items.Where(x => x.Id == item.Id).Select(x => x.Quantity).FirstOrDefault();
                amount += (int)(item.Price * 100) * quantity;
            }

            return amount;
        }

        public Response Do(Request request)
        {
            var ids = request.Cart.Select(x => x.Id).ToArray();
            var inventoryToUpdate = _context.Inventory.Where(x => ids.Contains(x.Id)).ToList();
            var token = request.Token;
            if (inventoryToUpdate == null || request.Token == null || request.Token == "")
                return new Response { Status = 400 };


            //update inventory
            foreach (var inventory in inventoryToUpdate)
            {
                if (inventory.Quantity - request.Cart.FirstOrDefault(x => x.Id == inventory.Id).Quantity < 0)
                    return new Response { Status = 409 };
                inventory.Quantity -= request.Cart.FirstOrDefault(x => x.Id == inventory.Id).Quantity;
            }

            //used to create payment for stripe
            var options = new ChargeCreateOptions
            {
                Amount = CalculateOrderAmount(request.Cart),
                Currency = "usd",
                Description = "Purchase at Webshop.",
                Source = token,
            };
            
            var charge = _paymentService.CreatePayment(options);

            var order = new Models.Order
            {
                StripeRef = charge.Id,
                FirstName = request.CustomerInformation.FirstName,
                LastName = request.CustomerInformation.LastName,
                Email = request.CustomerInformation.Email,
                PhoneNumber = request.CustomerInformation.Phone,
                Address1 = request.CustomerInformation.Address1,
                Address2 = request.CustomerInformation.Address2,
                City = request.CustomerInformation.City,
                PostCode = request.CustomerInformation.PostCode,
                State = request.CustomerInformation.State,
                Country = request.CustomerInformation.Country,
                OrderInventory = request.Cart.Select(x => new OrderInventory
                {
                    InventoryId = x.Id,
                    Quantity = x.Quantity
                }).ToList(),
                Total = CalculateOrderAmount(request.Cart) / 100.00,
                OrderDate = DateTime.UtcNow,
                Status = "ordered",
            };
            _context.Orders.Add(order);



            var response = _context.SaveChanges();
            
            return new Response { Status = 201};
        }

        public class CustomerInformation
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
            public string PostCode { get; set; }
        }

        public class CartItem
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
        }

        public class Request
        {
            public string Token { get; set; }            
            public CustomerInformation CustomerInformation { get; set; }
            public IEnumerable<CartItem> Cart { get; set; }            
        }

        public class Response
        {
            public int Status { get; set; }
        }
    }
}
