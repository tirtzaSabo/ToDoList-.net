using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyTask.Interfaces;
using MyTask.Models;
using MyTask.Services;

namespace MyTask.controllers
{
    [ApiController]
    [Route("[controller]")]
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
        public ActionResult<String> Login([FromBody] User u)
        {
            User user = UserService.GetUser(u.Name!, u.Password!);

            if (user == null)
            {
                return Unauthorized();
            }

            var claims = new List<Claim> { new("id", user.Id.ToString()), };
            if (user.Password == "12345678")
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
        [Route("/_user")]
        [Authorize(Policy = "User")]
        public ActionResult<User> GetUser()
        {
            var user = UserService.Get(this.userId);
            if (user == null)
                return NotFound();
            return user;
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

        [HttpDelete("{userId}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Delete(int userId)
        {
            var user = UserService.Get(userId);
            if (user is null)
                return NotFound();

            UserService.Delete(userId);

            return Content(UserService.Count.ToString());
        }

        [HttpPut]
        [Authorize(Policy = "User")]
        public ActionResult put(User newUser)
        {
            newUser.Id = this.userId;
            var result = UserService.Update(newUser);
            if (!result)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [HttpGet]
        [Route("/ifAdmin")]
        [Authorize(Policy = "Admin")]
        public ActionResult<String> GetIfAdmin()
        {
            return new OkObjectResult("true");
        }
    }
}
