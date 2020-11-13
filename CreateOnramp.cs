using System;
using System.Collections.Generic;
using QueueCalcs.DataStructures;



namespace QueueCalcs
{
    class CreateOnramp
    {

        public static List<InterchangeIntersectionData> TightDiamond()
        {
            //No separate right-turn flared turn lane; e.g., tight diamond interchanges along I-95 in Miami

            IntersectionMovementData MovementEBright = new IntersectionMovementData(1, "EB Right", NemaMovementNumbers.EBRight, 1, 250, true);
            IntersectionMovementData MovementWBleft = new IntersectionMovementData(2, "WB Left", NemaMovementNumbers.WBLeft, 1, 400, true);
            IntersectionMovementData MovementSBthru = new IntersectionMovementData(3, "SB Thru", NemaMovementNumbers.SBThru, 1, 0, true);

            List<TimingStageData> TimingStages = new List<TimingStageData>();
            List<IntersectionMovementData> TimingStageMovements = new List<IntersectionMovementData>();
            TimingStageMovements.Add(MovementEBright);

            TimingStageData newTimingStage = new TimingStageData(1, TimingStageMovements, 35, 5);
            TimingStages.Add(newTimingStage);

            TimingStageMovements = new List<IntersectionMovementData>();
            TimingStageMovements.Add(MovementWBleft);            

            newTimingStage = new TimingStageData(2, TimingStageMovements, 25, 5);
            TimingStages.Add(newTimingStage);

            TimingStageMovements = new List<IntersectionMovementData>();
            TimingStageMovements.Add(MovementSBthru);

            newTimingStage = new TimingStageData(3, TimingStageMovements, 15, 5);
            TimingStages.Add(newTimingStage);

            List<OnRampSegmentData> OnRampSegments = new List<OnRampSegmentData>();

            OnRampSegmentData newSegment = new OnRampSegmentData(1, SegmentType.Shared, 2, 1000);
            OnRampSegments.Add(newSegment);

            List<RampQueueDetector> Qdetectors = new List<RampQueueDetector>();
            RampQueueDetector IntQdetector = new RampQueueDetector(1, DetectorType.IntermediateQueue, DetectorMovement.All, 500); //, SegmentType.Shared);
            Qdetectors.Add(IntQdetector);

            RampQueueDetector AdvQdetector = new RampQueueDetector(2, DetectorType.AdvanceQueue, DetectorMovement.All, 950);
            Qdetectors.Add(AdvQdetector);            

            OnRampData newOnRamp = new OnRampData(1, "Southbound", OnRampSegments, Qdetectors);

            InterchangeIntersectionData Intersection = new InterchangeIntersectionData(1, new List<IntersectionMovementData> { MovementEBright, MovementWBleft, MovementSBthru }, true, newOnRamp);
            Intersection.Signal.Cycle = new CycleData(TimingStages);
            //Intersection.Traffic = new TrafficStreamData(0.12f, 0.6f, 0.4f, 0.75f, 0.25f);
            Intersection.Traffic = new TrafficStreamData(0.528f, 0.352f, 0.09f, 0.03f);
            Intersection.Vehicles = new VehicleData(Intersection.Traffic);

            List<InterchangeIntersectionData> Intersections = new List<InterchangeIntersectionData>();

            Intersections.Add(Intersection);

            return Intersections;
        }

        public static List<InterchangeIntersectionData> DDI()
        {            
            IntersectionMovementData MovementEBright = new IntersectionMovementData(1, "EB Right", NemaMovementNumbers.EBRight, 1, 400, true);
            IntersectionMovementData MovementWBleft = new IntersectionMovementData(2, "WB Left", NemaMovementNumbers.WBLeft, 1, 600, true);
            //IntersectionMovementData MovementSBthru = new IntersectionMovementData("SB Thru", NemaMovementNumbers.SBThru, 1, 0, true);

            List<TimingStageData> TimingStages = new List<TimingStageData>();

            List<IntersectionMovementData> TimingStageMovements = new List<IntersectionMovementData>();            
            TimingStageMovements.Add(MovementEBright);
            TimingStageData newTimingStage = new TimingStageData(1, TimingStageMovements, 40, 5);
            TimingStages.Add(newTimingStage);            

            TimingStageMovements = new List<IntersectionMovementData>();
            TimingStageMovements.Add(MovementWBleft);
            TimingStageMovements.Add(MovementEBright);  //free right-turn

            newTimingStage = new TimingStageData(2, TimingStageMovements, 35, 5);
            TimingStages.Add(newTimingStage);            

            List<OnRampSegmentData> OnRampSegments = new List<OnRampSegmentData>();

            OnRampSegmentData newSegment = new OnRampSegmentData(1, SegmentType.Shared, 2, 600);
            OnRampSegments.Add(newSegment);

            newSegment = new OnRampSegmentData(2, SegmentType.LeftTurn, 2, 200);
            OnRampSegments.Add(newSegment);           

            newSegment = new OnRampSegmentData(3, SegmentType.RightTurn, 1, 300);
            OnRampSegments.Add(newSegment);

            List<RampQueueDetector> Qdetectors = new List<RampQueueDetector>();
            RampQueueDetector IntQdetector = new RampQueueDetector(1, DetectorType.IntermediateQueue, DetectorMovement.All, 500); //, SegmentType.Shared);
            Qdetectors.Add(IntQdetector);

            RampQueueDetector AdvQdetector = new RampQueueDetector(2, DetectorType.AdvanceQueue, DetectorMovement.Left, 750);
            Qdetectors.Add(AdvQdetector);

            AdvQdetector = new RampQueueDetector(3, DetectorType.AdvanceQueue, DetectorMovement.Right, 850);
            Qdetectors.Add(AdvQdetector);

            OnRampData newOnRamp = new OnRampData(1, "Northbound", OnRampSegments, Qdetectors);

            InterchangeIntersectionData Intersection = new InterchangeIntersectionData(1, new List<IntersectionMovementData> { MovementEBright, MovementWBleft }, true, newOnRamp);
            Intersection.Signal.Cycle = new CycleData(TimingStages);            

            List<InterchangeIntersectionData> Intersections = new List<InterchangeIntersectionData>();
            Intersections.Add(Intersection);

            return Intersections;
        }


    }
}
