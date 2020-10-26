using System;
using System.Collections.Generic;


namespace QueueCalcs.DataStructures
{

    public class AnalysisData
    {
        int _analysisPeriodSec;
        int _analysisPeriodMin;
        int _timeStepsPerSecond;

        public AnalysisData()
        {
            AnalysisPeriodSec = 3600;
            AnalysisPeriodMin = AnalysisPeriodSec / 60;
            TimeStepsPerSecond = 1;
        }

        public int AnalysisPeriodSec { get => _analysisPeriodSec; set => _analysisPeriodSec = value; }
        public int AnalysisPeriodMin { get => _analysisPeriodMin; set => _analysisPeriodMin = value; }
        public int TimeStepsPerSecond { get => _timeStepsPerSecond; set => _timeStepsPerSecond = value; }
    }

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
        float[] _numQueuedVehs;
        float _queueLengthFt;
        float _queueLengthFtPerLane;
        float[] _pctQueueStorageOccupied;
        //float _pctOccupiedLeftQstorage;
        //float _pctOccupiedRightQstorage;
        float _pctTimeMeteringRateMax;

        //TimeIndex, inputs.Traffic.ArrivalFlowRateVehPerHr[ArrivalRateTimePeriodIndex], ArrivalsPerTimeStep, DepartFlowRateVehPerHr, DepartFlowRateVehPerTimeStep, VehServedPerTimeStep, CumulativeArrivals, CumulativeDepartures, QueueLengthVeh, QueueLengthFt, PctTimeMeteringRateMax);

        public ResultsData(float timeStepSec, byte timingStageID, float LeftArrivalsTimeStep, float RightArrivalsTimeStep, float ThruArrivalsTimeStep, float TotalArrivalsTimeStep, int departureRateVPH, float vehServedTimeStep, float cumulativeArrivalsVeh, float cumulativeDeparturesVeh, float[] numQueuedVehs, float queueLengthFt, float queueLengthFtPerLane, float[] pctQstorageOccupied, float pctTimeMeteringRateMax)
        {
            _timeStepSec = timeStepSec;
            _timingStageID = timingStageID;
            //_arrivalRateVehPerHr = arrivalRateVPH;
            //_arrivalsTimeStep = arrivalsTimeStep;
            _arrivalsTimeStep = new float[4];
            _arrivalsTimeStep[0] = LeftArrivalsTimeStep;
            _arrivalsTimeStep[1] = RightArrivalsTimeStep;
            _arrivalsTimeStep[2] = ThruArrivalsTimeStep;
            _arrivalsTimeStep[3] = TotalArrivalsTimeStep;
            _departureRateVehPerHr = departureRateVPH;
            //_departuresTimeStep = departuresTimeStep;
            _vehServedTimeStep = vehServedTimeStep;
            _cumulativeArrivalsVeh = cumulativeArrivalsVeh;
            _cumulativeDeparturesVeh = cumulativeDeparturesVeh;
            //_numQueuedVehs = numQueuedVehs;
            _numQueuedVehs = new float[4];
            _numQueuedVehs[(int)QueuedVehMovement.Total] = numQueuedVehs[(int)QueuedVehMovement.Total];
            _numQueuedVehs[(int)QueuedVehMovement.Left] = numQueuedVehs[(int)QueuedVehMovement.Left];
            _numQueuedVehs[(int)QueuedVehMovement.Right] = numQueuedVehs[(int)QueuedVehMovement.Right];
            _queueLengthFt = queueLengthFt;
            _queueLengthFtPerLane = queueLengthFtPerLane;
            //_pctQueueStorageOccupied = pctQstorageOccupied;
            _pctQueueStorageOccupied = new float[4];
            _pctQueueStorageOccupied[(int)SegmentType.Shared] = pctQstorageOccupied[(int)SegmentType.Shared];
            _pctQueueStorageOccupied[(int)SegmentType.LeftTurn] = pctQstorageOccupied[(int)SegmentType.LeftTurn];
            _pctQueueStorageOccupied[(int)SegmentType.RightTurn] = pctQstorageOccupied[(int)SegmentType.RightTurn];
            _pctTimeMeteringRateMax = pctTimeMeteringRateMax;
        }

        public float TimeStepSec { get => _timeStepSec; set => _timeStepSec = value; }
        public byte TimingStageID { get => _timingStageID; set => _timingStageID = value; }
        public int ArrivalRateVehPerHr { get => _arrivalRateVehPerHr; set => _arrivalRateVehPerHr = value; }
        public int DepartureFlowRateVehPerHr { get => _departureRateVehPerHr; set => _departureRateVehPerHr = value; }
        public float[] ArrivalsTimeStep { get => _arrivalsTimeStep; set => _arrivalsTimeStep = value; }
        public float DeparturesTimeStep { get => _departuresTimeStep; set => _departuresTimeStep = value; }
        public float VehServedTimeStep { get => _vehServedTimeStep; set => _vehServedTimeStep = value; }
        public float[] NumQueuedVehs { get => _numQueuedVehs; set => _numQueuedVehs = value; }
        public float QueueLengthFt { get => _queueLengthFt; set => _queueLengthFt = value; }
        public float QueueLengthFtPerLane { get => _queueLengthFtPerLane; set => _queueLengthFtPerLane = value; }
        public int AddMeteringRateMax { get => _addMeteringRateMax; set => _addMeteringRateMax = value; }        
        public float PctTimeMeteringRateMax { get => _pctTimeMeteringRateMax; set => _pctTimeMeteringRateMax = value; }
        public float CumulativeArrivals { get => _cumulativeArrivalsVeh; set => _cumulativeArrivalsVeh = value; }
        public float CumulativeDepartures { get => _cumulativeDeparturesVeh; set => _cumulativeDeparturesVeh = value; }
        public float[] PctQueueStorageOccupied { get => _pctQueueStorageOccupied; set => _pctQueueStorageOccupied = value; }
        
        //public float PctOccupiedLeftQstorage { get => _pctOccupiedLeftQstorage; set => _pctOccupiedLeftQstorage = value; }
        //public float PctOccupiedRightQstorage { get => _pctOccupiedRightQstorage; set => _pctOccupiedRightQstorage = value; }
    }
}
