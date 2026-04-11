using ImbuziSmart.Server.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core with SQL Server
builder.Services.AddDbContext<ImbuziDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("ImbuziSmart.Server")));

// TODO: Add IdentityServer4 / JWT Bearer authentication
// builder.Services.AddAuthentication("Bearer")
//     .AddJwtBearer("Bearer", options => {
//         options.Authority = builder.Configuration["Identity:Authority"];
//         options.Audience = "imbuzi-api";
//     });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
