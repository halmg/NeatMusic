using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Utility;
using System.Threading;

namespace NeatMusic
{

    /// <summary>
    /// Implements host-parasite coevolution evaluation. Assumes there is another
    /// NEAT evolutionary algorithm running in parallel, evolving "parasites"
    /// simulataneously. 
    /// 
    /// For more information, see the following technical report:
    /// Stanley, K.O. and Miikkulainen, R. "Competitive Coevolution through 
    /// Evolutionary Complexification", Technical Report AI2002-298, 
    /// Department of Computer Sciences, The University of Texas at Austin, 2004.
    /// </summary>
    public class MusicListModulesEvaluator<TGenome, TPhenome> : IGenomeListEvaluator<TGenome>
        where TGenome : class, global::SharpNeat.Core.IGenome<TGenome>
        where TPhenome : class
    {
        public String HostName { get; protected set; }
        readonly int _parasiteGenomesPerEvaluation;
        readonly int _hallOfFameGenomesPerEvaluation;
        private readonly List<ModuleNeatEvolutionAlgorithm<TGenome>> _eaParasites; 
        readonly IGenomeDecoder<TGenome, TPhenome> _genomeDecoder;
        readonly ICoevolutionPhenomeListEvaluator<TPhenome> _phenomeListEvaluator;
        readonly ParallelOptions _parallelOptions;
        readonly Random _random;

        private List<TGenome>[] _parasiteGenomes;
        List<TGenome> _hallOfFame;

        #region Constructors
        /// <summary>
        /// Construct with the provided IGenomeDecoder and ICoevolutionPhenomeEvaluator. 
        /// The number of parallel threads defaults to Environment.ProcessorCount.
        /// </summary>
        public MusicListModulesEvaluator(String name, int parasiteGenomesPerEvaluation,
                                           int hallOfFameGenomesPerEvaluation,
                                           ModuleNeatEvolutionAlgorithm<TGenome> algorithm,
                                           List<ModuleNeatEvolutionAlgorithm<TGenome>> eaParasites,
                                           IGenomeDecoder<TGenome, TPhenome> genomeDecoder,
                                           ICoevolutionPhenomeListEvaluator<TPhenome> phenomeListEvaluator)
            : this(name, parasiteGenomesPerEvaluation, hallOfFameGenomesPerEvaluation, algorithm, 
                    eaParasites, genomeDecoder, phenomeListEvaluator, new ParallelOptions())
        {
        }

        /// <summary>
        /// Construct with the provided IGenomeDecoder, ICoevolutionPhenomeEvaluator and ParalleOptions.
        /// The number of parallel threads defaults to Environment.ProcessorCount.
        /// </summary>
        public MusicListModulesEvaluator(String name, int parasiteGenomesPerEvaluation,
                                           int hallOfFameGenomesPerEvaluation,
                                           ModuleNeatEvolutionAlgorithm<TGenome> algorithm,
                                           List<ModuleNeatEvolutionAlgorithm<TGenome>> eaParasites,
                                           IGenomeDecoder<TGenome, TPhenome> genomeDecoder,
                                           ICoevolutionPhenomeListEvaluator<TPhenome> phenomeListEvaluator,
                                           ParallelOptions options)
        {
            Debug.Assert(parasiteGenomesPerEvaluation >= 0);
            Debug.Assert(hallOfFameGenomesPerEvaluation >= 0);

            HostName = name;
            _parasiteGenomesPerEvaluation = parasiteGenomesPerEvaluation;
            _hallOfFameGenomesPerEvaluation = hallOfFameGenomesPerEvaluation;
            _genomeDecoder = genomeDecoder;
            _phenomeListEvaluator = phenomeListEvaluator;
            _parallelOptions = options;

            _eaParasites = eaParasites;
            _hallOfFame = new List<TGenome>();
            _random = new Random();
            
            _parasiteGenomes = new List<TGenome>[MusicEnvironment.MODULE_COUNT - 1];
            for (int i = 0; i < _parasiteGenomes.Length; i++)
            {
                _parasiteGenomes[i] = new List<TGenome>();
        }
            
            algorithm.UpdateEvent += new EventHandler(eaParasite_UpdateEvent);
        }
        #endregion

        /// <summary>
        /// Gets the total number of individual genome evaluations that have been performed by this evaluator.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _phenomeListEvaluator.EvaluationCount; }
        }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _phenomeListEvaluator.StopConditionSatisfied; }
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {
            //_eaParasites.ForEach(ea => ea.ParasiteGenomes.Clear());
            _hallOfFame.Clear();
            _phenomeListEvaluator.Reset();
        }

        /// <summary>
        /// Main genome evaluation loop with no phenome caching (decode on each evaluation).
        /// Individuals are competed pairwise against every parasite in the parasite list
        /// and against a randomly selected subset of the hall of fame.
        /// </summary>
        public void Evaluate(IList<TGenome> hostGenomeList)
        {
            //Create a temporary list of fitness values with the scores of the inner evaluator.
            FitnessInfo[] results = new FitnessInfo[hostGenomeList.Count];
            for (int i = 0; i < results.Length; i++)
                results[i] = FitnessInfo.Zero;

            // Randomly select champions from the hall of fame.
            TGenome[] champions = _hallOfFame.Count > 0
                                    ? new TGenome[_hallOfFameGenomesPerEvaluation]
                                    : new TGenome[0];
            for (int i = 0; i < champions.Length; i++)
            {
                // Pick a random champion's index
                int hallOfFameIdx = _random.Next(_hallOfFame.Count);

                // Add the champion to the list of competitors.
                champions[i] = _hallOfFame[hallOfFameIdx];
            }

            // Acquire a lock on the parasite genome list.
            //Monitor.Enter(_parasiteGenomes);

            // Exhaustively compete individuals against each other.
            Parallel.For(0, hostGenomeList.Count, delegate (int i)
            {
                // Decode the host genome.
                TPhenome host = _genomeDecoder.Decode(hostGenomeList[i]);
                // Check that the host genome is valid.
                if (host == null)
                    return;

                
                // Evaluate the host against the parasites.
                //List<TPhenome> parasites = _parasiteGenomes.Select(_genomeDecoder.Decode).Where(parasite => parasite != null).ToList();
                for (int j = 0; j < MusicEnvironment.SETS_OF_PARASITES; j++)
                {
                    
                    var parasites = new List<TPhenome>();
                    
                    foreach (var l in _parasiteGenomes)
                    {
                        if (l.Count > 0)
                        {
                            var genome = l[j];
                            if (genome != null)
                                parasites.Add(_genomeDecoder.Decode(genome));
                        }
                    }

                    if (parasites.Count == 0)
                        return;

                    parasites.Insert(0, host);

                    lock (parasites)
                    {
                        FitnessInfo parasiteFitness;
                        _phenomeListEvaluator.Evaluate(parasites, out parasiteFitness);
                        results[i]._fitness += parasiteFitness._fitness;
                    }
                }

                // Evaluate the host against the champions.
                FitnessInfo championFitness;
                List<TPhenome> finalChampions = champions.Select(_genomeDecoder.Decode).Where(champion => champion != null).ToList();

                if (finalChampions.Count == 0)
                    return;

                finalChampions.Insert(0, host);
                Monitor.Enter(finalChampions);

                _phenomeListEvaluator.Evaluate(finalChampions, out championFitness);
                
                // Add the results to each genome's overall fitness.
                results[i]._fitness += championFitness._fitness;
            });

            // Release the lock on the parasite genome list.
            //Monitor.Exit(_parasiteGenomes);
            

            // Update every genome in the population with its new fitness score.
            Monitor.Enter(hostGenomeList);
            TGenome champ = hostGenomeList[0];
            for (int i = 0; i < results.Length; i++)
            {
                TGenome hostGenome = hostGenomeList[i];
                hostGenome.EvaluationInfo.SetFitness(results[i]._fitness);
                //hostGenome.EvaluationInfo.AlternativeFitness = results[i]._alternativeFitness;
            
                // Check if this genome is the generational champion
                if (hostGenome.EvaluationInfo.Fitness > champ.EvaluationInfo.Fitness)
                    champ = hostGenome;
            }
            
            // Add the new champion to the hall of fame.
            _hallOfFame.Add(champ);
            Monitor.Exit(hostGenomeList);

            // Clean up hall of fame. Don't know this is good, just trying it out.
            if (_hallOfFame.Count == 100)
            {
                _hallOfFame.Sort((g1, g2) =>
                        g2.EvaluationInfo.Fitness.CompareTo(g1.EvaluationInfo.Fitness));
            
                // Keep only the top genomes
                _hallOfFame.RemoveRange(10, 90);
            }
        }

        /// <summary>
        /// Updates the champion genomes every generation.
        /// </summary>
        private void eaParasite_UpdateEvent(object sender, EventArgs args)
        {
            // Make sure that the event sender is a NEAT EA.
            Debug.Assert(sender is ModuleNeatEvolutionAlgorithm<TGenome>);

            var algorithm = (ModuleNeatEvolutionAlgorithm<TGenome>)sender;

            // Cast the EA so we can access the current champion.
            //List<NeatEvolutionAlgorithm<TGenome>> eas = (List<NeatEvolutionAlgorithm<TGenome>>)sender;
            //NeatEvolutionAlgorithm<TGenome> ea = (NeatEvolutionAlgorithm<TGenome>)sender;
            // Lock the list to prevent issues with the Evaluate method.

            // Acquire a lock on the parasite genome list.
            //Monitor.Enter(_parasiteGenomes);

            // Clear the competitors list since we're dealing with a new population.
            //_parasiteGenomes.Clear();

            // Add the best genome from every specie to the list
            //foreach (var ea in eas)

            lock (algorithm.ParasiteGenomes)
            {
                List<TGenome> bestGenomes = new List<TGenome>();

                foreach (var specie in algorithm.SpecieList)
                {
                    lock (specie)
                    {
                        if (specie.GenomeList.Count > 0)
                        {
                            TGenome genome = findMax(specie.GenomeList);
                            bestGenomes.Add(genome);
                        }
                        }
                    }

                bestGenomes.Sort((g1, g2) =>
                        g2.EvaluationInfo.Fitness.CompareTo(g1.EvaluationInfo.Fitness));

                foreach (var ea in _eaParasites)
                {
                    ea.UpdateParasiteGenomeList(algorithm.Id, bestGenomes);    
                }


            }

            _parasiteGenomes = algorithm.ParasiteGenomes;
        }

        /// <summary>
        /// Returns the genome in the list with the highest fitness.
        /// </summary>
        private TGenome findMax(List<TGenome> genomeList)
        {
            Debug.Assert(genomeList.Count > 0);

            lock (genomeList)
            {
            TGenome max = genomeList[0];
            if (genomeList.Count == 1) return max;
            for (int i = 0; i < genomeList.Count; i++)
                if (i < genomeList.Count)
                    if (genomeList[i].EvaluationInfo.Fitness > max.EvaluationInfo.Fitness)
                        max = genomeList[i];
            return max;
        }
            
    }
}
}
