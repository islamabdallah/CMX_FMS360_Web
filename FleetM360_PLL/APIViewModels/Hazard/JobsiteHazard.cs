using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.APIViewModels.Hazard
{
    public class JobsiteHazard
    {
        public int ID { get; set; }
        public string Jobsite_ID { get; set; }
        public string Jobsite_AR { get; set; }
        public string Jobsite_EN { get; set; }
        public string Jobsite_Lat { get; set; }
        public string Jobsite_Long { get; set; }
        public string Country { get; set; }
        public string Company { get; set; }
        public string Risk_AR { get; set; }
        public string Risk_EN { get; set; }
        public string Image { get; set; }
        public string Risk_Lat { get; set; }
        public string Risk_Long { get; set; }
    }
}
