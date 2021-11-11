using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace LaurensProjects.Models1
{
    public partial class airportdbContext : DbContext
    {
        public airportdbContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("DB_CONNECTION_STRING");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
        public airportdbContext(DbContextOptions<airportdbContext> options) : base(options)
        {
        }

        public virtual DbSet<CityWeatherLog> CityWeatherLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<CityWeatherLog>(entity =>
            {
                entity.HasKey(e => e.CityId);

                entity.ToTable("CityWeatherLog");

                entity.Property(e => e.CityId).ValueGeneratedNever();

                entity.Property(e => e.CityName)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.FromDate).HasColumnType("datetime");

                entity.Property(e => e.GeneratedAt).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.ToDate).HasColumnType("datetime");

                entity.Property(e => e.WindSpeed).HasColumnType("decimal(18, 0)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}