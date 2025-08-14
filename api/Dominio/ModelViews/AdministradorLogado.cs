using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lista_veiculos.Dominio.ModelViews
{
    public record AdministradorLogado
    {
        public string Email { get; init; } = default!;
        public string Perfil { get; init; } = default!;
        public string Token { get; init; } = default!;
    }
}