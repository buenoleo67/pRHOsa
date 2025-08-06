namespace pRHosaApp1.Services.Response;
    using System.Text.Json.Serialization;
    public class ClimaResponse
    {
        

        [JsonPropertyName("CodigoCliente")]
        public string CodigoCliente { get; set; } = string.Empty;

        [JsonPropertyName("PessoaNome")]
        public string PessoaNome { get; set; } = string.Empty;

        [JsonPropertyName("PessoaCPF")]
        public string PessoaCPF { get; set; } = string.Empty;

        [JsonPropertyName("ProprietarioId")]
        public string? ProprietarioId { get; set; }

        [JsonPropertyName("PropriedadeNome")]
        public string? PropriedadeNome { get; set; }

        [JsonPropertyName("MunicipioPropriedade")]
        public string? MunicipioPropriedade { get; set; }

        [JsonPropertyName("UF")]
        public string? UF { get; set; }

        [JsonPropertyName("CAR")]
        public string? CAR { get; set; }

        [JsonPropertyName("LATITUDE")]
        public string? LATITUDE { get; set; }

        [JsonPropertyName("LONGITUDE")]
        public string? LONGITUDE { get; set; }

}
