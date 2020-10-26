using System;
using System.Collections.Generic;
using System.Diagnostics;
using QueueCalcs.DataStructures;


namespace QueueCalcs
{

    public partial class QueueAnalysis
    {

        static int SharedSegmentIndex = 0;
        static int LeftTurnSegmentIndex = 0;
        static int RightTurnSegmentIndex = 0;

        static bool WriteResultsToOutputWindow = false;  //true


        public static void SingleRampSegment(List<InterchangeIntersectionData> intersections)
        {
            AnalysisData Analysis = new AnalysisData();
            Random randNum = new Random(1234);
            bool IsAdvanceQdetActive = false;
            bool IsIntQdetActive = false;
            int ActiveTimingStageID = 0;

            List<ResultsData> Results = new List<ResultsData>();
            ResultsData TimeStepResult;

            foreach (InterchangeIntersectionData intersection in intersections)
            {
                intersection.OnRamp.Segments[0].Results = new QueuingAnalysisResults();

                intersection.OnRamp.Segments[0].FlowRate.DeparturesVehPerHr = intersection.OnRamp.Meter.BaseRateVehPerHr;
                intersection.OnRamp.Meter.NumTimeStepsAtMaxMeteringRate = 0;
                intersection.OnRamp.Segments[0].QueueStorageCapacityVeh = intersection.OnRamp.Segments[0].QueueStorageLaneFt / intersections[0].Vehicles.AvgVehicleSpacingFt;
            }

            int NumCycles = -1;
            int TimingStageIndex = -1;

            //May need to set up separate method to deal with interchanges with no signals (FullClo)           

            for (int TimeIndex = 0; TimeIndex < Analysis.AnalysisPeriodSec; TimeIndex++)  //if no signal is present, intersection movement departures = movement arrivals
            {
                foreach (InterchangeIntersectionData intersection in intersections)
                {
                    if (intersection.IsSignalControlled == true)
                    {
                        if (TimeIndex % intersections[0].Signal.Cycle.LengthSec == 0)
                        {
                            //TimingStageIndex = 0;
                            NumCycles++;
                        }
                    }

                    if (intersection.IsSignalControlled == true)
                    {
                        float RelativeCycleTime = TimeIndex - (NumCycles * intersection.Signal.Cycle.LengthSec);
                        TimingStageIndex = GetTimingStageIndex(RelativeCycleTime, intersection.Signal.Cycle);
                        ActiveTimingStageID = TimingStageIndex + 1;
                    }

                    intersection.OnRamp.Segments[0].FlowRate.ArrivalsVehPerTimeStep = 0;

                    //Get arrivals and departures for each intersection movement per cycle
                    foreach (IntersectionMovementData movement in intersection.Movements)  //.Signal.Cycle.TimingStages[TimingStageIndex].Movements)
                    {
                        //Check if movement is signal controlled; if so, use signal timing characteristics to determine outflow rate from intersection to on-ramp
                        if (movement.IsSignalControlled == true)
                        {
                            //update intersection arrival flow rates every cycle
                            //arrivals occur during entire cycle time, departures occur during associated timing stage green time
                            //assume undersaturated, so departures = arrivals, but use sat flow for queue clearance time?
                            if (TimeIndex % intersections[0].Signal.Cycle.LengthSec == 0)
                            {
                                movement.FlowRate.ArrivalsVehPerCycle = PoissonArrivals.CalculateArrivalsPerCycle(movement.FlowRate.ArrivalsVehPerHr / intersection.Signal.Cycle.NumCyclesPerHour, randNum);
                                movement.FlowRate.ArrivalsVehPerTimeStep = movement.FlowRate.ArrivalsVehPerCycle / intersection.Signal.Cycle.TimingStages[TimingStageIndex].GreenTime;
                            }
                        }
                        else
                        {
                            //if not signal controlled, calculate arrivals on a per-minute basis
                            if (TimeIndex % intersections[0].FlowRateUpdateIntervalDefaultSec == 0)
                            {
                                movement.FlowRate.ArrivalsVehPerMin = PoissonArrivals.CalculateArrivalsPerCycle(movement.FlowRate.ArrivalsVehPerHr / intersection.FlowRateUpdateIntervalDefaultSec, randNum);
                                movement.FlowRate.ArrivalsVehPerTimeStep = movement.FlowRate.ArrivalsVehPerMin / 60;
                            }
                        }
                    }

                    //Departures from intersection occur for a given movement during the timing stage for which the subject movement is assigned
                    //to-do: do not include lost time
                    intersection.FlowRate.DeparturesVehPerTimeStep = 0;
                    for (int i = 0; i < intersection.Signal.Cycle.TimingStages.Count; i++)
                    {
                        foreach (IntersectionMovementData tsMovement in intersection.Signal.Cycle.TimingStages[i].Movements)
                        {
                            if (i == ActiveTimingStageID - 1)
                                tsMovement.FlowRate.DeparturesVehPerTimeStep = tsMovement.FlowRate.ArrivalsVehPerTimeStep;
                            else
                                tsMovement.FlowRate.DeparturesVehPerTimeStep = 0;

                            intersection.FlowRate.DeparturesVehPerTimeStep += tsMovement.FlowRate.DeparturesVehPerTimeStep;
                        }
                    }

                    intersection.OnRamp.Segments[0].Results.QueueLengthVehPreviousTimeStep = intersection.OnRamp.Segments[0].Results.NumVehsInQueue[(int)QueuedVehMovement.Total];
                    intersection.OnRamp.Segments[0].FlowRate.ArrivalsVehPerTimeStep += intersection.FlowRate.DeparturesVehPerTimeStep;
                    intersection.OnRamp.Segments[0].Results.CumulativeArrivals[(int)QueuedVehMovement.Total] += intersection.OnRamp.Segments[0].FlowRate.ArrivalsVehPerTimeStep;
                    intersection.OnRamp.Segments[0].Results.NumVehsInQueue[(int)QueuedVehMovement.Total] += intersection.OnRamp.Segments[0].FlowRate.ArrivalsVehPerTimeStep;
                    intersection.OnRamp.Segments[0].Results.QueueLengthFt[(int)QueuedVehMovement.Total] = intersection.OnRamp.Segments[0].Results.NumVehsInQueue[(int)QueuedVehMovement.Total] * intersections[0].Vehicles.AvgVehicleSpacingFt;
                    intersection.OnRamp.Segments[0].Results.QueueLengthFtPerLane[(int)QueuedVehMovement.Total] = intersection.OnRamp.Segments[0].Results.QueueLengthFt[(int)QueuedVehMovement.Total] / intersection.OnRamp.Segments[0].NumLanes;
                    intersection.OnRamp.Segments[0].Results.PctQueueStorageOccupied[(int)SegmentType.Shared] = intersection.OnRamp.Segments[0].Results.NumVehsInQueue[(int)QueuedVehMovement.Total] / intersection.OnRamp.Segments[0].QueueStorageCapacityVeh * 100;

                    //Check if vehicles should be discharged from meter stop bar ----------------------------------------------------------
                    intersection.OnRamp.Meter.ReleaseIntervalSec = 3600f / intersection.OnRamp.Meter.CurrentRateVehPerHr * intersection.OnRamp.Segments[0].NumLanes;

                    if (TimeIndex > 0 && TimeIndex % intersection.OnRamp.Meter.ReleaseIntervalSec == 0)
                    {
                        float MeteringRateVehPerTimeStep = (float)intersection.OnRamp.Meter.CurrentRateVehPerHr / Analysis.AnalysisPeriodSec * Analysis.TimeStepsPerSecond;
                        //discharge of vehicles typically takes 2-3 seconds
                        //int MaxNumVehiclesDischarged = 1 * intersection.OnRamp.Segments[0].NumLanes;  //assumes 1 vehicle per green interval, per lane
                        float MaxNumVehiclesDischarged = intersection.OnRamp.Meter.ReleaseIntervalSec * MeteringRateVehPerTimeStep;

                        intersection.OnRamp.Segments[0].Results.VehServedPerTimeStep = Math.Min(intersection.OnRamp.Segments[0].Results.NumVehsInQueue[(int)QueuedVehMovement.Total], MaxNumVehiclesDischarged);
                        //intersection.OnRamp.Segments[0].NumVehiclesQueued -= intersection.OnRamp.Segments[0].Results.VehServedPerTimeStep;
                        intersection.OnRamp.Segments[0].Results.NumVehsInQueue[(int)QueuedVehMovement.Total] -= intersection.OnRamp.Segments[0].Results.VehServedPerTimeStep;
                    }
                    else
                    {
                        intersection.OnRamp.Segments[0].Results.VehServedPerTimeStep = 0;
                    }

                    intersection.OnRamp.Segments[0].Results.CumulativeDepartures += intersection.OnRamp.Segments[0].Results.VehServedPerTimeStep;

                    intersection.OnRamp.Segments[0].Results.NumVehsInQueue[(int)QueuedVehMovement.Total] = intersection.OnRamp.Segments[0].Results.CumulativeArrivals[(int)QueuedVehMovement.Total] - intersection.OnRamp.Segments[0].Results.CumulativeDepartures;
                    intersection.OnRamp.Segments[0].Results.DeltaTimeStepQueueLengthVeh = intersection.OnRamp.Segments[0].Results.NumVehsInQueue[(int)QueuedVehMovement.Total] - intersection.OnRamp.Segments[0].Results.QueueLengthVehPreviousTimeStep;

                    if (intersection.OnRamp.Segments[0].Results.DeltaTimeStepQueueLengthVeh > 0)  //Queue is increasing in size
                    {
                        CheckQueueDetectorActivation(out IsAdvanceQdetActive, out IsIntQdetActive, intersection.OnRamp);
                        UpdateMeteringRate(IsAdvanceQdetActive, IsIntQdetActive, true, intersection.OnRamp);
                    }
                    else if (intersection.OnRamp.Segments[0].Results.DeltaTimeStepQueueLengthVeh < 0)  //Queue is decreasing in size
                    {
                        CheckQueueDetectorDeactivation(out IsAdvanceQdetActive, out IsIntQdetActive, intersection.OnRamp);
                        UpdateMeteringRate(IsAdvanceQdetActive, IsIntQdetActive, false, intersection.OnRamp);
                    }
                    //skip if DeltaTimeStepQueueLengthVeh = 0

                    intersection.OnRamp.Meter.PctTimeMeteringRateMax = intersection.OnRamp.Meter.NumTimeStepsAtMaxMeteringRate * 100f / (TimeIndex + 1);

                    //intersection.OnRamp.Segments[0].Results.QueueLengthFt[(int)TurnType.All]
                    float IntxDeparturesLeftTurn = 0;
                    float IntxDeparturesRightTurn = 0;
                    float IntxDeparturesThru = 0;
                    foreach (IntersectionMovementData tsMovement in intersection.Signal.Cycle.TimingStages[ActiveTimingStageID - 1].Movements)
                    {
                        if (tsMovement.NemaPhaseId == NemaMovementNumbers.WBLeft || tsMovement.NemaPhaseId == NemaMovementNumbers.EBLeft)
                            IntxDeparturesLeftTurn = tsMovement.FlowRate.DeparturesVehPerTimeStep;
                        else if (tsMovement.NemaPhaseId == NemaMovementNumbers.WBRight || tsMovement.NemaPhaseId == NemaMovementNumbers.EBRight)
                            IntxDeparturesRightTurn = tsMovement.FlowRate.DeparturesVehPerTimeStep;
                        else if (tsMovement.NemaPhaseId == NemaMovementNumbers.NBThru || tsMovement.NemaPhaseId == NemaMovementNumbers.SBThru)
                            IntxDeparturesThru = tsMovement.FlowRate.DeparturesVehPerTimeStep;
                    }

                    TimeStepResult = new ResultsData(
                        TimeIndex, 
                        (byte)(TimingStageIndex + 1), 
                        IntxDeparturesLeftTurn, 
                        IntxDeparturesRightTurn, 
                        IntxDeparturesThru, 
                        intersection.FlowRate.DeparturesVehPerTimeStep, 
                        intersection.OnRamp.Meter.CurrentRateVehPerHr, 
                        intersection.OnRamp.Segments[0].Results.VehServedPerTimeStep, 
                        intersection.OnRamp.Segments[0].Results.CumulativeArrivals[(int)QueuedVehMovement.Total], 
                        intersection.OnRamp.Segments[0].Results.CumulativeDepartures, 
                        intersection.OnRamp.Segments[0].Results.NumVehsInQueue,
                        intersection.OnRamp.Segments[0].Results.QueueLengthFt[(int)QueuedVehMovement.Total],
                        intersection.OnRamp.Segments[0].Results.QueueLengthFtPerLane[(int)QueuedVehMovement.Total], 
                        intersection.OnRamp.Segments[0].Results.PctQueueStorageOccupied, 
                        intersection.OnRamp.Meter.PctTimeMeteringRateMax);
                    
                    Results.Add(TimeStepResult);
                    //intersection.Movements[(int)QueuedVehMovement.Left].FlowRate.DeparturesVehPerTimeStep
                }
            }
            WriteResultsToFile(Results);
        }


        public static void MultipleRampSegments(List<InterchangeIntersectionData> intersections)
        {
            /*
            AnalysisData Analysis = new AnalysisData();

            SharedSegmentIndex = interchange.Intersections[0].OnRamp.Segments.FindIndex(segment => segment.Type.Equals(SegmentType.Shared));
            LeftTurnSegmentIndex = interchange.Intersections[0].OnRamp.Segments.FindIndex(segment => segment.Type.Equals(SegmentType.LeftTurn));
            RightTurnSegmentIndex = interchange.Intersections[0].OnRamp.Segments.FindIndex(segment => segment.Type.Equals(SegmentType.RightTurn));

            foreach (IntersectionData intersection in interchange.Intersections)
            {
                intersection.OnRamp.QueuingResults = new QueuingAnalysisResults();

                intersection.OnRamp.QueuingResults.DepartFlowRateVehPerHr = intersection.OnRamp.Meter.BaseRateVehPerHr;
                intersection.OnRamp.Meter.NumTimeStepsAtMaxMeteringRate = 0;

                foreach (OnRampSegmentData RampSegment in intersection.OnRamp.Segments)
                {
                    //Number of vehicles that can be stored in shared queue storage distance
                    RampSegment.QueueStorageCapacityVeh = RampSegment.QueueStorageLaneFt / interchange.Vehicles.AvgVehicleSpacingFt;
                }
            }

            Random randNum = new Random();

            bool IsAdvanceQdetActive = false;
            bool IsIntQdetActive = false;

            List<ResultsData> Results = new List<ResultsData>();
            ResultsData TimeStepResult;

            //interchange.Intersections[0].OnRamp.QueuingResults.DepartFlowRateVehPerHr = interchange.Intersections[0].OnRamp.Meter.BaseRateVehPerHr;

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

                    intersection.OnRamp.QueuingResults.ArrivalsPerTimeStep = new float[4];
                    //Get arrivals and departures for each intersection movement per cycle
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
                                movement.FlowRate.ArrivalsVehPerCycle = PoissonArrivals.CalculateArrivalsPerCycle(movement.FlowRate.ArrivalsVehPerHr / intersection.Signal.Cycle.NumCyclesPerHour, randNum);
                                movement.FlowRate.DeparturesVehPerTimeStep = movement.FlowRate.ArrivalsVehPerCycle / intersection.Signal.Cycle.TimingStages[TimingStageIndex].GreenTime;
                            }
                        }
                        else
                        {
                            //if not signal controlled, calculate arrivals on a per-minute basis
                            if (TimeIndex % interchange.Intersections[0].FlowRateUpdateIntervalDefaultSec == 0)
                            {
                                movement.FlowRate.ArrivalsVehPerMin = PoissonArrivals.CalculateArrivalsPerCycle(movement.FlowRate.ArrivalsVehPerHr / intersection.FlowRateUpdateIntervalDefaultSec, randNum);
                                movement.FlowRate.DeparturesVehPerTimeStep = movement.FlowRate.ArrivalsVehPerMin / 60;
                            }
                        }

                        intersection.OnRamp.QueuingResults.DepartFlowRateVehPerTimeStep += movement.FlowRate.DeparturesVehPerTimeStep;
                        //Cumulative number of arrivals per movement, and total -------------------------------------------------

                        //intersection.OnRamp.QueuingResults.CumulativeArrivals[(int)QueuedVehMovement.Total] += movement.FlowRate.DeparturesVehPerTimeStep;

                        //assign intersection movement departures to corresponding ramp segment input flow rate
                        //if number of segments == 1 (only shared, no separate left/right legs), all intersection movement departures are assigned to single segment                         

                        if (movement.NemaPhaseId == NemaMovementNumbers.WBLeft || movement.NemaPhaseId == NemaMovementNumbers.EBLeft)
                        {
                            intersection.OnRamp.QueuingResults.CumulativeArrivals[(int)QueuedVehMovement.Left] += movement.FlowRate.DeparturesVehPerTimeStep;
                            intersection.OnRamp.Segments[LeftTurnSegmentIndex].FlowRate.ArrivalsVehPerTimeStep += movement.FlowRate.DeparturesVehPerTimeStep;
                            intersection.OnRamp.QueuingResults.ArrivalsPerTimeStep[(int)QueuedVehMovement.Left] += intersection.OnRamp.Segments[LeftTurnSegmentIndex].FlowRate.ArrivalsVehPerTimeStep;
                            intersection.OnRamp.QueuingResults.CumulativeArrivals[(int)QueuedVehMovement.Total] += intersection.OnRamp.Segments[LeftTurnSegmentIndex].FlowRate.ArrivalsVehPerTimeStep;
                        }
                        else if (movement.NemaPhaseId == NemaMovementNumbers.WBRight || movement.NemaPhaseId == NemaMovementNumbers.EBRight)
                        {
                            intersection.OnRamp.QueuingResults.CumulativeArrivals[(int)QueuedVehMovement.Right] += movement.FlowRate.DeparturesVehPerTimeStep;
                            intersection.OnRamp.Segments[RightTurnSegmentIndex].FlowRate.ArrivalsVehPerTimeStep += movement.FlowRate.DeparturesVehPerTimeStep;
                            intersection.OnRamp.QueuingResults.ArrivalsPerTimeStep[(int)QueuedVehMovement.Right] += intersection.OnRamp.Segments[RightTurnSegmentIndex].FlowRate.ArrivalsVehPerTimeStep;
                            intersection.OnRamp.QueuingResults.CumulativeArrivals[(int)QueuedVehMovement.Total] += intersection.OnRamp.Segments[RightTurnSegmentIndex].FlowRate.ArrivalsVehPerTimeStep;
                        }

                        //Determine number of vehicles on each segment -----------------------------------------------------------
                        //Check if shared link has any available capacity
                        //if not, vehicles will back up onto left/right segments, if present

                        float AvailableQueueStorageSharedLink = intersection.OnRamp.Segments[SharedSegmentIndex].QueueStorageCapacityVeh - intersection.OnRamp.Segments[SharedSegmentIndex].NumVehiclesQueued;

                        if (AvailableQueueStorageSharedLink > 0)
                        {
                            float LeftTurnFlowPerTimeStepPerLane = intersection.OnRamp.QueuingResults.ArrivalsPerTimeStep[(int)QueuedVehMovement.Left] / intersection.OnRamp.Segments[LeftTurnSegmentIndex].NumLanes;
                            //volume served from right-turn segment will be 1/2 of left-turn per-lane flow; that is, it is assumed that left-turn outer lane is shared equally by left-turn and right-turn traffice (zipper merge)
                            float NumVehiclesFromRightTurnPerTimeStep = 0.5f * LeftTurnFlowPerTimeStepPerLane;
                            float NumVehiclesFromLeftTurnPerTimeStep = intersection.OnRamp.QueuingResults.ArrivalsPerTimeStep[(int)QueuedVehMovement.Left] - NumVehiclesFromRightTurnPerTimeStep;
                            float TotalVehiclesPerTimeStep = NumVehiclesFromLeftTurnPerTimeStep + NumVehiclesFromRightTurnPerTimeStep;

                            float LeftTurnProp = NumVehiclesFromLeftTurnPerTimeStep / TotalVehiclesPerTimeStep;
                            float RightTurnProp = NumVehiclesFromRightTurnPerTimeStep / TotalVehiclesPerTimeStep;

                            if (TotalVehiclesPerTimeStep >= AvailableQueueStorageSharedLink)
                            {
                                intersection.OnRamp.Segments[LeftTurnSegmentIndex].FlowRate.DeparturesVehPerTimeStep = AvailableQueueStorageSharedLink * LeftTurnProp;
                                intersection.OnRamp.Segments[RightTurnSegmentIndex].FlowRate.DeparturesVehPerTimeStep = AvailableQueueStorageSharedLink * RightTurnProp;
                            }
                            else
                            {
                                intersection.OnRamp.Segments[LeftTurnSegmentIndex].FlowRate.DeparturesVehPerTimeStep = NumVehiclesFromLeftTurnPerTimeStep;
                                intersection.OnRamp.Segments[RightTurnSegmentIndex].FlowRate.DeparturesVehPerTimeStep = NumVehiclesFromRightTurnPerTimeStep;
                            }
                            //intersection.OnRamp.Segments[0].FlowRate.ArrivalsVehPerTimeStep
                            //if combination of queued vehicles for left-turn and right-turn segments is greater than shared segment capacity, calculate proportional split between two movements to feed shared segment
                            //reduce queued vehicles on upstream segments
                            //increase
                            //set departure rate for upstream segments to rate to fill capacity of shared link
                        }
                        else
                        {
                            intersection.OnRamp.Segments[LeftTurnSegmentIndex].FlowRate.DeparturesVehPerTimeStep = 0;
                            intersection.OnRamp.Segments[RightTurnSegmentIndex].FlowRate.DeparturesVehPerTimeStep = 0;
                        }

                        //intersection.OnRamp.QueuingResults.CumulativeArrivals += intersection.OnRamp.QueuingResults.ArrivalsPerSec[(int)TurnType.All];
                    }

                    //intersection.OnRamp.Segments[SharedSegmentIndex].NumVehiclesQueued +=
                    //intersection.OnRamp.Segments[SharedSegmentIndex].NumVehiclesQueued -= intersection.OnRamp.Segments[SharedSegmentIndex].FlowRate.DeparturesVehPerSec;
                    //intersection.OnRamp.Segments[SharedSegmentIndex].QueueStorageAvailableLaneFt =

                    //QueueLengthVeh = (Arrivals - Departures) + QueueLengthVeh;
                    intersection.OnRamp.QueuingResults.QueueLengthVehPreviousTimeStep = intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total];

                    //first determine if shared storage is filled up; if so, then calculate how much of left-only and right-only queue storage is filled
                    //assume lane that right turn lane feeds into uses zipper method (i.e., alternating) for merging
                    //total vehicle in queue vs joint queue storage area
                    intersection.OnRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Total] = intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total] * interchange.Vehicles.AvgVehicleSpacingFt;

                    if (intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total] > intersection.OnRamp.Segments[SharedSegmentIndex].QueueStorageCapacityVeh)
                    //if (intersection.OnRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Total] > intersection.OnRamp.QueueStorageLaneFt[(int)SegmentType.Shared])
                    {
                        intersection.OnRamp.QueuingResults.PctQueueStorageOccupied[(int)SegmentType.Shared] = 100;
                        //What is percent left and percent right vehicles in queue?

                        //queue is spilling back into left-only and/or right-only sections of on-ramp
                        //Left-only
                        //intersection.OnRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Left] = intersection.OnRamp.QueuingResults.NumVehsInQueue[3] * interchange.Vehicles.AvgVehicleSpacingFt;

                        //q length (veh) for right/left will be diff in arrivals and departures
                        //assume vehicles depart equally from each shared q storage lane
                        //so capacity of lane that right lane feeds into will be 1/NumLanes of shared storage area
                        //the outer lane will serve 1/2 left and 1/2 right demand when shared storage is full/nearly full

                        intersection.OnRamp.QueuingResults.PctQueueStorageOccupied[(int)SegmentType.LeftTurn] = 0;
                        intersection.OnRamp.QueuingResults.PctQueueStorageOccupied[(int)SegmentType.RightTurn] = 0;
                    }
                    else
                    {
                        //intersection.OnRamp.QueuingResults.PctQueueStorageOccupied[(int)SegmentType.Shared] = intersection.OnRamp.QueuingResults.QueueLengthFt[(int)QueuedVehMovement.Total] / intersection.OnRamp.QueueStorageLaneFt[(int)SegmentType.Shared] * 100;

                        intersection.OnRamp.QueuingResults.PctQueueStorageOccupied[(int)SegmentType.Shared] = intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total] / intersection.OnRamp.Segments[SharedSegmentIndex].QueueStorageCapacityVeh * 100;
                        intersection.OnRamp.QueuingResults.PctQueueStorageOccupied[(int)SegmentType.LeftTurn] = 0;
                        intersection.OnRamp.QueuingResults.PctQueueStorageOccupied[(int)SegmentType.RightTurn] = 0;
                    }


                    //Check if vehicles should be discharged from meter stop bar ----------------------------------------------------------
                    intersection.OnRamp.Meter.ReleaseIntervalSec = 3600f / intersection.OnRamp.Meter.CurrentRateVehPerHr * intersection.OnRamp.Segments[SharedSegmentIndex].NumLanes;

                    if (TimeIndex % intersection.OnRamp.Meter.ReleaseIntervalSec == 0)
                    {
                        //discharge of vehicles typically takes 2-3 seconds
                        int MaxNumVehiclesDischarged = 1 * intersection.OnRamp.Segments[SharedSegmentIndex].NumLanes;  //assumes 1 vehicle per green interval, per lane

                        intersection.OnRamp.QueuingResults.VehServedPerTimeStep = Math.Min(intersection.OnRamp.Segments[SharedSegmentIndex].NumVehiclesQueued, MaxNumVehiclesDischarged);

                        intersection.OnRamp.Segments[SharedSegmentIndex].NumVehiclesQueued -= intersection.OnRamp.QueuingResults.VehServedPerTimeStep;
                        intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total] -= intersection.OnRamp.QueuingResults.VehServedPerTimeStep;
                    }
                    else
                    {
                        intersection.OnRamp.QueuingResults.VehServedPerTimeStep = 0;
                    }

                    //intersection.OnRamp.QueuingResults.ArrivalsPerSec[(int)TurnType.All] += movement.FlowRate.DeparturesVehPerSec;

                    //discharge vehicles from segment with meter
                    //intersection.OnRamp.Segments[SharedSegmentIndex].FlowRate.DeparturesVehPerSec = intersection.OnRamp.Meter.CurrentRateVehPerHr / 3600f;

                    intersection.OnRamp.QueuingResults.DepartFlowRateVehPerTimeStep = (float)intersection.OnRamp.Meter.CurrentRateVehPerHr / Analysis.AnalysisPeriodSec * Analysis.TimeStepsPerSecond;
                    //intersection.OnRamp.QueuingResults.VehServedPerTimeStep = Math.Min(intersection.OnRamp.QueuingResults.ArrivalsPerSec[(int)QueuedVehMovement.Total] + intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total], intersection.OnRamp.QueuingResults.DepartFlowRateVehPerTimeStep);
                    intersection.OnRamp.QueuingResults.VehServedPerTimeStep = Math.Min(intersection.OnRamp.Segments[0].FlowRate.ArrivalsVehPerTimeStep + intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total], intersection.OnRamp.QueuingResults.DepartFlowRateVehPerTimeStep);

                    intersection.OnRamp.QueuingResults.CumulativeDepartures += intersection.OnRamp.QueuingResults.VehServedPerTimeStep;

                    intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total] = intersection.OnRamp.QueuingResults.CumulativeArrivals[(int)QueuedVehMovement.Total] - intersection.OnRamp.QueuingResults.CumulativeDepartures;
                    intersection.OnRamp.QueuingResults.DeltaTimeStepQueueLengthVeh = intersection.OnRamp.QueuingResults.NumVehsInQueue[(int)QueuedVehMovement.Total] - intersection.OnRamp.QueuingResults.QueueLengthVehPreviousTimeStep;

                    if (intersection.OnRamp.QueuingResults.DeltaTimeStepQueueLengthVeh > 0)  //Queue is increasing in size
                    {
                        CheckQueueDetectorActivation(out IsAdvanceQdetActive, out IsIntQdetActive, intersection.OnRamp);
                        UpdateMeteringRate(IsAdvanceQdetActive, IsIntQdetActive, true, intersection.OnRamp);
                    }
                    else  //Queue is decreasing in size
                    {
                        CheckQueueDetectorDeactivation(out IsAdvanceQdetActive, out IsIntQdetActive, intersection.OnRamp);
                        UpdateMeteringRate(IsAdvanceQdetActive, IsIntQdetActive, false, intersection.OnRamp);
                    }

                    intersection.OnRamp.Meter.PctTimeMeteringRateMax = intersection.OnRamp.Meter.NumTimeStepsAtMaxMeteringRate * 100f / (TimeIndex + 1);

                    //intersection.OnRamp.QueuingResults.ArrivalsPerTimeStep[(int)TurnType.Left], intersection.OnRamp.QueuingResults.ArrivalsPerTimeStep[(int)TurnType.Thru], intersection.OnRamp.QueuingResults.ArrivalsPerTimeStep[(int)TurnType.Right], intersection.OnRamp.QueuingResults.ArrivalsPerTimeStep[(int)TurnType.All]

                    //intersection.OnRamp.QueuingResults.QueueLengthFt[(int)TurnType.All]
                    //TimeStepResult = new ResultsData(TimeIndex, (byte)(TimingStageIndex + 1), intersection.OnRamp.QueuingResults.ArrivalsPerHour[(int)QueuedVehMovement.Total], intersection.OnRamp.QueuingResults.ArrivalsPerTimeStep, intersection.OnRamp.QueuingResults.DepartFlowRateVehPerHr, intersection.OnRamp.QueuingResults.DepartFlowRateVehPerTimeStep, intersection.OnRamp.QueuingResults.VehServedPerTimeStep, intersection.OnRamp.QueuingResults.CumulativeArrivals[(int)QueuedVehMovement.Total], intersection.OnRamp.QueuingResults.CumulativeDepartures, intersection.OnRamp.QueuingResults.NumVehsInQueue, intersection.OnRamp.QueuingResults.PctQueueStorageOccupied, intersection.OnRamp.Meter.PctTimeMeteringRateMax);
                    //Results.Add(TimeStepResult);

                    if (WriteResultsToOutputWindow == true)
                    {
                        Debug.WriteLine("Time Index: " + ((double)TimeIndex / 10).ToString("0.0") + "s; Metering Rate: " + intersection.OnRamp.QueuingResults.DepartFlowRateVehPerHr);
                        //Debug.WriteLine("Queue Length: " + QueueLengthVeh.ToString("0.0"));
                        Debug.WriteLine("------------------------");
                    }
                    //}
                }
            }
            WriteResultsToFile(Results);
            */
        }

       
        private static void WriteResultsToFile(List<ResultsData> results)
        {
            string ResultsFilename = @"..\..\..\Results.csv";
            //string ResultsFilename = @"C:\Users\swash\OneDrive - University of Florida\FDOT Interchange Ramp Metering\Task 3-Experimental Design\Results.csv";
            //string ResultsFilename = @"C:\Users\ariel\OneDrive\Desktop\Interchanges Data\Queue Calc Results\Results.csv";
            FileInputOutput.WriteResultsData(ResultsFilename, results);
        }


        private static void UpdateMeteringRate(bool IsAdvanceQdetActive, bool IsIntQdetActive, bool isQueueIncreasing, OnRampData onRamp)
        {
            if (isQueueIncreasing == true)
            {                
                //first check advance q det
                //if false, then check int q det

                if (IsAdvanceQdetActive == true)
                {
                    onRamp.Meter.CurrentRateVehPerHr = onRamp.Meter.MaxRateVehPerHr;
                    onRamp.Meter.NumTimeStepsAtMaxMeteringRate++;
                }
                else if (IsIntQdetActive == true)
                {
                    onRamp.Meter.CurrentRateVehPerHr = onRamp.Meter.BaseRateVehPerHr + onRamp.Meter.AddedRateVehPerHr;
                }
                else
                {
                    onRamp.Meter.CurrentRateVehPerHr = onRamp.Meter.BaseRateVehPerHr;
                }
                onRamp.Segments[0].Results.DepartFlowRateVehPerHr = onRamp.Meter.CurrentRateVehPerHr;
            }
            else  //Queue is decreasing in size
            {
                if (IsAdvanceQdetActive == false)
                {
                    onRamp.Meter.CurrentRateVehPerHr = onRamp.Meter.BaseRateVehPerHr;
                }
                else if (IsAdvanceQdetActive == false)
                {
                    onRamp.Meter.CurrentRateVehPerHr = onRamp.Meter.BaseRateVehPerHr + onRamp.Meter.AddedRateVehPerHr;
                }
                else
                {
                    onRamp.Meter.CurrentRateVehPerHr = onRamp.Meter.MaxRateVehPerHr;
                    onRamp.Meter.NumTimeStepsAtMaxMeteringRate++;
                }
                onRamp.Segments[0].Results.DepartFlowRateVehPerHr = onRamp.Meter.CurrentRateVehPerHr;
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
