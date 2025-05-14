using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels.HazardData
{
    public class Risk
    {
        public int ID { get; set; }
        public string RiskText { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string? Comment { get; set; }
        public bool Active { get; set; }
        //public string Shipment_ID { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Company { get; set; }

        //[ForeignKey("RiskLevel")]
        public int RiskLevelID { get; set; }
        public virtual RiskLevel RiskLevel { get; set; }

        public string? Destination { get; set; }
    }
}
