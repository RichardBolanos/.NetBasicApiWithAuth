using GestionTareasApi.Data;
using GestionTareasApi.Dtos;
using GestionTareasApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestionTareasApi.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Registra un nuevo usuario.
        /// </summary>
        /// <param name="userDto">Datos del usuario</param>
        /// <response code="200">Usuario registrado correctamente</response>
        /// <response code="400">Datos inválidos</response>
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Registra un nuevo usuario", Description = "Crea un nuevo usuario con un nombre de usuario y contraseña")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("Usuario registrado");
        }

        /// <summary>
        /// Inicia sesión y genera un token JWT.
        /// </summary>
        /// <param name="userDto">Datos del usuario</param>
        /// <response code="200">Token JWT generado correctamente</response>
        /// <response code="401">Credenciales incorrectas</response>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Inicia sesión", Description = "Verifica las credenciales y devuelve un token JWT si son válidas")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
                return Unauthorized("Credenciales incorrectas");

            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: new[] { new Claim(ClaimTypes.Name, user.Username) },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
