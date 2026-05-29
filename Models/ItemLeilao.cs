namespace BeneficentEvent.Models;

public class ItemLeilao
{
    public Guid Id { get; set; }

    public Guid LeilaoId { get; set; }

    public Leilao Leilao { get; set; } = null!;

    public string Nome { get; set; } = string.Empty;

    public string Lote { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public decimal LanceInicial { get; set; }

    public decimal LanceAtual { get; set; }

    public ICollection<Lance> Lances { get; set; } = new List<Lance>();
}