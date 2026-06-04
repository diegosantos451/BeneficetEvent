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

    public async Task<Guid>CriarAsync(CriarVendaRequest request)
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == request.EventoId);
        if(evento is null)
            throw new RegraNegocioException("Evento não cadastrado para realizar venda.");
        
        var venda = new Venda
        {
            Id = Guid.NewGuid(),
            EventoId = request.EventoId,
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
        return venda.Id;
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

    public async Task EditarAsync(Guid id, EditarVendaRequest request)
    {
        var venda = await _context.Vendas.Include(x => x.Itens).FirstOrDefaultAsync(x => x.Id == id);
        if(venda is null)
            throw new RegraNegocioException("Venda não encontrada.");
        var idsRecebidos = request.ItensVenda.Select(x => x.Id).ToList();
        var removerProdutos = venda.Itens.Where(x => !idsRecebidos.Contains(x.Id)).ToList();        
        venda.DataVenda = request.DataVenda;
        venda.FormaPagamento = request.formaPagamento;
        venda.ValorTotal = 0;

        foreach(var item in request.ItensVenda)
        {
            var itemExistente  = venda.Itens.FirstOrDefault(x => x.Id == item.Id);
            if(itemExistente is null)
            {
                var novoProduto = new ItemVenda
                {
                    Id = Guid.NewGuid(),
                    ProdutoId = item.ProdutoId,
                    VendaId = venda.Id,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario
                };
                venda.Itens.Add(novoProduto);
            }
            else
            {
                itemExistente.Quantidade = item.Quantidade;
                itemExistente.PrecoUnitario = item.PrecoUnitario;
            }   
        }
        _context.RemoveRange(removerProdutos);
        venda.ValorTotal = venda.Itens.Sum(x => x.Quantidade * x.PrecoUnitario);
        await _context.SaveChangesAsync();
        
    }
}
