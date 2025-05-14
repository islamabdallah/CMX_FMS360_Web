using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.ViewModels
{
    public class JobSiteModel
    {
        public long Id { get; set; }

        [DefaultValue(false)]
        public bool IsDelted { get; set; }

        [DefaultValue(true)]
        public bool IsVisible { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }


        public double Longitude { get; set; }

        public string Desc { get; set; }

        public bool HasNetworkCoverage { get; set; }
        public string Material { get; set; }
        public double Qty { get; set; }
    }
}
