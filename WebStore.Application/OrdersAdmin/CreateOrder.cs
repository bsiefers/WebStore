using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Database;
using WebStore.Models;

namespace WebStore.Application.OrdersAdmin
{
    public class CreateOrder
    {
        private ApplicationDbContext _context;
        public CreateOrder(ApplicationDbContext context)
        {
            _context = context;
        }
        private int CalculateOrderAmount(IEnumerable<CartItem> items)
        {
            int amount = 0;
            IEnumerable<int> inventoryIds = items.Select(item => item.Id);
            var inventory = _context.Inventory.Include(x => x.Product).Where(x => inventoryIds.Contains(x.Id));

            foreach (var item in inventory)
            {
                int quantity = items.Where(x => x.Id == item.Id).Select(x => x.Quantity).FirstOrDefault();
                amount += (int)(item.Product.Price * 100) * quantity;
            }

            return amount;
        }

        public async Task<Response> Do(Request req)
        {
            var order = new Order
            {                
                FirstName = req.CustomerInformation.FirstName,
                LastName = req.CustomerInformation.LastName,
                Email = req.CustomerInformation.Email,
                PhoneNumber = req.CustomerInformation.Phone,
                Address1 = req.CustomerInformation.Address1,
                Address2 = req.CustomerInformation.Address2,
                City = req.CustomerInformation.City,
                PostCode = req.CustomerInformation.PostCode,
                State = req.CustomerInformation.State,
                Country = req.CustomerInformation.Country,
                OrderInventory = req.Cart.Select(x => new OrderInventory
                {
                    InventoryId = x.Id,                                      
                    Quantity = x.Quantity
                }).ToList(),
                Status = "Ordered",
                OrderDate = DateTime.UtcNow,
                Total = req.Total,
                Note = req.Note
            };
            IEnumerable<int> inventoryIds = req.Cart.Select(item => item.Id);
            var inventory = _context.Inventory.Include(x => x.Product).Where(x => inventoryIds.Contains(x.Id));

            foreach (var item in inventory)
            {
                int quantity = req.Cart.Where(x => x.Id == item.Id).Select(x => x.Quantity).FirstOrDefault();
                if (quantity <= item.Quantity)
                    item.Quantity -= quantity;
                else
                    return null;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new Response
            {
                Id = order.Id,
                Email = order.Email,
                Phone = order.PhoneNumber,
                Name = order.FirstName + " " + order.LastName,
                Status = order.Status,
                Total = order.Total,
                OrderDate = order.OrderDate.ToString("MM/dd/yy HH:mm:ss"),
                Note = order.Note
            };
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
            public string PostCode { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
        }
        public class CartItem
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
        }

         
        public class Request
        {            
            public double Total { get; set; }
            public string Status { get; set; }
            public CustomerInformation CustomerInformation { get; set; }
            public IEnumerable<CartItem> Cart { get; set; }
            public string Note { get; set; }
        }

        public class Response
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Status { get; set; }
            public double Total { get; set; }
            public string OrderDate { get; set; }
            public string Note { get; set; }
        }
    }
}
