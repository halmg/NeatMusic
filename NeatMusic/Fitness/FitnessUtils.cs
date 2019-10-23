using System;
using System.Collections.Generic;
using System.Linq;
using Keyboard; 

namespace NeatMusic
{
    public class FitnessUtils
    {

        private static double[] currentDurationPhrase = MusicLibrary.CurrentSongTrainingDuration(MusicEnvironment.NOTES_FOR_TRAINING);
        private static int[] currentPitchPhrase = MusicLibrary.CurrentSongTrainingPitch(MusicEnvironment.NOTES_FOR_TRAINING);
        private static Music.Key currentKey = MusicLibrary.CurrentSongKey();
        private static double[] durationInputs = MusicEnvironment.createDurationInputs(currentDurationPhrase);
        private static int[] pitchInputs = MusicEnvironment.createPitchInputs(currentPitchPhrase, currentDurationPhrase);
        private static Music.Chord[] Chords = MusicLibrary.CurrentSongChords();
        private static double[] inputDurations = null;

        #region RHYTHM GENERATING

        public static double[][] GetRhythmOutput(List<NeatPlayer> players, out List<int>[] rests, out double[][] outputs)
        {

            //convert inputs to correct values
            double[] inputs = durationInputs;
            //calculate the outputs, inputs are conbined in a cascading manner
            outputs = new double[players.Count][];

            for (int i = 0; i < outputs.Length; i++)
            {
                outputs[i] = new double[inputs.Length];
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                for (int p = 0; p < players.Count; p++)
                {

                    var brainInput = MusicEnvironment.getBrainInputDelayed(outputs, inputs, p, players.Count, i);

                    outputs[p][i] = players[p].calculateOutput(brainInput);

                }
            }

            double[][] durations = new double[outputs.Length][];

            rests = new List<int>[outputs.Length];

            for (int i = 0; i < outputs.Length; i++)
            {
                durations[i] = MusicEnvironment.OutputsToDurations(outputs[i], out rests[i]);
            }

            return durations;
        }

        public static double[] GetRhythmInput()
        {
            return durationInputs;
        }

        public static double[] GetRhythmInputDurations()
        {
            return currentDurationPhrase;
        }

        #endregion

        #region PITCH GENERATING

        public static int[][] GetPitchOutput(List<NeatPlayer> players)
        {
            //convert inputs to correct values
            int[] inputs = currentPitchPhrase;

            //inputs = currentKey.Pitches.Select(p => p.Value).ToArray();


            int[][] outputs = new int[players.Count][];

            for (int i = 0; i < outputs.Length; i++)
            {
                outputs[i] = new int[inputs.Length];
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                for (int p = 0; p < players.Count; p++)
                {
                    var brainInput = MusicEnvironment.getBrainInputDelayed(outputs, inputs, p, players.Count, i);
                    
                    outputs[p][i] = MusicEnvironment.convertToMidiClass(players[p].calculateOutput(brainInput));
                    
                }
            }

            return outputs;
        }

        public static int[] GetPitchInput()
        {
            return currentPitchPhrase;
        }

        public static Music.Chord[] GetChords()
        {
            return Chords;
        }

        #endregion

        #region RHYTHM HELPER FUNCTIONS

        /*
        * RHYTHM (Regularity) HELPER FUNCTIONS
        */

        public static double RhythmDifference(double[] inputs, double[][] outputs)
        {
            double difference = 0;
            var o = outputs[0];
            for (int i = 0; i < inputs.Length; i++)
            {
                //foreach (double[] o in outputs)
                //{
                difference += Math.Abs(inputs[i] - o[i]);
                //}
            }
            return difference;
        }

        public static int SameDurationsInput(double[] inputDurations, double[][] durations)
        {
            //discourage creating same sequences
            int sameDurations = 0;

            for (int i = 0; i < durations[0].Length && i < inputDurations.Length; i++)
            {

                if (durations[0][i].Equals(inputDurations[i]))
                    sameDurations += 1;
            }

            return sameDurations;
        }

        public static int SameDurationsModules(double[][] durations)
        {
            //discourage creating same sequences
            int sameDurations = 0;
            for (int i = 0; i < durations[0].Length; i++)
            {
                for (int d = 1; d < durations.Length; d++)
                {

                    if (durations[d].Length <= i)
                        break;
                    if (durations[d - 1][i].Equals(durations[d][i]))
                        sameDurations++;
                }
            }

            return sameDurations;
        }

        public static int TooFew(double[] inputs, double[][] durations, List<int>[] rests)
        {
            int tooFew = 0;

            int limit = (inputs.Length/4);
            
            if (durations[0].Length < limit)
                tooFew += 5 * (limit - durations[0].Length);
            return tooFew;
        }

        public static int TooMany(double[] inputs, double[][] durations)
        {
            int tooMany = 0;

            int limit = ((inputs.Count()/4)*3);

            if (durations[0].Count() > limit)
                tooMany += durations[0].Length - limit;
            return tooMany;
        }

        public static int RestCheck(double[] inputs, double[][] durations, List<int>[] rests)
        {


            int result = 0;

            double restDurations = 0.0;
            foreach (int rest in rests[0])
            {
                restDurations += durations[0][rest];
            }

            double inputDuration = MusicEnvironment.ticksToDuration(inputs.Length);

            if (restDurations > 2 * inputDuration * (1.0 / MusicEnvironment.MODULE_COUNT + 0.005))
                result += (int)restDurations * 20;
            if (rests[0].Count < 5)
                result += (5 - rests[0].Count) * 10;

            return result;
            
        }

        public static bool[][] RestsInTicks(double[][] durations, List<int>[] rests, int length)
        {
            bool[][] isRest = new bool[durations.Length][];

            for (int k = 0; k < durations.Length; k++)
            {
                isRest[k] = new bool[length];

                var restLocation = new List<int>();

                foreach (int rest in rests[k])
                {
                    double location = durations[k].TakeWhile((d, i) => i < rest).Sum();
                    restLocation.Add((int) MusicEnvironment.durationToTicks(location));
                }

                for (int i = 0; i < restLocation.Count; i++)
                {
                    int loc = restLocation[i];
                    int dur = (int) MusicEnvironment.durationToTicks(durations[k][rests[k][i]]);
                    for (int j = loc; j < loc + dur; j++)
                    {
                        isRest[k][j] = true;
                    }
                }
            }

            return isRest;
        }

        public static int PlayingAtDifferentTimes(double[][] durations, List<int>[] rests, double[] inputs)
        {
            
            bool[][] isRest = RestsInTicks(durations, rests, inputs.Length);


            int differentTimes = 0;

            for (int i = 0; i < isRest[0].Length; i++)
            {
                int playersAtTick = MusicEnvironment.MODULE_COUNT;

                for (int k = 0; k < isRest.Length; k++)
                {
                    if (isRest[k][i])
                    {
                        playersAtTick--;
                    }
                }

                if (playersAtTick != MusicEnvironment.MODULE_COUNT / 2)
                {

                    differentTimes += Math.Abs(MusicEnvironment.MODULE_COUNT / 2 - playersAtTick);
                }
            }

            return differentTimes;
        }

        public static double DistanceFromTotal(double[][] durations) //double[] durations1, double[] durations2)
        {
            return Math.Abs(currentDurationPhrase.Sum() - durations[0].Sum());
        }

        public static int StandardDeviation(double[] outputs)
        {
            double standardDeviation = SmoothedZSmoothening.StandardDeviation(outputs);

            if (standardDeviation < 0.2) return 100;

            return 0;
        }

        public static int TooShort(double[][] durations)
        {
            int tooShort = 0;

            foreach (double duration in durations[0])
            {
                if (duration < 0.125)
                    tooShort += 1;
            }

            return tooShort;
        }

        public static int TooLong(double[][] durations)
        {
            int tooLong = 0;

            foreach (double duration in durations[0])
            {
                if (duration > 4.0)
                    tooLong += 1;
            }

            return tooLong;
        }

        public static int OnBeat(double[][] durations)
        {
            int onBeat = 0;

            double total = 0.0;

            foreach (double d in durations[0])
            {
                total += d;
                if (Math.Abs(total % 0.0625) > 0.000000001)
                {
                    //The duration is not on the sixteenth beat
                    onBeat += 1;
                }
            }

            return onBeat;
        }

        #endregion rhythm

        #region PITCH HELPER FUNCTIONS

        /*
        * PITCH (Harmoniousness) HELPER FUNCTIONS
        */

        public static int LowerThanPrevious(int[][] outputs)
        {
            int lowerThanPrevious = 0;

            for (int i = 0; i < outputs[0].Length; i++)
            {
                for (int k = 1; k < outputs.Length; k++)
                {
                    if (outputs[k]
                        [i] >= outputs[k - 1][i])
                        lowerThanPrevious++;
                }
            }

            return lowerThanPrevious;
        }

        public static int TooBigGap(int[][] outputs)
        {
            int tooBigGap = 0;

            foreach (int[] output in outputs)
            {
                for (int i = 0; i < output.Length - 1; i++)
                {
                    if (output[i] - output[i + 1] > 7 || output[i] - output[i + 1] < -7)
                        tooBigGap++;
                }
            }

            return tooBigGap;
        }

        public static int PitchDifference(int[] inputs, int[][] outputs)
        {
            //mimicking of the input

            int difference = 0;
            var o = outputs[0];

            for (int i = 0; i < inputs.Length; i++)
            {
                difference += Math.Abs(inputs[i] % 12 - o[i] % 12);
            }

            return difference;
        }

        public static int OutOfKey(int[][] outputs)
        {
            int outOfkey = 0;
            foreach (var output in outputs)
            {
                for (int i = 0; i < output.Length; i++)
                {
                    if (!currentKey.Contains(output[i]))
                        outOfkey += 1;
                }
            }
            return outOfkey;
        }

        public static int SameNote(int[][] outputs)
        {
            //discourage playing always the same note
            int sameNote = 0;
            
            for (int i = 0; i < outputs[0].Length - 1; i++)
            {
                if (outputs[0][i] % 12 == outputs[0][i + 1] % 12)
                {
                    sameNote++;
                }
            }

            return sameNote;
        }
        
        public static int BadPitch(int[][] outputs, int[] inputs)
        {
            int badPitch = 0;
            int[] output = outputs[0];

            List<Music.Chord> CMajor = new List<Music.Chord>() { Music.Chord.C, Music.Chord.Dm, Music.Chord.Em, Music.Chord.F, Music.Chord.G, Music.Chord.Am, Music.Chord.Bm };

            for (int i = 0; i < output.Length; i++)
            {
                Music.Pitch inputPitch = Music.Pitch.OfInt(inputs[i]);
                if (inputPitch == Music.Pitch.rest) continue;

                Music.Chord currentChord = CMajor.Find(c => c.First.Equals(inputPitch));
                if (currentChord == null) currentChord = CMajor[0];

                Music.Pitch currentPitch = Music.Pitch.OfInt(output[i]);
                if (currentPitch == Music.Pitch.rest) continue;
                
                if (!(currentPitch.Equals(currentChord.Third) || currentPitch.Equals(currentChord.Fifth)))
                    badPitch += 1;
             }
            
             return badPitch;
        }

        public static int IdentityInput(int[] inputs, int[][] outputs) //int[] nextNote, int[] outputs1, int[] outputs2)
        {
            //discourage the modules from playing the same things 
            int identity = 0;
            
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] % 12 == outputs[0][i] % 12)
                    identity += 1;
            }

            return identity;
        }

        public static int IdentityModules(int[][] outputs) //int[] nextNote, int[] outputs1, int[] outputs2)
        {
            //discourage the modules from playing the same things 

            int identity = 0;

            var child = outputs[0];

            for (int i = 1; i < outputs.Length; i++)
            {
                for (int j = 0; j < outputs[0].Length; j++)
                {
                    if (child[j].Equals(outputs[i][j]))
                        identity += 1;
                }
            }

            return identity;
        }

        public static int BelowInput(int[] inputs, int[][] outputs)
        {
            int belowInput = 0;

            int[] output = outputs[0];

            for (int i = 0; i < output.Length; i++)
            {
                int inp = inputs[i] + 12;
                int o = output[i];

                if (o > inp)
                    belowInput++;
            }

            return belowInput;
        }

        public static int DissonanceInput(int[] inputs, int[][] outputs)
        {
            int dissonance = 0;

            foreach (var output in outputs)
            {
                dissonance += IterateDissonance(inputs, output);
            }
            return dissonance;
        }

        public static int DissonanceModules(int[][] outputs)
        {
            int dissonance = 0;

            foreach (var output in outputs)
            {
                foreach (var other in outputs)
                {
                    if (other == output)
                        continue;
                    dissonance += IterateDissonance(other, output);
                }
            }
            return dissonance;
        }

        public static int IterateDissonance(int[] input1, int[] input2)
        {
            int dissonance = 0;
            for (int i = 0; i < input1.Length; i++)
            {
                var val1 = input1[i] % 12;
                var val2 = input2[i];

                if (val1 == (val2 - 2) % 12 || val1 == (val2 - 1) % 12 || val1 == (val2 + 1) % 12 || val1 == (val2 + 2) % 12)
                    dissonance += 1;
            }
            return dissonance;
        }
        
        public static int Chord(int[][] outputs, Music.Chord[] chords)
        {
            int ch = 0;
            for (int i = 0; i < outputs[0].Length; i++)
            {
                if (!chords[i].Contains(outputs[0][i]))
                    ch += 1;
            }
            return ch;
        }
        #endregion 
    }
}