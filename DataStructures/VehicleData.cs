using System;
using System.Collections.Generic;



namespace QueueCalcs
{
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


        public float[] SmallAutoLengthFt { get => _smallAutoLengthFt; set => _smallAutoLengthFt = value; }
        public float[] LargeAutoLengthFt { get => _largeAutoLengthFt; set => _largeAutoLengthFt = value; }
        public float[] SmallTruckLengthFt { get => _smallTruckLengthFt; set => _smallTruckLengthFt = value; }
        public float[] LargeTruckLengthFt { get => _largeTruckLengthFt; set => _largeTruckLengthFt = value; }
        public float AvgSmallAutoLengthFt { get => _avgSmallAutoLengthFt; set => _avgSmallAutoLengthFt = value; }
        public float AvgLargeAutoLengthFt { get => _avgLargeAutoLengthFt; set => _avgLargeAutoLengthFt = value; }
        public float AvgSmallTruckLengthFt { get => _avgSmallTruckLengthFt; set => _avgSmallTruckLengthFt = value; }
        public float AvgLargeTruckLengthFt { get => _avgLargeTruckLengthFt; set => _avgLargeTruckLengthFt = value; }
        public float AvgVehLengthFt { get => _avgVehLengthFt; set => _avgVehLengthFt = value; }
        public float[] StopGapValues { get => _stopGapValues; set => _stopGapValues = value; }
        public float AvgVehicleSpacingFt { get => _avgVehicleSpacingFt; set => _avgVehicleSpacingFt = value; }


        public void SetValues(float[] stopGapValues, float[] smallAutoLengthFt, float[] largeAutoLengthFt, float[] smallTruckLengthFt, float[] largeTruckLengthFt, TrafficStreamData traffic)
        {
            //AvgSmallAutoLengthFt = (14.57f + 16.7f + 16.22f + 14.78f + 15.57f + 15.53f) / 6;
            //AvgLargeAutoLengthFt = (16.4f + 18.98f + 16.94f + 19.31f) / 4;
            //AvgLargeTruckLengthFt = (55 + 68.5f + 74.6f) / 3;

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
