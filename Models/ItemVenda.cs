using System.Text.Json.Serialization;

namespace BeneficentEvent.Models;
public class ItemVenda
{
    public Guid Id { get; set; }

    public Guid VendaId { get; set; }

    public Venda Venda { get; set; } = null!;

    public Guid ProdutoId { get; set; }

    public Produto Produto { get; set; } = null!;

    public decimal Quantidade { get; set; }

    public decimal PrecoUnitario { get; set; }
}