using ApbdCw7.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApbdCw7.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Pc> PCs => Set<Pc>();
    public DbSet<Component> Components => Set<Component>();
    public DbSet<PcComponent> PCComponents => Set<PcComponent>();
    public DbSet<ComponentManufacturer> ComponentManufacturers => Set<ComponentManufacturer>();
    public DbSet<ComponentType> ComponentTypes => Set<ComponentType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ComponentManufacturer>(entity =>
        {
            entity.ToTable("ComponentManufacturers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Abbreviation).HasMaxLength(30).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(300).IsRequired();
            entity.Property(e => e.FoundationDate).HasColumnType("date");
        });

        modelBuilder.Entity<ComponentType>(entity =>
        {
            entity.ToTable("ComponentTypes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Abbreviation).HasMaxLength(30).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
        });

        modelBuilder.Entity<Component>(entity =>
        {
            entity.ToTable("Components");
            entity.HasKey(e => e.Code);
            entity.Property(e => e.Code).HasMaxLength(10).IsFixedLength().IsRequired();
            entity.Property(e => e.Name).HasMaxLength(300).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.HasOne(e => e.Manufacturer)
                .WithMany(m => m.Components)
                .HasForeignKey(e => e.ComponentManufacturersId)
                .HasConstraintName("Components_ComponentManufacturers");
            entity.HasOne(e => e.Type)
                .WithMany(t => t.Components)
                .HasForeignKey(e => e.ComponentTypesId)
                .HasConstraintName("Components_ComponentTypes");
        });

        modelBuilder.Entity<Pc>(entity =>
        {
            entity.ToTable("PCs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Weight).IsRequired();
            entity.Property(e => e.Warranty).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired();
            entity.Property(e => e.Stock).IsRequired();
        });

        modelBuilder.Entity<PcComponent>(entity =>
        {
            entity.ToTable("PCComponents");
            entity.HasKey(e => new { e.PcId, e.ComponentCode });
            entity.Property(e => e.ComponentCode).HasMaxLength(10).IsFixedLength().IsRequired();
            entity.Property(e => e.Amount).IsRequired();
            entity.HasOne(e => e.Pc)
                .WithMany(p => p.PcComponents)
                .HasForeignKey(e => e.PcId)
                .HasConstraintName("PCComponents_PCs")
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Component)
                .WithMany(c => c.PcComponents)
                .HasForeignKey(e => e.ComponentCode)
                .HasConstraintName("PCComponents_Components");
        });

        SeedData.Apply(modelBuilder);
    }
}
