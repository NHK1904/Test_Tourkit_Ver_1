using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Test_Tourkit.DTO;

namespace Test_Tourkit.Models
{
    public partial class Test_TourkitContext : DbContext
    {
        public Test_TourkitContext()
        {
        }

        public Test_TourkitContext(DbContextOptions<Test_TourkitContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductCategory> ProductCategories { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId).HasColumnName("Product_Id");

                entity.Property(e => e.DateAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("Date_Added");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(255)
                    .HasColumnName("Product_Name");

                entity.HasMany(d => d.ProductCategories)
                    .WithMany(p => p.Products)
                    .UsingEntity<Dictionary<string, object>>(
                        "ProductCategoryMapping",
                        l => l.HasOne<ProductCategory>().WithMany().HasForeignKey("ProductCategoryId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Product_C__Produ__29572725"),
                        r => r.HasOne<Product>().WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Product_C__Produ__286302EC"),
                        j =>
                        {
                            j.HasKey("ProductId", "ProductCategoryId").HasName("PK__Product___313915EC2D2124C6");

                            j.ToTable("Product_Category_Mapping");

                            j.IndexerProperty<int>("ProductId").HasColumnName("Product_Id");

                            j.IndexerProperty<int>("ProductCategoryId").HasColumnName("Product_Category_Id");
                        });
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("Product_Category");

                entity.Property(e => e.ProductCategoryId).HasColumnName("Product_Category_Id");

                entity.Property(e => e.DateAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("Date_Added");

                entity.Property(e => e.ProductCategory1)
                    .HasMaxLength(255)
                    .HasColumnName("Product_Category");
            });


            OnModelCreatingPartial(modelBuilder);

        }


        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public DbSet<Test_Tourkit.DTO.ProductDTO>? ProductDTO { get; set; }
    }
}
