using FluentValidation;
using FluentValidation.AspNetCore;
using Inventory.Api.Middleware;
using Inventory.Api.Mapping;
using Inventory.Application.Interfaces;
using Inventory.Application.Services;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Inventory.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Serilog
            builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());

            // DB
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Port=5432;Database=marketplace;Username=pguser;Password=pgpass";

            builder.Services.AddDbContext<InventoryDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Services & Repositories
            builder.Services.AddScoped<IStockRepository, StockRepository>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();

            // AutoMapper & Validators
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Выполняем миграции ТОЛЬКО если это НЕ тестовое окружение
            if (!app.Environment.IsEnvironment("Testing"))
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
                db.Database.Migrate();
            }

            // Swagger + CORS в Dev
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseCors("AllowAll");
            }

            app.UseRouting();

            // Глобальный обработчик ошибок
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
