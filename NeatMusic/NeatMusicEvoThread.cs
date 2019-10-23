using log4net.Config;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

// This is just a copy of the Program file adapted to execute as a thread
namespace NeatMusic
{
    public class NeatMusicEvoThread
    {
        static NeatEvolutionAlgorithm<NeatGenome>[] _ea;
        const string CHAMPION_FILE = @"host_parasite_champion.xml";

        volatile bool _shouldStop = false;

        static bool _isWriting = false;

        public void doWork()
        {
            // Initialise log4net (log to console).
            XmlConfigurator.Configure(new FileInfo("log4net.properties"));

            // Experiment classes encapsulate much of the nuts and bolts of setting up a NEAT search.
            MusicExperiment experiment = new MusicExperiment();

            // Load config XML.
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load(@"config.xml");
            experiment.Initialize("NeatMusic", xmlConfig.DocumentElement);

            // Create evolution algorithm and attach update event.
            _ea = experiment.CreateEvolutionAlgorithms();
            _ea[0].UpdateEvent += new EventHandler(ea_UpdateEvent);

            // Start algorithm (it will run on a background thread).
            _ea[0].StartContinue();
            _ea[1].StartContinue();
            _ea[2].StartContinue();
            _ea[3].StartContinue();

            // Hit return to quit.
            //Console.ReadLine();

            // continue execution until RequestStop is called
            while (!_shouldStop)
            {

            }

            // stop the ga threads
            _ea[0].UpdateEvent -= new EventHandler(ea_UpdateEvent);
            _ea[0].RequestPause();
            _ea[1].RequestPause();
            _ea[2].RequestPause();
            _ea[3].RequestPause();
        }

        public void RequestStop()
        {
            _shouldStop = true;
        }

        static void ea_UpdateEvent(object sender, EventArgs e)
        {
            // set the isWriting flag
            _isWriting = true;

            //Console.WriteLine(string.Format("gen={0:N0} bestFitnessX={1:N6} bestFitnessO={2:N6}\n\t bestFitnessX={3:N6} bestFitnessO={4:N6}",
            //    _ea[0].CurrentGeneration, _ea[0].Statistics._maxFitness, _ea[1].Statistics._maxFitness, _ea[2].Statistics._maxFitness, _ea[3].Statistics._maxFitness));

            // Save the best genome to file
            var doc = NeatGenomeXmlIO.SaveComplete(
                new List<NeatGenome>() {
                    _ea[0].CurrentChampGenome,
                    _ea[1].CurrentChampGenome,
                    _ea[2].CurrentChampGenome,
                    _ea[3].CurrentChampGenome
                }, false);
            doc.Save(CHAMPION_FILE);

            // finished writing, unset flag
            _isWriting = false;
        }

        public bool isWriting()
        {
            return _isWriting;
        }

        public void copyChampionFile()
        {
            while (_isWriting) ;
            File.Copy(@"host_parasite_champion.xml", @"host_parasite_champion_to_load.xml", true);
        }

    }
}
