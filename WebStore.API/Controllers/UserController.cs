using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        public UserController(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
            _jwtConfig = new JWTConfig { Key = config["JWT:Key"], Issuer = config["JWT:Issuer"] };
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUser.Request request) {
            try
            {
                if (request == null || request.Email == null || request.Password == null)
                    return BadRequest();
                var response = new LoginUser(_jwtConfig, _context).Do(request);
                if (response.Token == null)
                    return Unauthorized();


                return Ok(response);
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUser.Request request)
        {
            try
            {
                if (request == null || request.Email == null || request.Password == null)
                    return BadRequest();
                var response = await new CreateUser(_jwtConfig, _context).Do(request);
                if (response.Token == null)
                    return Conflict();
            
                return Ok(response);            

            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
