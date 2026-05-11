namespace MaterialApi.Models.Entities;

public class Material
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int? SupplierId { get; set; }
}
