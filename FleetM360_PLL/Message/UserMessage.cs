using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Message
{
    public static class UserMessage
    {
        public static string[] growth_Weight = new string[3]
        {
            "",
            "weight not ended yet",
            "الوزن لم ينتهي بعد"
        };
        public static string[] endMaintainance = new string[3]
       {
            "",
            "The maintenance has been completed.",
            "تم الانتهاء من الصيانة"
       };
        public static string[] startMaintainance = new string[3]
       {
            "",
            "The truck is still under maintenance.",
            "الشاحنة لا تزال تحت الصيانة"
       };
       
        public static string[] truckReplaced = new string[3]
       {
            "",
            "The truck has been replaced with another one.",
            "تم استبدال الشاحنة بشاحنة أخرى"
       };
        public static string[] failedMaintainance = new string[3]
      {
            "",
            "There was an error during maintenance. Please contact the admin.",
            "حدث خطأ أثناء الصيانة. من فضلك كلم المسؤول"
      };
        public static string[] Done = new string[3]
        {
            " ",
            "Done",
            "العملية تمت بنجاح"
        };

        public static string[] SuccessfulProcess = new string[3]
        {
            "",
            "Successful Process",
            "العملية تمت بنجاح"
        };

        public static string[] LoginDone = new string[3]
        {
            "",
            "Sucessful login",
            "تم الدخول بنجاح"
        };

        public static string[] LoginFailed = new string[3]
        {
            "",
            "Invalid User Number Or Password",
           "رقم المستخدم أو كلمة المرور خطأ"
        };


        public static string[] EmailNotFound = new string[3]
        {
            "",
           "Email not Exist",
           "البريد الالكتروني غير موجود"
        };

        public static string[] LoginIndirect = new string[3]
        {
            "",
           "Sorry, Service not available for this user",
           "هذه الخدمة ليست متاحة لهذا المستخدم"
           // "هذه الخدمة ليست متاحة الا للموظفين التابعين لشركة سيمكس"
        };

        public static string[] LoginInvalidNumber = new string[3]
        {
            "",
            "User Number not Exist",
            "رقم المستخدم خطأ"
        };

        public static string[] SuccessfulPasswordChange = new string[3]
        {
            "",
            "Sucessful process, Your Password has been changed",
            "تم تغيير كلمة المرور بنجاح"
        };

        public static string[] ChangePasswordFailed = new string[]
        {
            "",
            "Wrong old password",
           "عملية فاشلة ، كلمة المرور السابقة خطأ"
        };

        public static string[] InvalidEmployeeData = new string[]
        {
            "",
            "Failed Process, Invalid User Data",
            "عملية فاشلة ، بيانات المستخدم خطأ"
        };
        public static string[] NoPhoneNumber = new string[]
       {
            "",
            "Failed Process, No Phone Number",
            "عملية فاشلة ، لا يوجد رقم هاتف"
       };

        public static string[] InValidData = new string[]
        {
            "",
            "Invaild data",
            "بيانات خاطئة"
        };
        public static string[] InValidPassword = new string[]
       {
            "",
            "Invaild Password",
            "كلمة المرور لا تتطابق مع الشروط المطلوبة"
       };

        public static string[] FailedProcess = new string[]
        {
            "",
            "Failed Process",
            "عملية فاشلة"
        };

        public static string[] WrongToken = new string[]
            {
                "",
                "session has been expired!",
                "يرجى تسجيل الدخول مرة أخرى!"
            };

    }

    public static class truckMessage
    {
        public static string[] Done = new string[3]
        {
          " ",
          nameof (Done),
          "العملية تمت بنجاح"
        };
        public static string[] SuccessfulProcess = new string[3]
        {
          "",
          "Successful Process",
          "العملية تمت بنجاح"
        };
        public static string[] InvalidTruck = new string[3]
        {
          "",
          "Failed Process, Truck Not Exist",
          "عملية فاشلة ، رقم الشاحنة غير موجود "
        };
        public static string[] AlreadyAssigned = new string[3]
        {
          "",
          "Failed Process, Truck Already Assigned To Mobile",
          "عملية فاشلة ، الشاحنة تابعة لموبايل "
        };
        public static string[] InValidData = new string[3]
        {
          "",
          "Invaild Truck",
          "بيانات خاطئة"
        };
        public static string[] FailedProcess = new string[3]
        {
          "",
          "Failed Process",
          "عملية فاشلة"
        };

    }
}
