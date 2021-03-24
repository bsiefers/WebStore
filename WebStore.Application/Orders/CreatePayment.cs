using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Database;

namespace WebStore.Application.Orders
{
    public sealed class CreatePayment
    {

        ApplicationDbContext _context;
        public CreatePayment(ApplicationDbContext context)
        {
            _context = context;
        }


        private int CalculateOrderAmount(IEnumerable<CartItem> items)
        {
            int amount = 0;
            IEnumerable<int> inventoryIds = items.Select(item => item.Id);
            var inventory = _context.Inventory.Include(x => x.Product).Where(x => inventoryIds.Contains(x.Id));

            foreach(var item in inventory)
            {
                int quantity = items.Where(x => x.Id == item.Id).Select(x => x.Quantity).FirstOrDefault();
                amount += (int)(item.Price * 100) * quantity;
            }

            return amount;
        }

        public Response Do(Request request)
        {
            var token = request.Token;
            //used to create payment for stripe
            var options = new ChargeCreateOptions
            {
                Amount = CalculateOrderAmount(request.Cart),
                Currency = "usd",
                Description = "Purchase at Webshop.",
                Source = token,
            };
            var service = new ChargeService();
            var charge = service.Create(options);


            new CreateOrder(_context).Do(new CreateOrder.Request
            {
                CustomerInformation = new CreateOrder.CustomerInformation { 
                    FirstName = request.CustomerInformation.FirstName,
                    LastName = request.CustomerInformation.LastName,
                    Email = request.CustomerInformation.Email,
                    Phone = request.CustomerInformation.Phone,
                    Address1 = request.CustomerInformation.Address1,
                    Address2 = request.CustomerInformation.Address2,
                    City = request.CustomerInformation.City,
                    PostCode = request.CustomerInformation.PostCode,
                    State = request.CustomerInformation.State,
                    Country = request.CustomerInformation.Country
                },
                StripeReference = charge.Id,
                Inventory = request.Cart.Select(x => new CreateOrder.Inventory
                {
                    InventoryId = x.Id,
                    Quantity = x.Quantity
                }).ToList(),
                Total = CalculateOrderAmount(request.Cart)/100.00
            });

            return new Response {
                Success = true
            };
        }

        public class CartItem
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
        }

        public class CustomerInformation
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string PostCode { get; set; }
        }

        public class Request
        {
            public string Token { get; set; }
            public CustomerInformation CustomerInformation { get; set; }
            public IEnumerable<CartItem> Cart { get; set; }
        }


        public class Response
        {
            public bool Success { get; set; }
        }
    }
}
