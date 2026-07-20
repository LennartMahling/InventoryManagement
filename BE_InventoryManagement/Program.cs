using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using BE_InventoryManagement;

var builder = WebApplication.CreateBuilder(args);

//Datenbank registrieren,nutzt die InventoryContext Klasse
builder.Services.AddDbContext<InventoryContext>(options =>
    options.UseSqlite("Data Source=inventory.db"));

//HTTP Client registrieren
builder.Services.AddHttpClient();

//CORS aktivieren
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();


//GET: Gibt in den Inhalt des Inventars als Liste mit Datentyp "Product" zurück. 
app.MapGet("/api/inventory", async (InventoryContext db) =>
{
    var items = await db.Inventory.ToListAsync();
    return Results.Ok(items);
});

//POST: Schreibt ein neues Produkt in die Datenbank hinein
app.MapPost("/api/inventory", async(Product input, InventoryContext db, IHttpClientFactory httpClientFactory) =>
{
    //Prüft ob gleicher Artikel mit gleichem MHD vorhanden ist. Falls ja, wird der neue Eintrag nur dazu addiert
    var existingProduct = await db.Inventory.FirstOrDefaultAsync(p => p.ArticleNumber == input.ArticleNumber && p.ExpirationDate == input.ExpirationDate);

    if (existingProduct != null)
    {
        existingProduct.Quantity += input.Quantity;
        await db.SaveChangesAsync();
        return Results.Ok(existingProduct);
    }

    input.Id = 0;
    //Fallback Werte
    string productName = "Unbekanntes Produkt";
    string companyName = "Unbekannte Marke";

    //HTTP Verbindung wird nach Anleitung von openfoodfacts erstellt
    var client = httpClientFactory.CreateClient();
    client.DefaultRequestHeaders.Add("User-Agent", "LebensmittelInventar/1.0");

    
    try
    {
        //Request mit der Artikelnummer (EAN)
        var response = await client.GetAsync($"https://world.openfoodfacts.org/api/v2/product/{input.ArticleNumber}.json");

        //Beginn JSON Parsing
        if (response.IsSuccessStatusCode)
        {
            //Rohe HTTP Antwort in einen String
            var jsonString = await response.Content.ReadAsStringAsync();
            //Entsprechende Antwort in HTTP Format umwandeln
            using var jsonDocument = JsonDocument.Parse(jsonString);
            var root = jsonDocument.RootElement;

            //Prüfen ob der Statuscode (Produkt gefunden) 1 entspricht und in statusProp auslagern
            if (root.TryGetProperty("status", out var statusProp) && statusProp.GetInt32() == 1)
            {
                var productData = root.GetProperty("product");
                
                //Prüfen ob "product_name" hinterlegt ist, falls Daten unvollständig sind
                if (productData.TryGetProperty("product_name", out var nameProp))
                {
                    //JSON Ausgabe in C# String umwandeln
                    productName = nameProp.GetString() ?? productName;
                }
                
                //Prüfen ob "brands" hinterlegt ist, falls Daten unvollständig sind
                if (productData.TryGetProperty("brands", out var brandProp))
                {
                    //JSON Ausgabe in C# String umwandeln
                    companyName = brandProp.GetString() ?? companyName;
                }
            }
        }
    }
    catch (Exception)
    {
        //Falls API offline ist, bleibt der Fallback auf die Defaultwerte
    }

    
    //Objekt mit Daten anreichern
    input.ProductName = productName;
    input.CompanyName = companyName;
    input.Price = 0.0m;
    
    //Daten in die Datenbank schreiben
    db.Inventory.Add(input);
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/inventory/{input.Id}", input);
});

app.MapPut("api/inventory/{id}", async (int id, Product updateData, InventoryContext db) =>
    {
        var product = await db.Inventory.FindAsync(id);

        if (product is null)
        {
            return Results.NotFound();
        }
        
        //Werte manuell mit dem Frontend überschreiben
        product.ProductName = updateData.ProductName;
        product.CompanyName = updateData.CompanyName;
        product.Quantity = updateData.Quantity;
        product.ExpirationDate = updateData.ExpirationDate;
        product.Price = updateData.Price;
        
        await db.SaveChangesAsync();
        return Results.Ok(product);
    });

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
    
    app.Run();