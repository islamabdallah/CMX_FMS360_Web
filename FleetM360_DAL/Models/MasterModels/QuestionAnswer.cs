using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class QuestionAnswer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string AnswerNameEN { get; set; }
        public string AnswerNameAR { get; set; }
        public bool AnswerValue { get; set; }

        [DefaultValue(false)]
        public bool IsDelted { get; set; }

        [DefaultValue(true)]
        public bool IsVisible { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Foreign key
        public int QuestionId { get; set; }

        // Navigation property - Each PreCheckAnswer belongs to one PreCheckQuestion
        public Question Question { get; set; }
    }
}
