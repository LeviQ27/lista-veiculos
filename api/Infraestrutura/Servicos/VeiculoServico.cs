using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lista_veiculos.Dominio.Entidades;
using lista_veiculos.Dominio.interfaces;
using lista_veiculos.Infraestrutura.Db;

namespace lista_veiculos.Infraestrutura.Servicos
{
    public class VeiculoServico : IVeiculoServico
    {
        private readonly DbContexto _contexto;

        public VeiculoServico(DbContexto db)
        {
            _contexto = db;
        }

        public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
        {
            var query = _contexto.Veiculos.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(v => v.Nome.Contains(nome));
            }

            if (!string.IsNullOrEmpty(marca))
            {
                query = query.Where(v => v.Marca.Contains(marca));
            }

            return query.Skip(((pagina ?? 1) - 1) * 10).Take(10).ToList();
        }

        public Veiculo BuscaPorId(int id)
        {
            return _contexto.Veiculos.Find(id) ?? throw new KeyNotFoundException("Veículo não encontrado.");
        }

        public void Adicionar(Veiculo veiculo)
        {
            _contexto.Veiculos.Add(veiculo);
            _contexto.SaveChanges();
        }

        public void Atualizar(Veiculo veiculo)
        {
            _contexto.Veiculos.Update(veiculo);
            _contexto.SaveChanges();
        }

        public void Apagar(Veiculo veiculo)
        {
            _contexto.Veiculos.Remove(veiculo);
            _contexto.SaveChanges();
        }
    }
}