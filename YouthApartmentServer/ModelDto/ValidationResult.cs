using System.Collections.Generic;

namespace YouthApartmentServer.ModelDto;

public enum ValidationStatus
{
    Success,
    Failure,
    NotFound
}

public class ValidationResult
{
    public ValidationStatus Status { get; private set; } = ValidationStatus.Success;
    public List<string> Errors { get; } = new();
    public bool IsValid => Status == ValidationStatus.Success && Errors.Count == 0;

    public void AddError(string? message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        if (Status == ValidationStatus.Success)
        {
            Status = ValidationStatus.Failure;
        }
        Errors.Add(message);
    }

    public void AddErrors(IEnumerable<string>? messages)
    {
        if (messages == null) return;
        foreach (var message in messages)
        {
            AddError(message);
        }
    }

    public void MarkNotFound(string? message = null)
    {
        Status = ValidationStatus.NotFound;
        if (!string.IsNullOrWhiteSpace(message))
        {
            Errors.Add(message);
        }
    }
}

public class ValidationResult<T> : ValidationResult
{
    public T? Data { get; set; }
}
