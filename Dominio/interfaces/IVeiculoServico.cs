using minimalApi.Dominio.Entidades;
using minimalApi.DTOs;

namespace MinimalApi.Dominio.Interfaces;

//Interface para o serviço de administrador
public interface IVeiculoServico
{
    //cria uma lista de veículos com paginação e filtro por nome e marca do veículo
    List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null);

    //cria um veículo por Id
    Veiculo? BuscarPorId(int id);

    // inclui um veículo
    void Incluir(Veiculo veiculo);

    // atualiza um veículo
    void Atualizar(Veiculo veiculo);

    // apaga um veículo
    void Apagar(Veiculo veiculo);
}

