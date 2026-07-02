using InsurancePolicyManager.Application.Interfaces;
using InsurancePolicyManager.Application.Services;
using InsurancePolicyManager.Domain.Interfaces;
using InsurancePolicyManager.Infrastructure.Repositories;
using InsurancePolicyManager.Infrastructure.Persistence;
using InsurancePolicyManager.Infrastructure.Services;
using InsurancePolicyManager.Api.Middlewares;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using InsurancePolicyManager.Application.Validators;
using InsurancePolicyManager.Application.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Insurance Policy Manager API",
        Version = "v1",
        Description = "API para gestão de apólices de seguro automóvel."
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
    
builder.Services.AddScoped<IPolicyNumberGenerator, PolicyNumberGenerator>();
builder.Services.AddScoped<IApoliceRepository, ApoliceRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IApoliceService, ApoliceService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();
builder.Services.AddValidatorsFromAssemblyContaining<CriarApoliceValidator>();
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    SeedData.Seed(db);
}

// HTTPS redirection desabilitado propositalmente: a aplicação roda via Docker em HTTP,
// sem necessidade de certificado SSL para o escopo deste projeto.
// app.UseHttpsRedirection();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
