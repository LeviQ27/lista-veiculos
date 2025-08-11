using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lista_veiculos.Dominio.DTOs;
using lista_veiculos.Dominio.Entidades;
using lista_veiculos.Dominio.interfaces;
using lista_veiculos.Infraestrutura.Db;

namespace lista_veiculos.Infraestrutura.Servicos
{
    public class AdministradorServico : IAdministradorServico
    {
        private readonly DbContexto _contexto;
        public AdministradorServico(DbContexto db)
        {
            _contexto = db;
        }
        public Administrador? Login(LoginDTO loginDTO)
        {
            // Implementar lógica de autenticação
            return _contexto.Administradores
                .Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        }
    }
}