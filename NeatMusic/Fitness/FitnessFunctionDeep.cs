using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NeatMusic.MusicEnvironment;
using static NeatMusic.FitnessUtils;

namespace NeatMusic.Fitness
{
    public class FitnessFunctionDeep : IFitnessFunction
    {

        public static double CalculateRegularity(List<NeatPlayer> players)
        {
            List<int>[] rests;
            double[][] outputs;

            double[][] durations = FitnessUtils.GetRhythmOutput(players, out rests, out outputs);
            double[] inputs = FitnessUtils.GetRhythmInput();
            double[] inputDurations = FitnessUtils.GetRhythmInputDurations();

            double fitness = PERFECT_RHYTHM_SCORE;
            
            int tooFew = TooFew(inputDurations, durations, rests) * 10;
            fitness -= tooFew;

            int tooMany = TooMany(inputDurations, durations);
            fitness -= tooMany;

            int sameDurationsInput = SameDurationsInput(inputDurations, durations) * 10;
            int sameDurationsOutputs = SameDurationsModules(durations) * 5;
            fitness -= sameDurationsInput;
            fitness -= sameDurationsOutputs;

            int restCheck = RestCheck(inputs, durations, rests);
            fitness -= restCheck;

            //makes sure an appropriate number of modules are playing at the same time
            int differentTimes = PlayingAtDifferentTimes(durations, rests, inputs) * 5;
            fitness -= differentTimes;

            int standardDeviation = StandardDeviation(outputs[0]);
            fitness -= standardDeviation;

            int onBeat = OnBeat(durations) * 10;
            fitness -= onBeat;


            return fitness;
        }

        public static double CalculateHarmoniousness(List<NeatPlayer> players)
        {
            var outputs = FitnessUtils.GetPitchOutput(players);
            var inputs = FitnessUtils.GetPitchInput();
            
            int fitness = PERFECT_PITCH_SCORE;

            int outOfKey = OutOfKey(outputs) * 20;
            fitness -= outOfKey;

            int badPitch = BadPitch(outputs, inputs);
            fitness -= badPitch;

            int sameNote = SameNote(outputs) * 25;
            fitness -= sameNote;
            
            int idInput = IdentityInput(inputs, outputs) * 10;
            int idOutput = IdentityModules(outputs) * 5;

            fitness -= idOutput;
            fitness -= idInput;

            int tooBigGap = TooBigGap(outputs) * 10;
            fitness -= tooBigGap;

            int dissonanceInput = DissonanceInput(inputs, outputs) * 5;
            int dissonanceOutput = DissonanceModules(outputs) * 5;
            
            fitness -= dissonanceInput;
            fitness -= dissonanceOutput;

            int chord = Chord(outputs, FitnessUtils.GetChords()) * 10;
            fitness -= chord;

            int belowInput = BelowInput(inputs, outputs) * 5;
            fitness -= belowInput;
            
            return fitness;
        }

        double IFitnessFunction.CalculateHarmoniousness(List<NeatPlayer> players)
        {
            return CalculateHarmoniousness(players);
        }

        double IFitnessFunction.CalculateRegularity(List<NeatPlayer> players)
        {
            return CalculateRegularity(players);
        }


    }
}
