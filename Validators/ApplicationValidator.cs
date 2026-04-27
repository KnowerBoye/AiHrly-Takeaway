using AihrlyApi.DTOs;
using FluentValidation;
using AihrlyApi.Entities;

namespace AihrlyApi.Validators
{
    


public class UpdateApplicationStageRequestValidator : AbstractValidator<UpdateApplicationStageRequest>
{
    public UpdateApplicationStageRequestValidator()
    {
        RuleFor(x => x.stage)
            .NotEmpty()
            .WithMessage("Stage is required.")
            .IsEnumName(typeof(ApplicationStages), caseSensitive: false)
            .WithMessage(x => $"'{x.stage}' is not a valid stage. " +
                             $"Allowed values are: {string.Join(", ", Enum.GetNames<ApplicationStages>())}.");

    }
}


public class AddApplicationNoteRequestValidator : AbstractValidator<CreateApplicationNoteRequest>
{
    public AddApplicationNoteRequestValidator()
    {
        RuleFor(x => x.description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(x => x.type)
            .NotEmpty()
            .WithMessage("Type is required.")
            .IsEnumName(typeof(ApplicationNoteType), caseSensitive: false)
            .WithMessage(x => $"'{x.type}' is not a valid note type. " +
                             $"Allowed values are: {string.Join(", ", Enum.GetNames<ApplicationNoteType>())}.");
            
    }
}

}