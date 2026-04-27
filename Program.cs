using Microsoft.EntityFrameworkCore;
using AihrlyApi.Data;
using AihrlyApi.Services;
using System.Text.Json.Serialization;
using FluentValidation;
using AihrlyApi.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

// builder.Services.ConfigureHttpJsonOptions(options => { options.SerializerOptions.Converters.Add( new JsonStringEnumConverter() ); });

builder.Services.AddDbContext<ApiDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));



// register validator from assembly 

builder.Services.AddValidatorsFromAssemblyContaining<UpdateApplicationStageRequestValidator>();


// add services
builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<ApplicationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseStatusCodePages();



var apiGroup = app.MapGroup("/api");

apiGroup.MapJobEndpoint();
apiGroup.MapApplicationEndpoint();
apiGroup.MapNotesEndpoint();
apiGroup.MapScoresEndpoint();



app.Run();

