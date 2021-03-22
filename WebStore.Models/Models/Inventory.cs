using System;
using System.Collections.Generic;
using System.Text;

namespace WebStore.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public double Price { get; set; }
        public string InventoryImage { get; set; }
        public Product Product { get; set; }
        public ICollection<OrderInventory> OrderInventory { get; set; }
    }
}
