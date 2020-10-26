using System.Xml.Serialization;


namespace QueueCalcs.DataStructures
{
    public class RampMeteringData
    {
        float _releaseIntervalSec;   //cycle length, in seconds, for a given hourly metering rate (e.g., 900 veh/h = 4 s/veh, 240 veh/h = 15 s/veh)
        int _currentRateVehPerHr;
        int _baseRateVehPerHr;
        int _addedRateVehPerHr;           // Amount added, only once, to the metering rate
        int _maxRateVehPerHr;
        int _numTimeStepsAtMaxMeteringRate;
        float _pctTimeMeteringRateMax;

        public RampMeteringData()
        { }

        public RampMeteringData(int baseMeteringRateVehPerHr, int addedRateVehPerHr, int maxRateVehPerHr)
        {
            _currentRateVehPerHr = baseMeteringRateVehPerHr;
            _baseRateVehPerHr = baseMeteringRateVehPerHr;
            _addedRateVehPerHr = addedRateVehPerHr;        //Amount added for intermediate queue override
            _maxRateVehPerHr = maxRateVehPerHr;
        }

        public int CurrentRateVehPerHr { get => _currentRateVehPerHr; set => _currentRateVehPerHr = value; }
        public int BaseRateVehPerHr { get => _baseRateVehPerHr; set => _baseRateVehPerHr = value; }
        public int AddedRateVehPerHr { get => _addedRateVehPerHr; set => _addedRateVehPerHr = value; }
        public int MaxRateVehPerHr { get => _maxRateVehPerHr; set => _maxRateVehPerHr = value; }
        [XmlIgnore]
        public int NumTimeStepsAtMaxMeteringRate { get => _numTimeStepsAtMaxMeteringRate; set => _numTimeStepsAtMaxMeteringRate = value; }
        [XmlIgnore]
        public float PctTimeMeteringRateMax { get => _pctTimeMeteringRateMax; set => _pctTimeMeteringRateMax = value; }
        [XmlIgnore]
        public float ReleaseIntervalSec { get => _releaseIntervalSec; set => _releaseIntervalSec = value; }


        //public float IntermediateQueueDetectorLengthFt { get => _intermediateQueueDetectorLengthFt; set => _intermediateQueueDetectorLengthFt = value; }
        //public int AdvanceQueueDetectorLengthFt { get => _advanceQueueDetectorLengthFt; set => _advanceQueueDetectorLengthFt = value; }
    }
}
