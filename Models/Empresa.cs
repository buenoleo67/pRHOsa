namespace pRHosaApp1.Models
{
    public class Empresa
    {
        public int EmpresaId { get; set; }             // Identificador único
        public string Nome { get; set; } = string.Empty;  // Nome da empresa
        public string CNPJ { get; set; } = string.Empty;  // CNPJ
        public string Endereco { get; set; } = string.Empty; // Endereço
        public string? Telefone { get; set; }          // Telefone (opcional)
        public string? Email { get; set; }             // Email (opcional)
        public string? RepresentanteLegal { get; set; } // Representante Legal (opcional)
        public DateTime? DataFundacao { get; set; }    // Data de fundação (opcional)
        public DateTime? DataInicioDemo { get; set; }  // Data de início do modo demo
        public bool PlanoAtivo { get; set; }           // Indica se o plano já foi ativado
    }
}
