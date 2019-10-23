using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Core;

namespace NeatMusic
{
    class PitchEvaluator : ICoevolutionPhenomeListEvaluator<IBlackBox>
    {
        private ulong _evalCount;
        private bool _stopConditionSatisfied;

        public ulong EvaluationCount => _evalCount;
        public bool StopConditionSatisfied => _stopConditionSatisfied;

        /// <summary>
        /// Evaluates a list of phenomes and returns a list of their fitness infos.
        /// </summary>
        /// <param name="phenomes">The phenomes we want to evaluate.</param>
        /// <param name="fitness">The list of acumulated fitness info.</param>
        public void Evaluate(List<IBlackBox> phenomes, out FitnessInfo fitness)
        {
            _evalCount += 1;
            
            // Create the players from the phenomes.
            List<NeatPlayer> players = phenomes.Select(blackBox => new NeatPlayer(blackBox)).ToList();

            // Calculate the total harmoniousness for all the players. 
            double harmoniosness = MusicEnvironment.FITNESS_FUNCTION.CalculateHarmoniousness(players);

            fitness = new FitnessInfo(harmoniosness, 0);


            // Change this if stop condition is required. 
            _stopConditionSatisfied = false;
        }

        public void Reset()
        {
            //unnecessary for this problem
        }
    }
}
