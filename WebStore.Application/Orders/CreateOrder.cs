using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Database;
using WebStore.Models;

namespace WebStore.Application.Orders
{
    public class CreateOrder
    {
        private static CreateOrder instance = null;
        private static readonly object padlock = new object();

        CreateOrder()
        {
        }

        public static CreateOrder Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new CreateOrder();
                    }
                    return instance;
                }
            }
        }


        ApplicationDbContext _context;
        public CreateOrder(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Do(Request request)
        {
            lock (padlock)
            {
                var ids = request.Inventory.Select(x => x.InventoryId).ToArray();
                var inventoryToUpdate = _context.Inventory.Where(x => ids.Contains(x.Id)).ToList();
                foreach (var inventory in inventoryToUpdate)
                {
                    if (inventory.Quantity - request.Inventory.FirstOrDefault(x => x.InventoryId == inventory.Id).Quantity < 0)
                        return false;
                    inventory.Quantity -= request.Inventory.FirstOrDefault(x => x.InventoryId == inventory.Id).Quantity;
                }
                var order = new Order
                {
                    
                    StripeRef = request.StripeReference,
                    FirstName = request.CustomerInformation.FirstName,
                    LastName = request.CustomerInformation.LastName,
                    Email = request.CustomerInformation.Email,
                    PhoneNumber = request.CustomerInformation.Phone,
                    Address1 = request.CustomerInformation.Address1,
                    Address2 = request.CustomerInformation.Address2,
                    City = request.CustomerInformation.City,
                    PostCode = request.CustomerInformation.PostCode,
                    OrderInventory = request.Inventory.Select(x => new OrderInventory
                    {
                        InventoryId = x.InventoryId,
                        Quantity = x.Quantity
                    }).ToList(),
                    Total = request.Total,
                    OrderDate = DateTime.UtcNow,
                    Status = "ordered",

                };
                _context.Orders.Add(order);
                var response = _context.SaveChanges();
            }
            return true;
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
        }

        public class Inventory
        {
            public int InventoryId { get; set; }
            public int Quantity { get; set; }
        }

        public class Request
        {
            public string StripeReference { get; set; }
            public CustomerInformation CustomerInformation { get; set; }
            public IEnumerable<Inventory> Inventory { get; set; }
            public double Total { get;  set; }
        }
    }
}
