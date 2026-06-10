namespace BeneficentEvent.DTOs.Response;

public record ProdutoResponse(
    Guid Id,
    string Nome,
    string Categoria,
    decimal PrecoVenda
);