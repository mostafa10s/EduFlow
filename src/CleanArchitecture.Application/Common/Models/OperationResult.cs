
namespace CleanArchitecture.Application.Common.Models;

public class OperationResult
{
    protected OperationResult()
    {
        OperationId = Guid.NewGuid();
    }

    public Guid OperationId { get; }

    public bool Success { get; protected set; }

    public int Code { get; protected set; }

    public string Message { get; protected set; } = string.Empty;

    public IDictionary<string, string[]> Errors { get; protected set; }
        = new Dictionary<string, string[]>();

    public static OperationResult SuccessOperation(
        int code = 200,
        string message = "Operation completed successfully.")
    {
        return new OperationResult
        {
            Success = true,
            Code = code,
            Message = message
        };
    }

    public static OperationResult Failure(
        IDictionary<string, string[]>? errors = null,
        int code = 400,
        string message = "Operation failed.")
    {
        return new OperationResult
        {
            Success = false,
            Code = code,
            Message = message,
            Errors = errors ?? new Dictionary<string, string[]>()
        };
    }
}

public class OperationResult<TData> : OperationResult
{
    public TData? Data { get; private set; }

    public static OperationResult<TData> SuccessOperation(
        TData data,
        int code = 200,
        string message = "Operation completed successfully.")
    {
        return new OperationResult<TData>
        {
            Success = true,
            Code = code,
            Message = message,
            Data = data
        };
    }

    public new static OperationResult<TData> Failure(
        IDictionary<string, string[]>? errors = null,
        int code = 400,
        string message = "Operation failed.")
    {
        return new OperationResult<TData>
        {
            Success = false,
            Code = code,
            Message = message,
            Errors = errors ?? new Dictionary<string, string[]>()
        };
    }
}
