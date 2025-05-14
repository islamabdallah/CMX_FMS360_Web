using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FleetM360_Web.Controllers
{
    public class JobSiteController : Controller
    {
        private readonly IJobSiteService _jobsiteService;
        public JobSiteController(IJobSiteService jobsiteService)
        {
            _jobsiteService = jobsiteService;
        }

        public bool CheckJobsiteNumberIdentity(long jobsiteId)
        {
            JobSiteModel jobSiteModel = _jobsiteService.GetJobsite(jobsiteId);
            if (jobSiteModel != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public IActionResult Index()
        {
            var jobSites = _jobsiteService.GetAllJobsites().ToList();
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View(jobSites);
        }
        // GET: JobSiteController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }


        public string GetDetails(long id)
        {
            try
            {
                JobSiteModel model = _jobsiteService.GetJobsite(id);

                var x = JsonSerializer.Serialize(model);
                return x;
            }
            catch (Exception e)
            {
                return "Can not load details";
            }
        }

        public string CheckIfThisJobsiteAssignedToThisTripBeforeOrNot(long tripNumber, long jobsiteNumber)
        {
            //try
            //{
            //    TripJobsiteModel tripJobsiteModel = _tripJobsiteService.GetTripJobsiteModelByTripNumberAndJobsiteIdForAPI(tripNumber, jobsiteNumber);
            //    if (tripJobsiteModel == null)
            //    {
            //        JobSiteModel model = _jobsiteService.GetJobsite(jobsiteNumber);

            //        var x = JsonSerializer.Serialize(model);
            //        return x;
            //    }
            //    else
            //    {
            //        return JsonSerializer.Serialize("Failed Process, you can not Assign to this jobsite, it is assigned before");
            //    }

            //}
            //catch (Exception e)
            //{
            //    return JsonSerializer.Serialize("Failed Process, Can not load details");
            //}
            return null;
        }
        // GET: JobSiteController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: JobSiteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JobSiteModel model)
        {
            try
            {
                bool result = _jobsiteService.CreateJobsite(model).Result;
                if (result == true)
                {
                    TempData["Message"] = "Jobsite is created successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Failed Process, can not create jobsite";
                    return View(model);
                }
            }
            catch
            {
                return RedirectToAction("ERROR404");
            }

        }

        // GET: JobSiteController/Edit/5
        public ActionResult Edit(int id)
        {
            JobSiteModel jobSiteModel = _jobsiteService.GetJobsite(id);
            return View(jobSiteModel);
        }

        // POST: JobSiteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(JobSiteModel jobSiteModel)
        {
            try
            {
                jobSiteModel.UpdatedDate = DateTime.Now;
                jobSiteModel.IsDelted = false;
                bool result = _jobsiteService.UpdateJobsite(jobSiteModel).Result;
                if (result == true)
                {
                    TempData["Message"] = "Jobsite is updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Failed Process, can not update jobsite";
                    return View(jobSiteModel);
                }
            }
            catch
            {
                return RedirectToAction("ERROR404");
            }
        }

        // GET: JobSiteController/Delete/5
        public bool Delete(long jobsiteId)
        {
            try
            {
                JobSiteModel jobSiteModel = _jobsiteService.GetJobsite(jobsiteId);
                jobSiteModel.IsVisible = false;
                jobSiteModel.IsDelted = true;
                jobSiteModel.UpdatedDate = DateTime.Now;
                bool result = _jobsiteService.UpdateJobsite(jobSiteModel).Result;
                return result;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public ActionResult Search(JobSiteModel jobSiteModel)
        {
            _jobsiteService.GetAllJobsitesByNetworkCoverage(true);

            return null;
        }
    }
}
