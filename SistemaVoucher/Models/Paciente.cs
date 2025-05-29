using System.ComponentModel.DataAnnotations;

namespace SistemaVoucher.Models
{
    public class Paciente
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(14)]
        public string Cpf { get; set; } = string.Empty;

        [StringLength(15)]
        public string? Telefone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        public DateTime DataNascimento { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<Fila> Filas { get; set; } = new List<Fila>();
    }
}