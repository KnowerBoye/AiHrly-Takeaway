public static class ScoresEndpoints
{
    
    public static RouteGroupBuilder MapScoresEndpoint(this IEndpointRouteBuilder app)
    {
        
        var group = app.MapGroup("/scores");

        group.MapGet("/" , ()=> "Hello Scores endpoint");

        return group;
    }
}