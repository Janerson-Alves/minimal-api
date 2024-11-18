using minimalApi.DTOs;
using minimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Interfaces;
using minimalApi.Dominio.Entidades; // Adicione esta linha

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

    //Método para fazer login de um administrador no sistema com email e senha e retorna o administrador logado
    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administrador.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;
    }
}