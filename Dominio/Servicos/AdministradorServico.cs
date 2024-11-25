using minimalApi.DTOs;
using minimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Interfaces;
using minimalApi.Dominio.Entidades;

namespace minimalApi.Dominio.Servicos;

//Classe para o serviço de administrador
public class AdministradorServico : IAdministradorServico
{
    //Cria um contexto para o banco de dados
    private readonly DbContexto _contexto;

    //Construtor para o serviço de administrador
    public AdministradorServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

     public Administrador? BuscarPorId(int id)
    {
        return _contexto.Administrador.Where(v => v.Id == id).FirstOrDefault();
    }

    //Método para incluir um novo administrador no sistema
    public Administrador Incluir(Administrador administrador)
    {
        //Adiciona o administrador no banco de dados e salva as alterações
        _contexto.Administrador.Add(administrador);
        _contexto.SaveChanges();

        return administrador;
    }

    //Método para fazer login de um administrador no sistema com email e senha e retorna o administrador logado
    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administrador.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;
    }

    public List<Administrador> Todos(int? pagina)
    {
        //cria uma query para a entidade Administrador
        var query = _contexto.Administrador.AsQueryable();
        
        //mostrar 10 administradores por página
        int itensPorPagina = 10;

        //verifica se o administrador não é nula ou vazia
        if(pagina != null)
        {
        // faz a conta para a paginação dos administradores e pula os administradores que já foram listados
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        }
        //retorna a lista de veículos
        return query.ToList();
    }
}