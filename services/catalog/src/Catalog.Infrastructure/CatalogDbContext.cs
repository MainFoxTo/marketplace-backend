using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<ProductVersion> ProductVersions { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("catalog");

            // Конфигурация Product
            modelBuilder.Entity<Product>(b =>
            {
                b.ToTable("Products");
                b.HasKey(p => p.Id);
                b.Property(p => p.Sku).IsRequired().HasMaxLength(64);
                b.Property(p => p.Name).IsRequired().HasMaxLength(200);

                // Связь с версиями (односторонняя настройка)
                b.HasMany(p => p.Versions)
                 .WithOne(v => v.Product)
                 .HasForeignKey(v => v.ProductId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Конфигурация ProductVersion
            modelBuilder.Entity<ProductVersion>(b =>
            {
                b.ToTable("ProductVersions");
                b.HasKey(v => v.Id);
                b.Property(v => v.Sku).IsRequired().HasMaxLength(64);
                b.Property(v => v.Price).HasPrecision(18, 2);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}





//protected override void OnModelCreating(ModelBuilder modelBuilder)
//{
//    // 1. В какой "папке" (схеме) создавать таблицы?
//    modelBuilder.HasDefaultSchema("catalog"); // Все таблицы пойдут в схему `catalog`, а не в общую кучу

//    // 2. Настраиваем конкретно таблицу для Product
//    modelBuilder.Entity<Product>(b => // b = builder для продукта
//    {
//    // а) Даем таблице имя (хотя оно и так будет "Products")
//    b.ToTable("Products");

//    // б) Говорим, что поле `Id` - это главный ключ таблицы (Primary Key)
//    b.HasKey(p => p.Id);
//    // Эквивалент SQL: PRIMARY KEY (Id)

//    // в) Настраиваем поле Sku: ОНО ОБЯЗАТЕЛЬНОЕ и не длиннее 64 символов
//    b.Property(p => p.Sku).IsRequired().HasMaxLength(64);
//    // Эквивалент SQL: Sku NVARCHAR(64) NOT NULL

//    // г) Настраиваем поле Name: ОНО ОБЯЗАТЕЛЬНОЕ и не длиннее 200 символов
//    b.Property(p => p.Name).IsRequired().HasMaxLength(200);
//        // Эквивалент SQL: Name NVARCHAR(200) NOT NULL
//}