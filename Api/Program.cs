using Microsoft.EntityFrameworkCore;
using minimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Interfaces;
using minimalApi.DTOs;
using minimalApi.Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using MinimalApi.Dominio.ModelViews;
using minimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

#region Builder
// Cria um novo builder para a aplicação
var builder = WebApplication.CreateBuilder(args);

// Configura o builder para usar o arquivo appsettings.json o jwtsettings para a classe JwtSettings e a chave para o JWT Bearer Authentication Scheme
var key = builder.Configuration.GetSection("Jwt").ToString();

if(!string.IsNullOrEmpty(key)) key = "123456";


//Configurando o builder para usar o JWT Bearer Authentication Scheme para autenticação de usuários
builder.Services.AddAuthentication(option => {
    //configura o DefaultAuthenticateScheme e o DefaultChallengeScheme para JwtBearerDefaults.AuthenticationScheme
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//adiciona o JwtBearerOptions para o esquema de autenticação
}).AddJwtBearer(option => {
    //configura o token validation parameters para validar o tempo de vida do token e a chave de segurança
    option.TokenValidationParameters = new TokenValidationParameters{
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        //configura o ValidateIssuer e o ValidateAudience para false
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
//Configura o Builder para autorização de usuários na aplicação
builder.Services.AddAuthorization();

// Adiciona um serviço para a interface IAdministradorServico e a classe AdministradorServico
builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();

// Adiciona um serviço para a interface IVeiculoServico e a classe VeiculoServico
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();
// Adiciona um service endpoint para o swagger e a documentação da API
builder.Services.AddEndpointsApiExplorer();
// Adiciona um service para tornar a aplicação autenticável e autorizável com JWT
builder.Services.AddSwaggerGen(options => {
    // Adiciona um esquema de segurança para a aplicação
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT desta maneira: Bearer {seu token}"
    });
    // Adiciona um esquema de segurança para a aplicação
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// Adiciona o swagger para a aplicação
builder.Services.AddSwaggerGen();

//Cria a conexão com o banco de dados
builder.Services.AddDbContext<DbContexto>(options => {
    //configura o banco de dados para MySQL
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Mysql"))
    );
});
//cria a aplicação
var app = builder.Build();
#endregion

#region Home
// A home vai responder com um JSON contendo a documentação da API da nossa ModelView Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion


#region Administradores
string GerarTokenJwt(Administrador administrador)
{
    // verifica se o administrador é nulo
    if(!string.IsNullOrEmpty(key)) return string.Empty;
    //cria um novo token para o administrador na hora de fazer login e as credenciais para gerar o token
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    //cria uma lista de claims para o administrador
    var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil)
    };
    //cria um novo token com a data de expiração de 1 dia e as credenciais
    var token = new JwtSecurityToken(
        claims : claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials

    );
    //retorna o token gerado
    return new JwtSecurityTokenHandler().WriteToken(token);
}

//rota para fazer login no sistema com email e senha e retorna o administrador logado ou erro de autenticação
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {

    var adm = administradorServico.Login(loginDTO);
    //verifica se o login foi feito com sucesso

    if(adm != null){
        string token = GerarTokenJwt(adm);
        return Results.Ok(new AdministradorLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
        //retorna erro de autenticação
        return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administradores");

//rota para trazer todos os administradores cadastrados no sistema
app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) => {
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);

    foreach(var adm in administradores)
    {
        adms.Add(new AdministradorModelView{
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }
    //retorna a lista de administradores
    return Results.Ok(adms);
}).RequireAuthorization().
RequireAuthorization(new AuthorizeAttribute { Roles = "adm"})
.WithTags("Administradores");

// Rpta para traer 1 administrador por Id
app.MapGet("/administradores/{id}", ([FromRoute]int id, IAdministradorServico administradorServico) => {
    //busca o administrador por Id
    var administrador = administradorServico.BuscarPorId(id);
    //verifica se o administrador é nulo
    if(administrador == null) return Results.NotFound();
    //retorna OK com o administrador encontrado
    return Results.Ok(new AdministradorModelView{
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm"})
.WithTags("Administradores");


//rota para fazer login no sistema com email e senha e retorna o administrador logado ou erro de autenticação
app.MapPost("/administradores/", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) => {
   // Cria uma nova instância de ErrosDeValidacao para armazenar as mensagens de erro
    var validacao = new ErrosDeValidacao{
        Mensagens = new List<string>()
    };

    //verifica se o email é nulo ou vazio
    if(string.IsNullOrEmpty(administradorDTO.Email))
        validacao.Mensagens.Add("Email não pode ser vazio");
    //verifica se a senha é nula ou vazia
    if(string.IsNullOrEmpty(administradorDTO.Senha))
        validacao.Mensagens.Add("Senha não pode ser vazio");
    //verifica se o perfil é nulo ou vazio
    if(administradorDTO.Perfil == null)
        validacao.Mensagens.Add("Perfil não pode ser vazio");

    //verifica se a validação tem mensagens
    if(validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);
    
    //cria um novo veículo com os dados do veículoDTO
    var administrador = new Administrador{
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };


    //inclui o veículo no banco de dados
    administradorServico.Incluir(administrador);
    //retorna o veículo criado com o Id passando a rota para o veículo url e o veículo criado em JSON.
    return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView{
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
}).RequireAuthorization().
RequireAuthorization(new AuthorizeAttribute { Roles = "Adm"})
.WithTags("Administradores");
#endregion

#region Veiculos
// Função para validar o DTO de veículo
ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    //cria uma nova instância de ErrosDeValidacao
    var validacao = new ErrosDeValidacao{Mensagens = new List<string>()};
    //verifica se o nome do veículo é nulo ou vazio
    if (string.IsNullOrEmpty(veiculoDTO.Nome))
        validacao.Mensagens.Add("O Nome não pode ser vazio");
    //verifica se a marca do veículo é nula ou vazia
    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        validacao.Mensagens.Add("A Marca não pode ficar em branco");
    //verifica se o ano do veículo é menor que 1950
    if (veiculoDTO.Ano < 1950)
        validacao.Mensagens.Add("Veículo muito antigo, aceito somente anos superiores a 1950");
    //retorna a validação
    return validacao;
}


//rota para listar todos os veículos cadastrados no sistema e retorna uma lista de veículos
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {

    //valida o DTO do veículo
    var validacao = validaDTO(veiculoDTO);
    //verifica se a validação tem mensagens
    if(validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    //cria um novo veículo com os dados do veículoDTO
    var veiculo = new Veiculo{
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    //inclui o veículo no banco de dados
    veiculoServico.Incluir(veiculo);
    //retorna o veículo criado com o Id passando a rota para o veículo url e o veículo criado em JSON.
    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor"})
.WithTags("Veiculos");

//Rota para busca uma lista de veículos com paginação e filtro por nome e marca do veículo
app.MapGet("/veiculos", ([FromQuery]int? pagina, IVeiculoServico veiculoServico) => {
    var veiculos = veiculoServico.Todos(pagina);
    //retorna o veículo criado com o Id passando a rota para o veículo url e o veículo criado em JSON.
    return Results.Ok(veiculos);
}).RequireAuthorization().WithTags("Veiculos");

//Rota de buscar 1 veiculo por Id
app.MapGet("/veiculos/{id}", ([FromRoute]int id, IVeiculoServico veiculoServico) => {
    //busca um veículo por Id
    var veiculo = veiculoServico.BuscarPorId(id);
    //verifica se o veículo é nulo
    if(veiculo == null)
        return Results.NotFound();
    //retorna um veículo criado com o Id passando a rota para o veículo url e o veículo criado em JSON.
    return Results.Ok(veiculo);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor"})
.WithTags("Veiculos");

// Rpta para alterar um veículo
app.MapPut("/veiculos/{id}", ([FromRoute]int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {

    //valida o DTO do veículo
    var validacao = validaDTO(veiculoDTO);
    //verifica se a validação tem mensagens
    if(validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    //busca um veículo por Id
    var veiculo = veiculoServico.BuscarPorId(id);
    //verifica se o veículo é nulo
    if(veiculo == null)
        return Results.NotFound();

    //atualiza o veículo com os dados do veículoDTO
    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    //atualiza o veículo no banco de dados
    veiculoServico.Atualizar(veiculo);

    //retorna um veículo criado com o Id passando a rota para o veículo url e o veículo criado em JSON.
    return Results.Ok(veiculo);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm"})
.WithTags("Veiculos");

// Rpta para apagar um veículo
app.MapDelete("/veiculos/{id}", ([FromRoute]int id, IVeiculoServico veiculoServico) => {
    //busca um veículo por Id
    var veiculo = veiculoServico.BuscarPorId(id);
    //verifica se o veículo é nulo
    if(veiculo == null)
        return Results.NotFound();

    //Apaga o veículo no banco de dados
    veiculoServico.Apagar(veiculo);
    
    //Retorna um status 204 No Content vazio
    return Results.NoContent();
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm"})
.WithTags("Veiculos");
#endregion
#region App
//configura o swagger para a aplicação
app.UseSwagger();
app.UseSwaggerUI();

//configura a aplicação para usar autenticação
app.UseAuthentication();
app.UseAuthorization();

//roda a aplicação
app.Run();
#endregion
