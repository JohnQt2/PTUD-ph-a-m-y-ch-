using MaterialApi.Models.Common;
using MaterialApi.Models.Dtos;
using MaterialApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MaterialApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaterialsController : ControllerBase
{
    private readonly IMaterialService _materialService;

    public MaterialsController(IMaterialService materialService)
    {
        _materialService = materialService;
    }

    // ─── READ ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Lấy danh sách vật liệu có phân trang và tìm kiếm theo tên.
    /// </summary>
    /// <param name="page">Số trang (mặc định: 1)</param>
    /// <param name="pageSize">Số bản ghi mỗi trang (mặc định: 5, tối đa: 50)</param>
    /// <param name="search">Từ khóa tìm kiếm theo tên vật liệu hoặc nhà cung cấp (tùy chọn)</param>
    [HttpGet]
    public IActionResult GetMaterials(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? search = null)
    {
        // Validation tham số phân trang
        if (page < 1)
            return BadRequest(ApiResponse<object>.FailureResponse("Số trang (page) phải lớn hơn 0."));

        if (pageSize < 1 || pageSize > 50)
            return BadRequest(ApiResponse<object>.FailureResponse("Số bản ghi mỗi trang (pageSize) phải từ 1 đến 50."));

        var result = _materialService.GetMaterialsPaged(page, pageSize, search);
        return Ok(ApiResponse<PagedResult<MaterialDto>>.SuccessResponse(result,
            $"Trang {result.Page}/{result.TotalPages} - Tổng {result.TotalCount} bản ghi."));
    }

    /// <summary>Lấy vật liệu theo ID.</summary>
    [HttpGet("{id:int}")]
    public IActionResult GetMaterialById(int id)
    {
        var material = _materialService.GetMaterialById(id);
        return Ok(ApiResponse<MaterialDto>.SuccessResponse(material));
    }

    /// <summary>Lấy vật liệu có nhà cung cấp - Inner Join.</summary>
    [HttpGet("inner-join")]
    public IActionResult GetMaterialsInnerJoin()
    {
        var materials = _materialService.GetMaterialsWithSuppliers();
        return Ok(ApiResponse<IEnumerable<MaterialDto>>.SuccessResponse(materials));
    }

    /// <summary>Lấy tất cả vật liệu kể cả không có nhà cung cấp - Left Join.</summary>
    [HttpGet("left-join")]
    public IActionResult GetMaterialsLeftJoin()
    {
        var materials = _materialService.GetMaterialsWithSuppliersLeftJoin();
        return Ok(ApiResponse<IEnumerable<MaterialDto>>.SuccessResponse(materials));
    }

    // ─── CREATE ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Tạo mới một vật liệu.
    /// Áp dụng: Data Annotations validation (ModelState) + trim + business validation.
    /// </summary>
    [HttpPost]
    public IActionResult CreateMaterial([FromBody] CreateMaterialRequest request)
    {
        // 1. Data Annotations validation (tự động qua [ApiController], nhưng ta xử lý rõ ràng)
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return UnprocessableEntity(
                ApiResponse<object>.FailureResponse("Dữ liệu không hợp lệ.", errors));
        }

        // 2. Gọi service (bên trong có trim + business validation)
        var created = _materialService.CreateMaterial(request);

        return CreatedAtAction(
            nameof(GetMaterialById),
            new { id = created.Id },
            ApiResponse<MaterialDto>.SuccessResponse(created, "Tạo vật liệu thành công."));
    }

    // ─── UPDATE ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Cập nhật vật liệu theo ID.
    /// Áp dụng: Data Annotations validation (ModelState) + trim + business validation.
    /// </summary>
    [HttpPut("{id:int}")]
    public IActionResult UpdateMaterial(int id, [FromBody] UpdateMaterialRequest request)
    {
        // 1. Data Annotations validation
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return UnprocessableEntity(
                ApiResponse<object>.FailureResponse("Dữ liệu không hợp lệ.", errors));
        }

        // 2. Gọi service (bên trong có trim + business validation)
        var updated = _materialService.UpdateMaterial(id, request);

        return Ok(ApiResponse<MaterialDto>.SuccessResponse(updated, "Cập nhật vật liệu thành công."));
    }

    // ─── DELETE ──────────────────────────────────────────────────────────────

    /// <summary>Xóa vật liệu theo ID.</summary>
    [HttpDelete("{id:int}")]
    public IActionResult DeleteMaterial(int id)
    {
        _materialService.DeleteMaterial(id);
        return Ok(ApiResponse<object>.SuccessResponse(null!, "Xóa vật liệu thành công."));
    }
}
