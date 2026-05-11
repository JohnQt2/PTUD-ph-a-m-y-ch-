using MaterialApi.Models.Entities;

namespace MaterialApi.Data;

/// <summary>
/// Dữ liệu giả lập (in-memory) dùng thay cho database.
/// Dùng static field (không phải property) để dữ liệu tồn tại
/// trong suốt vòng đời ứng dụng và có thể thêm/sửa/xóa.
/// </summary>
public static class SeedData
{
    // ─── ID counter ───────────────────────────────────────────────────────────
    private static int _nextMaterialId = 11;   // tiếp nối id cuối cùng trong seed

    // ─── Materials ────────────────────────────────────────────────────────────
    public static List<Material> Materials { get; } = new List<Material>
    {
        new Material { Id = 1,  Name = "Xi măng PC40",      Unit = "Tấn",   UnitPrice = 1500000,  SupplierId = 2 },
        new Material { Id = 2,  Name = "Thép cuộn Ø6",      Unit = "Tấn",   UnitPrice = 16500000, SupplierId = 1 },
        new Material { Id = 3,  Name = "Thép thanh Ø18",    Unit = "Tấn",   UnitPrice = 17200000, SupplierId = 1 },
        new Material { Id = 4,  Name = "Gạch đỏ đặc",       Unit = "Viên",  UnitPrice = 1200,     SupplierId = 3 },
        new Material { Id = 5,  Name = "Cát vàng",          Unit = "Khối",  UnitPrice = 350000,   SupplierId = 4 },
        new Material { Id = 6,  Name = "Đá 1x2",            Unit = "Khối",  UnitPrice = 420000,   SupplierId = 4 },
        new Material { Id = 7,  Name = "Gạch block",        Unit = "Viên",  UnitPrice = 9500,     SupplierId = 3 },
        new Material { Id = 8,  Name = "Xi măng PCB30",     Unit = "Tấn",   UnitPrice = 1400000,  SupplierId = 2 },
        new Material { Id = 9,  Name = "Ống nhựa PVC",      Unit = "Mét",   UnitPrice = 67000,    SupplierId = null },
        new Material { Id = 10, Name = "Sơn chống thấm",    Unit = "Thùng", UnitPrice = 890000,   SupplierId = null },
    };

    // ─── Suppliers ────────────────────────────────────────────────────────────
    public static List<Supplier> Suppliers { get; } = new List<Supplier>
    {
        new Supplier { Id = 1, Name = "Thép Hòa Phát",        Address = "Hà Nội",   ContactPhone = "0901000001" },
        new Supplier { Id = 2, Name = "Xi măng Xuân Thành",   Address = "Hà Nam",   ContactPhone = "0901000002" },
        new Supplier { Id = 3, Name = "Gạch Viglacera",       Address = "Bắc Ninh", ContactPhone = "0901000003" },
        new Supplier { Id = 4, Name = "Cát sạch Sông Đà",     Address = "Hòa Bình", ContactPhone = "0901000004" },
    };

    // ─── ID generation ────────────────────────────────────────────────────────
    /// <summary>Trả về ID tiếp theo cho Material mới (auto-increment đơn giản).</summary>
    public static int GetNextMaterialId() => _nextMaterialId++;
}
