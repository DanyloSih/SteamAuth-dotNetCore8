﻿using System;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Text;

namespace DanPie.SteamAuthHelper
{
    /// <summary>
    /// Class to help align system time with the Steam server time. Not super advanced; probably not taking some things into account that it should.
    /// Necessary to generate up-to-date codes. In general, this will have an error of less than a second, assuming Steam is operational.
    /// </summary>
    public class TimeAligner
    {
        private static bool _aligned = false;
        private static int _timeDifference = 0;

        public static long GetSteamTime()
        {
            if (!TimeAligner._aligned)
            {
                TimeAligner.AlignTime();
            }
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() + _timeDifference;
        }

        public static async Task<long> GetSteamTimeAsync()
        {
            if (!TimeAligner._aligned)
            {
                await TimeAligner.AlignTimeAsync();
            }
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() + _timeDifference;
        }

        public static void AlignTime()
        {
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                try
                {
                    string response = client.UploadString(APIEndpoints.TWO_FACTOR_TIME_QUERY, "steamid=0");
                    TimeQuery query = JsonConvert.DeserializeObject<TimeQuery>(response);
                    TimeAligner._timeDifference = (int)(query.Response.ServerTime - currentTime);
                    TimeAligner._aligned = true;
                }
                catch (WebException)
                {
                    return;
                }
            }
#pragma warning restore SYSLIB0014 // Type or member is obsolete
        }

        public static async Task AlignTimeAsync()
        {
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            WebClient client = new WebClient();
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            try
            {
                client.Encoding = Encoding.UTF8;
                string response = await client.UploadStringTaskAsync(new Uri(APIEndpoints.TWO_FACTOR_TIME_QUERY), "steamid=0");
                TimeQuery query = JsonConvert.DeserializeObject<TimeQuery>(response);
                TimeAligner._timeDifference = (int)(query.Response.ServerTime - currentTime);
                TimeAligner._aligned = true;
            }
            catch (WebException)
            {
                return;
            }
        }

        internal class TimeQuery
        {
            [JsonProperty("response")]
            internal TimeQueryResponse Response { get; set; }

            internal class TimeQueryResponse
            {
                [JsonProperty("server_time")]
                public long ServerTime { get; set; }
            }

        }
    }
}
