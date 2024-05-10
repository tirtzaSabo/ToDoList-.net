using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTask.Interfaces;
using MyTask.Models;
using MyTask.Services;

namespace MyTask.controllers
{
    [ApiController]
    [Route("controller")]
    public class UserController : ControllerBase
    {
        private int userId;
        IUserService UserService;

        public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            UserService = userService;
            string id = httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;
            this.userId = int.Parse(id != null ? id : "0");
        }

        [HttpPost]
        [Route("/login")]
        public ActionResult<String> Login([FromBody] User User)
        {
            var dt = DateTime.Now;

            User user = new User();
            user = UserService.GetUser(user.Name, user.password);

            if (user == null)
            {
                return Unauthorized();
            }

            var claims = new List<Claim> { new Claim("id", user.Id.ToString()), };
            if (user.password == "326131117")
            {
                claims.Add(new Claim("type", "Admin"));
            }
            else
            {
                claims.Add(new Claim("type", "User"));
            }

            var token = UserTokenService.GetToken(claims);

            return new OkObjectResult(UserTokenService.WriteToken(token));
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public ActionResult<List<User>> GetAll() => UserService.GetAll();

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public IActionResult Create(User user)
        {
            UserService.Add(user);
            return CreatedAtAction(nameof(Create), new { id = user.Id }, user);
        }
    }
}
