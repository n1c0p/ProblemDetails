using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProblemDetails.Entities;

namespace ProblemDetail.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController : ControllerBase
{
   
    private readonly ILogger<PeopleController> _logger;

    public PeopleController(ILogger<PeopleController> logger)
    {
        _logger = logger;
    }

    [HttpGet("notfound")]
    public IActionResult Get() => NoContent();

    [HttpPost("person")]
    public IActionResult Post(Person person) => NoContent();

    [HttpGet("exception")]
    public IActionResult GetEXception() => throw new Exception("Errore frullino 42");

    [HttpGet("httpexception")]
    public IActionResult GetHttpEXception() => throw new HttpRequestException("errore api");

}