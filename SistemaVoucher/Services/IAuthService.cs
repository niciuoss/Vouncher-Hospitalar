using SistemaVoucher.Models;

namespace SistemaVoucher.Services {
    public interface IAuthService {
        Task<string?> LoginAsync(string username, string password);
        Task<Usuario?> GetUsuarioPorUsernameAsync(string username);
        Task<Usuario> CriarUsuarioAsync(string username, string password, TipoUsuario role);
        Task<bool> ValidarUsuarioAsync(string username, string password);
        string GerarJwtToken(Usuario usuario);
        Task<bool> UsuarioExisteAsync(string username);
    }
}