using Microsoft.EntityFrameworkCore;
using AihrlyApi.Data;
using AihrlyApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

builder.Services.AddDbContext<ApiDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));




// add services
builder.Services.AddScoped<JobService>();

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

