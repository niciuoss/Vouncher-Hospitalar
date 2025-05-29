namespace SistemaVoucher.DTOs
{
    public class SalaDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Especialidade { get; set; }
        public bool Ativa { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CriarSalaDto
    {
        public string Nome { get; set; } = string.Empty;
        public string? Especialidade { get; set; }
        public bool Ativa { get; set; } = true;
    }

    public class AtualizarSalaDto
    {
        public string? Nome { get; set; }
        public string? Especialidade { get; set; }
        public bool? Ativa { get; set; }
    }
}