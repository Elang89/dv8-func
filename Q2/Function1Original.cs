using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Azure.Devices;

namespace SomeFunctionApp
{
    public static class Function1Original
    {
        private static string DBstr = Environment.GetEnvironmentVariable("SQLServerConnectionString", EnvironmentVariableTarget.Process);
        private static string connectionString = Environment.GetEnvironmentVariable("IoTHubConnectionString", EnvironmentVariableTarget.Process);
        private static RegistryManager registryManager = RegistryManager.CreateFromConnectionString(connectionString);


        [FunctionName("Function1Original")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            /*The list should be changed to a list of objects and the dictionaries removed and made into a class */

            List<int> wellList = new List<int>();
            Dictionary<int, string> UWIDic = new Dictionary<int, string>();
            Dictionary<int, string> PadDic = new Dictionary<int, string>();
            Dictionary<int, string> AreaDic = new Dictionary<int, string>();

            using (SqlConnection conn = new SqlConnection(DBstr))
            {
                conn.Open();
                var text = "SELECT * FROM [dbo].[Wells]";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    using (SqlDataReader oReader = cmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            wellList.Add(Convert.ToInt32(oReader["Id"]));
                            UWIDic.Add(Convert.ToInt32(oReader["Id"]), oReader["Wellid"].ToString());
                            PadDic.Add(Convert.ToInt32(oReader["Id"]), oReader["FieldName"].ToString());
                            AreaDic.Add(Convert.ToInt32(oReader["Id"]), oReader["Area"].ToString());
                        }
                    }

                }
                /* This conn.Close() should be removed entirely, the connection is still used below 
                in the Update statement. Closing connections and opening them like this, requires resource allocation
                and API calls that are costly. It is better to only call it once. */
                conn.Close();
                string Write2DBtext = "";

                string nowTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");

                foreach (int wellid in wellList)
                {
                    log.LogInformation("Reading device twin: " + wellid);
                    DigitalTwinReading singleTwin = new DigitalTwinReading();
                    singleTwin = await ReadDigitalTwin(wellid);

                    /* This line is too long, furthermore, 
                    if one requires the use a query in this manner it is better
                    to do it with bind parameters, it is not the case in here, but if this were 
                    an HTTP trigger the query itself is vulnerable to SQL injection. Additionally, since this is a
                    loop, this string is being built multiple times. String concatenation will use a lot of memory with this
                    process  */
                    Write2DBtext = "UPDATE DeviceTwinCache SET UpdatedAt='" + nowTime
                    + "', connectionState='" + singleTwin.ConnectionToCloud
                    + "', VFDRunning='" + singleTwin.VFDRunning
                    + "', ADCInclinometer='" + singleTwin.ADCInclinometer
                    + "', ADCLoadcell='" + singleTwin.LoadCell
                    + "', VFDControl='" + singleTwin.VFDControl
                    + "', POCMode ='" + singleTwin.POCMode
                    + "', sensorWHP ='" + singleTwin.sensorWHP
                    + "', sensorCHP ='" + singleTwin.sensorCHP
                    + "', sensorPLP ='" + singleTwin.sensorPLP
                    + "', VFDReady ='" + singleTwin.VFDReady
                    + "', VFDFault ='" + singleTwin.VFDFault
                    + "', VFDON ='" + singleTwin.VFDON
                    + "', isFullVersion = " + singleTwin.isFullVersion
                    + ", MaxSpm = " + singleTwin.maxSPM
                    + ", sensorGasMeter ='" + singleTwin.sensorGasMeter
                    + "', sensorFlowMeter = '" + singleTwin.sensorFlowMeter
                    + "'  WHERE Wellid=" + wellid;

                    /* This should be removed, moreover, connections to databases should never be opened in loops.
                    This is opening and closing the connection multiple times, and that requires a lot of resources.  
                    */
                    conn.Open();

                    // This comman should be instantiated once and not created multiple times
                    using (SqlCommand command = new SqlCommand(Write2DBtext, conn))
                    {
                        command.ExecuteNonQuery();
                    }

                    // This should also be removed for the same reason as above. 
                    conn.Close();

                    log.LogInformation($"{wellid} - record has been updated to DB.");
                }

                conn.Close();
                log.LogInformation($"Finished.");
            }

        }

        private static async Task<DigitalTwinReading> ReadDigitalTwin(int DeviceId)
        {
            DigitalTwinReading dtr = new DigitalTwinReading();

            Microsoft.Azure.Devices.Shared.Twin twin;
            try
            {
                twin = await registryManager.GetTwinAsync(DeviceId.ToString());

            }
            catch
            {
                Console.WriteLine("Cannot establish connection to digital twin");
                dtr.ConnectionToCloud = "Diconnected";
                dtr.VFDON = "?";
                dtr.VFDReady = "?";
                dtr.VFDFault = "?";
                dtr.VFDRunning = "?";
                dtr.ADCInclinometer = "?";
                dtr.LoadCell = "?";
                dtr.POCMode = "?";
                dtr.VFDControl = "?";
                return dtr;
            }


            if (twin != null)
            {
                try
                {
                    dtr.ConnectionToCloud = twin.ConnectionState.ToString();
                    dtr.VFDON = twin.Properties.Reported["VFD"]["ON"];
                    dtr.VFDReady = twin.Properties.Reported["VFD"]["Ready"];
                    dtr.VFDFault = twin.Properties.Reported["VFD"]["Fault"];
                    dtr.VFDRunning = twin.Properties.Reported["VFD"]["Running"];
                    dtr.ADCInclinometer = twin.Properties.Reported["ADC"]["inclinometer"];
                    dtr.LoadCell = twin.Properties.Reported["ADC"]["loadcell"];
                }
                catch
                {
                    Console.WriteLine("Digital Twin Catch statement here");
                    dtr.ConnectionToCloud = "Diconnected";
                    dtr.VFDON = "?";
                    dtr.VFDReady = "?";
                    dtr.VFDFault = "?";
                    dtr.VFDRunning = "?";
                    dtr.ADCInclinometer = "?";
                    dtr.LoadCell = "?";
                }

                try
                {
                    dtr.POCMode = twin.Properties.Reported["POC"]["Mode"];
                }
                catch
                {
                    dtr.POCMode = "?";
                }
                try
                {
                    dtr.VFDControl = twin.Properties.Reported["VFD"]["Control"];
                }
                catch
                {
                    dtr.VFDControl = "?";
                }

                try
                {
                    dtr.sensorWHP = twin.Properties.Reported["Sensor"]["WHP"];
                    dtr.sensorCHP = twin.Properties.Reported["Sensor"]["CHP"];
                    dtr.sensorPLP = twin.Properties.Reported["Sensor"]["PLP"];
                    dtr.sensorGasMeter = twin.Properties.Reported["Sensor"]["GasMeter"];
                    dtr.sensorFlowMeter = twin.Properties.Reported["Sensor"]["FlowMeter"];
                }
                catch
                {
                    dtr.sensorWHP = "?";
                    dtr.sensorCHP = "?";
                    dtr.sensorPLP = "?";
                    dtr.sensorGasMeter = "?";
                    dtr.sensorFlowMeter = "?";
                }


                try
                {
                    dtr.maxSPM = twin.Properties.Reported["POC"]["PF"]["maxSPM"];
                }
                catch
                {
                    dtr.maxSPM = 0;
                }


                try
                {
                    string versionFeatures = twin.Properties.Reported["Version"]["Features"];
                    if (versionFeatures == "Lite")
                    {
                        dtr.isFullVersion = 0;
                    }
                    else
                    {
                        dtr.isFullVersion = 1;
                    }

                }
                catch
                {
                    dtr.isFullVersion = 1;
                }

            }
            else
            {
                Console.WriteLine("Digital twin is NULL");
                dtr.ConnectionToCloud = "Diconnected";
                dtr.VFDON = "?";
                dtr.VFDReady = "?";
                dtr.VFDFault = "?";
                dtr.VFDRunning = "?";
                dtr.ADCInclinometer = "?";
                dtr.LoadCell = "?";
                dtr.POCMode = "?";
                dtr.VFDControl = "?";
                dtr.sensorWHP = "?";
                dtr.sensorCHP = "?";
                dtr.sensorPLP = "?";
                dtr.sensorGasMeter = "?";
                dtr.sensorFlowMeter = "?";
                dtr.maxSPM = 0;
                dtr.isFullVersion = 1;
            }
            return dtr;
        }


        class DigitalTwinReading
        {
            public string ConnectionToCloud { get; set; }
            public string ADCInclinometer { get; set; }
            public string LoadCell { get; set; }

            public string VFDON { get; set; }
            public string VFDReady { get; set; }
            public string VFDFault { get; set; }
            public string VFDRunning { get; set; }
            public string VFDControl { get; set; }
            public string POCMode { get; set; }

            public string sensorWHP { get; set; }
            public string sensorCHP { get; set; }

            public string sensorPLP { get; set; }
            public string sensorGasMeter { get; set; }
            public string sensorFlowMeter { get; set; }

            public int isFullVersion { get; set; }

            public double maxSPM { get; set; }
        }
    }
}
