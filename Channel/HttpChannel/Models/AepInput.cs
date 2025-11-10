namespace HttpChannel.Models
{
    public class AepInput
    {
        public long Timestamp { get; set; }
        public string TenantId { get; set; }
        public long ServiceId { get; set; }
        public string ProductId { get; set; }
        public string MessageType { get; set; }
        public int EventType { get; set; }
        public EventContent EventContent { get; set; }
        public string DeviceId { get; set; }
        public string IMEI { get; set; }
    }
}
