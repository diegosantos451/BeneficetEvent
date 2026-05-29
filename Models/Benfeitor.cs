namespace BeneficentEvent.Models;

public class Benfeitor
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string CPF { get; set; } = string.Empty;

    public string? Telefone { get; set; }

    public string? Email { get; set; }

    public string? Endereco { get; set; }

    public ICollection<ParticipacaoEvento> Eventos { get; set; } = new List<ParticipacaoEvento>();

    public ICollection<Doacao> Doacoes { get; set; } = new List<Doacao>();

    public ICollection<Lance> Lances { get; set; } = new List<Lance>();
}