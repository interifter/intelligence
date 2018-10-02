using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleEvolutionaryAlgorithm;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BrainFk;

namespace SimpleEvolutionaryAlgorithm.Test {
    [TestClass]
    public partial class GenomeTest {

        [TestMethod]
        public void ConstructorTest() {
            int length = 10;
            Genome<int> g1 = new Genome<int>(length) {
                Fitness = 1
            };

            Genome<int> g2 = new Genome<int>() {
                Length = length,
                Genes = new int[length],
                Fitness = 1
            };

            Assert.IsTrue(g1.Equals(g2));


        }

        [TestMethod]
        public void CompareToTest() {

            Genome<int> a = new Genome<int>() { Fitness = 0 };
            Genome<int> b = new Genome<int>() { Fitness = 0 };
            object c = new Genome<int>() { Fitness = 0 };
            object d = new Genome<int>() { Fitness = 1 };

            //
            //Basic comparisons.
            //
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.CompareTo(b), 0);
            Assert.AreEqual(b.CompareTo(a), 0);
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(a));
            Assert.IsTrue(a == b);
            Assert.IsTrue(b == a);
            Assert.IsFalse(a != b);
            Assert.IsFalse(b != a);

            //
            //Inheritance check.
            //
            Assert.AreEqual(a, c);
            Assert.IsTrue(a == c);

            //
            //Ensure items with different fitness values are not equal.
            //
            Assert.IsFalse(a == d);
            Assert.IsTrue(a != d);
        }

        [TestMethod]
        public void CrossoverTest() {
            int length = 10;
            int childCount = 20;
            Genome<int> g1 = new Genome<int>(length);
            Genome<int> g2 = new Genome<int>(length);

            var child = g1.Crossover(g2, 1).Single();
            Assert.IsTrue(child != null);

            var children = g1.Crossover(g2, childCount);
            Assert.IsTrue(children.Count == childCount);
            foreach (var item in children) {
                Assert.IsNotNull(item);
            }

        }

        [TestMethod]
        public void GenerateGenomeTest() {
            var g1 = Genome<double>.Generate(() => { return new Random().NextDouble(); }, 20, 0.01);
            var g2 = Genome<string>.Generate(() => { return ((char)DateTime.Now.Minute).ToString(); }, 20, 0.01);
            Assert.IsNotNull(g1);
            Assert.IsNotNull(g2);

        }

        [TestMethod]
        public void GeneratePoolTest() {
            int poolsize = 100;
            int genesize = 2;
            var genomes = new List<Genome<double>>();
            var overallFitness = 0.0;
            var target = 0;
            var generations = 10;
            Genome<double> best = null;
            for (int i = 0; i < poolsize; i++) {
                var g = Genome<double>.Generate(() => { return new Random(DateTime.Now.Millisecond).NextDouble(); }, genesize, 0.01);
                g.Fitness = 1.0 / Function(g.Genes[0], g.Genes[1]);
                if (best == null) {
                    best = g;
                }

                if (g.Fitness > best.Fitness) {
                    best = g;
                }

                overallFitness += g.Fitness;
                g.Fitness = overallFitness;
                genomes.Add(g);
            }


            //
            Assert.IsNotNull(genomes);
            Assert.IsNotNull(best);


        }

        [TestMethod]
        public void GaTest() {
            GA<double, double> ga = new GA<double, double>() {
                CrossoverRate = 0.7,
                Generations = 100,
                GeneSize = 1,
                MutationRate = .01,
                PoolSize = 100,
            };
        }

        public char RandomFunction(int? seed = null) {
            for (int i = 0; i < 2500000; i++) { }
            char[] validChars = new[] { '>', '<', '+', '-', '.', ',', '[', ']' };
            Random r;
            if (seed.HasValue) {
                r = new Random(seed.Value);
            }
            else {
                r = new Random();
            }

            return validChars[r.Next(0, validChars.Length)];
        }



        [TestMethod]
        public void BrainFkFitnessTestForTwoPlusThree() {
            int poolsize = 10;
            int geneStartsize = 100;
            var genomes = new List<Genome<char>>();
            double overallFitness = 0.0;
            int generations = 10;
            Genome<string> best = null;
            double expectedResult = 5.0;
            string fileName = @"C:\Code\populateBfResults.txt";
            //initialize population
            int i = 0;
            while (genomes.Count < poolsize) {
                var g = Genome<char>.Generate(() => RandomFunction(), new Random().Next(i % 3, geneStartsize), 0.01);
                //for (int k = 0; k < 2000; k++) { }
                double fitness = 0.0;
                try {
                    var result = Interpreter.Interpret(string.Join("", g.Genes), 30, (byte)2, (byte)3);
                    g.Fitness = (double)result[0] / expectedResult; //The closer to 1, the better the solution.
                    fitness = g.Fitness;
                }
                catch { //This will likely happen a lot 
                }

                if(fitness > 0)
                    genomes.Add(g);

                i++;
            }

            if(!File.Exists(fileName)) {
                using (StreamWriter sw = new StreamWriter(@"C:\Code\populateBfResults.txt", true)) {
                    sw.Write("Iterations(i-val)");
                    for (int w = 0; w < poolsize; w++) {
                        sw.Write($"\tFormula {w}\tFitness {w}");
                    }
                    sw.WriteLine();
                }
            }

            //Run 1: 328 generations just to populate.
            using(StreamWriter sw = new StreamWriter(fileName, true)) {
                sw.Write($"{i}");
                for (int w = 0; w < poolsize; w++) {
                    sw.Write($"\t{string.Join("", genomes[w].Genes)}\t{genomes[w].Fitness}");
                   
                }
                sw.WriteLine();
            }


            
        }

        [TestMethod]
        public void FitnessTest() {
            int poolsize = 100;
            int genesize = 1;
            var genomes = new List<Genome<double>>();
            var overallFitness = 0.0;
            
            var generations = 10;
            List<double> data = new List<double>();
            using(StreamReader sr = new StreamReader("Resources\\Book2.csv")) {
                string line = "";
                while((line = sr.ReadLine()) != null) {
                    data.Add(double.Parse(line));
                }
            }

            var target = 0.4975624267;
            var targetFitness = ExponentialFitness(data, ExponentialPdf, target);

            Genome<double> best = null;

            for (int i = 0; i < poolsize; i++) {
                var g = Genome<double>.Generate(() => { return new Random(DateTime.Now.Millisecond).NextDouble(); }, genesize, 0.1);
                g.Fitness = ExponentialFitness(data, ExponentialPdf, g.Genes);
                
                if (best == null) {
                    best = g.Copy();
                }

                if (g.Fitness > best.Fitness) {
                    best = g.Copy();
                }
                    overallFitness += g.Fitness;
                
                g.Fitness = overallFitness;
                genomes.Add(g);
            }

            for (int i = 0; i < generations; i++) {
                var newGen = new List<Genome<double>>();
                newGen.Add(best);
                var newOverallFitness = best.Fitness;

                while (newGen.Count < 100) {
                    var v1 = new Random(DateTime.Now.Millisecond).NextDouble() * overallFitness;
                    var v2 = new Random(DateTime.Now.Millisecond).NextDouble() * overallFitness;

                    if (v1 > overallFitness || v2 > overallFitness) {
                        throw new ArgumentOutOfRangeException("Lookup value is greater than Total Fitness.");
                    }
                    var g1 = genomes.First(t => t.Fitness >= v1);
                    var g2 = genomes.First(t => t.Fitness >= v2);
                    while (g1.Equals(g2)) {
                        v2 = new Random(DateTime.Now.Millisecond).NextDouble() * overallFitness;
                        if (v2 > overallFitness) {
                            throw new ArgumentOutOfRangeException("Lookup value is greater than Total Fitness.");
                        }
                        g2 = genomes.First(t => t.Fitness >= v2);

                    }

                    var offspring = g1.Crossover(g2, new Random().Next(1, 3)).ToList();
                    for(int k = 0; k < offspring.Count; k++) {
                      offspring[k].Fitness = ExponentialFitness(data, ExponentialPdf, offspring[k].Genes);

                        if (offspring[k].Fitness > best.Fitness) {
                            best = offspring[k].Copy();
                        }
                            newOverallFitness += offspring[k].Fitness;
                        

                        offspring[k].Fitness = newOverallFitness;
                    }
                    newGen.AddRange(offspring);
                }

                overallFitness = newOverallFitness;
                genomes.Clear();
                genomes.AddRange(newGen);


            }


        }

        public double ExponentialFitness(ICollection<double> data, Func<double, double[], double> func, params double[] parameters) {
            double sum = 0.0;
            for(int i = 0; i < data.Count; i++) {
                var p0 = func(i + 1, parameters);
                var p1 = data.ElementAt(i) - p0;
                var p2 = Math.Pow(p1, 2);
                var p3 = Math.Sqrt(p2);
                sum += Math.Pow(data.ElementAt(i) - func(data.ElementAt(i), parameters), 2);
            }


            return 1.0 / sum;
        }

        public double ExponentialPdf(double x, params double[] parameters) {
            double theta = parameters[0];
            return Math.Exp(-x / theta) / theta;

        }

        public double Function(double x, double y) {
            return (3 * (x + 1) / (y + 1)) + y;
        }
    }
}
