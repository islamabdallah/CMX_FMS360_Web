using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.APIViewModels.Hazard
{
    public class RiskBusinessModel
    {
        public int ID { get; set; }
        public string Risk_AR { get; set; }
        public string Risk_EN { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string Comment { get; set; }
        public bool Active { get; set; }
        public string Reference { get; set; }
        public string color { get; set; }
        public string RiskLevel_AR { get; set; }
        public string RiskLevel_EN { get; set; }
        public string Country { get; set; }
        public string Company { get; set; }
        public string Destination { get; set; }
    }
}
