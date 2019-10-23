using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using log4net.Config;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System.Threading.Tasks; 
using System.Threading;
using System.Diagnostics;
using SharpNeat.Core;

namespace NeatMusic
{
    class Program
    {
        static void Main(string[] args)
        {

            bool createMany = false;
            IAnnGenerator generator = createMany ? (IAnnGenerator) new MultipleRunsGenerator() : (IAnnGenerator) new SingleRunGenerator();
            generator.Start();
            Console.ReadLine();
        }
    }
}
