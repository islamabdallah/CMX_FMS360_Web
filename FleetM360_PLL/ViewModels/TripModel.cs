using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.ViewModels
{
    public class TripModel
    {
        //trip master Data
        public long ParentTrip { get; set; }
        public long TripNumber { get; set; }//Sap Number
        public string TruckNumber { get; set; }
        public string SiloNumber { get; set; }
        public long TypeId { get; set; } // (Cement/Backuling/Missions)
        public long SubTypeId { get; set; }
        public DateTime Date { get; set; }
        public long StatusId { get; set; } //(Pending/OnRoad/Completed) 
        public string StageEn { get; set; }
        public string StageAR { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsConverted { get; set; }
        public double Qty { get; set; }
        public long Id { get; set; }
        public bool IsDelted { get; set; }
        public bool IsVisible { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime ArrivedDate { get; set; }
        public DateTime departureDate { get; set; }


        //trip data related to driver & trucks
        // public TruckModel selectedTruck { get; set; }
        public List<TruckModel> Trucks { get; set; }
        public List<DriverModel> Drivers { get; set; }
        public List<JobSiteModel> Sourse { get; set; }
        public List<JobSiteModel> Distination { get; set; }
        public List<JobSiteModel> JobSites { get; set; }
        public List<TripDriverModel> selectedTripDrivrs { get; set; }

    }
}
