using BeneficentEvent.Enums;

namespace BeneficentEvent.DTOs.Response;

public record EventoDetalheResponse(
    Guid Id,
    string Nome,
    string? Descricao,
    DateTime DataInicio,
    DateTime DataFim,
    string? Local,
    StatusEvento Status,

    //dados calculáveis
    int QuantidadeParticipante,
    int QuantidadeDoacaoMaterial,
    int TotalDoacoes,
    int TotalProdutos,
    int TotalLeiloes,
    int TotalBingos,
    int TotalPremiosBingos,
    decimal TotalDespesas,
    decimal TotalReceita,
    decimal LucroLivreAproximado,
    decimal TotalDoacaoDinheiro,
    decimal TotalArremateLeilao,
    
    List<BenfeitoresEventoResponse> Participantes,
    List<DoacoesEventoResponse> Doacoes,
    List<ProdutosEventoResponse> Produtos,
    List<DespesasEventoResponse> Despesas,
    List<BingosEventosResponse> Bingos,
    List<LeiloesEventosResponse> Leiloes,
    List<MovimentosEventoResponse> Movimentos
);

public record BenfeitoresEventoResponse(
    string NomeBenfeitor,
    string Funcao,
    string? Observacao
);

public record DoacoesEventoResponse(
    string NomeBenfeitor,
    TipoDoacao Tipo,
    decimal ValorMonetario,
    DateTime Data,
    string? Observacao,
    List<ItensDoacoesEventoResponse> Itens
);

public record ItensDoacoesEventoResponse(
    string Nome,
    decimal Quantidade,
    string Unidade,
    decimal ValorEstimado
);

public record ProdutosEventoResponse(
    string Nome,
    string Categoria,
    decimal PrecoVenda,
    decimal Quantidade
);

public record DespesasEventoResponse(
    string Descricao,
    string Categoria,
    string? Fornecedor,
    decimal Valor,
    DateTime Data
);

public record BingosEventosResponse(
    string Nome,
    decimal ValorCartela,
    DateTime DataSorteio,
    List<PremiosBingosEventosResponse> Premios
);

public record PremiosBingosEventosResponse(
    string Descricao,
    decimal ValorEstimado
);

public record LeiloesEventosResponse(
    string NomeLeilao,
    DateTime Data,
    List<ItensLeiloesEventosResponse> ItensLeilao
);

public record ItensLeiloesEventosResponse(
    string Nome,
    string Lote,
    string? Descricao,
    decimal LanceInicial,
    decimal LanceAtual,
    List<LancesItensLeiloesEventoResponse> Lances
);

public record LancesItensLeiloesEventoResponse(
    string NomeBenfeitor,
    decimal Valor,
    DateTime Data
);

public record MovimentosEventoResponse(
    TipoMovimento TipoMovimento,
    decimal Valor,
    string Origem,
    DateTime Data
);