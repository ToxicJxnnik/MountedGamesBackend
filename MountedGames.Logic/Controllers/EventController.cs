using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MountedGames.Logic.Data;
using MountedGames.Logic.Models;
using MountedGames.Logic.Models.Event;

namespace MountedGames.Logic.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController(MountedGamesDbContext db) : ControllerBase
{
    // Alle Turniere abrufen
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await db.Events.OrderBy(e => e.StartDate).ToListAsync();
        return Ok(events);
    }

    // Ein Turnier abrufen
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var evnt = await db.Events.FindAsync(id);
        return evnt is null ? NotFound() : Ok(evnt);
    }

    // Neues Turnier anlegen
    [Authorize(Roles = UserRoles.Organizer)]
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromBody] Event input)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
            return BadRequest("Title is required.");
        if (input.EndDate < input.StartDate)
            return BadRequest("EndDate must be after StartDate.");

        db.Events.Add(input);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
    }
}
