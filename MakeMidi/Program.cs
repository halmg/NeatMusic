using Keyboard;
using log4net.Config;
using NeatMusic;
using Sanford.Multimedia.Midi;
using SharpNeat.Core;
using SharpNeat.Decoders.Neat;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SharpNeat.Domains;
using static NeatMusic.Music.Pitch;
using static NeatMusic.Music.Chord;
using static NeatMusic.Music.Key;

namespace MakeMidi
{
    class Program
    {
        static void Main(string[] args)
        {
            bool createMany = false;
            MidiCreator creator = createMany ? (MidiCreator) new MultipleMidiCreator() : (MidiCreator) new SingleMidiCreator();
            creator.Create();
            Console.WriteLine("Press any button to end program.");
            Console.ReadLine();
        }
    }
}
