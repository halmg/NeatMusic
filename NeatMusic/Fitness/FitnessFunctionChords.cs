using System;
using System.Collections.Generic;
using NeatMusic.Fitness;
using static NeatMusic.MusicEnvironment;
using static NeatMusic.FitnessUtils;

namespace NeatMusic
{
    public class FitnessFunctionChords : IFitnessFunction
    {
        public static double CalculateRegularity(List<NeatPlayer> players)
        {
            List<int>[] rests;
            double[][] outputs;

            double[][] durations = FitnessUtils.GetRhythmOutput(players, out rests, out outputs);
            double[] inputs = FitnessUtils.GetRhythmInput();

            double fitness = PERFECT_RHYTHM_SCORE;

            fitness -= Math.Round(RhythmDifference(inputs, outputs));

            return fitness;
        }

        public static double CalculateHarmoniousness(List<NeatPlayer> players)
        {
            var outputs = FitnessUtils.GetPitchOutput(players);
            var inputs = FitnessUtils.GetPitchInput();

            int fitness = PERFECT_PITCH_SCORE;

            fitness -= OutOfKey(outputs) * 10;

            fitness -= BadPitch(outputs, inputs);

            fitness -= SameNote(outputs) * 50;

            fitness -= IdentityInput(inputs, outputs) * 15;
            fitness -= IdentityModules(outputs) * 10;

            fitness -= TooBigGap(outputs) * 10;

            //fitness -= DissonanceInput(inputs, outputs);
            //fitness -= DissonanceModules(outputs);

            fitness -= Chord(outputs, FitnessUtils.GetChords()) * 100;
            
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