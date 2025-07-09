using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SistemaVoucher.Data;
using SistemaVoucher.Models;

namespace SistemaVoucher.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var usuario = await GetUsuarioPorUsernameAsync(username);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
                return null;

            usuario.LastLoginAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            await _context.SaveChangesAsync();

            return GerarJwtToken(usuario);
        }

        public async Task<Usuario?> GetUsuarioPorUsernameAsync(string username)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == username && u.Ativo);
        }

        public async Task<Usuario> CriarUsuarioAsync(string username, string password, TipoUsuario role)
        {
            var usuario = new Usuario
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role,
                Ativo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        public async Task<bool> ValidarUsuarioAsync(string username, string password)
        {
            var usuario = await GetUsuarioPorUsernameAsync(username);
            return usuario != null && BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
        }

        public string GerarJwtToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException("SecretKey não configurada"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Username),
                    new Claim(ClaimTypes.Role, usuario.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpiryInHours"] ?? "24")),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> UsuarioExisteAsync(string username)
        {
            return await _context.Usuarios.AnyAsync(u => u.Username == username);
        }
    }
}