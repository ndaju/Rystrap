namespace Rystrap.Models.Persistable
{
    public class BanEvent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string PlaceId { get; set; } = "";
        public string PlaceName { get; set; } = "";
        public string ServerId { get; set; } = "";
        public string BanType { get; set; } = "Kick";
        public string Reason { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string AccountName { get; set; } = "";
    }
}
