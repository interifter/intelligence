using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicNeuralNetwork {
    public class OutputNeuron : INeuron {

        public OutputNeuron() {
            Parents = new List<Connection>();
        }

        public string Description {
            set;
            get;
        }

        public IList<Connection> Parents {
            set; get;
        }

        public double Constant {
            set; get;
        }

        public double Value {
            set;
            get;
        }

        public void Train(double desired) {
            double guess = Compute();
            double error = desired - guess;
            for(int i = 0; i < Parents.Count; i++) {
                Parents[i].Weight += Constant * error * Parents[i].Neuron.Value;
            }
        }

        public double Compute() {
            var sum = 0.0;

            foreach(var connection in Parents) {
                sum += connection.Weight * connection.Neuron.Value;
            }

            if(sum < 0) {
                return -1;
            }
            else if (sum > 0) {
                return 1;
            }
            else {
                return 0;
            }

        }
    }
}
