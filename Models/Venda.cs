using System.Text.Json.Serialization;
using BeneficentEvent.Enums;
namespace BeneficentEvent.Models;

public class Venda
{
    public Guid Id { get; set; }

    public Guid EventoId { get; set; }

    public Evento Evento { get; set; } = null!;

    public DateTime DataVenda { get; set; }

    public decimal ValorTotal { get; set; }

    public FormaPagamento FormaPagamento { get; set; }

    public ICollection<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
}