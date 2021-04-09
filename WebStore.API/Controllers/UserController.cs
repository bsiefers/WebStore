using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebStore.Application.CreateUser;
using WebStore.Application.LoginUser;
using WebStore.Database;
using WebStore.Models.Configs;

namespace WebStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _config;
        private JWTConfig _jwtConfig;
        private ApplicationDbContext _context;
        private ILogger<UserController> _logger;
        public UserController(IConfiguration config, ApplicationDbContext context, ILogger<UserController> logger)
        {
            _config = config;
            _context = context;
            _jwtConfig = new JWTConfig { Key = config["JWT:Key"], Issuer = config["JWT:Issuer"] };
        }
        /* POST
         * [api/users/login]
         * body{email, password}
         * logins an user with a JWT
         * returns a JWT
         */
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUser.Request request) {
            try
            {
                if (request == null || request.Email == null || request.Password == null)
                {
                    _logger.LogDebug("Login request failed because atleast one part is null");
                    return BadRequest();
                }
                var response = new LoginUser(_jwtConfig, _context).Do(request);
                if (response.Status == 401)
                {
                    _logger.LogDebug("Login request failed because email password combination didn\'t match.");
                    return Unauthorized();
                }else if(response.Status == 400)
                {
                    _logger.LogDebug("Login request failed because password didn\'t pass validation");
                    return BadRequest();
                }
                else
                {
                    _logger.LogInformation("Succesful login.");
                    return Ok(response.Token);
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
                return StatusCode(500);
            }
        }

        /* POST
         * [api/users/login]
         * body{email, password}
         * creates a new user
         * returns a JWT
         */
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUser.Request request)
        {
            try
            {
                if (request == null || request.Email == null || request.Password == null)
                {
                    _logger.LogDebug("Signup request failed because atleast one part is null");
                    return BadRequest();
                }
                var response = await new CreateUser(_jwtConfig, _context).Do(request);
                if (response.Status == 409)
                {
                    _logger.LogDebug("Signup request failed because there already exists an email with the record.");
                    return Conflict();
                }else if (response.Status == 400)
                {
                    _logger.LogDebug("Login request failed because password didn\'t pass validation");
                    return BadRequest();
                }
                else
                {
                    _logger.LogInformation("Succesful user creation.");
                    return Ok(response.Token);
                }
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
