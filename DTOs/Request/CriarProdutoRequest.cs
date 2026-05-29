namespace BeneficentEvent.DTOs.Request;

public record CriarProdutoRequest(
    Guid Id,
    Guid EventoId,
    string Nome,
    string Categoria,
    decimal PrecoVenda,
    decimal QuantidadeEstoque
);
