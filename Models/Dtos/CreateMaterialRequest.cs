using System.ComponentModel.DataAnnotations;

namespace MaterialApi.Models.Dtos;

/// <summary>
/// DTO nhận dữ liệu từ client khi tạo mới một Material.
/// Áp dụng Data Annotations để validation tự động qua ModelState.
/// </summary>
public class CreateMaterialRequest
{
    [Required(ErrorMessage = "Tên vật liệu không được để trống.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Tên vật liệu phải từ 2 đến 100 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Đơn vị tính không được để trống.")]
    [StringLength(30, MinimumLength = 1,
        ErrorMessage = "Đơn vị tính không được vượt quá 30 ký tự.")]
    public string Unit { get; set; } = string.Empty;

    [Required(ErrorMessage = "Đơn giá không được để trống.")]
    [Range(0.01, double.MaxValue,
        ErrorMessage = "Đơn giá phải lớn hơn 0.")]
    public decimal UnitPrice { get; set; }

    // SupplierId là optional (nullable)
    [Range(1, int.MaxValue,
        ErrorMessage = "SupplierId phải là số nguyên dương nếu được cung cấp.")]
    public int? SupplierId { get; set; }
}
