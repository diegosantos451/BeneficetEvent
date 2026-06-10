namespace BeneficentEvent.DTOs.Response;

public record ProdutoDetalheResponse(
    Guid Id,
    string Nome,
    string NomeEvento,
    string Categoria,
    decimal PrecoVenda,
    decimal QuantidadeEstoque
);