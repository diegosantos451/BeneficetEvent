using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.Enums;
using BeneficentEvent.Exceptions;
using BeneficentEvent.Models;
using Microsoft.EntityFrameworkCore;

namespace BeneficentEvent.Services;

public class DespesaService
{
    private readonly AppDbContext _context;

    public DespesaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Despesa>> ListarAsync()
    {
        return await _context.Despesas.ToListAsync();
    }

    public async Task<Despesa?> ObterPorIdAsync(Guid id)
    {
        return await  _context.Despesas.FirstOrDefaultAsync(x => x.Id == id);

    }
    public async Task<Guid> CriarAsync(CriarDespesaRequest request)
    {
        var despesa = new Despesa
        {
            Id = Guid.NewGuid(),
            EventoId = request.EventoId,
            Categoria = request.Categoria,
            Data = request.Data,
            Descricao = request.Descricao,
            Fornecedor = request.Fornecedor,
            Valor = request.Valor
        };

        var movimento = new MovimentoFinanceiro
        {
          Id = despesa.Id,
          EventoId = request.EventoId,
          Tipo = TipoMovimento.Despesa,
          Valor = despesa.Valor,
          Origem = despesa.Descricao,
          DataMovimento = despesa.Data
        };
        
        _context.MovimentosFinanceiros.Add(movimento);
        _context.Despesas.Add(despesa);
        await _context.SaveChangesAsync();
        return despesa.Id;
    }

    public async Task EditarAsync(Guid id, CriarDespesaRequest request)
    {
        var despesa = await _context.Despesas.FirstOrDefaultAsync(x => x.Id == id);
        if(despesa is null)
            throw new RegraNegocioException("Despesa não cadastrada.");

        var movimento = await _context.MovimentosFinanceiros.FirstOrDefaultAsync(x =>x.Id == despesa.Id);
        if(movimento is null)
            throw new RegraNegocioException("erro ao editar movimento financeiro.");
            
        
        despesa.Categoria = request.Categoria;
        despesa.Data = request.Data;
        despesa.Descricao = request.Descricao;
        despesa.Fornecedor = request.Fornecedor;
        despesa.Valor = request.Valor;
        
        //editando movimento financeiro de acordo com o valor alterado na despesa 
        movimento.Valor = despesa.Valor;

        await _context.SaveChangesAsync();
    }

    public async Task ExcluirAsync(Guid id)
    {
        var despesa = await _context.Despesas.FirstOrDefaultAsync(x => x.Id == id);
        if(despesa is null)
            throw new RegraNegocioException("Despesa não cadastrada.");
        _context.Despesas.Remove(despesa);
        await _context.SaveChangesAsync();
    }
}