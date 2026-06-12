using BeneficentEvent.Data;
using BeneficentEvent.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<DoacaoService>();
builder.Services.AddScoped<DespesaService>();
builder.Services.AddScoped<EventoService>();
builder.Services.AddScoped<BenfeitorService>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<VendaService>();
builder.Services.AddScoped<LeilaoService>();
builder.Services.AddScoped<BingoService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

//adicionando o banco de dados (tmb eh adicionada a string de conexao no appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
{
   options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")); 
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//serializa json
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();