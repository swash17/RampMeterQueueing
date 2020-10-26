using System;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace QueueCalcs.DataStructures
{

    public enum SegmentType : int
    {
        Shared = 0,
        LeftTurn = 1,
        RightTurn = 2,
        Thru = 3
    }

    public enum QueuedVehMovement : int
    {
        Left = 0,
        Right = 1,
        Thru = 2,        
        Total = 3,
    }


    public class OnRampSegmentData
    {
        byte _id;
        SegmentType _type;
        byte _numLanes;
        //float _numVehiclesQueued;
        float _queueStorageDistFt;
        float _queueStorageLaneFt;
        float _queueStorageCapacityVeh;
        float _queueStorageAvailableLaneFt;
        FlowRateData _flowRate;
        QueuingAnalysisResults _queuingResults;


        public OnRampSegmentData()
        { }

        public OnRampSegmentData(byte id, SegmentType type, byte numLanes, float queueStorageDistFt) //, List<RampQueueDetector> queueDetectors)
        {
            _id = id;
            _type = type;
            _numLanes = numLanes;
            _queueStorageDistFt = queueStorageDistFt;
            _queueStorageLaneFt = _queueStorageDistFt * _numLanes;
            _flowRate = new FlowRateData();
            _queuingResults = new QueuingAnalysisResults();
        }

        [XmlAttribute("ID")]
        public byte Id { get => _id; set => _id = value; }
        public SegmentType Type { get => _type; set => _type = value; }
        public byte NumLanes { get => _numLanes; set => _numLanes = value; }
        //public float NumVehiclesQueued { get => _numVehiclesQueued; set => _numVehiclesQueued = value; }
        public float QueueStorageDistPerLaneFt { get => _queueStorageDistFt; set => _queueStorageDistFt = value; }
        [XmlIgnore]
        public float QueueStorageLaneFt { get => _queueStorageLaneFt; set => _queueStorageLaneFt = value; }
        [XmlIgnore]
        public float QueueStorageCapacityVeh { get => _queueStorageCapacityVeh; set => _queueStorageCapacityVeh = value; }
        [XmlIgnore]
        public float QueueStorageAvailableLaneFt { get => _queueStorageAvailableLaneFt; set => _queueStorageAvailableLaneFt = value; }
        [XmlIgnore]
        public FlowRateData FlowRate { get => _flowRate; set => _flowRate = value; }
        [XmlIgnore]
        public QueuingAnalysisResults Results { get => _queuingResults; set => _queuingResults = value; }

    }


    public class OnRampData
    {
        byte _id;
        string _label;
        List<OnRampSegmentData> _segments;        
        RampMeteringData _meter;
        List<RampQueueDetector> _queueDetectors;
                

        public OnRampData()
        {
            _segments = new List<OnRampSegmentData>();
            _queueDetectors = new List<RampQueueDetector>();
            _meter = new RampMeteringData();            
        }
                
        
        public OnRampData(byte id, string label, List<OnRampSegmentData> segments, List<RampQueueDetector> queueDetectors)
        {
            _id = id;
            _label = label;
            _segments = segments;            
            _queueDetectors = queueDetectors;

            _meter = new RampMeteringData(240, 180, 900);            

            int IntQDetIndex = _queueDetectors.FindIndex(detector => detector.Movement.Equals(DetectorMovement.All));
            int AdvQDetLeftIndex = _queueDetectors.FindIndex(detector => detector.Movement.Equals(DetectorMovement.Left));
            int AdvQDetRightDetectorIndex = _queueDetectors.FindIndex(detector => detector.Movement.Equals(DetectorMovement.Right));

            int SharedSegmentIndex = _segments.FindIndex(segment => segment.Type.Equals(SegmentType.Shared));
            int LeftTurnSegmentIndex = _segments.FindIndex(segment => segment.Type.Equals(SegmentType.LeftTurn));
            int RightTurnSegmentIndex = _segments.FindIndex(segment => segment.Type.Equals(SegmentType.RightTurn));

            foreach (RampQueueDetector queueDetector in _queueDetectors)
            {
                if (queueDetector.Movement == DetectorMovement.All)
                {
                    queueDetector.DistanceLaneFt = queueDetector.DistanceUpstreamFromMeterFt * _segments[SharedSegmentIndex].NumLanes;
                }
                else if (queueDetector.Movement == DetectorMovement.Left)
                {
                    queueDetector.DistanceLaneFt = (queueDetector.DistanceUpstreamFromMeterFt - _segments[LeftTurnSegmentIndex].QueueStorageDistPerLaneFt) + _segments[LeftTurnSegmentIndex].QueueStorageLaneFt;
                }
                else if (queueDetector.Movement == DetectorMovement.Right)
                {                    
                    queueDetector.DistanceLaneFt = (queueDetector.DistanceUpstreamFromMeterFt - _segments[RightTurnSegmentIndex].QueueStorageDistPerLaneFt) + _segments[RightTurnSegmentIndex].QueueStorageLaneFt;
                }
            }
           
            //IntermediateQueueDetectorLengthFt = 6;
            //AdvanceQueueDetectorLengthFt = 6;
        }

        [XmlAttribute("ID")]
        public byte Id { get => _id; set => _id = value; }
        public string Label { get => _label; set => _label = value; }        
        public RampMeteringData Meter { get => _meter; set => _meter = value; }
        public List<OnRampSegmentData> Segments { get => _segments; set => _segments = value; }
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
        byte _id;
        DetectorType _type;
        DetectorMovement _movement;
        float _distanceUpstreamFromMeterFt;
        float _distanceLaneFt;
        //float _lengthFt;
        SegmentType _includedInSegType;

        public RampQueueDetector()
        {
            //de/serialization
        }

        public RampQueueDetector(byte id, DetectorType type, DetectorMovement movement, float distUpstreamFromMeterFt) //, SegmentType includedInSegType)
        {
            _id = id;
            _type = type;
            _movement = movement;
            _distanceUpstreamFromMeterFt = distUpstreamFromMeterFt;             // From ramp meter stop bar
            //_includedInSegType = includedInSegType;
        }

        [XmlAttribute("ID")]
        public byte Id { get => _id; set => _id = value; }
        public DetectorType Type { get => _type; set => _type = value; }
        public DetectorMovement Movement { get => _movement; set => _movement = value; }
        public float DistanceUpstreamFromMeterFt { get => _distanceUpstreamFromMeterFt; set => _distanceUpstreamFromMeterFt = value; }
        [XmlIgnore]
        public float DistanceLaneFt { get => _distanceLaneFt; set => _distanceLaneFt = value; }
        public SegmentType IncludedInSegType { get => _includedInSegType; set => _includedInSegType = value; }
    }

    public class QueuingAnalysisResults
    {
        //float[] _arrivalsPerCycle;     //indexed by timing stage
        //int[] _arrivalsPerHour;
        float[] _arrivalsPerTimeStep;  //0-left, 1-thru, 2-right, 3-total

        float[] _cumulativeArrivals;
        float _vehServedPerTimeStep;
        int _departFlowRateVehPerHr;
        float _departFlowRateVehPerTimeStep;
        float _cumulativeDepartures;

        float[] _numVehsInQueue;  //indexed with the enum 'QueuedVehMovement' (0-left, 1-thru, 2-right, 3-total)
        float[] _queueLengthFt;   //indexed with the enum 'QueuedVehMovement'
        float[] _queueLengthFtPerLane;
        float[] _pctQueueStorageOccupied; //indexed with the enum 'QueuedVehMovement'

        float _queueLengthVehPreviousTimeStep;
        float _deltaTimeStepQueueLengthVeh;
        byte _arrivalRateTimePeriodIndex;  //Input arrival flow rates are in veh/h, but for 15-min periods


        public QueuingAnalysisResults()
        {
            //_arrivalsPerCycle = new float[4];            
            // _arrivalsPerHour = new int[4];
            _arrivalsPerTimeStep = new float[4];
            _numVehsInQueue = new float[4];
            _queueLengthFt = new float[4];
            _queueLengthFtPerLane = new float[4];
            _pctQueueStorageOccupied = new float[4];
            _cumulativeArrivals = new float[4];
        }

        //to-do: remove XmlIgnore statements from here and just add to QueuingAnalysisResults where it is referenced from other classes
        [XmlIgnore]
        public float[] ArrivalsPerTimeStep { get => _arrivalsPerTimeStep; set => _arrivalsPerTimeStep = value; }
        [XmlIgnore]
        public float[] NumVehsInQueue { get => _numVehsInQueue; set => _numVehsInQueue = value; }
        [XmlIgnore]
        public float[] QueueLengthFt { get => _queueLengthFt; set => _queueLengthFt = value; }
        [XmlIgnore]
        public float[] QueueLengthFtPerLane { get => _queueLengthFtPerLane; set => _queueLengthFtPerLane = value; }
        [XmlIgnore]
        public float DepartFlowRateVehPerTimeStep { get => _departFlowRateVehPerTimeStep; set => _departFlowRateVehPerTimeStep = value; }
        [XmlIgnore]
        public int DepartFlowRateVehPerHr { get => _departFlowRateVehPerHr; set => _departFlowRateVehPerHr = value; }
        [XmlIgnore]
        public float QueueLengthVehPreviousTimeStep { get => _queueLengthVehPreviousTimeStep; set => _queueLengthVehPreviousTimeStep = value; }
        [XmlIgnore]
        public float DeltaTimeStepQueueLengthVeh { get => _deltaTimeStepQueueLengthVeh; set => _deltaTimeStepQueueLengthVeh = value; }
        [XmlIgnore]
        public byte ArrivalRateTimePeriodIndex { get => _arrivalRateTimePeriodIndex; set => _arrivalRateTimePeriodIndex = value; }
        [XmlIgnore]
        public float[] CumulativeArrivals { get => _cumulativeArrivals; set => _cumulativeArrivals = value; }
        //[XmlIgnore]
        //public float[] ArrivalsPerCycle { get => _arrivalsPerCycle; set => _arrivalsPerCycle = value; }
        [XmlIgnore]
        public float VehServedPerTimeStep { get => _vehServedPerTimeStep; set => _vehServedPerTimeStep = value; }
        [XmlIgnore]
        public float CumulativeDepartures { get => _cumulativeDepartures; set => _cumulativeDepartures = value; }
        //[XmlIgnore]
        //public int[] ArrivalsPerHour { get => _arrivalsPerHour; set => _arrivalsPerHour = value; }
        [XmlIgnore]
        public float[] PctQueueStorageOccupied { get => _pctQueueStorageOccupied; set => _pctQueueStorageOccupied = value; }
        
    }

}
