using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lista_veiculos.Dominio.ModelViews
{
    public struct ErrosDeValidacao
    {
        public ErrosDeValidacao()
        {
        }

        public List<string> Mensagens { get; set; } = new List<string>();
    }
}