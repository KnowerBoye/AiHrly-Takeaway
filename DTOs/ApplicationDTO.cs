using System.ComponentModel.DataAnnotations;
using AihrlyApi.Entities;
using System.Text.Json.Serialization;


namespace AihrlyApi.DTOs
{
public class CreateApplicationRequest
{
    [Required]
    public string name { get; set; }

    [Required]
    [EmailAddress]
    public string email { get; set; }

    public string? coverLetter { get; set; }
}
}


public class ApplicationResponse
{
    public Guid id { get; set; }
    public string name { get; set; }
    public string email { get; set; }

    
    public ApplicationStages current_stage { get; set; }

    public string? coverLetter { get; set; }
}


public class UpdateApplicationStageRequest
    {
        [Required]
        public string stage { get; set; }

        public string comment { get; set; } = "";
    }


public class CreateApplicationNoteRequest
    {

        public string type { get; set; }

        public string description { get; set; }
    }


public class ApplicationNoteResponse
    {
        public Guid id { get; set;}
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ApplicationNoteType type {get; set;} 
        public string description {get; set;}
        public Guid created_by {get; set;}
        public string author_name {get; set;}

    }


public class ScoreDetailDto
{
    public int score { get; set; }
    public string? comment { get; set; }
    public DateTime last_updated { get; set; }
    public Guid updated_by { get; set; }
    public string author_name { get; set; }
}

public class ApplicationScoresDto
{
    public ScoreDetailDto? culture_fit { get; set; }
    public ScoreDetailDto? interview { get; set; }
    public ScoreDetailDto? assessment { get; set; }
}

public class ApplicationStageHistoryDto
{
    public ApplicationStages from { get; set; }
    public ApplicationStages to { get; set; }
    public DateTime changed_at { get; set; }
    public string comment { get; set; }
    public Guid changed_by { get; set; }
    public string author_name { get; set; }
}


public class ApplicationProfileResponse
{
    public Guid id { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public string? coverLetter { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ApplicationStages currentStage { get; set; }

    public ApplicationScoresDto scores { get; set; }

    public List<ApplicationNoteResponse> notes { get; set; }

    public List<ApplicationStageHistoryDto> stageHistory { get; set; }
}



public class UpsertScoreRequest
{
    [Range(1, 5)]
    public int score { get; set; }

    public string? comment { get; set; }
}