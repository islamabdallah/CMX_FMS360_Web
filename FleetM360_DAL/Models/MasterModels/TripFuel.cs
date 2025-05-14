using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class TripFuel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long userNumber { get; set; }
        public long TripLogId { get; set; }
        public string? truckId { get; set; }
        public string? tripId { get; set; }
        public long gasStationId { get; set; }
        public double numberOfKilometers { get; set; }
        public double doubleFuelQuantityInnLiters { get; set; }
        public double fuelCost { get; set; }
        public long cashPaymentMethodId { get; set; }
        public string driverComment { get; set; }
        //public List<IFormFile>? numberOfKilometersImages { get; set; }
        //public List<IFormFile>? images { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        [DefaultValue(false)]
        public bool IsDelted { get; set; }

        [DefaultValue(true)]
        public bool IsVisible { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
