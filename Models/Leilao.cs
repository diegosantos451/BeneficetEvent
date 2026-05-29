namespace BeneficentEvent.Models;

public class Leilao
{
    public Guid Id { get; set; }

    public Guid EventoId { get; set; }

    public Evento Evento { get; set; } = null!;

    public string Nome { get; set; } = string.Empty;

    public DateTime Data { get; set; }

    public ICollection<ItemLeilao> Itens { get; set; } = new List<ItemLeilao>();
}