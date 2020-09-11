using System;
using System.Collections.Generic;


namespace QueueCalcs.DataStructures
{
    public class ResultsData
    {
        float _timeStepSec;
        byte _timingStageID;
        int _arrivalRateVehPerHr;
        int _departureRateVehPerHr;
        float[] _arrivalsTimeStep;
        float _departuresTimeStep;
        float _vehServedTimeStep;
        float _cumulativeArrivalsVeh;
        float _cumulativeDeparturesVeh;
        int _addMeteringRateMax;
        float _queueLengthVeh;
        float _queueLengthFt;
        float _pctTimeMeteringRateMax;

        //TimeIndex, inputs.Traffic.ArrivalFlowRateVehPerHr[ArrivalRateTimePeriodIndex], ArrivalsPerTimeStep, DepartFlowRateVehPerHr, DepartFlowRateVehPerTimeStep, VehServedPerTimeStep, CumulativeArrivals, CumulativeDepartures, QueueLengthVeh, QueueLengthFt, PctTimeMeteringRateMax);

        public ResultsData(float timeStepSec, byte timingStageID, int arrivalRateVPH, float[] arrivalsTimeStep, int departureRateVPH, float departuresTimeStep, float vehServedTimeStep, float cumulativeArrivalsVeh, float cumulativeDeparturesVeh, float queLengthVeh, float queLenghtFt, float pctTimeMeteringRateMax)
        {
            _timeStepSec = timeStepSec;
            _timingStageID = timingStageID;
            _arrivalRateVehPerHr = arrivalRateVPH;
            _arrivalsTimeStep = new float[4];
            _arrivalsTimeStep[0] = arrivalsTimeStep[0];
            _arrivalsTimeStep[1] = arrivalsTimeStep[1];
            _arrivalsTimeStep[2] = arrivalsTimeStep[2];
            _arrivalsTimeStep[3] = arrivalsTimeStep[3];
            _departureRateVehPerHr = departureRateVPH;
            _departuresTimeStep = departuresTimeStep;
            _vehServedTimeStep = vehServedTimeStep;
            _cumulativeArrivalsVeh = cumulativeArrivalsVeh;
            _cumulativeDeparturesVeh = cumulativeDeparturesVeh;
            _queueLengthVeh = queLengthVeh;
            _queueLengthFt = queLenghtFt;
            _pctTimeMeteringRateMax = pctTimeMeteringRateMax;
        }

        public float TimeStepSec { get => _timeStepSec; set => _timeStepSec = value; }
        public byte TimingStageID { get => _timingStageID; set => _timingStageID = value; }
        public int ArrivalRateVehPerHr { get => _arrivalRateVehPerHr; set => _arrivalRateVehPerHr = value; }
        public int DepartureFlowRateVehPerHr { get => _departureRateVehPerHr; set => _departureRateVehPerHr = value; }
        public float[] ArrivalsTimeStep { get => _arrivalsTimeStep; set => _arrivalsTimeStep = value; }
        public float DeparturesTimeStep { get => _departuresTimeStep; set => _departuresTimeStep = value; }
        public float VehServedTimeStep { get => _vehServedTimeStep; set => _vehServedTimeStep = value; }
        public float QueueLengthVeh { get => _queueLengthVeh; set => _queueLengthVeh = value; }
        public float QueueLengthFt { get => _queueLengthFt; set => _queueLengthFt = value; }
        public int AddMeteringRateMax { get => _addMeteringRateMax; set => _addMeteringRateMax = value; }        
        public float PctTimeMeteringRateMax { get => _pctTimeMeteringRateMax; set => _pctTimeMeteringRateMax = value; }
        public float CumulativeArrivals { get => _cumulativeArrivalsVeh; set => _cumulativeArrivalsVeh = value; }
        public float CumulativeDepartures { get => _cumulativeDeparturesVeh; set => _cumulativeDeparturesVeh = value; }
        
    }
}
