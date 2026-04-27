using System.ComponentModel.DataAnnotations;

namespace AihrlyApi.Entities
{
    public enum Status{
        open , 
        closed
    };
    public class Job
    {
        public Guid id {get; set;}
        [Required]
        public string title {get; set;}
        [Required]
        public string description {get; set;}
        [Required]
        public string location {get; set;}
        [Required]
        public Status status {get; set;} = Status.open;

        public ICollection<Application> applications {get; set;}

    }
}