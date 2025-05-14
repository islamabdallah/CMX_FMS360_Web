using FleetM360_DAL.Models.MasterModels;
using FleetM360_DAL.Models.MasterModels.HazardData;
using FleetM360_DAL.Repository.EntityFramework;
using FleetM360_PLL.APIViewModels.Hazard;
using FleetM360_PLL.Services.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Implementation
{
    public class RiskService : IRiskService
    {
        private ApplicationDBContext _context;

        public RiskService(
          ApplicationDBContext context
         )
        {
            _context = context;
        }
        public Task<int> AddAsync(Risk risk)
        {
            throw new NotImplementedException();
        }

        public async Task<int> AddDriverFeedbackAsync(DriverFeedback driverFeedback)
        {
            try
            {
                ApiTemplate template = new ApiTemplate() { Shipment_ID = driverFeedback.Shipment_ID, Country = driverFeedback.Country, Company = driverFeedback.Company };
                var selectedDriver = _context.Drivers.Where(t=>t.IsVisible==true && t.PhoneNumber==driverFeedback.DriverMobile).FirstOrDefault();
                var selectedShipment = _context.Trips.Where(t => t.IsVisible == true && t.Id ==Convert.ToInt64(driverFeedback.Shipment_ID)).FirstOrDefault();
                //Update Truck Number
                //if (!string.IsNullOrEmpty(driverFeedback.DriverMobile))
                //{
                //    var _trucknumber = //await _shipmentRepo.GetTruckDetails(driverFeedback.DriverMobile, driverFeedback.Country, driverFeedback.Company);
                //    if (_trucknumber != null)
                //    {
                //        driverFeedback.TruckNumber = _trucknumber.TruckNumber.ToString();
                //    }
                //}

                // var selectedShipment = await _shipmentRepo.GetByshipment_ID(template);
                if (selectedDriver != null)
                {
                    driverFeedback.DriverNumber = selectedDriver.DriverNumber.ToString();
                }
                if (selectedShipment != null)
                {
                    driverFeedback.TruckNumber = selectedShipment.TruckNumber;
                }

                //  driverFeedback.TruckNumber = "islam";
                //   driverFeedback.DriverName = "islam";
                driverFeedback.SubmitDate = DateTime.Now.ToString();
                driverFeedback.Status = EnumStatus.Pending;
                driverFeedback.isRead = false;

                if (driverFeedback.Risk_ID != null && driverFeedback.Risk_ID != "0")
                    driverFeedback.Type = EnumType.Remove;
                else if (driverFeedback.lat != null && driverFeedback.Long != null && driverFeedback.Risk_ID == "0")
                    driverFeedback.Type = EnumType.Add;
                else
                    return 0;

                //var result = await _context.DriverFeedback.AddAsync(driverFeedback);
                await _context.DriverFeedbacks.AddAsync(driverFeedback);
                var result = await _context.SaveChangesAsync();
                return result;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public Task<IEnumerable<RiskBusinessModel>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Risk>> GetAllAsyncByCountry(ApiTemplate template)
        {
            try
            {
                //To be considered 
                template.Destination = string.Empty;
                template.Company = string.Empty;

                string sql = string.Empty;
                if (!string.IsNullOrEmpty(template.Country) && !string.IsNullOrEmpty(template.Company) && !string.IsNullOrEmpty(template.Destination))
                {
                    template.Country = template.Country.Trim();
                    template.Company = template.Company.Trim();
                    template.Destination = template.Destination.Trim();
                    return await _context.Risks.Where(t => t.Active == true && t.Country == template.Country && t.Company == template.Company && t.Destination == template.Destination)
                            .Include(o => o.RiskLevel)
                            .ToListAsync();
                }
                else if (!string.IsNullOrEmpty(template.Company) && string.IsNullOrEmpty(template.Destination))
                {
                    template.Country = template.Country.Trim();
                    template.Company = template.Company.Trim();
                    return await _context.Risks.Where(t => t.Active == true && t.Country == template.Country && t.Company == template.Company)
                           .Include(o => o.RiskLevel)
                           .ToListAsync();
                }
                else if (string.IsNullOrEmpty(template.Company) && string.IsNullOrEmpty(template.Destination))
                {
                    template.Company = template.Company.Trim();
                    return await _context.Risks.Where(t => t.Active == true && t.Country == template.Country)
                               .Include(o => o.RiskLevel)
                               .ToListAsync();
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Task<IEnumerable<JobsiteHazard>> GetAllAsyncByJobsite(ApiTemplate template)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RiskLevel>> GetAllRiskLevelAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetCheckPoints()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<JobsiteHazard>> JobsiteList(ApiTemplate template)
        {
            throw new NotImplementedException();
        }

        public Task<string> SupportByCountry(ApiTemplate template)
        {
            throw new NotImplementedException();
        }
    }
}
