using WebStore.Database;

using System.Linq;
using System.Threading.Tasks;

namespace WebStore.Application.InventoryAdmin
{
    public class DeleteInventory
    {

        ApplicationDbContext _context;
        public DeleteInventory(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response> Do(int Id)
        {
            var inventory = _context.Inventory.FirstOrDefault(x => x.Id == Id);
            if (inventory == null)
                return new Response { Status = 404 };
            _context.Inventory.Remove(inventory);
            await _context.SaveChangesAsync();
            return new Response { Status = 200 };
        }

        public class Response {
            public int Status { get; set; }
        }
    }
}
