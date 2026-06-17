using BeneficentEvent.Data;
using BeneficentEvent.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// DATABASE
// ======================================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);


// ======================================================
// CONTROLLERS + JSON
// ======================================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

builder.Services.AddEndpointsApiExplorer();


// ======================================================
// AUTHENTICATION JWT
// ======================================================
var chave = builder.Configuration["Jwt:Key"] ?? "chavealeatoriode32caracteres@#$%¨¨&*())_)(*&¨%$)";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "",
            ValidAudience = "",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),ClockSkew = TimeSpan.Zero
        };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine(
                $"[JWT] Falha: {context.Exception.Message}"
            );

            return Task.CompletedTask;
        },

        OnTokenValidated = context =>
        {
            Console.WriteLine(
                $"[JWT] Token válido: {context.Principal?.Identity?.Name}"
            );

            return Task.CompletedTask;
        },

        OnChallenge = context =>
        {
            Console.WriteLine(
                $"[JWT] Challenge: {context.Error}"
            );

            return Task.CompletedTask;
        },

        OnMessageReceived = context =>
        {
            Console.WriteLine(
                "[JWT] Token recebido"
            );

            return Task.CompletedTask;
        }

    };

});


// ======================================================
// DEPENDENCY INJECTION - SERVICES
// ======================================================
builder.Services.AddScoped<DoacaoService>();
builder.Services.AddScoped<DespesaService>();
builder.Services.AddScoped<EventoService>();
builder.Services.AddScoped<BenfeitorService>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<VendaService>();
builder.Services.AddScoped<LeilaoService>();
builder.Services.AddScoped<BingoService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<AuthService>();


// ======================================================
// SWAGGER + JWT
// ======================================================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "BeneficentEvent API",
            Version = "v1",
            Description =
                "API com autenticação JWT"
        }
    );

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type = SecuritySchemeType.Http,

            Scheme = "Bearer",

            BearerFormat = "JWT",

            In = ParameterLocation.Header,

            Description =
            "Digite somente o token JWT."
        }
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                    new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },

                Array.Empty<string>()
            }
        }
    );

});


// ======================================================
// BUILD APP
// ======================================================
var app = builder.Build();


// ======================================================
// HTTP PIPELINE
// ======================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "BeneficentEvent API v1"
        );
    });

}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();