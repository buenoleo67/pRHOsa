using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using pRHosaApp1.Services.Response;
using pRHosaApp1.Models;

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
        try
        {
            return await _httpClient.GetFromJsonAsync<ICollection<PessoasResponse>>(url);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Erro ao buscar pessoa por CPF: {ex.Message}");
            return null;
        }
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

        try
        {
            var response = await _httpClient.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
                return "success";
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return await ReadApiErrorMessageAsync(response, "Ja existe um usuario cadastrado com este e-mail.");
            }
            else
            {
                return await ReadApiErrorMessageAsync(response, "Erro ao registrar usuario.");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Erro de conectividade ao cadastrar usuario: {ex.Message}");
            return "network_error";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro inesperado ao cadastrar usuario: {ex.Message}");
            return "unexpected_error";
        }
    }
    public async Task<LoginResult> PostLogin(string email, string senha)
    {
        var url = $"/api/usuarios/login?email={Uri.EscapeDataString(email)}&senha={Uri.EscapeDataString(senha)}";

        try
        {
            var response = await _httpClient.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                var rawContent = await response.Content.ReadAsStringAsync();
                LoginApiResponse? loginResponse = null;

                if (!string.IsNullOrWhiteSpace(rawContent))
                {
                    try
                    {
                        loginResponse = JsonSerializer.Deserialize<LoginApiResponse>(rawContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                    catch (JsonException)
                    {
                    }
                }

                return new LoginResult
                {
                    Success = true,
                    Email = loginResponse?.Email ?? email,
                    Message = loginResponse?.Message
                        ?? (string.IsNullOrWhiteSpace(rawContent) ? "Login realizado com sucesso." : rawContent)
                };
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return new LoginResult
                {
                    Success = false,
                    ErrorCode = "invalid_credentials",
                    Message = "Credenciais invalidas."
                };
            }

            var apiError = await ReadApiErrorMessageAsync(response, "Erro ao realizar login.");
            return new LoginResult
            {
                Success = false,
                ErrorCode = "error",
                Message = apiError
            };
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Erro de conectividade ao realizar login: {ex.Message}");
            return new LoginResult
            {
                Success = false,
                ErrorCode = "network_error",
                Message = "Nao foi possivel conectar com a API de autenticacao."
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro inesperado ao realizar login: {ex.Message}");
            return new LoginResult
            {
                Success = false,
                ErrorCode = "unexpected_error",
                Message = "Ocorreu um erro inesperado ao realizar login."
            };
        }
    }

    private static string? NormalizeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        return email.Trim().ToLowerInvariant();
    }

    private static async Task<string> ReadApiErrorMessageAsync(HttpResponseMessage response, string fallbackMessage)
    {
        try
        {
            var rawContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(rawContent))
            {
                return fallbackMessage;
            }

            try
            {
                using var document = JsonDocument.Parse(rawContent);
                var root = document.RootElement;

                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String)
                    {
                        var detailText = detail.GetString();
                        if (!string.IsNullOrWhiteSpace(detailText))
                        {
                            return detailText!;
                        }
                    }

                    if (root.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
                    {
                        var titleText = title.GetString();
                        if (!string.IsNullOrWhiteSpace(titleText))
                        {
                            return titleText!;
                        }
                    }

                    if (root.TryGetProperty("message", out var message) && message.ValueKind == JsonValueKind.String)
                    {
                        var messageText = message.GetString();
                        if (!string.IsNullOrWhiteSpace(messageText))
                        {
                            return messageText!;
                        }
                    }
                }
            }
            catch (JsonException)
            {
            }

            return rawContent;
        }
        catch
        {
            return fallbackMessage;
        }
    }

    public async Task<UsuarioResponse?> GetUsuarioInfo(string email)
    {
        var url = $"/api/usuarios/info?email={Uri.EscapeDataString(email)}";

        try
        {
            var response = await _httpClient.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UsuarioResponse>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Erro ao buscar usuario: {ex.Message}");
            return null;
        }
    }
    public async Task<List<UsuarioGestaoInfo>?> GetUsuariosGestao(string email)
    {
        var url = $"/api/usuarios?email={Uri.EscapeDataString(email)}";

        try
        {
            return await _httpClient.GetFromJsonAsync<List<UsuarioGestaoInfo>>(url);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Erro ao buscar usuários: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> AtualizarUsuarioGestao(UsuarioColaboradorUpdateRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("/api/usuarios/gestao", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao atualizar usuario/colaborador: {ex.Message}");
            return false;
        }
    }

    public async Task<string> CriarAcessoVinculado(AcessoColaboradorRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/usuarios/vincular-acesso", request);
            if (response.IsSuccessStatusCode)
            {
                return "success";
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao criar acesso vinculado: {ex.Message}");
            return "Erro ao criar acesso vinculado.";
        }
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
            var response = await _httpClient.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ColaboradorResponse>();
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
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<TarefaResponse>>();
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Erro ao buscar tarefas do usuário. Status: {response.StatusCode}, Erro: {errorMessage}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Erro ao buscar tarefas do usuário: {ex.Message}");
        }

        return null;
    }

    public async Task<List<TarefaResponse>?> GetTarefasPainel(string email)
    {
        var url = $"/api/tarefas/painel?email={Uri.EscapeDataString(email)}";

        try
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<TarefaResponse>>();
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Erro ao buscar tarefas do painel. Status: {response.StatusCode}, Erro: {errorMessage}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine("ℹ️ Endpoint de painel de tarefas indisponível. Tentando fallback para /api/tarefas/usuario.");
                return await GetTarefasUsuario(email);
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Erro ao buscar tarefas do painel: {ex.Message}");
        }

        return await GetTarefasUsuario(email);
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

    public async Task<string> PostNecessidade(string emailUsuario, NecessidadeRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/necessidades?usuario={Uri.EscapeDataString(emailUsuario)}", request);

            if (response.IsSuccessStatusCode)
                return "success";

            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Erro ao enviar necessidade. Status: {response.StatusCode}, Erro: {errorMessage}");
            return string.IsNullOrWhiteSpace(errorMessage)
                ? "Nao foi possivel salvar a necessidade."
                : errorMessage;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao enviar necessidade: {ex.Message}");
            return "Nao foi possivel salvar a necessidade.";
        }
    }
    public async Task<List<NecessidadeResponse>?> GetNecessidadesUsuario(string emailUsuario)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/necessidades?usuario={Uri.EscapeDataString(emailUsuario)}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<NecessidadeResponse>>();
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Erro ao buscar necessidades do usuário. Status: {response.StatusCode}, Erro: {errorMessage}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar necessidades: {ex.Message}");
        }

        return null;
    }

    public async Task<List<NecessidadeResponse>?> GetNecessidadesPainel(string emailUsuario)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/necessidades/painel?email={Uri.EscapeDataString(emailUsuario)}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<NecessidadeResponse>>();
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Erro ao buscar painel de necessidades. Status: {response.StatusCode}, Erro: {errorMessage}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine("ℹ️ Endpoint de painel de necessidades indisponível. Tentando fallback para /api/necessidades.");
                return await GetNecessidadesUsuario(emailUsuario);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar painel de necessidades: {ex.Message}");
        }

        return await GetNecessidadesUsuario(emailUsuario);
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

    public async Task<string> PostUsuarioCompleto(string email, string senha, string empresaNome)
    {
        var payload = new { email, senha, empresaNome };
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("/api/usuarios/cadastrar-completo", content);

            if (response.IsSuccessStatusCode)
                return "success";
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return await ReadApiErrorMessageAsync(response, "Ja existe um usuario cadastrado com este e-mail.");
            }
            else
                return await ReadApiErrorMessageAsync(response, "Erro ao registrar usuario e empresa.");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Erro de conectividade ao cadastrar usuario completo: {ex.Message}");
            return "network_error";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro inesperado ao cadastrar usuario completo: {ex.Message}");
            return "unexpected_error";
        }
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

    public async Task<OperacaoResumoResponse?> GetResumoOperacao(string email)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/operacao/resumo?email={Uri.EscapeDataString(email)}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<OperacaoResumoResponse>();
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Erro ao buscar resumo operacional. Status: {response.StatusCode}, Erro: {errorMessage}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar resumo operacional: {ex.Message}");
        }

        var tarefas = await GetTarefasPainel(email) ?? new();
        var necessidades = await GetNecessidadesPainel(email) ?? new();
        return MontarResumoFallback(tarefas, necessidades);
    }

    private static OperacaoResumoResponse MontarResumoFallback(
        IReadOnlyCollection<TarefaResponse> tarefas,
        IReadOnlyCollection<NecessidadeResponse> necessidades)
    {
        var agora = DateTime.Now;
        var tarefasPendentes = tarefas.Count(t => t.Status is "Aberta" or "Em Andamento");
        var tarefasFechadas = tarefas.Count(t => t.Status == "Fechada");

        var resumo = new OperacaoResumoResponse
        {
            PerfilAtual = "Visao operacional",
            TotalNecessidadesAbertas = necessidades.Count(n => n.Status == "Aberta"),
            TotalNecessidadesCriticas = necessidades.Count(n => n.Status == "Aberta" && (n.Prioridade <= 2 || n.Complexidade >= 9)),
            TotalTarefasPendentes = tarefasPendentes,
            TotalTarefasAtrasadas = tarefas.Count(t => t.Status is "Aberta" or "Em Andamento" && t.DataLimite < agora),
            TotalTarefasSemResponsavel = tarefas.Count(t => !t.UsuarioResponsavelId.HasValue && (t.Status is "Aberta" or "Em Andamento")),
            TotalAlertasHoje = necessidades.Count(n => n.UltimoAlertaEm.HasValue && n.UltimoAlertaEm.Value.ToLocalTime().Date == agora.Date),
            PercentualConclusaoTarefas = tarefas.Count == 0 ? 0 : Math.Round((decimal)tarefasFechadas * 100 / tarefas.Count, 1)
        };

        resumo.FilasNecessidades =
        [
            new OperacaoResumoItem { Label = "Abertas", Total = resumo.TotalNecessidadesAbertas, Meta = "Demandas ativas" },
            new OperacaoResumoItem { Label = "Criticas", Total = resumo.TotalNecessidadesCriticas, Meta = "Prioridade imediata" },
            new OperacaoResumoItem { Label = "Com alerta", Total = necessidades.Count(n => n.TotalAlertas > 0 && n.Status == "Aberta"), Meta = "Precisam de resposta" },
            new OperacaoResumoItem { Label = "Finalizadas", Total = necessidades.Count(n => n.Status == "Fechada"), Meta = "Historico concluido" }
        ];

        resumo.CargaResponsavel = tarefas
            .Where(t => t.Status is "Aberta" or "Em Andamento")
            .GroupBy(t => string.IsNullOrWhiteSpace(t.ResponsavelNome) ? "Nao atribuido" : t.ResponsavelNome)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new OperacaoResumoItem
            {
                Label = g.Key,
                Total = g.Count(),
                Meta = g.Select(x => x.ResponsavelCargo).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? "Carteira ativa"
            })
            .ToList();

        return resumo;
    }


}
public class TarefasPorStatusResponse
{
    public string Status { get; set; } = "";
    public int Total { get; set; }
}


public class UsuarioResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
}

public class LoginResult
{
    public bool Success { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
}

public class LoginApiResponse
{
    public string Message { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class ColaboradorRequest
{
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public int TipoUsuario { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Endereco { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;

    public int EmpresaId { get; set; }
    public DateTime DataAdmissao { get; set; }
    public string Genero { get; set; } = string.Empty;
}
public class ColaboradorResponse
{
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;

    public int EmpresaId { get; set; }
    public int TipoUsuario { get; set; }
}

public class Cargo
{
    public int CargoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int EmpresaId { get; set; }
}

public class UsuarioGestaoInfo
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
    public int? ColaboradorId { get; set; }
    public string? ColaboradorNome { get; set; }
    public string? CPF { get; set; }
    public int? TipoUsuario { get; set; }
    public string? Cargo { get; set; }
    public int? EmpresaId { get; set; }
}

public class UsuarioColaboradorUpdateRequest
{
    public string EmailAtual { get; set; } = string.Empty;
    public string NovoEmail { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public int TipoUsuario { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public int EmpresaId { get; set; }
}

public class AcessoColaboradorRequest
{
    public int? ColaboradorId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class NecessidadeAlerta
{
    public int Id { get; set; }
    public int NecessidadeId { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.Now;
}





