using FleetM360_DAL.Models.MasterModels;
using FleetM360_DAL.Models.MasterModels.HazardData;
using FleetM360_DAL.Repository.EntityFramework;
using FleetM360_PLL.APIViewModels.Hazard;
using FleetM360_PLL.APIViewModels.Trucks;
using FleetM360_PLL.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FleetM360_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HazardRiskController : ControllerBase
    {

        private IRiskService _service;
        private readonly ApplicationDBContext _context;

        public HazardRiskController(
          IRiskService service, ApplicationDBContext context
         )
        {
            _service = service;
            _context = context;
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
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
            ApiTemplateModel apiTemplateModel = new ApiTemplateModel();
            apiTemplateModel.risks = new List<ApiTemplate>();
            foreach (Risk riskBusinessModel in allAsyncByCountry)
            {
                ApiTemplate apiTemplate = new ApiTemplate()
                {
                    Risk_ID = riskBusinessModel.ID,
                    Risk_AR = riskBusinessModel.RiskText,
                    Risk_EN = riskBusinessModel.RiskText,
                    Active = riskBusinessModel.Active,
                    Lat =riskBusinessModel.Lat,
                    Long = riskBusinessModel.Long,
                    RiskLevel = riskBusinessModel.RiskLevel.RiskLevel_EN,
                    Country = riskBusinessModel.Country,
                    Company = riskBusinessModel.Company,
                    Destination = riskBusinessModel.Destination
                };
                apiTemplateModel.risks.Add(apiTemplate);
            }
            apiTemplateModel.isConverted = false;
            if (!string.IsNullOrEmpty(template.Shipment_ID))
            {
                var trip = await _context.Trips.Where(t => t.IsVisible == true && t.Id == Convert.ToInt64(template.Shipment_ID)).FirstOrDefaultAsync();

                if (trip != null)
                {
                    if (trip.IsConverted == true && trip.ConvertedSeen == false)
                    {
                        apiTemplateModel.isConverted = true;
                    }
                }

            }
            return (IActionResult)Ok((object)new
            {
                flag = true,
                message = "Done",
                data = apiTemplateModel
            });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
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
                    lat = template.Lat.ToString(),
                    Long = template.Long.ToString(),
                    DriverMobile = template.MobileNumber,
                    DriverNumber = template.MobileNumber,
                    Country = template.Country,
                    Company = template.Company
                };
                var result = await _service.AddDriverFeedbackAsync(driverFeedback);
                if (result == 0)
                    return BadRequest(new { flag = false, message = "Error, Cannot add the driver feedback", data = 0 });
                if (result == -1)
                    return BadRequest(new { flag = false, message = "DriverFeedback is Already exist", data = 0 });
                convertCheckResult resultt = new convertCheckResult();

                resultt.isConverted = false;
                if (!string.IsNullOrEmpty(template.Shipment_ID))
                {
                    var trip = await _context.Trips.Where(t => t.IsVisible == true && t.Id == Convert.ToInt64(template.Shipment_ID)).FirstOrDefaultAsync();
                   
                    if(trip != null)
                    {
                        if (trip.IsConverted == true && trip.ConvertedSeen == false)
                        {
                            resultt.isConverted = true;
                        }
                    }
                  
                }
                return Ok(new { flag = true, message = "DriverFeedback is Add", data = resultt });
            }
            return BadRequest(new { flag = false, message = "Error, Cannot add the driver feedback", data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Add([FromBody] List<ApiTemplate> templateList)
        {
            bool isConvert = false;
            if(templateList == null )
                return (IActionResult)BadRequest((object)new
                {
                    flag = false,
                    message = "Error, Cannot add the shipment risks",
                    data = 0
                });
            if (templateList.Count > 0)
            {
                if (!string.IsNullOrEmpty(templateList[0].Shipment_ID))
                {
                    var trip = await _context.Trips.Where(t => t.IsVisible == true && t.Id == Convert.ToInt64(templateList[0].Shipment_ID)).FirstOrDefaultAsync();

                    if (trip != null)
                    {
                        if (trip.IsConverted == true && trip.ConvertedSeen == false)
                        {
                            isConvert = true;
                        }
                    }

                }
            }
            return templateList.Count > 0  ? (IActionResult)Ok((object)new
            {
                flag = true,
                message = string.Format("Number of shipment risk added =   {0}", (object)templateList.Count),
                data = new{num=templateList.Count, isConverted= isConvert }
            }) : (IActionResult)BadRequest((object)new
            {
                flag = false,
                message = "Error, Cannot add the shipment risks",
                data = 0
            });
        }

    }

}
