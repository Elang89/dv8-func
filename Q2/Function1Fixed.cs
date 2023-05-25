using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Azure.Devices;
using System.Data;

namespace SomeFunctionApp
{
    public static class Function1Fixed
    {
        private static string DBstr = Environment.GetEnvironmentVariable("SQLServerConnectionString", EnvironmentVariableTarget.Process);
        private static string connectionString = Environment.GetEnvironmentVariable("IoTHubConnectionString", EnvironmentVariableTarget.Process);
        private static RegistryManager registryManager = RegistryManager.CreateFromConnectionString(connectionString);


        [FunctionName("Function1Fixed")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            List<Well> wellList = new List<Well>();

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
                            Well currentWell = new Well
                            {
                                WellId = Convert.ToInt32(oReader["Wellid"]),
                                FieldName = oReader["FieldName"].ToString(),
                                Area = oReader["Area"].ToString()
                            };
                            wellList.Add(currentWell);
                        }
                    }

                }

                string Write2DBtext = @"UPDATE DeviceTwinCache SET 
                UpdatedAt = @UpdatedAt,
                connectionState = @connectionState,
                VFDRunning = @VFDRunning,
                ADCInclinometer = @ADCInclinometer,
                ADCLoadcell= @ADCLoadcell,
                POCMode = @POCMode,
                sensorWHP = @sensorWHP,
                sensorCHP = @sensorCHP,
                sensorPLP = @sensorPLP,
                VFDReady = @VFDReady,
                VFDFault = @VFDFault,
                VFDON = @VFDON,
                isFullVersion = @isFullVersion,
                MaxSpm = @MaxSpm,
                sensorGasMeter = @SensorGasMeter,
                sensorFlowerMeter = @sensorFlowerMeter,
                WHERE WellId = @WellId";

                string nowTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
                SqlCommand command = new SqlCommand(Write2DBtext, conn);

                SqlParameter updatedAtParam = new SqlParameter("@UpdatedAt", SqlDbType.DateTimeOffset);
                SqlParameter connectionStateParam = new SqlParameter("@connectionState", SqlDbType.VarChar);
                SqlParameter VFDRunningParam = new SqlParameter("@VFDRunningParam", SqlDbType.VarChar);
                SqlParameter ADCInclinometerParam = new SqlParameter("@ADCInclinometer", SqlDbType.VarChar);
                SqlParameter ADCLoadcellParam = new SqlParameter("@ADCLoadcell", SqlDbType.VarChar);
                SqlParameter VFDControlParam = new SqlParameter("@VFDControl", SqlDbType.VarChar);
                SqlParameter POCModeParam = new SqlParameter("@POCMode", SqlDbType.VarChar);
                SqlParameter sensorWHPParam = new SqlParameter("@SensorWHP", SqlDbType.VarChar);
                SqlParameter sensorCHPParam = new SqlParameter("@SensorCHP", SqlDbType.VarChar);
                SqlParameter sensorPLPParam = new SqlParameter("@SensorPLP", SqlDbType.VarChar);
                SqlParameter VFDReadyParam = new SqlParameter("@VFDReady", SqlDbType.VarChar);
                SqlParameter VFDFaultParam = new SqlParameter("@VFDFault", SqlDbType.VarChar);
                SqlParameter VFDONParam = new SqlParameter("@VFDON", SqlDbType.VarChar);
                SqlParameter isFullVersionParam = new SqlParameter("@isFullVersionParam", SqlDbType.VarChar);
                SqlParameter MaxSpmParam = new SqlParameter("@MaxSpm", SqlDbType.VarChar);
                SqlParameter sensorGasMeterParam = new SqlParameter("@sensorGasMeter", SqlDbType.Int);
                SqlParameter sensorFlowMeterParam = new SqlParameter("@sensorFlowMeter", SqlDbType.Int);
                SqlParameter WellidParam = new SqlParameter("@Wellid", SqlDbType.Int);

                List<SqlParameter> prms = new List<SqlParameter>(new SqlParameter[]
                {
                    updatedAtParam, connectionStateParam, VFDRunningParam, ADCInclinometerParam, ADCLoadcellParam,
                    VFDControlParam, POCModeParam, sensorWHPParam, sensorCHPParam, sensorPLPParam, VFDReadyParam,
                    VFDFaultParam, VFDONParam, isFullVersionParam, MaxSpmParam, sensorGasMeterParam, sensorFlowMeterParam,
                    WellidParam
                });

                command.Parameters.AddRange(prms.ToArray());

                foreach (Well well in wellList)
                {
                    log.LogInformation("Reading device twin: " + well.WellId);
                    DigitalTwinReading singleTwin = new DigitalTwinReading();
                    singleTwin = await ReadDigitalTwin(well.WellId);

                    command.Parameters["@UpdatedAt"].Value = nowTime;
                    command.Parameters["@connectionState"].Value = singleTwin.ConnectionToCloud;
                    command.Parameters["@VFDRunning"].Value = singleTwin.VFDRunning;
                    command.Parameters["@ADCInclinator"].Value = singleTwin.ADCInclinometer;
                    command.Parameters["@ADCLoadcell"].Value = singleTwin.LoadCell;
                    command.Parameters["@VFDControl"].Value = singleTwin.VFDControl;
                    command.Parameters["@POCMode"].Value = singleTwin.POCMode;
                    command.Parameters["@sensorWHP"].Value = singleTwin.sensorWHP;
                    command.Parameters["@sensorCHP"].Value = singleTwin.sensorCHP;
                    command.Parameters["@sensornPLP"].Value = singleTwin.sensorPLP;
                    command.Parameters["@VFDReady"].Value = singleTwin.VFDReady;
                    command.Parameters["@VFDFault"].Value = singleTwin.VFDFault;
                    command.Parameters["@VFDON"].Value = singleTwin.VFDON;
                    command.Parameters["@nisFullVersion"].Value = singleTwin.isFullVersion;
                    command.Parameters["@MaxSpm"].Value = singleTwin.maxSPM;
                    command.Parameters["@sensorGasMeter"].Value = singleTwin.sensorGasMeter;
                    command.Parameters["@sensorFlowMeter"].Value = singleTwin.sensorFlowMeter;
                    command.Parameters["@Wellid"].Value = well.WellId;

                    command.ExecuteNonQuery();

                    log.LogInformation($"{well.WellId} - record has been updated to DB.");
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

        class Well
        {
            public int WellId { get; set; }
            public string FieldName { get; set; }
            public string Area { get; set; }

        }
    }
}
