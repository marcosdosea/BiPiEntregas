using Microsoft.EntityFrameworkCore;
using Core;
using Core.Service;
using Service;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


var app = builder.Build();

builder.Services.AddDbContext<BiPiEntregasContext>(options =>
              options.UseMySQL(builder.Configuration.GetConnectionString("BiPiEntregasDatabase") ?? throw new InvalidOperationException("Conexão com o banco de dados não foi possível. Favor entrar em contato com o Administrador.")));

builder.Services.AddTransient<IPessoaService, PessoaService>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();