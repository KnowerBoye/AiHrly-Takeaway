using AihrlyApi.DTOs;
using AihrlyApi.Entities;
using AihrlyApi.Services;
using AihrlyApi.Validators;
public static class ApplicationEndpoints
{
    
    public static RouteGroupBuilder MapApplicationEndpoint(this IEndpointRouteBuilder app)
    {
        
        var group = app.MapGroup("/applications");




        /// <summary>
        /// Fetch full application profile
        /// </summary>
        
        group.MapGet("/{id:guid}" , async (Guid id , ApplicationService service)=> {

            var application = await service.GetApplicationProfileAsync(id);

            if(application is null) return Results.NotFound();

            return Results.Ok(application);
        });


        /// <summary>
        /// Update application stage
        /// </summary>
        group.MapPatch("/{id:guid}/stage" , async (Guid id , 
        UpdateApplicationStageRequest request , 
        ApplicationService service , 
        HttpContext httpContext) =>
        {

            var TeamMemberId = (Guid) httpContext.Items["TeamMemberId"];

            var result = await service.UpdateStageAsync(id , request , TeamMemberId);

            if(result is null) return Results.NotFound();

            if(!result.isSuccess) return Results.BadRequest(result.error);

            return Results.NoContent();


            
        })
        .AddEndpointFilter<TeamMemberFilter>()
        .AddEndpointFilter<FluentValidationFilter<UpdateApplicationStageRequest>>();

        
        
        
        /// <summary>
        /// Add a note to an application
        /// </summary>
        group.MapPost("/{id:guid}/notes" , async (Guid id , 
        CreateApplicationNoteRequest request, 
        ApplicationService service , 
        HttpContext httpContext) =>
        {

            var TeamMemberId = (Guid) httpContext.Items["TeamMemberId"];

            var result = await service.AddNoteAsync(id , request , TeamMemberId );
            
        })
        .AddEndpointFilter<TeamMemberFilter>()
        .AddEndpointFilter<FluentValidationFilter<CreateApplicationNoteRequest>>();




        /// <summary>
        /// Get all notes for an application
        /// </summary>
        group.MapGet("/{id:guid}/notes" , async (Guid id , ApplicationService service) => {

            var result = await service.GetNotesAsync(id);

            if(result is null) return Results.NotFound();

            return Results.Ok(result);
        });


        group.MapPut("/{id:guid}/scores/culture-fit" , async (Guid id , 
        UpsertScoreRequest request ,
        ApplicationService service ,
        HttpContext httpContext) =>{
            
            var TeamMemberId = (Guid) httpContext.Items["TeamMemberId"];

            var result = await service.UpsertScoreAsync(id , ScoreDimension.culture_fit , request , TeamMemberId);

            if(result is null) return Results.NotFound();

            return Results.NoContent();
        }
        ).AddEndpointFilter<TeamMemberFilter>()
        .AddEndpointFilter<ValidationFilter<UpsertScoreRequest>>();


        group.MapPut("/{id:guid}/scores/interview" , async (Guid id , 
        UpsertScoreRequest request ,
        ApplicationService service ,
        HttpContext httpContext) =>{    
            
            var TeamMemberId = (Guid) httpContext.Items["TeamMemberId"];

            var result = await service.UpsertScoreAsync(id , ScoreDimension.interview , request , TeamMemberId);

            if(result is null) return Results.NotFound();

            return Results.NoContent();
        }
        ).AddEndpointFilter<TeamMemberFilter>()
        .AddEndpointFilter<ValidationFilter<UpsertScoreRequest>>();


        group.MapPut("/{id:guid}/scores/assessment" , async (Guid id , 
        UpsertScoreRequest request ,
        ApplicationService service ,
        HttpContext httpContext) =>{
            
            var TeamMemberId = (Guid) httpContext.Items["TeamMemberId"];

            var result = await service.UpsertScoreAsync(id , ScoreDimension.assessment , request , TeamMemberId);

            if(result is null) return Results.NotFound();

            return Results.NoContent();
        }
        ).AddEndpointFilter<TeamMemberFilter>()
        .AddEndpointFilter<ValidationFilter<UpsertScoreRequest>>();


        return group;
    }
}