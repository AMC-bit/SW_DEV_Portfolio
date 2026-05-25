using BlazorServerProgettoAnagrafiche.Components;
using Microsoft.EntityFrameworkCore;
using ProgettoAnagrafiche.Models;
using BlazorServerProgettoAnagrafiche.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add controllers
builder.Services.AddControllers();

// add services
builder.Services.AddScoped<ContattoService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<RicercaService>();

// Register DbContextFactory for EF Core
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Map controllers before Blazor components
app.MapControllers();

// Map Blazor components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
