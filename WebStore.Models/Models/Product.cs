using System;
using System.Collections.Generic;
using System.Text;

namespace WebStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string ProductImage { get; set; }
        public ICollection<Inventory> Inventory { get; set; }
        public ICollection<OrderInventory> OrderProducts { get; set; }
    }
}
