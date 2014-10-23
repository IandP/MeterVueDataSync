using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Text;

namespace MeterVueDataSync
{
    class LatestDataOutput
    {
        public DateTime MaxDate { get; set; }
        public string ContinuationToken { get; set; }
        public List<LatestDataRow> data { get; set; }
    }

    class LatestDataRow
    {
        public Guid m { get; set; }
        public string d { get; set; }
        public DateTime t { get; set; }
        public Double v { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                DateTime d;
                if (DateTime.TryParse(args[0], out d))
                {
                    Properties.Settings.Default.LatestData = d;
                    Properties.Settings.Default.Save();
                }
            }

            syncData();
        }

        private static void syncData()
        {
            string APIName = ConfigurationManager.AppSettings["MeterVueURL_APIName"];
            string APIKey = ConfigurationManager.AppSettings["MeterVueURL_APIKey"];
            string DataURL = ConfigurationManager.AppSettings["MeterVueURL_DataURL"];

            while (true)
            {
                DateTime LatestData = Properties.Settings.Default.LatestData;

                DataURL = DataURL + String.Format("?UpdatedDate={0:o}", LatestData);


                string json_meters = string.Empty;
                string json_data = String.Empty;

                using (var w = new WebClient())
                {
                    // Set auth credentials both ways
                    w.Credentials = new NetworkCredential(APIName, APIKey);

                    string _auth = string.Format("{0}:{1}", APIName, APIKey);
                    string _enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(_auth));
                    string _cred = string.Format("{0} {1}", "Basic", _enc);
                    w.Headers[HttpRequestHeader.Authorization] = _cred;


                    while (!String.IsNullOrEmpty(DataURL))
                    {
                        json_data = w.DownloadString(DataURL);

                        LatestDataOutput output = JsonConvert.DeserializeObject<LatestDataOutput>(json_data);

                        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SyncDatabase"].ConnectionString))
                        {
                            connection.Open();

                            using (SqlCommand command = connection.CreateCommand())
                            {
                                command.CommandText = "MERGE MeterData AS target"
                                                    + " USING (SELECT @MeterGUID,@DataType,@TimeStamp,@DataValue) AS source (MeterGUID,DataType,TimeStamp,DataValue)"
                                                    + " ON (target.MeterGUID = source.MeterGUID AND "
                                                    + " target.DataType = source.DataType AND "
                                                    + "	target.TimeStamp = source.TimeStamp AND"
                                                    + " target.DataValue = source.DataValue)"
                                                    + " WHEN MATCHED THEN "
                                                    + " UPDATE SET DataValue = source.DataValue"
                                                    + " WHEN NOT MATCHED THEN"
                                                    + " INSERT (MeterGUID,DataType,TimeStamp,DataValue)"
                                                    + " VALUES (source.MeterGUID,source.DataType,source.TimeStamp,source.DataValue);";

                                foreach (LatestDataRow ldr in output.data)
                                {
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@MeterGUID", ldr.m);
                                    command.Parameters.AddWithValue("@DataType", ldr.d);
                                    command.Parameters.AddWithValue("@TimeStamp", ldr.t);
                                    command.Parameters.AddWithValue("@DataValue", ldr.v);
                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        Properties.Settings.Default.LatestData = output.MaxDate;
                        Properties.Settings.Default.Save();

                        DataURL = output.ContinuationToken;
                    }
                }

                System.Threading.Thread.Sleep(60 * 60 * 1000);

            }


        }

    }
}
