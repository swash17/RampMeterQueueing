using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using QueueCalcs.DataStructures;


namespace QueueCalcs
{

    public class QueueLength
    {
        public static void CalculateQueLength(InterchangeData interchange)
        {

            AnalysisData Analysis = new AnalysisData();

            //revise to include explict queue storage between meter stop bar to intermediate and advance queue detectors
            //also separate queue storage by left turns and right turns

            foreach (IntersectionData intersection in interchange.Intersections)
            {
                intersection.OnRamp.QueuingResults = new QueuingAnalysisResults();
                //Number of vehicles that can be stored in shared queue storage distance
                intersection.OnRamp.QueueStorageCapacityVeh[(int)QueueStorage.Shared] = intersection.OnRamp.QueueStorageLaneFt[(int)QueueStorage.Shared] / interchange.Vehicles.AvgVehicleSpacingFt;
            }

            Random randNum = new Random();
            int TimeStepSec = 1;
            //int DepartFlowRateVehPerHr = interchange.intersection.OnRamps[0].Meter.BaseMeteringRateVehPerHr;            
            bool WriteResultsToOutputWindow = false;  //true

            bool IsAdvanceQdetActive = false;
            bool IsIntQdetActive = false;

            List<ResultsData> Results = new List<ResultsData>();
            ResultsData TimeStepResult;
            interchange.Intersections[0].OnRamp.Meter.NumTimeStepsAtMaxMeteringRate = 0;
            interchange.Intersections[0].OnRamp.QueuingResults.DepartFlowRateVehPerHr = interchange.Intersections[0].OnRamp.Meter.BaseMeteringRateVehPerHr;

            int NumCycles = -1;
            int TimingStageIndex = -1;

            //May need to set up separate method to deal with interchanges with no signals (FullClo)

            for (int TimeIndex = 0; TimeIndex < Analysis.AnalysisPeriodSec; TimeIndex++)  //if no signal is present, intersection movement departures = movement arrivals
            {                 
                foreach (IntersectionData intersection in interchange.Intersections)
                {
                    if (intersection.IsSignalControlled == true)
                    {
                        if (TimeIndex % interchange.Intersections[0].Signal.Cycle.LengthSec == 0)
                        {
                            //TimingStageIndex = 0;
                            NumCycles++;
                        }
                    }

                    //intersection.OnRamp.QueuingResults.ArrivalsPerSec[(int)TurnType.All] = 0;
                    intersection.OnRamp.QueuingResults.ArrivalsPerSec = new float[4];
                    //Get arrivals for each movement per cycle
                    foreach (IntersectionMovementData movement in intersection.Movements)  //.Signal.Cycle.TimingStages[TimingStageIndex].Movements)
                    {
                        //Check if movement is signal controlled; if so, use signal timing characteristics to determine outflow rate from intersection to on-ramp
                        if (movement.IsSignalControlled == true)
                        {
                            float RelativeCycleTime = TimeIndex - (NumCycles * intersection.Signal.Cycle.LengthSec);
                            TimingStageIndex = GetTimingStageIndex(RelativeCycleTime, intersection.Signal.Cycle);

                            //update flow rates every cycle
                            //arrivals occur during entire cycle time, departures occur during associated timing stage green time
                            //assume undersaturated, so departures = arrivals, but use sat flow for queue clearance time?
                            if (TimeIndex % interchange.Intersections[0].Signal.Cycle.LengthSec == 0)
                            {                               
                                movement.ArrivalFlowRateVehPerCycle = PoissonArrivals.CalculateArrivalsPerCycle(movement.ArrivalFlowRateVehPerHr / intersection.Signal.Cycle.NumCyclesPerHour, randNum);
                                movement.DepartureFlowRateVehPerSec = movement.ArrivalFlowRateVehPerCycle / intersection.Signal.Cycle.TimingStages[TimingStageIndex].GreenTime;
                            }
                        }
                        else
                        {
                            //if not signal controlled, calculate arrivals on a per-minute basis
                            if (TimeIndex % interchange.Intersections[0].FlowRateUpdateIntervalDefaultSec == 0)
                            {
                                movement.ArrivalFlowRateVehPerMin = PoissonArrivals.CalculateArrivalsPerCycle(movement.ArrivalFlowRateVehPerHr / intersection.FlowRateUpdateIntervalDefaultSec, randNum);
                                movement.DepartureFlowRateVehPerSec = movement.ArrivalFlowRateVehPerMin / 60;
                            }
                        }

                        if (movement.NemaPhaseId == NemaMovementNumbers.WBLeft || movement.NemaPhaseId == NemaMovementNumbers.EBLeft)
                        {
                            intersection.OnRamp.QueuingResults.ArrivalsPerSec[(int)TurnType.Left] += movement.DepartureFlowRateVehPerSec;
                        }
                        else if (movement.NemaPhaseId == NemaMovementNumbers.WBRight || movement.NemaPhaseId == NemaMovementNumbers.EBRight)
                        {
                            intersection.OnRamp.QueuingResults.ArrivalsPerSec[(int)TurnType.Right] += movement.DepartureFlowRateVehPerSec;
                        }
                        intersection.OnRamp.QueuingResults.ArrivalsPerSec[(int)TurnType.All] += movement.DepartureFlowRateVehPerSec;
                    }

                    intersection.OnRamp.QueuingResults.CumulativeArrivals += intersection.OnRamp.QueuingResults.ArrivalsPerSec[(int)TurnType.All];

                    //DepartFlowRateVehPerTimeStep = DepartFlowRateVehPerHr / 3600f * TimeStepSec;
                    intersection.OnRamp.QueuingResults.DepartFlowRateVehPerTimeStep = (float)intersection.OnRamp.QueuingResults.DepartFlowRateVehPerHr / Analysis.AnalysisPeriodSec * TimeStepSec;
                    intersection.OnRamp.QueuingResults.VehServedPerTimeStep = Math.Min(intersection.OnRamp.QueuingResults.ArrivalsPerSec[(int)QueuedVehMovement.Total] + intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total], intersection.OnRamp.QueuingResults.DepartFlowRateVehPerTimeStep);

                    intersection.OnRamp.QueuingResults.CumulativeDepartures += intersection.OnRamp.QueuingResults.VehServedPerTimeStep;

                    //QueueLengthVeh = (Arrivals - Departures) + QueueLengthVeh;
                    intersection.OnRamp.QueuingResults.QueueLengthVehPreviousTimeStep = intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total];
                    intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total] = intersection.OnRamp.QueuingResults.CumulativeArrivals - intersection.OnRamp.QueuingResults.CumulativeDepartures;
                    intersection.OnRamp.QueuingResults.DeltaTimeStepQueueLengthVeh = intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total] - intersection.OnRamp.QueuingResults.QueueLengthVehPreviousTimeStep;

                    //first determine if shared storage is filled up; if so, then calculate how much of left-only and right-only queue storage is filled
                    //assume lane that right turn lane feeds into uses zipper method (i.e., alternating) for merging
                    //total vehicle in queue vs joint queue storage area
                    intersection.OnRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Total] = intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total] * interchange.Vehicles.AvgVehicleSpacingFt;
                    
                    if (intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total] > intersection.OnRamp.QueueStorageCapacityVeh[(int)QueueStorage.Shared])
                    {
                        //What is percent left and percent right vehicles in queue?

                        //queue is spilling back into left-only and/or right-only sections of on-ramp
                        //Left-only
                        //intersection.OnRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Left] = intersection.OnRamp.QueuingResults.NumVehsInQueue[3] * interchange.Vehicles.AvgVehicleSpacingFt;
                    }

                    if (intersection.OnRamp.QueuingResults.DeltaTimeStepQueueLengthVeh > 0)  //Queue is increasing in size
                    {
                        CheckQueueDetectorActivation(out IsAdvanceQdetActive, out IsIntQdetActive, intersection.OnRamp);
                        //first check advance q det
                        //if false, then check int q det

                        if (IsAdvanceQdetActive == true)
                        {
                            intersection.OnRamp.QueuingResults.DepartFlowRateVehPerHr = intersection.OnRamp.Meter.MeteringRateMaxVehPerHr;
                            intersection.OnRamp.Meter.NumTimeStepsAtMaxMeteringRate++;
                        }
                        else if (IsIntQdetActive == true)
                        {
                            intersection.OnRamp.QueuingResults.DepartFlowRateVehPerHr = intersection.OnRamp.Meter.BaseMeteringRateVehPerHr + intersection.OnRamp.Meter.AddMeteringRateVehPerHr;
                        }
                        else
                        {
                            intersection.OnRamp.QueuingResults.DepartFlowRateVehPerHr = intersection.OnRamp.Meter.BaseMeteringRateVehPerHr;
                        }
                    }
                    else  //Queue is decreasing in size
                    {
                        CheckQueueDetectorDeactivation(out IsAdvanceQdetActive, out IsIntQdetActive, intersection.OnRamp);

                        if (IsAdvanceQdetActive == false)
                        {
                            intersection.OnRamp.QueuingResults.DepartFlowRateVehPerHr = intersection.OnRamp.Meter.BaseMeteringRateVehPerHr;
                        }
                        else if (IsAdvanceQdetActive == false)
                        {
                            intersection.OnRamp.QueuingResults.DepartFlowRateVehPerHr = intersection.OnRamp.Meter.BaseMeteringRateVehPerHr + intersection.OnRamp.Meter.AddMeteringRateVehPerHr;
                        }
                        else
                        {
                            intersection.OnRamp.QueuingResults.DepartFlowRateVehPerHr = intersection.OnRamp.Meter.MeteringRateMaxVehPerHr;
                            intersection.OnRamp.Meter.NumTimeStepsAtMaxMeteringRate++;
                        }
                    }

                    intersection.OnRamp.Meter.PctTimeMeteringRateMax = intersection.OnRamp.Meter.NumTimeStepsAtMaxMeteringRate * 100f / (TimeIndex + 1);

                    //intersection.OnRamp.QueuingResults.ArrivalsPerTimeStep[(int)TurnType.Left], intersection.OnRamp.QueuingResults.ArrivalsPerTimeStep[(int)TurnType.Thru], intersection.OnRamp.QueuingResults.ArrivalsPerTimeStep[(int)TurnType.Right], intersection.OnRamp.QueuingResults.ArrivalsPerTimeStep[(int)TurnType.All]

                    TimeStepResult = new ResultsData(TimeIndex, (byte)(TimingStageIndex + 1), intersection.OnRamp.QueuingResults.ArrivalsPerHour[(int)TurnType.All], intersection.OnRamp.QueuingResults.ArrivalsPerSec, intersection.OnRamp.QueuingResults.DepartFlowRateVehPerHr, intersection.OnRamp.QueuingResults.DepartFlowRateVehPerTimeStep, intersection.OnRamp.QueuingResults.VehServedPerTimeStep, intersection.OnRamp.QueuingResults.CumulativeArrivals, intersection.OnRamp.QueuingResults.CumulativeDepartures, intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)TurnType.All], intersection.OnRamp.QueuingResults.QueueLengthFt[(int)TurnType.All], intersection.OnRamp.Meter.PctTimeMeteringRateMax);
                    Results.Add(TimeStepResult);

                    if (WriteResultsToOutputWindow == true)
                    {
                        Debug.WriteLine("Time Index: " + ((double)TimeIndex / 10).ToString("0.0") + "s; Metering Rate: " + intersection.OnRamp.QueuingResults.DepartFlowRateVehPerHr);
                        //Debug.WriteLine("Queue Length: " + QueueLengthVeh.ToString("0.0"));
                        Debug.WriteLine("------------------------");
                    }
                    //}
                }
            }

            string ResultsFilename = @"..\..\..\Results.csv";
            //string ResultsFilename = @"C:\Users\swash\OneDrive - University of Florida\FDOT Interchange Ramp Metering\Task 3-Experimental Design\Results.csv";
            FileInputOutput.WriteResultsData(ResultsFilename, Results);
        }


        //Also check condition where advance queue detector is within shared queue storage area
        private static void CheckQueueDetectorActivation(out bool IsAdvanceQdetActive, out bool IsIntQdetActive, OnRampData onRamp)
        {
            IsAdvanceQdetActive = false;
            IsIntQdetActive = false;

            //Check Advance Q detector first ---------------------------------
            //first check if queue is beyond shared queue storage area
            if (onRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total] > onRamp.QueueStorageCapacityVeh[(int)QueueStorage.Shared])
            {
                //check advance queue detector for both left- and right-turn storage area

                //int QdetectorIndex = intersection.OnRamp.QueueDetectors.FindIndex(detector => detector.Type.Equals(DetectorType.AdvanceQueue));
                for (int DetIndex = 0; DetIndex < onRamp.QueueDetectors.Count; DetIndex++)
                {
                    if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.AdvanceQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Left)
                    {
                        if (onRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Left] > onRamp.QueueDetectors[DetIndex].DistanceLaneFt)
                        {
                            IsAdvanceQdetActive = true;
                        }
                    }
                    if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.AdvanceQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Right)
                    {
                        if (onRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Right] > onRamp.QueueDetectors[DetIndex].DistanceLaneFt)
                        {
                            IsAdvanceQdetActive = true;
                        }
                    }
                }
            }
            else
            {
                for (int DetIndex = 0; DetIndex < onRamp.QueueDetectors.Count; DetIndex++)
                {
                    if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.AdvanceQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.All)
                    {
                        if (onRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Total] > onRamp.QueueDetectors[DetIndex].DistanceLaneFt)
                        {
                            IsAdvanceQdetActive = true;
                        }
                    }                    
                }
            }

            //If Advance Q detector is not activated, then check Intermediate Q detector ---------------------------------
            if (IsAdvanceQdetActive == false)
            {
                if (onRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total] > onRamp.QueueStorageCapacityVeh[(int)QueueStorage.Shared])
                {
                    //check advance queue detector for both left- and right-turn storage area

                    //int QdetectorIndex = intersection.OnRamp.QueueDetectors.FindIndex(detector => detector.Type.Equals(DetectorType.AdvanceQueue));
                    for (int DetIndex = 0; DetIndex < onRamp.QueueDetectors.Count; DetIndex++)
                    {
                        if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.IntermediateQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Left)
                        {
                            if (onRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Left] > onRamp.QueueDetectors[DetIndex].DistanceLaneFt)
                            {
                                IsIntQdetActive = true;
                            }
                        }
                        if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.IntermediateQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Right)
                        {
                            if (onRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Right] > onRamp.QueueDetectors[DetIndex].DistanceLaneFt)
                            {
                                IsIntQdetActive = true;
                            }
                        }
                    }
                }
                else
                {
                    for (int DetIndex = 0; DetIndex < onRamp.QueueDetectors.Count; DetIndex++)
                    {
                        if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.IntermediateQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.All)
                        {
                            if (onRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Total] > onRamp.QueueDetectors[DetIndex].DistanceLaneFt)
                            {
                                IsIntQdetActive = true;
                            }
                        }

                    }
                }
            }

        }


        private static void CheckQueueDetectorDeactivation(out bool IsAdvanceQdetActive, out bool IsIntQdetActive, OnRampData onRamp)
        {
            IsAdvanceQdetActive = true;
            IsIntQdetActive = true;

            //Check Intermediate Q detector first ---------------------------------
            //first check if queue is beyond shared queue storage area
            if (onRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total] > onRamp.QueueStorageCapacityVeh[(int)QueueStorage.Shared])
            {
                //check advance queue detector for both left- and right-turn storage area

                //int QdetectorIndex = intersection.OnRamp.QueueDetectors.FindIndex(detector => detector.Type.Equals(DetectorType.AdvanceQueue));
                for (int DetIndex = 0; DetIndex < onRamp.QueueDetectors.Count; DetIndex++)
                {
                    if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.IntermediateQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Left)
                    {
                        if (onRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Left] < onRamp.QueueDetectors[DetIndex].DistanceLaneFt - 50)
                        {
                            IsIntQdetActive = false;
                        }
                    }
                    if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.IntermediateQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Right)
                    {
                        if (onRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Right] < onRamp.QueueDetectors[DetIndex].DistanceLaneFt - 50)
                        {
                            IsIntQdetActive = false;
                        }
                    }
                }
            }
            else
            {
                for (int DetIndex = 0; DetIndex < onRamp.QueueDetectors.Count; DetIndex++)
                {
                    if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.IntermediateQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.All)
                    {
                        if (onRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Total] > onRamp.QueueDetectors[DetIndex].DistanceLaneFt - 150)
                        {
                            IsIntQdetActive = false;
                        }
                    }                    
                }
            }

            //If Intermediate Q detector is activated, then check Advance Q detector ---------------------------------
            if (IsAdvanceQdetActive == false)
            {
                if (onRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total] > onRamp.QueueStorageCapacityVeh[(int)QueueStorage.Shared])
                {
                    //check advance queue detector for both left- and right-turn storage area

                    //int QdetectorIndex = intersection.OnRamp.QueueDetectors.FindIndex(detector => detector.Type.Equals(DetectorType.AdvanceQueue));
                    for (int DetIndex = 0; DetIndex < onRamp.QueueDetectors.Count; DetIndex++)
                    {
                        if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.AdvanceQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Left)
                        {
                            if (onRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Left] < onRamp.QueueDetectors[DetIndex].DistanceLaneFt - 50)
                            {
                                IsIntQdetActive = false;
                            }
                        }
                        if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.AdvanceQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Right)
                        {
                            if (onRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Right] < onRamp.QueueDetectors[DetIndex].DistanceLaneFt - 50)
                            {
                                IsIntQdetActive = false;
                            }
                        }
                    }
                }
                else
                {
                    for (int DetIndex = 0; DetIndex < onRamp.QueueDetectors.Count; DetIndex++)
                    {
                        if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.AdvanceQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.All)
                        {
                            if (onRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Total] > onRamp.QueueDetectors[DetIndex].DistanceLaneFt - 150)
                            {
                                IsAdvanceQdetActive = false;
                            }
                        }
                    }
                }
            }


        }



        public static int GetTimingStageIndex(float relativeCycleTime, CycleData cycle)
        {
            int TimingStageIndex = -1;

            //foreach (float timingStageStartTime in interchange.Signal.Cycle.TimingStageStartTimes)
            for (int ts = 1; ts <= cycle.TimingStageStartTimes.Length; ts++)
            {
                if (ts == cycle.TimingStageStartTimes.Length)
                {
                    TimingStageIndex = cycle.TimingStageStartTimes.Length - 1;
                    break;
                }
                else if (relativeCycleTime < cycle.TimingStageStartTimes[ts])
                {
                    TimingStageIndex = ts - 1;
                    break;
                }
            }            

            return TimingStageIndex;
        }

    }
}

/*           
int FFS = 88; //ft/s
int initialTimeSec = (int)(PriorToRampLengthFt + RampLengthFt * ControlPointPosition) / FFS;

    double TimeRemainder = timeIndex % (3600 / MeteringRate);
                if (TimeRemainder <= greenInterval)
                {
                    QueueLengthVeh = QueueLengthVeh - (1 / greenInterval);         //This needs to be changed
                    if (QueueLengthVeh < 0)
                        QueueLengthVeh = 0;
                }

*/


//to-do: break departure flow rate into sat flow rate and arrival rate
//if (TimeIndex % inputs.Signal.TimingStageStartTimes[TimingStageIndex] == 0) //Only update at the frequency of the update interval
//{
/*
//for uniform arrivals
if (TimeIndex <= 15)  //900
    ArrivalRateTimePeriodIndex = 0;
else if (TimeIndex <= 30)  //1800
    ArrivalRateTimePeriodIndex = 1;
else if (TimeIndex <= 45)  //2700
    ArrivalRateTimePeriodIndex = 2;
else
    ArrivalRateTimePeriodIndex = 3;
*/
