using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Database;
using WebStore.Models;

namespace WebStore.Application.ProductsAdmin
{
    public class UpdateProduct
    {
        private ApplicationDbContext _context;
        public UpdateProduct(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response> Do(Request req)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == req.Id);
            if (product == null)
                return new Response { Status = 404 };
            if (req.ProductImage == null || req.ProductImage.Length == 0)
            {
                product.Name = req.Name;
                product.Description = req.Description;

                await _context.SaveChangesAsync();
                return new Response
                {
                    Status = 200,
                    Product = new ProductViewModel
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description
                    }
                };
            }
            else
            {
                var allowExtentions = new List<string> { ".gif", ".png", ".jpg", ".jpeg" };
                int maxSize = 2 * 1024 * 1024; //2mb
                string fileName = req.ProductImage.FileName;
                string extention = Path.GetExtension(fileName);

                if (!allowExtentions.Contains(extention) || req.ProductImage.Length > maxSize)
                    return new Response { Status = 400};

                using (var ms = new MemoryStream())
                {
                    req.ProductImage.CopyTo(ms);
                    var imageBytes = ms.ToArray();
                    string productImage = Convert.ToBase64String(imageBytes);

                    product.Name = req.Name;
                    product.Description = req.Description;
                    product.ProductImage = productImage;

                    await _context.SaveChangesAsync();

                    return new Response
                    {
                        Status = 200,
                        Product = new ProductViewModel
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Description = product.Description,
                            ProductImage = "data:image/png;base64," + Convert.ToBase64String(imageBytes)
                        }
                    };
                }
            }
        }

        public class Request
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public IFormFile ProductImage { get; set; }
        }

        public class ProductViewModel{
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ProductImage { get; set; }
        }

        public class Response
        {        
            public int Status { get; set; }

            public ProductViewModel Product { get; set; }
        }
    }


}
