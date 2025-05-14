using FleetM360_PLL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.APIViewModels.Trip
{
    public class TripApiModel
    {
        public string? truckId { get; set; }
        public string? truckNumber { get; set; }
        public string? tripId { get; set; }
        public string? loadingDriver { get; set; }
        public string? roadDriver { get; set; }
        public List<SubTripApiModel> subTrips { get; set; }
        // public userApiModel driver { get; set; }
    }
    public class HomeDataModel
    {
        public userApiModel driver { get; set; }
        public List<TripApiModel> trips { get; set; }
    }
    public class SubTripApiModel
    {
        public string? truckNumber { get; set; }
        public string? truckId { get; set; }
        public string? tripId { get; set; }
        public string? material { get; set; }
        public double? quantity { get; set; }
        public string? status { get; set; }
        public int? start { get; set; }
        public string? fromAddress { get; set; }
        public string? toAddress { get; set; }
        public string? fromDate { get; set; }
        public string? toDate { get; set; }
        public List<LocationApiModel> fromLocations { get; set; }
        public List<LocationApiModel> toLocations { get; set; }
    }
   
    public class LocationApiModel
    {
        public int? tripLocationId { get; set; }
        public string? customerName { get; set; }
        public string? customerPhoneNumber { get; set; }
        public string? recipientName { get; set; }
        public string? recipientPhoneNumber { get; set; }
        public string? status { get; set; }
        public string? address { get; set; }
        public string? materialType { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public double? qty { get; set; }
        public double? remainqty { get; set; }

    }
    public class userApiModel
    {
        public string userNumber { get; set; }
        public string userName { get; set; }
        public string userPhoneNumber { get; set; }

    }
}
