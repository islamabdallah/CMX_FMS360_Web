using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace FleetM360_DAL.Models.MasterModels
{
    public class PreCheckQuestion 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PreCheckQuestionId { get; set; }
        public string QuestionName { get; set; }
        public string Category { get; set; }
        public string MainCategory { get; set; }



        [DefaultValue(false)]
        public bool IsDelted { get; set; }

        [DefaultValue(true)]
        public bool IsVisible { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation property - One question has many answers
        public ICollection<PreCheckAnswer> PreCheckAnswers { get; set; } = new List<PreCheckAnswer>();
    }
}
