public static class ApplicationEndpoints
{
    
    public static RouteGroupBuilder MapApplicationEndpoint(this IEndpointRouteBuilder app)
    {
        
        var group = app.MapGroup("/application");

        group.MapGet("/" , ()=> "Hello Application endpoint");

        return group;
    }
}