using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebStore.Database;

namespace WebStore.Application.Products
{
    public class GetProductByInventoryId
    {
        ApplicationDbContext _context;
        public GetProductByInventoryId(ApplicationDbContext context)
        {
            _context = context;
        }

        public Response Do(int Id)
        {
            var product = _context.Inventory
                .Where(x => x.Id == Id)
                .Include(x => x.Product).FirstOrDefault();
            ProductViewModel pvm = null;
            if(product != null)
            {
                 pvm = new ProductViewModel
                {
                    Name = product.Product.Name,
                    Description = product.Product.Description,
                    ProductImage = product.Product.ProductImage != null ?
                                    "data:image/png;base64," + 
                                    product.Product.ProductImage :
                                    null
                };
            }

            if (pvm == null)
            {
                return new Response { Status = 404 };
            }
            else
            {
                return new Response { Product = pvm, Status = 200 };
            }
        }

        public class ProductViewModel
        {
            public string Name { get; set; }
            public string ProductImage { get; set; }
            public string Description { get; set; }            
        }
        public class Response
        {
            public int Status { get; set; }
            public ProductViewModel Product { get; set; }
        }
    }
}