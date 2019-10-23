using System;
using SharpNeat.Phenomes;

namespace NeatMusic
{
    public class NeatPlayer
    {
        //private IBlackBox brain; //only rhythm for now

        public IBlackBox Brain { get; set; }

        public NeatPlayer(IBlackBox blackBox)
        {
            Brain = blackBox;
        }

        /// <summary>
        /// Gets the next move as dictated by the neural network.
        /// </summary>
        public double calculateOutput(double[] inputs)
        {
            // Clear the network
            Brain.ResetState();

            // Convert the game board into an input array for the network
            setInputSignalArray(Brain.InputSignalArray, inputs);

            // Activate the network
            Brain.Activate();


            return Brain.OutputSignalArray[0];
        }

        // Loads the board into the input signal array.
        // This just flattens the 2d board into a 1d array.
        private void setInputSignalArray(ISignalArray inputArr, double[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                inputArr[i] = inputs[i];
            }
        }

    }
}