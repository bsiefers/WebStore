using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WebStore.Database;
using WebStore.Models;

namespace WebStore.Application.ProductsAdmin
{
    public class CreateProduct
    {
        private ApplicationDbContext _context;
        public CreateProduct(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<Response> Do(Request req)
        {


            var product = new Product
            {
                Name = req.Name,
                Description = req.Description
            };
            //if uploaded image is null then just updates non-image data
            if (req.ProductImage == null || req.ProductImage.Length == 0)
            {

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return new Response
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description
                };
            }
            else
            {
                var allowExtentions = new List<string> { ".gif", ".png", ".jpg", ".jpeg" };
                int maxSize = 2 * 1024 * 1024; //2mb
                string fileName = req.ProductImage.FileName;
                string extention =  Path.GetExtension(fileName);

                if (!allowExtentions.Contains(extention) || req.ProductImage.Length > maxSize)
                    return null;
                //copies to memory and stores it as a 64-bit string in the database
                using (var ms = new MemoryStream())
                {
                    req.ProductImage.CopyTo(ms);
                    var imageBytes = ms.ToArray();
                    string productImage = Convert.ToBase64String(imageBytes);
                    product.ProductImage = productImage;

                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    return new Response
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        ProductImage = "data:image/png;base64," + imageBytes
                    };
                }
            }
        }

        public class Request
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public IFormFile ProductImage { get; set; }
        }

        public class Response
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }            
            public string ProductImage { get; set; }
        }
    }


}
