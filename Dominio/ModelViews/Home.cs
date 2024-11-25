using Microsoft.AspNetCore.SignalR;

namespace MinimalApi.Dominio.ModelViews;

// Classe de modelo para a Home
public struct Home
{
    // Propriedades da Home que mostram a mensagem de boas vindas e o link para a documentação
    public string Mensagem { get => "Bem vindo a API de veículos - Minimal API"; }
    // Link para a documentação da API
    public string Doc { get => "/swagger"; }
}