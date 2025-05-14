using FleetM360_DAL.Models.MasterModels;
using FleetM360_DAL.Models.MasterModels.HazardData;
using FleetM360_PLL.APIViewModels.Hazard;
using FleetM360_PLL.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FleetM360_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HazardRiskController : ControllerBase
    {

        private IRiskService _service;
       // private IRiskService _riskService;
       // private IShipmentStatusService _shipmentStatusService;

        public HazardRiskController(
          IRiskService service
         // IRiskService riskService
         // IShipmentStatusService shipmentStatusService
         )
        {
            _service = service;
            //_riskService = riskService;
            //_shipmentStatusService = shipmentStatusService;
        }

        [HttpPost]
        [Route("AllRisk")]
        public async Task<IActionResult> GetRisks(ApiTemplate template)
        {
            if (template == null || string.IsNullOrEmpty(template.Country))
                return (IActionResult)BadRequest((object)new
                {
                    flag = false,
                    message = "Error",
                    data = 0
                });
            IEnumerable<Risk> allAsyncByCountry = await _service.GetAllAsyncByCountry(template);
            if (allAsyncByCountry == null)
                return (IActionResult)BadRequest((object)new
                {
                    flag = false,
                    message = "Error",
                    data = 0
                });
            List<ApiTemplate> apiTemplateList = new List<ApiTemplate>();
            foreach (Risk riskBusinessModel in allAsyncByCountry)
            {
                ApiTemplate apiTemplate = new ApiTemplate()
                {
                    Risk_ID = riskBusinessModel.ID,
                    Risk_AR = riskBusinessModel.RiskText,
                    Risk_EN = riskBusinessModel.RiskText,
                    Active = riskBusinessModel.Active,
                    Lat = riskBusinessModel.Lat,
                    Long = riskBusinessModel.Long,
                    RiskLevel = riskBusinessModel.RiskLevel.RiskLevel_EN,
                    Country = riskBusinessModel.Country,
                    Company = riskBusinessModel.Company,
                    Destination = riskBusinessModel.Destination
                };
                apiTemplateList.Add(apiTemplate);
            }
            return (IActionResult)Ok((object)new
            {
                flag = true,
                message = "Done",
                data = apiTemplateList
            });
        }


        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add([FromBody] ApiTemplate template)
        {
            if (template != null && !string.IsNullOrEmpty(template.Country))
            {
                var driverFeedback = new DriverFeedback()
                {
                    Shipment_ID = template.Shipment_ID,
                    Risk_ID = template.Risk_ID.ToString(),
                    lat = template.Lat,
                    Long = template.Long,
                    DriverMobile = template.MobileNumber,
                    Country = template.Country,
                    Company = template.Company
                };
                var result = await _service.AddDriverFeedbackAsync(driverFeedback);
                if (result == 0)
                    return BadRequest(new { flag = false, message = "Error, Cannot add the driver feedback", data = 0 });
                if (result == -1)
                    return BadRequest(new { flag = false, message = "DriverFeedback is Already exist", data = 0 });

                //var number = await _service.GetNumberUnReadAsync(template);
                //await _hubContext.Clients.All.SendAsync("Notify", number, driverFeedback);
                return Ok(new { flag = true, message = "DriverFeedback is Add", data = 0 });
            }
            return BadRequest(new { flag = false, message = "Error, Cannot add the driver feedback", data = 0 });
        }

        //[HttpPost]
        // [Route("Update")]
        //public async Task<IActionResult> Add([FromBody] List<ApiTemplate> templateList)
        //{
        //    int num = await _service.AddAsync(templateList);
        //    return num != -1 ? (IActionResult)Ok((object)new
        //    {
        //        flag = true,
        //        message = string.Format("Number of shipment risk added =   {0}", (object)num),
        //        data = num
        //    }) : (IActionResult)BadRequest((object)new
        //    {
        //        flag = false,
        //        message = "Error, Cannot add the shipment risks",
        //        data = 0
        //    });
        //}

    }

}
