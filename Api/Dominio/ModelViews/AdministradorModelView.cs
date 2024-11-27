namespace MinimalApi.Dominio.ModelViews;

// Classe de modelo para a Home
public record AdministradorModelView
{
    public int Id { get; set; } = default;
    public string Email { get; set; } = default;
    public string Perfil { get; set; } = default;
}

