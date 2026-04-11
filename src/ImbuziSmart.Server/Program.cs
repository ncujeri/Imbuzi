using Microsoft.EntityFrameworkCore;
using ImbuziSmart.Server.Data;
using ImbuziSmart.Server.Middleware;

var builder = WebApplication.CreateBuilder(args);

// EF Core with SQL Server
builder.Services.AddDbContext<ImbuziDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("ImbuziSmart.Server")));

// Multi-tenancy support
builder.Services.AddHttpContextAccessor();

// Authentication — IdentityServer4 / JWT Bearer
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Identity:Authority"];
        options.TokenValidationParameters.ValidateAudience = false;
    });
builder.Services.AddAuthorization();

// API controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseTenantValidation();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
