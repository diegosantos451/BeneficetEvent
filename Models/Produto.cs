using System.Text.Json.Serialization;

namespace BeneficentEvent.Models;

public class Produto
{
    public Guid Id { get; set; }

    public Guid EventoId { get; set; }

    [JsonIgnore]
    public Evento Evento { get; set; } = null!;

    public string Nome { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public decimal PrecoVenda { get; set; }

    public decimal QuantidadeEstoque { get; set; }

    public ICollection<ItemVenda> ItensVenda { get; set; } = new List<ItemVenda>();
}