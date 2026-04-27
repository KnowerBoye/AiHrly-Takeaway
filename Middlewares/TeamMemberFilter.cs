public class TeamMemberFilter : IEndpointFilter
{
    
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context , EndpointFilterDelegate next)
    {
        
        var httpContext = context.HttpContext;

        if (!httpContext.Request.Headers.TryGetValue("X-Team-Member-Id", out var memberIdString))
        {
            return Results.Unauthorized();
        }

        if (!Guid.TryParse(memberIdString, out var memberId))
        {
            return Results.Unauthorized();
        }


        httpContext.Items["TeamMemberId"] = memberId;

        return await next(context);
    }
}