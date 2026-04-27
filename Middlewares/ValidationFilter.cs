using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using FluentValidation;

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

            return Results.ValidationProblem(errors);
        }

        return await next(context);
    }
}




public class FluentValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
{

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, 
        EndpointFilterDelegate next)
    {
        var dto = context.Arguments.OfType<T>().FirstOrDefault();

        var validationResult = await validator.ValidateAsync(dto);

        if (!validationResult.IsValid) return Results.ValidationProblem(validationResult.ToDictionary());
        

        return await next(context);
    }
}



