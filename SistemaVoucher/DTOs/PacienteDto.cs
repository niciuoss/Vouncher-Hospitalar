namespace SistemaVoucher.DTOs
{
    public class PacienteDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CriarPacienteDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public DateTime DataNascimento { get; set; }
    }

    public class AtualizarPacienteDto
    {
        public string? Nome { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public DateTime? DataNascimento { get; set; }
    }
}