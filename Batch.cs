public class Batch
{
    public int Id { get; set; }
    public string BatchNumber { get; set; } = "";
    public int ItemId { get; set; }
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? Date { get; set; }
    public string? Status { get; set; }
}
