namespace HttpChannel.Models
{
    public class AEPCommandInput
    {
        public AEPConten content { get; set; }
        public string deviceId { get; set; }
        public string Operator { get; set; }
        public long productId { get; set; }
        public int ttl { get; set; }
        public int deviceGroupId { get; set; }
        public int level { get; set; }

    }
    public class AEPConten
    {
        public Params Params { get; set; }
        public string serviceIdentifier { get; set; }
    }

    public class Params
    {
        public string data { get; set; }
    }
}
