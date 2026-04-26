namespace AihrlyApi.Entities
{


    public enum TeamMemberRole{
        recruiter , 
        hiring_manager
    };
    
    public class TeamMember
    {
        public Guid id {get ; set; }
        public string name { get; set; }
        public string email { get; set; }
        public TeamMemberRole role { get; set; }
    }
}