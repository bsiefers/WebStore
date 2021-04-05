using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebStore.Database;

namespace WebStore.Application.InventoryAdmin
{
    public class GetInventory
    {
        ApplicationDbContext _context;
        public GetInventory(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ProductViewModel> Do()
        {
            var inventory = _context.Products
                .Include(x => x.Inventory)
                .Select(x => new ProductViewModel 
                {
                    
                    Id= x.Id,
                    ProductName = x.Name,
                    Description = x.Description,
                    Inventory = x.Inventory.Select(y => new InventoryViewModel
                    {
                        Id = y.Id,
                        Description = y.Description,
                        ProductId = y.ProductId,
                        Quantity = y.Quantity,
                        Price = y.Price,
                        InventoryImage = "data:image/png;base64," + y.InventoryImage
                    }),

                })
                .ToList();
            return inventory;
        }

        public class InventoryViewModel
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
            public string InventoryImage { get; set; }
            public string Description { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
        }

        public class ProductViewModel
        {
            public int Id { get; set; }
            public string ProductName { get; set; } 
            public string Description{ get; set; }
            public string InventoryImage { get; set; }
            public IEnumerable<InventoryViewModel> Inventory { get; set; }
        }
    }
}
