using GerenciadorEventos.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorEventos
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseDefaultServiceProvider(options =>
            {
                options.ValidateScopes = false; // Evita erro caso o BD ainda não tenha sido criado
            });

            // Adiciona suporte a MVC
            builder.Services.AddControllersWithViews();

            // Configuração da Entity Framework Core com SQL Server
            builder.Services.AddDbContext<Context>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"), // Aqui está a correção!
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));

            var app = builder.Build();

            // Configuração de ambiente
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Configuração das rotas
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Popula o banco de dados com os dados iniciais
            SeedData.EnsurePopulated(app);

            app.Run();
        }
    }
}
