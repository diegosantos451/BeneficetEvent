namespace BeneficentEvent.DTOs.Response;

public record DashboardResponse(
    int TotalEventos,
    int EventosAtivos,
    int EventosEncerrados,
    int ProximosEventos,
    decimal MediaArrecadacaoEvento,

    int TotalBenfeitores,

    decimal TotalArrecadado,
    decimal TotalDespesas,
    decimal LucroLiquido,

    int TotalDoacoes,
    int TotalVendas,
    int TotalBingos,

    decimal ReceitaLeiloes,
    decimal ReceitaDoacoes,
    decimal ReceitaBingos,
    decimal ReceitaVendas,
    
    List<RankingEventoResponse> TopEventos,
    List<RankingBenfeitorResponse> TopBenfeitores,
    List<ReceitaMensalResponse> ReceitaMensal
);

public record RankingEventoResponse(
    string NomeEvento,
    decimal Arrecadado
);

public record RankingBenfeitorResponse(
    string NomeBenfeitor,
    decimal TotalDoado
);

public record ReceitaMensalResponse(
    string Mes,
    decimal Valor
);