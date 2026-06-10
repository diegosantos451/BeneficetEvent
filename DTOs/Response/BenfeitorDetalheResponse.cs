namespace BeneficentEvent.DTOs.Response;

public record BenfeitorDetalheResponse(
    Guid Id,
    string Nome,
    string Cpf,
    string? Telefone,
    string? Email,
    string? Endereco,
    List<BenfeitorEventosResponse> Eventos,
    List<BenfeitorDoacoesResponse> Doacoes,
    List<BenfeitorLancesResponse> Lances
);

public record BenfeitorEventosResponse(
    Guid EventoId,
    string EventoNome,
    string Funcao,
    string? Observacao
);
 public record BenfeitorDoacoesResponse(
    Guid DoacaoId,
    DateTime Data,
    decimal ValorMonetario,
    List<BenfeitorItensDoacaoResponse> Itens
 );

 public record BenfeitorItensDoacaoResponse(
    string Nome,
    decimal Quantidade,
    string Unidade,
    decimal ValorEstimado
 );

 public record BenfeitorLancesResponse(
    string NomeItem,
    DateTime DataLeilao,
    decimal Valor
 );