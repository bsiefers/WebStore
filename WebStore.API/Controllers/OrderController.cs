using Microsoft.AspNetCore.Mvc;
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

        public OrderController(ApplicationDbContext context)
        {
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
                    return BadRequest();

                if (request.CustomerInformation == null || request.CustomerInformation.FirstName == null ||
                    request.CustomerInformation.LastName == null || request.CustomerInformation.Phone == null ||
                    request.CustomerInformation.PostCode == null || request.CustomerInformation.Email == null ||
                    request.CustomerInformation.Address1 == null)
                    return BadRequest();

                

                var response = new CreatePayment(_context).Do(request);
                if (response == null)
                    return BadRequest();

                return Ok(response);
            }
            catch(Exception e)
            {
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