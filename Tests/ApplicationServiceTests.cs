using AihrlyApi.Data;
using AihrlyApi.DTOs;
using AihrlyApi.Entities;
using AihrlyApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AihrlyApi.Tests;

public class ApplicationServiceTests
{
    private ApiDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApiDbContext(options);

     
        context.TeamMembers.AddRange(
            new TeamMember
            {
                id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                name = "Noah Boye",
                email = "noahboye@mail.com",
                role = TeamMemberRole.hiring_manager
            },
            new TeamMember
            {
                id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                name = "Jane Doe",
                email = "janedoe@mail.com",
                role = TeamMemberRole.recruiter
            },
            new TeamMember
            {
                id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                name = "John Boye",
                email = "johnboye@mail.com",
                role = TeamMemberRole.hiring_manager
            }
        );

        context.SaveChanges();

        return context;
    }

    #region Test 1: Stage Transition Rules - Valid Transitions

    [Fact]
    public void IsValidTransition_ReturnsTrue_ForValidTransitions()
    {

        Assert.True(ApplicationService.IsValidTransition(ApplicationStages.applied, ApplicationStages.screening));
        Assert.True(ApplicationService.IsValidTransition(ApplicationStages.applied, ApplicationStages.rejected));

        Assert.True(ApplicationService.IsValidTransition(ApplicationStages.screening, ApplicationStages.interview));
        Assert.True(ApplicationService.IsValidTransition(ApplicationStages.screening, ApplicationStages.rejected));

        Assert.True(ApplicationService.IsValidTransition(ApplicationStages.interview, ApplicationStages.offer));
        Assert.True(ApplicationService.IsValidTransition(ApplicationStages.interview, ApplicationStages.rejected));

        Assert.True(ApplicationService.IsValidTransition(ApplicationStages.offer, ApplicationStages.hired));
        Assert.True(ApplicationService.IsValidTransition(ApplicationStages.offer, ApplicationStages.rejected));
    }

    [Fact]
    public void IsValidTransition_ReturnsFalse_ForInvalidTransitions()
    {
        
        Assert.False(ApplicationService.IsValidTransition(ApplicationStages.applied, ApplicationStages.interview));
        Assert.False(ApplicationService.IsValidTransition(ApplicationStages.applied, ApplicationStages.offer));
        Assert.False(ApplicationService.IsValidTransition(ApplicationStages.applied, ApplicationStages.hired));

        // check backwards
        Assert.False(ApplicationService.IsValidTransition(ApplicationStages.screening, ApplicationStages.applied));
        Assert.False(ApplicationService.IsValidTransition(ApplicationStages.interview, ApplicationStages.screening));
        Assert.False(ApplicationService.IsValidTransition(ApplicationStages.offer, ApplicationStages.interview));

        // terminal states
        Assert.False(ApplicationService.IsValidTransition(ApplicationStages.hired, ApplicationStages.offer));
        Assert.False(ApplicationService.IsValidTransition(ApplicationStages.hired, ApplicationStages.rejected));
        Assert.False(ApplicationService.IsValidTransition(ApplicationStages.rejected, ApplicationStages.interview));
        Assert.False(ApplicationService.IsValidTransition(ApplicationStages.rejected, ApplicationStages.applied));

        // same state transitions
        Assert.False(ApplicationService.IsValidTransition(ApplicationStages.applied, ApplicationStages.applied));
        Assert.False(ApplicationService.IsValidTransition(ApplicationStages.screening, ApplicationStages.screening));
    }

    #endregion

    #region Test 2: test create application and add note features

    [Fact]
    public async Task CreateApplication_AddNote_ReadBack_AuthorNameResolvedCorrectly()
    {
       
       
        using var context = CreateDbContext();
        var service = new ApplicationService(context);


        var jobId = Guid.NewGuid();
        context.Jobs.Add(new Job
        {
            id = jobId,
            title = "Software Engineer",
            description = "Test job",
            location = "Remote",
            status = Status.open
        });
        await context.SaveChangesAsync();


        var createRequest = new CreateApplicationRequest
        {
            name = "Test Applicant",
            email = "test@example.com",
            coverLetter = "I am interested in this position"
        };

        var createResult = await service.CreateAsync(jobId, createRequest);
        Assert.NotNull(createResult);
        Assert.True(createResult.isSuccess);
        Assert.NotNull(createResult.data);

        var applicationId = createResult.data.id;

        // Add a note
        var teamMemberId = Guid.Parse("00000000-0000-0000-0000-000000000002"); 
        var noteRequest = new CreateApplicationNoteRequest
        {
            type = "general",
            description = "Strong candidate with good communication skills"
        };

        var noteResult = await service.AddNoteAsync(applicationId, noteRequest, teamMemberId);
        Assert.NotNull(noteResult);
        Assert.Equal("general", noteResult.type.ToString());
        Assert.Equal("Strong candidate with good communication skills", noteResult.description);

  
        var profile = await service.GetApplicationProfileAsync(applicationId);
        Assert.NotNull(profile);
        Assert.Single(profile.notes);

        var note = profile.notes[0];
        Assert.Equal("Strong candidate with good communication skills", note.description);
        Assert.Equal("Jane Doe", note.author_name); 
        Assert.Equal(teamMemberId, note.created_by);
    }

    #endregion

    #region Test 3: test score updates 

    [Fact]
    public async Task UpsertScore_SubmitTwice_SecondValueWins_UpdatedFieldsReflectSecondSubmission()
    {

        using var context = CreateDbContext();
        var service = new ApplicationService(context);


        var jobId = Guid.NewGuid();
        context.Jobs.Add(new Job
        {
            id = jobId,
            title = "Software Engineer",
            description = "Test job",
            location = "Remote",
            status = Status.open
        });
        await context.SaveChangesAsync();


        var createRequest = new CreateApplicationRequest
        {
            name = "Test Applicant",
            email = "test@example.com",
            coverLetter = "I am interested"
        };

        var createResult = await service.CreateAsync(jobId, createRequest);
        var applicationId = createResult.data.id;

        var firstTeamMemberId = Guid.Parse("00000000-0000-0000-0000-000000000001"); 
        var secondTeamMemberId = Guid.Parse("00000000-0000-0000-0000-000000000003"); 


        var firstScoreRequest = new UpsertScoreRequest
        {
            score = 3,
            comment = "Average performance"
        };

        var firstResult = await service.UpsertScoreAsync(applicationId, ScoreDimension.interview, firstScoreRequest, firstTeamMemberId);
        Assert.NotNull(firstResult);
        Assert.True(firstResult.isSuccess);

  
        //add delay leter 

        var secondScoreRequest = new UpsertScoreRequest
        {
            score = 5,
            comment = "Excellent performance after reconsideration"
        };

        var secondResult = await service.UpsertScoreAsync(applicationId, ScoreDimension.interview, secondScoreRequest, secondTeamMemberId);
        Assert.NotNull(secondResult);
        Assert.True(secondResult.isSuccess);


        var profile = await service.GetApplicationProfileAsync(applicationId);
        Assert.NotNull(profile);
        Assert.NotNull(profile.scores.interview);

  
        Assert.Equal(5, profile.scores.interview.score);
        Assert.Equal("Excellent performance after reconsideration", profile.scores.interview.comment);


        Assert.Equal(secondTeamMemberId, profile.scores.interview.updated_by);


        Assert.Equal("John Boye", profile.scores.interview.author_name);

        Assert.True(profile.scores.interview.last_updated > DateTime.UtcNow.AddSeconds(-5));
    }

    #endregion

    #region Test 4: test duplicate applicaton prevention

    [Fact]
    public async Task CreateApplication_SameEmailSameJob_ReturnsError()
    {
  
        using var context = CreateDbContext();
        var service = new ApplicationService(context);


        var jobId = Guid.NewGuid();
        context.Jobs.Add(new Job
        {
            id = jobId,
            title = "Software Engineer",
            description = "Test job",
            location = "Remote",
            status = Status.open
        });
        await context.SaveChangesAsync();

        var email = "test@example.com";
        var firstRequest = new CreateApplicationRequest
        {
            name = "Test Applicant",
            email = email,
            coverLetter = "First application"
        };

        var firstResult = await service.CreateAsync(jobId, firstRequest);
        Assert.NotNull(firstResult);
        Assert.True(firstResult.isSuccess);
        Assert.NotNull(firstResult.data);

        var secondRequest = new CreateApplicationRequest
        {
            name = "Test Applicant Again",
            email = email,
            coverLetter = "Second application"
        };

        var secondResult = await service.CreateAsync(jobId, secondRequest);
        Assert.NotNull(secondResult);
        Assert.False(secondResult.isSuccess);
        Assert.Equal("An application with the same email already exists for this job.", secondResult.error);
    }

    #endregion
}