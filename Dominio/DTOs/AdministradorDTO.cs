using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Dominio.Enuns;

namespace minimalApi.DTOs
{
//classe LoginDTO para receber os dados do login
    public class AdministradorDTO
    {
        public string Email { get; set; } = default;
        public string Senha { get; set; } = default;
        public Perfil? Perfil { get; set; } = default;
    }
}

