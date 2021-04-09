using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebStore.Database;

namespace WebStore.Application.Products
{
    public class GetProducts
    {
        ApplicationDbContext _context;
        public GetProducts(ApplicationDbContext context)
        {
            _context = context;
        }

        public Response Do()
        {
           var pvm = _context.Products.Include(x => x.Inventory).ToList()
                .Where(x => x.Inventory != null && x.Inventory.Count > 0)
                .Select(pvm => new ProductViewModel
           {
               Name = pvm.Name,
               Description = pvm.Description,
               ProductImage = pvm.ProductImage != null ?
                                "data:image/png;base64," + pvm.ProductImage :
                                null
           });
            return new Response
            {
                Products = pvm,
                Status = 200
            };
        }
        public class ProductViewModel
        {
            public string Name { get; set; }
            public string Description { get; set; }            
            public string ProductImage { get; set; }
        }
        public class Response
        {
            public IEnumerable<ProductViewModel> Products { get; set; }
            public int Status { get; set; }
        }
    }
}
