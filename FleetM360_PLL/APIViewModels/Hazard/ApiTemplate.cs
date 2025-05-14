using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.APIViewModels.Hazard
{
    public class ApiTemplate
    {
        public int Risk_ID { get; set; }
        public string Risk_AR { get; set; }
        public string Risk_EN { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string Comment { get; set; }
        public bool Active { get; set; }
        public string Shipment_ID { get; set; }
        public string MobileNumber { get; set; }
        public string RiskLevel { get; set; }
        public string Country { get; set; }
        public string Company { get; set; }
        public string Jobsite { get; set; }
        public string Destination { get; set; }
    }
}
