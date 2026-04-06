using Amigos.DataAccessLayer;
using Amigos.Services;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace Amigos
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var supportedCultures = new[] { "es-ES", "en-US" };

            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture("es-ES")
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);


            // Add services to the container.
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.AddControllersWithViews()
                .AddViewLocalization();

            builder.Services.AddSingleton<IInc, IncImpl>();
            // builder.Services.AddTransient<IInc, IncImpl>();

            // Para añadir la base de datos
            builder.Services.AddDbContext<AmigoDBContext>(options => options.UseSqlite("Data Source=Amigos.db"));

            var app = builder.Build();

            app.UseRequestLocalization(localizationOptions);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "Prueba",
                pattern: "{controller}/{action}/{valor}/{veces}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}