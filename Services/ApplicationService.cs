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



    // stage transition validator 

    private static readonly Dictionary<ApplicationStages, ApplicationStages[]> ValidTransitions = new()
    {
        [ApplicationStages.applied]   = [ApplicationStages.screening, ApplicationStages.rejected],
        [ApplicationStages.screening] = [ApplicationStages.interview, ApplicationStages.rejected],
        [ApplicationStages.interview] = [ApplicationStages.offer, ApplicationStages.rejected],
        [ApplicationStages.offer]     = [ApplicationStages.hired, ApplicationStages.rejected],
        [ApplicationStages.hired]     = [],
        [ApplicationStages.rejected]  = [],
    };

    public static bool IsValidTransition(ApplicationStages from, ApplicationStages to) =>
        ValidTransitions.TryGetValue(from, out var targets) && targets.Contains(to);
        

    //application management
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


    
    public async Task<Result> UpdateStageAsync(
    Guid applicationId,
    UpdateApplicationStageRequest request,
    Guid teamMemberId)
    {
        var application = await context.Applications
            .FirstOrDefaultAsync(a => a.id == applicationId);

        if (application is null) return null;

        var current = application.current_stage;
        var target = Enum.Parse<ApplicationStages>(request.stage, ignoreCase: true);

        if (!IsValidTransition(current, target)) return new Result
        {
            isSuccess = false,
            error = $"Invalid stage transition from {current} to {target}."
        };

        application.current_stage = target;

        var history = new ApplicationStageHistory
        {
            id = Guid.NewGuid(),
            applicationId = applicationId,
            from_stage = current,
            to_stage = target,
            changed_by = teamMemberId,
            comment = request.comment 
        };

        context.ApplicationStageHistories.Add(history);

        await context.SaveChangesAsync();

       

        return new Result {
            isSuccess = true
        };
    }



    

    public async Task<ApplicationProfileResponse?> GetApplicationProfileAsync(Guid id)
{
    var application = await context.Applications
        .Where(a => a.id == id)

        .Select(a => new ApplicationProfileResponse
        {
            id = a.id,
            name = a.name,
            email = a.email,
            coverLetter = a.coverLetter,
            currentStage = a.current_stage,

            scores = new ApplicationScoresDto
            {
                culture_fit = a.scores
                    .Where(s => s.dimension == ScoreDimension.culture_fit)
                    .Select(s => new ScoreDetailDto
                    {
                        score = s.score,
                        comment = s.comment,
                        last_updated = s.updatedAt,
                        updated_by = s.updatedBy,
                        author_name = s.teamMember.name
                    })
                    .FirstOrDefault(),

                interview = a.scores
                    .Where(s => s.dimension == ScoreDimension.interview)
                    .Select(s => new ScoreDetailDto
                    {
                        score = s.score,
                        comment = s.comment,
                        last_updated = s.updatedAt,
                        updated_by = s.updatedBy,
                        author_name = s.teamMember.name
                    })
                    .FirstOrDefault(),

                assessment = a.scores
                    .Where(s => s.dimension == ScoreDimension.assessment)
                    .Select(s => new ScoreDetailDto
                    {
                        score = s.score,
                        comment = s.comment,
                        last_updated = s.updatedAt,
                        updated_by = s.updatedBy,
                        author_name = s.teamMember.name
                    })
                    .FirstOrDefault()
            },

            notes = a.notes
                .OrderByDescending(n => n.created_at)
                .Select(n => new ApplicationNoteResponse
                {
                    id = n.id,
                    type = n.type,
                    description = n.description,
                    created_by = n.created_by,
                    author_name = n.teamMember.name
                })
                .ToList(),

            stageHistory = a.stage_history
                .OrderByDescending(h => h.changed_at)
                .Select(h => new ApplicationStageHistoryDto
                {
                    from = h.from_stage,
                    to = h.to_stage,
                    changed_at = h.changed_at,
                    comment = h.comment,
                    changed_by = h.changed_by
                })
                .ToList()
        })
        .FirstOrDefaultAsync();

        if (application is null) return null;
        

        return application;
}








    //note management

    public async Task<Result?> AddNoteAsync(
    Guid applicationId,
    CreateApplicationNoteRequest request,
    Guid teamMemberId)
{
    var application = await context.Applications
        .FirstOrDefaultAsync(a => a.id == applicationId);

    if (application is null) return null;



    var note = new ApplicationNote
    {
        id = Guid.NewGuid(),
        applicationId = applicationId,
        type = Enum.Parse<ApplicationNoteType>(request.type, ignoreCase: true),
        description = request.description,
        created_by = teamMemberId,
        created_at = DateTime.UtcNow
    };

    context.ApplicationNotes.Add(note);

    await context.SaveChangesAsync();

    return new Result
    {
        isSuccess = true
    };
}



    public async Task<PagedResult<ApplicationNoteResponse>?> GetNotesAsync(Guid applicationId)
        {
            var applicationExists = await context.Applications
                .AnyAsync(a => a.id == applicationId);

            if (!applicationExists)   return null;

            var notes = await context.ApplicationNotes
                .Where(n => n.applicationId == applicationId)
                .OrderByDescending(n => n.created_at)
                .Include(n => n.teamMember)
                .Select(n => new ApplicationNoteResponse
                {
                    id = n.id,
                    type = n.type,
                    description = n.description,
                    created_by = n.created_by,
                    author_name = n.teamMember.name
                })
                .ToListAsync();

            return new PagedResult<ApplicationNoteResponse>
            {
                items = notes,
                totalCount = notes.Count
            };
        }



        /// add score for applicaiton 
        /// 
        
        public async Task<Result?> UpsertScoreAsync(
            Guid applicationId,
            ScoreDimension dimension,
            UpsertScoreRequest request,
            Guid teamMemberId)
        {
            var applicationExists = await context.Applications
                .AnyAsync(a => a.id == applicationId);

            if (!applicationExists) return null;

            var existingScore = await context.ApplicationScores
                .FirstOrDefaultAsync(s =>
                    s.applicationId == applicationId &&
                    s.dimension == dimension);

            if (existingScore is null) {
        
                var score = new ApplicationScore
                {
                    id = Guid.NewGuid(),
                    applicationId = applicationId,
                    dimension = dimension,
                    score = request.score,
                    comment = request.comment,
                    updatedBy = teamMemberId,
                
                };

                context.ApplicationScores.Add(score);
            }
            else {

                existingScore.score = request.score;
                existingScore.comment = request.comment;
                existingScore.updatedBy = teamMemberId;
                existingScore.updatedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();

            return new Result
            {
                isSuccess = true
            };
        }

    
    }
}
