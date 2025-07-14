using CsvHelper.Configuration;

namespace MeterReadingsApi.Models.Dtos
{
    public class MeterReadingDto
    {
        public int AccountId { get; set; }
        public DateTime MeterReadingDateTime { get; set; }
        // Will this always be int32?
        public int MeterReadValue { get; set; }

        public class CsvMap : ClassMap<MeterReadingDto>
        {
            public CsvMap()
            {
                Map(x => x.AccountId).Name("AccountId");

                Map(x => x.MeterReadingDateTime)
                    .TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>()
                    .TypeConverterOption.Format("dd/MM/yyyy HH:mm")
                    .Name("MeterReadingDateTime");

                Map(x => x.MeterReadValue).Name("MeterReadValue");
            }
        }
    }
}
