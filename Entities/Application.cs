using System.ComponentModel.DataAnnotations;

namespace AihrlyApi.Entities
{   

    public enum ApplicationStages
    {
        applied , 
        screening, 
        interview ,
        offer ,
        hired ,
        rejected
    }


    public enum ApplicationNoteType
    {
        general , 
        screening , 
        interview , 
        reference_check , 
        red_flag
    }

    public enum ScoreDimension { culture_fit, interview, assessment }
    public class Application
    {
        
        public Guid id { get; set; }
        public Guid jobId { get; set; }

        public Job job { get; set; }

        [Required]
        public string name { get; set; }
        [Required]
        public string email { get; set; }

        public string? coverLetter { get; set; } = "";
        public ApplicationStages current_stage { get; set; } = ApplicationStages.applied;

        public ICollection<ApplicationNote> notes { get; set; }

        public ICollection<ApplicationStageHistory> stage_history { get; set; }

        public ICollection<ApplicationScore> scores { get; set; } = new List<ApplicationScore>();
    }


    public class ApplicationNote
    {
        public Guid id { get; set;}

        [Required]
        public ApplicationNoteType type {get; set;} 
        [Required]
        public string description {get; set;}
        public Guid applicationId {get; set;}

        public Guid created_by {get; set;}

        public TeamMember teamMember {get; set;}

        [Required]
        public DateTime created_at {get; set;} = DateTime.UtcNow;
    }


    public class ApplicationStageHistory
    {
        public Guid id {get ; set;}
        public Guid applicationId {get; set;}

        [Required]
        public ApplicationStages from_stage {get; set;}
        [Required]
        public ApplicationStages to_stage {get; set;}

        public Guid changed_by {get; set;}

        public TeamMember teamMember {get; set;}
        [Required]
        public DateTime changed_at {get; set;} = DateTime.UtcNow;
        [Required]
        public string comment {get; set;} = "";
    }




    public class ApplicationScore
    {
        public Guid id { get; set; }
        public Guid applicationId { get; set; }
        [Required]
        public ScoreDimension dimension { get; set; }

        [Required]
        [Range(1 , 5)]
        public int score { get; set; } 
        public string? comment { get; set; }
        public Guid updatedBy { get; set; } 

        public TeamMember teamMember { get; set; }
        [Required]
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;
    }
}