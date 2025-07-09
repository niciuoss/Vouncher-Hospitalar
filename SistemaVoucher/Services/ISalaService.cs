using SistemaVoucher.DTOs;
using SistemaVoucher.Models;

namespace SistemaVoucher.Services
{
    public interface ISalaService
    {
        Task<IEnumerable<SalaDto>> GetTodasSalasAsync();
        Task<IEnumerable<SalaDto>> GetSalasAtivasAsync();
        Task<SalaDto?> GetSalaPorIdAsync(Guid id);
        Task<SalaDto> CriarSalaAsync(CriarSalaDto criarSalaDto);
        Task<SalaDto?> AtualizarSalaAsync(Guid id, AtualizarSalaDto atualizarSalaDto);
        Task<bool> DeletarSalaAsync(Guid id);
        Task<bool> SalaExisteAsync(string nome);
    }
}