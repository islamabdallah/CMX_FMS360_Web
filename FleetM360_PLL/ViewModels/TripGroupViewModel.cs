using FleetM360_DAL.Models.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.ViewModels
{
    public class TripGroupViewModel
    {
        public long ParentTrip { get; set; }
        public DateTime DepartureDate { get; set; }
        public List<Trip> Trips { get; set; } = new List<Trip>();
    }
}
