using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddScoped<DoacaoService>();
builder.Services.AddScoped<DespesaService>();
builder.Services.AddScoped<EventoService>();
builder.Services.AddScoped<BenfeitorService>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<VendaService>();
builder.Services.AddScoped<LeilaoService>();
builder.Services.AddScoped<BingoService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters
        .Add(new JsonStringEnumConverter());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


//adicionando o banco de dados (tmb eh adicionada a string de conexao no appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
{
   options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")); 
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Aplica a política CORS globalmente
app.UseCors("AllowAll");

app.UseAuthorization();


app.UseHttpsRedirection();
app.MapControllers();
app.Run();