using System.ComponentModel.DataAnnotations;

namespace AihrlyApi.Entities
{


    public enum TeamMemberRole{
        recruiter , 
        hiring_manager
    };
    
    public class TeamMember
    {
        public Guid id {get ; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public TeamMemberRole role { get; set; }
    }
}