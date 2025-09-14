namespace MountedGames.Logic.Models.Event
{
    public class RaceState
    {
        HorseMan[] HorseMen { get; set; }
        public RaceState(HorseMan[] horseMen)
        {
            for (int i = 0; i < horseMen.Length; i++)
            {
                HorseMen[i] = horseMen[i];
                horseMen[i].Position = i;
            }
        }

        public void UpdateHorseMan(HorseMan horseMan, int position)
        {
            if (position < 0 || position >= HorseMen.Length)
                throw new ArgumentOutOfRangeException(nameof(position), "Index is out of range.");

            (HorseMen[horseMan.Position], HorseMen[position]) = (HorseMen[position], HorseMen[horseMan.Position]);
        }
    }
}
