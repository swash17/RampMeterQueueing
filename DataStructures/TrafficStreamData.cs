using System;
using System.Collections.Generic;


namespace QueueCalcs
{

    public enum ArrivalDistributionType
    {
        uniform,
        Poisson
    }

    public enum AllowedRampEntryMovements
    {
        Left = 0,
        Thru = 1,
        Right = 2
    }

    public class TrafficStreamData
    {
        ArrivalDistributionType _arrivalDist;
        AllowedRampEntryMovements _allowedMovements;
        float _rampPropHeavyVeh;
        float _propSmallAuto;
        float _propLargeAuto;
        float _propSmallTruck;
        float _propLargeTruck;
        //through movement volumes are either not allowed or are negligible at ramp terminals
        //int[] _arrivalFlowRateVPH;  //0-left, 1-thru, 2-right
        int[] _arrivalFlowRateTotalVPH;
        int[] _avgArrivalsPerCycleVeh;
        int[] _arrivalsPerCycleVeh;
        float[] _arrivalFlowRateVehPerSec;

        //public TrafficStreamData()
        //{
        //    //parameterless constructor needed for serialization
        //}

        public TrafficStreamData()
        {
            _arrivalDist = ArrivalDistributionType.Poisson;
            _rampPropHeavyVeh = 0.12f;       // Small + Large Truck % of Node 7011
            _propSmallAuto = 0.6f * (1 - _rampPropHeavyVeh);
            _propLargeAuto = 0.4f * (1 - _rampPropHeavyVeh);
            _propSmallTruck = 0.5f * _rampPropHeavyVeh;
            _propLargeTruck = 0.5f * _rampPropHeavyVeh;

                       

        }

        public ArrivalDistributionType ArrivalDist { get => _arrivalDist; set => _arrivalDist = value; }
        public float RampPropHeavyVeh { get => _rampPropHeavyVeh; set => _rampPropHeavyVeh = value; }
        public float PropSmallAuto { get => _propSmallAuto; set => _propSmallAuto = value; }
        public float PropLargeAuto { get => _propLargeAuto; set => _propLargeAuto = value; }
        public float PropSmallTruck { get => _propSmallTruck; set => _propSmallTruck = value; }
        public float PropLargeTruck { get => _propLargeTruck; set => _propLargeTruck = value; }
        //public int[] ArrivalFlowRateVPH { get => _arrivalFlowRateVPH; set => _arrivalFlowRateVPH = value; }
        public int[] ArrivalFlowRateTotalVPH { get => _arrivalFlowRateTotalVPH; set => _arrivalFlowRateTotalVPH = value; }
        public float[] ArrivalFlowRateVehPerSec { get => _arrivalFlowRateVehPerSec; set => _arrivalFlowRateVehPerSec = value; }
        public AllowedRampEntryMovements AllowedMovements { get => _allowedMovements; set => _allowedMovements = value; }
        public int[] AvgArrivalsPerCycleVeh { get => _avgArrivalsPerCycleVeh; set => _avgArrivalsPerCycleVeh = value; }
        public int[] ArrivalsPerCycleVeh { get => _arrivalsPerCycleVeh; set => _arrivalsPerCycleVeh = value; }
    }
}
