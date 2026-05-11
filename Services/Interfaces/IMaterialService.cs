using MaterialApi.Models.Common;
using MaterialApi.Models.Dtos;

namespace MaterialApi.Services.Interfaces;

public interface IMaterialService
{
    string ImplementationName { get; }

    // ─── READ ────────────────────────────────────────────────────────────────
    /// <summary>Lấy danh sách vật liệu (Inner Join) có phân trang + tìm kiếm.</summary>
    PagedResult<MaterialDto> GetMaterialsPaged(
        int page, int pageSize, string? searchName = null);

    /// <summary>Lấy tất cả vật liệu có nhà cung cấp (Inner Join).</summary>
    IEnumerable<MaterialDto> GetMaterialsWithSuppliers();

    /// <summary>Lấy tất cả vật liệu kể cả không có nhà cung cấp (Left Join).</summary>
    IEnumerable<MaterialDto> GetMaterialsWithSuppliersLeftJoin();

    /// <summary>Lấy vật liệu theo ID.</summary>
    MaterialDto GetMaterialById(int id);

    // ─── CREATE ──────────────────────────────────────────────────────────────
    /// <summary>Tạo mới một vật liệu. Trả về DTO của bản ghi vừa tạo.</summary>
    MaterialDto CreateMaterial(CreateMaterialRequest request);

    // ─── UPDATE ──────────────────────────────────────────────────────────────
    /// <summary>Cập nhật vật liệu theo ID. Trả về DTO sau khi cập nhật.</summary>
    MaterialDto UpdateMaterial(int id, UpdateMaterialRequest request);

    // ─── DELETE ──────────────────────────────────────────────────────────────
    /// <summary>Xóa vật liệu theo ID.</summary>
    void DeleteMaterial(int id);
}
