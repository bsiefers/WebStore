using System;
using System.Collections.Generic;
using System.Text;

namespace WebStore.Models
{
    public class Order
    {
        //Id
        public int Id { get; set; }
        public string StripeRef { get; set; }

        //customer info
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }        
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }

        //meta-data
        public string Status { get; set; }
        public double Total { get; set; }
        public DateTime OrderDate { get; set; }
        public IEnumerable<OrderInventory> OrderInventory { get; set; }
        public string Note { get; set; }
    }
}
