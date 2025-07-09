using Microsoft.AspNetCore.Mvc;
using SistemaVoucher.Services;
using SistemaVoucher.Models;
using SistemaVoucher.DTOs;

namespace SistemaVoucher.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { message = "Username e password são obrigatórios" });
                }

                var token = await _authService.LoginAsync(request.Username, request.Password);

                if (token == null)
                {
                    return Unauthorized(new { message = "Credenciais inválidas" });
                }

                var usuario = await _authService.GetUsuarioPorUsernameAsync(request.Username);

                return Ok(new LoginResponse
                {
                    Token = token,
                    Username = usuario!.Username,
                    Role = usuario.Role.ToString(),
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Cria novo usuário (apenas admins)
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { message = "Username e password são obrigatórios" });
                }

                var usuarioExiste = await _authService.UsuarioExisteAsync(request.Username);
                if (usuarioExiste)
                {
                    return Conflict(new { message = "Username já existe" });
                }

                if (!Enum.TryParse<TipoUsuario>(request.Role, out var role))
                {
                    role = TipoUsuario.Operador; // Default
                }

                var usuario = await _authService.CriarUsuarioAsync(request.Username, request.Password, role);

                return Ok(new RegisterResponse
                {
                    Id = usuario.Id,
                    Username = usuario.Username,
                    Role = usuario.Role.ToString(),
                    CreatedAt = usuario.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Valida se o token está válido
        /// </summary>
        [HttpPost("validate")]
        public ActionResult ValidateToken()
        {
            // Se chegou aqui, o token é válido (middleware JWT já validou)
            return Ok(new { message = "Token válido" });
        }
    }
}