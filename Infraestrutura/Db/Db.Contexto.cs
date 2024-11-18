using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using minimalApi.Dominio.Entidades;

namespace minimalApi.Infraestrutura.Db;

//herda de DbContext para criar o contexto do banco de dados que vem do Entity Framework Core
public class DbContexto : DbContext
{
    //cria um construtor para receber a configuração para injecao de dependência
    private readonly IConfiguration _configuracaoAppSettings;
    //Criando o construtor
    public DbContexto(IConfiguration configuracaoAppSettings)
    {
        //atribui a configuração para a variável _configuracaoAppSettings
        _configuracaoAppSettings = configuracaoAppSettings;
    }

    //Cria um DbSet para a entidade Administrador
    public DbSet<Administrador> Administrador { get; set; } = default!;

    public DbSet<Veiculo> Veiculos { get; set; } = default!;


    //Cria um método para configurar o modelo do banco de dados
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Cria um novo administrador para ser inserido no banco de dados
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador {
                Id = 1,
                Email = "administrador@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            }
        );
    }


    //cria um DbSet para a entidade Cliente alterando o nome da tabela para Clientes
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //verifica se o banco de dados não foi configurado
        if(!optionsBuilder.IsConfigured)
        {

            //cria uma variável para receber a string de conexão do arquivo appsettings.json
            var stringConexao = _configuracaoAppSettings.GetConnectionString("mysql")?.ToString();

            //verifica se a string de conexão não é nula ou vazia
            if(!string.IsNullOrEmpty(stringConexao))
            {
                //configura o banco de dados para MySQL
                optionsBuilder.UseMySql(
                    stringConexao, 
                    ServerVersion.AutoDetect(stringConexao)
                );
            }
        }

    }
}