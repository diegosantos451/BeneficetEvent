namespace BeneficentEvent.Models;

public class Lance
{
    public Guid Id { get; set; }

    public Guid ItemLeilaoId { get; set; }

    public ItemLeilao ItemLeilao { get; set; } = null!;

    public Guid BenfeitorId { get; set; }

    public Benfeitor Benfeitor { get; set; } = null!;

    public decimal Valor { get; set; }

    public DateTime DataHora { get; set; }
}