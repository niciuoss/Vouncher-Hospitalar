using Microsoft.EntityFrameworkCore;
using SistemaVoucher.Models;

namespace SistemaVoucher.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<Fila> Filas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Paciente
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Cpf).IsRequired().HasMaxLength(14);
                entity.Property(e => e.Telefone).HasMaxLength(15);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.DataNascimento).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                // Index único para CPF
                entity.HasIndex(e => e.Cpf).IsUnique();

                // Index para email (se não for nulo)
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configuração da entidade Sala
            modelBuilder.Entity<Sala>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Especialidade).HasMaxLength(100);
                entity.Property(e => e.Ativa).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).IsRequired();

                // Index para nome da sala
                entity.HasIndex(e => e.Nome);
            });

            // Configuração da entidade Fila
            modelBuilder.Entity<Fila>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PacienteId).IsRequired();
                entity.Property(e => e.SalaId).IsRequired();
                entity.Property(e => e.NumeroVoucher).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Prioridade).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.CreatedAt).IsRequired();

                // Relacionamentos
                entity.HasOne(e => e.Paciente)
                    .WithMany(p => p.Filas)
                    .HasForeignKey(e => e.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Sala)
                    .WithMany(s => s.Filas)
                    .HasForeignKey(e => e.SalaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.PacienteId);
                entity.HasIndex(e => e.SalaId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.SalaId, e.Status });
            });

            // Configuração da entidade Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.Ativo).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).IsRequired();

                // Index único para username
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Dados iniciais (Seed Data)
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Criar usuário admin padrão
            var adminId = Guid.NewGuid();
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = adminId,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), // Senha padrão
                    Role = TipoUsuario.Admin,
                    Ativo = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Criar salas padrão
            var salaClinicoId = Guid.NewGuid();
            var salaPediatria = Guid.NewGuid();
            var salaGinecologia = Guid.NewGuid();

            modelBuilder.Entity<Sala>().HasData(
                new Sala
                {
                    Id = salaClinicoId,
                    Nome = "Sala 01",
                    Especialidade = "Clínico Geral",
                    Ativa = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Sala
                {
                    Id = salaPediatria,
                    Nome = "Sala 02",
                    Especialidade = "Pediatria",
                    Ativa = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Sala
                {
                    Id = salaGinecologia,
                    Nome = "Sala 03",
                    Especialidade = "Ginecologia",
                    Ativa = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}