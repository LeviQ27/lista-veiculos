using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using lista_veiculos.Dominio.Entidades;
using lista_veiculos.Infraestrutura.Db;
using lista_veiculos.Infraestrutura.Servicos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Dominio.Servicos
{
    [TestClass]
    public class AdministradorServicoTest
    {
        private DbContexto CriarContextoDeTeste()
        {
            var assmblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(assmblyPath ?? "", "..", "..", "..");

            var builder = new ConfigurationBuilder()
                .SetBasePath(path ?? Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            return new DbContexto(configuration);
        }

        [TestMethod]
        public void TestandoSalvarAdministrador()
        {
            var context = CriarContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

            var adm = new Administrador { Email = "admin@test.com", Senha = "Senha123", Perfil = "Adm" };
            
            var administradoresServico = new AdministradorServico(context);

            // Act
            administradoresServico.Adicionar(adm);

            //Assert
            Assert.AreEqual(1, administradoresServico.Todos(1).Count());
        }

        [TestMethod]
        public void TestandoBuscarPorId()
        {
            //Arrange
            var context = CriarContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

            var adm = new Administrador { Email = "admin@test.com", Senha = "Senha123", Perfil = "Adm" };
            var administradoresServico = new AdministradorServico(context);

            //Act
            administradoresServico.Adicionar(adm);
            var resultado = administradoresServico.BuscaPorId(adm.Id);

            //Assert
            Assert.AreEqual(1, resultado?.Id);
        }
    }
}