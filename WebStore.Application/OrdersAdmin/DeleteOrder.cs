using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Database;

namespace WebStore.Application.OrdersAdmin
{
    public class DeleteOrder
    {
        ApplicationDbContext _context;
        public DeleteOrder(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Response> Do(int Id)
        {            
            var order = _context.Orders.Where(x => x.Id == Id).FirstOrDefault();
            if (order == null)
                return new Response { Status = 404 };
            Dictionary<int, int> orderedItems = new Dictionary<int, int>();
            foreach(var orderInventory in order.OrderInventory)
            {
                orderedItems.Add(orderInventory.InventoryId, orderInventory.Quantity);
            }            
            var inventories = _context.Inventory.Where(x => orderedItems.Keys.Contains(x.Id));
            foreach(var inventory in inventories)
            {
                inventory.Quantity += orderedItems[inventory.Id];
            }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return new Response { Status = 200 };
        }

        public class Response
        {
            public int Status { get; set; }
        }
    }
}
