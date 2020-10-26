using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using QueueCalcs.DataStructures;


namespace QueueCalcs
{
    public class FileInputOutput
    {

        public static void SerializeInputData(string filename, List<InterchangeIntersectionData> inputs)
        {
            FileStream myStream = new FileStream(filename, FileMode.Create);
            XmlSerializer mySerializer = new XmlSerializer(typeof(List<InterchangeIntersectionData>));

            mySerializer.Serialize(myStream, inputs);
            myStream.Close();
        }

        public static List<InterchangeIntersectionData> DeserializeInputData(string filename)
        {
            FileStream myFileStream = new FileStream(filename, FileMode.Open);
            XmlSerializer mySerializer = new XmlSerializer(typeof(List<InterchangeIntersectionData>));
            List<InterchangeIntersectionData> inputs = (List<InterchangeIntersectionData>)mySerializer.Deserialize(myFileStream);
            myFileStream.Close();

            return inputs;
        }


        public static void WriteResultsData(string outputFilename, List<ResultsData> results)
        {
            StreamWriter swResults = new StreamWriter(outputFilename, false);
                        
            //swResults.WriteLine("Time Step (s), Timing Stage ID, On-Ramp Arrivals LT (veh/ts), On-Ramp Arrivals RT (veh/ts), On-Ramp Arrivals Thru (veh/ts), On-Ramp Arrivals Time Step Total (veh/ts), Metering Rate (veh/h), Veh Served (veh), Cumulative Arrivals (Veh), Cumulative Departures (veh), Num Queued Veh, Queue Length (ft), Queue Length (ft/lane), %Occ Shared Q Storage , %Occ Left Q Storage, %Occ Right Q Storage, % Time Max Meter Rate");
            swResults.WriteLine("Time Step (s), Timing Stage ID, On-Ramp Arrivals LT (veh/ts), On-Ramp Arrivals RT (veh/ts), On-Ramp Arrivals Thru (veh/ts), On-Ramp Arrivals Time Step Total (veh/ts), Metering Rate (veh/h), Veh Served (veh), Cumulative Arrivals (Veh), Cumulative Departures (veh), Num Queued Veh, Queue Length (ft), Queue Length (ft/lane), %Occ Shared Q Storage , % Time Max Meter Rate");

            foreach (ResultsData result in results)
            {
                swResults.Write(result.TimeStepSec);
                swResults.Write(",");
                swResults.Write(result.TimingStageID);
                swResults.Write(",");
                //swResults.Write(result.ArrivalRateVehPerHr);
                //swResults.Write(",");
                swResults.Write(result.ArrivalsTimeStep[(int)QueuedVehMovement.Left]);
                swResults.Write(",");
                swResults.Write(result.ArrivalsTimeStep[(int)QueuedVehMovement.Right]);
                swResults.Write(",");
                swResults.Write(result.ArrivalsTimeStep[(int)QueuedVehMovement.Thru]);
                swResults.Write(",");
                swResults.Write(result.ArrivalsTimeStep[(int)QueuedVehMovement.Total]);
                swResults.Write(",");
                swResults.Write(result.DepartureFlowRateVehPerHr); //metering rate
                swResults.Write(",");
                //swResults.Write(result.DeparturesTimeStep);
                //swResults.Write(",");
                swResults.Write(result.VehServedTimeStep);
                swResults.Write(",");
                swResults.Write(result.CumulativeArrivals);
                swResults.Write(",");
                swResults.Write(result.CumulativeDepartures);
                swResults.Write(",");
                swResults.Write(result.NumQueuedVehs[(int)QueuedVehMovement.Total]);
                swResults.Write(",");
                swResults.Write(result.QueueLengthFt.ToString("0.0"));
                swResults.Write(",");
                swResults.Write(result.QueueLengthFtPerLane.ToString("0.0"));
                swResults.Write(",");
                swResults.Write(result.PctQueueStorageOccupied[(int)SegmentType.Shared].ToString("0.0"));
                swResults.Write(",");
                //swResults.Write(result.PctQueueStorageOccupied[(int)SegmentType.LeftTurn].ToString("0.0"));
                //swResults.Write(",");
                //swResults.Write(result.PctQueueStorageOccupied[(int)SegmentType.RightTurn].ToString("0.0"));
                //swResults.Write(",");
                swResults.Write(result.PctTimeMeteringRateMax.ToString("0.00"));
                swResults.WriteLine();
            }

            swResults.Close();

        }


    }
}
