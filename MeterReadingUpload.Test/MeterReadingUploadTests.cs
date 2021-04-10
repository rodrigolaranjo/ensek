using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MeterReadingUpload.Test
{
    public class MeterReadingUploadTests : IClassFixture<WebApplicationFactory<MeterReadingUpload.Startup>>
    {
        /// <remarks>Hardcoded just because it's a test. It shouldn't be hardcoded in a real application.</remarks>
        private const string connStr = "Server=database-2.cfjx1uroaxbm.eu-west-2.rds.amazonaws.com;Port=5432;Database=postgres;User Id=postgres;Password=Ensek2021";

        public HttpClient Client { get; }

        public MeterReadingUploadTests(WebApplicationFactory<MeterReadingUpload.Startup> fixture)
        {
            Client = fixture.CreateClient();
        }

        [Fact]
        public async Task UploadMeterReadings()
        {
            CleanUpDatabase();
            string csv = File.ReadAllText("Meter_Reading.csv");
            HttpContent content = new StringContent(csv);
            HttpResponseMessage response = await Client.PostAsync("/meter-reading-uploads", content);
            MeterReadingResult uploadResult = JsonConvert.DeserializeObject<MeterReadingResult>(await response.Content.ReadAsStringAsync());

            Assert.Equal(24, uploadResult.successful);
            Assert.Equal(11, uploadResult.failed);
        }

        private void CleanUpDatabase()
        {
            ExecuteProcedure("ensek.deleteMeterReadings", new List<NpgsqlParameter>());
        }

        private static void ExecuteProcedure(string procName, List<NpgsqlParameter> parameters)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(connStr);
            try
            {
                string paramNames = String.Join(", ", parameters.Select(p => $"@{p.ParameterName.ToLower()}").ToList());

                string cmdText = $"CALL {procName.ToLower()}({paramNames})";

                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand(cmdText, conn);

                foreach (NpgsqlParameter param in parameters)
                {
                    param.ParameterName = param.ParameterName.ToLower();
                    cmd.Parameters.Add(param);
                }

                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

    }
}
