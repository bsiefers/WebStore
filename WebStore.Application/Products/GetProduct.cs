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

        public ProductViewModel Do(string name)
        {
           return _context.Products
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
    }
}
