using AutoMapper;
using FleetM360_DAL.Data.Repository;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Implementation
{
    public class JobSiteService : IJobSiteService
    {
        private readonly IRepository<JobSite, long> _repository;
        private readonly ILogger<JobSiteService> _logger;
        private readonly IMapper _mapper;

        public JobSiteService(IRepository<JobSite, long> repository,
          ILogger<JobSiteService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        public Task<bool> CreateJobsite(JobSiteModel model)
        {
            model.IsVisible = true;
            model.CreatedDate = DateTime.Now;
            model.UpdatedDate = DateTime.Now;
            var jobSite = _mapper.Map<JobSite>(model);
            try
            {
                var result = _repository.Add(jobSite);

                if (result != null)
                {
                    return Task<bool>.FromResult<bool>(true);
                }
                else
                {
                    return Task<bool>.FromResult<bool>(false);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return Task<bool>.FromResult<bool>(false);
        }

        public bool DeleteJobsite(long id)
        {
            try
            {
                var jobsite = _repository.Find(t => t.Id == id).FirstOrDefault();
                if (jobsite != null)
                {
                    jobsite.IsDelted = true;
                    jobsite.IsVisible = false;
                    bool result = _repository.Update(jobsite);
                    return result;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return false;
        }

        public List<JobSiteModel> GetAllJobsites()
        {
            try
            {
                var jobSites = _repository.Findlist().Result;
                List<JobSiteModel> jobSiteModels = _mapper.Map<List<JobSiteModel>>(jobSites).ToList();
                return jobSiteModels;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<JobSiteModel> GetAllActiveJobsites()
        {
            try
            {
                var jobSites = _repository.Find(j => j.IsVisible == true).ToList();
                List<JobSiteModel> jobSiteModels = _mapper.Map<List<JobSiteModel>>(jobSites).ToList();
                return jobSiteModels;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public List<JobSiteModel> GetAllJobsitesByNetworkCoverage(bool hasNetwork)
        {
            try
            {
                var jobSites = _repository.Find(j => j.IsVisible == true && j.HasNetworkCoverage == hasNetwork);
                List<JobSiteModel> jobSiteModels = _mapper.Map<List<JobSiteModel>>(jobSites).ToList();
                return jobSiteModels;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public JobSiteModel GetJobsite(long id)
        {
            try
            {
                JobSite jobsite = _repository.Find(t => t.IsVisible == true && t.Id == id).First();
                JobSiteModel jobSiteModel = _mapper.Map<JobSiteModel>(jobsite);
                return jobSiteModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
        }

        public async Task<bool> UpdateJobsite(JobSiteModel model)
        {
            try
            {
                JobSite jobSite = _mapper.Map<JobSite>(model);
                bool updateResult = _repository.Update(jobSite);
                return updateResult;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
