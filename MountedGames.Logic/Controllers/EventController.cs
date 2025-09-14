using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MountedGames.Logic.Data;
using MountedGames.Logic.Models;

namespace MountedGames.Logic.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly MountedGamesDbContext _db;

    public EventController(MountedGamesDbContext db)
    {
        _db = db;
    }

    // Alle Turniere abrufen
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await _db.Events.OrderBy(e => e.StartDate).ToListAsync();
        return Ok(events);
    }

    // Ein Turnier abrufen
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var evnt = await _db.Events.FindAsync(id);
        return evnt is null ? NotFound() : Ok(evnt);
    }

    // Neues Turnier anlegen
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Event input)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
            return BadRequest("Title is required.");
        if (input.EndDate < input.StartDate)
            return BadRequest("EndDate must be after StartDate.");

        _db.Events.Add(input);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
    }
}
