using System.Net;
using Artroplus.Core.IInterface;
using Artroplus.Core.IRepositories;
using Artroplus.Data.Context;
using Artroplus.Data.Repositories;
using Artroplus.Service.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// BAĞLANTI AYARLARI
// CLAUDE.md Kural 4: Connection string Environment Variable'dan okunur
// =============================================
var connectionString = Environment.GetEnvironmentVariable("ARTROPLUS_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ArtroplusDbContext>(options =>
    options.UseSqlServer(connectionString));

// =============================================
// DI KAYITLARI
// CLAUDE.md Kural 7: Servisler interface üzerinden kaydedilir
// =============================================
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

// =============================================
// AUTHENTICATION & AUTHORIZATION
// CLAUDE.md Kural 1: Tüm endpoint'ler korunmalıdır
// =============================================
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Artroplus API", Version = "v1" });
});

var app = builder.Build();

// =============================================
// IP WHITELIST MIDDLEWARE — CLAUDE.md Kural 3
// Bu middleware kaldırılamaz ve devre dışı bırakılamaz.
// Artroplus.Api yalnızca localhost / 127.0.0.1 kaynaklı isteklere yanıt verir.
// =============================================
app.Use(async (context, next) =>
{
    var remoteIp = context.Connection.RemoteIpAddress;

    if (remoteIp == null ||
        (!IPAddress.IsLoopback(remoteIp) &&
         !remoteIp.Equals(IPAddress.Parse("127.0.0.1"))))
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Erişim Reddedildi: Bu API yalnızca yerel ağdan erişilebilir.");
        return;
    }

    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
