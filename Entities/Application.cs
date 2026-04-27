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
    public class Application
    {
        
        public Guid id { get; set; }
        public Guid jobId { get; set; }

        public Job job { get; set; }
        public string name { get; set; }
        public string email { get; set; }

        public string? coverLetter { get; set; } = null;
        public ApplicationStages current_stage { get; set; } = ApplicationStages.applied;

        public ICollection<ApplicationNote> notes { get; set; }

        public ICollection<ApplicationStageHistory> stage_history { get; set; }
    }


    public class ApplicationNote
    {
        public Guid id { get; set;}
        public string type {get; set;} 
        public string description {get; set;}
        public Guid applicationId {get; set;}

        public Guid created_by {get; set;}

        public TeamMember teamMember {get; set;}

        public DateTime created_at {get; set;} 
    }


    public class ApplicationStageHistory
    {
        public Guid id {get ; set;}
        public Guid applicationId {get; set;}
        public ApplicationStages from_stage {get; set;}
        public ApplicationStages to_stage {get; set;}

        public Guid changed_by {get; set;}
        public DateTime changed_at {get; set;}
        public string comment {get; set;}
    }
}