using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class Danger : EntityWithIdentityId<int>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int DangerCategoryId { get; set; }

        public DangerCategory DangerCategory { get; set; }
        [Required]
        public string Icon { get; set; }

        //  public int step { get; set; }
    }
}
