using System;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace QueueCalcs.DataStructures
{
    public enum NemaMovementNumbers
    {
        EBLeft = 5,
        EBThru = 2,
        EBRight = 12,
        WBLeft = 1,
        WBThru = 6,
        WBRight = 16,
        NBLeft = 3,
        NBThru = 8,
        NBRight = 18,
        SBLeft = 7,
        SBThru = 4,
        SBRight = 14
    }

    public enum TurnMovementType
    {
        Left = 0,
        Thru = 1,
        Right = 2,        
    }        

    public class SignalData
    {
        CycleData _cycle;        

        public SignalData()
        {
            _cycle = new CycleData();
        }

        public SignalData(CycleData cycle) //(List<TimingStageData> timingStages)
        {
            _cycle = cycle;                        
        }
        
        public CycleData Cycle { get => _cycle; set => _cycle = value; }
    }

    public class CycleData
    {
        List<TimingStageData> _timingStages;
        float[] _timingStageStartTimes; //relative start time within cycle
        float _lengthSec;
        float _numCyclesPerHour;
        float[] _avgArrivalsPerCycleVeh;
        int[] _arrivalsPerCycleVeh;

        public CycleData()
        { }

        public CycleData(List<TimingStageData> timingStages)
        {
            int NumTimingStages = timingStages.Count;
            _timingStages = timingStages;
            _lengthSec = 0;

            foreach (TimingStageData timingStage in timingStages)
                _lengthSec += timingStage.LengthSec;

            _timingStageStartTimes = new float[NumTimingStages];
            float CycleRefStartTime = 0;
            _timingStageStartTimes[0] = CycleRefStartTime;
            for (int i = 1; i < NumTimingStages; i++)
                _timingStageStartTimes[i] = _timingStageStartTimes[i - 1] + timingStages[i - 1].LengthSec;

            _avgArrivalsPerCycleVeh = new float[NumTimingStages];
            _numCyclesPerHour = 3600 / _lengthSec;
            int ArraySize = (int)(Math.Round(_numCyclesPerHour, 0, MidpointRounding.AwayFromZero) + 1);
            _arrivalsPerCycleVeh = new int[ArraySize];
        }

        [XmlIgnore]
        public float LengthSec { get => _lengthSec; set => _lengthSec = value; }
        [XmlIgnore]
        public float[] TimingStageStartTimes { get => _timingStageStartTimes; set => _timingStageStartTimes = value; }

        //[XmlElement(ElementName = "")]
        public List<TimingStageData> TimingStages { get => _timingStages; set => _timingStages = value; }
        [XmlIgnore] //ABC suppressed since variable is calculated using lengthSec
        public float NumCyclesPerHour { get => _numCyclesPerHour; set => _numCyclesPerHour = value; }
        [XmlIgnore]
        public float[] AvgArrivalsPerCycleVeh { get => _avgArrivalsPerCycleVeh; set => _avgArrivalsPerCycleVeh = value; }
        [XmlIgnore]
        public int[] ArrivalsPerCycleVeh { get => _arrivalsPerCycleVeh; set => _arrivalsPerCycleVeh = value; }
    }


    [XmlRoot(ElementName = "TimingStage")]
    public class TimingStageData
    {        
        byte _id;        
        float _greenTime;
        float _lostTime;
        float _lengthSec;
        float _relativeCycleStartTime;
        
        //int[] _avgDeparturesPerCycleVeh;  //0-left, 1-thru, 2-right, 3-total
        List<IntersectionMovementData> _movements;

        public TimingStageData()
        { }

        public TimingStageData(byte id, List<IntersectionMovementData> movements, float greenTime, float lostTime)
        {
            _id = id;
            _movements = movements;
            _greenTime = greenTime;
            _lostTime = lostTime;
            _lengthSec = _greenTime + _lostTime;
        }

        [XmlAttribute("ID")]
        public byte Id { get => _id; set => _id = value; }
        public float GreenTime { get => _greenTime; set => _greenTime = value; }
        public float LostTime { get => _lostTime; set => _lostTime = value; }
        [XmlIgnore]
        public float LengthSec { get => _lengthSec; set => _lengthSec = value; }        
        public List<IntersectionMovementData> Movements { get => _movements; set => _movements = value; }
        [XmlIgnore]
        public float RelativeCycleStartTime { get => _relativeCycleStartTime; set => _relativeCycleStartTime = value; }
        //[XmlIgnore]
        //public int[] AvgDeparturesPerCycleVeh { get => _avgDeparturesPerCycleVeh; set => _avgDeparturesPerCycleVeh = value; }
        
    }

    

}
