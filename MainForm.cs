using System;
using System.Windows.Forms;
using System.Collections.Generic;
using QueueCalcs.DataStructures;


namespace QueueCalcs
{
    public partial class MainForm : Form
    {
        //string FileName = @"X:\OneDrive\Software Projects\QueuingCalcs\Test.xml";
        string FileName = @"..\..\..\InputData.xml";
        List<InterchangeIntersectionData> Intersections;
        byte InterchangeSelection = 1;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {            

            if (Intersections == null)
            {
                //MessageBox.Show("Input data file was not opened, so simulation will be run with default values.", "Input Data Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rtbMessages.Text = "Input data file was not opened, so simulation was run with default values.";

                if (InterchangeSelection == 1)
                    Intersections = CreateOnramp.TightDiamond();
                else
                    Intersections = CreateOnramp.DDI();
                
                btnStart.Text = "Queuing Analysis (Default Values) Ran";
            }
            else
            {
                btnStart.Text = "Queuing Analysis (Loaded Values) Ran";
            }

            if (InterchangeSelection == 1)
                QueueAnalysis.SingleRampSegment(Intersections);
            else
                QueueAnalysis.MultipleRampSegments(Intersections);

        }

        private void btnWriteDataFile_Click(object sender, EventArgs e)
        {            

            if (Intersections == null)
            {
                if (InterchangeSelection == 1)
                    Intersections = CreateOnramp.TightDiamond();
                else
                    Intersections = CreateOnramp.DDI();
            }                
            else
                btnWriteDataFile.Text = "Ouput Successful";

            FileInputOutput.SerializeInputData(FileName, Intersections);
        }

        private void btnReadDataFile_Click(object sender, EventArgs e)
        {
            btnReadDataFile.Text = "Input Successful";
            Intersections = FileInputOutput.DeserializeInputData(FileName);

            foreach (InterchangeIntersectionData intersection in Intersections)
                intersection.Vehicles.SetValues(intersection.Vehicles.StopGapValues, intersection.Vehicles.SmallAutoLengthFt, intersection.Vehicles.LargeAutoLengthFt, intersection.Vehicles.SmallTruckLengthFt, intersection.Vehicles.LargeTruckLengthFt, intersection.Traffic);
        }
    }
}
