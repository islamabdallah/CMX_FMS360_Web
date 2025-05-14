using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.APIViewModels.Drivers
{
   // [Bind(include: "DriverNumber,Password")]
    public class LoginModel
    {
        public long UserNumber { get; set; }      
        public int languageId { get; set; }
        public long? truckId {  get; set; }
        public long id { get; set; }
        public string? Password { get; set; }
    }
   
    public class UserData
    {
        public TokenModel TokenModel { get; set; }
        public long UserNumber { get; set; }
        public bool termsAndConditionsAccept { get; set; }


    }
    public class sendMaintenanceEndTime
    {
        public string truckId { get; set; }
        public int userNumber { get; set; }
        public string? parentTrip { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public DateTime dateTime { get; set; }
        public int languageId { get; set; }
    }
    public class UserApiModel
    {
        public long UserNumber { get; set; }
        public int languageId { get; set; }
        public string? truckId { get; set; }
        public string? tripId { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public int? tripLocationId { get; set; }
    }


    public class StartTripApiModel
    {
        public bool medicalStatus { get; set; }
        public string? screen { get; set; }
        public List<PrecheckQuestionApiModel> preCheckQuestions { get; set; }


    }
    public class QuestionDataModel
    {
        public List<PrecheckQuestionApiModel> preCheckQuestions { get; set; }
    }
    public class PrecheckQuestionApiModel
    {
        public int questionId { get; set; }
        public string questionName { get; set; }
        public string questionCategory { get; set; }
        public List<PrecheckAnswerApiModel> questionNAnswers { get; set; }

    }
    public class PrecheckAnswerApiModel
    {
        public string answerName { get; set; }
        public bool answerValue { get; set; }

    }



}
