using System.ComponentModel.DataAnnotations;

namespace AihrlyApi.Entities
{
    

    public class Notification
    {
        public Guid id {get; set;}

        public Guid application_id {get; set;}

        [Required]
        public string type {get; set;}

        [Required]
        public DateTime sent_at = DateTime.UtcNow;


    }
}