using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class DangerCategory : EntityWithIdentityId<int>
    {
        [Required]
        public string Name { get; set; }
    }
}
