using Microsoft.AspNetCore.SignalR;
using SistemaVoucher.Hubs;

namespace SistemaVoucher.Services
{
    public interface ISignalRService
    {
        Task NotificarNovoChamado(Guid salaId, string numeroVoucher, string nomePaciente);
        Task NotificarFilaAtualizada(Guid salaId);
        Task NotificarAdmins(string message, object data);
        Task NotificarPacienteChamado(Guid pacienteId, string sala);
    }
}

