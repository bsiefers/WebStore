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
    public class GetOrder
    {
        ApplicationDbContext _context;
        public GetOrder(ApplicationDbContext context)
        {
            _context = context;
        }

        public Response Do(int Id)
        {
            var order = _context.Orders
                 .Where(x => x.Id == Id)
                 .Include(x => x.OrderInventory).Select(x =>
                     new Response
                     {
                         //Id
                         Id = x.Id,
                         StripeRef = x.StripeRef,
                         //customer info
                         FirstName = x.FirstName,
                         LastName = x.LastName,
                         Email = x.Email,
                         PhoneNumber = x.PhoneNumber,
                         Address1 = x.Address1,
                         Address2 = x.Address2,
                         City = x.City,
                         PostCode = x.PostCode,
                         Status = x.Status,
                         Total = x.Total,
                         OrderDate = x.OrderDate.ToString("MM/dd/yy HH:mm:ss"),
                         Note = x.Note,
                         State = x.State,
                         Country = x.Country,
                         OrderInventory = x.OrderInventory.Select(y => new OrderInventoryViewModel{
                             InventoryId = y.InventoryId,
                             ProductName = y.Inventory.Product.Name,
                             Description = y.Inventory.Description,
                             Quantity = y.Quantity
                         })
                     }).FirstOrDefault();
                    
            return order;
        }
        public class OrderInventoryViewModel
        {
            public string ProductName { get; set; }
            public string Description { get; set; }
            public int InventoryId { get; set; }
            public int Quantity { get; set; }
        }
        public class Response
        {
            //Id
            public int Id { get; set; }
            public string StripeRef { get; set; }

            //customer info
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string PostCode { get; set; }
            public string State { get; set; }
            public string Country { get; set; }

            //meta-data
            public string Note { get; set; }
            public string Status { get; set; }
            public double Total { get; set; }
            public string OrderDate { get; set; }
            public IEnumerable<OrderInventoryViewModel> OrderInventory { get; set; }
        }
    }
}
