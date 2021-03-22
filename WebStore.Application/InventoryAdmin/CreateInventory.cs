using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebStore.Database;
using WebStore.Models;

namespace WebStore.Application.InventoryAdmin
{
    public class CreateInventory
    {
        ApplicationDbContext _context;
        public CreateInventory(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response> Do(Request req)
        {
            var inventory = new Inventory
            {
                ProductId = req.ProductId,
                Description = req.Description,
                Price = req.Price,
                Quantity = req.Quantity
            };

            if (req.InventoryImage == null || req.InventoryImage.Length == 0)
            {

                _context.Inventory.Add(inventory);
                await _context.SaveChangesAsync();

                return new Response
                {
                    Id = inventory.Id,
                    Description = inventory.Description,
                    Quantity = inventory.Quantity, 
                    Price = inventory.Price
                };
            }
            else
            {
                var allowExtentions = new List<string> { ".gif", ".png", ".jpg", ".jpeg" };
                int maxSize = 2 * 1024 * 1024; //2mb
                string fileName = req.InventoryImage.FileName;
                string extention = Path.GetExtension(fileName);

                if (!allowExtentions.Contains(extention) || req.InventoryImage.Length > maxSize)
                    return null;

                using (var ms = new MemoryStream())
                {
                    req.InventoryImage.CopyTo(ms);
                    var imageBytes = ms.ToArray();

                    string inventoryImage = Convert.ToBase64String(imageBytes);
                    inventory.InventoryImage = inventoryImage;

                    _context.Inventory.Add(inventory);
                    await _context.SaveChangesAsync();

                    return new Response
                    {
                        Id = inventory.Id,
                        Description = inventory.Description,
                        Quantity = inventory.Quantity,
                        Price = inventory.Price,
                        InventoryImage = "data:image/png;base64," + inventoryImage
                    };
                }
            }
        }

        public class Request
        {
            public int ProductId { get; set; }
            public string Description { get; set; }
            public int Quantity { get; set; }
            public double Price { get; set; }
            public IFormFile InventoryImage {get; set;}
        }

        public class Response
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public string InventoryImage { get; set; }
            public int Quantity { get; set; }
            public double Price { get; set; }
        }
    }
}
