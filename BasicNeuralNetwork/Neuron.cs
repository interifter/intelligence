using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicNeuralNetwork {
    public class Neuron : INeuron {

        public Neuron() {
            Parents = new List<Connection>();
        }

        public string Description { set; get; }

        public IList<Connection> Parents {
            set; get;
        }

        /// <summary>
        ///     The value of the input neuron.
        /// </summary>
        public double Value { get; set; }

        /// <inheritdoc />
        public double Compute() {
            return Value;
        }
    }
}
