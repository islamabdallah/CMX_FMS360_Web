using FleetM360_DAL.Models.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Contracts
{
    public interface ITruckSiloService
    {
        //List<JobSiteModel> GetAllJobsites();
        //Task<bool> CreateJobsite(JobSiteModel model);
        //Task<bool> UpdateJobsite(JobSiteModel model);
        //bool DeleteJobsite(long id);
        TruckSilo GetLastActiveTruckSilo(string truckNumber);
        //List<JobSiteModel> GetAllJobsitesByNetworkCoverage(bool hasNetwork);

    }
}
