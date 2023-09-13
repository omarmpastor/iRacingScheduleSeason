using iRacingBusinessLayer.Models;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace iRacingBusinessLayer
{
    public class iRacingLoadPage
    {
        private static string RemoveDiacritics(string str)
        {
            if (null == str) return string.Empty;
            var chars = str
                .Normalize(NormalizationForm.FormD)
                .ToCharArray()
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        public static bool CheckConnection(string proxyAddress = "", string proxyUserName = "", string proxyPassword = "")
        {
            try
            {
                WebClient webClient = new WebClient();
                if (proxyAddress != "")
                {
                    webClient.Proxy = GetWebProxy(proxyAddress, proxyUserName, proxyPassword);
                }

                using (var stream = webClient.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static WebProxy GetWebProxy(string proxyAddress = "", string proxyUserName = "", string proxyPassword = "")
        {
            if (proxyAddress == "")
            {
                return new WebProxy();
            }

            WebProxy proxy = new WebProxy
            {
                Address = new Uri(proxyAddress),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = true
            };

            if (proxyUserName != "" && proxyPassword != "")
            {
                proxy.UseDefaultCredentials = false;
                proxy.Credentials = new NetworkCredential(
                    userName: proxyUserName,
                    password: proxyPassword);
            }

            return proxy;
        }


        private static HttpClient GetHttpClient(string proxyAddress = "", string proxyUserName = "", string proxyPassword = "")
        {
            if (proxyAddress == "")
            {
                return new HttpClient();
            }

            WebProxy proxy = new WebProxy
            {
                Address = new Uri(proxyAddress),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = true
            };

            if (proxyUserName != "" && proxyPassword != "")
            {
                proxy.UseDefaultCredentials = false;
                proxy.Credentials = new NetworkCredential(
                    userName: proxyUserName,
                    password: proxyPassword);
            }

            var httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
            };

            // Disable SSL verification
            httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            return new HttpClient(handler: httpClientHandler, disposeHandler: true);
        }

        private static string GetPage(string url, string proxyAddress = "", string proxyUserName = "", string proxyPassword = "")
        {

            string page = string.Empty;
            HttpClient httpClient;

            if (proxyAddress == "")
            {
                httpClient = new HttpClient();
            }
            else
            {
                var proxy = GetWebProxy(proxyAddress, proxyUserName, proxyPassword);
                var httpClientHandler = new HttpClientHandler
                {
                    Proxy = proxy,
                };
                httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                httpClient = new HttpClient(handler: httpClientHandler, disposeHandler: true);
            }

            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36 Edg/105.0.1343.53");
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml");
            httpClient.DefaultRequestHeaders.Add("Accept-Charset", "ISO-8859-1");


            using (HttpResponseMessage response = httpClient.GetAsync(url).Result)
            {
                using (HttpContent content = response.Content)
                {
                    string result = content.ReadAsStringAsync().Result;

                    page = String.Join("", result);
                }
            }

            httpClient.Dispose();

            return HttpUtility.HtmlDecode(page);
        }

        public static List<IRacingContent> GetCars(string proxyAddress = "", string proxyUserName = "", string proxyPassword = "")
        {
            List<IRacingContent> cars = new List<IRacingContent>();
            string pageCars = GetPage("https://www.iracing.com/cars/", proxyAddress, proxyUserName, proxyPassword);

            string patternCars = "<div data-name=\"(?<item>.*)\" data-order=\"\\d+\" data-type=\"(?<type>\\w+)\"";
            MatchCollection matchesCars = Regex.Matches(pageCars, patternCars);

            foreach (Match item in matchesCars)
            {
                string carName = RemoveDiacritics(item.Groups["item"].Value)
                    .Replace("  ", " ").Replace("–", "-").Replace("’", "'");
                bool isFree = (item.Groups["type"].Value.Trim() == "free") ? true : false;
                cars.Add(new IRacingContent(carName)
                {
                    IsFree = isFree,
                    Type = IRacingContentType.Car
                });
            }

            return cars;
        }

        public static List<IRacingContent> GetTracks(string proxyAddress = "", string proxyUserName = "", string proxyPassword = "")
        {
            List<IRacingContent> tracks = new List<IRacingContent>();
            string pageTracks = GetPage("https://www.iracing.com/tracks/", proxyAddress, proxyUserName, proxyPassword);

            string patternTracks = "<div data-name=\"(?<item>.*)\" data-type=\"(?<type>\\w+)\"";
            MatchCollection matchesTracks = Regex.Matches(pageTracks, patternTracks);

            foreach (Match item in matchesTracks)
            {
                string trackName = RemoveDiacritics(item.Groups["item"].Value).Replace("  ", " ");
                bool isFree = (item.Groups["type"].Value.Trim() == "free")?true:false;
                tracks.Add(new IRacingContent(trackName) {
                    IsFree = isFree,
                    Type = IRacingContentType.Track
                });
            }

            // Fix
            tracks.Add(new IRacingContent("Nurburgring Combined") { Type = IRacingContentType.Track });

            return tracks;
        }
    }
}
