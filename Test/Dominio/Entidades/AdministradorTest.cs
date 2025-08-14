using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lista_veiculos.Dominio.Entidades;
using lista_veiculos.Dominio.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Dominio
{
    [TestClass]
    public class AdministradorTest
    {
        [TestMethod]
        public void TestAdministradorGetSetPropriedades()
        {
            Assert.IsTrue(true);

            // Arrange
            var administrador = new Administrador
            {
                Id = 1,
                Email = "test@test.com",
                Senha = "123456",
                Perfil = "Adm"
            };

            var editor = new Administrador
            {
                Id = 2,
                Email = "editor@test.com",
                Senha = "654321",
                Perfil = "Editor"
            };

            // Act

            // Assert
            Assert.AreEqual(1, administrador.Id);
            Assert.AreEqual("test@test.com", administrador.Email);
            Assert.AreEqual("123456", administrador.Senha);
            Assert.AreEqual("Adm", administrador.Perfil);

            Assert.AreEqual(2, editor.Id);
            Assert.AreEqual("editor@test.com", editor.Email);
            Assert.AreEqual("654321", editor.Senha);
            Assert.AreEqual("Editor", editor.Perfil);
        }
    }
}