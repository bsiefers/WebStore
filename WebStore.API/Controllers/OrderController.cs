using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebStore.Application.Orders;
using WebStore.Database;

namespace WebStore.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private ApplicationDbContext _context;
        private ILogger<OrderController> _logger;
        public OrderController(ApplicationDbContext context, ILogger<OrderController> logger)
        {
            _logger = logger;
            _context = context;
        }
        /* POST
         * [/api/orders/payment]
         * {customerInfomation:{firstName, lastName, phone, postCode, city, },}
         * creates a payment and its respective order
         */
        [HttpPost("orders/payment")]
        public IActionResult CreatePayment([FromBody] CreatePayment.Request request)
        {
            try
            {
                if (request == null || request.Cart == null)
                {
                    _logger.LogDebug("Create payment request failed because of atleast one part is null");
                    return BadRequest();
                }

                if (request.CustomerInformation == null || request.CustomerInformation.FirstName == null ||
                    request.CustomerInformation.LastName == null || request.CustomerInformation.Phone == null ||
                    request.CustomerInformation.PostCode == null || request.CustomerInformation.Email == null ||
                    request.CustomerInformation.Address1 == null)
                {
                    _logger.LogDebug("Create payment request failed because of atleast one part is null");
                    return BadRequest();
                }
                

                var response = new CreatePayment(_context).Do(request);
                if (response == null)
                {
                    _logger.LogDebug("Create payment request failed when trying to create payment with Stripe.");
                    return BadRequest();
                }
                _logger.LogInformation("Created payment for associated email " + request.CustomerInformation.Email + ".");
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
                return StatusCode(500);
            }
        }

        /* GET
         * [/api/orders/{int}]
         * gets the reference for an order
         */
        [HttpGet("orders/{int}")]
        public IActionResult GetOrder(int Id)
        {
            try
            {
                var response = new GetOrder(_context).Do(Id);
                if (response == null)
                {
                    _logger.LogDebug("Order with ID " + Id + " was not found.");
                    return NotFound();
                }
                _logger.LogDebug("Order get request for: " + Id);
                return Ok(response);
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