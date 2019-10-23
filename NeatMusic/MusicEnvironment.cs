using Keyboard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeatMusic.Fitness;

namespace NeatMusic
{
    public class MusicEnvironment
    {
        private static double[] currentDurationPhrase = MusicLibrary.CurrentSongTrainingDuration(NOTES_FOR_TRAINING);
        private static int[] currentPitchPhrase = MusicLibrary.CurrentSongTrainingPitch(NOTES_FOR_TRAINING);

        // general program parameters
        public const int MODULE_COUNT = 3;
        public const double TICKS_PER_QUARTER = 8;
        public const double QUARTERS_PER_BAR = 1;
        public const int NOTES_FOR_TRAINING = 30;
        public const int AVAILABLE_PITCHES = 24;
        public const int PERFECT_PITCH_SCORE = 40000;
        public const int PERFECT_RHYTHM_SCORE = 20000;
        public const int SETS_OF_PARASITES = 3;
        

        // parameters for smoothening algorithm
        public const int LAG = 5;
        public const double THRESHOLD = 1;
        public const double INFLUENCE = 0.5;

        // The fitness function to use for evaluating genomes
        public static IFitnessFunction FITNESS_FUNCTION = new FitnessFunctionChords();
        public static bool RESTS = false;

        public static double[] createDurationInputs(double[] currentPhrase)
        {
            //24 ticks per quarter note, ergo 24*4 ticks per measure
            List<Double> list = new List<Double>();
            foreach (double v in currentPhrase)
            {
                int ticksForValue = (int)(v * TICKS_PER_QUARTER * QUARTERS_PER_BAR);
                double decay = 1d / (ticksForValue - 1);
                double currentValue = 1;
                for (int i = 0; i < ticksForValue; i++)
                {
                    list.Add(currentValue);
                    currentValue -= decay;
                }
            }
            return list.ToArray();
        }

        public static int[] createPitchInputs(int[] currentPitchPhrase, double[] currentDurationPhrase)
        {
            //24 ticks per quarter note, ergo 24*4 ticks per measure
            List<int> list = new List<int>();
            int index = 0;
            foreach (double v in currentDurationPhrase)
            {
                int ticksForValue = (int)(v * TICKS_PER_QUARTER * QUARTERS_PER_BAR);
                for (int i = 0; i < ticksForValue; i++)
                {
                    list.Add(currentPitchPhrase[index]);
                }
                index++;
            }
            return list.ToArray();
        }

        public static int convertToPitchClass(double v)
        {
            double unit = 1d / 128d;
            int midiValue = (int)(v / unit);
            //return the pitch class
            return midiValue % AVAILABLE_PITCHES;
        }

        public static int convertToMidiClass(double v)
        {
            double unit = 1d / AVAILABLE_PITCHES;
            int midiValue = (int)(v / unit);
            //return the pitch class
            return midiValue;
        }

        //durations can be 0.25, 0.5, 0.75, 1
        public static double convertToDuration(double v)
        {
            if (v < 0.125)
                return 0.125;
            if (v < 0.25)
                return 0.25;
            if (v < 0.5)
                return 0.5;
            if (v < 0.75)
                return 0.75;
            return 1;
        }


        public static double[] getBrainInputOnlyInput<T>(T[][] outputs, T[] inputs, int player, int playersCount, int i)
        {
            double[] brainInput = new double[playersCount];
            brainInput[0] = Convert.ToDouble(inputs[i]);
            brainInput[1] = brainInput[0];
            

            return brainInput;
        }

        /*

        brainInput[0] = {inputs[i], outputs[1][i-1], outputs[2][i-1], outputs[3][i-1]} 
        brainInput[1] = {inputs[i], outputs[0][i-1], outputs[2][i-1], outputs[3][i-1]}
        brainInput[2] = {inputs[i], outputs[0][i-1], outputs[1][i-1], outputs[3][i-1]}
        brainInput[3] = {inputs[i], outputs[0][i-1], outputs[1][i-1], outputs[3][i-1]}
        
        */

        public static void SetSongById(int songId)
        {
            string song = "unknown";
            switch (songId%1000/10)
            {
                case 0:
                    MusicLibrary.CurrentSong = MusicLibrary.ForrestGumpSong;
                    song = "Forrest Gump";
                    break;
                case 1:
                    MusicLibrary.CurrentSong = MusicLibrary.BirtheShort;
                    song = "Vi Maler Byen Rød";
                    break;
                case 2:
                    MusicLibrary.CurrentSong = MusicLibrary.AuldLangSyneSong;
                    song = "Auld Lang Syne";
                    break;
                case 3:
                    MusicLibrary.CurrentSong = MusicLibrary.Flim_Slow_Short;
                    song = "Flim";
                    break;
                default:
                    throw new IOException("Illegal song id");
            }
            Console.WriteLine(song);
        }

        public static void SetFitnessById(int songId)
        {
            string fitness = "unknown";
            RESTS = true;
            // Decide fitness
            switch ((songId - songId % 1000) / 1000 - 1)
            {
                case 0:
                    FITNESS_FUNCTION = new FitnessFunctionChords();
                    RESTS = false;
                    fitness = "Chords";
                    break;
                case 1:
                    FITNESS_FUNCTION = new FitnessFunctionRhythm();
                    fitness = "Rhythm";
                    break;
                case 2:
                    FITNESS_FUNCTION = new FitnessFunctionSameNote();
                    fitness = "SameNote";
                    break;
                case 3:
                    FITNESS_FUNCTION = new FitnessFunctionDeep();
                    fitness = "Deep";
                    break;
                case 4:
                    FITNESS_FUNCTION = new FitnessFunctionBadPitch();
                    fitness = "Bad Pitch";
                    break;
                default:
                    throw new IOException("Illegal fitness id");
            }
            Console.WriteLine("Fitness: " + fitness);
        }

        public static double[] getBrainInputDelayed<T>(T[][] outputs, T[] inputs, int player, int playersCount, int i)
        {
            double[] brainInput = new double[playersCount];
            brainInput[0] = Convert.ToDouble(inputs[i]);

            if (i == 0)
            {
                for (int j = 1; j < playersCount; j++)
                    brainInput[j] = 0.5;

            }
            else
            {
                for (int j = 0; j < playersCount; j++)
                {
                    if (j < player)
                    {
                        brainInput[j + 1] = Convert.ToDouble(outputs[j][i - 1]);
                    }
                    else if (j == player)
                    {
                        //continue - do nothing      
                    }
                    else if (j > player)
                    {
                        brainInput[j] = Convert.ToDouble(outputs[j][i - 1]);
                    }
                }
            }
            
            return brainInput;
        }


        /*

        brainInput[0] = {inputs[i], 0.5, 0.5, 0.5} 
        brainInput[1] = {inputs[i], outputs[0][i], 0.5, 0.5}
        brainInput[2] = {inputs[i], outputs[0][i], outputs[1][i], 0.5}
        brainInput[3] = {inputs[i], outputs[0][i], outputs[1][i], outputs[3][i]}
        
        */


        public static double[] getBrainInputCascading<T>(T[][] outputs, T[] inputs, int player, int playersCount, int i)
        {
            double[] brainInput = new double[playersCount];
            brainInput[0] = Convert.ToDouble(inputs[i]);

            if (i == 0 && player == 0)
            {
                for (int j = 1; j < playersCount; j++)
                    brainInput[j] = 0.5;

            }
            else
            {
                for (int j = 0; j < playersCount - 1; j++)
                {
                    if (j < player)
                    {
                        brainInput[j + 1] = Convert.ToDouble(outputs[j][i]);
                    }
                    else
                    {
                        brainInput[j + 1] = 0.5;
                    }
                }
            }

            return brainInput;
        }

        /*

        brainInput[0] = {inputs[i], inputs[i], inputs[i], inputs[i]} 
        brainInput[1] = {inputs[i], outputs[0][i], inputs[i], outputs[0][i]}
        brainInput[2] = {inputs[i], outputs[0][i], outputs[1][i], inputs[i]}
        brainInput[3] = {inputs[i], outputs[0][i], outputs[1][i], outputs[3][i]}
        
        */

        public static double[] getBrainInputCascadingDuplicating<T>(T[][] outputs, T[] inputs, int player, int playersCount, int i)
        {
            double[] brainInput = new double[playersCount];
            brainInput[0] = Convert.ToDouble(inputs[i]);

            if (i == 0 && player == 0)
            {
                for (int j = 1; j < playersCount; j++)
                    brainInput[j] = 0.5;
            }
            else
            {
                for (int j = 0; j < playersCount - 1; j++)
                {
                    if (j < player)
                    {
                        brainInput[j + 1] = Convert.ToDouble(outputs[j][i]);
                    }
                    else
                    {
                        if (player == 0)
                        {
                            brainInput[j + 1] = brainInput[0];
                        }
                        else
                        {
                            brainInput[j + 1] = brainInput[j % player];
                        }
                    }
                }
            }

            return brainInput;
        }


        public static double[] OutputsToDurations(double[] durationOutput, out List<int> rests)
        {
            //24 ticks per quarter note, ergo 24*4 ticks per measure
            //look at outputs and find out when there are new notes and what they are
            List<double> durationsList = new List<double>();
            rests = new List<int>();
            int lastNewNoteTick = 0;

            SmoothedZSmoothening smoothening = new SmoothedZSmoothening(LAG, THRESHOLD, INFLUENCE);

            for (int currentTick = 1; currentTick < durationOutput.Length; currentTick++)
            {
                int peak = smoothening.isPeak(durationOutput[currentTick]);

                if (peak == 1)
                {
                    durationsList.Add((currentTick - lastNewNoteTick) / (TICKS_PER_QUARTER * QUARTERS_PER_BAR));
                    lastNewNoteTick = currentTick;
                }
                else if (RESTS && peak == -1)
                {
                    if (rests.LastOrDefault() == durationsList.Count - 1)
                    {
                        //Last note was a rest

                        durationsList[durationsList.Count - 1] += (currentTick - lastNewNoteTick)/
                                                                  (TICKS_PER_QUARTER*QUARTERS_PER_BAR);
                        lastNewNoteTick = currentTick;
                    }
                    else
                    {
                        //Last note was NOT a rest

                    durationsList.Add((currentTick - lastNewNoteTick) / (TICKS_PER_QUARTER * QUARTERS_PER_BAR));
                    lastNewNoteTick = currentTick;
                    rests.Add(durationsList.Count - 1);
                }
            }
            }
            durationsList.Add((durationOutput.Length - lastNewNoteTick) / (TICKS_PER_QUARTER * QUARTERS_PER_BAR));

            return durationsList.ToArray();
        }

        public static double ticksToDuration(double ticks)
        {
            return ticks / (TICKS_PER_QUARTER * QUARTERS_PER_BAR); 
        }

        public static double durationToTicks(double duration)
        {
            return duration*(TICKS_PER_QUARTER*QUARTERS_PER_BAR);
        }

        public static void setTargetPhrase(double[] durations, int[] pitches)
        {
            currentDurationPhrase = durations;
            currentPitchPhrase = pitches;
        }
    }
}
