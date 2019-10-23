using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyboard;
using Sanford.Multimedia.Midi;

namespace NeatMusic
{
    public abstract class MidiCreator
    {
        public const int REST_NOTE = -1;

        // Override this
        public abstract void Create();

        static void addNote(int pitch, double duration, Track track, double current_time)
        {
            int pulses = convertToPulses(duration);
            int current_pulses = convertToPulses(current_time);
            track.Insert(current_pulses, makeStartEvent(pitch));
            track.Insert(current_pulses + pulses, makeStopEvent(pitch));
        }

        protected static int convertToPulses(double duration)
        {
            //let's assume 20 pulses per 1/4 note
            double ticks = duration * (24 / 0.25);
            return (int)ticks;
        }

        protected static void printFitness(int moduleCount, List<NeatPlayer> pitchPlayers, List<NeatPlayer> rhythmPlayers)
        {

            if (moduleCount == 2)
            {
                /*
                    FITNESS PRINT FOR 2 MODULES
                */

                double pitchFitness = 0;

                pitchFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateHarmoniousness(pitchPlayers);
                pitchFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateHarmoniousness(new List<NeatPlayer>() { pitchPlayers[1], pitchPlayers[0] });

                double rhythmFitness = 0;

                rhythmFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateRegularity(rhythmPlayers);
                rhythmFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateRegularity(new List<NeatPlayer>() { rhythmPlayers[1], rhythmPlayers[0] });

                Console.WriteLine(@"Pitch fitness: {0}", pitchFitness);
                Console.WriteLine(@"Rhythm fitness: {0}", rhythmFitness);
            }
            else if (moduleCount == 3)
            {
                /*
                    FITNESS PRINT FOR 3 MODULES
                */

                double pitchFitness = 0;

                pitchFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateHarmoniousness(pitchPlayers);
                pitchFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateHarmoniousness(new List<NeatPlayer>() { pitchPlayers[1], pitchPlayers[0], pitchPlayers[2] });
                pitchFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateHarmoniousness(new List<NeatPlayer>() { pitchPlayers[2], pitchPlayers[0], pitchPlayers[1] });

                double rhythmFitness = 0;

                rhythmFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateRegularity(rhythmPlayers);
                rhythmFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateRegularity(new List<NeatPlayer>() { rhythmPlayers[1], rhythmPlayers[0], rhythmPlayers[2] });
                rhythmFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateRegularity(new List<NeatPlayer>() { rhythmPlayers[2], rhythmPlayers[0], rhythmPlayers[1] });

                Console.WriteLine(@"Pitch fitness: {0}", pitchFitness);
                Console.WriteLine(@"Rhythm fitness: {0}", rhythmFitness);
            }
            else if (moduleCount == 4)
            {
                /*
                    FITNESS PRINT FOR 4 MODULES
                */

                double pitchFitness = 0;

                pitchFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateHarmoniousness(pitchPlayers);
                pitchFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateHarmoniousness(new List<NeatPlayer>() { pitchPlayers[1], pitchPlayers[0], pitchPlayers[2], pitchPlayers[3] });
                pitchFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateHarmoniousness(new List<NeatPlayer>() { pitchPlayers[2], pitchPlayers[0], pitchPlayers[1], pitchPlayers[3] });
                pitchFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateHarmoniousness(new List<NeatPlayer>() { pitchPlayers[3], pitchPlayers[0], pitchPlayers[1], pitchPlayers[2] });

                double rhythmFitness = 0;

                rhythmFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateRegularity(rhythmPlayers);
                rhythmFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateRegularity(new List<NeatPlayer>() { rhythmPlayers[1], rhythmPlayers[0], rhythmPlayers[2], rhythmPlayers[3] });
                rhythmFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateRegularity(new List<NeatPlayer>() { rhythmPlayers[2], rhythmPlayers[0], rhythmPlayers[1], rhythmPlayers[3] });
                rhythmFitness += MusicEnvironment.FITNESS_FUNCTION.CalculateRegularity(new List<NeatPlayer>() { rhythmPlayers[3], rhythmPlayers[0], rhythmPlayers[1], rhythmPlayers[2] });

                Console.WriteLine(@"Pitch fitness: {0}", pitchFitness);
                Console.WriteLine(@"Rhythm fitness: {0}", rhythmFitness);
            }
        }

        protected static void printResults(List<double> durationList, List<int> pitchList, int moduleCount)
        {
            //write the results
            Console.WriteLine("module" + moduleCount);
            foreach (var v in durationList)
            {
                Console.Write(v.ToString() + " ");
            }
            Console.WriteLine();

            foreach (var pitch in Music.Converter.ToPitches(pitchList))
            {
                Console.Write(pitch + " ");
            }
            Console.WriteLine();
        }

        protected static void findNewNotesFromOutputOriginal(double[] rhythmOutput, int[] pitchOutput, int length, out List<double> durationList, out List<int> pitchList)
        {
            durationList = new List<double>();
            pitchList = new List<int>();

            int lastNewNoteTick = 0;

            for (int currentTick = 1; currentTick < length; currentTick++)
            {
                if (rhythmOutput[currentTick] > rhythmOutput[currentTick - 1])
                {
                    durationList.Add((currentTick - lastNewNoteTick) / MusicEnvironment.TICKS_PER_QUARTER * MusicEnvironment.QUARTERS_PER_BAR);
                    pitchList.Add(pitchOutput[lastNewNoteTick]);
                    lastNewNoteTick = currentTick;
                }
            }

            durationList.Add((length - lastNewNoteTick) / MusicEnvironment.TICKS_PER_QUARTER * MusicEnvironment.QUARTERS_PER_BAR);
            pitchList.Add(pitchOutput[lastNewNoteTick]);

        }

        protected static void findNewNotesFromOutput(double[] rhythmOutput, int[] pitchOutput, int length, out List<double> durationList, out List<int> pitchList)
        {
            durationList = new List<double>();
            pitchList = new List<int>();

            int lastNewNoteTick = 0;
            SmoothedZSmoothening smoothening = new SmoothedZSmoothening(MusicEnvironment.LAG, MusicEnvironment.THRESHOLD, MusicEnvironment.INFLUENCE);

            for (int currentTick = 1; currentTick < length; currentTick++)
            {
                int peak = smoothening.isPeak(rhythmOutput[currentTick]);

                if (peak == 1)
                {
                    double duration = (currentTick - lastNewNoteTick) /
                                      (MusicEnvironment.TICKS_PER_QUARTER * MusicEnvironment.QUARTERS_PER_BAR);
                    durationList.Add(duration);
                    pitchList.Add(pitchOutput[lastNewNoteTick]);
                    lastNewNoteTick = currentTick;
                }
                else if (MusicEnvironment.RESTS && peak == REST_NOTE)
                {
                    double duration = (currentTick - lastNewNoteTick) /
                                      (MusicEnvironment.TICKS_PER_QUARTER * MusicEnvironment.QUARTERS_PER_BAR);
                    durationList.Add(duration);
                    pitchList.Add(REST_NOTE);
                    lastNewNoteTick = currentTick;
                }
            }

            durationList.Add((length - lastNewNoteTick) / (MusicEnvironment.TICKS_PER_QUARTER * MusicEnvironment.QUARTERS_PER_BAR));
            pitchList.Add(pitchOutput[lastNewNoteTick]);

        }

        protected static void mergeRests(List<double> duration, List<int> pitch)
        {
            List<int> pitchIndex = new List<int>();
            for (int i = 0; i < pitch.Count; i++)
            {
                if (pitch[i] == REST_NOTE && pitch[i + 1] == REST_NOTE) pitchIndex.Add(i);
            }

            for (int i = pitchIndex.Count - 1; i >= 0; i--)
            {
                int val = pitchIndex[i];

                duration[val] = duration[val] + duration[val + 1];
                duration.RemoveAt(val + 1);

                pitch.RemoveAt(val);
            }
        }

        protected static Track getTrack(int[] pitchPhrase, double[] durationPhrase, int offset)
        {
            double currentTime = 0;
            Track t = new Track();

            for (int i = 0; i < pitchPhrase.Length; i++)
            {
                if (pitchPhrase[i] == REST_NOTE)
                {
                    // The note is a rest - continue, don't add a note
                    // In MIDI, a rest is the absence of a note
                }
                else
                {
                    addNote(pitchPhrase[i] + offset, durationPhrase[i], t, currentTime);
                }

                currentTime += durationPhrase[i];
            }
            return t;
        }

        protected static ChannelMessage makeStartEvent(int pitch)
        {
            ChannelMessageBuilder builder = new ChannelMessageBuilder();
            builder.Command = ChannelCommand.NoteOn;
            builder.MidiChannel = 0;
            builder.Data1 = pitch;
            builder.Data2 = 127;
            builder.Build();
            return builder.Result;
        }

        protected static ChannelMessage makeStopEvent(int pitch)
        {
            ChannelMessageBuilder builder = new ChannelMessageBuilder();
            builder.Command = ChannelCommand.NoteOff;
            builder.MidiChannel = 0;
            builder.Data1 = pitch;
            builder.Data2 = 127;
            builder.Build();
            return builder.Result;
        }
    }
}
