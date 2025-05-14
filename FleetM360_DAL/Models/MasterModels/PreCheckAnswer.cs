using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace FleetM360_DAL.Models.MasterModels
{
    public class PreCheckAnswer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PreCheckAnswerId { get; set; }
        public string AnswerNameEN {  get; set; }
        public string AnswerNameAR { get; set; }
        public bool AnswerValue {  get; set; }

        [DefaultValue(false)]
        public bool IsDelted { get; set; }

        [DefaultValue(true)]
        public bool IsVisible { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Foreign key
        public int PreCheckQuestionId { get; set; }

        // Navigation property - Each PreCheckAnswer belongs to one PreCheckQuestion
        public PreCheckQuestion PreCheckQuestion { get; set; }
    }
}
