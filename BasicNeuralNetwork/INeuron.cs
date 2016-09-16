using System.Collections.Generic;

namespace BasicNeuralNetwork {
    public interface INeuron {
        string Description { get; set; }
        double Value { get; set; }
        IList<Connection> Parents { set; get; }
        double Compute();
    }
}