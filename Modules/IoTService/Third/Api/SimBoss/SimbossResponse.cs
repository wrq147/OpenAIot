using System;
using Newtonsoft.Json;


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
            return JsonConvert.SerializeObject(this);
        }
    }
}
