using AihrlyApi.DTOs;
using AihrlyApi.Entities;
using Microsoft.EntityFrameworkCore;
using AihrlyApi.Data;



namespace AihrlyApi.Services
{


   
    public class Result
    {
        public bool isSuccess { get; set; }
        public string? error { get; set; }

    }
    public class ApplicationService(ApiDbContext context)
    {
        


    public async Task<Result?> CreateAsync(Guid jobId, CreateApplicationRequest request)
        {   

            var checkJob = await context.Jobs.AnyAsync(j => j.id == jobId);

            if(!checkJob) return null;

            var exists = await context.Applications.AnyAsync(a => a.email == request.email && a.jobId == jobId);

            if(exists) return new Result
            {
                isSuccess = false,
                error = "An application with the same email already exists for this job."
            };

             
            var application = new Application
            {
                id = Guid.NewGuid(),
                jobId = jobId,
                name = request.name,
                email = request.email,
                coverLetter = request.coverLetter,
            };

            context.Applications.Add(application);
            await context.SaveChangesAsync();

            return new Result
            {
                isSuccess = true
            };
        }

    public async Task<PagedResult<ApplicationResponse>?> GetByJobAsync(
    Guid jobId,
    ApplicationStages? stage)
        {


            var checkJob = await context.Jobs.AnyAsync(j => j.id == jobId);

            if(!checkJob) return null;

            var query = context.Applications
                .Where(a => a.jobId == jobId);

            if (stage is not null)
                query = query.Where(a => a.current_stage == stage);

            var total = await query.CountAsync();

            var items = await query
                .Select(app => new ApplicationResponse
                {
                    id = app.id,
                    name = app.name,
                    email = app.email,
                    current_stage = app.current_stage
                })
                .ToListAsync();

            return new PagedResult<ApplicationResponse>
            {
                items = items,
                totalCount = total
            };
        }


    
    }
}
