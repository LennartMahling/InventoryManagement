using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using BE_InventoryManagement;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InventoryContext>(options =>
    options.UseSqlite("Data Source=inventory.db"));

builder.Services.AddHttpClient();

var app = builder.Build();

//
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

app.Run();