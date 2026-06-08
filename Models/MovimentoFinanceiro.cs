using System.Text.Json.Serialization;
using BeneficentEvent.Enums;

namespace BeneficentEvent.Models;

public class MovimentoFinanceiro
{
    public Guid Id { get; set; }

    [JsonIgnore]
    public Guid EventoId { get; set; }

    [JsonIgnore]
    public Evento Evento { get; set; } = null!;

    public TipoMovimento Tipo { get; set; }

    public decimal Valor { get; set; }

    public string Origem { get; set; } = string.Empty;

    public DateTime DataMovimento { get; set; }
}