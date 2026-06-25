namespace pRHosaApp1.Models;

public class TarefaRequest
{
    public int Necessidade { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string ResponsavelEmail { get; set; } = string.Empty;
    public int? UsuarioResponsavelId { get; set; }
    public DateTime DataLimite { get; set; }

    public DateTime DataCriacao { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<Comentario> HistoricoComentarios { get; set; } = new();
}

public class Comentario
{
    public string Autor { get; set; } = "";
    public string AutorNome { get; set; } = "";
    public string Texto { get; set; } = "";
    public DateTime Data { get; set; }
}


public class TarefaResponse
{
    public int TarefaId { get; set; }
    public int Necessidade { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime DataLimite { get; set; }

    public string Status { get; set; } = string.Empty;
    public int? UsuarioResponsavelId { get; set; }
    public string ResponsavelNome { get; set; } = string.Empty;
    public string ResponsavelCargo { get; set; } = string.Empty;
    public int? NecessidadeVinculadaId { get; set; }
    public string NecessidadeTitulo { get; set; } = string.Empty;
    public int? NecessidadePrioridade { get; set; }
    public int? NecessidadeComplexidade { get; set; }
    public string TipoNecessidadeNome { get; set; } = string.Empty;
    public bool PossuiNecessidadeVinculada { get; set; }
}

public class ColaboradorInfo
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public int EmpresaId { get; set; }
    public string Email { get; set; } = string.Empty;
    public int TipoUsuario { get; set; }
    public string Cargo { get; set; } = string.Empty;
}

public class NecessidadeRequest
{
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int TipoNecessidade { get; set; }
    public int Prioridade { get; set; }
    public int Complexidade { get; set; }
    public int EmpresaId { get; set; }

}
public class NecessidadeResponse : NecessidadeRequest
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }

    public int? NotaSatisfacaoColaborador { get; set; }
    public int? NotaSatisfacaoRH { get; set; }

    public List<NecessidadeAlerta> Alertas { get; set; } = new();
    public string TipoNome { get; set; } = string.Empty;
    public int TotalAlertas { get; set; }
    public DateTime? UltimoAlertaEm { get; set; }
    public int TotalTarefasAbertas { get; set; }
    public DateTime? ProximoPrazo { get; set; }
    public int? ResponsavelAtualId { get; set; }
    public string ResponsavelAtualNome { get; set; } = string.Empty;
    public string ResponsavelAtualCargo { get; set; } = string.Empty;
}

/*
public class NecessidadeResponse : NecessidadeRequest
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int TipoNecessidade { get; set; }
    public int Prioridade { get; set; }
    public int Complexidade { get; set; }
    public string Status { get; set; } = string.Empty;

    public List<NecessidadeAlerta> Alertas { get; set; } = new(); // 👈 Adiciona isso

}*/
/*
public class NecessidadeAlerta
{
    public int Id { get; set; }
    public int NecessidadeId { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
}*/

public class TipoNecessidade
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int EmpresaId { get; set; }
}


public class TarefasPorStatusResponse
{
    public string Status { get; set; } = "";
    public int Total { get; set; }
}
public class NecessidadesPorTipoResponse
{
    public string Tipo { get; set; } = "";
    public int Total { get; set; }
}
public class TotaisDashboardResponse
{
    public int TotalTarefas { get; set; }
    public int TotalNecessidades { get; set; }
}

public class OperacaoResumoResponse
{
    public string PerfilAtual { get; set; } = string.Empty;
    public int TotalNecessidadesAbertas { get; set; }
    public int TotalNecessidadesCriticas { get; set; }
    public int TotalTarefasPendentes { get; set; }
    public int TotalTarefasAtrasadas { get; set; }
    public int TotalTarefasSemResponsavel { get; set; }
    public int TotalAlertasHoje { get; set; }
    public decimal PercentualConclusaoTarefas { get; set; }
    public List<OperacaoResumoItem> FilasNecessidades { get; set; } = new();
    public List<OperacaoResumoItem> CargaResponsavel { get; set; } = new();
}

public class OperacaoResumoItem
{
    public string Label { get; set; } = string.Empty;
    public int Total { get; set; }
    public string Meta { get; set; } = string.Empty;
}

