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

        public ProductViewModel Do(int Id)
        {
            return _context.Inventory
                .Where(x => x.Id == Id)
                .Include(x => x.Product)
                .Select(pvm => new ProductViewModel
                {
                    Name = pvm.Product.Name,
                    Description = pvm.Product.Description,
                    ProductImage = Convert.FromBase64String(pvm.InventoryImage)
                }).FirstOrDefault();            
        }

        public class ProductViewModel
        {
            public string Name { get; set; }
            public byte[] ProductImage { get; set; }
            public string Description { get; set; }            
        }
    }
}