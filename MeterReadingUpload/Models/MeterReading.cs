using System;

namespace MeterReadingUpload.Models
{
    public class MeterReading
    {
        public string AccountId { get; set; }

        public string MeterReadingDateTime { get; set; }

        public string MeterReadValue { get; set; }

        internal DateTime MeterReadingDateTime_DateTime
        {
            get
            {
                return DateTime.TryParse(MeterReadingDateTime, out DateTime result) ? result : new DateTime();
            }
        }

        internal bool Valid
        {
            get
            {
                return !String.IsNullOrEmpty(MeterReadValue)
                    && MeterReadValue.Length.Equals(5)
                    && int.TryParse(MeterReadValue, out _)
                    && int.Parse(MeterReadValue, System.Globalization.NumberStyles.Integer) > -1
                    && DateTime.TryParse(MeterReadingDateTime, out _);
            }
        }

        internal bool Recorded { get; set; }
    }
}
