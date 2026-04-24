public static class NotesEndpoints
{
    
    public static RouteGroupBuilder MapNotesEndpoint(this IEndpointRouteBuilder app)
    {


        var group = app.MapGroup("/notes");

        group.MapGet("/" , ()=> "Hello Notes endpoint");

        return group;
        
    }
}