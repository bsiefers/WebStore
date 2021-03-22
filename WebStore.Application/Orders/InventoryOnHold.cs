using System;
using System.Collections.Generic;
using System.Text;
using WebStore.Models;

namespace WebStore.Application.Orders
{
    public class InventoryOnHold
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
