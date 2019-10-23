using System;
using System.Collections.Generic;
using static NeatMusic.MusicEnvironment;
using static NeatMusic.FitnessUtils;

namespace NeatMusic.Fitness
{
    public class FitnessFunctionSameNote : IFitnessFunction
    {
        public static double CalculateRegularity(List<NeatPlayer> players)
        {
            List<int>[] rests;
            double[][] outputs;

            double[][] durations = FitnessUtils.GetRhythmOutput(players, out rests, out outputs);
            double[] inputs = FitnessUtils.GetRhythmInput();
            double[] inputDurations = FitnessUtils.GetRhythmInputDurations();

            double fitness = PERFECT_RHYTHM_SCORE;

            double distanceFromTotal = DistanceFromTotal(durations);
            fitness -= distanceFromTotal;

            int tooFew = TooFew(inputs, durations, rests);
            fitness -= tooFew;

            int tooMany = TooMany(inputs, durations);
            fitness -= tooMany;

            int sameDurations = SameDurationsInput(inputDurations, durations);
            sameDurations += SameDurationsModules(durations);
            fitness -= sameDurations;

            int restCheck = RestCheck(inputs, durations, rests);
            fitness -= restCheck;

            int differentTimes = PlayingAtDifferentTimes(durations, rests, inputs);
            //fitness -= differentTimes;

            return fitness;
        }

        public static double CalculateHarmoniousness(List<NeatPlayer> players)
        {
            var outputs = FitnessUtils.GetPitchOutput(players);
            var inputs = FitnessUtils.GetPitchInput();

            int fitness = PERFECT_PITCH_SCORE;

            int outOfKey = OutOfKey(outputs) * 10;
            fitness -= outOfKey;

            int badPitch = BadPitch(outputs, inputs);
            fitness -= badPitch;

            int sameNote = SameNote(outputs) * 100;
            fitness -= sameNote;

            int idInput = IdentityInput(inputs, outputs) * 15;
            int idOutput = IdentityModules(outputs) * 10;

            fitness -= idOutput;
            fitness -= idInput;

            int tooBigGap = TooBigGap(outputs) * 10;
            fitness -= tooBigGap;

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