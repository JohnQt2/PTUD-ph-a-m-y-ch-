using MaterialApi.Data;
using MaterialApi.Exceptions;
using MaterialApi.Models.Common;
using MaterialApi.Models.Dtos;
using MaterialApi.Models.Entities;
using MaterialApi.Services.Interfaces;

namespace MaterialApi.Services.Implementations;

/// <summary>
/// Triển khai IMaterialService sử dụng LINQ Method Syntax.
/// Áp dụng: trim value, business validation, phân trang.
/// </summary>
public class MethodSyntaxMaterialService : IMaterialService
{
    public string ImplementationName => "Method Syntax Style";

    // ─── READ ────────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public PagedResult<MaterialDto> GetMaterialsPaged(
        int page, int pageSize, string? searchName = null)
    {
        // Trim từ khóa tìm kiếm trước khi dùng
        var keyword = searchName?.Trim().ToLower() ?? string.Empty;

        // Left Join để giữ cả vật liệu không có nhà cung cấp
        var query = SeedData.Materials
            .GroupJoin(
                SeedData.Suppliers,
                m => m.SupplierId,
                s => s.Id,
                (m, sGroup) => new { m, sGroup }
            )
            .SelectMany(
                x => x.sGroup.DefaultIfEmpty(),
                (x, s) => new MaterialDto
                {
                    Id         = x.m.Id,
                    Name       = x.m.Name,
                    Unit       = x.m.Unit,
                    UnitPrice  = x.m.UnitPrice,
                    SupplierName         = s?.Name ?? "Chưa có nhà cung cấp",
                    SupplierAddress      = s?.Address,
                    SupplierContactPhone = s?.ContactPhone,
                    SourceImplementation = ImplementationName
                }
            );

        // Lọc theo từ khóa (nếu có)
        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(dto =>
                dto.Name.ToLower().Contains(keyword) ||
                dto.SupplierName.ToLower().Contains(keyword));

        // Sắp xếp ổn định theo Id trước khi phân trang
        query = query.OrderBy(dto => dto.Id);

        return PagedResult<MaterialDto>.Create(query, page, pageSize);
    }

    /// <inheritdoc/>
    public IEnumerable<MaterialDto> GetMaterialsWithSuppliers()
    {
        return SeedData.Materials.Join(
            SeedData.Suppliers,
            material => material.SupplierId,
            supplier => supplier.Id,
            (material, supplier) => new MaterialDto
            {
                Id         = material.Id,
                Name       = material.Name,
                Unit       = material.Unit,
                UnitPrice  = material.UnitPrice,
                SupplierName         = supplier.Name,
                SupplierAddress      = supplier.Address,
                SupplierContactPhone = supplier.ContactPhone,
                SourceImplementation = ImplementationName
            }
        ).ToList();
    }

    /// <inheritdoc/>
    public IEnumerable<MaterialDto> GetMaterialsWithSuppliersLeftJoin()
    {
        return SeedData.Materials
            .GroupJoin(
                SeedData.Suppliers,
                material => material.SupplierId,
                supplier => supplier.Id,
                (material, supplierGroup) => new { material, supplierGroup }
            )
            .SelectMany(
                x => x.supplierGroup.DefaultIfEmpty(),
                (x, supplier) => new MaterialDto
                {
                    Id         = x.material.Id,
                    Name       = x.material.Name,
                    Unit       = x.material.Unit,
                    UnitPrice  = x.material.UnitPrice,
                    SupplierName         = supplier?.Name ?? "Chưa có nhà cung cấp",
                    SupplierAddress      = supplier?.Address,
                    SupplierContactPhone = supplier?.ContactPhone,
                    SourceImplementation = ImplementationName
                }
            ).ToList();
    }

    /// <inheritdoc/>
    public MaterialDto GetMaterialById(int id)
    {
        var material = SeedData.Materials.FirstOrDefault(m => m.Id == id);
        if (material == null)
            throw new NotFoundException($"Không tìm thấy vật liệu với Id = {id}.");

        var supplier = SeedData.Suppliers.FirstOrDefault(s => s.Id == material.SupplierId);

        return new MaterialDto
        {
            Id         = material.Id,
            Name       = material.Name,
            Unit       = material.Unit,
            UnitPrice  = material.UnitPrice,
            SupplierName         = supplier?.Name ?? "Chưa có nhà cung cấp",
            SupplierAddress      = supplier?.Address,
            SupplierContactPhone = supplier?.ContactPhone,
            SourceImplementation = ImplementationName
        };
    }

    // ─── CREATE ──────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public MaterialDto CreateMaterial(CreateMaterialRequest request)
    {
        // 1. Trim toàn bộ string inputs
        var trimmedName = request.Name.Trim();
        var trimmedUnit = request.Unit.Trim();

        // 2. Business validation: kiểm tra trùng tên (case-insensitive)
        var isDuplicate = SeedData.Materials
            .Any(m => m.Name.Trim().Equals(trimmedName, StringComparison.OrdinalIgnoreCase));

        if (isDuplicate)
            throw new ValidationException($"Vật liệu có tên \"{trimmedName}\" đã tồn tại.");

        // 3. Kiểm tra SupplierId hợp lệ (nếu được cung cấp)
        if (request.SupplierId.HasValue)
        {
            var supplierExists = SeedData.Suppliers.Any(s => s.Id == request.SupplierId.Value);
            if (!supplierExists)
                throw new NotFoundException($"Không tìm thấy nhà cung cấp với Id = {request.SupplierId}.");
        }

        // 4. Tạo entity mới
        var newMaterial = new Material
        {
            Id         = SeedData.GetNextMaterialId(),
            Name       = trimmedName,
            Unit       = trimmedUnit,
            UnitPrice  = request.UnitPrice,
            SupplierId = request.SupplierId
        };

        SeedData.Materials.Add(newMaterial);

        // 5. Trả về DTO của bản ghi vừa tạo
        return GetMaterialById(newMaterial.Id);
    }

    // ─── UPDATE ──────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public MaterialDto UpdateMaterial(int id, UpdateMaterialRequest request)
    {
        // 1. Kiểm tra tồn tại
        var material = SeedData.Materials.FirstOrDefault(m => m.Id == id);
        if (material == null)
            throw new NotFoundException($"Không tìm thấy vật liệu với Id = {id}.");

        // 2. Trim toàn bộ string inputs
        var trimmedName = request.Name.Trim();
        var trimmedUnit = request.Unit.Trim();

        // 3. Business validation: kiểm tra trùng tên với bản ghi KHÁC
        var isDuplicate = SeedData.Materials
            .Any(m => m.Id != id &&
                      m.Name.Trim().Equals(trimmedName, StringComparison.OrdinalIgnoreCase));

        if (isDuplicate)
            throw new ValidationException($"Vật liệu có tên \"{trimmedName}\" đã tồn tại.");

        // 4. Kiểm tra SupplierId hợp lệ (nếu được cung cấp)
        if (request.SupplierId.HasValue)
        {
            var supplierExists = SeedData.Suppliers.Any(s => s.Id == request.SupplierId.Value);
            if (!supplierExists)
                throw new NotFoundException($"Không tìm thấy nhà cung cấp với Id = {request.SupplierId}.");
        }

        // 5. Cập nhật
        material.Name       = trimmedName;
        material.Unit       = trimmedUnit;
        material.UnitPrice  = request.UnitPrice;
        material.SupplierId = request.SupplierId;

        // 6. Trả về DTO sau khi cập nhật
        return GetMaterialById(id);
    }

    // ─── DELETE ──────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public void DeleteMaterial(int id)
    {
        var material = SeedData.Materials.FirstOrDefault(m => m.Id == id);
        if (material == null)
            throw new NotFoundException($"Không tìm thấy vật liệu với Id = {id}.");

        SeedData.Materials.Remove(material);
    }
}
