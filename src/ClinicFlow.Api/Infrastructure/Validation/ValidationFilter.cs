using FluentValidation;

namespace ClinicFlow.Api.Infrastructure.Validation;

/// <summary>
/// Runs the FluentValidation validator for <typeparamref name="TRequest"/> before the endpoint handler.
/// Invalid input never reaches the handler: the filter returns 400 with errors grouped by property.
/// </summary>
public sealed class ValidationFilter<TRequest>(IValidator<TRequest> validator) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();

        if (request is null)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Request body is missing or invalid.");
        }

        var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

        if (!result.IsValid)
        {
            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        return await next(context);
    }
}

public static class ValidationExtensions
{
    /// <summary>Adds <see cref="ValidationFilter{TRequest}"/> to an endpoint and documents the 400 response.</summary>
    public static RouteHandlerBuilder WithValidation<TRequest>(this RouteHandlerBuilder builder) =>
        builder
            .AddEndpointFilter<ValidationFilter<TRequest>>()
            .ProducesValidationProblem();
}
