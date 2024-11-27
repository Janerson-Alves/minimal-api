using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimalApi.DTOs
{
    //Classe de modelo para o veículo em record para ser imutável
    public record VeiculoDTO
    {
        //Nome do veículo
        public string Nome { get; set; } = default!;
        //Marca do veículo
        public string Marca { get; set; } = default!;
        //Ano do veículo
        public int Ano { get; set; } = default!;
    }
}

