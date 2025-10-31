using API.Data;
using API.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LojaContext _db;

        public AuthController(LojaContext db) => _db = db;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.Email) || string.IsNullOrEmpty(req.Senha))
                return BadRequest("Dados inválidos");

            var user = _db.Usuarios.FirstOrDefault(u => u.Email == req.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(req.Senha, user.PasswordHash))
                return Unauthorized("Dados inválidos");

            var token = GerarToken(user);
            return Ok(new { token });
        }

        private string GerarToken(Usuario user)
        {
            // Removido aviso de nulo: user já foi validado no Login
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("chave_secreta_muito_longa_1234567890"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}