using Microsoft.EntityFrameworkCore;
using UniBiblio.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// Aggiungi DBContext. Dependency Injection
builder.Services.AddDbContext<unibiblioContext>(options =>
    options.UseMySql("Server=localhost;Database=unibiblio;User=root;Password=DB09Gennaio;", new MySqlServerVersion(new Version(8, 0, 31))));

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

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.UseAuthorization();

app.MapRazorPages();

app.Run();
