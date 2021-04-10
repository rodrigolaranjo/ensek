using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace MeterReadingUpload.Controllers
{
    [Route("meter-reading-uploads")]
    [ApiController]
    public class MeterReadingUploadsController : ControllerBase
    {
        [HttpPost]
        public MeterReadingResult Get()
        {
            StreamReader body = new StreamReader(Request.Body);
            Task<string> requestBody = body.ReadToEndAsync();
            string csv = requestBody.Result;

            return BusinessProcess.MeterReadings.Add(csv);
        }
    }
}