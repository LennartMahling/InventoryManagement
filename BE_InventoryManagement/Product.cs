namespace BE_InventoryManagement;

public class Product
{
    public int Id { get; set; }
    public string ArticleNumber { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double Price { get; set; }
    public DateTime ExpirationDate { get; set; }
}