using SistemaVoucher.DTOs;
using SistemaVoucher.Models;

namespace SistemaVoucher.Services
{
    public interface IPacienteService
    {
        Task<IEnumerable<PacienteDto>> GetTodosPacientesAsync();
        Task<PacienteDto?> GetPacientePorIdAsync(Guid id);
        Task<PacienteDto?> GetPacientePorCpfAsync(string cpf);
        Task<PacienteDto> CriarPacienteAsync(CriarPacienteDto criarPacienteDto);
        Task<PacienteDto?> AtualizarPacienteAsync(Guid id, AtualizarPacienteDto atualizarPacienteDto);
        Task<bool> DeletarPacienteAsync(Guid id);
        Task<bool> PacienteExisteAsync(string cpf);
    }
}