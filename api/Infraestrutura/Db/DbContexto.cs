using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lista_veiculos.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace lista_veiculos.Infraestrutura.Db
{
    public class DbContexto : DbContext
    {
        private readonly IConfiguration _configurationAppSettings;

        public DbContexto(IConfiguration configurationAppSettings)
        {
            _configurationAppSettings = configurationAppSettings;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Administrador>().HasData(
                new Administrador
                {
                    Id = 1,
                    Email = "admin@example.com",
                    Senha = "adminpassword",
                    Perfil = "Adm"
                }
            );
        }

        public DbSet<Administrador> Administradores { get; set; } = default!;

        public DbSet<Veiculo> Veiculos { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var stringDeConexao = _configurationAppSettings.GetConnectionString("MySql")?.ToString();
                if (!string.IsNullOrEmpty(stringDeConexao))
                {
                    optionsBuilder.UseMySql(stringDeConexao, ServerVersion.AutoDetect(stringDeConexao));
                }
            }
        }
    }
}