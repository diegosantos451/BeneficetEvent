using System.Text.Json.Serialization;

namespace BeneficentEvent.Models;
public class ItemDoacao
{
    public Guid Id { get; set; }

    public Guid DoacaoId { get; set; }
    
    [JsonIgnore]
    public Doacao Doacao { get; set; } = null!;

    public string Nome { get; set; } = string.Empty;

    public decimal Quantidade { get; set; }

    public string Unidade { get; set; } = string.Empty;

    public decimal ValorEstimado { get; set; }
}