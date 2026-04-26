using System.ComponentModel.DataAnnotations;
using AihrlyApi.Entities;

namespace AihrlyApi.DTOs

{
    

public class CreateJobRequest
{
    [Required]
    [MinLength(3)]
    public string title {get; set;}
    public string description {get; set;}
    [Required]
    public string location {get; set;}
}


    public class JobResponse
    {
        public Guid id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string location { get; set; }

        public Status status { get; set; }
      
    }
}
