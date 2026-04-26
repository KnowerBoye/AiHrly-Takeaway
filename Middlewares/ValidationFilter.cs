using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var dto = context.Arguments.OfType<T>().FirstOrDefault();

        if (dto is null) return Results.BadRequest();

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(dto);

        var isValid = Validator.TryValidateObject(
            dto,
            validationContext,
            validationResults,
            validateAllProperties: true);

        if (!isValid)
        {
            var errors = validationResults
                .GroupBy(x => x.MemberNames.FirstOrDefault() ?? "")
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            return Results.BadRequest(new
            {
                message = "Validation failed",
                errors
            });
        }

        return await next(context);
    }
}