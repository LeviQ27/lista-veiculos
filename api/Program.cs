using lista_veiculos.Dominio.DTOs;
using lista_veiculos.Dominio.Entidades;
using lista_veiculos.Dominio.interfaces;
using lista_veiculos.Dominio.ModelViews;
using lista_veiculos.Infraestrutura.Db;
using lista_veiculos.Infraestrutura.Servicos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDto, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.Login(loginDto);
    if (administrador != null)
    {
        return Results.Ok("Login successful");
    }
    else
    {
        return Results.Unauthorized();
    }
}).WithTags("Administradores");
#endregion

#region Veículos
ErrosDeValidacao validaDTO(VeiculoDTO veiculoDto)
{
    var validacao = new ErrosDeValidacao();
    if (string.IsNullOrEmpty(veiculoDto.Nome))
    {
        validacao.Mensagens.Add("O nome do veículo é obrigatório.");
    }
    if (string.IsNullOrEmpty(veiculoDto.Marca))
    {
        validacao.Mensagens.Add("A marca do veículo é obrigatória.");
    }
    if (veiculoDto.Ano <= 1950)
    {
        validacao.Mensagens.Add("Ano do veículo deve ser maior que 1950.");
    }
    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDto, IVeiculoServico veiculoServico) =>
{

    var validacao = validaDTO(veiculoDto);
    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var veiculo = new Veiculo
    {
        Nome = veiculoDto.Nome,
        Marca = veiculoDto.Marca,
        Ano = veiculoDto.Ano
    };
    veiculoServico.Adicionar(veiculo);
    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);

}).WithTags("Veículos");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    try
    {
        var veiculos = veiculoServico.Todos(pagina);
        return Results.Ok(veiculos);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erro ao buscar veículos: {ex.Message}");
    }
}).WithTags("Veículos");

app.MapGet("/veiculos/{id:int}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    try
    {
        var veiculo = veiculoServico.BuscaPorId(id);
        return Results.Ok(veiculo);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound("Veículo não encontrado.");
    }
}).WithTags("Veículos");

app.MapPut("/veiculos/{id:int}", ([FromRoute] int id, [FromBody] VeiculoDTO veiculoDto, IVeiculoServico veiculoServico) =>
{
    try
    {
        var veiculo = veiculoServico.BuscaPorId(id);
        var validacao = validaDTO(veiculoDto);
        if (validacao.Mensagens.Count > 0)
        {
            return Results.BadRequest(validacao);
        }
        veiculo.Nome = veiculoDto.Nome;
        veiculo.Marca = veiculoDto.Marca;
        veiculo.Ano = veiculoDto.Ano;
        veiculoServico.Atualizar(veiculo);
        return Results.Ok(veiculo);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound("Veículo não encontrado.");
    }
}).WithTags("Veículos");

app.MapDelete("/veiculos/{id:int}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    try
    {
        var veiculo = veiculoServico.BuscaPorId(id);
        veiculoServico.Apagar(veiculo);
        return Results.NoContent();
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound("Veículo não encontrado.");
    }
}).WithTags("Veículos");
#endregion

#region App
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
#endregion