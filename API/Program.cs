using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using API.Extensions;
using System.Reflection;
using AspNetCoreRateLimit;
using Serilog;
using Serilog.Core;
using API.Helpers.Errors;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//Configuramos serilog
Logger logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(builder.Configuration)
                            .Enrich.FromLogContext()
                            .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.
builder.Services.AddAutoMapper(Assembly.GetEntryAssembly());
builder.Services.ConfigureRateLimiting();
builder.Services.ConfigureCors();

builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
    options.ReturnHttpNotAcceptable = true;
}).AddXmlSerializerFormatters();

//agregamos los métodos de extensión
builder.Services.AddAplicacionServices();
builder.Services.ConfigureApiVersioning();
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddValidtionErrors();

builder.Services.AddDbContext<TiendaContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("Conexion"),
                                    new MySqlServerVersion(new Version(8, 0, 28)));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

//usamos mensajes personalizados para las exepciones
app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errors/{0}");

app.UseIpRateLimiting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ejecuta migraciones pendientes al iniciar la aplicación
using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    ILoggerFactory loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        TiendaContext context = services.GetRequiredService<TiendaContext>();
        await context.Database.MigrateAsync();
        await TiendaContextSeed.SeedAsync(context, loggerFactory);
        await TiendaContextSeed.SeedRolesAsync(context, loggerFactory);
    }
    catch (Exception ex)
    {
        ILogger<Program> _logger = loggerFactory.CreateLogger<Program>();
        _logger.LogError(ex, "Ocurrió un error durante la migración");
    }
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//Custom middlewares

app.MapControllers();

app.Run();
