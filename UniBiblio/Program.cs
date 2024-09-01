using Microsoft.EntityFrameworkCore;
using UniBiblio.Models;

var builder = WebApplication.CreateBuilder(args);

// Aggiungi Razor Pages e modifica i percorsi di ricerca delle view
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        // Aggiungi la cartella Pages ai percorsi di ricerca delle view
        options.PageViewLocationFormats.Add("/Pages/{0}.cshtml");
    });

// Aggiungi supporto per la configurazione
builder.Configuration.AddJsonFile("appsettings.json");

// Aggiungi i servizi delle sessioni
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Timeout della sessione
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddRazorPages();
// Aggiungi DBContext. Dependency Injection
builder.Services.AddDbContext<UniBiblioContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 31))));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Configura le sessioni
app.UseSession();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.UseAuthorization();

app.MapRazorPages();

app.Run();
