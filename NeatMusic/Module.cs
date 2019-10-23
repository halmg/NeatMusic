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
    class Module
    {
        public String Name { get; protected set; }
        public int Id { get; private set; } 

        public List<Module> OtherModules { get; protected set; }

        private MusicListModulesEvaluator<NeatGenome, IBlackBox> _genomeRhythmModulesEvaluator;
        private MusicListModulesEvaluator<NeatGenome, IBlackBox> _genomePitchModulesEvaluator;
        
        public ModuleNeatEvolutionAlgorithm<NeatGenome> RhythmEvolutionAlgorithm { get; protected set; }
        public ModuleNeatEvolutionAlgorithm<NeatGenome> PitchEvolutionAlgorithm { get; protected set; }

        private const int GENERATION_ZERO = 0;

        public Module(int id, NeatEvolutionAlgorithmParameters evolutionAlgorithmParameters, 
            ISpeciationStrategy<NeatGenome> speciationStrategyPitch, ISpeciationStrategy<NeatGenome> speciationStrategyRhythm,
            IComplexityRegulationStrategy complexityRegulationStrategyPitch, IComplexityRegulationStrategy complexityRegulationStrategyRhythm)
        {
            Name = "M"+id;
            Id = id;
            //RhythmEvolutionAlgorithm = new NeatEvolutionAlgorithm<NeatGenome>(evolutionAlgorithmParameters, speciationStrategyRhythm, complexityRegulationStrategyRhythm);
            //PitchEvolutionAlgorithm = new NeatEvolutionAlgorithm<NeatGenome>(evolutionAlgorithmParameters, speciationStrategyPitch, complexityRegulationStrategyPitch);
            RhythmEvolutionAlgorithm = new ModuleNeatEvolutionAlgorithm<NeatGenome>(Name + "_R", Id, evolutionAlgorithmParameters, speciationStrategyRhythm, complexityRegulationStrategyRhythm);
            PitchEvolutionAlgorithm = new ModuleNeatEvolutionAlgorithm<NeatGenome>(Name + "_P", Id, evolutionAlgorithmParameters, speciationStrategyPitch, complexityRegulationStrategyPitch);
        }
        
        public void SetParasiteModules(List<Module> otherModules, int parasitesCount, int championsCount, IGenomeDecoder<NeatGenome, IBlackBox> neatGenomeDecoderPitch, IGenomeDecoder<NeatGenome, IBlackBox> neatGenomeDecoderRhythm)
        {
            
            OtherModules = otherModules;
            _genomeRhythmModulesEvaluator = new MusicListModulesEvaluator<NeatGenome, IBlackBox>(
                Name + "_R", parasitesCount, championsCount, RhythmEvolutionAlgorithm,
                otherModules.Select(m => m.RhythmEvolutionAlgorithm).ToList(),
                neatGenomeDecoderRhythm, new RhythmEvaluator());
            _genomePitchModulesEvaluator = new MusicListModulesEvaluator<NeatGenome, IBlackBox>(
                Name + "_P", parasitesCount, championsCount, PitchEvolutionAlgorithm,
                otherModules.Select(m => m.PitchEvolutionAlgorithm).ToList(),
                neatGenomeDecoderPitch, new PitchEvaluator());
        }

        public void Initialize(IGenomeFactory<NeatGenome> genomeFactory1, IGenomeFactory<NeatGenome> genomeFactory2, int populationSize) 
        {
            // Make sure a parasite module has been attached.
            Debug.Assert(OtherModules != null);

            RhythmEvolutionAlgorithm.Initialize(_genomeRhythmModulesEvaluator, genomeFactory1, genomeFactory1.CreateGenomeList(populationSize, GENERATION_ZERO));
            PitchEvolutionAlgorithm.Initialize(_genomePitchModulesEvaluator, genomeFactory2, genomeFactory2.CreateGenomeList(populationSize, GENERATION_ZERO));

            RhythmEvolutionAlgorithm.UpdateScheme = new UpdateScheme(1);
            PitchEvolutionAlgorithm.UpdateScheme = new UpdateScheme(1);
        }
    }
}
