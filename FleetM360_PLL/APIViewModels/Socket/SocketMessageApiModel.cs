using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.APIViewModels.Socket
{
    public class SocketMessageApiModel
    {
        public string status { get; set; }
        public DateTime time { get; set; }
        public string? Weight_value { get; set; }
        public SocketTruckApiModel? truck { get; set; }
       // public  {  get; set; }
    }
    public class SocketTruckApiModel
    {
        public string truckId { get; set; }
    }
}
