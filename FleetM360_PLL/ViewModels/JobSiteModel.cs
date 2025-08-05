using FleetM360_DAL.Models.MasterModels;
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
        public string? Number { get; set; }
        public double Latitude { get; set; }

        public double Longitude { get; set; }
        public string? City { get; set; }
        public string Desc { get; set; }

        public bool HasNetworkCoverage { get; set; }
        public string? State { get; set; }
        public string? Type { get; set; }

        public string? CustomerNumber { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerPhoneNumber { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientPhoneNumber { get; set; }
        public string? IdealKM { get; set; }
        public string? IdealTime { get; set; }
        public string Material { get; set; }
        public double Qty { get; set; }
    }
    public class JobSiteVModel
    {
        public List<JobSite> sites { get; set; }
        public List<Material> materials { get; set; }
    }
}
