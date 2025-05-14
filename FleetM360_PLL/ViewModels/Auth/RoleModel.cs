﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.ViewModels.Auth
{
    public class RoleModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string ArabicRoleName { get; set; }

        public string Id { get; set; }

        [NotMapped]
        public string[] AddIds { get; set; }

        [NotMapped]
        public string[] DeleteIds { get; set; }
        public bool isChacked { get; set; }

        public bool IsDelted { get; set; }
        public bool IsVisible { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
