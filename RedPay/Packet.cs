using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedPay
{
    public class Packet
    {
        public string app { get; set; } //APP
        public object data { get; set; } //VALUE
        public string dataformat { get; set; } // Value format(XML/JSON):- X/J 
        public string iv { get; set; } //IV
        public string responsecode { get; set; } //RESPONSE CODE
        public string compress { get; set; } // Compress T / F
        public string encrypt { get; set; } //Encrypt T / F
        public string utctime { get; set; } //UTC Time yyyy-MM-dd HH:MM:ss.fff
    }
}
