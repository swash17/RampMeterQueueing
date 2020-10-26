using System;
using System.Collections.Generic;
using System.Xml.Serialization;



namespace QueueCalcs.DataStructures
{
        

    public class FlowRateData
    {
        float _arrivalsVehPerHr;
        float _arrivalsVehPerMin;
        float _arrivalsVehPerTimeStep;  //defaults to 1 second
        float _arrivalsVehPerCycle;
        float _departuresVehPerHr;
        float _departuresVehPerTimeStep;

        public FlowRateData()
        { }

        public float ArrivalsVehPerHr { get => _arrivalsVehPerHr; set => _arrivalsVehPerHr = value; }
        [XmlIgnore]
        public float ArrivalsVehPerMin { get => _arrivalsVehPerMin; set => _arrivalsVehPerMin = value; }
        [XmlIgnore]
        public float ArrivalsVehPerTimeStep { get => _arrivalsVehPerTimeStep; set => _arrivalsVehPerTimeStep = value; }
        [XmlIgnore]
        public float ArrivalsVehPerCycle { get => _arrivalsVehPerCycle; set => _arrivalsVehPerCycle = value; }
        [XmlIgnore]
        public float DeparturesVehPerHr { get => _departuresVehPerHr; set => _departuresVehPerHr = value; }
        [XmlIgnore]
        public float DeparturesVehPerTimeStep { get => _departuresVehPerTimeStep; set => _departuresVehPerTimeStep = value; }        
    }


    [XmlRoot(ElementName = "InterchangeIntersection")]
    public class InterchangeIntersectionData
    {
        byte _id;
        List<IntersectionMovementData> _movements;
        bool _isSignalControlled;
        SignalData _signal;
        OnRampData _onRamp;
        int _flowRateUpdateIntervalDefaultSec;
        FlowRateData _flowRate;
        TrafficStreamData _traffic;
        VehicleData _vehicles;

        public InterchangeIntersectionData()
        {
            //for de/serialization
        }

        public InterchangeIntersectionData(byte id, List<IntersectionMovementData> movements, bool isSignalControlled, OnRampData onRamp)
        {
            _id = id;
            _movements = movements;
            _isSignalControlled = isSignalControlled;
            if (_isSignalControlled == true)
                _signal = new SignalData();

            _onRamp = onRamp;
            _flowRateUpdateIntervalDefaultSec = 60;
            _flowRate = new FlowRateData();
        }

        [XmlAttribute("ID")]
        public byte Id { get => _id; set => _id = value; }
        public List<IntersectionMovementData> Movements { get => _movements; set => _movements = value; }
        [XmlIgnore]
        public int FlowRateUpdateIntervalDefaultSec { get => _flowRateUpdateIntervalDefaultSec; set => _flowRateUpdateIntervalDefaultSec = value; }
        public OnRampData OnRamp { get => _onRamp; set => _onRamp = value; }
        public bool IsSignalControlled { get => _isSignalControlled; set => _isSignalControlled = value; }
        public SignalData Signal { get => _signal; set => _signal = value; }
        [XmlIgnore]
        public FlowRateData FlowRate { get => _flowRate; set => _flowRate = value; }
        public TrafficStreamData Traffic { get => _traffic; set => _traffic = value; }
        public VehicleData Vehicles { get => _vehicles; set => _vehicles = value; }
    }


    [XmlRoot(ElementName = "Movement")]
    public class IntersectionMovementData
    {
        byte _id;
        int _associatedRampId;
        string _label;
        NemaMovementNumbers _nemaPhaseId;
        bool _isSignalControlled;
        FlowRateData _flowRate;       

        public IntersectionMovementData()
        { }

        public IntersectionMovementData(byte id, string label, NemaMovementNumbers nemaPhaseId, int associatedRampId, float demandVolVehPerHr, bool isSignalControlled)
        {
            _id = id;
            _label = label;
            _nemaPhaseId = nemaPhaseId;
            _associatedRampId = associatedRampId;
            _flowRate = new FlowRateData();
            _flowRate.ArrivalsVehPerHr = demandVolVehPerHr;  //is _arrivalFlowRateVehPerHr needed in input file if it is taking value from demandVolVehPerHr ? ABC
            _isSignalControlled = isSignalControlled;
        }

        [XmlAttribute("ID")]
        public byte Id { get => _id; set => _id = value; }
        public int AssociatedRampId { get => _associatedRampId; set => _associatedRampId = value; }
        public string Label { get => _label; set => _label = value; }
        public NemaMovementNumbers NemaPhaseId { get => _nemaPhaseId; set => _nemaPhaseId = value; }
        public bool IsSignalControlled { get => _isSignalControlled; set => _isSignalControlled = value; }
        public FlowRateData FlowRate { get => _flowRate; set => _flowRate = value; }
    }

}
