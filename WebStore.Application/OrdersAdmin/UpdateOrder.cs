using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Database;
using WebStore.Models;

namespace WebStore.Application.OrdersAdmin
{
    public class UpdateOrder
    {
        ApplicationDbContext _context;
        public UpdateOrder(ApplicationDbContext context)
        {
            _context = context;
        }


        private bool isInventoryAvailable(int Id, int quantity)
        {
             return quantity <= _context.Inventory.Where(x => x.Id == Id).FirstOrDefault().Quantity;
        }
        public async Task<Response> Do(Request request)
        {
            var order = _context.Orders.FirstOrDefault(x => x.Id == request.OrderId);
            if (order == null)
                return new Response { Status = 404 };
            order.FirstName = request.CustomerInformation.FirstName;
            order.LastName = request.CustomerInformation.LastName;
            order.Email = request.CustomerInformation.Email;
            order.PhoneNumber = request.CustomerInformation.Phone;
            order.Address1 = request.CustomerInformation.Address1;
            order.Address2 = request.CustomerInformation.Address2;
            order.City = request.CustomerInformation.City;
            order.Status = request.Status;
            order.PostCode = request.CustomerInformation.PostCode;
            order.State = request.CustomerInformation.State;
            order.Country = request.CustomerInformation.Country;
            order.Note = request.Note;
            order.Total = request.Total;

            List<OrderInventory> newItems = new List<OrderInventory>();
            foreach(var item in request.Cart)
            {
                var quantityDiff = 0;

                var foundItem = _context.OrderInventory
                    .Where(x => x.OrderId == order.Id && x.InventoryId == item.InventoryId)
                    .FirstOrDefault();
                if (foundItem == null)
                {
                    quantityDiff -= item.Quantity;
                    newItems.Add(new OrderInventory
                    {
                        OrderId = order.Id,
                        InventoryId = item.InventoryId,
                        Quantity = item.Quantity
                    });
                }
                else
                {
                    quantityDiff = item.Quantity - foundItem.Quantity;
                    foundItem.Quantity += quantityDiff;
                    newItems.Add(foundItem);
                }
                if (!isInventoryAvailable(item.InventoryId, quantityDiff))
                    return new Response { Status = 409 };
                var inventory = _context.Inventory.Where(x => x.Id == item.InventoryId).FirstOrDefault();
                inventory.Quantity += quantityDiff;
            }
            order.OrderInventory = newItems;
            
            await _context.SaveChangesAsync();

            return new Response
            {
                Status = 200,
                Order = new OrderViewModel
                {
                    Id = order.Id,
                    Name = order.FirstName + " " + order.LastName,
                    Email = order.Email,
                    Phone = order.PhoneNumber,
                    orderDate = order.OrderDate.ToString("MM/dd/yy HH:mm:ss"),
                    Total = order.Total,
                    Status = order.Status
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

        public class OrderViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }            
            public string Note { get; set; }
            public string orderDate { get; set; }
            public double Total { get; set; }
            public string Status { get; set; }
        }
        public class Cart
        {
            public int InventoryId { get; set; }
            public int Quantity { get; set; }
        }


        public class Request
        {
            public int OrderId { get; set; }
            public string Status { get; set; }
            public string Note { get; set; }
            public double Total { get; set; }
            public CustomerInformation CustomerInformation { get; set; }
            public IEnumerable<Cart> Cart { get; set; }
        }

        public class Response
        {
            public int Status { get; set; }
            public OrderViewModel Order { get; set; }
        }
    }
}
