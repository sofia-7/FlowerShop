using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcFlowers.Models;
using MvcFlowers.Models;

namespace MvcFlowers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public class LoginData
        {
            public string login { get; set; }
            public string password { get; set; }
        }
        [HttpPost]
        public object GetToken([FromForm] LoginData ld)
        {
            var user = SharedData.Users.FirstOrDefault(u => u.Login == ld.login && u.Password == ld.password);
            if (user == null)
            {
                Response.StatusCode = 401;
                return new { message = "wrong login/password" };
            }
            return AuthOptions.GenerateToken(user.IsAdmin);
        }
        [HttpGet("users")]
        public List<User> GetUsers()
        {
            return SharedData.Users;
        }
        [HttpGet("token")]
        public object GetToken()
        {
            return AuthOptions.GenerateToken();
        }
        [HttpGet("token/secret")]
        public object GetAdminToken()
        {
            return AuthOptions.GenerateToken(true);
        }
    }
}