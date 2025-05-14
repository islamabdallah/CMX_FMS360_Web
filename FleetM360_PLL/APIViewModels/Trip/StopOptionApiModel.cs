using FleetM360_DAL.Models.MasterModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FleetM360_PLL.APIViewModels.Trip
{
    public class StopOptionApiModel
    {
        public string id { get; set; }
        public string label { get; set; }
        public string? iconBath { get; set; }
        public string? color { get; set; }
    }
    public class StopDataApiModel
    {
        public int languageId { get; set; }
        public long userNumber { get; set; }
        public string? truckId { get; set; }
        public string? tripId { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public int stopOptionId {  get; set; }
    }
    public class FuelDataModel{
       public List<GasStationModel>gasStations { get; set; }
        public List<CashPaymentMethodModel>cashPaymentMethodModel { get; set; }
    }
    public class GasStationModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }
    public class CashPaymentMethodModel
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? icon { get; set; }
    }

    public class sendFuelDataApiModel
    {
        public int userNumber { get; set; }
        public int languageId { get; set; }
        public string? truckId { get; set; }
        public string? tripId { get; set; }
        public string gasStationId { get; set; }
        public double numberOfKilometers { get; set; }
        public double doubleFuelQuantityInnLiters { get; set; }
        public double fuelCost { get; set; }
        public string cashPaymentMethodId { get; set; }
        public string? driverComment { get; set; }
        public List<IFormFile>? numberOfKilometersImages { get; set; }
        public List<IFormFile>? images { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
    }
}
