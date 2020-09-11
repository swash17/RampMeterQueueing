using System;
using System.Collections.Generic;



namespace QueueCalcs.DataStructures
{

    public enum QueueStorage : int
    {
        Shared = 0,
        Left = 1,
        Right = 2,
        Thru = 3
    }

    public class OnRampData
    {
        byte _id;
        //List<LaneData> _lanes;
        byte[] _numLanes;  //0-shared, 1-left, 2-right, 3-total
        float[] _queueStorageDistFt; //0-shared, 1-left, 2-right, 3-total
        float[] _queueStorageLaneFt; //0-shared, 1-left, 2-right, 3-total
        float[] _queueStorageCapacityVeh; //0-shared, 1-left, 2-right, 3-total
        RampMeteringData _meter;
        List<RampQueueDetector> _queueDetectors;
        QueuingAnalysisResults _queuingResults;

        

        public OnRampData()
        {
            _numLanes = new byte[4];
            _meter = new RampMeteringData();
            _queuingResults = new QueuingAnalysisResults();
            _queueStorageCapacityVeh = new float[4];
        }

        public OnRampData(byte id, byte numLanesSharedStorage, byte numLanesLeftTurnStorage, byte numLanesRightTurnStorage, float queueStorageSharedDistFt, float queueStorageLeftTurnDistFt, float queueStorageRightTurnDistFt, List<RampQueueDetector> queueDetectors)
        {
            _id = id;
            _numLanes = new byte[4];
            _numLanes[(int)QueueStorage.Shared] = numLanesSharedStorage;
            _numLanes[(int)QueueStorage.Left] = numLanesLeftTurnStorage;
            _numLanes[(int)QueueStorage.Right] = numLanesRightTurnStorage;

            _queueStorageDistFt = new float[4];
            _queueStorageDistFt[(int)QueueStorage.Shared] = queueStorageSharedDistFt;
            _queueStorageDistFt[(int)QueueStorage.Left] = queueStorageLeftTurnDistFt;
            _queueStorageDistFt[(int)QueueStorage.Right] = queueStorageRightTurnDistFt;

            _queueStorageLaneFt = new float[4];
            _queueStorageLaneFt[(int)QueueStorage.Shared] = _queueStorageDistFt[(int)QueueStorage.Shared] * _numLanes[(int)QueueStorage.Shared];
            _queueStorageLaneFt[(int)QueueStorage.Left] = _queueStorageDistFt[(int)QueueStorage.Left] * _numLanes[(int)QueueStorage.Left];
            _queueStorageLaneFt[(int)QueueStorage.Right] = _queueStorageDistFt[(int)QueueStorage.Right] * _numLanes[(int)QueueStorage.Right];

            _queueStorageCapacityVeh = new float[4];
            _meter = new RampMeteringData();
            _queueDetectors = queueDetectors;  // new List<RampQueueDetector>();
            _queuingResults = new QueuingAnalysisResults();

            foreach (RampQueueDetector queueDetector in _queueDetectors)
            {
                if (queueDetector.DistanceUpstreamFromMeterFt < _queueStorageDistFt[0])
                {
                    queueDetector.DistanceLaneFt = queueDetector.DistanceUpstreamFromMeterFt * _numLanes[(int)QueueStorage.Shared];
                }
                else
                {
                    //if (queueDetector.Movement == DetectorMovement.Left)
                    queueDetector.DistanceLaneFt = (queueDetector.DistanceUpstreamFromMeterFt - _queueStorageDistFt[(int)QueueStorage.Shared]) + _queueStorageLaneFt[(int)QueueStorage.Shared];
                }

                //_intermediateQueueDetectorDistanceLaneFt = _queueStorageLaneFt[0] * _numLanes[0] + _queueStorageLaneFt[1] * _numLanes[1] + _queueStorageLaneFt[2] * _numLanes[2];
            }

           
            //IntermediateQueueDetectorLengthFt = 6;
            //AdvanceQueueDetectorLengthFt = 6;
        }

        public byte Id { get => _id; set => _id = value; }
        public byte[] NumLanes { get => _numLanes; set => _numLanes = value; }
        public float[] QueueStorageLaneFt { get => _queueStorageLaneFt; set => _queueStorageLaneFt = value; }
        //public float QueueStorageRightTurnOnlyLaneFt { get => _queueStorageRightTurnOnlyLaneFt; set => _queueStorageRightTurnOnlyLaneFt = value; }
        //public float QueueStorageLeftTurnOnlyLaneFt { get => _queueStorageLeftTurnOnlyLaneFt; set => _queueStorageLeftTurnOnlyLaneFt = value; }
        public float[] QueueStorageCapacityVeh { get => _queueStorageCapacityVeh; set => _queueStorageCapacityVeh = value; }
        public RampMeteringData Meter { get => _meter; set => _meter = value; }
        public QueuingAnalysisResults QueuingResults { get => _queuingResults; set => _queuingResults = value; }        
        
        public float[] QueueStorageDistFt { get => _queueStorageDistFt; set => _queueStorageDistFt = value; }
        public List<RampQueueDetector> QueueDetectors { get => _queueDetectors; set => _queueDetectors = value; }
    }

    public enum DetectorType
    {
        IntermediateQueue,
        AdvanceQueue
    }

    public enum DetectorMovement
    {
        All,
        Left,
        Right
    }


    public class RampQueueDetector
    {
        DetectorType _type;
        DetectorMovement _movement;
        float _distanceUpstreamFromMeterFt;
        float _distanceLaneFt;
        //float _lengthFt;

        public RampQueueDetector()
        {
            //de/serialization
        }

        public RampQueueDetector(DetectorType type, DetectorMovement movement, float distUpstreamFromMeterFt)
        {
            _type = type;
            _movement = movement;
            _distanceUpstreamFromMeterFt = distUpstreamFromMeterFt;             // From ramp meter stop bar

            
        }

        public DetectorType Type { get => _type; set => _type = value; }
        public DetectorMovement Movement { get => _movement; set => _movement = value; }
        public float DistanceUpstreamFromMeterFt { get => _distanceUpstreamFromMeterFt; set => _distanceUpstreamFromMeterFt = value; }
        public float DistanceLaneFt { get => _distanceLaneFt; set => _distanceLaneFt = value; }
        
    }

    public class QueuingAnalysisResults
    {
        float[] _arrivalsPerCycle;     //indexed by timing stage
        int[] _arrivalsPerHour;
        float[] _arrivalsPerSec;  //0-left, 1-thru, 2-right, 3-total

        float _cumulativeArrivals;
        float _vehServedPerTimeStep;
        int _departFlowRateVehPerHr;
        float _departFlowRateVehPerTimeStep;
        float _cumulativeDepartures;

        float[] _numVehsInQueue;   //0-left, 1-thru, 2-right, 3-total
        float[] _queueLengthFt;    //0-left, 1-thru, 2-right, 3-total
        float[] _pctQueueStorageOccupied; //0-left, 1-thru, 2-right, 3-total

        float _queueLengthVehPreviousTimeStep;
        float _deltaTimeStepQueueLengthVeh;
        byte _arrivalRateTimePeriodIndex;  //Input arrival flow rates are in veh/h, but for 15-min periods


        public QueuingAnalysisResults()
        {
            _arrivalsPerCycle = new float[4];
            _arrivalsPerSec = new float[4];
            _arrivalsPerHour = new int[4];
            _numVehsInQueue = new float[4];
            _queueLengthFt = new float[4];
        }

        public float[] ArrivalsPerSec { get => _arrivalsPerSec; set => _arrivalsPerSec = value; }
        public float[] NumVehsInQueue { get => _numVehsInQueue; set => _numVehsInQueue = value; }
        public float[] QueueLengthFt { get => _queueLengthFt; set => _queueLengthFt = value; }
        public float DepartFlowRateVehPerTimeStep { get => _departFlowRateVehPerTimeStep; set => _departFlowRateVehPerTimeStep = value; }
        public int DepartFlowRateVehPerHr { get => _departFlowRateVehPerHr; set => _departFlowRateVehPerHr = value; }
        public float QueueLengthVehPreviousTimeStep { get => _queueLengthVehPreviousTimeStep; set => _queueLengthVehPreviousTimeStep = value; }
        public float DeltaTimeStepQueueLengthVeh { get => _deltaTimeStepQueueLengthVeh; set => _deltaTimeStepQueueLengthVeh = value; }
        public byte ArrivalRateTimePeriodIndex { get => _arrivalRateTimePeriodIndex; set => _arrivalRateTimePeriodIndex = value; }
        public float CumulativeArrivals { get => _cumulativeArrivals; set => _cumulativeArrivals = value; }
        public float[] ArrivalsPerCycle { get => _arrivalsPerCycle; set => _arrivalsPerCycle = value; }
        public float VehServedPerTimeStep { get => _vehServedPerTimeStep; set => _vehServedPerTimeStep = value; }
        public float CumulativeDepartures { get => _cumulativeDepartures; set => _cumulativeDepartures = value; }
        public int[] ArrivalsPerHour { get => _arrivalsPerHour; set => _arrivalsPerHour = value; }
        public float[] PctQueueStorageOccupied { get => _pctQueueStorageOccupied; set => _pctQueueStorageOccupied = value; }
    }

}
