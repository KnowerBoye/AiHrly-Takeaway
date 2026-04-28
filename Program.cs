using Microsoft.EntityFrameworkCore;
using AihrlyApi.Data;
using AihrlyApi.Services;
using System.Text.Json.Serialization;
using FluentValidation;
using AihrlyApi.Validators;
using Hangfire;
using Hangfire.MemoryStorage;

var builder = WebApplication.CreateBuilder(args);




// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

// builder.Services.ConfigureHttpJsonOptions(options => { options.SerializerOptions.Converters.Add( new JsonStringEnumConverter() ); });

builder.Services.AddDbContext<ApiDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddHangfire(config =>
    config.UseMemoryStorage()); 

builder.Services.AddHangfireServer();



// register validator from assembly 

builder.Services.AddValidatorsFromAssemblyContaining<UpdateApplicationStageRequestValidator>();


// add services
builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<ApplicationService>();
// builder.Services.AddScoped<NotificationService>();

var app = builder.Build();



//just for runnign seed script
if (args.Length > 0 && args[0] == "seed")
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();

    await SeedData.Run(context);

    Console.WriteLine("Seed completed");
    return;
}


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    db.Database.Migrate();
}

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

