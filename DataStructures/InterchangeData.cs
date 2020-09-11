using System;
using System.Collections.Generic;




namespace QueueCalcs.DataStructures
{

    public class AnalysisData
    {
        int _analysisPeriodSec;
        int _analysisPeriodMin;

        public AnalysisData()
        {
            AnalysisPeriodSec = 3600;
            AnalysisPeriodMin = AnalysisPeriodSec / 60;
        }

        public int AnalysisPeriodSec { get => _analysisPeriodSec; set => _analysisPeriodSec = value; }
        public int AnalysisPeriodMin { get => _analysisPeriodMin; set => _analysisPeriodMin = value; }
    }

    public class InterchangeData
    {
        List<IntersectionData> _intersections;        
        TrafficStreamData _traffic;
        VehicleData _vehicles;
        
        public InterchangeData()
        { }

        public InterchangeData(List<IntersectionData> intersections)
        {
            _intersections = intersections;
            _traffic = new TrafficStreamData();
            _vehicles = new VehicleData(_traffic);
        }

        public List<IntersectionData> Intersections { get => _intersections; set => _intersections = value; }
        public TrafficStreamData Traffic { get => _traffic; set => _traffic = value; }
        public VehicleData Vehicles { get => _vehicles; set => _vehicles = value; }        
        
    }

       

    

}
