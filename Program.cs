var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

//rota para login com autenticação simples de email e senha
app.MapPost("/login", (minimalApi.DTOs.LoginDTO loginDTO) => {
    //verifica se o email e senha são válidos
    if(loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")
        return Results.Ok("Login com Successo");
    else
        //retorna erro de autenticação
        return Results.Unauthorized();
});

//roda a aplicação
app.Run();

