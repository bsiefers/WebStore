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

        private bool ValidateRequest(Request request)
        {

            if (request == null)
                return false;
            //cart validation
            if (request.Cart == null || request.Cart.Count() == 0)
                return false;

            //custmer info validation
            if (request.CustomerInformation == null)
                return false;

            if (request.CustomerInformation.FirstName == null || request.CustomerInformation.FirstName == "" ||
                request.CustomerInformation.LastName == null || request.CustomerInformation.LastName == "")
                return false;

            if (request.CustomerInformation.PostCode == null || request.CustomerInformation.PostCode == "" ||
                request.CustomerInformation.State == null || request.CustomerInformation.State == "")
                return false;

            if (request.CustomerInformation.Country == null || request.CustomerInformation.Country == "" ||
                request.CustomerInformation.Address1 == null || request.CustomerInformation.Address1 == "")
                return false;

            if (request.CustomerInformation.Email == null || request.CustomerInformation.Email == "")
                return false;

            //Order validation
            var validStatuses = new[] { "ordered", "shipped", "fufilled" };
            if (request.Status == null || !(validStatuses.Contains(request.Status.ToLower())))
                return false;
            
            return true;
        }

        public async Task<Response> Do(Request req)
        {

            if (!ValidateRequest(req))
                return new Response { Status = 400 };
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

            //get ids their inventories update the add items
            IEnumerable<int> inventoryIds = req.Cart.Select(item => item.Id);
            var inventory = _context.Inventory.Include(x => x.Product).Where(x => inventoryIds.Contains(x.Id));
            //for each item if not enough stock return null
            foreach (var item in inventory)
            {
                int quantity = req.Cart.Where(x => x.Id == item.Id).Select(x => x.Quantity).FirstOrDefault();
                if (quantity <= item.Quantity)
                    item.Quantity -= quantity;
                else
                    return new Response { Status = 409 };
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new Response
            {
                Status = 201,
                Order = new OrderViewModel
                {
                    Id = order.Id,
                    Email = order.Email,
                    Phone = order.PhoneNumber,
                    Name = order.FirstName + " " + order.LastName,
                    Status = order.Status,
                    Total = order.Total,
                    OrderDate = order.OrderDate.ToString("MM/dd/yy HH:mm:ss"),
                    Note = order.Note
                }
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

         
        public class OrderViewModel
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
            public OrderViewModel Order { get; set; }
            public int Status { get; set; }
        }
    }
}
