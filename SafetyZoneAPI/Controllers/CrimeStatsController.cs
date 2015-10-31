using SafetyZoneAPI.Models;
using SafetyZoneAPI.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SafetyZoneAPI.Controllers
{
    public class CrimeStatsController : ApiController
    {
        public CrimeDataViewModel Get(double latitude, double longitude)
        {
            var service = new CrimeDataService();
            var lgaAndScore = service.DetermineLga(latitude, longitude);
            var lga = lgaAndScore.First().Key;
            var index = service.DetermineCrimeRatingIndex(lgaAndScore[lga]);
            return new CrimeDataViewModel() {
                Lat = latitude,
                Long = longitude,
                LGAName = lga,
                CrimeRatingIndex = index,
                Rate = lgaAndScore[lga]
                };
        }
        public async Task<HttpResponseMessage> Post()
        {
            var crimeService = new CrimeDataService();
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);
                string line = null;
                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);
                    TextReader reader = File.OpenText(file.LocalFileName);
                    Dictionary<string, double> records = new Dictionary<string, double>();
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] tokens = line.Split(',');
                        var lga = tokens[0];
                        var stat = double.Parse(tokens[1]);
                        records[lga] = stat;
                    }
                    line = crimeService.InitialiseCrimeStatistics(records).ToString();
                }
                return Request.CreateResponse(HttpStatusCode.OK, line!=null ? line : "");
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

    }
}
