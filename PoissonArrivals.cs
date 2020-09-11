using System;


namespace QueueCalcs
{
    class PoissonArrivals
    {
        
        public static int CalculateArrivals(float arrivalsPerHour, Random randNum)
        {
            float VehArrivalsPerSec = arrivalsPerHour / 3600;
            int TimeIntervalSec = 60;  //i.e., 1 minute
            float VehArrivalsPerMin = VehArrivalsPerSec * TimeIntervalSec;

            ulong FactorialResult = 1;
            ulong NumArrivalsPerMin = 0;
            double CumPoisMin = Math.Exp(-VehArrivalsPerMin);   //Probability of zero arrivals
            double r = randNum.NextDouble();

            while (CumPoisMin <= r)
            {
                NumArrivalsPerMin = NumArrivalsPerMin + 1;
                FactorialResult = FactorialResult * NumArrivalsPerMin;
                CumPoisMin = CumPoisMin + (Math.Exp(-VehArrivalsPerMin) * Math.Pow(VehArrivalsPerMin, NumArrivalsPerMin) / FactorialResult);
            }

            //System.Diagnostics.Debug.WriteLine("Time Index: " + timeIndex.ToString() + "; Arrivals/Min: " + NumArrivalsPerMin);
            System.Diagnostics.Debug.WriteLine(NumArrivalsPerMin);

            return (int)NumArrivalsPerMin;
        }

        public static int CalculateArrivalsPerCycle(float arrivalsPerCycle, Random randNum)
        {
            //float VehArrivalsPerSec = arrivalsPerCycle / timingStageDurationSec;
            //int TimeIntervalSec = 1;
            //float VehArrivalsPerMin = VehArrivalsPerSec * TimeIntervalSec;

            ulong FactorialResult = 1;
            ulong NumArrivalsPerCycle = 0;
            double CumPoisCycle = Math.Exp(-arrivalsPerCycle);   //Probability of zero arrivals
            double r = randNum.NextDouble();

            while (CumPoisCycle <= r)
            {
                NumArrivalsPerCycle = NumArrivalsPerCycle + 1;
                FactorialResult = FactorialResult * NumArrivalsPerCycle;
                CumPoisCycle = CumPoisCycle + (Math.Exp(-arrivalsPerCycle) * Math.Pow(arrivalsPerCycle, NumArrivalsPerCycle) / FactorialResult);
            }

            //System.Diagnostics.Debug.WriteLine("Time Index: " + timeIndex.ToString() + "; Arrivals/Min: " + NumArrivalsPerMin);
            System.Diagnostics.Debug.WriteLine(NumArrivalsPerCycle);

            return (int)NumArrivalsPerCycle;
        }

    }
}
