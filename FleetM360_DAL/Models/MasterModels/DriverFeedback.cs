using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class DriverFeedback
    {
        public int ID { get; set; }
        public string? DriverNumber { get; set; }
        public EnumType Type { get; set; }
        public string? DriverMobile { get; set; }
        public EnumStatus Status { get; set; }
        public string? Shipment_ID { get; set; }
        public string? Risk_ID { get; set; }
        public EnumDecision Decision { get; set; }
        public bool isRead { get; set; }
        public string? lat { get; set; }
        public string? Long { get; set; }
        public string? Country { get; set; }
        public string? Company { get; set; }
        public string? SubmitDate { get; set; }
        public string? TruckNumber { get; set; }
    }
    public enum EnumType
    {
        Add = 1,
        Remove = 2
    }
    public enum EnumStatus
    {
        Pending = 1,
        Complete = 2
    }
    public enum EnumDecision
    {
        Approve = 1,
        Reject = 2
    }
}
