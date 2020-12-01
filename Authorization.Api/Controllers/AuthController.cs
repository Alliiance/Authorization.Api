using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Authorization.Api.Models;
using Authorization.Api.Models.DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Authorization.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private AccountsContext context;
        private readonly IOptions<AuthOptions> authOptions;

        public AuthController(AccountsContext context, IOptions<AuthOptions> authOptions)
        {
            this.context = context;
            this.authOptions = authOptions;
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Login([FromBody] LoginReuest request)
        {   
            var user = AuthenticateUser(request.Login, request.Password);

            if (user != null)
            {
                var token = GenerateJWT(user);

                return Ok(new
                {
                    access_token = token
                });
            }

            return Unauthorized("You entered an incorrect username or password");
        }

        [Route("registration")]
        [HttpPost]
        public IActionResult Post([FromBody] User account)
        {
            var users = context.User.ToList();
            if (users.Any(w => w.Login == account.Login))
            {
                return Unauthorized("This login is already taken");
            }
            else
            {
                context.User.Add(account);
                context.SaveChanges();
                var token = GenerateJWT(account);

                return Ok(new
                {
                    access_token = token
                });
            }
        }

        [Route("getUsers")]
        [Authorize]
        [HttpGet]
        public IActionResult GetGetAllUsers()
        {
            var users = context.User.ToList();
            return Ok(users);
        }

        private User AuthenticateUser(string login, string password)
        {
            var user = context.User.ToList();
            return user.SingleOrDefault(u => u.Login == login && u.Password == password);
        }

        private string GenerateJWT(User user)
        {
            var authParams = authOptions.Value;

            var securityKey = authParams.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.FamilyName, user.Login ),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            };

            var token = new JwtSecurityToken(authParams.Issuer,
                authParams.Audience,
                claims,
                expires: DateTime.Now.AddSeconds(authParams.TokenLifetime),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
