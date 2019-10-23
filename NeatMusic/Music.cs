using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static NeatMusic.Music.Pitch;
using static NeatMusic.Music.Key;
using static NeatMusic.Music.Duration;
using static NeatMusic.Music.Chord;
using static NeatMusic.Music.ChordKind;


namespace NeatMusic
{
    public static class Music
    {
        private static readonly List<Pitch> Pitches = new List<Pitch>() { c, cis, d, dis, e, f, fis, g, gis, a, ais, b };
        private static readonly List<Duration> Durations = new List<Duration>() { Whole, Whole_Dotted, Half, Half_Dotted, Quarter, Quarter_Dotted, Eighth, Eighth_Dotted, Sixteenth, Sixteenth_Dotted };
        private static readonly List<Chord> Chords = new List<Chord>() {C, Cm, Cdim, D, Dm, Ddim, E, Em, Edim, F, Fm, Fdim, G, Gm, Gdim, A, Am, Adim, B, Bm, Bdim};
        private static readonly List<Key> Keys = new List<Key>() { C_Major, C_Minor, D_Major, D_Minor, E_Major, E_Minor, F_Major, F_Minor, G_Major, G_Minor, A_Major, A_Minor, B_Major, B_Minor };

        //private const bool Major = true;
        //private const bool Minor = false;

        public enum KeyKind
        {
            Major, Minor
        }

        public enum ChordKind
        {
            Major, Minor, Dim
        }

        public class Pitch
        {
            private string Name { get; }
            public int Value { get; }

            public static readonly Pitch rest = new Pitch("_", -1);
            public static readonly Pitch c = new Pitch("c", 0);
            public static readonly Pitch d = new Pitch("d", 2);
            public static readonly Pitch e = new Pitch("e", 4);
            public static readonly Pitch f = new Pitch("f", 5);
            public static readonly Pitch g = new Pitch("g", 7);
            public static readonly Pitch a = new Pitch("a", 9);
            public static readonly Pitch b = new Pitch("b", 11);

            public static readonly Pitch cis = new Pitch("cis", 1);
            public static readonly Pitch dis = new Pitch("dis", 3);
            public static readonly Pitch eis = new Pitch("eis", 5);
            public static readonly Pitch fis = new Pitch("fis", 6);
            public static readonly Pitch gis = new Pitch("gis", 8);
            public static readonly Pitch ais = new Pitch("ais", 10);
            public static readonly Pitch bis = new Pitch("bis", 0);

            public static readonly Pitch cb = new Pitch("cb", 11);
            public static readonly Pitch db = new Pitch("db", 1);
            public static readonly Pitch eb = new Pitch("eb", 3);
            public static readonly Pitch fb = new Pitch("fb", 4);
            public static readonly Pitch gb = new Pitch("gb", 6);
            public static readonly Pitch ab = new Pitch("ab", 8);
            public static readonly Pitch bb = new Pitch("bb", 10);

            public static readonly Pitch cP = new Pitch("c", 12);
            public static readonly Pitch dP = new Pitch("d", 14);
            public static readonly Pitch eP = new Pitch("e", 16);
            public static readonly Pitch fP = new Pitch("f", 17);
            public static readonly Pitch gP = new Pitch("g", 19);
            public static readonly Pitch aP = new Pitch("a", 21);
            public static readonly Pitch bP = new Pitch("b", 23);

            public static readonly Pitch cPP = new Pitch("c", 24);


            private Pitch(string name, int value)
            {
                Name = name;
                Value = value;
            }

            public override string ToString()
            {
                return Name;
            }

            protected bool Equals(Pitch other)
            {
                return string.Equals(Name, other.Name) && Value == other.Value;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name?.GetHashCode() ?? 0) * 397) ^ Value;
                }
            }

            public static Pitch OfInt(int pitch)
            {
                if (pitch == -1) return rest;

                return Pitches[pitch % 12];
            }
        }

        public class Duration
        {
            private string Name { get; }
            public double Value { get; }

            public static readonly Duration Whole = new Duration("Whole", 1.0);
            public static readonly Duration Whole_Quarter = new Duration("Whole Quarter", 1.25);
            public static readonly Duration Whole_Dotted = new Duration("Whole Dotted", 1.5);
            public static readonly Duration Half = new Duration("Half", 0.5);
            public static readonly Duration Half_Eighth = new Duration("Half", 0.625);
            public static readonly Duration Half_Dotted = new Duration("Half Dotted", 0.75);
            public static readonly Duration Quarter = new Duration("Quarter", 0.25);
            public static readonly Duration Quarter_Dotted = new Duration("Quarter Dotted", 0.375);
            public static readonly Duration Eighth = new Duration("Eighth", 0.125);
            public static readonly Duration Eighth_Dotted = new Duration("Eighth Dotted", 0.1875);
            public static readonly Duration Sixteenth = new Duration("Sixteenth", 0.0625);
            public static readonly Duration Sixteenth_Dotted = new Duration("Sixteenth Dotted", 0.0625);

            private Duration(string name, double value)
            {
                Name = name;
                Value = value;
            }

            public override string ToString()
            {
                return Name;
            }

            public static Duration OfDouble(double duration)
            {
                Duration dur = Durations.First(d => d.Value.Equals(duration));
                return dur;
            }
        }

        public class Chord
        {
            private string Name { get; }
            public ChordKind Kind { get; }
            public List<Pitch> ChordPitches { get; }
            public Pitch First => ChordPitches[0];
            public Pitch Third => ChordPitches[1];
            public Pitch Fifth => ChordPitches[2];

            // All the major chords.
            public static readonly Chord C = new Chord("C", Major, new List<Pitch>() { c, e, g });
            public static readonly Chord D = new Chord("D", Major, new List<Pitch>() { d, fis, a });
            public static readonly Chord E = new Chord("E", Major, new List<Pitch>() { e, gis, b });
            public static readonly Chord F = new Chord("F", Major, new List<Pitch>() { f, a, c });
            public static readonly Chord G = new Chord("G", Major, new List<Pitch>() { g, b, d });
            public static readonly Chord A = new Chord("A", Major, new List<Pitch>() { a, cis, e });
            public static readonly Chord B = new Chord("B", Major, new List<Pitch>() { b, dis, fis });

            // All the minor chords. 
            public static readonly Chord Cm = new Chord("Cm", Minor, new List<Pitch>() { c, eb, g });
            public static readonly Chord Dm = new Chord("Dm", Minor, new List<Pitch>() { d, f, a });
            public static readonly Chord Em = new Chord("Em", Minor, new List<Pitch>() { e, g, b });
            public static readonly Chord Fm = new Chord("Fm", Minor, new List<Pitch>() { f, ab, c });
            public static readonly Chord Gm = new Chord("Gm", Minor, new List<Pitch>() { g, bb, d });
            public static readonly Chord Am = new Chord("Am", Minor, new List<Pitch>() { a, c, e });
            public static readonly Chord Bm = new Chord("Bm", Minor, new List<Pitch>() { b, d, fis });

            // All the diminished chords. 
            public static readonly Chord Cdim = new Chord("Cdim", Minor, new List<Pitch>() { c, eb, fis });
            public static readonly Chord Ddim = new Chord("Ddim", Minor, new List<Pitch>() { d, f, gis });
            public static readonly Chord Edim = new Chord("Edim", Minor, new List<Pitch>() { e, g, ais });
            public static readonly Chord Fdim = new Chord("Fdim", Minor, new List<Pitch>() { f, ab, b});
            public static readonly Chord Gdim = new Chord("Gdim", Minor, new List<Pitch>() { g, bb, cis });
            public static readonly Chord Adim = new Chord("Adim", Minor, new List<Pitch>() { a, c, eis });
            public static readonly Chord Bdim = new Chord("Bdim", Minor, new List<Pitch>() { b, d, f });

            private Chord(string name, ChordKind kind, List<Pitch> pitches)
            {
                Name = name;
                Kind = kind;
                ChordPitches = pitches;
            }

            public override string ToString()
            {
                return Name;
            }

            protected bool Equals(Chord other)
            {
                return string.Equals(Name, other.Name) && Kind == other.Kind && Equals(ChordPitches, other.ChordPitches);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = Name?.GetHashCode() ?? 0;
                    hashCode = (hashCode * 397) ^ Kind.GetHashCode();
                    hashCode = (hashCode * 397) ^ (ChordPitches?.GetHashCode() ?? 0);
                    return hashCode;
                }
            }

            public bool Contains(Pitch pitch)
            {
                return ChordPitches.Contains(pitch);
            }

            public bool Contains(int pitch)
            {
                return Contains(Pitch.OfInt(pitch));
            }
        }

        public class Key
        {
            private string Name { get; }
            public KeyKind Kind { get; }

            // Pitches
            public List<Pitch> Pitches { get; }
            public Pitch First => Pitches[0];
            public Pitch Second => Pitches[1];
            public Pitch Third => Pitches[2];
            public Pitch Fourth => Pitches[3];
            public Pitch Fifth => Pitches[4];
            public Pitch Sixth => Pitches[5];
            public Pitch Seventh => Pitches[6];

            // Chords
            public List<Chord> Chords { get; }
            public Chord First_Chord => Chords[0];
            public Chord Second_Chord => Chords[1];
            public Chord Third_Chord => Chords[2];
            public Chord Fourth_Chord => Chords[3];
            public Chord Fifth_Chord => Chords[4];
            public Chord Sixth_Chord => Chords[5];
            public Chord Seventh_Chord => Chords[6];

            private List<ChordKind> MajorOrder = new List<ChordKind>() {Major, Minor, Minor, Major, Major, Minor, Dim};
            private List<ChordKind> MinorOrder = new List<ChordKind>() {Minor, Dim, Major, Minor, Major, Major, Major};

            // All the major keys.
            public static readonly Key C_Major = new Key("C Major", KeyKind.Major, new List<Pitch>() { c, d, e, f, g, a });
            //public static readonly Key C_Major = new Key("C Major", KeyKind.Major, new List<Chord>() { C, Dm, Em, F, G, Am, Bdim });
            public static readonly Key D_Major = new Key("D Major", KeyKind.Major, new List<Pitch>() { d, e, fis, g, a, b, cis });
            public static readonly Key E_Major = new Key("E Major", KeyKind.Major, new List<Pitch>() { e, fis, gis, a, b, cis, dis });
            public static readonly Key F_Major = new Key("F Major", KeyKind.Major, new List<Pitch>() { f, g, a, bb, c, d, e });
            public static readonly Key G_Major = new Key("G Major", KeyKind.Major, new List<Pitch>() { g, a, b, c, d, e, fis });
            public static readonly Key A_Major = new Key("A Major", KeyKind.Major, new List<Pitch>() { a, b, cis, d, e, fis, gis });
            public static readonly Key B_Major = new Key("B Major", KeyKind.Major, new List<Pitch>() { b, cis, dis, e, fis, gis, ais });
            
            // All the minor keys. 
            public static readonly Key C_Minor = new Key("C Minor", KeyKind.Minor, new List<Pitch>() { c, d, eb, f, g, ab, bb });
            public static readonly Key D_Minor = new Key("D Minor", KeyKind.Minor, new List<Pitch>() { d, e, f, g, a, bb, c });
            public static readonly Key E_Minor = new Key("E Minor", KeyKind.Minor, new List<Pitch>() { e, fis, g, a, b, c, d });
            public static readonly Key F_Minor = new Key("F Minor", KeyKind.Minor, new List<Pitch>() { f, g, ab, bb, c, db, eb });
            public static readonly Key G_Minor = new Key("G Minor", KeyKind.Minor, new List<Pitch>() { g, a, bb, c, d, eb, f });
            public static readonly Key A_Minor = new Key("A Minor", KeyKind.Minor, new List<Pitch>() { a, b, c, d, e, f, g });
            public static readonly Key B_Minor = new Key("B Minor", KeyKind.Minor, new List<Pitch>() { b, cis, d, e, fis, g, a });

            // All the major keys.
            


            public bool Contains(Pitch pitch)
            {
                return Pitches.Contains(pitch);
            }

            public bool Contains(int pitch)
            {
                return Contains(Pitch.OfInt(pitch));
            }

            private Key(string name, KeyKind kind, List<Pitch> pitches)
            {
                Name = name;
                Kind = kind;
                Pitches = pitches;
            }

            private Key(string name, KeyKind kind, List<Chord> chords)
            {
                Name = name;
                Kind = kind;
                Chords = chords;
                Pitches = chords.Select(c => c.First).ToList();
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public static class Converter
        {
            public static string ToString(int pitch)
            {
                return Pitches[pitch % 12].ToString();
            }

            public static string ToString(List<int> pitches)
            {
                return pitches.Aggregate("", (current, pitch) => current + (ToString(pitch) + " "));
            }

            public static List<Pitch> ToPitches(int[] pitches)
            {
                return ToPitches(pitches.ToList());
            }

            public static List<Pitch> ToPitches(List<int> pitches)
            {
                var result = new List<Pitch>();

                foreach (int pitch in pitches)
                {
                    if (pitch == -1)
                        result.Add(rest);
                    else
                        result.Add(Pitches[pitch % 12]);
                }

                return result;
            }

            public static int[] GetValues(Pitch[] pitches)
            {
                return GetValues(pitches.ToList());
            }

            public static int[] GetValues(List<Pitch> pitches)
            {
                return pitches.Select(pitch => pitch.Value).ToArray();
            }
        }

        public static List<Chord> Harmonize(Song song)
        {
            List<Chord> chords = new List<Chord>();
            
            List<Pitch> pitches = song.Pitches;
            List<Duration> durations = song.Durations;
            Key key = song.Key;
            
            double totalDuration = 0;
            double durationCount = 0;
            int lastIndex = 0;
            
            Console.Write("Chord sequence: \t");

            for (int i = 0; i < durations.Count; i++)
            {
                totalDuration += durations[i].Value;
                durationCount += durations[i].Value;
                
                if (durationCount >= 1.0)
                {
                    var pitchesInChord = pitches.GetRange(lastIndex, i - lastIndex + 1);
                    List<Chord> candidates = new List<Chord>() { C, Dm, Em, F, G, Am, Bdim };
                    
                    Chord pitch = PredictChordRandom(pitchesInChord, key);
                    
                    pitchesInChord.ForEach(p => chords.Add(pitch));

                    lastIndex = i + 1;
                    durationCount -= 1.0;
                }

            }
            
            Console.WriteLine();
            
            chords.Add(C);

            return chords;
        }

        private static Chord PredictChord(List<Pitch> pitches, Key key)
        {
            List<Chord> candidates = new List<Chord>() { C, Dm, Em, F, G, Am, Bdim };
            int[] candCount = new int[candidates.Count];

            //If a 
            foreach (var p in pitches)
            {
                for (int i = 0; i < candidates.Count; i++)
                {
                    if (candidates[i].Contains(p))
                    {
                        candCount[i] += 1;
                    }
                }
            }

            int max = 0;

            for (int i = 0; i < candCount.Length; i++)
            {
                if (candCount[i] > candCount[max])
                {
                    max = i;
                }
            }
            
            Console.Write(@"{0}, ", candidates[max]);

            return candidates[max];
        }


        private static Chord PredictChordRandom(List<Pitch> pitches, Key key)
        {
            List<Chord> candidates = new List<Chord>() {C, Dm, Em, F, G, Am, Bdim};
            int[] candCount = new int[candidates.Count];

            //If a 
            foreach (var p in pitches)
            {
                for (int i = 0; i < candidates.Count; i++)
                {
                    if (candidates[i].Contains(p))
                    {
                        candCount[i] += 1;
                    }
                }
            }

            int max = 0;
            List<int> chordsAtMax = new List<int>();

            for (int i = 0; i < candCount.Length; i++)
            {
                if (candCount[i] > candCount[max])
                {
                    max = i;

                    chordsAtMax = new List<int>();
                    chordsAtMax.Add(i);
                }
                else if (candCount[i] == candCount[max])
                {
                    chordsAtMax.Add(i);
                }
            }

            Chord chord = null;

            if (chordsAtMax.Count == 1)
            {
                chord = candidates[max];
            }
            else if (chordsAtMax.Count > 1)
            {
                Random rand = new Random();
                int index = chordsAtMax[rand.Next(chordsAtMax.Count)];

                chord = candidates[index];
            }

            Console.WriteLine();
            Console.Write("\t");
            Console.Write(@"{0}, ", chord);

            foreach (var p in pitches)
            {
                Console.Write(@"{0} ", p);
            }
            
            return chord;
        }

        public static class Analyser
        {
            public static Key GuessKey(int[] pitches)
            {
                return GuessKey(pitches.ToList());
            }
            public static Key GuessKey(List<int> pitches)
            {
                return GuessKey(Converter.ToPitches(pitches));
            }
            public static Key GuessKey(Pitch[] pitches)
            {
                return GuessKey(pitches.ToList());
            }

            /// <summary>
            /// a very naive implementation that tries to guess which key a melody is in.
            /// It will return null if there is any deviation from a pure key scale or if there are too many possible solutions.
            /// </summary>
            /// <param name="pitches">The melody that we want to guess what key it is in.</param>
            /// <returns>The guessed key the melody is in. If it could not guess it, the return value is null.</returns>
            public static Key GuessKey(List<Pitch> pitches)
            {
                // Find all distinct pitches to avoid iterating through a long melody.
                List<Pitch> distinct = pitches.Distinct().ToList();

                // Find the keys that include the same pitches as the given melody.
                List<Key> possibleKeys = (from key in Keys let excpt = distinct.Except(key.Pitches) where excpt.ToList().Count == 0 select key).ToList();

                // If we end up with two possible keys, we know it is either major or the relative minor.
                if (possibleKeys.Count == 2)
                {
                    // Find the ending pitch of the melody.
                    Pitch endingPitch = pitches[pitches.Count - 1];

                    // If the last pitch is not part of the tonika chord, we assume it must the other relative key.
                    Key first = possibleKeys[0];
                    Key second = possibleKeys[1];
                    if (first.Kind == KeyKind.Major && endingPitch.Equals(first.Pitches[5]))
                        return second;
                    if (first.Kind != KeyKind.Major && endingPitch.Equals(first.Pitches[4]))
                        return first;

                    // If we were not able to tell a difference we just return the Major of the keys. 
                    return first.Kind == KeyKind.Major ? first : second;
                }
                return null;
            }
        }
    }
}
