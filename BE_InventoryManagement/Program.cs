using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using BE_InventoryManagement;

var builder = WebApplication.CreateBuilder(args);

//Datenbank registrieren,nutzt die InventoryConext Klasse
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


//GET: Gibt in den Inventarinhalt als Liste mit Datentyp "Product" zurück. 
List<Product> GetInventory(InventoryContext db)
{
    return db.Inventory.ToList();
}

app.MapGet("/api/inventory", GetInventory);

app.Run();






/*
var httpClientFactory = app.Services.GetRequiredService<IHttpClientFactory>();
var client = httpClientFactory.CreateClient();
client.DefaultRequestHeaders.Add("User-Agent", "InventarsystemTHW");

string testBarcode = "4013143010061";

Console.WriteLine(testBarcode);
Console.WriteLine($" STARTE API-TEST FÜR BARCODE: {testBarcode}");

try
{
    var response = await client.GetAsync($"https://world.openfoodfacts.org/api/v2/product/{testBarcode}.json");
    
    if(response.IsSuccessStatusCode)
    {
        string json = await response.Content.ReadAsStringAsync();
        
        Console.WriteLine(json);
    }
    else
    {
        Console.WriteLine(response.StatusCode);
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    //throw;
}

app.Run();*/