using FleetM360_DAL.Models.MasterModels;
using FleetM360_PLL.APIViewModels.Trip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.ViewModels
{
    public class SubTripDetailsViewModel
    {
        public Trip trip { get; set; }
        public List<LocationApiModel>? fromLocations { get; set; }
        public List<LocationApiModel> toLocations { get; set; }
        public List<long> driversNumber { get; set; }
        public List<Driver> drivers { get; set; }
        public double AllRemainQty { get; set; }
        public List<long>? remainLocations { get; set; }
        public List<JobSiteModel> JobSites { get; set; }

    }
}
