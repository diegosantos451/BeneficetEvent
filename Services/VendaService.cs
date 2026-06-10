using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.Exceptions;
using BeneficentEvent.Models;
using Microsoft.EntityFrameworkCore;
using BeneficentEvent.Enums;
using BeneficentEvent.DTOs.Response;

namespace BeneficentEvent.Services;

public class VendaService
{
    private readonly AppDbContext _context;
    public VendaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<VendaResponse>> ListarAsync()
    {
        return await _context.Vendas.Select(x => new VendaResponse(
            x.Id,
            x.Evento.Nome,
            x.DataVenda,
            x.ValorTotal,
            x.FormaPagamento
        )).ToListAsync();
    }

    public async Task<VendaDetalheResponse> BuscarPorIdAsync(Guid id)
    {
        var venda = await _context.Vendas.Include(x => x.Itens).ThenInclude(x => x.Produto).Include(x => x.Evento).FirstOrDefaultAsync(x => x.Id == id);

        if (venda is null)
            throw new RegraNegocioException("Venda não cadastrada.");
        
        return new VendaDetalheResponse(
            venda.Id,
            venda.Evento.Nome,
            venda.DataVenda,
            venda.ValorTotal,
            venda.FormaPagamento,

            venda.Itens.Select(x => new ItensVendaResponse(
                x.Id,
                x.ProdutoId,
                x.Produto.Nome,
                x.Quantidade,
                x.PrecoUnitario
            )).ToList()
        );
    }

    public async Task<Guid> CriarAsync(CriarVendaRequest request)
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == request.EventoId);

        if (evento is null)
            throw new RegraNegocioException("Evento não cadastrado para realizar venda.");

        if (evento.Status == StatusEvento.Encerrado)
            throw new RegraNegocioException("Não é possível realizar vendas para um evento encerrado.");

        if (request.DataVenda < DateTime.Today)
            throw new RegraNegocioException("O campo data deve ser maior que a data atual.");

        var venda = new Venda
        {
            Id = Guid.NewGuid(),
            EventoId = request.EventoId,
            DataVenda = request.DataVenda,
            FormaPagamento = request.formaPagamento,
            ValorTotal = 0
        };

        _context.Vendas.Add(venda);

        foreach (var itemVenda in request.ItemVendas)
        {
            if (itemVenda.Quantidade <= 0)
                throw new RegraNegocioException("O campo quantidade deve ser maior que 0.");
            if (itemVenda.PrecoUnitario <= 0)
                throw new RegraNegocioException("O campo preço unitário deve ser maior que 0.");

            var item = new ItemVenda
            {
                Id = Guid.NewGuid(),
                VendaId = venda.Id,
                ProdutoId = itemVenda.ProdutoId,
                Quantidade = itemVenda.Quantidade,
                PrecoUnitario = itemVenda.PrecoUnitario
            };

            venda.ValorTotal += item.Quantidade * item.PrecoUnitario;
            _context.ItensVenda.Add(item);
        }

        var movimento = new MovimentoFinanceiro
        {
            Id = venda.Id,
            EventoId = request.EventoId,
            Tipo = TipoMovimento.Receita,
            Valor = venda.ValorTotal,
            Origem = "Venda de produtos",
            DataMovimento = DateTime.UtcNow
        };

        _context.MovimentosFinanceiros.Add(movimento);
        await _context.SaveChangesAsync();
        return venda.Id;
    }

    public async Task ExcluirAsync(Guid id)
    {
        var venda = await _context.Vendas.Include(x => x.Itens).FirstOrDefaultAsync(x => x.Id == id);

        if (venda is null)
            throw new RegraNegocioException("Venda não cadastrada.");

        var movimento = await _context.MovimentosFinanceiros.FirstOrDefaultAsync(x => x.Id == venda.Id);

        if (movimento is null)
            throw new RegraNegocioException("Erro ao excluir venda.");

        _context.MovimentosFinanceiros.Remove(movimento);
        _context.ItensVenda.RemoveRange(venda.Itens);
        _context.Vendas.Remove(venda);
        await _context.SaveChangesAsync();
    }

    public async Task EditarAsync(Guid id, EditarVendaRequest request)
    {
        var venda = await _context.Vendas.Include(x => x.Itens).FirstOrDefaultAsync(x => x.Id == id);
        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == request.EventoId);

        if (venda is null)
            throw new RegraNegocioException("Venda não encontrada.");

        if (evento is null)
            throw new RegraNegocioException("Evento não cadastrado para realizar venda.");

        if (evento.Status == StatusEvento.Encerrado)
            throw new RegraNegocioException("Não é possível realizar vendas para um evento encerrado.");

        if (request.DataVenda < DateTime.Today)
            throw new RegraNegocioException("O campo data deve ser maior que a data atual.");


        var idsRecebidos = request.ItensVenda.Select(x => x.Id).ToList();
        var removerProdutos = venda.Itens.Where(x => !idsRecebidos.Contains(x.Id)).ToList();

        venda.DataVenda = request.DataVenda;
        venda.FormaPagamento = request.formaPagamento;
        venda.ValorTotal = 0;

        foreach (var item in request.ItensVenda)
        {
            var itemExistente = venda.Itens.FirstOrDefault(x => x.Id == item.Id);

            if (item.Quantidade <= 0)
                throw new RegraNegocioException("O campo quantidade deve ser maior que 0.");

            if (item.PrecoUnitario <= 0)
                throw new RegraNegocioException("O campo preço unitário deve ser maior que 0.");

            if (itemExistente is null)
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

        var movimento = await _context.MovimentosFinanceiros.FirstOrDefaultAsync(x => x.Id == venda.Id);

        venda.ValorTotal = venda.Itens.Sum(x => x.Quantidade * x.PrecoUnitario);

        if (!(movimento is null))
            movimento.Valor = venda.ValorTotal;

        await _context.SaveChangesAsync();

    }
}
