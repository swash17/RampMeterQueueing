# Ramp Metering Queuing Calculations User Guide

## Introduction
[//]: # (This syntax works like a comment, and won't appear in any output.)

This program performs macroscopic queuing calculations for an on-ramp with metering activated. It is intended to help identify the necessary amount of queue storage space on the on-ramp to avoid repeated spill-back of the queue onto the adjoining arterial roadway. Alternatively, it can be used to identify maximum volumes that can be accommodated from the various on-ramp-feeding arterial movements without creating a perpetual queue spill-back problem. This document provides instructions on how to run the program, edit the input file, create graphs from the output file, and work directly with the source code.

## Working with the Program
There are two ways to work with the program, described as follows.

### Downloading and Running the Executable File
If you want to run the compiled version of the program, you can download the latest 'Release' package (see the link in the right-side panel of this screen). Download this program executable file (QueueCalcs.exe) to any folder on your computer. If you are not able to download the file, check your settings for [Windows Defender SmartScreen](https://docs.microsoft.com/en-us/windows/security/threat-protection/microsoft-defender-smartscreen/microsoft-defender-smartscreen-overview "https://docs.microsoft.com/en-us/windows/security/threat-protection/microsoft-defender-smartscreen/microsoft-defender-smartscreen-overview").

The program requires Version 4.5, or higher, of the Microsoft .NET Framework. If the program fails to run after launching QueueCalcs.exe, check which version(s) of the .NET Framework you have installed on your computer. You can check this in the Programs and Features section of the Control Panel. If needed, you can download the latest version, for free, from [Microsoft](https://www.microsoft.com/en-us/download/details.aspx?id=53345 "https://www.microsoft.com/en-us/download/details.aspx?id=53345").

### Running the Code from within Visual Studio
The program is written in the C# programming language within the .NET Framework system of libraries. To run the program from within the code development environment, you will need to download the code and open it within [Visual Studio (VS)](https://visualstudio.microsoft.com/ "https://visualstudio.microsoft.com/").

To download the code without the option of pushing code revisions back to the GitHub code repository, see the 'Open with Visual Studio' and 'Download ZIP' options under the 'Code' menu (green button in upper-right corner of middle panel of this screen).

If you are interested in making revisions to the code and contributing those to the GitHub repository, you will need to clone the code. To clone code to VS from a GitHub repository, follow the instructions provided [here.](https://docs.microsoft.com/en-us/visualstudio/get-started/tutorial-open-project-from-repo?view=vs-2019 "https://docs.microsoft.com/en-us/visualstudio/get-started/tutorial-open-project-from-repo?view=vs-2019"). You will also need to contact Dr. Washburn (swash@ce.ufl.edu) to request to be added to the GitHub project as a contributor.

## Specifying the Input Values for the Queuing Analysis
[//]: # (Step through the inputs required in the input file:)

There are two methods to specify the input values used in the queuing analysis: 
1. Edit the InputData.xml file, or 
2. Write a method inside the CreateOnramp class and change the call from the default method to the new method. The second option is of course only available if you are working with the code within the Visual Studio environment.

### Option 1 - Modifying InputData.xml file

The input data file is in ***XML*** (eXtensible Markup Language) format and can be edited using any text editor. If you are not very familiar with XML, you can refer to the following sites, for starters, for more information:

* https://en.wikipedia.org/wiki/XML
* https://www.w3schools.com/XML/xml_whatis.asp
* https://www.w3schools.com/xml/
 
The input file must include values for the following data items. An example file is shown below the following description. It is also included in the repository.

[//]: # (Eventually add aerial photos of different ramp entry configurations)

* ***On-ramp Configuration:***
  * ***Metering Rates:***
  The on-ramp queue will discharge per the ramp metering rate, as follows:
    * Base metering rate: Approximate metering rate for freeway mainline flow conditions, when queue override adjustment is not active (for heavily congested conditions, rate should not be less than 240 veh/h).
    * Additional metering rate: 180 veh/h (The base rate will be incremented by this value when the on-ramp queue backs up to the intermediate queue detector position)
    * Maximum rate: 900 veh/h. This rate will be activated when the on-ramp queue backs up the advance queue detector position.
    
  * ***Segments:*** The on-ramp roadway can be divided into multiple segments. This might be necessary when left- and right-turning vehicles first turn onto separate roadway segments before merging into a shared on-ramp segment (the multiple segment feature is forthcoming)
    * ***Lanes:*** Number of lanes for the the on-ramp segment.
    * ***Queue Storage:*** Number of feet per lane available for vehicle queuing.
  * ***Queue Detectors:*** 
    * ID, numbered sequentially starting with 1
    * Type, "IntermediateQueue" or "AdvanceQueue"
    * Movement, movement(s) served by this detector: "All", "Left", or "Right" ("Left" also includes the through movement)
    * DistanceUpstreamFromMeterFt, the distance, in feet, from the ramp meter stop bar to the intermediate or advance detector

    [//]: # (IncludedInSegType, need to revisit if this is still necessary, given the movement input)
* ***Timing Stages:***
The `<TimingStages>` section of the input file contains the information about each timing stage present in the signal timing plan. The information that needs to be entered for each timing stage is as follows:
  * ID, numbered sequentially starting with 1
  * Green Time, in seconds
  * Lost Time, in seconds
* ***Movements:*** The `<Movements>` section identifies each movement that feeds the on-ramp within each timing stage. The information that needs to be entered for each movement is as follows:
  * Associated Ramp ID
  * Label, string value
  * NemaPhaseId (travel direction, EB, SB, WB, NB, with turn direction appended, Left, Thru, Right)
  * IsSignalControlled, true or false
  * Arrival flow rate, in veh/h
  
A typical timing stage configuration for a diamond interchange (assuming arterial roadway is oriented East-West) is shown in the following figure:

<img align="center" src="Images\TimingStages.png" />

The following schematic illustrates how these timing stage movements correspond to the ramp terminal intersection movements:

<img align="center" src="Images\OnRampSchematic.png" />

  * ***Traffic Data:*** The proportion of each of four vehicle type categories must be entered (small auto, large auto, small truck, and large truck). The total must equal 1.0.
  * ***Vehicle Data:*** The program includes inputs used to determine the amount of space, in feet, occupied on the on-ramp by queued vehicles. These values, at the moment, can only be changed through the source code and not through the input file.
    * ***Stop Gap:*** the distance between the rear bumper of a queued vehicle and the front bumper of the queued vehicle immediately behind it.
    * ***Average length of vehicle type:*** the average length of the vehicle lengths considered for a given vehicle category

|**Vehicle Category**|**Vehicle Lengths Considered (ft)**| Stop Gap Values (ft)
|-----------------|---------------------------------------:|------------:|
|Small Auto | 14.57, 16.7, 16.22, 14.78, 15.57, 15.53| 10|
|Large Auto | 16.4, 18.98, 16.94, 19.31| 10|
|Small Truck | 29| 14|
|Large Truck | 55, 68.5, 74.6| 20|


[//]: # (This is an equation syntax *E*~0~=*mc*^2^~2~)

The Average Vehicle Spacing (in ft) is calculated as follows:

---
**<span style="color: blue">S&#772;=  ~i~&sum; (G~i~+ L&#772;~i~ )P~i~</span>**
___
Where, 
* S&#772; = Average vehicle spacing (ft).
* G~i~ = Stop gap value of ***i^th^*** vehicle category (ft). 
* L&#772;~i~ = Average length of ***i^th^*** vehicle category (ft).
* P~i~ = Proportion of ***i^th^*** vehicle category in traffic stream.
* ***i*** = Vehicle category (small auto, large auto, small truck, large truck)
  
  
## InputData.xml File
```xml
<?xml version="1.0"?>
<ArrayOfInterchangeIntersectionData xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <InterchangeIntersectionData ID="1">
    <OnRamp ID="1">
      <Label>Southbound</Label>
      <Meter>
        <BaseRateVehPerHr>240</BaseRateVehPerHr>
        <AddedRateVehPerHr>180</AddedRateVehPerHr>
        <MaxRateVehPerHr>900</MaxRateVehPerHr>
      </Meter>
      <Segments>
        <OnRampSegmentData ID="1">
          <Type>Shared</Type>
          <NumLanes>2</NumLanes>
          <QueueStorageDistPerLaneFt>1000</QueueStorageDistPerLaneFt>
        </OnRampSegmentData>
      </Segments>
      <QueueDetectors>
        <RampQueueDetector ID="1">
          <Type>IntermediateQueue</Type>
          <Movement>All</Movement>
          <DistanceUpstreamFromMeterFt>500</DistanceUpstreamFromMeterFt>
          <IncludedInSegType>Shared</IncludedInSegType>
        </RampQueueDetector>
        <RampQueueDetector ID="2">
          <Type>AdvanceQueue</Type>
          <Movement>All</Movement>
          <DistanceUpstreamFromMeterFt>950</DistanceUpstreamFromMeterFt>
          <IncludedInSegType>Shared</IncludedInSegType>
        </RampQueueDetector>
      </QueueDetectors>
    </OnRamp>
    <IsSignalControlled>true</IsSignalControlled>
    <Signal>
      <Cycle>
        <TimingStages>
          <TimingStageData ID="1">
            <GreenTime>35</GreenTime>
            <LostTime>5</LostTime>
            <Movements>
              <IntersectionMovementData ID="1">
                <AssociatedRampId>1</AssociatedRampId>
                <Label>EB Right</Label>
                <NemaPhaseId>EBRight</NemaPhaseId>
                <IsSignalControlled>true</IsSignalControlled>
                <FlowRate>
                  <ArrivalsVehPerHr>250</ArrivalsVehPerHr>
                </FlowRate>
              </IntersectionMovementData>
            </Movements>
          </TimingStageData>
          <TimingStageData ID="2">
            <GreenTime>25</GreenTime>
            <LostTime>5</LostTime>
            <Movements>
              <IntersectionMovementData ID="2">
                <AssociatedRampId>1</AssociatedRampId>
                <Label>WB Left</Label>
                <NemaPhaseId>WBLeft</NemaPhaseId>
                <IsSignalControlled>true</IsSignalControlled>
                <FlowRate>
                  <ArrivalsVehPerHr>400</ArrivalsVehPerHr>
                </FlowRate>
              </IntersectionMovementData>
            </Movements>
          </TimingStageData>
          <TimingStageData ID="3">
            <GreenTime>15</GreenTime>
            <LostTime>5</LostTime>
            <Movements>
              <IntersectionMovementData ID="3">
                <AssociatedRampId>1</AssociatedRampId>
                <Label>SB Thru</Label>
                <NemaPhaseId>SBThru</NemaPhaseId>
                <IsSignalControlled>true</IsSignalControlled>
                <FlowRate>
                  <ArrivalsVehPerHr>0</ArrivalsVehPerHr>
                </FlowRate>
              </IntersectionMovementData>
            </Movements>
          </TimingStageData>
        </TimingStages>
      </Cycle>
    </Signal>
    <Traffic>
      <PropSmallAuto>0.528</PropSmallAuto>
      <PropLargeAuto>0.352</PropLargeAuto>
      <PropSmallTruck>0.09</PropSmallTruck>
      <PropLargeTruck>0.03</PropLargeTruck>
    </Traffic>
    <Vehicles />
  </InterchangeIntersectionData>
</ArrayOfInterchangeIntersectionData>
```

### Option 2 - Write a Method Inside the CreateOnramp Class


You can write your own method to create your desired intersection/on-ramp configuration. To do this, add a new method to the CreateOnramp class, in the CreateOnramp.cs file. You can also make a copy of the TightDiamond method (shown below), rename it and modify as desired. You will also need to add a call to this method inside of the btnStart_Click method in the MainForm.cs file.

<img align="center" src="Images\CreateOnrampClass.png" />

## Running The Program 

After starting the program, the following screen will be displayed.

<img align="center" src="Images\ProgramUI.png" />

If the analysis will be based on the InputData.xml file, press the Read Input File button, which will read the InputData.xml file (this file should be in the same folder as the program). Then press the Run Queuing Analysis button. When the program has finished running, a message indicating such will be written to the text box at the bottom of the form. The output file, Results.csv, will now be present in the program folder.

If the analysis will utilize a method written within the CreateOnramp class, do **not** press the Read Input File button. Just press the Run Queuing Analysis button. The rest of the process is as described above. Make sure the desired method in the CreateOnramp class is referenced from the btnStart_Click method.

### Queuing Simulation Process 

* The analysis simulates for 3600 s (1 hour), with 1-s time steps.
* Arrivals per time step for each intersection movement are determined by:
  * *Movement is signal controlled:* The arrivals per cycle at the intersection is based on a Poission distribution, using the average arrivals per cycle (hourly flow rate divide by the number of cycles per hour). Arrivals per time step are calculated by dividing the arrivals per cycle by the cycle length. This rate is updated at the start of every cycle.
  * *Movement is not signal controlled:* The arrivals per cycle at the intersection is based on a Poission distribution, using average arrivals per minute (hourly flow rate divide by 60 min/h). Arrivals per time step are calculated by dividing the arrivals per minute by 60 s/h. This rate is updated every 60 s.

* The departures per time step for an intersection movement are calculated by dividing arrivals per cycle at an intersection by the green time for the timing stage(s) during which that movement is active. Movement departures only occur during their respective active timing stage(s). *A future update will split this into departures of queued vehicles at the saturation flow rate and departues at the arrival rate post queue clearance.*

* The arrivals to the on-ramp are the sum of the intersection departures for the active movements during the given time step.
 
* Departures from the ramp meter occur at the intervals equal to the current “cycle length”. This cycle length is calculated as follows, assuming for example, a current metering rate of 240 veh/h: 3600 s/h/240 veh/h = 15 seconds. If the on-ramp includes more than one metered lane, then this release interval would be multiplied by the number of lanes; e.g., 2 vehicles released every 30 seconds for a two-lane on-ramp.

* The queue length (veh) is calculated every time step, as the difference between cumulative arrivals and cumulative departures. The queue length, in feet, is equal to the queue length in vehicles times the average vehicle spacing. Queue length, in lane-ft, is calculated as queue length in feet divided by the number of ramp segment lanes.

* For each time step, the current queue length is compared to the queue length for the previous time step. The following steps are taken, depending on whether the change in queue length is positive or negative. he change in queue length determines what checks are made by the analysis. Increasing queues will prompt checks for queue detector activation and decreasing queues will prompt checks for queue detector deactivation.

  * *Increasing queue length:* The queue length is checked against the specified location of the advance queue detector. If this distance is equaled or exceeded, the ramp metering rate will be set to the maximum rate. The number of times steps at which the maximum metering rate is activated will be incremented. If the queue length is less than the position of the advance queue detector, then the queue length is checked against the specified position of the intermediate queue detector. If this distance is equaled or exceeded, the ramp metering rate will be set to the base metering rate plus the incremental metering rate (e.g., 240 + 180 = 420 veh/h). Otherwise, the metering rate will be set the base rate. *This process is more complicated if there are multiple segments that comprise the on-ramp, which occurs when there is a split-entry configuration for left- and right-turning movements. This capability is forthcoming.*
  * *Decreasing queue length:* The reverse process to the above is performed.

* Various measures are recorded every time step and then written to the output file at the end of the simulation, discussed in the following section.

## Output File

When the program finishes running, it will write a file named Results.csv to the program folder. This file includes the following output fields:

* **Time Step (ts)** -> One second each, from 0-3599 seconds, at an increment of 1 second.
* **Timing Stage ID** -> Indicates the timing stage for the given time step.
* **On-Ramp Arrivals** -> Input Demand (in veh/ts) at the upstream of the on-ramp. The input demand values are presented both by movement and as a summation of each movement's input demand.
* **Metering Rate** -> Metering rate, in veh/h, set for the current time step (base, intermediate, or maximum).
* **Vehicles Served** -> Number of vehicles served by the ramp meter for a given time step. Vehicle departures occur at the meter 'cycle length' intervals. Other time steps will have zero vehicle departures.
* **Cumulative Arrivals** -> The total arrivals from the upstream intersection to the on-ramp through the current time step.
* **Cumulative Departures** -> The total vehicles served by the on-ramp up through the current time step.
* **Number of Queued Vehicles** -> Cumulative arrivals minus cumulative departures for the given time step.
* **Queue Length (ft)** -> Number of queued vehicles times the average vehicle spacing (ft).
* **Queue Length (ft/ln)** -> Queue length (ft) divided by number of on-ramp segment lanes.
* **Percentage Occupancy of Shared Queue Storage** -> Queue length (in ft/ln) divided by the Queue storage (in ft/ln)  x 100 for the given time step.
* **Percentage of time that the meter operates in advance override mode** -> Number of time steps the advance queue detector is active divided by 3600 s x 100.

You can use [this macro code](https://github.com/swash17/RampMeterQueueing/blob/master/QueuingAnalysisResultsMacro.txt) to generate the following two types of graphs from the data present in the Results.csv file.

* Cumulative Arrivals, Cumulative Departures, and Queue Length versus Simulation Time:
<img align="center" src="Images\ResultsGraph1.jpg" />

* Percent Occupancy of Shared Queue Storage versus Simulation Time:
<img align="center" src="Images\ResultsGraph2.jpg" />
