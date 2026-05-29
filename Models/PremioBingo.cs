namespace BeneficentEvent.Models;

public class PremioBingo
{
    public Guid Id { get; set; }

    public Guid BingoId { get; set; }

    public Bingo Bingo { get; set; } = null!;

    public string Descricao { get; set; } = string.Empty;

    public decimal ValorEstimado { get; set; }
}