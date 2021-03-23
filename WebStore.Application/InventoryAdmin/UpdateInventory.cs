using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebStore.Database;
using WebStore.Models;

namespace WebStore.Application.InventoryAdmin
{
    public class UpdateInventory
    {
        ApplicationDbContext _context;
        public UpdateInventory(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response> Do(Request req)
        {
            Inventory inventory = _context.Inventory.Where(x => x.Id == req.Id).FirstOrDefault();
            if (inventory == null)
                return new Response { Status = 404 };

            //if uploaded image is null then just updates non-image data
            if (req.InventoryImage == null || req.InventoryImage.Length == 0)
            {
                inventory.Quantity = req.Quantity;
                inventory.Description = req.Description;
                inventory.Price = req.Price;
                await _context.SaveChangesAsync();
                return new Response
                {
                    Status = 200,
                    Inventory = new InventoryViewModel
                    {
                        Id = inventory.Id,
                        Quantity = inventory.Quantity,
                        Description = inventory.Description,
                        Price = inventory.Price
                    }
                };
            }
            else
            {
                var allowExtentions = new List<string> { ".gif", ".png", ".jpg", ".jpeg" };
                int maxSize = 2 * 1024 * 1024; //2mb
                string fileName = req.InventoryImage.FileName;
                string extention = Path.GetExtension(fileName);

                if (!allowExtentions.Contains(extention) || req.InventoryImage.Length > maxSize)
                    return new Response { Status = 400 };
                //copies image to memory and stores it as a 64-bit string in the database
                using (var ms = new MemoryStream())
                {
                    req.InventoryImage.CopyTo(ms);
                    var imageBytes = ms.ToArray();
                    string inventoryImage = Convert.ToBase64String(imageBytes);

                    inventory.Quantity = req.Quantity;
                    inventory.Description = req.Description;
                    inventory.InventoryImage = inventoryImage;
                    inventory.Price = req.Price;
                    await _context.SaveChangesAsync();

                    return new Response
                    {
                        Status = 200,
                        Inventory =new InventoryViewModel {
                            Id = inventory.Id,
                            Quantity = inventory.Quantity,
                            Description = inventory.Description,
                            Price = inventory.Price,
                            InventoryImage = "data:image/png;base64," + inventoryImage
                        }
                    };
                }
            }
        }


        public class Request
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
            public string Description { get; set; }
            public double Price { get; set; }
            
            public int Quantity { get; set; }

            public IFormFile InventoryImage { get; set; }
        }

        public class InventoryViewModel
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
            public string Description { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
            public string InventoryImage { get; set; }
        }

        public class Response
        {
            public int Status { get; set; }
            public InventoryViewModel Inventory { get; set; }
        }
    }
}
