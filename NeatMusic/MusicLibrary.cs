using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static NeatMusic.Music.Pitch;
using static NeatMusic.Music.Key;
using static NeatMusic.Music.Duration;
using static NeatMusic.Music.Chord;

namespace NeatMusic
{
    public static class MusicLibrary
    {
        public static Song ForrestGumpSong = new Song(C_Major, new List<Music.Pitch>()
        {
            e, f, g, g, e, g, c, g, e,
            f, g, a, a, f, a, a,
            f, g, a, a, f, a, d, b, g,
            e, f, g, g, c, g, g,
            a, b, c, c, a, c, a, c, a,
            f, g, a, a, f, a, a,
            f, g, a, a, f, d, e, f, d,
            e
        }, new List<Music.Duration>()
        {
            Eighth, Eighth, Eighth, Quarter, Quarter, Quarter_Dotted, Eighth, Quarter, Quarter_Dotted,
            Eighth, Eighth, Eighth, Quarter, Quarter, Eighth, Whole,
            Eighth, Eighth, Eighth, Quarter, Quarter, Quarter_Dotted, Eighth, Quarter, Quarter_Dotted,
            Eighth, Eighth, Eighth, Quarter, Quarter, Eighth, Whole,
            Eighth, Eighth, Eighth, Quarter, Quarter, Quarter_Dotted, Eighth, Quarter, Quarter_Dotted,
            Eighth, Eighth, Eighth, Quarter, Quarter, Eighth, Whole,
            Eighth, Eighth, Eighth, Quarter, Quarter, Quarter_Dotted, Eighth, Quarter, Quarter_Dotted,
            Whole
        });

        public static Song ForrestGumpSongShort = new Song(C_Major, new List<Music.Pitch>()
        {
            f, g, a, a, f, a, d, b, g,
            e, f, g, g, c, g, g,
            a, b, c, c, a, c, a, c, a,
            f, g, a, a, f, a, a,
            f, g, a, a, f, d, e, f, d,
            e
        }, new List<Music.Duration>()
        {
            Eighth, Eighth, Eighth, Quarter, Quarter, Quarter_Dotted, Eighth, Quarter, Quarter_Dotted,
            Eighth, Eighth, Eighth, Quarter, Quarter, Eighth, Whole,
            Eighth, Eighth, Eighth, Quarter, Quarter, Quarter_Dotted, Eighth, Quarter, Quarter_Dotted,
            Eighth, Eighth, Eighth, Quarter, Quarter, Eighth, Whole,
            Eighth, Eighth, Eighth, Quarter, Quarter, Quarter_Dotted, Eighth, Quarter, Quarter_Dotted,
            Whole
        });

        public static Song ForrestGumpSongMiddle = new Song(C_Major, new List<Music.Pitch>()
        {
            e, f, g, g, e, g, c, g, e,
            f, g, a, a, f, a, a,
            f, g, a, a, f, a, d, b, g,
            e, f, g, g, c, g, g,
            a, b, c, c, a, c, a, c, a,
            f, g, a, a, f, a, a,
            f, g, a, a, f, d, e, f, d,
            e
        }, new List<Music.Duration>()
        {
            Eighth, Eighth, Eighth, Quarter, Quarter, Quarter_Dotted, Eighth, Quarter, Quarter_Dotted,
            Eighth, Eighth, Eighth, Quarter, Quarter, Eighth, Whole,
            Eighth, Eighth, Eighth, Quarter, Quarter, Quarter_Dotted, Eighth, Quarter, Quarter_Dotted,
            Eighth, Eighth, Eighth, Quarter, Quarter, Eighth, Whole,
            Eighth, Eighth, Eighth, Quarter, Quarter, Quarter_Dotted, Eighth, Quarter, Quarter_Dotted,
            Eighth, Eighth, Eighth, Quarter, Quarter, Eighth, Whole,
            Eighth, Eighth, Eighth, Quarter, Quarter, Quarter_Dotted, Eighth, Quarter, Quarter_Dotted,
            Whole
        });

        public static Song AuldLangSyneSong = new Song(C_Major, new List<Music.Pitch>()
        {
            g,
            c, g, c, e,
            d, c, d, e, d,
            c, c, e, g,
            a, a,
            g, e, e, c,
            d, c, d, e, d,
            c, a, a, g,
            c
        }, new List<Music.Duration>()
        {
            Quarter,
            Quarter_Dotted, Eighth, Quarter, Quarter,
            Quarter_Dotted, Eighth, Quarter, Eighth, Eighth,
            Quarter_Dotted, Eighth, Quarter, Quarter,
            Half_Dotted, Quarter,
            Quarter_Dotted, Eighth, Quarter, Quarter,
            Quarter_Dotted, Eighth, Quarter, Eighth, Eighth,
            Quarter_Dotted, Eighth, Quarter, Quarter,
            Half_Dotted
        });

        public static Song KeshaPichSong = new Song(C_Major, new int[]
        {
            4, 2, 0,
            0, 2, 4,
            0, 2, 4, 5, 4, 2,
            7, 4, 9, 7,
            4, 5, 7,
            4, 2, 4, 0, 2,
            0, 0, 2, 4, 5, 7, 2,
            7, 4, 9, 7,
            4, 5, 7,
            7, 7, 7, 5, 4, 0
        }, new double[]
        {
            0.125, 0.125, 0.125,
            0.25, 0.25, 0.25,
            0.125, 0.125, 0.125, 0.125, 0.125, 0.75,
            0.125, 0.125, 0.125, 0.25,
            0.125, 0.125, 0.25,
            0.25, 0.25, 0.25, 0.375, 0.25,
            0.125, 0.125, 0.125, 0.125, 0.125, 0.125, 0.75,
            0.125, 0.125, 0.125, 0.25,
            0.125, 0.125, 0.25,
            0.125, 0.125, 0.125, 0.125, 0.25, 0.5
        });

        public static Song FrereJacques = new Song(C_Major, new List<Music.Pitch>()
        {
            c, d, e, c, c, d, e, c,
            e, f, g, e, f, g, 
            g, a, g, f, e, c,
            g, a, g, f, e, c,
            c, g, c,
            c, g, c
        }, new List<Music.Duration>()
        {
            Quarter, Quarter,Quarter,Quarter, Quarter, Quarter, Quarter, Quarter,
            Quarter, Quarter, Half, Quarter, Quarter, Half,
            Eighth, Eighth, Eighth, Eighth, Quarter, Quarter,
            Eighth, Eighth, Eighth, Eighth, Quarter, Quarter,
            Quarter, Quarter, Half,
            Quarter, Quarter, Half
        });

        public static Song Minuet = new Song(C_Major, new List<Music.Pitch>()
        {
            g, c,d,e,f,
            g,c,c,
            a, f,g,a,b,
            c,c,c,
            f,g,f,e,d,
            e,f,e,d,c,
            b,c,d,e,c,
            d

        }, new List<Music.Duration>()
        {
            Quarter, Eighth, Eighth,Eighth,Eighth,
            Quarter,Quarter,Quarter,
            Quarter, Eighth, Eighth,Eighth,Eighth,
            Quarter,Quarter,Quarter,
            Quarter, Eighth, Eighth,Eighth,Eighth,
            Quarter, Eighth, Eighth,Eighth,Eighth,
            Quarter, Eighth, Eighth,Eighth,Eighth,
            Whole
        });

        public static Song Lullaby = new Song(C_Major, new List<Music.Pitch>()
        {
            e,e,
            g,e,e,
            g,e,g,
            c, b, a,
            a,g,d,e,
            f,d,d,e,
            f,d,f,
            b,a,g,b,
            c,c,c,
            c,a,f,
            g,e,c,
            f,g,a,
            g,c,c,
            c,a,f,
            g,e,c,
            f,e,d,
            c

        }, new List<Music.Duration>()
        {
            Eighth,Eighth,
            Quarter_Dotted, Eighth, Quarter,
            Half, Eighth,Eighth,
            Quarter, Quarter_Dotted, Eighth,
            Quarter,Quarter,Eighth,Eighth,
            Quarter,Quarter,Eighth,Eighth,
            Half, Eighth,Eighth,
            Eighth,Eighth,Quarter,Quarter,
            Half, Eighth,Eighth,
            Half, Eighth,Eighth,
            Half, Eighth,Eighth,
            Quarter,Quarter,Quarter,
            Half, Eighth,Eighth,
            Half, Eighth,Eighth,
            Half, Eighth,Eighth,
            Quarter,Quarter,Quarter,
            Whole
        });

        public static Song OdeToJoy = new Song(C_Major, new List<Music.Pitch>()
        {
            e,e,f,g,
            g,f,e,d,
            c,c,d,e,
            e,d,d,
            e,e,f,g,
            g,f,e,d,
            c,c,d,e,
            d,c,c,
            d,d,e,c,
            d,e,f,e,c,
            d,e,f,e,d,
            c,d,g,
            e,e,f,g,
            g,f,e,d,
            c,c,d,e,
            d,c,c
        }, new List<Music.Duration>()
        {
            Quarter,Quarter,Quarter,Quarter,
            Quarter,Quarter,Quarter,Quarter,
            Quarter,Quarter,Quarter,Quarter,
            Quarter_Dotted,Eighth,Half,
            Quarter,Quarter,Quarter,Quarter,
            Quarter,Quarter,Quarter,Quarter,
            Quarter,Quarter,Quarter,Quarter,
            Quarter_Dotted,Eighth,Half,
            Quarter,Quarter,Quarter,Quarter,
            Quarter,Eighth,Eighth,Quarter,Quarter,
            Quarter,Eighth,Eighth,Quarter,Quarter,
            Quarter,Quarter,Half,
            Quarter,Quarter,Quarter,Quarter,
            Quarter,Quarter,Quarter,Quarter,
            Quarter,Quarter,Quarter,Quarter,
            Quarter_Dotted,Eighth,Half
        });

        public static Song Flim_Slow = new Song(C_Major, new List<Music.Pitch>()
        {
            gP, fP, eP, cP,
            b, cP, b, cP, b, g,
            f, a, eP, cP,
            b, g,
            gP, fP, eP, cP,
            b, cP, b, cP, b, g,
            f, a, eP, cP,
            b, g
        }, new List<Music.Duration>()
        {
            Quarter, Quarter, Quarter, Whole_Quarter,
            Eighth, Eighth, Eighth, Eighth, Quarter, Whole_Quarter,
            Quarter, Quarter, Quarter, Whole_Quarter,
            Half_Dotted, Whole_Quarter,
            Quarter, Quarter, Quarter, Whole_Quarter,
            Eighth, Eighth, Eighth, Eighth, Quarter, Whole_Quarter,
            Quarter, Quarter, Quarter, Whole_Quarter,
            Half_Dotted, Whole_Quarter
        });


        public static Song Flim_Slow_Short = new Song(C_Major, new List<Music.Pitch>()
        {
            gP, fP, eP, cP,
            b, cP, b, cP, b, g,
            f, a, eP, cP,
            b, g,
        }, new List<Music.Duration>()
        {
            Quarter, Quarter, Quarter, Whole_Quarter,
            Eighth, Eighth, Eighth, Eighth, Quarter, Whole_Quarter,
            Quarter, Quarter, Quarter, Whole_Quarter,
            Half_Dotted, Whole_Quarter,
        });

        public static Song Flim_Fast = new Song(C_Major, new List<Music.Pitch>()
        {
            gP, fP, eP, cP,
            b, cP, b, cP, b, g,
            f, a, eP, cP,
            b, g,
            gP, fP, eP, cP,
            b, cP, b, cP, b, g,
            f, a, eP, cP,
            b, g
        }, new List<Music.Duration>()
        {
            Eighth, Eighth, Eighth, Half_Eighth,
            Sixteenth, Sixteenth, Sixteenth, Sixteenth, Eighth, Half_Eighth,
            Eighth, Eighth, Eighth, Half_Eighth,
            Quarter_Dotted, Half_Eighth,
            Eighth, Eighth, Eighth, Half_Eighth,
            Sixteenth, Sixteenth, Sixteenth, Sixteenth, Eighth, Half_Eighth,
            Eighth, Eighth, Eighth, Half_Eighth,
            Quarter_Dotted, Half_Eighth
        });

        public static Song Birthe = new Song(C_Major, new List<Music.Pitch>()
        {
            eP, dP, eP,
            dP, cP, g, g,
            dP, cP, g, cP,
            b, cP, a, 
            fP, eP, fP, eP, dP, dP,
            dP, eP, dP, b, a, g,
            rest, eP, dP, eP,
            dP, cP, g, g,
            dP, cP, g, cP, b, cP, a,
            a, gis, a, fP, dP,
            eP, fP, eP, g,
            fP, eP, dP, rest, 
            dP, cP, b, cP
        }, new List<Music.Duration>()
        {
            Quarter, Quarter, Quarter, Quarter, Quarter, Quarter,
            Eighth, Quarter_Dotted, Quarter, Quarter, Quarter, Quarter, Eighth, Half_Dotted, Quarter_Dotted,
            Quarter, Quarter, Quarter, Eighth, Half,
            Eighth, Quarter, Quarter, Quarter, Quarter, Whole,
            Quarter, Quarter, Quarter, Quarter, Quarter, Quarter, Quarter, 
            Eighth, Quarter_Dotted, Quarter, Quarter, Quarter, Quarter, Eighth, Half_Dotted,
            Quarter_Dotted, Quarter, Quarter, Quarter, Quarter,
            Quarter, Quarter, Eighth, Quarter_Dotted,
            Quarter, Quarter, Whole, Quarter, 
            Quarter, Quarter, Quarter, Whole,
        });

        public static Song BirtheShort = new Song(C_Major, new List<Music.Pitch>()
        {
            eP, dP, eP,
            dP, cP, g, g,
            dP, cP, g, cP, b, cP, a,
            a, gis, a, fP, dP,
            eP, fP, eP, g,
            fP, eP, dP, rest,
            dP, cP, b, cP
        }, new List<Music.Duration>()
        {
            Quarter, Quarter, Quarter, Quarter, Quarter, Quarter,
            Eighth, Quarter_Dotted, Quarter, Quarter, Quarter, Quarter, Eighth, Half_Dotted,
            Quarter_Dotted, Quarter, Quarter, Quarter, Quarter,
            Quarter, Quarter, Eighth, Quarter_Dotted,
            Quarter, Quarter, Whole, Quarter,
            Quarter, Quarter, Quarter, Whole,
        });

        public static int[] CurrentSongPitch()
        {
            return CurrentSong.Pitches.Select(p => p.Value).ToArray();
        }

        public static int[] CurrentSongTrainingPitch(int notesToCopy)
        {
            if (notesToCopy > CurrentSongPitch().Length) notesToCopy = CurrentSongPitch().Length;
            int[] training = new int[notesToCopy];
            Array.Copy(CurrentSong.Pitches.Select(p => p.Value).ToArray(), training, notesToCopy);
            return training;
        }

        public static double[] CurrentSongDuration()
        {
            return CurrentSong.Durations.Select(p => p.Value).ToArray();
        }

        public static double[] CurrentSongTrainingDuration(int notesToCopy)
        {
            if (notesToCopy > CurrentSongDuration().Length) notesToCopy = CurrentSongDuration().Length;
            double[] training = new double[notesToCopy];
            Array.Copy(CurrentSong.Durations.Select(p => p.Value).ToArray(), training, notesToCopy);
            return training;
        }

        public static Music.Key CurrentSongKey()
        {
            return CurrentSong.Key;

        }
        
        public static Music.Chord[] CurrentSongChords()
        {
            return Music.Harmonize(CurrentSong).ToArray();
        }

        // CHANGE CURRENT SONG TO YOUR SONG OF CHOICE HERE!!! :D :D :D
        public static Song CurrentSong = ForrestGumpSong;

    }

    public class Song
    {
        public Music.Key Key { get; }
        public List<Music.Pitch> Pitches { get; }
        public List<Music.Duration> Durations { get; }

        public Song(Music.Key key, List<Music.Pitch> pitches, List<Music.Duration> durations)
        {
            Key = key;
            Pitches = pitches;
            Durations = durations;
        }

        public Song(Music.Key key, int[] pitches, double[] durations)
            : this(key, pitches.Select(p => Music.Pitch.OfInt(p)).ToList(),
                durations.Select(d => Music.Duration.OfDouble(d)).ToList()) {}
    }
}
