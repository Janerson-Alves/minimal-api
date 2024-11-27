using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace minimalApi.Dominio.Entidades;
public class Administrador
{
    //cria um Id para a entidade
    [Key]
    //cria um Id auto incrementável para a entidade para o banco de dados
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    //cria um campo obrigatório para o email
    [Required]
    //cria um campo de tamanho máximo de 255 caracteres
    [StringLength(255)]
    public string Email { get; set; }

    //cria um campo de tamanho máximo de 50 caracteres
    //cria um campo obrigatório para a senha
    [Required]
    [StringLength(50)]
    public string Senha { get; set; }

    //cria um campo de tamanho máximo de 10 caracteres
    //cria um campo obrigatório para o perfil
    [StringLength(10)]
    [Required]
    public string Perfil { get; set; }

}
