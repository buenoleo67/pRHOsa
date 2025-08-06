using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using pRHosaApp1.Services;
using pRHosaApp1.Services.Response;
using static pRHosaApp1.Pages.Tarefas;
using pRHosaApp1.Models;
using static System.Net.WebRequestMethods;

public class API
{
    private readonly HttpClient _httpClient;

    public API(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("API");
    }



    /**-----------------------------------------------------------------------------------------PREVISÃO DO TEMPO------------------------------------------------------------------------------------------*/

    public async Task<ICollection<ClimaResponse>?> GetClimaCidade(string cidade)
    {
        var url = $"/api/pessoas/getWeather?cityName={cidade}";
        return await _httpClient.GetFromJsonAsync<ICollection<ClimaResponse>>(url);
    }
    public async Task<ICollection<ClimaResponse>?> GetClimaAsync()
    {
        var url = $"/api/pessoas/propriedadesindicesdiarios";
        return await _httpClient.GetFromJsonAsync<ICollection<ClimaResponse>>(url);
    }

    /**-----------------------------------------------------------------------------------------PESSOAS------------------------------------------------------------------------------------------*/


    public async Task<ICollection<PessoasResponse>?> GetPessoasAsync()
    {
        return await _httpClient.GetFromJsonAsync<ICollection<PessoasResponse>>("/api/pessoas");
    }


    public async Task<ICollection<PessoasResponse>?> GetPessoaUser(string cpf)
    {
        var url = $"/api/pessoas/usuarioCadastrado?cpf={cpf}";
        return await _httpClient.GetFromJsonAsync<ICollection<PessoasResponse>>(url);
    }


    public async Task<ICollection<PropriedadesResponse>?> GetPropAsync()
    {
        return await _httpClient.GetFromJsonAsync<ICollection<PropriedadesResponse>>("/api/pessoas/pessoaXpropriedade");
    }


    public async Task<ICollection<PropriedadesResponse>?> GetPropUser(string cpf)
    {
        var url = $"/api/pessoas/pessoaXpropriedadeporUsuario?cpf={cpf}";
        return await _httpClient.GetFromJsonAsync<ICollection<PropriedadesResponse>>(url);
    }



    public async Task<ICollection<PropriedadesResponse>?> PostPessoa(string nome, string email, string cpf)
    {
        var url1 = $"/api/pessoas/registraPessoa?nome={Uri.EscapeDataString(nome)}&email={Uri.EscapeDataString(email)}&cpf={Uri.EscapeDataString(cpf)}";

        var response = await _httpClient.PostAsync(url1, null);

        if (response.IsSuccessStatusCode)
        {

            var url2 = $"/api/pessoas/Postuser?nome={Uri.EscapeDataString(nome)}&email={Uri.EscapeDataString(email)}";
            var novoUsuario = new
            {
                Nome = nome,
                Email = email
            };

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(novoUsuario), Encoding.UTF8, "application/json");

            var createBiUser = await _httpClient.PostAsync(url2, content);

            return await response.Content.ReadFromJsonAsync<ICollection<PropriedadesResponse>>();
        }

        return null;
    }

    public async Task<ICollection<PropriedadesResponse>?> DeletePessoa(string nome, string cpf)
    {
        var url = $"/api/pessoas/deletaPessoa?nome={Uri.EscapeDataString(nome)}&cpf={Uri.EscapeDataString(cpf)}";

        var response = await _httpClient.DeleteAsync(url);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ICollection<PropriedadesResponse>>();
        }

        return null;
    }
    /*------------------------------------------------------------pRHsa-----------------------------------------------------------------------------------*/
    public async Task<string> PostUsuario(string email, string senha)
    {
        var url = $"/api/usuarios/cadastrar?email={Uri.EscapeDataString(email)}&senha={Uri.EscapeDataString(senha)}";

        var response = await _httpClient.PostAsync(url, null);

        if (response.IsSuccessStatusCode)
            return "success"; // Usuário criado com sucesso
        else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            // Lê a mensagem de erro retornada pela API
            string errorMessage = await response.Content.ReadAsStringAsync();
            return errorMessage; // Retorna a mensagem real para exibição
        }
        else
        {
            return "error"; // Erro desconhecido
        }
    }
    public async Task<string> PostLogin(string email, string senha)
    {
        var url = $"/api/usuarios/login?email={Uri.EscapeDataString(email)}&senha={Uri.EscapeDataString(senha)}";

        var response = await _httpClient.PostAsync(url, null);

        if (response.IsSuccessStatusCode)
            return "success"; // Login bem-sucedido
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            return "invalid_credentials"; // Senha incorreta
        else
            return "error"; // Erro desconhecido
    }
    public async Task<UsuarioResponse?> GetUsuarioInfo(string email)
    {
        var url = $"/api/usuarios/info?email={Uri.EscapeDataString(email)}";

        return await _httpClient.GetFromJsonAsync<UsuarioResponse>(url);
    }
    public async Task<string> PostColaborador(ColaboradorRequest colaborador)
    {
        var url = "/api/colaboradores/cadastrar";
        var response = await _httpClient.PostAsJsonAsync(url, colaborador);

        if (response.IsSuccessStatusCode)
            return "success";

        string errorMessage = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"❌ Erro ao cadastrar colaborador. Status: {response.StatusCode}, Erro: {errorMessage}");
        return errorMessage;
    }

    public async Task<ColaboradorResponse?> GetColaboradorInfo(string email)
    {
        var url = $"/api/colaboradores/info?email={Uri.EscapeDataString(email)}";

        try
        {
            return await _httpClient.GetFromJsonAsync<ColaboradorResponse>(url);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Erro ao buscar colaborador: {ex.Message}");
            return null; // Retorna null se o colaborador não for encontrado
        }
    }
    public async Task<string> PostTarefa(string email, TarefaRequest tarefa)
    {
        var url = $"/api/tarefas/cadastrar?email={Uri.EscapeDataString(email)}";

        var response = await _httpClient.PostAsJsonAsync(url, tarefa);

        if (response.IsSuccessStatusCode)
            return "success"; // Cadastro bem-sucedido
        else
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            return errorMessage;
        }
    }

    public async Task<List<TarefaResponse>?> GetTarefasUsuario(string email)
    {
        var url = $"/api/tarefas/usuario?email={Uri.EscapeDataString(email)}";

        try
        {
            return await _httpClient.GetFromJsonAsync<List<TarefaResponse>>(url);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Erro ao buscar tarefas do usuário: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Comentario>?> GetComentariosPorTarefa(int tarefaId)
    {
        var url = $"/api/tarefas/{tarefaId}/comentarios";

        try
        {
            return await _httpClient.GetFromJsonAsync<List<Comentario>>(url);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Erro ao buscar comentários da tarefa {tarefaId}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> AdicionarComentarioAsync(int tarefaId, Comentario comentario)
    {
        var url = $"/api/tarefas/{tarefaId}/Postcomentarios";

        var response = await _httpClient.PostAsJsonAsync(url, comentario);

        if (response.IsSuccessStatusCode)
            return true;

        Console.WriteLine($"❌ Falha ao adicionar comentário à tarefa {tarefaId}: {await response.Content.ReadAsStringAsync()}");
        return false;
    }
    /*  public async Task<List<ColaboradorInfo>?> GetColaboradoresAsync()
      {
          try
          {
              return await _httpClient.GetFromJsonAsync<List<ColaboradorInfo>>("/api/colaboradores");
          }
          catch (HttpRequestException ex)
          {
              Console.WriteLine($"❌ Erro ao buscar colaboradores: {ex.Message}");
              return null;
          }
      }*/
    public async Task<List<ColaboradorInfo>?> GetColaboradoresAsync(int empresaId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<ColaboradorInfo>>($"/api/colaboradores?empresaId={empresaId}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Erro ao buscar colaboradores: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> AtualizarStatusTarefaAsync(int tarefaId, string novoStatus)
    {
        var url = $"/api/tarefas/{tarefaId}/status?novoStatus={Uri.EscapeDataString(novoStatus)}";
        var response = await _httpClient.PutAsync(url, null);
        return response.IsSuccessStatusCode;
    }


    public async Task<bool> AtualizarResponsavelTarefaAsync(int tarefaId, int responsavelId)
    {
        var url = $"/api/tarefas/{tarefaId}/responsavel?responsavelId={responsavelId}";
        var response = await _httpClient.PutAsync(url, null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PostNecessidade(string emailUsuario, NecessidadeRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/necessidades?usuario={emailUsuario}", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao enviar necessidade: {ex.Message}");
            return false;
        }
    }
    public async Task<List<NecessidadeResponse>?> GetNecessidadesUsuario(string emailUsuario)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/necessidades?usuario={emailUsuario}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<NecessidadeResponse>>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar necessidades: {ex.Message}");
        }

        return null;
    }
    public async Task<bool> AtualizarNotaSatisfacaoColaborador(int necessidadeId, int nota)
    {
        var url = $"/api/necessidades/{necessidadeId}/nota-colaborador?nota={nota}";
        var response = await _httpClient.PutAsync(url, null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AtualizarNotaSatisfacaoRH(int necessidadeId, int nota)
    {
        var url = $"/api/necessidades/{necessidadeId}/nota-rh?nota={nota}";
        var response = await _httpClient.PutAsync(url, null);
        return response.IsSuccessStatusCode;
    }


    public async Task<List<TipoNecessidade>?> GetTiposNecessidade()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/tipos-necessidade");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<TipoNecessidade>>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar tipos de necessidade: {ex.Message}");
        }

        return null;
    }
    public async Task<bool> PostEmpresa(Empresa empresa)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/empresas", empresa);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao salvar empresa: {ex.Message}");
            return false;
        }
    }

    public async Task<List<Empresa>?> GetEmpresasAsync(string email)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/empresas?email={Uri.EscapeDataString(email)}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<Empresa>>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar empresas: {ex.Message}");
        }

        return null;
    }
    public async Task<bool> PostCargo(Cargo cargo)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/cargos/cadastrar", cargo);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao enviar cargo: {ex.Message}");
            return false;
        }
    }
    public async Task<bool> PostTipoNecessidade(TipoNecessidade tipo)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/tiposnecessidade/cadastrar", tipo);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao cadastrar tipo de necessidade: {ex.Message}");
            return false;
        }
    }
    public async Task<List<TipoNecessidade>?> GetTiposNecessidade(string email)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/tipos-necessidade?email={Uri.EscapeDataString(email)}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<TipoNecessidade>>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar tipos de necessidade: {ex.Message}");
        }

        return null;
    }
    public async Task<List<Cargo>?> GetCargosPorEmail(string email)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/cargos?email={Uri.EscapeDataString(email)}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<Cargo>>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar cargos: {ex.Message}");
        }

        return null;
    }

    public async Task<bool> AtualizarEmpresa(Empresa empresa)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("/api/empresas", empresa);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao atualizar empresa: {ex.Message}");
            return false;
        }
    }
    public async Task<bool> VerificarDemoExpirada(string emailUsuario)
    {
        var empresas = await GetEmpresasAsync(emailUsuario);
        var empresa = empresas?.FirstOrDefault();

        if (empresa?.DataInicioDemo != null)
        {
            var diasAtivos = (DateTime.UtcNow - empresa.DataInicioDemo.Value).TotalDays;
            return diasAtivos > 30;
        }
        return false;
    }

    // NOVO MÉTODO
    public async Task<string> PostUsuarioCompleto(string email, string senha, string empresaNome)
    {
        var payload = new { email, senha, empresaNome };
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/usuarios/registrocompleto", content);

        if (response.IsSuccessStatusCode)
            return "success";
        else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            return errorMessage;
        }
        else
            return "error";
    }
    public async Task<bool> AtualizarPrioridadeNecessidade(int necessidadeId, int novaPrioridade)
    {
        var url = $"/api/necessidades/{necessidadeId}/prioridade?novaPrioridade={novaPrioridade}";
        var response = await _httpClient.PutAsync(url, null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AtualizarComplexidadeNecessidade(int necessidadeId, int novaComplexidade)
    {
        var url = $"/api/necessidades/{necessidadeId}/complexidade?novaComplexidade={novaComplexidade}";
        var response = await _httpClient.PutAsync(url, null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PostAlertaNecessidade(NecessidadeAlerta alerta)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/necessidades/{alerta.NecessidadeId}/alertas", alerta);
        return response.IsSuccessStatusCode;
    }
    public async Task<List<NecessidadeAlerta>> GetAlertasPorNecessidade(int necessidadeId)
    {
        var response = await _httpClient.GetAsync($"/api/necessidades/{necessidadeId}/alertas");

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<List<NecessidadeAlerta>>() ?? new();

        return new List<NecessidadeAlerta>();
    }
    public async Task<IEnumerable<NecessidadeAlerta>> ObterAlertasNecessidade(int necessidadeId)
    {
        var response = await _httpClient.GetAsync($"/api/necessidades/{necessidadeId}/alertas");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<NecessidadeAlerta>>() ?? new List<NecessidadeAlerta>();
        }

        return new List<NecessidadeAlerta>();
    }
    public async Task<List<TarefasPorStatusResponse>> GetTarefasPorStatus()
    {
        var response = await _httpClient.GetAsync("/api/dashboard/tarefas-por-status");

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<List<TarefasPorStatusResponse>>() ?? new();

        return new List<TarefasPorStatusResponse>();
    }


}
public class TarefasPorStatusResponse
{
    public string Status { get; set; } = "";
    public int Total { get; set; }
}


public class UsuarioResponse
{
    public int Id { get; set; } // Se for INT na API, deve ser INT aqui também
    public string Email { get; set; }
    public DateTime DataCadastro { get; set; }
}
public class ColaboradorRequest
{
    public string Nome { get; set; }
    public string CPF { get; set; }
    public int TipoUsuario { get; set; }
    public string Email { get; set; }
    public DateTime DataNascimento { get; set; }
    public string Endereco { get; set; }
    public string Cargo { get; set; }

    public int EmpresaId { get; set; }
    public DateTime DataAdmissao { get; set; }
    public string Genero { get; set; }
}
public class ColaboradorResponse
{
    public string Nome { get; set; }
    public string Cargo { get; set; }

    public int EmpresaId { get; set; }
    public int TipoUsuario { get; set; }
}

public class Cargo
{
    public int CargoId { get; set; }
    public string Nome { get; set; }
    public int EmpresaId { get; set; }
}

public class NecessidadeAlerta
{
    public int Id { get; set; }
    public int NecessidadeId { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.Now;
}





