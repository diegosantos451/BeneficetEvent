using BeneficentEvent.Enums;

namespace BeneficentEvent.Models;

public class Evento
{
    public Guid Id  { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataInicio { get; set; } = DateTime.Now;
    public DateTime DataFim { get; set; }
    public string? Local { get; set; }
    public StatusEvento Status { get; set; }
    public ICollection<ParticipacaoEvento> Participantes { get; set; } = new List<ParticipacaoEvento>();
    public ICollection<Doacao> Doacoes { get; set; } = new List<Doacao>();
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    public ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
    public ICollection<Bingo> Bingos { get; set; } = new List<Bingo>();
    public ICollection<Leilao> Leiloes { get; set; } = new List<Leilao>();
    public ICollection<MovimentoFinanceiro> MovimentosFinanceiros { get; set; } = new List<MovimentoFinanceiro>();
}