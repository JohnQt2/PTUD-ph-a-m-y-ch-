using MaterialApi.Data;
using MaterialApi.Exceptions;
using MaterialApi.Models.Common;
using MaterialApi.Models.Dtos;
using MaterialApi.Models.Entities;
using MaterialApi.Services.Interfaces;

namespace MaterialApi.Services.Implementations;

/// <summary>
/// Triển khai IMaterialService sử dụng LINQ Query Syntax.
/// Áp dụng: trim value, business validation, phân trang.
/// </summary>
public class QuerySyntaxMaterialService : IMaterialService
{
    public string ImplementationName => "Query Syntax Style";

    // ─── READ ────────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public PagedResult<MaterialDto> GetMaterialsPaged(
        int page, int pageSize, string? searchName = null)
    {
        // Trim từ khóa tìm kiếm trước khi dùng
        var keyword = searchName?.Trim().ToLower() ?? string.Empty;

        // Left Join bằng Query Syntax: into + DefaultIfEmpty
        var query = from material in SeedData.Materials
                    join supplier in SeedData.Suppliers
                        on material.SupplierId equals supplier.Id into supplierGroup
                    from subSupplier in supplierGroup.DefaultIfEmpty()
                    select new MaterialDto
                    {
                        Id         = material.Id,
                        Name       = material.Name,
                        Unit       = material.Unit,
                        UnitPrice  = material.UnitPrice,
                        SupplierName         = subSupplier != null ? subSupplier.Name : "Chưa có nhà cung cấp",
                        SupplierAddress      = subSupplier?.Address,
                        SupplierContactPhone = subSupplier?.ContactPhone,
                        SourceImplementation = ImplementationName
                    };

        // Lọc theo từ khóa (nếu có)
        if (!string.IsNullOrEmpty(keyword))
            query = from dto in query
                    where dto.Name.ToLower().Contains(keyword) ||
                          dto.SupplierName.ToLower().Contains(keyword)
                    select dto;

        // Sắp xếp ổn định theo Id trước khi phân trang
        query = from dto in query
                orderby dto.Id
                select dto;

        return PagedResult<MaterialDto>.Create(query, page, pageSize);
    }

    /// <inheritdoc/>
    public IEnumerable<MaterialDto> GetMaterialsWithSuppliers()
    {
        var query = from material in SeedData.Materials
                    join supplier in SeedData.Suppliers
                    on material.SupplierId equals supplier.Id
                    select new MaterialDto
                    {
                        Id         = material.Id,
                        Name       = material.Name,
                        Unit       = material.Unit,
                        UnitPrice  = material.UnitPrice,
                        SupplierName         = supplier.Name,
                        SupplierAddress      = supplier.Address,
                        SupplierContactPhone = supplier.ContactPhone,
                        SourceImplementation = ImplementationName
                    };

        return query.ToList();
    }

    /// <inheritdoc/>
    public IEnumerable<MaterialDto> GetMaterialsWithSuppliersLeftJoin()
    {
        var query = from material in SeedData.Materials
                    join supplier in SeedData.Suppliers
                    on material.SupplierId equals supplier.Id into supplierGroup
                    from subSupplier in supplierGroup.DefaultIfEmpty()
                    select new MaterialDto
                    {
                        Id         = material.Id,
                        Name       = material.Name,
                        Unit       = material.Unit,
                        UnitPrice  = material.UnitPrice,
                        SupplierName         = subSupplier != null ? subSupplier.Name : "Chưa có nhà cung cấp",
                        SupplierAddress      = subSupplier?.Address,
                        SupplierContactPhone = subSupplier?.ContactPhone,
                        SourceImplementation = ImplementationName
                    };

        return query.ToList();
    }

    /// <inheritdoc/>
    public MaterialDto GetMaterialById(int id)
    {
        var query = from material in SeedData.Materials
                    where material.Id == id
                    join supplier in SeedData.Suppliers
                        on material.SupplierId equals supplier.Id into supplierGroup
                    from subSupplier in supplierGroup.DefaultIfEmpty()
                    select new MaterialDto
                    {
                        Id         = material.Id,
                        Name       = material.Name,
                        Unit       = material.Unit,
                        UnitPrice  = material.UnitPrice,
                        SupplierName         = subSupplier != null ? subSupplier.Name : "Chưa có nhà cung cấp",
                        SupplierAddress      = subSupplier?.Address,
                        SupplierContactPhone = subSupplier?.ContactPhone,
                        SourceImplementation = ImplementationName
                    };

        var result = query.FirstOrDefault();
        if (result == null)
            throw new NotFoundException($"Không tìm thấy vật liệu với Id = {id}.");

        return result;
    }

    // ─── CREATE ──────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public MaterialDto CreateMaterial(CreateMaterialRequest request)
    {
        // 1. Trim toàn bộ string inputs
        var trimmedName = request.Name.Trim();
        var trimmedUnit = request.Unit.Trim();

        // 2. Business validation: kiểm tra trùng tên (case-insensitive)
        var isDuplicate = (from m in SeedData.Materials
                           where m.Name.Trim().ToLower() == trimmedName.ToLower()
                           select m).Any();

        if (isDuplicate)
            throw new ValidationException($"Vật liệu có tên \"{trimmedName}\" đã tồn tại.");

        // 3. Kiểm tra SupplierId hợp lệ (nếu được cung cấp)
        if (request.SupplierId.HasValue)
        {
            var supplierExists = (from s in SeedData.Suppliers
                                  where s.Id == request.SupplierId.Value
                                  select s).Any();
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
        var material = (from m in SeedData.Materials
                        where m.Id == id
                        select m).FirstOrDefault();

        if (material == null)
            throw new NotFoundException($"Không tìm thấy vật liệu với Id = {id}.");

        // 2. Trim toàn bộ string inputs
        var trimmedName = request.Name.Trim();
        var trimmedUnit = request.Unit.Trim();

        // 3. Business validation: kiểm tra trùng tên với bản ghi KHÁC
        var isDuplicate = (from m in SeedData.Materials
                           where m.Id != id &&
                                 m.Name.Trim().ToLower() == trimmedName.ToLower()
                           select m).Any();

        if (isDuplicate)
            throw new ValidationException($"Vật liệu có tên \"{trimmedName}\" đã tồn tại.");

        // 4. Kiểm tra SupplierId hợp lệ (nếu được cung cấp)
        if (request.SupplierId.HasValue)
        {
            var supplierExists = (from s in SeedData.Suppliers
                                  where s.Id == request.SupplierId.Value
                                  select s).Any();
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
        var material = (from m in SeedData.Materials
                        where m.Id == id
                        select m).FirstOrDefault();

        if (material == null)
            throw new NotFoundException($"Không tìm thấy vật liệu với Id = {id}.");

        SeedData.Materials.Remove(material);
    }
}