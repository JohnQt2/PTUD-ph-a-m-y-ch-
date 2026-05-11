namespace MaterialApi.Models.Dtos;

public class MaterialDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string? SupplierAddress { get; set; }
    public string? SupplierContactPhone { get; set; }
    public string SourceImplementation { get; set; } = string.Empty;
}
