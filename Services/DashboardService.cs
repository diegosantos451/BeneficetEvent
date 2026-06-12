using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Response;
using BeneficentEvent.Exceptions;
using BeneficentEvent.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using BeneficentEvent.Models;
namespace BeneficentEvent.Services;

public class DashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardResponse> ObterDashboardAsync()
    {
        var eventos = await _context.Eventos.ToListAsync();

        if (!eventos.Any())
            throw new RegraNegocioException("Nenhum evento cadastrado.");

        int TotalEventos = eventos.Count();
        int EventosAtivos = eventos?.Count(x => x.Status == StatusEvento.Andamento) ?? 0;
        int EventosEncerrados = eventos?.Count(x => x.Status == StatusEvento.Encerrado) ?? 0;
        int ProximosEventos = eventos?.Count(x => x.Status == StatusEvento.Planejamento) ?? 0;
        decimal MediaArrecadacaoEvento = 0;
        int TotalBenfeitores = eventos?.SelectMany(x => x.Participantes).Select(x => x.BenfeitorId).Distinct().Count() ?? 0;
        decimal TotalArrecadado = 0;
        decimal TotalDespesas = _context.MovimentosFinanceiros?.Where(x => x.Tipo == TipoMovimento.Despesa).Sum(x => x.Valor) ?? 0m; ;
        decimal LucroLiquido = 0;
        int TotalDoacao = _context.Doacoes?.Count() ?? 0;
        int TotalVendas = _context.Vendas?.Count() ?? 0;
        int TotalBingo = _context.Bingos?.Count() ?? 0;
        decimal ReceitaLeiloes = _context.MovimentosFinanceiros?.Where(x => x.Origem == OrigemMovimento.Leilao).Sum(x => x.Valor) ?? 0m;
        decimal ReceitaDoacoes = _context.MovimentosFinanceiros?.Where(x => x.Origem == OrigemMovimento.Doacao).Sum(x => x.Valor) ?? 0m;
        decimal ReceitaBingos = _context.MovimentosFinanceiros?.Where(x => x.Origem == OrigemMovimento.Bingo).Sum(x => x.Valor) ?? 0m;
        decimal ReceitaVendas = _context.MovimentosFinanceiros?.Where(x => x.Origem == OrigemMovimento.Venda).Sum(x => x.Valor) ?? 0m;

        TotalArrecadado = _context.MovimentosFinanceiros?.Where(x => x.Tipo == TipoMovimento.Receita).Sum(x => x.Valor) ?? 0m;

        if (EventosAtivos + EventosEncerrados > 0)
             MediaArrecadacaoEvento = TotalArrecadado / (EventosAtivos + EventosEncerrados);
        else
            MediaArrecadacaoEvento = 0;

        LucroLiquido = TotalArrecadado - TotalDespesas;

        var TopEventos = await _context.Eventos.Select(e => new RankingEventoResponse(
            e.Nome,
            _context.MovimentosFinanceiros.Where(x => x.EventoId == e.Id && x.Tipo == TipoMovimento.Receita).Sum(x => (decimal?)x.Valor) ?? 0m
        )).OrderByDescending(e => e.Arrecadado).Take(5).ToListAsync();

        var TopBenfeitores = await _context.Benfeitores.Select(b => new RankingBenfeitorResponse(
            b.Nome,
            b.Doacoes.Sum(d => (decimal?)d.ValorMonetario) ?? 0m)).OrderByDescending(b => b.TotalDoado).Take(5).ToListAsync();

        var ReceitaMensal = await _context.MovimentosFinanceiros.Where(x => x.Tipo == TipoMovimento.Receita).GroupBy(x => new
        {
            x.DataMovimento.Year,
            x.DataMovimento.Month
        })
        .Select(x => new ReceitaMensalResponse(
            x.Key.Month.ToString(),
            x.Sum(x => x.Valor)
        )).ToListAsync();

        return new DashboardResponse(
            TotalEventos,
            EventosAtivos,
            EventosEncerrados,
            ProximosEventos,
            MediaArrecadacaoEvento,
            TotalBenfeitores,
            TotalArrecadado,
            TotalDespesas,
            LucroLiquido,
            TotalDoacao,
            TotalVendas,
            TotalBingo,
            ReceitaLeiloes,
            ReceitaDoacoes,
            ReceitaBingos,
            ReceitaVendas,
            TopEventos,
            TopBenfeitores,
            ReceitaMensal
        );
    }
}