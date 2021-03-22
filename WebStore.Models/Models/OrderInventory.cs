using System;
using System.Collections.Generic;
using System.Text;

namespace WebStore.Models
{
    public class OrderInventory
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        
        public int Quantity { get; set; }
    }
}
