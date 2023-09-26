using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.Domain.Validation;
public class DomainValidation
{
    public static void NotNull(object? target, string filedName)
    {
        if(target is null)
            throw new EntityValidationException($"{filedName} should not be null");
    }

    public static void NotNullOrEmpty(string? target, string filedName)
    {
        if (string.IsNullOrWhiteSpace(target))
            throw new EntityValidationException($"{filedName} should not be null or empty");
    }
}
