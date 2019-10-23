using System.Collections.Generic;

namespace NeatMusic.Fitness
{
    public interface IFitnessFunction
    {
        double CalculateRegularity(List<NeatPlayer> players);
        double CalculateHarmoniousness(List<NeatPlayer> players);

    }
}