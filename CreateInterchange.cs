using System;
using System.Collections.Generic;
using QueueCalcs.DataStructures;



namespace QueueCalcs
{
    class CreateInterchange
    {

        public static InterchangeData DDI()
        {
            
            IntersectionMovementData MovementEBright = new IntersectionMovementData("EB Right", NemaMovementNumbers.EBRight, 1, 400, true);
            IntersectionMovementData MovementWBleft = new IntersectionMovementData("WB Left", NemaMovementNumbers.WBLeft, 1, 600, true);
            IntersectionMovementData MovementSBthru = new IntersectionMovementData("SB Thru", NemaMovementNumbers.SBThru, 1, 0, true);            

            List<TimingStageData> TimingStages = new List<TimingStageData>();
            List<IntersectionMovementData> TimingStageMovements = new List<IntersectionMovementData>();
            TimingStageMovements.Add(MovementEBright);

            TimingStageData newTimingStage = new TimingStageData(1, TimingStageMovements, 45);
            TimingStages.Add(newTimingStage);            

            TimingStageMovements = new List<IntersectionMovementData>();
            TimingStageMovements.Add(MovementWBleft);
            TimingStageMovements.Add(MovementEBright);  //free right-turn

            newTimingStage = new TimingStageData(2, TimingStageMovements, 15);
            TimingStages.Add(newTimingStage);

            TimingStageMovements = new List<IntersectionMovementData>();
            TimingStageMovements.Add(MovementSBthru);
            TimingStageMovements.Add(MovementEBright);  //free right-turn

            newTimingStage = new TimingStageData(3, TimingStageMovements, 15);
            TimingStages.Add(newTimingStage);

            //CycleData Cycle = new CycleData(TimingStages);

            List<RampQueueDetector> Qdetectors = new List<RampQueueDetector>();
            RampQueueDetector Qdetector = new RampQueueDetector(DetectorType.IntermediateQueue, DetectorMovement.All, 500);
            Qdetectors.Add(Qdetector);
            Qdetector = new RampQueueDetector(DetectorType.AdvanceQueue, DetectorMovement.Left, 750);
            Qdetectors.Add(Qdetector);
            Qdetector = new RampQueueDetector(DetectorType.AdvanceQueue, DetectorMovement.Right, 850);
            Qdetectors.Add(Qdetector);

            OnRampData newOnRamp = new OnRampData(1, 2, 2, 1, 600, 200, 300, Qdetectors);
            IntersectionData Intersection = new IntersectionData(1, new List<IntersectionMovementData> { MovementEBright, MovementWBleft, MovementSBthru }, true, newOnRamp);
            Intersection.Signal.Cycle = new CycleData(TimingStages);            

            List<IntersectionData> Intersections = new List<IntersectionData>();
            Intersections.Add(Intersection);

            InterchangeData Interchange = new InterchangeData(Intersections);            

            return Interchange;
        }


    }
}
