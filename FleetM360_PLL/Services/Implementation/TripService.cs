using AutoMapper;
using FleetM360_DAL.Data.Repository;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetM360_PLL.APIViewModels.Trip;
using FleetM360_PLL.APIViewModels.Drivers;
using FleetM360_DAL.Repository.EntityFramework;
using FleetM360_PLL.APIViewModels.Trucks;
using FleetM360_DAL.Migrations.ApplicationDB;
using static FleetM360_PLL.CommanData;
using System.Diagnostics;

namespace FleetM360_PLL.Services.Implementation
{
    public class TripService : ITripService
    {
        private readonly IRepository<Trip, long> _repository;
        private readonly IRepository<Truck, long> _truckRepository;
        private readonly IRepository<TripDriver, long> _tripDriverRepository;
        private readonly IRepository<Driver, long> _driverRepository;
        private readonly ILogger<TripService> _logger;
        private readonly IMapper _mapper;
        private readonly ITripDriverService _driver;
        private readonly IPlannedTripLocationService _plannedTripLocationService;
        private readonly ITripDriverService _tripDriverService;
        public ApplicationDBContext _context;

        public TripService(IRepository<Trip, long> repository, IRepository<Truck, long> truckRepository, IRepository<TripDriver, long> tripDriverRepository,
        IRepository<Driver, long> driverRepository, ILogger<TripService> logger, IMapper mapper, ITripDriverService driver,
        IPlannedTripLocationService plannedTripLocationService, ITripDriverService tripDriverService, ApplicationDBContext context)
        {
            _repository = repository;
            _truckRepository = truckRepository;
            _tripDriverRepository = tripDriverRepository;
            _driverRepository = driverRepository;
            _logger = logger;
            _mapper = mapper;
            _driver = driver;
            _plannedTripLocationService = plannedTripLocationService;
            _tripDriverService = tripDriverService;
            _context = context;
        }
        public async Task<bool> CreateTrip(TripModel model)
        {
            try
            {
                //var lasttrip = _repository.Find(e => e.IsVisible == true).Last();
                var lasttrip=await _context.Trips.Where(t=>t.IsVisible==true).OrderBy(t=>t.ParentTrip).LastOrDefaultAsync();
                if (lasttrip != null)
                {
                    model.ParentTrip = lasttrip.ParentTrip + 1;
                }
                else
                {
                    model.ParentTrip = 1;
                }
                model.StatusId = 1;
                model.IsCanceled = false;
                model.IsConverted = false;
                model.IsDelted = false;
                model.IsVisible = true;
                model.CreatedDate = DateTime.Now;
                model.UpdatedDate = DateTime.Now;
                Trip trip = new Trip()
                {
                      ParentTrip =model.ParentTrip,
        TripNumber=model.TripNumber,
        TruckNumber =model.TruckNumber,
         SiloNumber =model.SiloNumber,
         TypeId =model.TypeId,
         SubTypeId =model.SubTypeId,
         Date =model.Date,
       StatusId =model.StatusId,
        StageEn=model.StageEn,
        StageAR =model.StageAR,
         IsCanceled =model.IsCanceled,
        IsConverted =model.IsConverted,
        MustStart =model.MustStart,
         FromPlant=model.FromPlant,
        Qty =model.Qty,
        AssignQty =model.Qty,
        ArrivedDate=model.ArrivedDate,
        departureDate =model.departureDate,
        IsDelted=false,
        IsVisible=true,
        CreatedDate = DateTime.Now,
        UpdatedDate = DateTime.Now,
    };
               // var trip = _mapper.Map<Trip>(model);
                Trip result = _repository.Add(trip);
                if (result != null)
                {
                    model.TripNumber = result.TripNumber;
                    model.ParentTrip = result.ParentTrip;
                    if (model.Sourse != null)
                    {
                        if (model.Sourse.Count > 0)
                        {
                            foreach (var sourse in model.Sourse)
                            {
                                PlannedTripLocation plannedTripLocation = new PlannedTripLocation();
                                plannedTripLocation.CreatedDate = DateTime.Now;
                                plannedTripLocation.UpdatedDate = DateTime.Now;
                                plannedTripLocation.IsDelted = false;
                                plannedTripLocation.IsVisible = true;
                                plannedTripLocation.TruckNumber = model.TruckNumber;
                                plannedTripLocation.SiloNumber = model.SiloNumber;
                                plannedTripLocation.ParentTrip = model.ParentTrip;
                                plannedTripLocation.TripNumber = model.TripNumber;
                                plannedTripLocation.Location = sourse.Name;
                                plannedTripLocation.Lat = sourse.Latitude;
                                plannedTripLocation.Long = sourse.Longitude;
                                plannedTripLocation.Type = "Source";
                                plannedTripLocation.Qty = sourse.Qty;
                                plannedTripLocation.Material = sourse.Material;
                                plannedTripLocation.arrivalFlag = false;
                                plannedTripLocation.locationStatus = trip.FromPlant == true || trip.SubTypeId == 1 ? false : true;
                                var addedLocationResult = _plannedTripLocationService.CreateTripLocation(plannedTripLocation).Result;
                            }
                        }
                    }
                    if (model.Distination != null)
                    {
                        if (model.Distination.Count > 0)
                        {
                            foreach (var sourse in model.Distination)
                            {
                                PlannedTripLocation plannedTripLocation = new PlannedTripLocation();
                                plannedTripLocation.CreatedDate = DateTime.Now;
                                plannedTripLocation.UpdatedDate = DateTime.Now;
                                plannedTripLocation.IsDelted = false;
                                plannedTripLocation.IsVisible = true;
                                plannedTripLocation.TruckNumber = model.TruckNumber;
                                plannedTripLocation.SiloNumber = model.SiloNumber;
                                plannedTripLocation.ParentTrip = model.ParentTrip;
                                plannedTripLocation.TripNumber = model.TripNumber;
                                plannedTripLocation.Location = sourse.Name;
                                plannedTripLocation.Lat = sourse.Latitude;
                                plannedTripLocation.Long = sourse.Longitude;
                                plannedTripLocation.Type = "Dest";
                                plannedTripLocation.Qty = sourse.Qty;
                                plannedTripLocation.Material = sourse.Material;
                                plannedTripLocation.locationStatus = true;
                                plannedTripLocation.arrivalFlag = false;
                               // var addedLocationResult = _plannedTripLocationService.CreateTripLocation(plannedTripLocation).Result;
                               _context.PlannedTripLocations.Add(plannedTripLocation);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    if (model.onRoadDrivers != null)
                    {
                        if (model.onRoadDrivers.Count > 0)
                        {
                            foreach (var driver in model.onRoadDrivers)
                            {
                                TripDriverModel tripDriver = new TripDriverModel();
                                tripDriver.CreatedDate = DateTime.Now;
                                tripDriver.UpdatedDate = DateTime.Now;
                                tripDriver.IsDelted = false;
                                tripDriver.IsVisible = true;
                                tripDriver.TruckNumber = model.TruckNumber;
                                tripDriver.SiloNumber = model.SiloNumber;
                                tripDriver.ParentTrip = model.ParentTrip;
                                tripDriver.TripNumber = model.TripNumber;
                                tripDriver.DriverId = driver;
                                tripDriver.Role = "OnRoad";

                                var addedDriverResult = _tripDriverService.CreateTripDriver(tripDriver).Result;
                            }
                        }
                    }
                    if (model.loadDrivers != null)
                    {
                        if (model.loadDrivers.Count > 0)
                        {
                            foreach (var driver in model.loadDrivers)
                            {
                                TripDriverModel tripDriver = new TripDriverModel();
                                tripDriver.CreatedDate = DateTime.Now;
                                tripDriver.UpdatedDate = DateTime.Now;
                                tripDriver.IsDelted = false;
                                tripDriver.IsVisible = true;
                                tripDriver.TruckNumber = model.TruckNumber;
                                tripDriver.SiloNumber = model.SiloNumber;
                                tripDriver.ParentTrip = model.ParentTrip;
                                tripDriver.TripNumber = model.TripNumber;
                                tripDriver.DriverId = driver;
                                tripDriver.Role = "Loading";

                                var addedDriverResult = _tripDriverService.CreateTripDriver(tripDriver).Result;
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public Task<bool> DeleteTrip(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<Trip> GetActiveParentTripOfTruck(string truckNumber)
        {
            try
            {
                var trips = await _repository.Find(e => e.IsVisible == true && e.StatusId != 3 && e.TruckNumber == truckNumber).ToListAsync();
                if (trips != null && trips.Count > 0)
                {
                    var result = trips.OrderBy(e => e.departureDate).GroupBy(e => e.ParentTrip)
                           .Select(g => new TripGroupViewModel
                           {
                               ParentTrip = g.Key,
                               DepartureDate = _repository.Find(e => e.IsVisible == true && e.ParentTrip == g.Key).FirstOrDefaultAsync().Result.departureDate,
                               Trips = g.ToList()
                           })
                           .FirstOrDefault();
                   
                    if (result != null)
                    {
                        if(result.Trips.Count > 0)
                        {
                            return result.Trips[0];
                        }
                    }
                    return null;
                    
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<IGrouping<long, Trip>>> GetAllpendingTripGroupedByParentTrip()
        {
            var trips = await _repository.Find(e => e.IsVisible == true && e.StatusId == 1).ToListAsync();

            return trips.GroupBy(e => e.ParentTrip).ToList();
        }

        public async Task<List<TripGroupViewModel>> GetAllPendingTripofParentTrip()
        {
            var trips = await _repository.Find(e => e.IsVisible == true && e.StatusId == 1).ToListAsync();

            return trips.OrderBy(e=>e.departureDate).GroupBy(e => e.ParentTrip)
                        .Select(g => new TripGroupViewModel
                        {
                            ParentTrip = g.Key,
                            DepartureDate= _repository.Find(e => e.IsVisible == true && e.StatusId == 1 && e.ParentTrip==g.Key).FirstOrDefaultAsync().Result.departureDate,
                            Trips = g.ToList()
                        })
                        .ToList();
        }

        public async Task<List<TripApiModel>> GetAllPendingTripofTruckforMobile(string truckId, int languageId)
        {
            List<TripApiModel> tripApiModels = new List<TripApiModel>();
            var truck=await _truckRepository.Find(e=>e.IsVisible==true && e.Id == Convert.ToInt64(truckId)).FirstOrDefaultAsync();
            if (truck != null)
            {
                var trips = await _repository.Find(e => e.IsVisible == true && e.StatusId != 3 && e.TruckNumber==truck.TruckNumber).ToListAsync();
                if (trips != null && trips.Count>0)
                {

                    var result = trips.OrderBy(e => e.departureDate).GroupBy(e => e.ParentTrip)
                           .Select(g => new TripGroupViewModel
                           {
                               ParentTrip = g.Key,
                               DepartureDate = _repository.Find(e => e.IsVisible == true && e.ParentTrip == g.Key).FirstOrDefaultAsync().Result.departureDate,
                               Trips = g.ToList()
                           })
                           .ToList();
                    TripApiModel tripApiModel = new TripApiModel();
                    tripApiModel.truckNumber = truck.TruckNumber;
                    tripApiModel.truckId = truckId;
                    tripApiModel.tripId = result[0].ParentTrip.ToString();
                    var loadingModel = _tripDriverRepository.Find(e => e.ParentTrip == result[0].ParentTrip && e.Role=="Loading").FirstOrDefaultAsync().Result;
                    var onRoadModel = _tripDriverRepository.Find(e => e.ParentTrip == result[0].ParentTrip && e.Role == "OnRoad").FirstOrDefaultAsync().Result;
                    if (loadingModel != null)
                    {
                        tripApiModel.loadingDriver = _driverRepository.Find(e => e.DriverNumber == loadingModel.DriverId).FirstOrDefaultAsync().Result.FullName;
                    }
                    if (onRoadModel != null)
                    {
                        tripApiModel.roadDriver = _driverRepository.Find(e => e.DriverNumber == onRoadModel.DriverId).FirstOrDefaultAsync().Result.FullName;
                    }
                    tripApiModel.subTrips = new List<SubTripApiModel>();
                    bool hasCementTrip = false;
                    bool hasStart=false;
                    bool allDone = true;
                    if (result[0].Trips.Count() > 0) 
                    {

                        //Check for cement pending trip
                        foreach (var tr in result[0].Trips)
                        {
                            if (tr.StatusId != 3 && tr.StageEn != "Completed" && tr.SubTypeId == 1)
                            {
                                hasCementTrip = true;
                                break;
                            }
                        }

                        foreach (var trip in result[0].Trips)
                        {
                            SubTripApiModel subTrip = new SubTripApiModel();
                            if (trip.StatusId != 3 && trip.StageEn != "Completed" && trip.SubTypeId == 1 && hasStart == false)
                            {
                                subTrip.start = 1;
                                hasStart = true;
                                allDone = false;

                            }
                            else if (trip.StatusId != 3 && trip.StageEn != "Completed" && hasStart == false && hasCementTrip == false) 
                            {
                                subTrip.start = 1;
                                hasStart = true;
                                allDone = false;

                            }
                            else if (trip.StageEn == "Completed")
                            {
                                subTrip.start = 2;
                            }
                            else
                            {
                                subTrip.start = 0;
                            }
                            subTrip.material = trip.SubTypeId == 1 || trip.SubTypeId == 2 ? ((GeneralMaterial)trip.SubTypeId).ToString() : "Mission";//: "Backhualing";
                            subTrip.quantity = trip.Qty;
                            subTrip.status = languageId == 1 ? trip.StageEn.Trim() : trip.StageAR.Trim();
                            subTrip.tripId = trip.Id.ToString();
                            subTrip.truckNumber = truck.TruckNumber;
                            subTrip.truckId = truckId;
                            subTrip.fromDate = trip.departureDate.ToString("dd/MM/yyyy");
                            subTrip.toDate = trip.ArrivedDate.ToString("dd/MM/yyyy");
                            subTrip.tripType = (int)trip.SubTypeId;

                            subTrip.fromLocations = new List<LocationApiModel>();
                            var frmLocations = await _context.PlannedTripLocations.Where(a =>a.IsVisible==true && a.ParentTrip == trip.ParentTrip && a.TripNumber == trip.TripNumber && a.Type == "Source").ToListAsync();
                            if (frmLocations.Count > 0)
                            {
                                var distinctCities = frmLocations
                                .Where(location => location.Location != null) // التأكد من أن الموقع غير فارغ
                                .Select(location => location.Location)
                                .Distinct()
                                .OrderBy(city => city) // ترتيب المدن أبجديًا
                                .ToList();

                                subTrip.fromAddress = string.Join(", ", distinctCities);

                                //subTrip.fromAddress = frmLocations[0].Location;
                                // subTrip.toAddress = "";
                            }
                            subTrip.toLocations = new List<LocationApiModel>();
                            var tLocations = await _context.PlannedTripLocations.Where(a =>a.IsVisible==true && a.ParentTrip == trip.ParentTrip && a.TripNumber == trip.TripNumber && a.Type == "Dest").ToListAsync();
                            if (tLocations.Count > 0)
                            {
                                var Cities = tLocations
                               .Where(location => location.Location != null) // التأكد من أن الموقع غير فارغ
                               .Select(location => location.Location)
                               .Distinct()
                               .OrderBy(city => city) // ترتيب المدن أبجديًا
                               .ToList();

                                subTrip.toAddress = string.Join(", ", Cities);
                                //subTrip.toAddress = tLocations[0].Location;
                               // subTrip.toAddress = "";
                            }
                            tripApiModel.subTrips.Add(subTrip);
                        }
                    }
                    tripApiModels.Add(tripApiModel);
                   
                }

                return tripApiModels;
            }
            return new List<TripApiModel>();
        }

        public List<TripModel> GetAllTrip()
        {
            throw new NotImplementedException();
        }

        public async Task<StartTripApiModel> GetHealthPrecheck(UserApiModel model)
        {
            try
            {
               
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.tripId)).FirstOrDefault();
                if (trip != null)
                {
                    StartTripApiModel startTripApiModel = new StartTripApiModel
                    {
                        screen = "SplashWidget",//==map
                        preCheckQuestions = new List<PrecheckQuestionApiModel>()
                    };
                    if (trip.IsConverted == true && trip.ConvertedSeen == false)
                    {
                        startTripApiModel.isConverted = true;
                    }
                    else
                    {
                        startTripApiModel.isConverted = false;
                    }
                    var test =await _context.TripPrechecks.Where(t => t.ParentTrip == trip.ParentTrip && t.DriverId == model.UserNumber.ToString() && t.Category.Trim() == "الحالة الصحية".Trim() && t.Status== "Success").FirstOrDefaultAsync();
                    var loadedd =await _context.TripLogs.Where(t => t.ParentTrip == trip.ParentTrip && t.IsVisible == true && t.Event == "EndGrossWeight").FirstOrDefaultAsync() ;
                    var leave = await _context.TripLogs.Where(t => t.ParentTrip == trip.ParentTrip && t.IsVisible == true && t.Event == "LeavPlant").FirstOrDefaultAsync();
                    var roadModel =await _tripDriverRepository.Find(e => e.ParentTrip == trip.ParentTrip && e.Role == "OnRoad" && e.DriverId == model.UserNumber).FirstOrDefaultAsync();
                    if (( loadedd !=null || leave != null) && roadModel == null)
                    {
                        startTripApiModel.screen = "TripsWidget";
                        return startTripApiModel;
                    }
                    if (test == null)
                    {
                        startTripApiModel.medicalStatus=false;
                        startTripApiModel.preCheckQuestions = await _context.PreCheckQuestions
                       .Where(a => a.MainCategory.Trim() == "الحالة الصحية".Trim())
                       .Include(a => a.PreCheckAnswers) // Optional, not needed when projecting with Select
                       .Select(a => new PrecheckQuestionApiModel
                       {
                           questionId = a.PreCheckQuestionId,
                           questionName = a.QuestionName,
                           questionCategory = a.Category,
                           questionNAnswers = a.PreCheckAnswers.Select(b => new PrecheckAnswerApiModel
                           {
                               answerName = model.languageId == 1 ? b.AnswerNameEN : b.AnswerNameAR,
                               answerValue = b.AnswerValue

                           }).ToList()
                       })
                       .ToListAsync();
                        startTripApiModel.screen = "HealthPrecheck";
                        return startTripApiModel;
                    }
                    else
                    {
                        startTripApiModel.medicalStatus = true;    
                    }

                    var toolcheckload = await _context.TripPrechecks.Where(t => t.ParentTrip == trip.ParentTrip && t.Category.Trim() == "معدات للتحميل".Trim()).FirstOrDefaultAsync();
                    if (toolcheckload == null && loadedd == null && trip.SubTypeId == 1 && roadModel ==null)
                    {
                        startTripApiModel.screen = "PreCheckToolsScreen";
                        return startTripApiModel;
                    }
                    var toolcheck =await _context.TripPrechecks.Where(t => t.ParentTrip == trip.ParentTrip && t.Category.Trim() == "معدات".Trim()).FirstOrDefaultAsync();
                    if (toolcheck == null && roadModel!=null && trip.SubTypeId != 1)
                    {
                        startTripApiModel.screen = "PreCheckToolsScreen";
                        return startTripApiModel;
                    }
                    var precheck = _context.TripPrechecks.Where(t => t.ParentTrip == trip.ParentTrip && t.Category.Trim() == "فحص".Trim()).FirstOrDefaultAsync();
                    if (precheck.Result == null && trip.SubTypeId == 1)
                    {
                        startTripApiModel.screen = "PreCheckScreen";
                        return startTripApiModel;
                    }


                    var lastLog = _context.TripLogs.Where(t => t.ParentTrip == trip.ParentTrip && t.IsVisible ==true).OrderBy(t=>t.Id).LastOrDefault();
                    if (lastLog != null)
                    {
                        if ((lastLog.Event == "Maintainance" || lastLog.Event == "StartMaintainance") && leave==null)
                        {
                            startTripApiModel.screen = "CarInspectionScreen";
                            return startTripApiModel;
                        }
                        else if (lastLog.Event == "EndMaintainance" && leave == null)
                        {
                            startTripApiModel.screen = "PreCheckScreen";
                            return startTripApiModel;
                        }
                        else if (lastLog.Event == "PreCheck")
                        {
                            var precheckType = _context.TripPrechecks.Where(t => t.TripId == Convert.ToInt64(model.tripId) && t.DriverId == model.UserNumber.ToString()).OrderBy(t=>t.Id).LastOrDefault();
                            if (precheckType != null)
                            {
                                if (precheckType.Category.Trim() == "معدات".Trim() && precheckType.Status == "Success")
                                {
                                    startTripApiModel.screen = "PreCheckScreen";
                                    return startTripApiModel;
                                }
                                if (precheckType.Category.Trim() == "فحص".Trim() && precheckType.Status == "Success" &&loadedd==null && trip.SubTypeId==1)
                                {
                                    startTripApiModel.screen = "WeightDetailsPage";
                                    return startTripApiModel;
                                }
                                else if (precheckType.Category.Trim() == "فحص".Trim() && precheckType.Status == "Success" && (loadedd != null || leave ==null) )
                                {
                                    startTripApiModel.screen = "WaitingPlantScreen";
                                    return startTripApiModel;
                                }
                            }
                           
                        }
                        else if (lastLog.Event == "EndGrossWeight")//EndGrossWeight
                        {
                            startTripApiModel.screen = "PreCheckScreen";
                            return startTripApiModel;
                        }
                        else if (lastLog.Event == "EmptyWeight" || lastLog.Event == "GrossWeight")
                        {
                            startTripApiModel.screen = "WeightDetailsPage";
                            return startTripApiModel;
                        }
                    }

                    //var loaded = _context.TripLogs.Where(t => t.ParentTrip == trip.ParentTrip && t.IsVisible == true && t.Event== "GrossWeight").FirstOrDefault();
                    //if (loaded != null)
                    //{
                    //    startTripApiModel.screen = "WaitingPlantScreen";
                    //    return startTripApiModel;
                    //}

                   
                    //if (loadedd == null)
                    //{
                    //    startTripApiModel.screen = "WeightDetailsPage";
                    //    return startTripApiModel;
                    //}



                    return startTripApiModel;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<PrecheckQuestionApiModel>> GetPrecheckListForCheck(UserApiModel model)
        {
            try
            {
                List<PrecheckQuestionApiModel> preCheckQuestions = new List<PrecheckQuestionApiModel>();
                //var questions = await _context.PreCheckQuestions
                //.Where(a => a.MainCategory.Trim() == "فحص".Trim())
                //.Include(a => a.PreCheckAnswers)
                //.ToListAsync();

                preCheckQuestions = await _context.PreCheckQuestions
                .Where(a => a.MainCategory.Trim() == "فحص".Trim())
                .Include(a => a.PreCheckAnswers) // Optional, not needed when projecting with Select
                .Select(a => new PrecheckQuestionApiModel
                {
                    questionId = a.PreCheckQuestionId,
                    questionName = a.QuestionName,
                    questionCategory = a.Category,
                    questionNAnswers = a.PreCheckAnswers.Select(b => new PrecheckAnswerApiModel
                    {
                        answerName = model.languageId == 1 ? b.AnswerNameEN : b.AnswerNameAR,
                        answerValue = b.AnswerValue

                    }).ToList()
                })
                .ToListAsync();

                return preCheckQuestions;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Take5APIDataModel> GetTake5DataForMobile(UserApiModel loginModel)
        {
            try
            {

                Take5APIDataModel take5APIData = new Take5APIDataModel();

                take5APIData.stepOne = await _context.Questions
                .Where(a => a.IsVisible == true && a.Step == 1)
                   .Include(a => a.QuestionAnswers)
                    .Select(a => new QuestionModel
                    {
                        questionId = a.Id,
                        questionName = a.Text,
                        questionNAnswers = a.QuestionAnswers.Select(b => new PrecheckAnswerApiModel
                        {
                            answerName = loginModel.languageId == 1 ? b.AnswerNameEN : b.AnswerNameAR,
                            answerValue = b.AnswerValue

                        }).ToList()

                    }).ToListAsync();


                take5APIData.stepTwo = await _context.Questions
               .Where(a => a.IsVisible == true && a.Step == 2)
                .Include(a => a.QuestionAnswers)
               .Select(a => new QuestionModel
                   {
                       questionId = a.Id,
                       questionName = a.Text,
                   questionNAnswers = a.QuestionAnswers.Select(b => new PrecheckAnswerApiModel
                   {
                       answerName = loginModel.languageId == 1 ? b.AnswerNameEN : b.AnswerNameAR,
                       answerValue = b.AnswerValue

                   }).ToList()
               }).ToListAsync();

                var dangers = await _context.Dangers
                .Where(a => a.IsVisible == true)
                .Include(a => a.DangerCategory)
                .ToListAsync();
                var onSiteRisks = new List<OnSiteRiskModel>();

                foreach (var danger in dangers)
                {
                    var waysToDealWith = await _context.MeasureControls
                        .Where(b => b.IsVisible == true && b.DangerId == danger.Id)
                        .Select(b => b.Name)
                        .ToListAsync();

                    onSiteRisks.Add(new OnSiteRiskModel
                    {
                        id = danger.Id,
                        name = danger.Name,
                        category = danger.DangerCategory?.Name,
                        waysToDealWithOnSiteRisk = waysToDealWith
                    });
                }

                take5APIData.onSiteRisks = onSiteRisks;

                LocationQtyDataModel locationQtyData = new LocationQtyDataModel();

                var tripLocation = await _context.PlannedTripLocations.Where(a => a.Id == loginModel.tripLocationId).FirstOrDefaultAsync();
                if (tripLocation != null)
                {
                    locationQtyData.totalQty = tripLocation.Qty;
                    var lastReceive = await _context.ActualTripLocations.Where(_a => _a.PlannedTripLocationId == loginModel.tripLocationId).OrderBy(_a => _a.Id).LastOrDefaultAsync();
                    locationQtyData.remainingQty = lastReceive != null ? lastReceive.Remain : tripLocation.Qty;

                    locationQtyData.receivedQuantities = await _context.ActualTripLocations.Where(b => b.PlannedTripLocationId == loginModel.tripLocationId)
                         .Select(a => new ReceivedQuantityModel
                         {
                             qty = a.Received,
                             receivedDateTime = a.CreatedDate,
                         })
                        .ToListAsync();


                    take5APIData.locationQtyData = locationQtyData;
                    var trip=await _context.Trips.Where(t=>t.Id==Convert.ToInt64(loginModel.tripId) && t.IsVisible==true).FirstOrDefaultAsync();
                    if(trip != null && tripLocation.Type=="Dest")
                    {
                        trip.StageAR = "جاري التسليم";
                        trip.UpdatedDate = DateTime.Now;
                        trip.StageEn = "Under Deliver";
                        _context.Trips.Update(trip);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        trip.StageAR = "قيد التحميل";
                        trip.UpdatedDate = DateTime.Now;
                        trip.StageEn = "Loading";
                        _context.Trips.Update(trip);
                        await _context.SaveChangesAsync();
                    }
                    //    var Eventt = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "ArriveSite").FirstOrDefault();
                    //if (Eventt != null)
                    //{
                    //    TripLog tripLog = new TripLog()
                    //    {
                    //        ParentTrip = trip.ParentTrip,
                    //        TripNumber = trip.TripNumber,
                    //        Event = Eventt.LogName,
                    //        LogId = Eventt.Id,
                    //        Lat = loginModel.lat,
                    //        Long = loginModel.lng,
                    //        CreatedBy = loginModel.UserNumber.ToString(),
                    //        Date = DateTime.Now.ToString(),
                    //        CreatedDate = DateTime.Now,
                    //        UpdatedDate = DateTime.Now,
                    //        IsDelted = false,
                    //        IsVisible = true
                    //    };
                    //    _context.TripLogs.Add(tripLog);
                    //    await _context.SaveChangesAsync();
                    //}
                        return take5APIData;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<PrecheckQuestionApiModel>> GetToolsPrecheck(UserApiModel model)
        {
            try
            {
                List<PrecheckQuestionApiModel>preCheckQuestions = new List<PrecheckQuestionApiModel>();
                var trip =await _context.Trips.Where(a => a.Id == Convert.ToInt64(model.tripId)).FirstOrDefaultAsync();
                if (trip != null)
                {
                    var roadDriver = await _context.TripDrivers.Where(t => t.ParentTrip == trip.ParentTrip && t.TripNumber == trip.TripNumber && t.DriverId == model.UserNumber && t.Role== "OnRoad").FirstOrDefaultAsync();
                    var loadedd = await _context.TripLogs.Where(t => t.ParentTrip == trip.ParentTrip && t.IsVisible == true && t.Event == "EndGrossWeight").FirstOrDefaultAsync();
                    if (loadedd != null || trip.SubTypeId !=1 || roadDriver !=null)
                    {
                        preCheckQuestions = await _context.PreCheckQuestions
                       .Where(a => a.MainCategory.Trim() == "معدات".Trim())
                       .Include(a => a.PreCheckAnswers) // Optional, not needed when projecting with Select
                       .Select(a => new PrecheckQuestionApiModel
                       {
                           questionId = a.PreCheckQuestionId,
                           questionName = a.QuestionName,
                           questionCategory = a.Category,
                           questionNAnswers = a.PreCheckAnswers.Select(b => new PrecheckAnswerApiModel
                           {
                               answerName = model.languageId == 1 ? b.AnswerNameEN : b.AnswerNameAR,
                               answerValue = b.AnswerValue

                           }).ToList()
                       })
                       .ToListAsync();

                    }
                    else
                    {
                        preCheckQuestions = await _context.PreCheckQuestions
                       .Where(a => a.MainCategory.Trim() == "معدات للتحميل".Trim())
                       .Include(a => a.PreCheckAnswers) // Optional, not needed when projecting with Select
                       .Select(a => new PrecheckQuestionApiModel
                       {
                           questionId = a.PreCheckQuestionId,
                           questionName = a.QuestionName,
                           questionCategory = a.Category,
                           questionNAnswers = a.PreCheckAnswers.Select(b => new PrecheckAnswerApiModel
                           {
                               answerName = model.languageId == 1 ? b.AnswerNameEN : b.AnswerNameAR,
                               answerValue = b.AnswerValue

                           }).ToList()
                       })
                       .ToListAsync();
                    }
                }

               

                return preCheckQuestions;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<TripModel> GetTrip(long id)
        {
            var result = await _repository.Find(e => e.IsVisible == true && e.Id == id).FirstOrDefaultAsync();
            var trip = _mapper.Map<TripModel>(result);
            return trip;
            // throw new NotImplementedException();
        }

        public async Task<SubTripApiModel> GetTripDetailsForMobile(long id, int languageId)
        {
            SubTripApiModel tripApiModels = new SubTripApiModel();
            var trip = await _repository.Find(e => e.IsVisible == true && e.Id == id ).FirstOrDefaultAsync();
            if (trip != null)
            {
                SubTripApiModel subTrip = new SubTripApiModel();
                subTrip.material = trip.SubTypeId == 1 || trip.SubTypeId == 2 ? ((GeneralMaterial)trip.SubTypeId).ToString() : "Mission";//: "Backhualing";
                subTrip.tripType = (int)trip.SubTypeId;
                subTrip.start = 0;
               // var trips=await _repository.Find(e=>e.IsVisible==true && e.ParentTrip==trip.ParentTrip).ToListAsync(); //it used if i want toknow the start value
               
                subTrip.quantity = trip.Qty;
                subTrip.status =languageId==1? trip.StageEn.Trim() : trip.StageAR.Trim();
                subTrip.tripId = trip.Id.ToString();
                var truck = await _truckRepository.Find(e => e.IsVisible == true && e.TruckNumber == trip.TruckNumber).FirstOrDefaultAsync();
                subTrip.truckNumber = truck != null ? truck.TruckNumber : "";
                subTrip.truckId = truck != null ? truck.Id.ToString() : "";
                subTrip.fromDate = trip.departureDate.ToString("dd-MM-yyyy");
                subTrip.toDate = trip.ArrivedDate.ToString("dd-MM-yyyy");
                if(trip.IsConverted==true && trip.ConvertedSeen == false)
                {
                    subTrip.isConverted = true;
                }
                else
                {
                    subTrip.isConverted = false;
                }
                    subTrip.fromLocations = new List<LocationApiModel>();
                if (trip.SubTypeId != 1 && trip.FromPlant==false)
                {
                    subTrip.fromLocations = await _context.PlannedTripLocations.Where(a =>a.Converted !=true && a.ParentTrip == trip.ParentTrip && a.TripNumber == trip.TripNumber && a.Type == "Source") // Include AuthorId = 4 if needed
                    .Select(a => new LocationApiModel
                    {
                        tripLocationId = (int)a.Id,
                        customerName = a.customerName != null ? a.customerName : "",
                        customerPhoneNumber = a.customerPhoneNumber != null ? a.customerPhoneNumber : "",
                        recipientName = a.recipientName != null ? a.recipientName : "",
                        recipientPhoneNumber = a.recipientPhoneNumber != null ? a.recipientPhoneNumber : "",
                        locationStatus = a.locationStatus != null ? a.locationStatus : false,
                        address = a.Location,
                        materialType = a.Material,
                        lat = a.Lat,
                        lng = a.Long,
                        qty = a.Qty,
                        remainqty = a.Qty,
                        locationType = 1,
                        arrivalFlag = a.arrivalFlag
                    })
                    .ToListAsync();
                    if (subTrip.fromLocations.Count > 0)
                    {
                        subTrip.fromAddress = subTrip.fromLocations[0].address;
                        // subTrip.toAddress = "";
                        foreach (var loc in subTrip.fromLocations)
                        {
                            var remain = await _context.ActualTripLocations.Where(a => a.IsVisible == true && a.PlannedTripLocationId == loc.tripLocationId).OrderBy(t => t.Id).LastOrDefaultAsync();
                            if (remain != null)
                            {
                                loc.remainqty = remain.Remain;
                            }
                        }
                    }
                }
                subTrip.toLocations = new List<LocationApiModel>();
                subTrip.toLocations = await _context.PlannedTripLocations.Where(a =>a.Converted!=true && a.ParentTrip == trip.ParentTrip && a.TripNumber == trip.TripNumber && a.Type == "Dest") // Include AuthorId = 4 if needed
                .Select(a => new LocationApiModel
                {
                    tripLocationId = (int)a.Id,
                    customerName = a.customerName != null ? a.customerName : "",
                    customerPhoneNumber = a.customerPhoneNumber != null ? a.customerPhoneNumber : "",
                    recipientName = a.recipientName != null ? a.recipientName : "",
                    recipientPhoneNumber = a.recipientPhoneNumber != null ? a.recipientPhoneNumber : "",
                    locationStatus = a.locationStatus != null ? a.locationStatus : false,
                    address = a.Location,
                    materialType = a.Material,
                    lat = a.Lat,
                    lng = a.Long,
                    qty = a.Qty,
                    remainqty = a.Qty,
                    locationType = 2,
                    arrivalFlag=a.arrivalFlag
                })
                .ToListAsync();
                if (subTrip.toLocations.Count > 0)
                {
                    subTrip.toAddress = subTrip.toLocations[0].address;
                    // subTrip.toAddress = "";
                    foreach (var loc in subTrip.toLocations)
                    {
                        var remain = await _context.ActualTripLocations.Where(a => a.IsVisible == true && a.PlannedTripLocationId == loc.tripLocationId).OrderBy(t => t.Id).LastOrDefaultAsync();
                        if (remain != null)
                        {
                            loc.remainqty = remain.Remain;
                        }
                    }
                }

                return subTrip;
            }
                return null;
        }

        public async Task<TruckFaultsDataModel> GettruckFaults(UserApiModel model)
        {
            try
            {
                TruckFaultsDataModel truckFaultsDataModel = new TruckFaultsDataModel();
                List<TruckFaultsApiModel> preCheckQuestions = new List<TruckFaultsApiModel>();   
                var failer=await _context.TripPrechecks.Where(t=>t.DriverId==model.UserNumber.ToString() && t.TripId==Convert.ToInt64(model.tripId) && t.Status== "Failed").OrderBy(t=>t.Id).LastOrDefaultAsync();
                if (failer != null)
                { 
                    truckFaultsDataModel.startTime=failer.CreatedDate;
                    var falerlist=_context.TripPrecheckDetails.Where(t=>t.TripPrecheckId==failer.Id).ToListAsync().Result;
                    if (falerlist != null) 
                    {
                        if (falerlist.Count > 0)
                        {
                            foreach (var f in falerlist)
                            {
                                var question=await _context.PreCheckQuestions.Where(t=>t.PreCheckQuestionId==f.QuestionId).FirstOrDefaultAsync();
                                if(question != null)
                                {
                                    TruckFaultsApiModel truckFaultsApiModel = new TruckFaultsApiModel();
                                    truckFaultsApiModel.longDescription="There is aproblem at"+question.Category.ToString();
                                    truckFaultsApiModel.shortDescription=question.Category.ToString();
                                    preCheckQuestions.Add(truckFaultsApiModel);
                                }
                            }
                        }
                    }

                }
                 truckFaultsDataModel.truckFaults = preCheckQuestions.GroupBy(e => e.shortDescription)
                          .Select(g => new TruckFaultsApiModel
                          {
                              shortDescription = g.Key,
                              longDescription ="هناك مشكلة في "+_context.PreCheckQuestions.Where(e => e.IsVisible == true && e.Category == g.Key).FirstOrDefaultAsync().Result.Category
                             
                          })
                          .ToList();
                

                return truckFaultsDataModel;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task<bool> UpdateTrip(TripModel model)
        {
            throw new NotImplementedException();
        }

        //public async Task<List<TripApiModel>> GetAllPendingTripofTruckforMobileLamiaa(string truckId, int languageId)
        //{

        //    //Check Truck Exist 
        //    //Get Pending parent id -> select first based deptaure date 
        //    //Get subtrips 
        //        //in case cement trip not complated based deapture set flag 


        //    //call 
        //    //call
        //    //call


        //    List<TripApiModel> tripApiModels = new List<TripApiModel>();
        //    var truck = await _truckRepository.Find(e => e.IsVisible == true && e.Id == Convert.ToInt64(truckId)).FirstOrDefaultAsync();
        //    if (truck != null)
        //    {
        //        var trips = await _repository.Find(e => e.IsVisible == true && e.StatusId != 3 && e.TruckNumber == truck.TruckNumber).ToListAsync();
        //        if (trips != null && trips.Count > 0)
        //        {
        //            var result = trips.OrderBy(e => e.departureDate).GroupBy(e => e.ParentTrip)
        //                   .Select(g => new TripGroupViewModel
        //                   {
        //                       ParentTrip = g.Key,
        //                       DepartureDate = _repository.Find(e => e.IsVisible == true && e.ParentTrip == g.Key).FirstOrDefaultAsync().Result.departureDate,
        //                       Trips = g.ToList()
        //                   })
        //                   .ToList();
        //            TripApiModel tripApiModel = new TripApiModel();
        //            tripApiModel.truckNumber = truck.TruckNumber;
        //            tripApiModel.truckId = truckId;
        //            tripApiModel.tripId = result[0].ParentTrip.ToString();
        //            var loadingModel = _tripDriverRepository.Find(e => e.ParentTrip == result[0].ParentTrip && e.Role == "Loading").FirstOrDefaultAsync().Result;
        //            var onRoadModel = _tripDriverRepository.Find(e => e.ParentTrip == result[0].ParentTrip && e.Role == "OnRoad").FirstOrDefaultAsync().Result;
        //            if (loadingModel != null)
        //            {
        //                tripApiModel.loadingDriver = _driverRepository.Find(e => e.DriverNumber == loadingModel.DriverId).FirstOrDefaultAsync().Result.FullName;
        //            }
        //            if (onRoadModel != null)
        //            {
        //                tripApiModel.roadDriver = _driverRepository.Find(e => e.DriverNumber == onRoadModel.DriverId).FirstOrDefaultAsync().Result.FullName;
        //            }
        //            tripApiModel.subTrips = new List<SubTripApiModel>();
        //            bool hasCementTrip = false;
        //            bool hasStart = false;
        //            if (result[0].Trips.Count() > 0)
        //            {

        //                //Check for cement pending trip
        //                foreach (var tr in result[0].Trips)
        //                {
        //                    if (tr.StatusId != 3 && tr.StageEn != "Completed" && tr.SubTypeId == 1)
        //                    {
        //                        hasCementTrip = true;
        //                        break;
        //                    }
        //                }

        //                foreach (var trip in result[0].Trips)
        //                {
        //                    SubTripApiModel subTrip = new SubTripApiModel();
        //                    if (trip.StatusId != 3 && trip.StageEn != "Completed" && trip.SubTypeId == 1 && hasStart == false)
        //                    {
        //                        subTrip.start = 1;
        //                        hasStart = true;

        //                    }
        //                    else if (trip.StatusId != 3 && trip.StageEn != "Completed" && hasStart == false && hasCementTrip == false)
        //                    {
        //                        subTrip.start = 1;
        //                        hasStart = true;

        //                    }
        //                    else if (trip.StageEn == "Completed")
        //                    {
        //                        subTrip.start = 2;
        //                    }
        //                    else
        //                    {
        //                        subTrip.start = 0;
        //                    }
        //                    subTrip.material = trip.TypeId == 1 || trip.TypeId == 2 ? ((GeneralMaterial)trip.TypeId).ToString() : "Mission";//: "Backhualing";
        //                    subTrip.quantity = trip.Qty;
        //                    subTrip.status = languageId == 1 ? trip.StageEn.Trim() : trip.StageAR.Trim();
        //                    subTrip.tripId = trip.Id.ToString();
        //                    subTrip.truckNumber = truck.TruckNumber;
        //                    subTrip.truckId = truckId;
        //                    subTrip.fromDate = trip.departureDate.ToString("dd/MM/yyyy");
        //                    subTrip.toDate = trip.ArrivedDate.ToString("dd/MM/yyyy");
        //                    subTrip.tripType = (int)trip.SubTypeId;

        //                    subTrip.fromLocations = new List<LocationApiModel>();
        //                    var frmLocations = await _context.PlannedTripLocations.Where(a => a.ParentTrip == trip.ParentTrip && a.TripNumber == trip.TripNumber && a.Type == "Source").ToListAsync();
        //                    if (frmLocations.Count > 0)
        //                    {
        //                        subTrip.fromAddress = frmLocations[0].Location;
        //                        // subTrip.toAddress = "";
        //                    }
        //                    subTrip.toLocations = new List<LocationApiModel>();
        //                    var tLocations = await _context.PlannedTripLocations.Where(a => a.ParentTrip == trip.ParentTrip && a.TripNumber == trip.TripNumber && a.Type == "Dest").ToListAsync();
        //                    if (tLocations.Count > 0)
        //                    {
        //                        subTrip.toAddress = tLocations[0].Location;
        //                        // subTrip.toAddress = "";
        //                    }
        //                    tripApiModel.subTrips.Add(subTrip);
        //                }
        //            }
        //            tripApiModels.Add(tripApiModel);
        //        }
        //        return tripApiModels;
        //    }
        //    return new List<TripApiModel>();
        //}
    }
}
