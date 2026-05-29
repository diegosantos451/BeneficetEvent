namespace BeneficentEvent.Models;

public class Despesa
{
    public Guid Id { get; set; }

    public Guid EventoId { get; set; }

    public Evento Evento { get; set; } = null!;

    public string Descricao { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public string? Fornecedor { get; set; }

    public decimal Valor { get; set; }

    public DateTime Data { get; set; }
}