using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lista_veiculos.Dominio.Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Dominio.Entidades
{
    [TestClass]
    public class VeiculoTest
    {
        [TestMethod]
        public void TestVeiculoGetSetPropriedades()
        {
            // Arrange
            var veiculo = new Veiculo
            {
                Id = 1,
                Nome = "Corolla",
                Marca = "Toyota",
                Ano = 2020
            };

            var veiculo2 = new Veiculo
            {
                Id = 2,
                Nome = "Civic",
                Marca = "Honda",
                Ano = 2021
            };

            // Act

            // Assert
            Assert.AreEqual(1, veiculo.Id);
            Assert.AreEqual("Corolla", veiculo.Nome);
            Assert.AreEqual("Toyota", veiculo.Marca);
            Assert.AreEqual(2020, veiculo.Ano);

            Assert.AreEqual(2, veiculo2.Id);
            Assert.AreEqual("Civic", veiculo2.Nome);
            Assert.AreEqual("Honda", veiculo2.Marca);
            Assert.AreEqual(2021, veiculo2.Ano);
        }
    }
}