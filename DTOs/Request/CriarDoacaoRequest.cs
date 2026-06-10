using System.ComponentModel.DataAnnotations;
using BeneficentEvent.Enums;


namespace BeneficentEvent.DTOs.Request;

public record CriarDoacaoRequest(
    Guid EventoId,
    Guid BenfeitorId,
    TipoDoacao Tipo,
    decimal ValorMonetario,
    DateTime Data,
    string? Observacao,
    List<ItemDoacaoRequest> Itens
);

public record ItemDoacaoRequest(
    string Nome,
    decimal Quantidade,
    string Unidade,
    decimal ValorEstimado
);