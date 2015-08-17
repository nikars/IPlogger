using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace IP_logger
{
    public partial class Service1 : ServiceBase
    {
        private static System.Timers.Timer aTimer = new System.Timers.Timer();

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            InitTimer();
        }

        protected override void OnStop()
        {
            // Clear log file
        }

        private static void InitTimer()
        {
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 5000;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            WriteToFile("c:\\home_public_ip.txt", GetPublicIp());
        }

        public static string GetPublicIp()
        {
            string ipInfo;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://ipinfo.io/");
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    ipInfo = reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                    WriteToFile("c:\\home_public_ip.txt", errorText);
                }
                throw;
            }

            IPData ipData = JsonConvert.DeserializeObject<IPData>(ipInfo);

            return ipData.ip;
        }

        private static void WriteToFile(string fileName, string ipAddr)
        {
            using (StreamWriter w = File.CreateText(fileName))
            {
                w.Write("\r\nIP actualizado: ");
                w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                w.WriteLine("  :{0}", ipAddr);
            }
        }
    }

    public class IPData {
        public string ip { get; set; }
        public string hostname { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public string loc { get; set; }
        public string org { get; set; }
        public string postal { get; set; }
    }
}
