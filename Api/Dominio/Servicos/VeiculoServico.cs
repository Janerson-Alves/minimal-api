using minimalApi.DTOs;
using minimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Interfaces;
using minimalApi.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace minimalApi.Dominio.Servicos;

//Classe para o serviço de administrador
public class VeiculoServico : IVeiculoServico
{
    //Cria um contexto para o banco de dados
    private readonly DbContexto _contexto;

    //Construtor para o serviço de administrador
    public VeiculoServico(DbContexto contexto)
    {
        _contexto = contexto;
    }
    //Apaga um veículo
    public void Apagar(Veiculo veiculo)
    {
        _contexto.Veiculos.Remove(veiculo);
        _contexto.SaveChanges();
    }
    //Atualiza um veículo
    public void Atualizar(Veiculo veiculo)
    {
        _contexto.Veiculos.Update(veiculo);
        _contexto.SaveChanges();
    }
    //Busca um veículo por Id
    public Veiculo? BuscarPorId(int id)
    {
        return _contexto.Veiculos.Where(v => v.Id == id).FirstOrDefault();
    }
    //Inclui um veículo
    public void Incluir(Veiculo veiculo)
    {
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();
    }
    //Lista todos os veículos com paginação e filtro por nome e marca
    public List<Veiculo> Todos(int? pagina = 1, string nome = null, string marca = null)
    {
        //cria uma query para a entidade Veiculo
        var query = _contexto.Veiculos.AsQueryable();
        //verifica se o nome não é nulo ou vazio
        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome.ToLower()}%"));
        }
        //verifica se a marca não é nula ou vazia
        int itensPorPagina = 10;
        
        //verifica se a marca não é nula ou vazia
        if(pagina != null)
        {
        // faz a conta para a paginação dos veículos e pula os veículos que já foram listados
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        }
        //retorna a lista de veículos
        return query.ToList();
    }
}