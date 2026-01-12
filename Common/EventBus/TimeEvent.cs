
namespace Common.EventBus
{
    public class TimeEvent
    {
        public const string EventKey = "/EV.BUS.TIME";
        public TimeEvent() { }
        public long RuleId { get; set; }
        public bool HasValue { get; set; }
        public string ProductId { get; set; }
        public string HttpParams { get; set; }
        public string RuleJson { get; set; }
    }
}
