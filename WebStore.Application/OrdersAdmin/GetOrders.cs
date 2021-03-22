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
    public class GetOrders
    {
        ApplicationDbContext _context;
        public GetOrders(ApplicationDbContext context)
        {
            _context = context;
        }

        public Response Do()
        {
            return new Response 
            {
                Orders =_context.Orders.Include(x => x.OrderInventory).Select(x => new OrderViewModel
                {
                    Id = x.Id,
                    Email = x.Email,
                    Phone = x.PhoneNumber,
                    Name = x.FirstName + " " + x.LastName,
                    Status = x.Status,
                    Total = x.Total,
                    OrderDate = x.OrderDate.ToString("MM/dd/yy HH:mm:ss"),
                    Note = x.Note
                })
            };
        }

        public class OrderViewModel {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Status { get; set; }
            public double Total { get; set; }                 
            public string OrderDate { get; set; }
            public string Note { get; set; }
        }

        public class Response
        {
            public IEnumerable<OrderViewModel> Orders { get; set; }
        }
    }
}