using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace Minesweeper.WebApi.Controllers;

[ApiController]
[Route("api/users")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private static readonly ConcurrentBag<string> _users = new();

    [HttpGet]
    public ActionResult<List<string>> GetUsers()
    {
        return Ok(_users.ToArray());
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] UserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { error = "Name required" });
        _users.Add(request.Name);
        return Ok();
    }

    public class UserRequest
    {
        public string Name { get; set; } = "";
    }
}
