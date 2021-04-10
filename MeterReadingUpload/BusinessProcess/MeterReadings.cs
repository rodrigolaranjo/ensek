using CsvHelper;
using MeterReadingUpload.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MeterReadingUpload.BusinessProcess
{
    internal class MeterReadings
    {
        internal static MeterReadingResult Add(string csv)
        {
            List<Models.MeterReading> meterReadings = GetReadingsFromCSV(csv);

            StoreMeterReadings(ref meterReadings);

            MeterReadingResult meterReadingResult = new MeterReadingResult()
            {
                successful = meterReadings.Where(mr => mr.Recorded).Count(),
                failed = meterReadings.Where(mr => !mr.Recorded).Count()
            };

            return meterReadingResult;
        }

        private static void StoreMeterReadings(ref List<MeterReading> meterReadings)
        {
            foreach (MeterReading meterReading in meterReadings.Where(mr => mr.Valid))
                try
                {
                    Storage.RecordMeterReading(meterReading);
                    meterReading.Recorded = true;
                }
                catch
                {
                    //We may opt for a log or just ignore.
                }
        }

        private static List<MeterReading> GetReadingsFromCSV(string csv)
        {
            using StreamReader reader = new StreamReader(GenerateStreamFromString(csv));
            using CsvReader csvr = new CsvReader(reader, CultureInfo.InvariantCulture);
            IEnumerable<MeterReading> records = csvr.GetRecords<Models.MeterReading>();
            return records.ToList();
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
