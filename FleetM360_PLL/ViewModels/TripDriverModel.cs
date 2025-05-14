using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.ViewModels
{
    public class TripDriverModel
    {
        public long Id { get; set; }
        public long ParentTrip { get; set; }
        public long TripNumber { get; set; }//Sap Number
        public string TruckNumber { get; set; }
        public string SiloNumber { get; set; }
        public long DriverId { get; set; } // Employee Id
        public string Role { get; set; } //on raod/ loading/ CooDriver
        [DefaultValue(false)]
        public bool IsDelted { get; set; }

        [DefaultValue(true)]
        public bool IsVisible { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
