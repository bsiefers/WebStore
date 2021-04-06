using WebStore.Database;
using WebStore.Models;
using System.Linq;
using System.Threading.Tasks;


namespace WebStore.Application.ProductsAdmin
{
    public class DeleteProduct
    {
        private ApplicationDbContext _context;
        public DeleteProduct(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<Response> Do(int Id)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == Id);
            if (product == null)
                return new Response { Status = 404 };
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return new Response { Status = 200 };
        }
        public class Response 
        { 
            public int Status { get; set; } 
        }
    }


}
