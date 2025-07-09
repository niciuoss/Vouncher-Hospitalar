using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVoucher.Hubs
{
    public class FilaHub : Hub
    {
        public async Task JoinSalaGroup(string salaId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Sala_{salaId}");
        }

        public async Task LeaveSalaGroup(string salaId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Sala_{salaId}");
        }

        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }

        public async Task LeaveAdminGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}