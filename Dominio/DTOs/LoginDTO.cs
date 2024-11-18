using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimalApi.DTOs
{
//classe LoginDTO para receber os dados do login
    public class LoginDTO
    {
        public string Email { get; set; } = default;
        public string Senha { get; set; } = default;
    }
}
