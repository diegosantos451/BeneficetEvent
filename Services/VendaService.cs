using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.Exceptions;
using BeneficentEvent.Models;
using Microsoft.EntityFrameworkCore;

namespace BeneficentEvent.Services;

public class VendaService
{
    private readonly AppDbContext _context;
    public VendaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Venda>> ListarAsync()
    {
        return await _context.Vendas.Include(x => x.Itens).ToListAsync();
    }

    public async Task<Venda?> BuscarPorIdAsync(Guid id)
    {
        return await _context.Vendas.Include(x => x.Itens).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task CriarAsync(Guid idEvento, CriarVendaRequest request)
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == idEvento);
        if(evento is null)
            throw new RegraNegocioException("Evento não cadastrado para realizar venda.");
        
        var venda = new Venda
        {
            Id = Guid.NewGuid(),
            EventoId = idEvento,
            DataVenda = request.DataVenda,
            FormaPagamento = request.formaPagamento,
            ValorTotal = 0            
        };
        _context.Vendas.Add(venda);
        foreach(var ItemVenda in request.ItemVendas)
        {
            var item = new ItemVenda
            {
                Id = Guid.NewGuid(),
                VendaId = venda.Id,
                ProdutoId = ItemVenda.ProdutoId,
                Quantidade = ItemVenda.Quantidade,
                PrecoUnitario = ItemVenda.PrecoUnitario
            };
            venda.ValorTotal += item.Quantidade*item.PrecoUnitario;
            _context.ItensVenda.Add(item);
        }
        await _context.SaveChangesAsync();
    }

    public async Task ExcluirAsync(Guid id)
    {
        var venda = await _context.Vendas.Include(x => x.Itens).FirstOrDefaultAsync(x => x.Id == id);
        if(venda is null)
            throw new RegraNegocioException("Venda não cadastrada.");
        _context.ItensVenda.RemoveRange(venda.Itens);
        _context.Vendas.Remove(venda);
        await _context.SaveChangesAsync();
    }
}
