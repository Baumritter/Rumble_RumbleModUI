using MelonLoader;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ModUI
{
    public class PackageMetrics
    {
        public int downloads { get; set; }
        public int rating_score { get; set; }
        public string latest_version { get; set; }
    }
    public static class TS_API
    {
        public enum Status
        {
            BothSame,
            LocalNewer,
            GlobalNewer
        }

        private static bool debug = false;
        private static string URL = "https://thunderstore.io";
        private static string Command = "api/v1/package-metrics";
        private static HttpClient client = new HttpClient();

        private static PackageMetrics PackageMetrics { get; set; }
        private static Status VersionStatus = Status.BothSame;
        public static string LocalVersion;
        public static string Team;
        public static string Package;
        public static event Action<Status> OnVersionGet;

        public static void CheckVersion()
        {
            RequestData().GetAwaiter().GetResult();
            OnMetricsGet();
            OnVersionGet?.Invoke(VersionStatus);
        }

        private static void OnMetricsGet()
        {
            Version Local = new Version(LocalVersion);
            Version Global = new Version(PackageMetrics.latest_version);

            int Compare = Local.CompareTo(Global);

            if (Compare < 0) VersionStatus = Status.GlobalNewer;
            else if (Compare > 0) VersionStatus = Status.LocalNewer;
            else VersionStatus = Status.BothSame;

            if (debug) { MelonLogger.Msg(VersionStatus); }

        }

        private static PackageMetrics DeserializeResponse(string response)
        {
            PackageMetrics metrics = new PackageMetrics();
            string[] Split;
            string placeholder = response;

            placeholder = placeholder.Replace("{", "");
            placeholder = placeholder.Replace("}", "");

            Split = placeholder.Split(',');

            foreach(string SubString in Split)
            {
                string[] Split2 = SubString.Split(':');
                if (Split2[0].Contains("downloads")) metrics.downloads = int.Parse(Split2[1]);
                if (Split2[0].Contains("rating_score")) metrics.rating_score = int.Parse(Split2[1]);
                if (Split2[0].Contains("latest_version")) metrics.latest_version = Split2[1].Replace("\"","") ;
            }

            return metrics;
        }
        private static async Task<string> GetPackageMetrics(string path)
        {
            string packageMetrics = null;
            if (debug) MelonLogger.Msg(path);
            HttpResponseMessage response = await client.GetAsync(path)  ;
            if (response.IsSuccessStatusCode)
            {
                packageMetrics = await response.Content.ReadAsStringAsync() ;
                if (debug) MelonLogger.Msg("Package get");
            }
            return packageMetrics ;
        }

        private static async Task RequestData()
        {
            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                string temp = await GetPackageMetrics(Command + "/" + Team + "/" + Package);

                if (debug) MelonLogger.Msg("API - Response: " + temp);

                if (temp != null)
                {
                    PackageMetrics = DeserializeResponse(temp);
                }

            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex.Message);
            }

        }
    }
}
