using System;
using System.Collections.Generic;
using System.Diagnostics;

using Wisp.Nodes;

namespace Wisp
{
    public static class Debug
    {
        public static string Output { get; private set; }

        public static void AddOutput(string output)
        {
            Output += output;
        }

        public static void ClearOutput()
        {
            Output = "";
        }

        public class Sampler
        {
            Stopwatch stopwatch = new Stopwatch();
            List<float> samples = new List<float>();

            public void Start()
            {
                stopwatch.Reset();
                stopwatch.Start();
            }

            public void End()
            {
                stopwatch.Stop();
                AddSample((float)stopwatch.Elapsed.TotalMilliseconds);
            }

            public void AddSample(float sample) => samples.Add(sample);
            
            public float GetAverage()
            {
                float total = 0;
                foreach (var item in samples) total += item;
                float average = total / samples.Count;
                return average;
            }

            public void Clear() => samples.Clear();
        }
    }
}
