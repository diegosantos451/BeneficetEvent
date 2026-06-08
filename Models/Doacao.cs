using System.Text.Json.Serialization;
using BeneficentEvent.Enums;

namespace BeneficentEvent.Models;
public class Doacao
{
    public Guid Id { get; set; }

    public Guid EventoId { get; set; }

   
    public Evento Evento { get; set; } = null!;

    public Guid BenfeitorId { get; set; }

    public Benfeitor Benfeitor { get; set; } = null!;

    public TipoDoacao Tipo { get; set; }

    public decimal ValorMonetario { get; set; }

    public DateTime DataDoacao { get; set; }

    public string? Observacao { get; set; }

    public ICollection<ItemDoacao> Itens { get; set; } = new List<ItemDoacao>();
}