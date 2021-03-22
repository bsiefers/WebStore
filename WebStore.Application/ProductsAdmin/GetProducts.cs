using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebStore.Database;

namespace WebStore.Application.ProductsAdmin
{
    public class GetProducts
    {
        private ApplicationDbContext _context;
        public GetProducts(ApplicationDbContext context)
        {
            _context = context;            
        }


        public Response Do()
        {
            return new Response
            {
                Status = 200,
                PVM = _context.Products.Select(pvm =>
                    new ProductViewModel
                    {
                        Id = pvm.Id,
                        Name = pvm.Name,
                        Description = pvm.Description,
                        ProductImage = pvm.ProductImage != null ?
                                "data:image/png;base64," + pvm.ProductImage :
                                null
                    }
                ).ToList()
            };
            
        }
        public class ProductViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ProductImage { get; set; }
            public string Description { get; set; }            
        }
        public class Response
        {
            public int Status { get; set; }
            public IEnumerable<ProductViewModel> PVM { get; set; }
        }
    }

}
