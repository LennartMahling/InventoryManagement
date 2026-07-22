using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using BE_InventoryManagement;

var builder = WebApplication.CreateBuilder(args);

// Datenbank registrieren
builder.Services.AddDbContext<InventoryContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// HTTP Client registrieren
builder.Services.AddHttpClient();

// CORS aktivieren
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://thw.wetterbox-gh23.de", "https://www.thw.wetterbox-gh23.de")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();

// GET: Gibt den Inhalt des Inventars als Liste zurück
app.MapGet("/api/inventory", async (InventoryContext db) =>
{
    var items = await db.Inventory.ToListAsync();
    return Results.Ok(items);
});

// POST: Schreibt ein neues Produkt in die Datenbank
app.MapPost("/api/inventory", async (Product input, InventoryContext db, IHttpClientFactory httpClientFactory) =>
{
    var existingProduct = await db.Inventory.FirstOrDefaultAsync(p => p.ArticleNumber == input.ArticleNumber && p.ExpirationDate == input.ExpirationDate);

    if (existingProduct != null)
    {
        existingProduct.Quantity += input.Quantity;
        await db.SaveChangesAsync();
        return Results.Ok(existingProduct);
    }

    input.Id = 0;
    string productName = "Unbekanntes Produkt";
    string companyName = "Unbekannte Marke";

    var client = httpClientFactory.CreateClient();
    client.DefaultRequestHeaders.Add("User-Agent", "LebensmittelInventar/1.0 (MqxleYT@gmail.com)");

    try
    {
        var response = await client.GetAsync($"https://world.openfoodfacts.org/api/v2/product/{input.ArticleNumber}.json");

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            using var jsonDocument = JsonDocument.Parse(jsonString);
            var root = jsonDocument.RootElement;

            if (root.TryGetProperty("status", out var statusProp) && statusProp.GetInt32() == 1)
            {
                var productData = root.GetProperty("product");
                
                if (productData.TryGetProperty("product_name", out var nameProp))
                {
                    productName = nameProp.GetString() ?? productName;
                }
                
                if (productData.TryGetProperty("brandProp", out var brandProp) || productData.TryGetProperty("brands", out brandProp))
                {
                    companyName = brandProp.GetString() ?? companyName;
                }
            }
        }
    }
    catch (Exception)
    {
        // Fallback-Werte bleiben greifen
    }

    input.ProductName = productName;
    input.CompanyName = companyName;
    input.Price = 0.0m;
    
    db.Inventory.Add(input);
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/inventory/{input.Id}", input);
});

// PUT: Produkt bearbeiten
app.MapPut("/api/inventory/{id}", async (int id, Product updateData, InventoryContext db) =>
{
    var product = await db.Inventory.FindAsync(id);

    if (product is null)
    {
        return Results.NotFound();
    }
    
    product.ProductName = updateData.ProductName;
    product.CompanyName = updateData.CompanyName;
    product.Quantity = updateData.Quantity;
    product.ExpirationDate = updateData.ExpirationDate;
    product.Price = updateData.Price;
    
    await db.SaveChangesAsync();
    return Results.Ok(product);
});

// DELETE: Produkt löschen
app.MapDelete("/api/inventory/{id}", async (int id, InventoryContext db) =>
{
    var product = await db.Inventory.FindAsync(id);

    if (product is null)
    {
        return Results.NotFound();
    }

    db.Inventory.Remove(product);
    await db.SaveChangesAsync();
    
    return Results.NoContent();
});

// Automatische Migration / DB-Erstellung beim Start
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryContext>();
    db.Database.Migrate();
}

app.Run();