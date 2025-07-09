using Microsoft.EntityFrameworkCore;
using SistemaVoucher.Data;
using SistemaVoucher.DTOs;
using SistemaVoucher.Models;

namespace SistemaVoucher.Services
{
    public class FilaService : IFilaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISignalRService _signalRService;

        public FilaService(ApplicationDbContext context, ISignalRService signalRService)
        {
            _context = context;
            _signalRService = signalRService;
        }

        public async Task<IEnumerable<FilaDto>> GetFilasPorSalaAsync(Guid salaId)
        {
            return await _context.Filas
                .Include(f => f.Paciente)
                .Include(f => f.Sala)
                .Where(f => f.SalaId == salaId && f.Status != StatusFila.Atendido && f.Status != StatusFila.Cancelado)
                .OrderBy(f => f.Prioridade).ThenBy(f => f.CreatedAt)
                .Select(f => new FilaDto
                {
                    Id = f.Id,
                    PacienteId = f.PacienteId,
                    NomePaciente = f.Paciente.Nome,
                    SalaId = f.SalaId,
                    NomeSala = f.Sala.Nome,
                    NumeroVoucher = f.NumeroVoucher,
                    Status = f.Status.ToString(),
                    Prioridade = f.Prioridade,
                    TempoEstimado = f.TempoEstimado,
                    CreatedAt = f.CreatedAt,
                    ChamadoAt = f.ChamadoAt
                })
                .ToListAsync();
        }

        public async Task<FilaDto?> GetFilaPorIdAsync(Guid id)
        {
            var fila = await _context.Filas
                .Include(f => f.Paciente)
                .Include(f => f.Sala)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fila == null) return null;

            return new FilaDto
            {
                Id = fila.Id,
                PacienteId = fila.PacienteId,
                NomePaciente = fila.Paciente.Nome,
                SalaId = fila.SalaId,
                NomeSala = fila.Sala.Nome,
                NumeroVoucher = fila.NumeroVoucher,
                Status = fila.Status.ToString(),
                Prioridade = fila.Prioridade,
                TempoEstimado = fila.TempoEstimado,
                CreatedAt = fila.CreatedAt,
                ChamadoAt = fila.ChamadoAt
            };
        }

        public async Task<FilaDto> CriarFilaAsync(CriarFilaDto criarFilaDto)
        {
            var numeroVoucher = await GetProximoNumeroVoucherAsync(criarFilaDto.SalaId);
            var tempoEstimado = await CalcularTempoEstimadoAsync(criarFilaDto.SalaId);

            var fila = new Fila
            {
                PacienteId = criarFilaDto.PacienteId,
                SalaId = criarFilaDto.SalaId,
                NumeroVoucher = numeroVoucher,
                Prioridade = criarFilaDto.Prioridade,
                TempoEstimado = tempoEstimado,
                Status = StatusFila.Aguardando
            };

            _context.Filas.Add(fila);
            await _context.SaveChangesAsync();

            await _signalRService.NotificarFilaAtualizada(criarFilaDto.SalaId);

            return await GetFilaPorIdAsync(fila.Id) ?? throw new InvalidOperationException("Erro ao criar fila");
        }

        public async Task<FilaDto?> AtualizarFilaAsync(Guid id, AtualizarFilaDto atualizarFilaDto)
        {
            var fila = await _context.Filas.FindAsync(id);
            if (fila == null) return null;

            if (!string.IsNullOrEmpty(atualizarFilaDto.Status) && Enum.TryParse<StatusFila>(atualizarFilaDto.Status, out var status))
            {
                fila.Status = status;
                if (status == StatusFila.Chamando)
                {
                    fila.ChamadoAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
                }
                else if (status == StatusFila.Atendido)
                {
                    fila.AtendidoAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
                }
            }

            if (atualizarFilaDto.TempoEstimado.HasValue)
            {
                fila.TempoEstimado = atualizarFilaDto.TempoEstimado.Value;
            }

            await _context.SaveChangesAsync();
            await _signalRService.NotificarFilaAtualizada(fila.SalaId);

            return await GetFilaPorIdAsync(id);
        }

        public async Task<bool> DeletarFilaAsync(Guid id)
        {
            var fila = await _context.Filas.FindAsync(id);
            if (fila == null) return false;

            _context.Filas.Remove(fila);
            await _context.SaveChangesAsync();
            await _signalRService.NotificarFilaAtualizada(fila.SalaId);

            return true;
        }

        public async Task<FilaDto?> ChamarProximoPacienteAsync(Guid salaId)
        {
            var proximaFila = await _context.Filas
                .Include(f => f.Paciente)
                .Include(f => f.Sala)
                .Where(f => f.SalaId == salaId && f.Status == StatusFila.Aguardando)
                .OrderBy(f => f.Prioridade).ThenBy(f => f.CreatedAt)
                .FirstOrDefaultAsync();

            if (proximaFila == null) return null;

            proximaFila.Status = StatusFila.Chamando;
            proximaFila.ChamadoAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            await _context.SaveChangesAsync();

            await _signalRService.NotificarNovoChamado(salaId, proximaFila.NumeroVoucher.ToString(), proximaFila.Paciente.Nome);

            return await GetFilaPorIdAsync(proximaFila.Id);
        }

        public async Task<int> GetProximoNumeroVoucherAsync(Guid salaId)
        {
            var ultimoNumero = await _context.Filas
                .Where(f => f.SalaId == salaId && f.CreatedAt.Date == DateTime.UtcNow.Date)
                .MaxAsync(f => (int?)f.NumeroVoucher) ?? 0;

            return ultimoNumero + 1;
        }

        public async Task<IEnumerable<FilaDto>> GetFilasAguardandoAsync()
        {
            return await _context.Filas
                .Include(f => f.Paciente)
                .Include(f => f.Sala)
                .Where(f => f.Status == StatusFila.Aguardando)
                .OrderBy(f => f.SalaId).ThenBy(f => f.Prioridade).ThenBy(f => f.CreatedAt)
                .Select(f => new FilaDto
                {
                    Id = f.Id,
                    PacienteId = f.PacienteId,
                    NomePaciente = f.Paciente.Nome,
                    SalaId = f.SalaId,
                    NomeSala = f.Sala.Nome,
                    NumeroVoucher = f.NumeroVoucher,
                    Status = f.Status.ToString(),
                    Prioridade = f.Prioridade,
                    TempoEstimado = f.TempoEstimado,
                    CreatedAt = f.CreatedAt,
                    ChamadoAt = f.ChamadoAt
                })
                .ToListAsync();
        }

        public async Task<int> GetPosicaoNaFilaAsync(Guid filaId)
        {
            var fila = await _context.Filas.FindAsync(filaId);
            if (fila == null) return 0;

            return await _context.Filas
                .Where(f => f.SalaId == fila.SalaId &&
                           f.Status == StatusFila.Aguardando &&
                           (f.Prioridade < fila.Prioridade ||
                            (f.Prioridade == fila.Prioridade && f.CreatedAt < fila.CreatedAt)))
                .CountAsync() + 1;
        }

        public async Task<int> CalcularTempoEstimadoAsync(Guid salaId)
        {
            var filasAguardando = await _context.Filas
                .Where(f => f.SalaId == salaId && f.Status == StatusFila.Aguardando)
                .CountAsync();

            // Estimativa de 15 minutos por paciente
            return filasAguardando * 15;
        }
    }
}
