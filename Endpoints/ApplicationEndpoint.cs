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
        })
        .WithName("GetApplicationProfile")
        .WithSummary("Retrieve complete application profile")
        .WithDescription("Fetches the full application profile including personal details, current stage, scores across all dimensions, notes, and stage history.")
        .WithTags("Applications")
        .Produces<ApplicationProfileResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();


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

            if(!result.isSuccess) return Results.ValidationProblem(new Dictionary<string, string[]> { { "stage", new[] { result.error } } });

            return Results.NoContent();


            
        })
        .AddEndpointFilter<TeamMemberFilter>()
        .AddEndpointFilter<FluentValidationFilter<UpdateApplicationStageRequest>>()
        .WithName("UpdateApplicationStage")
        .WithSummary("Move application to next stage")
        .WithDescription("Updates the application's current stage with an optional comment. Requires team member authentication. Validates the stage transition and records the change in history.")
        .WithTags("Applications", "Stage Management")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status422UnprocessableEntity)
        .WithOpenApi();

        
        
        
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

            if(result is null) return Results.NotFound();

            return Results.Created($"api/applications/{id}/notes/{result.id}" , result);
            
        })
        .AddEndpointFilter<TeamMemberFilter>()
        .AddEndpointFilter<FluentValidationFilter<CreateApplicationNoteRequest>>()
        .WithName("AddApplicationNote")
        .WithSummary("Add note to application")
        .WithDescription("Creates a new note attached to an application. Requires team member authentication. Notes can be used to track feedback, follow-ups, or observations about the candidate.")
        .WithTags("Applications", "Notes")
        .Produces<ApplicationNoteResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();




        /// <summary>
        /// Get all notes for an application
        /// </summary>
        group.MapGet("/{id:guid}/notes" , async (Guid id , ApplicationService service) => {

            var result = await service.GetNotesAsync(id);

            if(result is null) return Results.NotFound();

            return Results.Ok(result);
        })
        .WithName("GetApplicationNotes")
        .WithSummary("Retrieve all notes for an application")
        .WithDescription("Fetches all notes associated with an application, including author information and timestamps. Notes are returned in chronological order.")
        .WithTags("Applications", "Notes")
        .Produces<List<ApplicationNoteDetails>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();




        /// application scoring endpoints 
        /// 
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
        .AddEndpointFilter<ValidationFilter<UpsertScoreRequest>>()
        .WithName("UpsertCultureFitScore")
        .WithSummary("Score application on culture fit")
        .WithDescription("Creates or updates the culture fit score for an application. Score must be between 1 and 5. Requires team member authentication.")
        .WithTags("Applications", "Scoring")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status422UnprocessableEntity)
        .WithOpenApi();


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
        .AddEndpointFilter<ValidationFilter<UpsertScoreRequest>>()
        .WithName("UpsertInterviewScore")
        .WithSummary("Score application on interview performance")
        .WithDescription("Creates or updates the interview score for an application. Score must be between 1 and 5. Requires team member authentication.")
        .WithTags("Applications", "Scoring")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status422UnprocessableEntity)
        .WithOpenApi();


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
        .AddEndpointFilter<ValidationFilter<UpsertScoreRequest>>()
        .WithName("UpsertAssessmentScore")
        .WithSummary("Score application on technical assessment")
        .WithDescription("Creates or updates the assessment score for an application. Score must be between 1 and 5. Requires team member authentication.")
        .WithTags("Applications", "Scoring")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status422UnprocessableEntity)
        .WithOpenApi();


        return group;
    }
}