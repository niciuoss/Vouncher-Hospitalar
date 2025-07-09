using Microsoft.EntityFrameworkCore;
using SistemaVoucher.Data;
using SistemaVoucher.DTOs;
using SistemaVoucher.Models;

namespace SistemaVoucher.Services
{
    public class SalaService : ISalaService
    {
        private readonly ApplicationDbContext _context;

        public SalaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalaDto>> GetTodasSalasAsync()
        {
            return await _context.Salas
                .Select(s => new SalaDto
                {
                    Id = s.Id,
                    Nome = s.Nome,
                    Especialidade = s.Especialidade,
                    Ativa = s.Ativa,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SalaDto>> GetSalasAtivasAsync()
        {
            return await _context.Salas
                .Where(s => s.Ativa)
                .Select(s => new SalaDto
                {
                    Id = s.Id,
                    Nome = s.Nome,
                    Especialidade = s.Especialidade,
                    Ativa = s.Ativa,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<SalaDto?> GetSalaPorIdAsync(Guid id)
        {
            var sala = await _context.Salas.FindAsync(id);
            if (sala == null) return null;

            return new SalaDto
            {
                Id = sala.Id,
                Nome = sala.Nome,
                Especialidade = sala.Especialidade,
                Ativa = sala.Ativa,
                CreatedAt = sala.CreatedAt
            };
        }

        public async Task<SalaDto> CriarSalaAsync(CriarSalaDto criarSalaDto)
        {
            var sala = new Sala
            {
                Nome = criarSalaDto.Nome,
                Especialidade = criarSalaDto.Especialidade,
                Ativa = criarSalaDto.Ativa
            };

            _context.Salas.Add(sala);
            await _context.SaveChangesAsync();

            return await GetSalaPorIdAsync(sala.Id) ?? throw new InvalidOperationException("Erro ao criar sala");
        }

        public async Task<SalaDto?> AtualizarSalaAsync(Guid id, AtualizarSalaDto atualizarSalaDto)
        {
            var sala = await _context.Salas.FindAsync(id);
            if (sala == null) return null;

            if (!string.IsNullOrEmpty(atualizarSalaDto.Nome))
                sala.Nome = atualizarSalaDto.Nome;

            if (!string.IsNullOrEmpty(atualizarSalaDto.Especialidade))
                sala.Especialidade = atualizarSalaDto.Especialidade;

            if (atualizarSalaDto.Ativa.HasValue)
                sala.Ativa = atualizarSalaDto.Ativa.Value;

            await _context.SaveChangesAsync();

            return await GetSalaPorIdAsync(id);
        }

        public async Task<bool> DeletarSalaAsync(Guid id)
        {
            var sala = await _context.Salas.FindAsync(id);
            if (sala == null) return false;

            _context.Salas.Remove(sala);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SalaExisteAsync(string nome)
        {
            return await _context.Salas.AnyAsync(s => s.Nome == nome);
        }
    }
}