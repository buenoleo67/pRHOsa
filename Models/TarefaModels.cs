namespace pRHosaApp1.Models;

public class TarefaRequest
{
    public int Necessidade { get; set; }
    public string Descricao { get; set; }
    public string ResponsavelEmail { get; set; }
    public int? UsuarioResponsavelId { get; set; }
    public DateTime DataLimite { get; set; }

    public DateTime DataCriacao { get; set; }
    public string Status { get; set; }
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
    public string Descricao { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataLimite { get; set; }

    public string Status { get; set; }
    public int? UsuarioResponsavelId { get; set; }
}

public class ColaboradorInfo
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public int EmpresaId { get; set; }


    public string Email { get; set; }
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
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int TipoNecessidade { get; set; }
    public int Prioridade { get; set; }
    public int Complexidade { get; set; }
    public string Status { get; set; } = string.Empty;

    public int? NotaSatisfacaoColaborador { get; set; }   // 👈 Campo para avaliação do Colaborador
    public int? NotaSatisfacaoRH { get; set; }            // 👈 Campo para avaliação do RH

    public List<NecessidadeAlerta> Alertas { get; set; } = new();
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
    public string Nome { get; set; }
    public int EmpresaId { get; set; }
}/*
public class Empresa
{
    public int EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? RepresentanteLegal { get; set; }
    public DateTime? DataFundacao { get; set; }

    public DateTime? DataInicioDemo { get; set; }
}
*/


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

