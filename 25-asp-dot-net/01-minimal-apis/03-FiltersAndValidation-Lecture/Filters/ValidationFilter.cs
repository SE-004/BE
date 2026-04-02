using System.ComponentModel.DataAnnotations;

namespace FiltersLecture.Filters;

public sealed class ValidationFilter<T> : IEndpointFilter where T : class
{
  public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
  {
    var dto = context.Arguments.OfType<T>().FirstOrDefault();

    if (dto is null)
    {
      return Results.BadRequest(new { error = $"Request body must include a {typeof(T).Name}" });
    }


    var validationResults = new List<ValidationResult>();
    var vc = new ValidationContext(dto);
    bool isValid = Validator.TryValidateObject(dto, vc, validationResults, validateAllProperties: true);

    if (!isValid)
    {
      var errors = validationResults
                    .GroupBy(v => v.MemberNames.FirstOrDefault() ?? string.Empty)
                    .ToDictionary(g => g.Key, g => g.Select(r => r.ErrorMessage ?? "Invalid").ToArray());

      return Results.ValidationProblem(errors, statusCode: StatusCodes.Status400BadRequest);
    }

    return await next(context);
  }
}