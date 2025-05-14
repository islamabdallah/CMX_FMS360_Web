using FleetM360_PLL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Contracts
{
    public interface IJobSiteService
    {
        List<JobSiteModel> GetAllJobsites();
        Task<bool> CreateJobsite(JobSiteModel model);
        Task<bool> UpdateJobsite(JobSiteModel model);
        bool DeleteJobsite(long id);
        JobSiteModel GetJobsite(long id);
        List<JobSiteModel> GetAllJobsitesByNetworkCoverage(bool hasNetwork);
        public List<JobSiteModel> GetAllActiveJobsites();
    }
}
