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
        .AddEndpointFilter<ValidationFilter<CreateJobRequest>>()
        .WithName("CreateJob")
        .WithSummary("Create a new job posting")
        .WithDescription("Creates a new job posting with title, description, and location. Returns the created job with its ID.")
        .WithTags("Jobs")
        .Produces<JobResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status422UnprocessableEntity)
        .WithOpenApi();

        
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
        })
        .WithName("GetJobById")
        .WithSummary("Retrieve a job posting by ID")
        .WithDescription("Fetches a specific job posting by its unique ID. Returns 404 if the job does not exist.")
        .WithTags("Jobs")
        .Produces<JobResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        
        
        /// <summary>
        /// Get all jobs with pagination and optional status filter
        /// </summary>
        group.MapGet("/" , async ( JobService service , 
        Status? status,  int page = 1 , int pageSize = 20) =>
        {
            
            page = Math.Max(page, 1);
            pageSize = Math.Max(pageSize, 1);

            var result = await service.GetAllAsync(status , page , pageSize);
            return Results.Ok(result);
        })
        .WithName("ListAllJobs")
        .WithSummary("List all job postings")
        .WithDescription("Retrieves a paginated list of all job postings. Supports optional filtering by status. Default page size is 20.")
        .WithTags("Jobs")
        .Produces<PagedResult<JobResponse>>(StatusCodes.Status200OK)
        .WithOpenApi();




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
        .AddEndpointFilter<ValidationFilter<CreateApplicationRequest>>()
        .WithName("CreateJobApplication")
        .WithSummary("Submit a job application")
        .WithDescription("Creates a new application for a specific job posting. Requires candidate name, email, and optional cover letter.")
        .WithTags("Jobs", "Applications")
        .Produces<ApplicationResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest)
        .WithOpenApi();




        /// <summary>
        /// List all applications for a job with optional stage filter
        /// </summary>
        group.MapGet("/{jobId:guid}/applications" , async (Guid jobId , 
        ApplicationStages? stage , ApplicationService service) =>
        {   
            var result = await service.GetByJobAsync(jobId , stage);

            if(result == null) return Results.NotFound("Job not found.");

            return Results.Ok(result);
            
        })
        .WithName("ListJobApplications")
        .WithSummary("Retrieve all applications for a job")
        .WithDescription("Fetches all applications submitted for a specific job. Supports optional filtering by application stage.")
        .WithTags("Jobs", "Applications")
        .Produces<List<ApplicationResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        return group;
    }




    


    
}