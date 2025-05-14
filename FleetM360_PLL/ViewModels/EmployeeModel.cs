using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.ViewModels
{
    public class EmployeeModel
    {
        public long EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string UserId { get; set; }
        [MaxLength(11)]
        public string PhoneNumber { get; set; }
        public string Company { get; set; }
        public string address { get; set; }
        public string NationalID { get; set; }
        public string? PersonalPhoto { get; set; }

        public string? UserToken { get; set; }
        public bool? ConditionsAccept { get; set; }
        public long Id { get; set; }

        [DefaultValue(false)]
        public bool IsDelted { get; set; }

        [DefaultValue(true)]
        public bool IsVisible { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //[Required]
        //[EmailAddress]
        //[Display(Name = "Email")]
        //public string Email { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Password")]
        //public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }
    }
}
