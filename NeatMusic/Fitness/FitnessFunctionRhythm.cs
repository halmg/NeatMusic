using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NeatMusic.FitnessUtils;
using static NeatMusic.MusicEnvironment;

namespace NeatMusic.Fitness
{
    public class FitnessFunctionRhythm : IFitnessFunction
    {

        public static double CalculateRegularity(List<NeatPlayer> players)
        {
            List<int>[] rests;
            double[][] outputs;

            double[][] durations = FitnessUtils.GetRhythmOutput(players, out rests, out outputs);
            double[] inputs = FitnessUtils.GetRhythmInput();
            double[] inputDurations = FitnessUtils.GetRhythmInputDurations();

            double fitness = PERFECT_RHYTHM_SCORE;

            //double distanceFromTotal = DistanceFromTotal(durations);
            //fitness -= distanceFromTotal;
            
            int tooFew = TooFew(inputDurations, durations, rests) * 10;
            fitness -= tooFew;

            int tooMany = TooMany(inputDurations, durations);
            fitness -= tooMany;

            int sameDurationsInput = SameDurationsInput(inputDurations, durations) * 20;
            int sameDurationsOutputs = SameDurationsModules(durations) * 10;
            fitness -= sameDurationsInput;
            fitness -= sameDurationsOutputs;

            int restCheck = RestCheck(inputs, durations, rests);
            fitness -= restCheck;
            
            //makes sure an appropriate number of modules are playing at the same time
            //int differentTimes = PlayingAtDifferentTimes(durations, rests, inputs) / 10;
            //fitness -= differentTimes;

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

            int outOfKey = OutOfKey(outputs) * 10;
            fitness -= outOfKey;

            int badPitch = BadPitch(outputs, inputs) * 5;
            fitness -= badPitch;

            int sameNote = SameNote(outputs) * 25;
            fitness -= sameNote;

            //
            //discourage the modules from playing the same things 
            int idInput = IdentityInput(inputs, outputs) * 15;
            int idOutput = IdentityModules(outputs) * 10;

            fitness -= idOutput;
            fitness -= idInput;


            //makes modules play in a hierarchy
            //NOTE: Probably broken - module position in the fitness function isn't fixed, which this function assumes
            int lower = LowerThanPrevious(outputs);
            fitness -= lower;

            //make sure the gap to the previous note is less than 7 semitones
            int tooBigGap = TooBigGap(outputs) * 10;
            fitness -= tooBigGap;

            int dissonanceInput = DissonanceInput(inputs, outputs) * 5;
            int dissonanceOutput = DissonanceModules(outputs) * 5;

            fitness -= dissonanceInput;
            fitness -= dissonanceOutput;

            int chord = Chord(outputs, FitnessUtils.GetChords()) * 5;
            fitness -= chord;

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
