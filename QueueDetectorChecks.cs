using System;
using System.Collections.Generic;
using System.Linq;
using QueueCalcs.DataStructures;


namespace QueueCalcs
{
    public partial class QueueAnalysis
    {

        //Also check condition where advance queue detector is within shared queue storage area
        private static void CheckQueueDetectorActivation(out bool IsAdvanceQdetActive, out bool IsIntQdetActive, OnRampData onRamp)
        {
            IsAdvanceQdetActive = false;
            IsIntQdetActive = false;

            //Check Advance Q detector first ---------------------------------
            //first check if queue is beyond shared queue storage area
            if (onRamp.Segments.Count > 1 && onRamp.Segments[0].Results.NumVehsInQueue[(int)QueuedVehMovement.Total] > onRamp.Segments[SharedSegmentIndex].QueueStorageCapacityVeh)
            {
                //check advance queue detector for both left- and right-turn storage area

                //int QdetectorIndex = intersection.OnRamp.QueueDetectors.FindIndex(detector => detector.Type.Equals(DetectorType.AdvanceQueue));
                for (int DetIndex = 0; DetIndex < onRamp.QueueDetectors.Count; DetIndex++)
                {
                    if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.AdvanceQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Left)
                    {
                        if (onRamp.Segments[0].Results.QueueLengthFtPerLane[(int)QueuedVehMovement.Left] > onRamp.QueueDetectors[DetIndex].DistanceLaneFt)
                        {
                            IsAdvanceQdetActive = true;
                        }
                    }
                    if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.AdvanceQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Right)
                    {
                        if (onRamp.Segments[0].Results.QueueLengthFtPerLane[(int)QueuedVehMovement.Right] > onRamp.QueueDetectors[DetIndex].DistanceLaneFt)
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
                    if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.AdvanceQueue) // && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.All)
                    {
                        if (onRamp.Segments[0].Results.QueueLengthFtPerLane[(int)QueuedVehMovement.Total] > onRamp.QueueDetectors[DetIndex].DistanceLaneFt)
                        {
                            IsAdvanceQdetActive = true;
                        }
                    }
                }
            }

            //If Advance Q detector is not activated, then check Intermediate Q detector ---------------------------------
            if (IsAdvanceQdetActive == false)
            {
                if (onRamp.Segments.Count > 1 && onRamp.Segments[0].Results.NumVehsInQueue[(int)QueuedVehMovement.Total] > onRamp.Segments[SharedSegmentIndex].QueueStorageCapacityVeh)
                {
                    //check advance queue detector for both left- and right-turn storage area

                    //int QdetectorIndex = intersection.OnRamp.QueueDetectors.FindIndex(detector => detector.Type.Equals(DetectorType.AdvanceQueue));
                    for (int DetIndex = 0; DetIndex < onRamp.QueueDetectors.Count; DetIndex++)
                    {
                        if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.IntermediateQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Left)
                        {
                            if (onRamp.Segments[0].Results.QueueLengthFtPerLane[(int)QueuedVehMovement.Left] > onRamp.QueueDetectors[DetIndex].DistanceLaneFt)
                            {
                                IsIntQdetActive = true;
                            }
                        }
                        if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.IntermediateQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Right)
                        {
                            if (onRamp.Segments[0].Results.QueueLengthFtPerLane[(int)QueuedVehMovement.Right] > onRamp.QueueDetectors[DetIndex].DistanceLaneFt)
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
                            if (onRamp.Segments[0].Results.QueueLengthFtPerLane[(int)QueuedVehMovement.Total] > onRamp.QueueDetectors[DetIndex].DistanceLaneFt)
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
            //int QdetectorIndex = intersection.OnRamp.QueueDetectors.FindIndex(detector => detector.Type.Equals(DetectorType.AdvanceQueue));
            IsAdvanceQdetActive = false;
            IsIntQdetActive = true;

            //Check Intermediate Q detector first ---------------------------------
            //first check if queue is beyond shared queue storage area
            if (onRamp.Segments.Count > 1 && onRamp.Segments[0].Results.NumVehsInQueue[(int)QueuedVehMovement.Total] > onRamp.Segments[SharedSegmentIndex].QueueStorageCapacityVeh)
            {
                //check advance queue detector for both left- and right-turn storage area                
                for (int DetIndex = 0; DetIndex < onRamp.QueueDetectors.Count; DetIndex++)
                {
                    if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.IntermediateQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Left)
                    {
                        if (onRamp.Segments[0].Results.QueueLengthFtPerLane[(int)QueuedVehMovement.Left] < onRamp.QueueDetectors[DetIndex].DistanceLaneFt - 50)
                        {
                            IsIntQdetActive = false;
                        }
                    }
                    if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.IntermediateQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Right)
                    {
                        if (onRamp.Segments[0].Results.QueueLengthFtPerLane[(int)QueuedVehMovement.Right] < onRamp.QueueDetectors[DetIndex].DistanceLaneFt - 50)
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
                        if (onRamp.Segments[0].Results.QueueLengthFtPerLane[(int)QueuedVehMovement.Total] < onRamp.QueueDetectors[DetIndex].DistanceLaneFt - 50)
                        {
                            IsIntQdetActive = false;
                        }
                    }
                }
            }

            //If Intermediate Q detector is activated, then check Advance Q detector ---------------------------------
            if (IsIntQdetActive == true)
            {
                IsAdvanceQdetActive = true;
                if (onRamp.Segments.Count > 1 && onRamp.Segments[0].Results.NumVehsInQueue[(int)QueuedVehMovement.Total] > onRamp.Segments[SharedSegmentIndex].QueueStorageCapacityVeh)
                {
                    //check advance queue detector for both left- and right-turn storage area
                    for (int DetIndex = 0; DetIndex < onRamp.QueueDetectors.Count; DetIndex++)
                    {
                        if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.AdvanceQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Left)
                        {
                            if (onRamp.Segments[0].Results.QueueLengthFtPerLane[(int)QueuedVehMovement.Left] < onRamp.QueueDetectors[DetIndex].DistanceLaneFt - 100)
                            {
                                IsIntQdetActive = false;
                            }
                        }
                        if (onRamp.QueueDetectors[DetIndex].Type == DetectorType.AdvanceQueue && onRamp.QueueDetectors[DetIndex].Movement == DetectorMovement.Right)
                        {
                            if (onRamp.Segments[0].Results.QueueLengthFtPerLane[(int)QueuedVehMovement.Right] < onRamp.QueueDetectors[DetIndex].DistanceLaneFt - 100)
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
                            if (onRamp.Segments[0].Results.QueueLengthFtPerLane[(int)QueuedVehMovement.Total] < onRamp.QueueDetectors[DetIndex].DistanceLaneFt - 100)
                            {
                                IsAdvanceQdetActive = false;
                            }
                        }
                    }
                }
            }
        }

    }
}
