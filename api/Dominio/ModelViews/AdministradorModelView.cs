using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lista_veiculos.Dominio.Enums;

namespace lista_veiculos.Dominio.ModelViews
{
    public record AdministradorModelView
    {
        public int Id { get; init; }
        public string Email { get; init; }
        public string Perfil { get; init; }
    }
}