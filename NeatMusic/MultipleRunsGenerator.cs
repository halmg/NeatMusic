using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using log4net.Config;
using NeatMusic.Fitness;
using SharpNeat.Core;
using SharpNeat.Genomes.Neat;

namespace NeatMusic
{
    class MultipleRunsGenerator : IAnnGenerator
    {
        static ModuleNeatEvolutionAlgorithm<NeatGenome>[] _ea;
        const string CHAMPION_FILE = @"..\..\..\NeatMusic\bin\Debug\host_parasite_champion.xml";
        private const int MODULES_COUNT = MusicEnvironment.MODULE_COUNT;

        private const int GENERATIONS_PER_SONG = 250;
        private const int NUMBER_OF_VARIATIONS = 4;

        public void Start()
        {
            int startSong = 1000;
            int j = 0;

            for (int i = startSong; i < startSong + 1000; i += 1000)
            {
                for (int k = 0; k < NUMBER_OF_VARIATIONS; k++)
                {
                    int currentNumber = i + k + j;
                    Generate(currentNumber);
                }
            }
        }

        static void Generate(int songId)
        {
            Console.WriteLine("_________________________________________________________________________");
            Console.WriteLine("Now generating song " + songId);
            
            // Initialise log4net (log to console).
            XmlConfigurator.Configure(new FileInfo("log4net.properties"));

            // Experiment classes encapsulate much of the nuts and bolts of setting up a NEAT search.
            MusicExperiment experiment = new MusicExperiment(MODULES_COUNT);

            // Set the currentFitness
            MusicEnvironment.SetFitnessById(songId);

            // Set the currentSong
            MusicEnvironment.SetSongById(songId);

            // Load config XML.
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load("config.xml");
            experiment.Initialize("NeatMusic", xmlConfig.DocumentElement);

            // Create evolution algorithm and attach update event.
            _ea = experiment.CreateEvolutionAlgorithms();
            _ea[0].UpdateEvent += new EventHandler(ea_UpdateEvent);

            // Start algorithm (it will run on a background thread).
            foreach (var neatEvolutionAlgorithm in _ea)
            {
                neatEvolutionAlgorithm.StartContinue();
            }

            // Continue until certain generations.
            while (_ea[0].CurrentGeneration< GENERATIONS_PER_SONG) { } 

            foreach (var neatEvolutionAlgorithm in _ea)
            {
                neatEvolutionAlgorithm.Stop();
            }
            

            // Wait for threads to pause.
            while (_ea.All(ea => ea.RunState != RunState.Paused))
            {
                Thread.Sleep(100);
            }

            // Save the best genome to file
            //Console.Write("\nFinal Genomes: ");
            Monitor.Enter(_ea);
            List<NeatGenome> finalGenomes = new List<NeatGenome>();
            foreach (var a in _ea)
            {
                //Console.Write("\t" + a.CurrentChampGenome.EvaluationInfo.Fitness + ", id_" + a.CurrentChampGenome.Id);
                finalGenomes.Add(a.CurrentChampGenome);
            }
            var doc = NeatGenomeXmlIO.SaveComplete(finalGenomes, false);
            string championFile = @"..\..\..\NeatMusic\bin\Debug\Champions\" + songId + ".xml";
            Console.WriteLine("\nSaving champion file...");
            Console.WriteLine("_________________________________________________________________________");
            doc.Save(championFile);
            Monitor.Exit(_ea);
        }

        static void ea_UpdateEvent(object sender, EventArgs e)
        {
            Console.Write("gen=" + _ea[0].CurrentGeneration +
                "\tmodules=" + _ea.Length / 2
                );

            foreach (var ea in _ea)
            {
                Console.Write("\t" + ea.CurrentChampGenome.EvaluationInfo.Fitness);
            }
            Console.WriteLine();
        }
    }
}
