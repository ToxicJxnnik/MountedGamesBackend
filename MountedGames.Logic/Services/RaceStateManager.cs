using System.Collections.Concurrent;
using MountedGames.Logic.Models.Event;

namespace MountedGames.Logic.Services
{
    public class RaceStateManager
    {
        ConcurrentDictionary<int, RaceState> races = [];

        public void AddRace(int eventId, HorseMan[] horseMen)
        {
            races[eventId] = new RaceState(horseMen);
        }

        public void UpdateHorseManPosition(int eventId, int horseManId, int position)
        {
            races[eventId].UpdateHorseMan(new HorseMan(), position);
        }
    }
}
