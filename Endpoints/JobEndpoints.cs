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

            await service.CreateAsync(job);

            return Results.Created();

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
        /// </summary>
        group.MapGet("/" , async (JobService service, Status? status , int page = 1 , int pageSize = 20) =>
        {
            
            page = Math.Max(page, 1);
            pageSize = Math.Max(pageSize, 1);

            var result = await service.GetAllAsync(status , page , pageSize);
            return Results.Ok(result);
        });

        return group;
    }


    
}