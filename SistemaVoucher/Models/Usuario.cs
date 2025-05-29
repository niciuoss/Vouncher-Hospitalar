using System.ComponentModel.DataAnnotations;

namespace SistemaVoucher.Models
{
    public enum TipoUsuario
    {
        Admin = 1,
        Operador = 2
    }

    public class Usuario
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public TipoUsuario Role { get; set; } = TipoUsuario.Operador;

        public bool Ativo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }
    }
}