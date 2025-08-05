using FleetM360_DAL.Models.MasterModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.ViewModels
{
    public class SapTripVModel
    {
        public SapTrip sapTrip { get; set; }
        //public List<SapTrip> sapTrips { get; set; }
        public string MaterialName {  get; set; }
        public List<long> loadDrivers { get; set; }
        public List<long> onRoadDrivers { get; set; }
       // public List<DriverModel> Drivers { get; set; }
    }

    public class SapTripViewModel
    {
        public SapTrip sapTrip { get; set; }
       public TruckSilo truckSilo { get; set; }
    }

    public class SupTrippVModel
    {
        public long ParentTrip { get; set; }
        public long TripNumber { get; set; }//Sap Number
        public long TruckId { get; set; }
        public long TypeId { get; set; } // (Cement/Backuling/Missions)
        public bool FromPlant { get; set; }
        public double Qty { get; set; }
        public SapTrip sapTrip { get; set; }
        public string MaterialName { get; set; }
        public DateTime ArrivedDate { get; set; }
        public DateTime departureDate { get; set; }
        public List<long> loadDrivers { get; set; }
        public List<long> onRoadDrivers { get; set; }
       // public List<DriverModel> Drivers { get; set; }
        public List<JobSiteModel> Sourse { get; set; }
        public List<JobSiteModel> Distination { get; set; }
    }
    public class MedicalResponseApiModel
    {
        public long ParentTrip { get; set; }
        //public long TripNumber { get; set; }//Sap Number
        public long TruckId { get; set; }
        public long TypeId { get; set; } // (Cement/Backuling/Missions)

        public double Qty { get; set; }

        public DateTime ArrivedDate { get; set; }
        public DateTime departureDate { get; set; }

       
    }
    public class ConvertResponseApiModel
    {
        public long TripNumber { get; set; }
        //public long TripNumber { get; set; }//Sap Number
       

        public double Qty { get; set; }

       


    }
}
