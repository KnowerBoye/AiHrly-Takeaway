using AihrlyApi.DTOs;
using AihrlyApi.Entities;
using AihrlyApi.Services;
public static class JobEndpoints
{
    
    public static RouteGroupBuilder MapJobEndpoint(this IEndpointRouteBuilder app)
    {
        
        var group = app.MapGroup("/jobs");

        /// <summary>
        /// Create job
        /// </summary>
        
        group.MapPost("/" , async (CreateJobRequest request , JobService service) =>
        {
            var job = new Job
            {
                title = request.title,
                description = request.description,
                location = request.location,
               
            };

            var jobResponse = await service.CreateAsync(job);

            return Results.Created($"api/jobs/{jobResponse?.id}", jobResponse);

        })
        .AddEndpointFilter<ValidationFilter<CreateJobRequest>>();

        
        /// <summary>
        /// Get job by id
        /// </summary>
        group.MapGet("/{id:guid}" , async (Guid id , JobService service) =>
        {
            var job = await service.GetByIdAsync(id);

            if (job == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(job);
        });

        
        
        /// <summary>
        /// Get all jobs with pagination and optional status filter
        group.MapGet("/" , async ( JobService service , 
        Status? status,  int page = 1 , int pageSize = 20) =>
        {
            
            page = Math.Max(page, 1);
            pageSize = Math.Max(pageSize, 1);

            var result = await service.GetAllAsync(status , page , pageSize);
            return Results.Ok(result);
        });




        /// <summary>
        /// Create application for a job
        /// </summary>
        group.MapPost("/{jobId:guid}/applications" , async (Guid jobId , 
        CreateApplicationRequest request , ApplicationService service) =>
        {

            // Console.WriteLine(jobId);
            
            var result = await service.CreateAsync(jobId , request);

            if(result == null) return Results.NotFound("Job not found.");

            if(!result.isSuccess) return Results.Problem(detail: result.error, statusCode: StatusCodes.Status400BadRequest);

            return Results.Created($"api/applications/{result.data?.id}", result.data);
        })
        .AddEndpointFilter<ValidationFilter<CreateApplicationRequest>>();



        /// <summary>
        /// list al applications for a job with optional stage filter
        /// </summary>

        group.MapGet("/{jobId:guid}/applications" , async (Guid jobId , 
        ApplicationStages? stage , ApplicationService service) =>
        {   
            var result = await service.GetByJobAsync(jobId , stage);

            if(result == null) return Results.NotFound("Job not found.");

            return Results.Ok(result);
            
        });

        return group;
    }




    


    
}