using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthenticationDemo.Models;

public partial class EShopConfigDbContext : DbContext
{
    public EShopConfigDbContext()
    {
    }

    public EShopConfigDbContext(DbContextOptions<EShopConfigDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ProductCatalog> ProductCatalogs { get; set; }

    public virtual DbSet<RefreshSession> RefreshSessions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCatalog>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__ProductC__B40CC6CD83882267");

            entity.ToTable("ProductCatalog");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(150);
        });

        modelBuilder.Entity<RefreshSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__RefreshS__C9F49290BF32C7BD");

            entity.Property(e => e.SessionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.RefreshToken).HasMaxLength(256);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshSessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__RefreshSe__UserI__46E78A0C");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A8AD19EBA");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160DD6DBFE7").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CEB674F74");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E476BE29E3").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__UserRoles__RoleI__3F466844"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__UserRoles__UserI__3E52440B"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__UserRole__AF2760AD874B70AD");
                        j.ToTable("UserRoles");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
