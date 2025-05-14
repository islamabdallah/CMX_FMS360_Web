using FleetM360_PLL.APIViewModels.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.APIViewModels.Trip
{
    public class Take5APIDataModel
    {
        public List<QuestionModel>? stepOne { get; set; }//done
        public List<OnSiteRiskModel> onSiteRisks { get; set; }
        public LocationQtyDataModel? locationQtyData { get; set; }
        public List<QuestionModel>? stepTwo { get; set; }//done
    }

    public class QuestionModel
    {
        public int? questionId { get; set; }
        public string? questionName { get; set; }
        public List<PrecheckAnswerApiModel>? questionNAnswers { get; set; }
        public string? questionCategory { get; set; }
    }
    public class OnSiteRiskModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public List<string> waysToDealWithOnSiteRisk { get; set; }
    }
    public class LocationQtyDataModel
    {
        public double? totalQty { get; set; }
        public double? remainingQty { get; set; }
        public List<ReceivedQuantityModel>? receivedQuantities { get; set; }
    }
    public class ReceivedQuantityModel
    {
        public double? qty { get; set; }
        public DateTime? receivedDateTime { get; set; }
    }

    public class sendTake5DataApiModel{
        public int userNumber { get; set; }
        public int languageId { get; set; }
        public string truckId { get; set; }
        public string tripId { get; set; }
        public AnsweredTake5QuestionsModel step1 { get; set; }
        public AnsweredTake5QuestionsModel step2 { get; set; }
        public List< OnSiteRiskModel >? onSiteRisks { get; set; }
        public Take5UnLoadingModel unLoading { get; set; }
        public int tripLocationId { get; set; }
    }

    public class AnsweredTake5QuestionsModel
    {
        public List<int>? falseIds { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public DateTime? start { get; set; }//in step2 = endUnloadingTime
        public DateTime? end { get; set; } //in step1=start unloadingTime
    }
    public class Take5UnLoadingModel
    {
        public double? receivedQty { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public int? hours { get; set; }
        public int? minutes { get; set; }
        public int? seconds { get; set; }
    }
}
