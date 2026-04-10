public static class ValidationExtensions
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder handler) where T : class
        => handler.AddEndpointFilter(new ValidationFilter<T>());
}
