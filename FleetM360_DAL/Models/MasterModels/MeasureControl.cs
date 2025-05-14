using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class MeasureControl : EntityWithIdentityId<long>
    {
        public string Name { get; set; }
        public int DangerId { get; set; }
        [Required]
        public Danger Danger { get; set; }
    }
}
