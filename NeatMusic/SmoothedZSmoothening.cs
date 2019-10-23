using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
Algorithm for properly detecting peaks in real time
see: https://stackoverflow.com/questions/22583391/peak-signal-detection-in-realtime-timeseries-data#22640362

    */

namespace Keyboard
{
    public class SmoothedZSmoothening
    {

        int _lag;
        double _threshold;
        double _influence;

        Queue<double> filteredY = new Queue<double>();
        double avgFilter;
        double stdFilter;
        bool initialized = false;

        public SmoothedZSmoothening(int lag, double threshold, double influence)
        {
            _lag = lag;
            _threshold = threshold;
            _influence = influence;
        }

        // send each signal, with each of them we save it and update the avg and stdev
        public int isPeak(double rawValue)
        {
            // if we don't have enough values to fit in the specified lag we add value to queue
            if (filteredY.Count < _lag)
            {
                filteredY.Enqueue(rawValue);
                return 0;
            }
            else
            {
                int result = 0;

                // check if first values are initialized, this is necessary as we can only initialize the variables when we have "lag" amount of values
                if (!initialized)
                {
                    avgFilter = filteredY.Average();
                    stdFilter = StandardDeviation(filteredY);

                    initialized = true;
                }
                // keep only the amount of elements that make sense considering the lag value
                while (filteredY.Count > _lag)
                {
                    filteredY.Dequeue();
                }
                // if a signal is found
                if (Math.Abs(rawValue-avgFilter) > _threshold * stdFilter)
                {
                    if (rawValue > avgFilter)
                    {
                        // there is a positive peak
                        result = 1;
                    }
                    else
                    {
                        // there is a negative peak, as we only care about positive we return false as well
                        result = -1;
                    }
                    // adjust filters
                    filteredY.Enqueue(_influence * rawValue + (1 - _influence) * filteredY.ElementAt(_lag-1));
                    avgFilter = filteredY.Average();
                    stdFilter = StandardDeviation(filteredY);
                }
                // if no signal is found
                else
                {
                    result = 0;
                    // adjust filters 
                    filteredY.Enqueue(rawValue);
                    avgFilter = filteredY.Average();
                    stdFilter = StandardDeviation(filteredY);
                }

                return result;
            }
        }

        public static double StandardDeviation(IEnumerable<double> values)
        {
            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }
    }
}
