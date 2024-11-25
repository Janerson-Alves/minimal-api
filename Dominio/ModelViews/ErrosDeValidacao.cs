using Microsoft.AspNetCore.SignalR;

namespace MinimalApi.Dominio.ModelViews;

// Classe de modelo para a Home
public struct ErrosDeValidacao
{
    // Propriedades da classe que vai ser usada para retornar erros de validação
    public List<string> Mensagens { get; set; }
}

