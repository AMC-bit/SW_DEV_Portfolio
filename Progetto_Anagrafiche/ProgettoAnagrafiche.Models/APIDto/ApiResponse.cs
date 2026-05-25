namespace ProgettoAnagrafiche.Models;

// apiresponse WITHOUT data

public class ApiResponse
    {
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public int StatusCode { get; set; }

    public ApiResponse()
        {
        Errors = new List<string>();
        }

    public static ApiResponse Ok(string message = "Success")
        => new()
            {
            Success = true,
            Message = message,
            StatusCode = 200,
            Errors = new()
            };

    public static ApiResponse Created(string message = "Created")
        => new()
            {
            Success = true,
            Message = message,
            StatusCode = 201,
            Errors = new()
            };

    public static ApiResponse BadRequest(string error, string message = "Validation failed")
        => BadRequest(new List<string> { error }, message);

    public static ApiResponse BadRequest(List<string> errors, string message = "Validation failed")
        => new()
            {
            Success = false,
            Errors = errors,
            Message = message,
            StatusCode = 400
            };

    public static ApiResponse NotFound(string message = "Not found")
        => new()
            {
            Success = false,
            Errors = new List<string> { message },
            Message = message,
            StatusCode = 404
            };

    public static ApiResponse InternalError(string message = "Internal server error")
        => new()
            {
            Success = false,
            Errors = new List<string> { message },
            Message = message,
            StatusCode = 500
            };
    }


/// <summary>
/// Standard API response wrapper used by all REST endpoints
/// WPF clients receive this JSON structure in responses
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int StatusCode { get; set; }


    public ApiResponse()
    {
        Errors = new List<string>();
    }

    public static ApiResponse<T> Ok(T data, string message = "Success")
        => new()
        {
            Success = true,
            Data = data,
            Message = message,
            StatusCode = 200,
            Errors = new()
        };

    public static ApiResponse<T> Created(T data, string message = "Created")
        => new()
        {
            Success = true,
            Data = data,
            Message = message,
            StatusCode = 201,
            Errors = new()
        };

    // accept a single string as overload too

    public static ApiResponse<T> BadRequest(string error, string message = "Validation failed")
    => BadRequest(new List<string> { error }, message);

    public static ApiResponse<T> BadRequest(List<string> errors, string message = "Validation failed")
        => new()
        {
            Success = false,
            Errors = errors,
            Message = message,
            StatusCode = 400
        };

    public static ApiResponse<T> NotFound(string message = "Not found")
        => new()
        {
            Success = false,
            Errors = new List<string> { message },
            Message = message,
            StatusCode = 404
        };

    public static ApiResponse<T> InternalError(string message = "Internal server error")
        => new()
        {
            Success = false,
            Errors = new List<string> { message },
            Message = message,
            StatusCode = 500
        };
}