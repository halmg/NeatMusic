using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Decoders;
using System.Threading.Tasks;
using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Phenomes;
using SharpNeat.Decoders.Neat;
using SharpNeat.DistanceMetrics;
using SharpNeat.SpeciationStrategies;
using System.Xml;

namespace NeatMusic
{
    internal class MusicExperiment
    {
        NeatEvolutionAlgorithmParameters _eaParams;
        NeatGenomeParameters _neatGenomeParams;
        string _name;
        int _populationSize;
        int _specieCount;
        NetworkActivationScheme _activationScheme;
        string _complexityRegulationStr;
        int? _complexityThreshold;
        string _description;
        ParallelOptions _parallelOptions;
        int _parasiteCount;
        int _championCount;
        private int _moduleCount;

        #region INeatExperiment Members
        public int InputCount => MusicEnvironment.MODULE_COUNT;
        public int OutputCount => 1;
        public int DefaultModuleCount => 2;

        public string Description => _description;

        public string Name => _name;

        public MusicExperiment()
        {
            _moduleCount = DefaultModuleCount;
        }

        public MusicExperiment(int moduleCount)
        {
            _moduleCount = moduleCount;
        }

        /// <summary>
        /// Gets the default population size to use for the experiment.
        /// </summary>
        public int DefaultPopulationSize
        {
            get { return _populationSize; }
        }

        /// <summary>
        /// Gets the NeatEvolutionAlgorithmParameters to be used for the experiment. Parameters on this object can be 
        /// modified. Calls to CreateEvolutionAlgorithm() make a copy of and use this object in whatever state it is in 
        /// at the time of the call.
        /// </summary>
        public NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters
        {
            get { return _eaParams; }
        }

        /// <summary>
        /// Gets the NeatGenomeParameters to be used for the experiment. Parameters on this object can be modified. Calls
        /// to CreateEvolutionAlgorithm() make a copy of and use this object in whatever state it is in at the time of the call.
        /// </summary>
        public NeatGenomeParameters NeatGenomeParameters
        {
            get { return _neatGenomeParams; }
        }

        /// <summary>
        /// Initialize the experiment with some optional XML configutation data.
        /// </summary>
        public void Initialize(string name, XmlElement xmlConfig)
        {
            _name = name;
            _populationSize = XmlUtils.GetValueAsInt(xmlConfig, "PopulationSize");
            _specieCount = XmlUtils.GetValueAsInt(xmlConfig, "SpecieCount");
            //_parasiteCount = XmlUtils.GetValueAsInt(xmlConfig, "ParasiteCount");
            _parasiteCount = InputCount - 1;
            _championCount = InputCount - 1;
            _activationScheme = ExperimentUtils.CreateActivationScheme(xmlConfig, "Activation");
            _complexityRegulationStr = XmlUtils.TryGetValueAsString(xmlConfig, "ComplexityRegulationStrategy");
            _complexityThreshold = XmlUtils.TryGetValueAsInt(xmlConfig, "ComplexityThreshold");
            _description = XmlUtils.TryGetValueAsString(xmlConfig, "Description");
            _parallelOptions = ExperimentUtils.ReadParallelOptions(xmlConfig);

            _eaParams = new NeatEvolutionAlgorithmParameters();
            _eaParams.SpecieCount = _specieCount;
            _neatGenomeParams = new NeatGenomeParameters();
        }

        /// <summary>
        /// Load a population of genomes from an XmlReader and returns the genomes in a new list.
        /// The genome2 factory for the genomes can be obtained from any one of the genomes.
        /// </summary>
        public List<NeatGenome> LoadPopulation(XmlReader xr)
        {
            return NeatGenomeUtils.LoadPopulation(xr, false, this.InputCount, this.OutputCount);
        }

        /// <summary>
        /// Save a population of genomes to an XmlWriter.
        /// </summary>
        public void SavePopulation(XmlWriter xw, IList<NeatGenome> genomeList)
        {
            // Writing node IDs is not necessary for NEAT.
            NeatGenomeXmlIO.WriteComplete(xw, genomeList, false);
        }

        /// <summary>
        /// Create a genome factory for the experiment.
        /// Create a genome factory with our neat genome parameters object and the appropriate number of input and output neuron genes.
        /// </summary>
        public IGenomeFactory<NeatGenome> CreateGenomeFactory()
        {
            return new NeatGenomeFactory(InputCount, OutputCount, _neatGenomeParams);
        }

        /// <summary>
        /// Creates a new genome decoder that can be used to convert a genome into a phenome.
        /// </summary>
        public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
        {
            return new NeatGenomeDecoder(_activationScheme);
        }

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// This overload requires no parameters and uses the default population size.
        /// </summary>
        public ModuleNeatEvolutionAlgorithm<NeatGenome>[] CreateEvolutionAlgorithms()
        {
            return CreateEvolutionAlgorithms(DefaultPopulationSize);
        }

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm list ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// </summary>
        public ModuleNeatEvolutionAlgorithm<NeatGenome>[] CreateEvolutionAlgorithms(int populationSize)
        {
            // Create the modules.
            List<Module> modules = new List<Module>();
            for (int i = 0; i < _moduleCount; i++)
            {
                // Create distance metric. Mismatched genes have a fixed distance of 10; for matched genes the distance is their weigth difference.
                IDistanceMetric distanceMetricPitch = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
                ISpeciationStrategy<NeatGenome> speciationStrategyPitch = new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetricPitch, _parallelOptions);
                IDistanceMetric distanceMetricRhythm = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
                ISpeciationStrategy<NeatGenome> speciationStrategyRhythm = new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetricRhythm, _parallelOptions);


                // Create complexity regulation strategy.
                IComplexityRegulationStrategy complexityRegulationPitch =
                    ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStr, _complexityThreshold);
                IComplexityRegulationStrategy complexityRegulationRhythm =
                    ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStr, _complexityThreshold);


                // Create and add a new module with strategies and the evolution parameters.
                Module module = new Module((i+1), _eaParams, 
                                            speciationStrategyPitch, speciationStrategyRhythm, 
                                            complexityRegulationPitch, complexityRegulationRhythm);
                modules.Add(module);
            }

            // Hook-up the modules with each other in circular order.
            // TODO Right now, they are hooked-up in circular order. Check whether it should be done otherwise.
            //for (int i = 0; i < modules.Count; i++)
            //{
            //    modules[i].SetParasiteModule(i != modules.Count - 1 ? modules[i + 1] : modules[0], _parasiteCount,
            //        _championCount, CreateGenomeDecoder());
            //}
            foreach (var module in modules)
            {
                module.SetParasiteModules(modules.Except(new List<Module>() {module}).ToList(), _parasiteCount, _championCount, CreateGenomeDecoder(), CreateGenomeDecoder());
            }

            // Initialize each module.
            foreach (var module in modules)
            {
                var rhythmFactory = CreateGenomeFactory();
                var pitchFactory = CreateGenomeFactory();
                module.Initialize(rhythmFactory, pitchFactory, populationSize);
            }
            
            // Finished. Return the evolution algorithms
            return
                modules.Select(m => m.RhythmEvolutionAlgorithm).Concat(modules.Select(m => m.PitchEvolutionAlgorithm)).ToArray();
        }
        #endregion
    }
}