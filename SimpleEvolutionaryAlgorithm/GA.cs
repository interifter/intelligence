using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEvolutionaryAlgorithm {
    public class GA<TGenome, TResult> {
        public int PoolSize { set; get; }
        public int GeneSize { set; get; }
        public int Generations { set; get; }
        public double CrossoverRate { set; get; }
        public double MutationRate { set; get; }
        public List<Genome<TGenome>> Genomes { set; get; }
        public Func<ICollection<TGenome>, Func<TGenome, ICollection<TGenome>, TResult>, ICollection<TGenome>> FitnessFunction { set; get; }

        private Genome<TGenome> best;

        public GA() {
            if (Genomes == null) {
                Genomes = new List<Genome<TGenome>>();
            }

            best = null;
        }

        public ICollection<Genome<TGenome>> InitializePopulation() {
            return new List<Genome<TGenome>>();
        }



    }
}
