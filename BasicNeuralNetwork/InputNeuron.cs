using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicNeuralNetwork {
    public class InputNeuron : INeuron {
        public string Description { set; get; }

        public IList<Connection> Parents {
            set;
            get;
        }

        public double Value { set; get; }

        public double Compute() {
            return Value;
        }
    }
}
