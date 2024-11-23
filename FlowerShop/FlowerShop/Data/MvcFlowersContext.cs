using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcFlowers.Models;

namespace MvcFlowers.Data
{
    public class MvcFlowersContext : DbContext
    {
        public MvcFlowersContext(DbContextOptions<MvcFlowersContext> options)
            : base(options)
        {
        }

        // public DbSet<MvcFlowers.Models.Flower> Flowers { get; set; } = default!;
        public DbSet<MvcFlowers.Models.Bouqet> Bouqet { get; set; } = default!;
        public DbSet<Flower> Flowers { get; set; }
        public DbSet<Pack> Packs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<FlowerInBouqet> FlowerInBouqets { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flower>()
                .Property(f => f.Price)
                .HasColumnType("decimal(18,2)"); //  точность и масштаб
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<FlowerInBouqet>().HasNoKey();

        //}
        //    // Настройка отношения "многие ко многим" между Bouqet и MonoFlowers
        //    modelBuilder.Entity<Bouqet>()
        //        .HasMany(b => b.Flowers)
        //        .WithMany()
        //        .UsingEntity<Dictionary<string, object>>(
        //            "BouqetFlowers", // Имя промежуточной таблицы
        //            j => j
        //                .HasOne<Flower>()
        //                .WithMany()
        //                .HasForeignKey("MonoFlowerId")
        //                .OnDelete(DeleteBehavior.Cascade),
        //            j => j
        //                .HasOne<Bouqet>()
        //                .WithMany()
        //                .HasForeignKey("BouqetId")
        //                .OnDelete(DeleteBehavior.Cascade),
        //            j =>
        //            {
        //                j.HasKey("BouqetId", "MonoFlowerId"); // Установка составного ключа
        //            });
        //}

    }
}
