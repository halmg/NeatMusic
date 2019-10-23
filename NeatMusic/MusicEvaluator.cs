using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Core;

namespace NeatMusic
{
    class MusicEvaluator : ICoevolutionPhenomeEvaluator<IBlackBox>
    {
        private ulong _evalCount;

        public ulong EvaluationCount
        {
            get
            {
                return _evalCount;
            }
        }

        public bool StopConditionSatisfied
        {
            get
            {
                return false; //always return false to never stop evolution
            }
        }

        public void Evaluate(IBlackBox phenome1pitch, IBlackBox phenome1rhythm, IBlackBox phenome2pitch, IBlackBox phenome2rhythm, out FitnessInfo fitness1, out FitnessInfo fitness2)
        {
            //produce some music with the two modules, evaluate it and return a value
            throw new NotImplementedException();
        }

        public void Reset()
        {
            //unnecessary for this problem
        }
    }
}
