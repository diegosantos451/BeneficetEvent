namespace BeneficentEvent.Models;
public class ParticipacaoEvento
{
    public Guid EventoId { get; set; }

    public Evento Evento { get; set; } = null!;

    public Guid BenfeitorId { get; set; }

    public Benfeitor Benfeitor { get; set; } = null!;

    public string Funcao { get; set; } = string.Empty;

    public string? Observacao { get; set; }
}