using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicNeuralNetwork {
    public class Trainer {

        public double[] inputs;
        public double answer;

        public Trainer(double x, double y, double signature, double a) {
            inputs = new double[4];
            inputs[0] = x;
            inputs[1] = y;
          
            inputs[2] = 1;
            inputs[3] = signature;
            answer = a;
        }

        
    }
}
