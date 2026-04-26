using AihrlyApi.Data;
using AihrlyApi.Entities;
using Microsoft.EntityFrameworkCore;
using AihrlyApi.DTOs;





namespace AihrlyApi.Services
{
    public class JobService(ApiDbContext context)
    {


        public async Task CreateAsync(Job job)
        {
            job.id = Guid.NewGuid();

            context.Jobs.Add(job);
            await context.SaveChangesAsync();

   
        }

        public async Task<JobResponse?> GetByIdAsync(Guid id)
        {
            var job = await context.Jobs
                .FirstOrDefaultAsync(j => j.id == id);

            if (job is null) return null; 

            return new JobResponse
            {
                id = job.id,
                title = job.title,
                description = job.description,
                location = job.location,
                status = job.status
           
            } ;
        }

        public async Task<PagedResult<JobResponse>> GetAllAsync(
            Status? status,
            int page,
            int pageSize)
        {
            var query = context.Jobs.AsQueryable();

            if (status is not null)
                query = query.Where(j => j.status == status);

            var total = await query.CountAsync();

            var jobs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(job => new JobResponse
                {
                    id = job.id,
                    title = job.title,
                    description = job.description,
                    location = job.location,
                    status = job.status
                })
                .ToListAsync();

    

            return new PagedResult<JobResponse>
            {
                items = jobs,
                totalCount = total
            };
        }
    }
}