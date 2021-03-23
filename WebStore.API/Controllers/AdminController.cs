using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebStore.Application.InventoryAdmin;
using WebStore.Application.ProductsAdmin;
using WebStore.Application.OrdersAdmin;
using WebStore.Database;


namespace WebStore.UI.Controllers
{
    [Authorize("Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        /*
         *  [ --Products-- ]
         */

        /* GET
         * [api/admin/products]
         * No body required
         * returns a list of products see product model for details
         */
        [HttpGet("products")]
        public IActionResult GetProducts() {

            try
            {
                GetProducts.Response response = new GetProducts(_context).Do();
                return Ok(response.PVM);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }
         /* GET
         * [/api/admin/products/{int}]
         * No body required
         * returns a product see product model for details
         */
        [HttpGet("products/{id}")]
        public IActionResult GetProduct(int Id) {
            try
            {
                var response = new GetProduct(_context).Do(Id);
                if (response == null)
                    return NotFound();
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }

        }
        /* POST
         * [admin/products]
         * Creates a new product
         * body {
         * name: String,
         * description: String,
         * productImage: File .gif .png .jpg .jpeg(optional)
         * }
         * Returns new Product
         */
        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProduct.Request req) {
            try
            {
                if (req == null || req.Description == null || req.Name == null)
                    return BadRequest();
                var response = await new CreateProduct(_context).Do(req);
                return Ok(response);
            }
            catch
            {
                return StatusCode(500);
            }

        }
        /* PUT
         * [admin/products]
         * Updates a new product
         * body {
         * name: String,
         * description: String,
         * productImage: File .gif .png .jpg .jpeg(optional)
         * }
         * Returns new Product
         */
        [HttpPut("products")]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProduct.Request req)
        {
            try
            {
                if (req == null || req.Description == null || req.Name == null)
                    return BadRequest();
                var response = await new UpdateProduct(_context).Do(req);
                if (response.Status != 200)
                    return StatusCode(response.Status);
                else
                    return Ok(response.Product);
            }
            catch
            {
                return StatusCode(500);
            }
        }
        /* DELETE
         * [admin/products/{int}]
         * Deletes product matching the id
         */
        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            try
            {
                var response = await new DeleteProduct(_context).Do(Id);
                if (response == null)
                    return BadRequest();

                return Ok(response);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        /*
         *  [ --inventory-- ]
         */


        /* GET
         * [/api/admin/inventory]
         * No body required
         * returns a list of inventories see inventory model for details
         */
        [HttpGet("inventory")]
        public IActionResult GetInventory()
        {
            try
            {
                var response = new GetInventory(_context).Do();
                return Ok(response);
            }
            catch
            {
                return StatusCode(500);
            }
        }


        /* POST
         * [admin/products]
         * Creates a new inventory
         * body {
         * description: String,
         * inventoryImage: File .gif .png .jpg .jpeg(optional)
         * }
         * Returns new Inventory
         */
        [HttpPost("inventory")]
        public async Task<IActionResult> CreateInventory([FromForm] CreateInventory.Request req)
        {
            try
            {
                if (req == null || req.Description == null)
                    return BadRequest();
                var response = await new CreateInventory(_context).Do(req);
                if (response == null)
                    return BadRequest();
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }
        /* PUT
         * [admin/products]
         * Updates an inventory
         * body {
         * description: String,
         * inventoryImage: File .gif .png .jpg .jpeg(optional)
         * }
         * Returns updated Inventory
         */
        [HttpPut("inventory")]
        public async Task<IActionResult> UpdateInventory([FromForm] UpdateInventory.Request req)
        {
            try
            {
                if (req == null)
                    return BadRequest();
                var response = await new UpdateInventory(_context).Do(req);                
                
                if (response.Status != 200)
                    return StatusCode(response.Status);
                else
                    return Ok(response.Inventory);                               
            }
            catch
            {
                return StatusCode(500);
            }
        }
        /* DELETE
         * [admin/inventory/{int}]
         * Deletes inventory matching the id
         */
        [HttpDelete("inventory/{id}")]
        public async Task<IActionResult> DeleteInventoryt(int Id)
        {
            try
            {
                var response = await new DeleteInventory(_context).Do(Id);
                if (response == null)
                    return NotFound();
                return Ok(response);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        /*
         *  [ --Orders-- ]
         */


        /* GET
        * [/api/admin/orders/id/{int}]
        * No body required
        * returns an order see order model for details
        */
        [HttpGet("orders/id/{id}")]
        public IActionResult GetOrderById(int Id)
        {
            try
            {
                var response = new GetOrder(_context).Do(Id);
                if (response == null)
                    return NotFound();
                return Ok(response);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        /* GET
        * [/api/admin/orders/status/{string}]
        * acceptable statuses: "ordered", "shipped", "fulfilled"
        * No body required
        * returns a list of orders see order model for details
        */
        [HttpGet("orders/status/{status}")]
        public IActionResult GetOrderByStatus(string status)
        {
            if (status == null)
                return BadRequest();
            var acceptableStatuses = new List<string> { "ordered", "shipped", "fulfilled" };
            status = status.ToLower();
            if (!acceptableStatuses.Contains(status))
                return BadRequest();

            try
            {
                return Ok(new GetOrderByStatus(_context).Do(status));
            }
            catch
            {
                return StatusCode(500);
            }
        }
        /* GET
        * [/api/admin/orders}]
        * acceptable statuses: "ordered", "shipped", "fulfilled"
        * No body required
        * returns a list of shorten orders
        * {
        * id,name,email,phone,status,total,orderdate,note
        * }
        */
        [HttpGet("orders")]
        public IActionResult GetOrders()
        {
            try
            {                
                return Ok(new GetOrders(_context).Do());
            }
            catch
            {
                return StatusCode(500);
            }
        }
        /* POST
        * [/api/admin/orders]
        * body
        * {
        *   cart: array of {inventoryId, quantity},
        *   customerInfomation: {firstName,lastName,email,
        *   phone,address1,address2,city,postCode,state,country},
        *   status: string,
        *   note: string
        *   total: double
        * }
        * returns a new order see order model for details
        */
        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrder.Request req) {
            if (req == null || req.Cart == null ||
                req.Status == null)
                return BadRequest();
            if (req.CustomerInformation == null || req.CustomerInformation.FirstName == null ||
                req.CustomerInformation.LastName == null || req.CustomerInformation.Phone == null ||
                req.CustomerInformation.PostCode == null || req.CustomerInformation.Email == null ||
                req.CustomerInformation.Address1 == null)
                return BadRequest();
            try 
            { 
                var response = await new CreateOrder(_context).Do(req);
                
                if (response == null)
                    return Conflict();
                return Ok(response);
            }
            catch
            {
                return StatusCode(500);
            }
        }
        /* Put
        * [/api/admin/orders]
        * body
        * {
        *   cart: array of {inventoryId, quantity},
        *   customerInfomation: {firstName,lastName,email,
        *   phone,address1,address2,city,postCode,state,country},
        *   status: string,
        *   note: string
        *   total: double
        * returns an updated order see order model for details
        * }
        */
        [HttpPut("orders")]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrder.Request req){
            if (req == null || req.Cart == null || 
                req.Status == null)
                return BadRequest();
            if (req.CustomerInformation == null || req.CustomerInformation.FirstName == null ||
                req.CustomerInformation.LastName == null || req.CustomerInformation.Phone == null ||
                req.CustomerInformation.PostCode == null || req.CustomerInformation.Email == null ||
                req.CustomerInformation.Address1 == null)
                return BadRequest();

            var response = await new UpdateOrder(_context).Do(req);
            if (response.Status == 409)
                return Conflict();
            if (response.Status == 404)
                return NotFound();

            return Ok(response.Order);
        }
        /* DELETE
         * [admin/orders/{int}]
         * Deletes order matching the id
         */
        [HttpDelete("orders/{id}")]
        public async Task<IActionResult> DeleteOrder(int Id)
        {
            try
            {
                var response = await new DeleteOrder(_context).Do(Id);
                if (response == null)
                    return NotFound();
                return Ok(response);
            }
            catch
            {
                return StatusCode(500);
            }
        }

    }
}
