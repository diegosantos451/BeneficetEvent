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
        var eventos = await _context.Eventos.AsSplitQuery().Include(x => x.Participantes).ThenInclude(x => x.Benfeitor).Include(x => x.Doacoes).
            ThenInclude(x => x.Benfeitor).Include(x => x.Doacoes).ThenInclude(x => x.Itens).Include(x => x.Produtos).Include(x => x.Despesas).Include(x => x.Bingos).
                ThenInclude(x => x.Premios).Include(x => x.Leiloes).ThenInclude(x => x.Itens)
                    .ThenInclude(x => x.Lances).ThenInclude(x => x.Benfeitor).Include(x => x.MovimentosFinanceiros).
                        ToListAsync();


        if (eventos is null)
            throw new RegraNegocioException("Nenhum evento cadastrado.");

        var VendasMensais = await _context.Vendas.GroupBy(x => new
        {
            x.DataVenda.Year,
            x.DataVenda.Month
        }).Select(x => ReceitaMensalResponse(
            x.Key.Month.ToString(),
            
        ))  

        int TotalEventos = eventos?.Count() ?? 0;
        int EventosAtivos = eventos?.Select(x => x.Status == StatusEvento.Andamento).Count() ?? 0;
        int EventosEncerrados = eventos?.Select(x => x.Status == StatusEvento.Encerrado).Count() ?? 0;
        int ProximosEventos = eventos?.Select(x => x.Status == StatusEvento.Planejamento).Count() ?? 0;
        decimal MediaArrecadacaoEvento = 0;
        int TotalBenfeitores = eventos?.Select(x => x.Participantes).Count() ?? 0;
        decimal TotalArrecadado = 0;
        decimal TotalDespesas = eventos?.SelectMany(x => x.Despesas).Sum(x => x.Valor) ?? 0m;
        decimal LucroLiquido = 0;
        int TotalDoacao = eventos?.Select(x => x.Doacoes).Count() ?? 0;
        int TotalVendas = _context.Vendas?.Count() ?? 0;
        int TotalBingo = _context.Bingos?.Count() ?? 0;
        decimal ReceitaLeiloes = eventos?.Where(x => x.Status == StatusEvento.Encerrado).SelectMany(x => x.Leiloes).SelectMany(x => x.Itens).Sum(x => x.LanceAtual) ?? 0m;
        decimal ReceitaDoacoes = eventos?.Where(x => x.Status == StatusEvento.Encerrado).SelectMany(x => x.Doacoes).Sum(x => x.ValorMonetario) ?? 0m;
        decimal ReceitaBingos = _context.MovimentosFinanceiros?.Where(x => x.Origem == "Venda de cartelas de bingo.").Sum(x => x.Valor) ?? 0m;
        decimal ReceitaVendas = _context.Vendas?.Sum(x => x.ValorTotal) ?? 0m;

        var TopEventos = await _context.Eventos.Select( e => new RankingEventoResponse(
            e.Nome,
            (e.Doacoes.Sum(d => (decimal?)d.ValorMonetario) ?? 0m) + (e.Leiloes.SelectMany(l => l.Itens).Sum(l => (decimal?)l.LanceAtual) ?? 0m) +
                 (_context.Vendas.Where(v => v.EventoId == e.Id).Sum(vt => (decimal?)vt.ValorTotal) ?? 0m) + (_context.MovimentosFinanceiros.
                    Where(m => m.EventoId == e.Id && m.Tipo == TipoMovimento.Receita).Sum(x =>(decimal?) x.Valor) ?? 0m)
                        )).OrderByDescending(e => e.Arrecadado).Take(5).ToListAsync();

        var TopBenfeitores = await _context.Benfeitores.Select(b => new RankingBenfeitorResponse(
            b.Nome,
            b.Doacoes.Sum(d => (decimal?)d.ValorMonetario) ?? 0m)).OrderByDescending(b => b.TotalDoado).Take(5).ToListAsync();

        var ReceitaMensal = await _context.Vendas.GroupBy()       

        return Ok(new DashboardResponse(

        ));
    }

}