namespace MaterialApi.Exceptions;

/// <summary>
/// Exception ném ra khi dữ liệu vi phạm business rules
/// (ví dụ: trùng tên, dữ liệu không hợp lệ về mặt nghiệp vụ).
/// Khác với Data Annotations validation (xảy ra ở tầng Controller),
/// exception này xảy ra ở tầng Service.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}
