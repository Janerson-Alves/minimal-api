using minimalApi.Dominio.Entidades;
using minimalApi.DTOs;

namespace MinimalApi.Dominio.Interfaces;

//Interface para o serviço de administrador
public interface IAdministradorServico
{
    //Método para fazer login de um administrador no sistema com email e senha e retorna o administrador logado
    Administrador? Login(LoginDTO loginDTO);
    //Método para incluir um novo administrador no sistema
    Administrador? Incluir(Administrador administrador);

    Administrador? BuscarPorId(int id);

    //Método para listar todos os administradores cadastrados no sistema
    List< Administrador> Todos(int? pagina);
    
}

