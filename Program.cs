using Microsoft.EntityFrameworkCore;
using minimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Interfaces;
using minimalApi.DTOs;
using minimalApi.Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;   

// Cria um novo builder para a aplicação
var builder = WebApplication.CreateBuilder(args);

// Adiciona um serviço para a interface IAdministradorServico e a classe AdministradorServico
builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();

//Cria a conexão com o banco de dados
builder.Services.AddDbContext<DbContexto>(options => {
    //configura o banco de dados para MySQL
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();



app.MapGet("/", () => "Hello World!");

//rota para fazer login no sistema com email e senha e retorna o administrador logado ou erro de autenticação
app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
    //verifica se o login foi feito com sucesso
    if(administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login com Successo");
    else
        //retorna erro de autenticação
        return Results.Unauthorized();
});

//roda a aplicação
app.Run();

