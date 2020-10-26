using System.Xml.Serialization;


namespace QueueCalcs.DataStructures
{

    public enum ArrivalDistributionType
    {
        uniform,
        Poisson
    }       

    public class TrafficStreamData
    {
        ArrivalDistributionType _arrivalDist;        
        float _propSmallAuto;
        float _propLargeAuto;
        float _propSmallTruck;
        float _propLargeTruck;
        float _rampPropHeavyVeh;


        public TrafficStreamData()
        {
            //parameterless constructor needed for serialization
        }

        //public TrafficStreamData(float rampPropHeavyVeh, float propSmallAuto, float propLargeAuto, float propSmallTruck, float propLargeTruck)
        public TrafficStreamData(float propSmallAuto, float propLargeAuto, float propSmallTruck, float propLargeTruck)
        {
            _arrivalDist = ArrivalDistributionType.Poisson;
            /*
            _rampPropHeavyVeh = rampPropHeavyVeh;
            _propSmallAuto = propSmallAuto * (1 - _rampPropHeavyVeh);
            _propLargeAuto = propLargeAuto * (1 - _rampPropHeavyVeh);
            _propSmallTruck = propSmallTruck * _rampPropHeavyVeh;
            _propLargeTruck = propLargeTruck * _rampPropHeavyVeh;
            */

            _propSmallAuto = propSmallAuto;
            _propLargeAuto = propLargeAuto;
            _propSmallTruck = propSmallTruck;
            _propLargeTruck = propLargeTruck;
            _rampPropHeavyVeh = propSmallTruck + propLargeTruck;
        }


        [XmlIgnore]
        public ArrivalDistributionType ArrivalDist { get => _arrivalDist; set => _arrivalDist = value; }        
        public float PropSmallAuto { get => _propSmallAuto; set => _propSmallAuto = value; }
        public float PropLargeAuto { get => _propLargeAuto; set => _propLargeAuto = value; }
        public float PropSmallTruck { get => _propSmallTruck; set => _propSmallTruck = value; }
        public float PropLargeTruck { get => _propLargeTruck; set => _propLargeTruck = value; }
        [XmlIgnore]
        public float RampPropHeavyVeh { get => _rampPropHeavyVeh; set => _rampPropHeavyVeh = value; }

    }


    public class VehicleData
    {
        float[] _stopGapValues;
        float[] _smallAutoLengthFt;
        float[] _largeAutoLengthFt;
        float[] _smallTruckLengthFt;
        float[] _largeTruckLengthFt;
        float _avgSmallAutoLengthFt;
        float _avgLargeAutoLengthFt;
        float _avgSmallTruckLengthFt;
        float _avgLargeTruckLengthFt;
        float _avgVehicleSpacingFt;
        float _avgVehLengthFt;

        //Musth have explict parameterless constructor for serialization
        public VehicleData()
        {
        }

        public VehicleData(TrafficStreamData traffic)
        {
            _stopGapValues = new float[] { 10, 10, 14, 20 };
            _smallAutoLengthFt = new float[] { 14.57f, 16.7f, 16.22f, 14.78f, 15.57f, 15.53f };
            _largeAutoLengthFt = new float[] { 16.4f, 18.98f, 16.94f, 19.31f };
            _smallTruckLengthFt = new float[] { 29 };
            _largeTruckLengthFt = new float[] { 55, 68.5f, 74.6f };

            SetValues(_stopGapValues, _smallAutoLengthFt, _largeAutoLengthFt, _smallTruckLengthFt, _largeTruckLengthFt, traffic);
        }

        //ABC suppressing vehicle length variables since they appear to be set above
        [XmlIgnore]
        public float[] SmallAutoLengthFt { get => _smallAutoLengthFt; set => _smallAutoLengthFt = value; }
        [XmlIgnore]
        public float[] LargeAutoLengthFt { get => _largeAutoLengthFt; set => _largeAutoLengthFt = value; }
        [XmlIgnore]
        public float[] SmallTruckLengthFt { get => _smallTruckLengthFt; set => _smallTruckLengthFt = value; }
        [XmlIgnore]
        public float[] LargeTruckLengthFt { get => _largeTruckLengthFt; set => _largeTruckLengthFt = value; }
        [XmlIgnore]
        public float AvgSmallAutoLengthFt { get => _avgSmallAutoLengthFt; set => _avgSmallAutoLengthFt = value; }
        [XmlIgnore]
        public float AvgLargeAutoLengthFt { get => _avgLargeAutoLengthFt; set => _avgLargeAutoLengthFt = value; }
        [XmlIgnore]
        public float AvgSmallTruckLengthFt { get => _avgSmallTruckLengthFt; set => _avgSmallTruckLengthFt = value; }
        [XmlIgnore]
        public float AvgLargeTruckLengthFt { get => _avgLargeTruckLengthFt; set => _avgLargeTruckLengthFt = value; }
        [XmlIgnore]
        public float AvgVehLengthFt { get => _avgVehLengthFt; set => _avgVehLengthFt = value; }
        [XmlIgnore]
        public float[] StopGapValues { get => _stopGapValues; set => _stopGapValues = value; }
        [XmlIgnore]
        public float AvgVehicleSpacingFt { get => _avgVehicleSpacingFt; set => _avgVehicleSpacingFt = value; }


        public void SetValues(float[] stopGapValues, float[] smallAutoLengthFt, float[] largeAutoLengthFt, float[] smallTruckLengthFt, float[] largeTruckLengthFt, TrafficStreamData traffic)
        {
            float CumulativeVehLength = 0;
            for (int i = 0; i < smallAutoLengthFt.Length; i++)
            {
                CumulativeVehLength += smallAutoLengthFt[i];
            }
            AvgSmallAutoLengthFt = CumulativeVehLength / smallAutoLengthFt.Length;

            CumulativeVehLength = 0;
            for (int i = 0; i < largeAutoLengthFt.Length; i++)
            {
                CumulativeVehLength += largeAutoLengthFt[i];
            }
            AvgLargeAutoLengthFt = CumulativeVehLength / largeAutoLengthFt.Length;

            CumulativeVehLength = 0;
            for (int i = 0; i < smallTruckLengthFt.Length; i++)
            {
                CumulativeVehLength += largeTruckLengthFt[i];
            }
            AvgSmallTruckLengthFt = CumulativeVehLength / smallTruckLengthFt.Length;

            CumulativeVehLength = 0;
            for (int i = 0; i < largeTruckLengthFt.Length; i++)
            {
                CumulativeVehLength += largeTruckLengthFt[i];
            }
            AvgLargeTruckLengthFt = CumulativeVehLength / largeTruckLengthFt.Length;

            AvgVehLengthFt = traffic.PropSmallAuto * AvgSmallAutoLengthFt + traffic.PropLargeAuto * AvgLargeAutoLengthFt + traffic.PropSmallTruck * AvgSmallTruckLengthFt + traffic.PropLargeTruck * AvgLargeTruckLengthFt;

            AvgVehicleSpacingFt = (stopGapValues[0] + AvgSmallAutoLengthFt) * traffic.PropSmallAuto + (stopGapValues[1] + AvgLargeAutoLengthFt) * traffic.PropLargeAuto + (stopGapValues[2] + AvgSmallTruckLengthFt) * traffic.PropSmallTruck + (stopGapValues[3] + AvgLargeTruckLengthFt) * traffic.PropLargeTruck;

        }
    }

}
