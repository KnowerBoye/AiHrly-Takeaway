public class TeamMemberFilter : IEndpointFilter
{
    
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context , EndpointFilterDelegate next)
    {
        
        var httpContext = context.HttpContext;

        if (httpContext.Request.Headers.TryGetValue("X-Team-Member", out var memberID))
        {
           
            return await next(context);
        }
        else
        {
            return Results.Unauthorized();
        }
    }
}