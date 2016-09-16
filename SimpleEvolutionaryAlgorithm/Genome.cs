using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEvolutionaryAlgorithm {
    public class Genome<T> : IComparable, IEquatable<Genome<T>> {

        public double Fitness { set; get; }
        public double MutationRate { set; get; }
        public T[] Genes { set; get; }
        public int Length { set; get; }

        public Genome() {

        }

        public Genome(int length) {
            Genes = new T[length];
            Length = length;
            
        }

        public Genome<T> Copy() {
            return new Genome<T>() {
                Fitness = Fitness,
                Genes = Genes,
                Length = Length,
                MutationRate = MutationRate
            };
        }

        public static Genome<T> Generate(Func<T> randomizer, int length, double mutationRate) {
            List<T> genes = new List<T>();
            for(int i = 0; i < length; i++) {
                genes.Add(randomizer());
            }

            return new Genome<T>() {
                Genes = genes.ToArray(),
                Fitness = 0,
                Length = length,
                MutationRate = mutationRate
            };
        }

        public void Mutate(Func<T> randomizer) {
            for (int i = 0; i < Length; i++) {
                if (new Random().NextDouble() < MutationRate) {
                    this.Genes[i] = randomizer();
                }
            }
        }


        public ICollection<Genome<T>> Crossover(Genome<T> parent, int children) {
            Genome<T>[] collection = new Genome<T>[children];

            int[] steps = new int[children];
            for (int i = 0; i < children; i++) {
                collection[i] = new Genome<T>(Length);
                steps[i] = (int)Math.Round((new Random().NextDouble() * Length));
            }

            steps = steps.OrderBy(g => g).ToArray();

            for (int i = 0; i < children; i++) {
                for (int j = 0; j < Length; j++) {
                    if(j < steps[i]) {
                        collection[i].Genes[j] = this.Genes[j];
                    }
                    else {
                        collection[i].Genes[j] = parent.Genes[j];
                    }
                }
            }

            return collection;
        }

        public int CompareTo(object obj) {
            if (!(obj is Genome<T>)) {
                throw new ArgumentException($"Object of type {obj.GetType().FullName} is not of type {this.GetType().FullName}");
            }
            else {
                return this.Fitness.CompareTo(((Genome<T>)obj).Fitness);
            }

        }

        public bool Equals(Genome<T> obj) {
            return Equals(obj as object);
        }

        public override bool Equals(object obj) {
            if(obj == null || !(obj is Genome<T>)) {
                return false;
            }

            int compareValue = this.CompareTo(obj);
            
            if (compareValue == 0) {
                return true;
            }
            else {
                return false;
            }

        }

        public override int GetHashCode() {
            return base.GetHashCode() ^ (int)(Fitness * 100);
        }

        //public static bool operator ==(Genome<T> a, object b) {
            
        //    return a.Equals(b);
        //} 

        //public static bool operator !=(Genome<T> a, object b) {
        //    return !(a == b);
        //}
    }
}
