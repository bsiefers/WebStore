using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebStore.Database;

namespace WebStore.Application.ProductsAdmin
{
    public class GetProduct
    {
        private ApplicationDbContext _context;
        public GetProduct(ApplicationDbContext context)
        {
            _context = context;            
        }


        public ProductViewModel Do(int Id)
        {
            return _context.Products.Where(pvm => pvm.Id == Id).Select(pvm => new ProductViewModel
            {
                Id = pvm.Id,
                Name = pvm.Name,
                Description = pvm.Description,
                ProductImage = "data:image/png;base64," + pvm.ProductImage
            }).FirstOrDefault();
        }
        public class ProductViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ProductImage { get; set; }
            public string Description { get; set; }            
        }
    }

}
