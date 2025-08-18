using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using lista_veiculos;
using lista_veiculos.Dominio.DTOs;
using lista_veiculos.Dominio.Entidades;
using lista_veiculos.Dominio.Enums;
using lista_veiculos.Dominio.interfaces;
using lista_veiculos.Dominio.ModelViews;
using lista_veiculos.Infraestrutura.Db;
using lista_veiculos.Infraestrutura.Servicos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration.GetSection("Jwt").ToString() ?? "123456789101112131415161718192021";
    }

    private string key;

    public IConfiguration Configuration { get; set; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdministradorServico, AdministradorServico>();
        services.AddScoped<IVeiculoServico, VeiculoServico>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() { Title = "Lista de Veículos", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
            });
        });

        services.AddDbContext<DbContexto>(options =>
        {
            options.UseMySql(
                Configuration.GetConnectionString("MySql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))
            );
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                #region Home
                endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
                #endregion

                #region Administradores
                string GerarTokenJwt(Administrador administrador)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var claims = new List<Claim>
                    {
                        new Claim("Email", administrador.Email),
                        new Claim("Perfil", administrador.Perfil.ToString()),
                        new Claim(ClaimTypes.Role, administrador.Perfil)
                    };
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    return tokenHandler.WriteToken(token);
                }

                endpoints.MapPost("/administradores/login", ([FromBody] LoginDTO loginDto, IAdministradorServico administradorServico) =>
                {
                    var administrador = administradorServico.Login(loginDto);
                    if (administrador != null)
                    {
                        var token = GerarTokenJwt(administrador);
                        return Results.Ok(new AdministradorLogado
                        {
                            Email = administrador.Email,
                            Perfil = administrador.Perfil.ToString(),
                            Token = token
                        });
                    }
                    else
                    {
                        return Results.Unauthorized();
                    }
                }).AllowAnonymous().WithTags("Administradores");

                endpoints.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDto, IAdministradorServico administradorServico) =>
                {
                    var validacao = new ErrosDeValidacao();
                    if (string.IsNullOrEmpty(administradorDto.Email))
                    {
                        validacao.Mensagens.Add("O email é obrigatório.");
                    }
                    if (string.IsNullOrEmpty(administradorDto.Senha))
                    {
                        validacao.Mensagens.Add("A senha é obrigatória.");
                    }
                    if (administradorDto.Perfil == null)
                    {
                        validacao.Mensagens.Add("O perfil é obrigatório.");
                    }

                    if (validacao.Mensagens.Count > 0)
                    {
                        return Results.BadRequest(validacao);
                    }

                    var administrador = new Administrador
                    {
                        Email = administradorDto.Email,
                        Senha = administradorDto.Senha,
                        Perfil = administradorDto.Perfil?.ToString() ?? Perfil.Editor.ToString()
                    };

                    administradorServico.Adicionar(administrador);
                    return Results.Created($"/administradores/{administrador.Id}", administrador);

                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Administradores");

                endpoints.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
                {
                    try
                    {
                        var adms = new List<AdministradorModelView>();
                        var administradores = administradorServico.Todos(pagina);
                        foreach (var adm in administradores)
                        {
                            adms.Add(new AdministradorModelView
                            {
                                Id = adm.Id,
                                Email = adm.Email,
                                Perfil = adm.Perfil.ToString()
                            });
                        }
                        return Results.Ok(adms);
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem($"Erro ao buscar administradores: {ex.Message}");
                    }
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Administradores");

                endpoints.MapGet("/administradores/{id:int}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
                {
                    try
                    {
                        var administrador = administradorServico.BuscaPorId(id);
                        var adm = new AdministradorModelView
                        {
                            Id = administrador!.Id,
                            Email = administrador.Email,
                            Perfil = administrador.Perfil.ToString()
                        };
                        return Results.Ok(adm);
                    }
                    catch (KeyNotFoundException)
                    {
                        return Results.NotFound("Administrador não encontrado.");
                    }
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Administradores");
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

                endpoints.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDto, IVeiculoServico veiculoServico) =>
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

                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("Veículos");

                endpoints.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
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
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("Veículos");

                endpoints.MapGet("/veiculos/{id:int}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
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
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("Veículos");

                endpoints.MapPut("/veiculos/{id:int}", ([FromRoute] int id, [FromBody] VeiculoDTO veiculoDto, IVeiculoServico veiculoServico) =>
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
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Veículos");

                endpoints.MapDelete("/veiculos/{id:int}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
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
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Veículos");
                #endregion
            });
        }
    }
}