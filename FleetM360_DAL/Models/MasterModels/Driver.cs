using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class Driver : EntityWithIdentityId<long>
    {
        public long DriverNumber { get; set; }
        public string FullName { get; set; }
        [MaxLength(11)]
        public string? PhoneNumber { get; set; }
        public string? Company { get; set; }
        public string? address { get; set; }
        public string? NationalID { get; set; }
        public string? PersonalPhoto { get; set; }

        public string? UserToken { get; set; }
        public string UserId { get; set; }
        public DateTime? DriverLicenceExpireDate { get; set; }
        public string? DriverLicenceNumber { get; set; }
        [DefaultValue(false)]
        public bool? ConditionsAccept { get; set; }
    }
}
