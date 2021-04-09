using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private ILogger<ProductController> _logger;
        public ProductController(ApplicationDbContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
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
                var response = new GetProducts(_context).Do();
                _logger.LogInformation("Get request for products retrieved: " + response.Products.Count() + "records.");
                return Ok(response.Products);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
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
                {
                    _logger.LogDebug("Get request failed because name given was null");
                    return BadRequest();
                }
                var response = new GetProduct(_context).Do(name);
                if (response.Status == 404)
                {
                    _logger.LogDebug("Get request failed because product with name " + name + " was not found");
                    return NotFound();
                }
                _logger.LogInformation("Get request for products retrieved for prduct: " + response.Product.Name);
                return Ok(response.Product);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
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
                if (response.Status == 404)
                {
                    _logger.LogDebug("Get request failed because product with ID " + Id + " was not found");
                    return NotFound();
                }
                _logger.LogInformation("Get request for products retrieved for prduct: " + response.Product.Name);
                return Ok(response.Product);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
                return StatusCode(500);
            }
        }
    }
}
