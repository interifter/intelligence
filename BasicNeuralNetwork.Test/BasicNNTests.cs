using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;

namespace BasicNeuralNetwork.Test {
    [TestClass]
    public class BasicNNTests {
        double max = 0.0;
        [TestMethod]
        public void NNTesting() {
            var count = 1000;
            Func<double, double> fA = funcA;
            Func<double, double> fB = funcB;
            Func<double, double> fC = funcC;
            max = Math.Max(fA.GetHashCode(), Math.Max(fB.GetHashCode(), fC.GetHashCode()));

            List<Connection> connections = new List<Connection>() {
                new Connection() {
                    Neuron = new InputNeuron() {
                        Description = "X",
                        Value = 0
                    },
                    Weight = new Random().NextDouble()
                },
                new Connection() {
                    Neuron = new InputNeuron() {
                        Description = "Y",
                        Value = 0
                    },
                    Weight = new Random().NextDouble()
                },
                new Connection() {
                    Neuron = new InputNeuron() {
                        Description = "Signature",
                        Value = 0
                    },
                    Weight = new Random().NextDouble()
                },
                new Connection() {
                    Neuron = new InputNeuron() {
                        Description = "Bias",
                        Value = 1
                    },
                    Weight = 1
                }
            };

            OutputNeuron perceptron = new OutputNeuron() {
                Constant = 0.001,
                Description = "Output",
                Parents = connections,
                Value = 0
            };

            var trainers = GenerateTrainingSet(funcA, count, 500, 500);
            var trainers2 = GenerateTrainingSet(funcA, count, -250, 250, -250, 250);
            var trainers3 = GenerateTrainingSet(funcB, count, 500, 500);
            var trainers4 = GenerateTrainingSet(funcB, count, -250, 250, -250, 250);
            var trainers5 = GenerateTrainingSet(funcC, count, -500, 500, -500, 500);
            var minIts = 3;
            var val = 0.0;
            //while(maxIts-- > 0 && val < .94) {
            //    val = RunTrial(count, perceptron, trainers4);
            //}
            double trial1, trial2, trial3, trial4, trial5;
            do {
                trial1 = RunTrial(count, perceptron, trainers);
                trial2 = RunTrial(count, perceptron, trainers3);
                trial3 = RunTrial(count, perceptron, trainers2);
                trial4 = RunTrial(count, perceptron, trainers3);
                trial5 = RunTrial(count, perceptron, trainers5);
                minIts--;
            } while ((trial1 < .94 && trial2 < .94 && trial3 < .94 && trial4 < .94 && trial5 < .94) || minIts > 0);
            
            
            
            

        }
        private static readonly IntPtr PseudoHandle = (IntPtr)(-2);

        public double RunTrial(int count, OutputNeuron perceptron, List<Trainer> trainers) {
            var passRate = 0.0;
            for (int i = 0; i < count; i++) {
                for (int j = 0; j < perceptron.Parents.Count; j++) {
                    perceptron.Parents[j].Neuron.Value = trainers[i].inputs[j];
                }
                double guess = perceptron.Compute();
                Debug.Write($"Coords: {trainers[i].inputs[0]}, {trainers[i].inputs[1]}. ");
                if (trainers[i].answer == -1) {
                    Debug.Write(" Is Below.");
                }
                else {
                    Debug.Write(" Is Above.");
                }

                if (guess == -1) {
                    Debug.Write(" Guess Below. ");
                }
                else {
                    Debug.Write(" Guess Above. ");
                }

                if (guess == trainers[i].answer) {
                    Debug.WriteLine("Correct Guess");
                    passRate++;
                }
                else {
                    Debug.WriteLine("Incorrect Guess");
                }

                perceptron.Train(trainers[i].answer);
            }

            Debug.WriteLine($"Pass Rate: {passRate / count}");
            return passRate / count;
        }

        public List<Trainer> GenerateTrainingSet(Func<double, double> func, int count, int minX, int maxX, int minY, int maxY) {
            List<Trainer> trainers = new List<Trainer>();

            int idleProcIndex = 0;
            for (int i = 0; i < count; i++) {
                double xa = new Random((int)IdleProcessors()[idleProcIndex++] + (int)ThreadTime(new SafeWaitHandle(PseudoHandle, true))).NextDouble() * minX;
                double xb = new Random((int)IdleProcessors()[idleProcIndex++] - (int)ThreadTime(new SafeWaitHandle(PseudoHandle, true))).NextDouble() * maxX;
                double x = xa + xb;
                double ya = new Random((int)IdleProcessors()[idleProcIndex++] + (int)ThreadTime(new SafeWaitHandle(PseudoHandle, true))).NextDouble() * minY;
                double yb = new Random((int)IdleProcessors()[idleProcIndex++] - (int)ThreadTime(new SafeWaitHandle(PseudoHandle, true))).NextDouble() * maxY;
                double y = ya + yb;
                double ans = 1;
                if (y < func(x)) {
                    ans = -1;
                }

                trainers.Add(new Trainer(x, y, ((double)func.GetHashCode()) / max, ans));

                if (idleProcIndex >= 8) {
                    idleProcIndex = 0;
                }
            }

            return trainers;
        }

        public List<Trainer> GenerateTrainingSet(Func<double, double> func, int count, int width, int height) {
            return GenerateTrainingSet(func, count, 0, width, 0, height);
        }

        [CLSCompliant(false)]
        public static UInt64[] IdleProcessors() {
            Int32 byteCount = Environment.ProcessorCount;
            UInt64[] cycleTimes = new UInt64[byteCount];
            byteCount *= 8;   // Size of UInt64
            if (!QueryIdleProcessorCycleTime(ref byteCount, cycleTimes))
                throw new Win32Exception();
            return cycleTimes;
        }


        [CLSCompliant(false)]
        public static UInt64 ThreadTime(SafeWaitHandle threadHandle) {
            UInt64 cycleTime;
            if (!QueryThreadCycleTime(threadHandle, out cycleTime))
                throw new Win32Exception();
            return cycleTime;
        }

        [DllImport("Kernel32", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean QueryIdleProcessorCycleTime(ref Int32 byteCount, UInt64[] CycleTimes);

        [DllImport("Kernel32", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean QueryThreadCycleTime(SafeWaitHandle threadHandle, out UInt64 CycleTime);



        public double funcA(double x) {
            return 2.0 * x + 1.0;
        }

        public double funcB(double x) {
            return x * x - x;
        }

        public double funcC(double x) {
            return -funcA(x);
        }
    }

    
}
