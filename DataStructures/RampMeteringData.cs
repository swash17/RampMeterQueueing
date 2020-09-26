using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace QueueCalcs.DataStructures
{
    public class RampMeteringData
    {

        int _baseMeteringRateVehPerHr;
        int _addMeteringRateVehPerHr;           // Amount added, only once, to the metering rate
        int _meteringRateMaxVehPerHr;
        int _numTimeStepsAtMaxMeteringRate;
        float _pctTimeMeteringRateMax;

        public RampMeteringData()
        {
            BaseMeteringRateVehPerHr = 240;
            AddMeteringRateVehPerHr = 180;             // Amount added, only once, to the metering rate
            MeteringRateMaxVehPerHr = 900;
        }
        [XmlIgnore] //Ignoring all three since variables are fixed immediately above. Other 2 variables don't appear to be fixed ABC
        public int BaseMeteringRateVehPerHr { get => _baseMeteringRateVehPerHr; set => _baseMeteringRateVehPerHr = value; }
        [XmlIgnore]
        public int AddMeteringRateVehPerHr { get => _addMeteringRateVehPerHr; set => _addMeteringRateVehPerHr = value; }
        [XmlIgnore]
        public int MeteringRateMaxVehPerHr { get => _meteringRateMaxVehPerHr; set => _meteringRateMaxVehPerHr = value; }
        public int NumTimeStepsAtMaxMeteringRate { get => _numTimeStepsAtMaxMeteringRate; set => _numTimeStepsAtMaxMeteringRate = value; }
        public float PctTimeMeteringRateMax { get => _pctTimeMeteringRateMax; set => _pctTimeMeteringRateMax = value; }

        //public float IntermediateQueueDetectorLengthFt { get => _intermediateQueueDetectorLengthFt; set => _intermediateQueueDetectorLengthFt = value; }
        //public int AdvanceQueueDetectorLengthFt { get => _advanceQueueDetectorLengthFt; set => _advanceQueueDetectorLengthFt = value; }
    }
}
