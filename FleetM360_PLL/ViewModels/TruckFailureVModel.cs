using FleetM360_DAL.Models.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.ViewModels
{
    public class TruckFailureVModel
    {
        public TruckFailure trucks{ get; set; }
        public bool hasActiveTrip { get; set; }

        public Trip? activeTrip { get; set; }
    }
}
