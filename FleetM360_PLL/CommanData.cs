using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL
{
    public class CommanData
    {
        public static string UploadMainFolder = @"C:\inetpub\wwwroot\_fms360\wwwroot/";
        public static string TermsPath = @"D:\_cemex\_projects\_AzureMore4U\More4UAzure\MoreForYou.Services\Implementation\TermsConditions\StaticFile\TermsConditionsFiles\";

        public enum Languages
        {
            Arabic = 2,
            English = 1
        };

        public enum TripStatus
        {
            Pending = 1,
            Started = 2,
            PreCheck = 3,
            Loading = 4,
            OnRoad = 5,
            Completed = 6
        }
        public enum TripStage
        {
            Pending = 1,//قيد الانتظار
            PreCheck = 2,//قيد الفحص
            Maintenance = 3,//في الصيانة
            Loading = 4,//قيد التحميل
            OnRoad = 5,//جاري النقل
            Arrived= 6,//تم الوصول
            Ondelever = 7,//جاري التسليم
            Delevered= 8,//تم التسليم
            Completed = 9//اكتملت
        }
        /*
         * 
         */

        public enum TruckStatus
        {
            Idle = 1,
            OnTrip = 2,
            Maintainance = 3,
            OffLine = 4
        }

    }
}
