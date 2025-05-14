using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels.HazardData
{
    public class RiskLevel
    {
        public int ID { get; set; }
        public string RiskLevel_AR { get; set; }
        public string RiskLevel_EN { get; set; }
        public bool Active { get; set; }
        public string color { get; set; }
        public string Comment { get; set; }
    }
}
