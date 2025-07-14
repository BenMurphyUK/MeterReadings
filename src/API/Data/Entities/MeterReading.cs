namespace MeterReadingsApi.Data.Entities
{
    public class MeterReading : BaseEntity
    {
        public int AccountId { get; set; }

        public virtual Account Account { get; set; } = null!;

        public DateTime MeterReadingDateTime { get; set; }

        public int MeterReadValue { get; set; }
    }
}
