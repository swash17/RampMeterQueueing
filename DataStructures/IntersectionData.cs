using System;
using System.Collections.Generic;
using System.Xml.Serialization;



namespace QueueCalcs.DataStructures
{

    
    public class IntersectionData
    {
        byte _id;
        List<IntersectionMovementData> _movements;
        bool _isSignalControlled;
        SignalData _signal;
        OnRampData _onRamp;
        int _flowRateUpdateIntervalDefaultSec;

        public IntersectionData()
        {
            //for de/serialization
        }

        public IntersectionData(byte id, List<IntersectionMovementData> movements, bool isSignalControlled, OnRampData onRamp)
        {
            _id = id;            
            _movements = movements;
            _isSignalControlled = isSignalControlled;
            if (_isSignalControlled == true)
                _signal = new SignalData();

            _onRamp = onRamp;
            _flowRateUpdateIntervalDefaultSec = 60;
        }

        public byte Id { get => _id; set => _id = value; }
        public List<IntersectionMovementData> Movements { get => _movements; set => _movements = value; }        
        public int FlowRateUpdateIntervalDefaultSec { get => _flowRateUpdateIntervalDefaultSec; set => _flowRateUpdateIntervalDefaultSec = value; }
        public OnRampData OnRamp { get => _onRamp; set => _onRamp = value; }
        public bool IsSignalControlled { get => _isSignalControlled; set => _isSignalControlled = value; }
        public SignalData Signal { get => _signal; set => _signal = value; }        
    }

    public enum QueuedVehMovement : int
    {
        Left = 0,
        Thru = 1,
        Right = 2,
        Total = 3,
    }

    public class IntersectionMovementData
    {
        //byte _id;
        int _associatedRampId;
        string _label;
        NemaMovementNumbers _nemaPhaseId;
        bool _isSignalControlled;
        float _arrivalFlowRateVehPerHr;
        float _arrivalFlowRateVehPerMin;
        float _arrivalFlowRateVehPerCycle;
        float _departureFlowRateVehPerHr;
        float _departureFlowRateVehPerSec;        

        public IntersectionMovementData()
        { }

        public IntersectionMovementData(string label, NemaMovementNumbers nemaPhaseId, int associatedRampId, float demandVolVehPerHr, bool isSignalControlled)
        {
            _label = label;
            _nemaPhaseId = nemaPhaseId;
            _associatedRampId = associatedRampId;
            _arrivalFlowRateVehPerHr = demandVolVehPerHr;
            _isSignalControlled = isSignalControlled;
        }

        //public byte Id { get => _id; set => _id = value; }
        public int AssociatedRampId { get => _associatedRampId; set => _associatedRampId = value; }
        public string Label { get => _label; set => _label = value; }
        public NemaMovementNumbers NemaPhaseId { get => _nemaPhaseId; set => _nemaPhaseId = value; }
        public float ArrivalFlowRateVehPerHr { get => _arrivalFlowRateVehPerHr; set => _arrivalFlowRateVehPerHr = value; }
        [XmlIgnore]
        public float ArrivalFlowRateVehPerMin { get => _arrivalFlowRateVehPerMin; set => _arrivalFlowRateVehPerMin = value; }
        [XmlIgnore]
        public float ArrivalFlowRateVehPerCycle { get => _arrivalFlowRateVehPerCycle; set => _arrivalFlowRateVehPerCycle = value; }
        [XmlIgnore]
        public float DepartureFlowRateVehPerHr { get => _departureFlowRateVehPerHr; set => _departureFlowRateVehPerHr = value; }
        [XmlIgnore]
        public float DepartureFlowRateVehPerSec { get => _departureFlowRateVehPerSec; set => _departureFlowRateVehPerSec = value; }
        public bool IsSignalControlled { get => _isSignalControlled; set => _isSignalControlled = value; }
        
    }

}
