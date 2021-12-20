using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{   
    
    public class UP_info
    {   
        public int device_number { get; set; } = 0;
        public  int wendu { get; set; } = 0;
        public  int shidu { get; set; } = 0;
        public  string event_time { set; get; } = string.Empty;
        public  string device_id { set; get; } = string.Empty;

    }

    public class Temp_Hum_Time
    {
        public int temperature { get; set; } = 0;
        public int humidity { get; set; } = 0;

        public string event_time { set; get; } = string.Empty;

    }
}
