namespace AihrlyApi.Entities
{
    public enum Status{
        open , 
        closed
    };
    public class Job
    {
        public Guid id {get; set;}
        public string title {get; set;}
        public string description {get; set;}
        public string location {get; set;}
        public Status status {get; set;}

        public ICollection<Application> applications {get; set;}

    }
}