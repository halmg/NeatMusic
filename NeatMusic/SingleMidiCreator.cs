using System;
using System.Collections.Generic;
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
    public class SingleMidiCreator : MidiCreator
    {
        public override void Create()
        {
            double[] currentDurationPhrase = MusicLibrary.CurrentSongDuration();
            int[] currentPitchPhrase = MusicLibrary.CurrentSongPitch();

            double[] rhythmInputs = MusicEnvironment.createDurationInputs(currentDurationPhrase);
            int[] pitchInputs = MusicEnvironment.createPitchInputs(currentPitchPhrase, currentDurationPhrase);

            string CHAMPION_FILE = @"..\..\..\NeatMusic\bin\Debug\host_parasite_champion.xml";

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

            foreach (var neatGenome in anns)
            {
                Console.WriteLine("id_" + neatGenome.Id);
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


            printFitness(MODULE_COUNT, pitchPlayers, rhythmPlayers);

            //get standard deviation
            Console.WriteLine(@"Input deviation: {0}", SmoothedZSmoothening.StandardDeviation(rhythmInputs));

            for (int i = 0; i < rhythmOutput.Length; i++)
            {
                double standardDev = SmoothedZSmoothening.StandardDeviation(rhythmOutput[i]);

                Console.WriteLine(@"Module {0} deviation: {1}", i, standardDev);
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
                printResults(durationsLists[i], pitchLists[i], i + 1);
            }

            Sequence seq = new Sequence();

            seq.Add(getTrack(currentPitchPhrase, currentDurationPhrase, 60));
            //save input phrase in separate file
            seq.Save("base.mid");

            for (int i = 0; i < MODULE_COUNT; i++)
            {

                //int offset = i%2 == 0 ? 60 + 12 * (i/2 + 1) : 48 - 12 * (i/2 + 1); //how should this be done?
                int offset = 48;

                Track t = getTrack(pitchLists[i].ToArray(), durationsLists[i].ToArray(), offset);

                Sequence singleTrack = new Sequence();
                singleTrack.Add(t);
                singleTrack.Save("track" + (i + 1) + ".mid");

                seq.Add(t);
            }

            seq.Save("test.mid");

            // Hit return to quit.
            Console.ReadLine();
        }
    }
}