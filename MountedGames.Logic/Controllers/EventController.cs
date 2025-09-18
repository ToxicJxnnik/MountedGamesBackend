using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MountedGames.Logic.Data;
using MountedGames.Logic.Models;
using MountedGames.Logic.Models.Event;
using System.Security.Claims;

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

    // Turnieranmeldung einreichen
    [HttpPost("{eventId:int}/register")]
    public async Task<IActionResult> Register(int eventId, [FromBody] EventRegistrationRequest request)
    {
        try
        {
            // Validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if event exists
            var targetEvent = await db.Events.FindAsync(eventId);
            if (targetEvent == null)
            {
                return NotFound($"Event with ID {eventId} not found");
            }

            // Get current user from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("Invalid user authentication");
            }

            // Get user details
            var user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            // Check if user already registered for this event
            var existingRegistration = await db.Set<EventRegistration>()
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

            if (existingRegistration != null)
            {
                return BadRequest("You are already registered for this event");
            }

            // Create registration
            var registration = new EventRegistration
            {
                EventId = eventId,
                UserId = userId,
                RiderName = $"{user.FirstName} {user.LastName}",
                BirthDate = user.DateOfBirth,
                AgeCategory = request.AgeCategory,
                HorseName = request.HorseName,
                StallSpaces = request.StallSpaces,
                CampingSpaces = request.CampingSpaces,
                ChargingSpaces = request.ChargingSpaces,
                Comments = request.Comments,
                RegistrationDate = DateTime.UtcNow
            };

            db.Set<EventRegistration>().Add(registration);
            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration successful",
                registrationId = registration.Id,
                eventTitle = targetEvent.Title,
                riderName = registration.RiderName
            });
        }
        catch (Exception ex)
        {
            // Log the error in production
            return StatusCode(500, new
            {
                error = "An error occurred while processing your registration",
                details = ex.Message
            });
        }
    }

    // Anmeldungen für ein Turnier abrufen (nur für Organisatoren)
    [Authorize(Roles = UserRoles.Organizer)]
    [HttpGet("{eventId:int}/registrations")]
    public async Task<IActionResult> GetEventRegistrations(int eventId)
    {
        var registrations = await db.Set<EventRegistration>()
            .Include(r => r.User)
            .Include(r => r.Event)
            .Where(r => r.EventId == eventId)
            .OrderBy(r => r.RegistrationDate)
            .Select(r => new
            {
                r.Id,
                r.RiderName,
                r.BirthDate,
                r.AgeCategory,
                r.HorseName,
                r.StallSpaces,
                r.CampingSpaces,
                r.ChargingSpaces,
                r.Comments,
                r.RegistrationDate,
                UserEmail = r.User!.Email,
                UserClub = r.User.Club
            })
            .ToListAsync();

        return Ok(registrations);
    }

    // Eigene Anmeldungen abrufen
    [Authorize]
    [HttpGet("my-registrations")]
    public async Task<IActionResult> GetMyRegistrations()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return BadRequest("Invalid user authentication");
        }

        var registrations = await db.Set<EventRegistration>()
            .Include(r => r.Event)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.RegistrationDate)
            .Select(r => new
            {
                r.Id,
                EventTitle = r.Event!.Title,
                EventLocation = r.Event.Location,
                EventStartDate = r.Event.StartDate,
                EventEndDate = r.Event.EndDate,
                r.RiderName,
                r.AgeCategory,
                r.HorseName,
                r.StallSpaces,
                r.CampingSpaces,
                r.ChargingSpaces,
                r.Comments,
                r.RegistrationDate
            })
            .ToListAsync();

        return Ok(registrations);
    }
}