using System.Text.Json.Serialization;

namespace pRHosaApp1.Services.Response
{
    public class PessoasResponse
    {
        [JsonPropertyName("CodigoCliente")]
        public string CodigoCliente { get; set; } = string.Empty;

        [JsonPropertyName("NomeCliente")]
        public string PessoaNome { get; set; } = string.Empty;

        [JsonPropertyName("E-Mail")]
        public string? PessoaEmail { get; set; }

       
        

    }

}

