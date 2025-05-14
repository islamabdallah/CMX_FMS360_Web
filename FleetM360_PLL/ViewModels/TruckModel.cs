using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.ViewModels
{
    public class TruckModel
    {
        public long Id { get; set; }

        [DefaultValue(false)]
        public bool IsDelted { get; set; }

        [DefaultValue(true)]
        public bool IsVisible { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string TruckNumber { get; set; }//		
        public string? SapKey { get; set; }//		
        public string Type { get; set; }
        public string Category { get; set; }
        public string LicenceNumber { get; set; }
        public string? DeviceId { get; set; }
        public DateTime? LicenceExpireDate { get; set; }
        public string? status { get; set; }
        public double? Long { get; set; }
        public double? Lat { get; set; }

        //public DateTime? LastCheck { get; set; }//		
        public string? Location { get; set; }
        public string? Model { get; set; }
        public string? Year { get; set; }
        public string? TruckManufacturer { get; set; }
        public string? Chassis { get; set; }
        public string? Engine { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
