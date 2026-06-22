namespace BrbHabitaciones.Application.DTOs.Common;

public record ApiResponse<T>(
    bool Success,
    T? Data = default,
    string? Message = null,
    IEnumerable<string>? Errors = null
)
{
    public static ApiResponse<T> Ok(T data, string? message = null) =>
        new(true, data, message);

    public static ApiResponse<T> Fail(string message, IEnumerable<string>? errors = null) =>
        new(false, default, message, errors);
}
