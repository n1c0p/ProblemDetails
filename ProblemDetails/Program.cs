using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// per gestire gli errori di validazione
builder.Services.Configure<ApiBehaviorOptions>( options =>
{

    options.InvalidModelStateResponseFactory = actionContex =>
    {
        var errors = actionContex.ModelState.Where(e => e.Value?.Errors.Any() ?? false).Select(e => new ValidationError(e.Key, e.Value.Errors.First().ErrorMessage));

        var httpContex = actionContex.HttpContext;

        var statusCode = StatusCodes.Status422UnprocessableEntity;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Type = $"https://httpstatuses.com/{statusCode}",
            Instance = httpContex.Request.Path,
            Title = "Errori di validazione"
        };

        problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? httpContex.TraceIdentifier);
        problemDetails.Extensions.Add("errors", errors);

        var result = new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };

        return result;
    };

});


// centralizzazione errori con la Problem detail
builder.Services.AddProblemDetails(options =>
{
    options.Map<HttpRequestException>(ex => new StatusCodeProblemDetails(StatusCodes.Status503ServiceUnavailable));

    options.Map<ApplicationException>(ex => new StatusCodeProblemDetails(StatusCodes.Status400BadRequest));

    options.Map<Exception>(ex =>
    {
        var error = new StatusCodeProblemDetails(StatusCodes.Status500InternalServerError)
        {
            Title = "Errore interno",
            Detail = ex.Message
        };

        return error;
    });
}); //.AddProblemDetailsConventions(); // -> questa serve se voglio centralizzare gli errori di validazione con la problemDetails, sostituisce quello di sopra


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseProblemDetails();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseAuthorization();

app.MapControllers();

app.Run();

record ValidationError(string Name, string Message);