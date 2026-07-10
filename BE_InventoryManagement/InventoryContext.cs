using System.Data.Common;

namespace BE_InventoryManagement;

using Microsoft.EntityFrameworkCore;

public class InventoryContext : DbContext
{
    //Konstruktor, options später in Program.cs
    public InventoryContext(DbContextOptions<InventoryContext> options) : base(options){}

    public DbSet<Product> Products { get; set; } = default!;
}