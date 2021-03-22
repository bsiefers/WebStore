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
    public class GetOrderByStatus
    {
        ApplicationDbContext _context;
        public GetOrderByStatus(ApplicationDbContext context)
        {
            _context = context;
        }

        public Response Do(string status)
        {

            return new Response
            {
                Order = _context.Orders
                 .Where(x => x.Status == status)
                 .Include(x => x.OrderInventory)
            };
        }

        public class Response
        {
            public IEnumerable<Order> Order { get; set; }
        }
    }
}
