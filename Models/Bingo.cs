namespace BeneficentEvent.Models;

public class Bingo
{
    public Guid Id { get; set; }

    public Guid EventoId { get; set; }

    public Evento Evento { get; set; } = null!;

    public string Nome { get; set; } = string.Empty;

    public decimal ValorCartela { get; set; }

    public DateTime DataSorteio { get; set; }

    public ICollection<PremioBingo> Premios { get; set; } = new List<PremioBingo>();
}