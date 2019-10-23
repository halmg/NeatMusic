using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Keyboard;
using log4net.Config;
using Sanford.Multimedia.Midi;
using SharpNeat.Core;
using SharpNeat.Decoders.Neat;
using SharpNeat.Domains;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;

namespace NeatMusic
{
    public class MultipleMidiCreator : MidiCreator
    {
        private string _champions_path = @"..\..\..\NeatMusic\bin\Debug\Champions\";

        public override void Create()
        {
            Console.WriteLine("Creating Midi Files...");
            Console.WriteLine("_________________________________________");

            // Find champion files
            var files = Directory.GetFiles(_champions_path);

            foreach (var filePath in files)
            {
                CreateNewMidi(filePath);
            }

            Console.WriteLine("\nDone...");
            Console.WriteLine("_________________________________________");
        }

        private void CreateNewMidi(string path)
        {
            string savingPath = @"Outputs\";
            string title = path.Replace(_champions_path, "");
            title = title.Replace(".xml", "");

            MusicEnvironment.SetSongById(int.Parse(title));

            double[] currentDurationPhrase = MusicLibrary.CurrentSongDuration();
            int[] currentPitchPhrase = MusicLibrary.CurrentSongPitch();

            double[] rhythmInputs = MusicEnvironment.createDurationInputs(currentDurationPhrase);
            int[] pitchInputs = MusicEnvironment.createPitchInputs(currentPitchPhrase, currentDurationPhrase);

            string CHAMPION_FILE = path;

            List<NeatGenome> anns;

            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.properties"));

            // Load config XML.
            XmlDocument xmlConfig = new XmlDocument();
            //load genomes
            // Save the best genome to file
            XmlReaderSettings xwSettings = new XmlReaderSettings();
            using (XmlReader xw = XmlReader.Create(CHAMPION_FILE, xwSettings))
            {
                anns = NeatGenomeXmlIO.ReadCompleteGenomeList(xw, false);
            }


            // Load config XML.
            XmlDocument xmlConfig1 = new XmlDocument();
            xmlConfig1.Load(@"..\..\..\NeatMusic\bin\Debug\config.xml");
            XmlElement xmlElement = xmlConfig1.DocumentElement;
            var activation = ExperimentUtils.CreateActivationScheme(xmlElement, "Activation");

            IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = new NeatGenomeDecoder(activation);

            int MODULE_COUNT = anns.Count / 2;
            int LENGHT = rhythmInputs.Length;
            int LENGTHPITCH = currentPitchPhrase.Length;
            //int LENGTHPITCH = LENGHT;
            int SEED_PITCH = 0;

            var rhythmPlayers = new List<NeatPlayer>();
            var pitchPlayers = new List<NeatPlayer>();

            for (int i = 0; i < MODULE_COUNT; i++)
            {
                rhythmPlayers.Add(new NeatPlayer(genomeDecoder.Decode(anns[i])));
            }
            for (int i = MODULE_COUNT; i < MODULE_COUNT * 2; i++)
            {
                pitchPlayers.Add(new NeatPlayer(genomeDecoder.Decode(anns[i])));
            }

            double[][] rhythmOutput = new double[MODULE_COUNT][];
            int[][] pitchOutput = new int[MODULE_COUNT][];


            for (int i = 0; i < MODULE_COUNT; i++)
            {
                rhythmOutput[i] = new double[LENGHT];
                pitchOutput[i] = new int[LENGTHPITCH];
            }

            for (int i = 0; i < MODULE_COUNT; i++)
            {
                rhythmOutput[i] = new double[LENGHT];
                pitchOutput[i] = new int[LENGHT];
            }

            for (int i = 0; i < LENGHT; i++)
            {
                for (int p = 0; p < MODULE_COUNT; p++)
                {
                    var brainInput = MusicEnvironment.getBrainInputDelayed(pitchOutput, pitchInputs, p, MODULE_COUNT, i);

                    pitchOutput[p][i] = MusicEnvironment.convertToMidiClass(pitchPlayers[p].calculateOutput(brainInput));
                }
            }

            for (int i = 0; i < LENGHT; i++)
            {
                for (int p = 0; p < MODULE_COUNT; p++)
                {
                    var brainInput = MusicEnvironment.getBrainInputDelayed(rhythmOutput, rhythmInputs, p, MODULE_COUNT, i);

                    rhythmOutput[p][i] = rhythmPlayers[p].calculateOutput(brainInput);

                }
            }

            //printFitness(MODULE_COUNT, pitchPlayers, rhythmPlayers);

            //get standard deviation
            //Console.WriteLine(@"Input deviation: {0}", SmoothedZSmoothening.StandardDeviation(rhythmInputs));

            for (int i = 0; i < rhythmOutput.Length; i++)
            {
                double standardDev = SmoothedZSmoothening.StandardDeviation(rhythmOutput[i]);

                //Console.WriteLine(@"Module {0} deviation: {1}", i, standardDev);
            }

            //look at outputs and find out when there are new notes and what they are
            //new note when current value is higher than previous one

            List<double>[] durationsLists = new List<double>[MODULE_COUNT];
            List<int>[] pitchLists = new List<int>[MODULE_COUNT];

            for (int i = 0; i < MODULE_COUNT; i++)
            {
                durationsLists[i] = new List<double>();
                pitchLists[i] = new List<int>();
                findNewNotesFromOutput(rhythmOutput[i], pitchOutput[i], LENGHT, out durationsLists[i], out pitchLists[i]);
                mergeRests(durationsLists[i], pitchLists[i]);
                //printResults(durationsLists[i], pitchLists[i], i + 1);
            }

            Sequence seq = new Sequence();

            seq.Add(getTrack(currentPitchPhrase, currentDurationPhrase, 60));

            for (int i = 0; i < MODULE_COUNT; i++)
            {
                int offset = 48;
                Track t = getTrack(pitchLists[i].ToArray(), durationsLists[i].ToArray(), offset);
                seq.Add(t);
            }

            // Save the final output
            string finalOutput = savingPath + title + ".mid";
            Console.WriteLine("Saving " + finalOutput);
            seq.Save(finalOutput);
        }
    }
}
