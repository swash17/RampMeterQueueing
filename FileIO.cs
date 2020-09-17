using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using QueueCalcs.DataStructures;


namespace QueueCalcs
{
    public class FileInputOutput
    {

        public static void SerializeInputData(string filename, InterchangeData inputs)
        {
            FileStream myStream = new FileStream(filename, FileMode.Create);
            XmlSerializer mySerializer = new XmlSerializer(typeof(InterchangeData));

            mySerializer.Serialize(myStream, inputs);
            myStream.Close();
        }

        public static InterchangeData DeserializeInputData(string filename)
        {
            FileStream myFileStream = new FileStream(filename, FileMode.Open);
            XmlSerializer mySerializer = new XmlSerializer(typeof(InterchangeData));
            InterchangeData inputs = (InterchangeData)mySerializer.Deserialize(myFileStream);
            myFileStream.Close();

            return inputs;
        }


        public static void WriteResultsData(string outputFilename, List<ResultsData> results)
        {
            StreamWriter swResults = new StreamWriter(outputFilename, false);

            //swResults.WriteLine("Time Step (sec), Timing Stage ID, On-Ramp Arrivals Time Step [Left], On-Ramp Arrivals Time Step [Thru], On-Ramp Arrivals Time Step [Right], On-Ramp Arrivals Time Step [Total], Departures Time Step, Veh Served Time Step, Cumulative Arrivals (Veh), Cumulative Departures (veh), Queue Length (veh), Queue Length (ft), % Queue Storage Occupied, % Time Max Meter Rate");
            swResults.WriteLine("Time Step (sec), Timing Stage ID, On-Ramp Arrivals Time Step [Left], On-Ramp Arrivals Time Step [Thru], On-Ramp Arrivals Time Step [Right], On-Ramp Arrivals Time Step [Total], Departures Time Step, Veh Served Time Step, Cumulative Arrivals (Veh), Cumulative Departures (veh), Queue Length (veh), Queue Length (ft), % Time Max Meter Rate");


            foreach (ResultsData result in results)
            {
                swResults.Write(result.TimeStepSec);
                swResults.Write(",");
                swResults.Write(result.TimingStageID);
                swResults.Write(",");
                //swResults.Write(result.ArrivalRateVehPerHr);
                //swResults.Write(",");
                swResults.Write(result.ArrivalsTimeStep[0]);
                swResults.Write(",");
                swResults.Write(result.ArrivalsTimeStep[1]);
                swResults.Write(",");
                swResults.Write(result.ArrivalsTimeStep[2]);
                swResults.Write(",");
                swResults.Write(result.ArrivalsTimeStep[3]);
                swResults.Write(",");
                //swResults.Write(result.DepartureFlowRateVehPerHr);
                //swResults.Write(",");
                swResults.Write(result.DeparturesTimeStep);
                swResults.Write(",");
                swResults.Write(result.VehServedTimeStep);
                swResults.Write(",");
                swResults.Write(result.CumulativeArrivals);
                swResults.Write(",");
                swResults.Write(result.CumulativeDepartures);
                swResults.Write(",");
                swResults.Write(result.QueueLengthVeh);
                swResults.Write(",");
                swResults.Write(result.QueueLengthFt);
                swResults.Write(",");
                swResults.Write(result.PctTimeMeteringRateMax.ToString("0.00"));
                swResults.WriteLine();
            }

            swResults.Close();

        }


    }
}
