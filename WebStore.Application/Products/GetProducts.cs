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

        public IEnumerable<ProductViewModel> Do()
        {
           return _context.Products.Include(x => x.Inventory).ToList().Where(x => x.Inventory != null && x.Inventory.Count > 0).Select(pvm => new ProductViewModel
           {
               Name = pvm.Name,
               Description = pvm.Description,
               ProductImage = pvm.ProductImage != null ?
                                "data:image/png;base64," + pvm.ProductImage :
                                null
           });
        }
        public class ProductViewModel
        {
            public string Name { get; set; }
            public string Description { get; set; }            
            public string ProductImage { get; set; }
        }
    }
}
