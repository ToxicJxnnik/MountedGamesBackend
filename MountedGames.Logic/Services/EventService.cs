using Microsoft.EntityFrameworkCore;
using MountedGames.Logic.Models.Event;

namespace MountedGames.Logic.Services
{
    public class EventService(DbContext context)
    {
        public bool StartEvent(int eventId)
        {
            var ev = context.Set<Event>().FirstOrDefault(e => e.Id == eventId);
            if (ev == null) return false;
            ev.Status = Enums.EventStatus.Ongoing;
            context.SaveChanges();
            return true;
        }
    }
}
