using Microsoft.EntityFrameworkCore;
using SistemaVoucher.Data;
using SistemaVoucher.DTOs;
using SistemaVoucher.Models;

namespace SistemaVoucher.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly ApplicationDbContext _context;

        public PacienteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PacienteDto>> GetTodosPacientesAsync()
        {
            return await _context.Pacientes
                .Select(p => new PacienteDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Cpf = p.Cpf,
                    Telefone = p.Telefone,
                    Email = p.Email,
                    DataNascimento = p.DataNascimento,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<PacienteDto?> GetPacientePorIdAsync(Guid id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null) return null;

            return new PacienteDto
            {
                Id = paciente.Id,
                Nome = paciente.Nome,
                Cpf = paciente.Cpf,
                Telefone = paciente.Telefone,
                Email = paciente.Email,
                DataNascimento = paciente.DataNascimento,
                CreatedAt = paciente.CreatedAt
            };
        }

        public async Task<PacienteDto?> GetPacientePorCpfAsync(string cpf)
        {
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.Cpf == cpf);
            if (paciente == null) return null;

            return new PacienteDto
            {
                Id = paciente.Id,
                Nome = paciente.Nome,
                Cpf = paciente.Cpf,
                Telefone = paciente.Telefone,
                Email = paciente.Email,
                DataNascimento = paciente.DataNascimento,
                CreatedAt = paciente.CreatedAt
            };
        }

        public async Task<PacienteDto> CriarPacienteAsync(CriarPacienteDto criarPacienteDto)
        {
            var paciente = new Paciente
            {
                Nome = criarPacienteDto.Nome,
                Cpf = criarPacienteDto.Cpf,
                Telefone = criarPacienteDto.Telefone,
                Email = criarPacienteDto.Email,
                DataNascimento = DateTime.SpecifyKind(criarPacienteDto.DataNascimento, DateTimeKind.Unspecified)
            };

            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();

            return await GetPacientePorIdAsync(paciente.Id) ?? throw new InvalidOperationException("Erro ao criar paciente");
        }

        public async Task<PacienteDto?> AtualizarPacienteAsync(Guid id, AtualizarPacienteDto atualizarPacienteDto)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null) return null;

            if (!string.IsNullOrEmpty(atualizarPacienteDto.Nome))
                paciente.Nome = atualizarPacienteDto.Nome;

            if (!string.IsNullOrEmpty(atualizarPacienteDto.Telefone))
                paciente.Telefone = atualizarPacienteDto.Telefone;

            if (!string.IsNullOrEmpty(atualizarPacienteDto.Email))
                paciente.Email = atualizarPacienteDto.Email;

            if (atualizarPacienteDto.DataNascimento.HasValue)
                paciente.DataNascimento = atualizarPacienteDto.DataNascimento.Value;

            await _context.SaveChangesAsync();

            return await GetPacientePorIdAsync(id);
        }

        public async Task<bool> DeletarPacienteAsync(Guid id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null) return false;

            _context.Pacientes.Remove(paciente);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> PacienteExisteAsync(string cpf)
        {
            return await _context.Pacientes.AnyAsync(p => p.Cpf == cpf);
        }
    }
}