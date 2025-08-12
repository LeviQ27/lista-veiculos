using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lista_veiculos.Dominio.ModelViews
{
    public struct Home
    {
        public string Mensagem { get => $"Bem-vindo ao sistema de gerenciamento de veículos! - {DateTime.Now.ToString("dd/MM/yyyy")} - Minimal API"; }
        public string Doc { get => "/swagger"; }
    }
}