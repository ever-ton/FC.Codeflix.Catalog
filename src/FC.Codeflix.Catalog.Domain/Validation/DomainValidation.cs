using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.Domain.Validation;
public class DomainValidation
{
    public static void NotNull(object? target, string filedName)
    {
        if (target is null)
            throw new EntityValidationException($"{filedName} should not be null");
    }

    public static void NotNullOrEmpty(string? target, string filedName)
    {
        if (string.IsNullOrWhiteSpace(target))
            throw new EntityValidationException($"{filedName} should not be null or empty");
    }

    public static void MinLength(string target, int minLength,string fieldName)
    {
        if (target.Length < minLength)
            throw new EntityValidationException($"{fieldName} should not be less than {minLength} characters long");
    }

    public static void MaxLength(string target, int maxLength, string fieldName)
    {
        if (target.Length > maxLength)
            throw new EntityValidationException($"{fieldName} should not be greater than {maxLength} characters long");
    }
}
