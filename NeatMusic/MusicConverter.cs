using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace NeatMusic
{
    public static class MusicConverter
    {
        private static readonly List<String> Notes = new List<String>()
        {
            "C",
            "CSharp",
            "D",
            "DSharp",
            "E",
            "F",
            "FSharp",
            "G",
            "GSharp",
            "A",
            "ASharp",
            "B"
        };

        public static List<String> ConvertDurations(double[] durations)
        {
            return durations.Select(ConvertDuration).ToList();
        }

        public static List<String> ConvertDurations(List<double> durations)
        {
            return durations.Select(ConvertDuration).ToList();
        }

        public static List<String> ConvertPitches(int[] pitches)
        {
            return pitches.Select(ConvertPitch).ToList();
        }

        public static List<String> ConvertPitches(List<int> pitches)
        {
            return pitches.Select(ConvertPitch).ToList();
        }

        public static int[] ConvertNotePitches(List<String> notePitches)
        {
            return notePitches.Select(ConvertNote).ToArray();
        }

        public static double[] ConvertNoteRhythms(List<String> noteRhythms)
        {
            return noteRhythms.Select(ConvertNoteRhythm).ToArray();
        }

        public static int ConvertNote(String note)
        {
            return Notes.IndexOf(note);
        }

        public static String ConvertPitch(int pitch)
        {
            if (pitch < 0 || pitch > 12) return "Unknown";
            return Notes[pitch % 12];
        }

        public static String ConvertDuration(double duration)
        {
            // Switch statements can not use double so we muliply with 1000 to get an integer representation.
            int representation = (int)Math.Round(duration * 1000);

            switch (representation)
            {
                case 125:
                    return "1/8";
                case 250:
                    return "1/4";
                case 375:
                    return "3/8";
                case 500:
                    return "1/2";
                case 625:
                    return "5/8";
                case 750:
                    return "3/4";
                case 1000:
                    return "1/1";
                case 1125:
                    return "9/8";
                default:
                    return duration.ToString();
            }
        }

        public static double ConvertNoteRhythm(String noteRhythm)
        {
            switch (noteRhythm)
            {
                case "1/8":
                    return 0.125;
                case "1/4":
                    return 0.25;
                case "3/8":
                    return 0.375;
                case "1/2":
                    return 0.5;
                case "5/8":
                    return 0.625;
                case "3/4":
                    return 0.75;
                case "1/1":
                    return 1.0;
                case "9/8":
                    return 1.125;
                default:
                    return 10000000.0;
            }
        }
    }
}
