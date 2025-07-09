using SistemaVoucher.DTOs;
using SistemaVoucher.Models;

namespace SistemaVoucher.Services
{
    public interface IFilaService
    {
        Task<IEnumerable<FilaDto>> GetFilasPorSalaAsync(Guid salaId);
        Task<FilaDto?> GetFilaPorIdAsync(Guid id);
        Task<FilaDto> CriarFilaAsync(CriarFilaDto criarFilaDto);
        Task<FilaDto?> AtualizarFilaAsync(Guid id, AtualizarFilaDto atualizarFilaDto);
        Task<bool> DeletarFilaAsync(Guid id);
        Task<FilaDto?> ChamarProximoPacienteAsync(Guid salaId);
        Task<int> GetProximoNumeroVoucherAsync(Guid salaId);
        Task<IEnumerable<FilaDto>> GetFilasAguardandoAsync();
        Task<int> GetPosicaoNaFilaAsync(Guid filaId);
        Task<int> CalcularTempoEstimadoAsync(Guid salaId);
    }
}