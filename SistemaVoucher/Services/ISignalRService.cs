using Microsoft.AspNetCore.SignalR;
using SistemaVoucher.Hubs;

namespace SistemaVoucher.Services
{
    public class SignalRService : ISignalRService
    {
        private readonly IHubContext<FilaHub> _hubContext;

        public SignalRService(IHubContext<FilaHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotificarNovoChamado(Guid salaId, string numeroVoucher, string nomePaciente)
        {
            await _hubContext.Clients.Group($"Sala_{salaId}").SendAsync("NovoChamado", new
            {
                NumeroVoucher = numeroVoucher,
                NomePaciente = nomePaciente,
                SalaId = salaId,
                Timestamp = DateTime.UtcNow
            });

            // Notificar também o painel geral (TVs)
            await _hubContext.Clients.All.SendAsync("ChamadaPainel", new
            {
                NumeroVoucher = numeroVoucher,
                SalaId = salaId,
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task NotificarFilaAtualizada(Guid salaId)
        {
            await _hubContext.Clients.Group($"Sala_{salaId}").SendAsync("FilaAtualizada", new
            {
                SalaId = salaId,
                Timestamp = DateTime.UtcNow
            });

            // Notificar admins sobre mudanças na fila
            await _hubContext.Clients.Group("Admins").SendAsync("FilaAtualizada", new
            {
                SalaId = salaId,
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task NotificarAdmins(string message, object data)
        {
            await _hubContext.Clients.Group("Admins").SendAsync("AdminNotification", new
            {
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task NotificarPacienteChamado(Guid pacienteId, string sala)
        {
            await _hubContext.Clients.All.SendAsync("PacienteChamado", new
            {
                PacienteId = pacienteId,
                Sala = sala,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}