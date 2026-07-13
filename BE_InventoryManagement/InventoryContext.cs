using System.Data.Common;

namespace BE_InventoryManagement;

using Microsoft.EntityFrameworkCore;

public class InventoryContext : DbContext
{
    //Konstruktor, options später in Program.cs
    public InventoryContext(DbContextOptions<InventoryContext> options) : base(options){}

    public DbSet<Product> Products { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            //Tabelle in der BD soll "Inventory" heißen
            entity.ToTable("Inventory");
            //ID ist der Primärschlüssel
            entity.HasKey(p => p.Id);
            //
            entity.HasIndex(p => new { p.ArticleNumber, p.ExpirationDate })
                .IsUnique();
            //ArticleNumber muss vorhanden sein und ist max. 13 Zahlen lang (EAN8 & EAN13)
            entity.Property(p => p.ArticleNumber)
                .IsRequired()
                .HasMaxLength(13);
            //ProductName muss vorhanden sein und ist max. 250 Zeichen lang
            entity.Property(p => p.ProductName)
                .IsRequired()
                .HasMaxLength(250);
            //CompanyName muss vorhanden sein und ist max. 250 Zeichen lang
            entity.Property(p => p.CompanyName)
                .IsRequired()
                .HasMaxLength(250);
            //Quantity muss vorhanden sein, standardmäßig 1 (Logik, da nie null!)
            entity.Property(p => p.Quantity)
                .IsRequired()
                .HasDefaultValue(1);
            //Price muss vorhanden sein, standardmößig 0.0
            entity.Property(p => p.Price)
                .IsRequired()
                .HasDefaultValue(0.0);
            //MHD muss vorhanden sein
            entity.Property(p => p.Price)
                .IsRequired();
        });
    }
}