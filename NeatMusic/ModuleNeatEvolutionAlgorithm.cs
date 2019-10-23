using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Core;
using SharpNeat.Decoders;
using SharpNeat.Decoders.Neat;
using SharpNeat.DistanceMetrics;
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using SharpNeat.SpeciationStrategies;

namespace NeatMusic
{
    public class ModuleNeatEvolutionAlgorithm<TGenome> : NeatEvolutionAlgorithm<TGenome>
        where TGenome : class, global::SharpNeat.Core.IGenome<TGenome>
    {
        public String Name { get; protected set; }
        public int Id { get; private set; }
        public TGenome BestChampion { get; set; }
        public List<TGenome>[] ParasiteGenomes { get; set; }

        public int NextParasite;

        public ModuleNeatEvolutionAlgorithm(String name, int id,
            NeatEvolutionAlgorithmParameters evolutionAlgorithmParameters,
            ISpeciationStrategy<TGenome> speciationStrategy,
            IComplexityRegulationStrategy complexityRegulationStrategy) 
            : base(evolutionAlgorithmParameters, speciationStrategy, complexityRegulationStrategy)
        {

            Name = name;
            Id = id;
            NextParasite = 0;
            ParasiteGenomes = new List<TGenome>[MusicEnvironment.MODULE_COUNT-1];
            for (int i = 0; i < ParasiteGenomes.Length; i++)
            {
                ParasiteGenomes[i] = new List<TGenome>();
            }
        }

        public void UpdateParasiteGenomeList(int senderId, List<TGenome> genomes)
        {
            int position = senderId > Id ? senderId - 2 : senderId - 1;
            ParasiteGenomes[position] = genomes;
        }

        public void Reset()
        {
            _genomeFactory = null;
            _genomeList = null;
        }
    }
}
