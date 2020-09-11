using System;
using System.Windows.Forms;
using QueueCalcs.DataStructures;


namespace QueueCalcs
{
    public partial class MainForm : Form
    {
        InterchangeData Interchange;
        //string FileName = @"X:\OneDrive\Software Projects\QueuingCalcs\Test.xml";
        string FileName = @"..\..\..\InputData.xml";

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (Interchange == null)
            {
                //MessageBox.Show("Input data file was not opened, so simulation will be run with default values.", "Input Data Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rtbMessages.Text = "Input data file was not opened, so simulation was run with default values.";
                Interchange = CreateInterchange.DDI();
                btnStart.Text = "Queuing Analysis (Default Values) Ran";
            }
            else
            {
                btnStart.Text = "Queuing Analysis (Loaded Values) Ran";
            }
            QueueLength.CalculateQueLength(Interchange);
        }

        private void btnWriteDataFile_Click(object sender, EventArgs e)
        {
            if (Interchange == null)
                Interchange = new InterchangeData();
            else
                btnWriteDataFile.Text = "Ouput Successful";
            FileInputOutput.SerializeInputData(FileName, Interchange);
        }

        private void btnReadDataFile_Click(object sender, EventArgs e)
        {
            btnReadDataFile.Text = "Input Successful";
            Interchange = FileInputOutput.DeserializeInputData(FileName);

            Interchange.Vehicles.SetValues(Interchange.Vehicles.StopGapValues, Interchange.Vehicles.SmallAutoLengthFt, Interchange.Vehicles.LargeAutoLengthFt, Interchange.Vehicles.SmallTruckLengthFt, Interchange.Vehicles.LargeTruckLengthFt, Interchange.Traffic);
        }
    }
}
