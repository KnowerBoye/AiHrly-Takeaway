
public static class JobEndpoints
{
    
    public static RouteGroupBuilder MapJobEndpoint(this IEndpointRouteBuilder app)
    {
        
        var group = app.MapGroup("/jobs");

        group.MapGet("/" , ()=> "Hello Jobs endpoint");

        return group;
    }
}