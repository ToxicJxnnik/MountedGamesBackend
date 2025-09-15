using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MountedGames.Logic.Models;
using MountedGames.Logic.Models.Event;
using MountedGames.Logic.Services;

namespace MountedGames.Logic.Hubs
{
    [Authorize] // Requires JWT authentication
    public class EventHub(EventService eventService, RaceStateManager raceStateManager) : Hub
    {
        public async Task<bool> StartRace(int eventId, int raceId)
        {
            if (eventService.StartEvent(eventId))
            {
                await Clients.All.SendAsync("EventStarted", eventId);
                return true;
            }

            raceStateManager.AddRace(eventId, [
                new HorseMan { Color = Color.AliceBlue},
                new HorseMan { Color = Color.Aquamarine},
                new HorseMan { Color = Color.Coral}
                ]);

            return false;
        }

        /*public async Task<bool> EndEvent(int eventId)
        {
            if (eventService.EndEvent(eventId))
            {
                await Clients.All.SendAsync("EventEnded", eventId);
                return true;
            }
            return false;
        }*/

        public async Task UpdateHorseManPosition(int eventId, int horseManId, int position)
        {
            raceStateManager.UpdateHorseManPosition(eventId, horseManId, position);
        }

        // Join a specific event group (e.g., for real-time updates during an event)
        public async Task JoinEventGroup(string eventId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Event_{eventId}");
            await Clients.Group($"Event_{eventId}").SendAsync("UserJoined", Context.User?.Identity?.Name);
        }

        // Leave an event group
        public async Task LeaveEventGroup(string eventId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Event_{eventId}");
            await Clients.Group($"Event_{eventId}").SendAsync("UserLeft", Context.User?.Identity?.Name);
        }

        // Send a message to all users in an event (only organizers and judges)
        [Authorize(Roles = $"{UserRoles.Organizer},{UserRoles.Judge}")]
        public async Task SendEventUpdate(string eventId, string message)
        {
            var userName = Context.User?.Identity?.Name ?? "System";
            await Clients.Group($"Event_{eventId}").SendAsync("EventUpdate", userName, message);
        }

        // Send a message to all connected users (admin only)
        [Authorize(Roles = UserRoles.Admin)]
        public async Task SendGlobalMessage(string message)
        {
            var userName = Context.User?.Identity?.Name ?? "Admin";
            await Clients.All.SendAsync("GlobalMessage", userName, message);
        }

        // Handle connection events
        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name ?? "Anonymous";
            var role = Context.User?.FindFirst("role")?.Value ?? "Unknown";

            Console.WriteLine($"User connected: {userName} (Role: {role})");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userName = Context.User?.Identity?.Name ?? "Anonymous";
            Console.WriteLine($"User disconnected: {userName}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}