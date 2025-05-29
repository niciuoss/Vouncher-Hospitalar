namespace SistemaVoucher.DTOs
{
    public class FilaDto
    {
        public Guid Id { get; set; }
        public Guid PacienteId { get; set; }
        public string NomePaciente { get; set; } = string.Empty;
        public Guid SalaId { get; set; }
        public string NomeSala { get; set; } = string.Empty;
        public int NumeroVoucher { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Prioridade { get; set; }
        public int? TempoEstimado { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ChamadoAt { get; set; }
    }

    public class CriarFilaDto
    {
        public Guid PacienteId { get; set; }
        public Guid SalaId { get; set; }
        public int Prioridade { get; set; } = 0;
    }

    public class AtualizarFilaDto
    {
        public string? Status { get; set; }
        public int? TempoEstimado { get; set; }
    }
}