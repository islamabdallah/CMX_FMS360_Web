using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.APIViewModels.Trucks
{
    public class TruckApiModel
    {
        public string? truckNumber { get; set; }
        public string?  truckId { get; set; }
        public string? truckStatus { get; set; }
        public double? truckLocationLat { get; set; }
        public double? truckLocationLong { get; set; }
        public string? truckLastCheck { get; set; }
        public string? truckLastLocation { get; set; }
        public string? truckModel { get; set; }
        public string? truckYear { get; set; }
        public string? truckManufacturer { get; set; }
        public string? truckChassis { get; set; }
        public string? truckEngine { get; set; }
        public string? truckLicenseNumber { get; set; }
        public string? truckPhoneNumber { get; set; }
        public string? deviceId { get; set; }
    }

    public class AssignedTruckApiModel
    {
        public long UserNumber { get; set; }
        public int languageId { get; set; }
        public string truckNumber { get; set; }
        public string truckId { get; set; }
        public string deviceId { get; set; }
        public string truckPhoneNumber { get; set; }
    }

    public class TruckStatusApiModel
    {
        public string? truckId { get; set; }
        public int languageId { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public DateTime dateTime { get; set; } = DateTime.Now;
        public string parentTrip { get; set; }
        public string subTrip { get; set; }
        public int locationId { get; set; }
        public int userNumber { get; set; }

    }
}
