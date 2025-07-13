namespace MeterReadingsApi.Data.Entities
{
    public class Account : BaseEntity
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public virtual ICollection<MeterReading>? MeterReadings { get; set; }
    }
}
