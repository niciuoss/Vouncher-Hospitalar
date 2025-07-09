using System.ComponentModel.DataAnnotations;

namespace SistemaVoucher.Models
{
    public enum StatusFila
    {
        Aguardando = 1,
        Chamando = 2,
        Atendido = 3,
        Cancelado = 4
    }

    public class Fila
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PacienteId { get; set; }

        [Required]
        public Guid SalaId { get; set; }

        public int NumeroVoucher { get; set; }

        public StatusFila Status { get; set; } = StatusFila.Aguardando;

        public int Prioridade { get; set; } = 0; // 0 = normal, 1 = prioritário

        public int? TempoEstimado { get; set; } // em minutos

        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

        public DateTime? ChamadoAt { get; set; }

        public DateTime? AtendidoAt { get; set; }

        // Navigation Properties
        public virtual Paciente Paciente { get; set; } = null!;
        public virtual Sala Sala { get; set; } = null!;
    }
}