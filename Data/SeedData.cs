using Bogus;
using AihrlyApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace AihrlyApi.Data{
public static class SeedData
{
    public static async Task Run(ApiDbContext context)
    {
     
        var defaultMemberId = Guid.Parse("00000000-0000-0000-0000-000000000001");


        var faker = new Faker();


        var job = new Job
        {
            id = Guid.NewGuid(),
            title = faker.Name.JobTitle(),
            description = faker.Lorem.Paragraph(),
            location = faker.Address.City(),
            status = Status.open,
            applications = new List<Application>()
        };


        for (int i = 0; i < 5; i++)
        {
            var app = new Application
            {
                id = Guid.NewGuid(),
                jobId = job.id,
                name = faker.Name.FullName(),
                email = faker.Internet.Email(),
                coverLetter = faker.Lorem.Paragraph(),
                current_stage = ApplicationStages.applied,
                notes = new List<ApplicationNote>(),
              
            };

            int noteCount = faker.Random.Int(2, 5);

            for (int n = 0; n < noteCount; n++)
            {
                app.notes.Add(new ApplicationNote
                {
                    id = Guid.NewGuid(),
                    applicationId = app.id,
                    type = faker.PickRandom<ApplicationNoteType>(),
                    description = faker.Lorem.Sentence(),
                    created_by = defaultMemberId,
                    created_at = DateTime.UtcNow.AddDays(-n)
                });
            }



            job.applications.Add(app);
        }

        context.Jobs.Add(job);

        await context.SaveChangesAsync();
    }
}
}