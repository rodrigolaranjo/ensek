using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeterReadingUpload.BusinessProcess
{
    internal class Storage
    {
        /// <remarks>Hardcoded just because it's a test. It shouldn't be hardcoded in a real application.</remarks>
        private const string connStr = "Server=database-2.cfjx1uroaxbm.eu-west-2.rds.amazonaws.com;Port=5432;Database=postgres;User Id=postgres;Password=Ensek2021";

        internal static void RecordMeterReading(Models.MeterReading meterReading)
        {
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                {new NpgsqlParameter("paccountid", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = meterReading.AccountId } },
                {new NpgsqlParameter("pmeterreadingdatetime", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = meterReading.MeterReadingDateTime_DateTime } },
                {new NpgsqlParameter("pmeterreadvalue", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = meterReading.MeterReadValue } }
            };

            ExecuteProcedure("ensek.addMeterReading", parameters);
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
