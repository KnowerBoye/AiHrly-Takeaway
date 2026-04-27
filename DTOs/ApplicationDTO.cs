using System.ComponentModel.DataAnnotations;
using AihrlyApi.Entities;

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