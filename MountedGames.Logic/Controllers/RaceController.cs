using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MountedGames.Logic.Models.Event;
using MountedGames.Logic.Services;

namespace MountedGames.Logic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaceController(RaceStateManager raceStateManager) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        [Route("create")]
        public ActionResult Create(int eventId)
        {
            raceStateManager.AddRace(eventId, [
                new HorseMan(),
                new HorseMan(),
                new HorseMan()
            ]);

            return Ok();
        }
    }
}
