using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebStore.Database;

namespace WebStore.Application.Products
{
    public class GetProduct
    {
        ApplicationDbContext _context;
        public GetProduct(ApplicationDbContext context)
        {
            _context = context;
        }

        public Response Do(string name)
        {
           var pvm = _context.Products
                .Where(x => x.Name == name)
                .Include(x => x.Inventory)
                .Select(pvm => new ProductViewModel
           {
               Name = pvm.Name,
               Description = pvm.Description,
               ProductImage = pvm.ProductImage != null ?
                                "data:image/png;base64," + pvm.ProductImage :
                                null,
               Inventory = pvm.Inventory.Select(y => new InventoryViewModel
               {
                   Id = y.Id,
                   Description = y.Description,
                   Price = y.Price.ToString("N2"),
                   InventoryImage = y.InventoryImage != null ?
                                "data:image/png;base64," + pvm.ProductImage :
                                null,
                   Stock = y.Quantity
               })
           }).FirstOrDefault();

            if(pvm == null)
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
            public string Description { get; set; }   
            public string ProductImage { get; set; }
            public IEnumerable<InventoryViewModel> Inventory { get; set; }
        }

        public class InventoryViewModel
        {
            public int Id { get; set; }
            public string InventoryImage { get; set; }
            public string Price { get; set; }
            public string Description { get; set; }
            public int Stock { get; set; }
        }
        public class Response
        {
            public int Status { get; set; }
            public ProductViewModel Product { get; set; }
        }
    }
}
