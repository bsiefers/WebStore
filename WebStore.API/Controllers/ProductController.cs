using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebStore.Application.Products;
using WebStore.Database;

namespace WebStore.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        /* GET
         * [/api/products]
         * No body required
         * returns a list of products that have atleast one inventory see product model for details
         */
        [HttpGet("products")]
        public IActionResult GetProducts()
        {
            try
            {
                return Ok(new GetProducts(_context).Do());
            }
            catch
            {
                return StatusCode(500);
            }
        }
        /* GET
         * [/api/products/{string}]
         * No body required
         * returns a product
         */
        [HttpGet("products/{name}")]
        public IActionResult GetProduct(string name)
        {
            try
            {
                if (name == null)
                    return BadRequest();
                var response = new GetProduct(_context).Do(name);
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
         * [/api/products/{int}]
         * No body required
         * returns a product
         */
        [HttpGet("products/getByIventoryId/{Id}")]
        public IActionResult GetProductByInventoryId(int Id) {
            try
            {
                var response = new GetProductByInventoryId(_context).Do(Id);
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
