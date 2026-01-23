using Common.Json;
using System;

namespace IoTService.Third.Api.SimBoss
{
    public class SimbossResponse<T>
    {
        public String Message
        {
            get; set;
        }

        public String Detail
        {
            get; set;
        }

        public String Code
        {
            get; set;
        }

        public Boolean Success
        {
            get; set;
        }

        public T Data
        {
            get; set;
        }

        public override String ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this, MyDefaultTextJsonConfig.DefaultOptions);
        }
    }
}
