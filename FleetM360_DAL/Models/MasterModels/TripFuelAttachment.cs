using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class TripFuelAttachment
    {
        public long Id { get; set; }
        public long TripFuelId { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        [DefaultValue(false)]
        public bool IsDelted { get; set; }
        [DefaultValue(true)]
        public bool IsVisible { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
