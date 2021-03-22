using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebStore.Database;

namespace WebStore.Application.Orders
{
    public class GetOrder
    {
        ApplicationDbContext _context;
        public GetOrder(ApplicationDbContext context)
        {
            _context = context;
        }

        public Response Do(int Id)
        {
            return _context.Orders
                .Where(x => x.Id == Id)
                .Include(x => x.OrderInventory)
                .ThenInclude(x => x.Inventory)
                .ThenInclude(x => x.Product)
                .Select(x => new Response
                {
                    
                    StripeRef = x.StripeRef,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    Address1 = x.Address1,
                    Address2 = x.Address2,
                    City = x.City,
                    PostCode = x.PostCode,
                    Products = x.OrderInventory.Select(y => new Product
                    {
                        Name = y.Inventory.Product.Name,
                        Description = y.Inventory.Product.Description,
                        Price =  $"$ {y.Inventory.Product.Price.ToString("N2")}",
                        Quantity = y.Quantity,
                        InventoryDescription = y.Inventory.Description
                    }),
                    TotalPrice = x.OrderInventory.Sum(y => y.Inventory.Product.Price * y.Quantity).ToString("N2")
                }).FirstOrDefault();
        }

        public class Response
        {                
            public int Id { get; set; }
            
            public string StripeRef { get; set; }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }

            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string PostCode { get; set; }

            public IEnumerable<Product> Products { get; set; }

            public string TotalPrice { get; set; }
        }

        public class Product
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Price { get; set; }
            public int Quantity { get; set; }
            public string InventoryDescription { get; set; }
        }

        public class Request
        {

        }
    }
}
