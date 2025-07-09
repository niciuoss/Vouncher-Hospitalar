using System.ComponentModel.DataAnnotations;

namespace SistemaVoucher.Models
{
    public class Sala
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Especialidade { get; set; }

        public bool Ativa { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

        // Navigation Properties
        public virtual ICollection<Fila> Filas { get; set; } = new List<Fila>();
    }
}
