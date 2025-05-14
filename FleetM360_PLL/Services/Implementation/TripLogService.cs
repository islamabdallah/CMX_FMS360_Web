using AutoMapper;
using FleetM360_DAL.Data.Repository;
using FleetM360_DAL.Models;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_DAL.Repository.EntityFramework;
using FleetM360_PLL.APIViewModels.Hazard;
using FleetM360_PLL.APIViewModels.Trip;
using FleetM360_PLL.APIViewModels.Trucks;
using FleetM360_PLL.Message;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Implementation
{
    public class TripLogService : ITripLogService
    {
        private readonly IRepository<TripLog, long> _repository;
        private readonly IRepository<Truck, long> _truckRepository;
        private readonly IRepository<TripDriver, long> _tripDriverRepository;
        private readonly IRepository<Driver, long> _driverRepository;
        private readonly ILogger<TripService> _logger;
        private readonly IMapper _mapper;
        private readonly ITripDriverService _driver;
        private readonly IPlannedTripLocationService _plannedTripLocationService;
        private readonly ITripDriverService _tripDriverService;
        public ApplicationDBContext _context;
        private readonly IDriverService _driverService;
        private readonly IEmployeeService _employeeService;

        public TripLogService(IRepository<TripLog, long> repository, IRepository<Truck, long> truckRepository, IRepository<TripDriver, long> tripDriverRepository,
        IRepository<Driver, long> driverRepository, ILogger<TripService> logger, IMapper mapper, ITripDriverService driver,
        IPlannedTripLocationService plannedTripLocationService, ITripDriverService tripDriverService, ApplicationDBContext context,
        IDriverService driverService,IEmployeeService employeeService)
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
            _driverService = driverService;
            _employeeService = employeeService;
        }
        public async Task<bool> CreateTrepFuelAsync(sendFuelDataApiModel model)
        {
            try
            {
                if (model != null)
                {
                    DriverModel driver = _driverService.GetDriver(model.userNumber);
                    if (driver != null)
                    {
                        var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.tripId)).FirstOrDefault();
                        if (trip != null)
                        {
                                var Event = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "Fuel").FirstOrDefault();
                            if (Event != null)
                            {
                                TripLog tripLog = new TripLog()
                                {
                                    ParentTrip = trip.ParentTrip,
                                    TripNumber = trip.TripNumber,
                                    Event = Event.LogName,
                                    LogId = Event.Id,
                                    Lat = model.lat,
                                    Long = model.lng,
                                    CreatedBy = driver.DriverNumber.ToString(),
                                    Date = DateTime.Now.ToString(),
                                    Comment = model.driverComment,
                                    IsDelted = false,
                                    IsVisible = true,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now,
                                };
                                _context.TripLogs.Add(tripLog);
                                await _context.SaveChangesAsync();

                                if (tripLog.Id >0)
                                {
                                    TripFuel fuel = new TripFuel()
                                    {
                                        userNumber= driver.DriverNumber,
                                        TripLogId= tripLog.Id,
                                        truckId=model.truckId,
                                        tripId = trip.Id.ToString(),
                                        gasStationId = Convert.ToInt64(model.gasStationId),
                                        numberOfKilometers = model.numberOfKilometers,
                                        doubleFuelQuantityInnLiters = model.doubleFuelQuantityInnLiters,
                                        lat = (double)model.lat,
                                        lng = (double)model.lng,
                                        fuelCost = model.fuelCost,
                                        cashPaymentMethodId = Convert.ToInt64(model.cashPaymentMethodId),
                                        driverComment = model.driverComment !=null?model.driverComment : "No Comment",
                                        IsDelted = false,
                                        IsVisible = true,
                                        CreatedDate = DateTime.Now,
                                        UpdatedDate = DateTime.Now,
                                    };
                                    _context.TripFuels.Add(fuel);
                                    await _context.SaveChangesAsync();

                                    if(fuel.Id > 0)
                                    {
                                        if(model.images != null)
                                        {
                                            if(model.images.Count > 0)
                                            {
                                                foreach(var image in model.images)
                                                {
                                                    var fileName = UploadedImageAsync(image, "FuelAttachments").Result;
                                                    if (fileName != null)
                                                    {
                                                        TripFuelAttachment tripFuelAttachment = new TripFuelAttachment()
                                                        {
                                                            TripFuelId = fuel.Id,
                                                            Name = fileName,
                                                            Category = "Images",
                                                            IsDelted = false,
                                                            IsVisible = true,
                                                            CreatedDate = DateTime.Now,
                                                            UpdatedDate = DateTime.Now,
                                                        };
                                                        _context.TripFuelAttachments.Add(tripFuelAttachment);
                                                        await _context.SaveChangesAsync();

                                                    }
                                                    

                                                }
                                            }
                                        }

                                        if(model.numberOfKilometersImages != null)
                                        {
                                            if (model.numberOfKilometersImages.Count > 0)
                                            {
                                                foreach (var image in model.numberOfKilometersImages)
                                                {
                                                    var fileName = UploadedImageAsync(image, "FuelAttachments").Result;
                                                    if (fileName != null)
                                                    {
                                                        TripFuelAttachment tripFuelAttachment = new TripFuelAttachment()
                                                        {
                                                            TripFuelId = fuel.Id,
                                                            Name = fileName,
                                                            Category = "KilometersImages",
                                                            IsDelted = false,
                                                            IsVisible = true,
                                                            CreatedDate = DateTime.Now,
                                                            UpdatedDate = DateTime.Now,
                                                        };
                                                        _context.TripFuelAttachments.Add(tripFuelAttachment);
                                                        await _context.SaveChangesAsync();

                                                    }

                                                }
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
                        }
                        else
                        {
                            return false;

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

        public async Task<string> UploadedImageAsync(IFormFile ImageName, string path)
        {
            string uniqueFileName = null;
            string filePath = null;

            if (ImageName != null)
            {
                string uploadsFolder = Path.Combine(CommanData.UploadMainFolder, path);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageName.FileName;
                uniqueFileName = uniqueFileName.Replace(" ", "");
                filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageName.CopyToAsync(fileStream);
                }
            }
            return uniqueFileName;
        }

        public async Task<bool> CreateStartRoadMaintenanceAsync(sendStopStartTime model)
        {
            try
            {
                if (model != null)
                {
                    DriverModel driver = _driverService.GetDriver(model.userNumber);
                    if (driver != null)
                    {
                        var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.tripId)).FirstOrDefault();
                        if (trip != null)
                        {
                            var Event = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "StartRoadMaintenance").FirstOrDefault();
                            if (Event != null)
                            {
                                TripLog tripLog = new TripLog()
                                {
                                    ParentTrip = trip.ParentTrip,
                                    TripNumber = trip.TripNumber,
                                    Event = Event.LogName,
                                    LogId = Event.Id,
                                    Lat = model.lat,
                                    Long = model.lng,
                                    CreatedBy = driver.DriverNumber.ToString(),
                                    Date = model.startTime.ToString(),
                                    Comment = model.driverComment,
                                    IsDelted = false,
                                    IsVisible = true,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now,
                                };
                                _context.TripLogs.Add(tripLog);
                                await _context.SaveChangesAsync();

                                if (tripLog.Id > 0)
                                {
                                    TripRoadMaintenance fuel = new TripRoadMaintenance()
                                    {
                                        userNumber = driver.DriverNumber,
                                        TripLogId = tripLog.Id,
                                        truckId = model.truckId,
                                        tripId = trip.Id,
                                        causeOfFailureId = Convert.ToInt64(model.causeOfFailure),
                                        wayOfDealId = Convert.ToInt64(model.wayOfDeal),
                                        driverComment = model.driverComment,
                                        lat = model.lat,
                                        lng = model.lng,
                                        IsDelted = false,
                                        IsVisible = true,
                                        CreatedDate = DateTime.Now,
                                        UpdatedDate = DateTime.Now,
                                    };
                                    _context.TripRoadMaintenances.Add(fuel);
                                    await _context.SaveChangesAsync();
                                   
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            return false;

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

        public async Task<bool> CreateEndRoadMaintenanceAsync(sendStopStartTime model)
        {
            try
            {
                if (model != null)
                {
                    DriverModel driver = _driverService.GetDriver(model.userNumber);
                    if (driver != null)
                    {
                        var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.tripId)).FirstOrDefault();
                        if (trip != null)
                        {
                            var Event = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "EndRoadMaintenance").FirstOrDefault();
                            if (Event != null)
                            {
                                TripLog tripLog = new TripLog()
                                {
                                    ParentTrip = trip.ParentTrip,
                                    TripNumber = trip.TripNumber,
                                    Event = Event.LogName,
                                    LogId = Event.Id,
                                    Lat = model.lat,
                                    Long = model.lng,
                                    CreatedBy = driver.DriverNumber.ToString(),
                                    Date = model.endTime.ToString(),
                                    Comment = model.driverComment,
                                    IsDelted = false,
                                    IsVisible = true,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now,
                                };
                                _context.TripLogs.Add(tripLog);
                                await _context.SaveChangesAsync();

                                if (tripLog.Id > 0)
                                {
                                    TripRoadMaintenance fuel = new TripRoadMaintenance()
                                    {
                                        userNumber = driver.DriverNumber,
                                        TripLogId = tripLog.Id,
                                        truckId = model.truckId,
                                        tripId = trip.Id,
                                        causeOfFailureId = Convert.ToInt64(model.causeOfFailure),
                                        wayOfDealId = Convert.ToInt64(model.wayOfDeal),
                                        driverComment = model.driverComment,
                                        lat = model.lat,
                                        lng = model.lng,
                                        IsDelted = false,
                                        IsVisible = true,
                                        CreatedDate = DateTime.Now,
                                        UpdatedDate = DateTime.Now,
                                    };
                                    _context.TripRoadMaintenances.Add(fuel);
                                    await _context.SaveChangesAsync();

                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            return false;

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

        public async Task<bool> CreateStartStopBanAsync(sendStopStartTime model)
        {
            try
            {
                if (model != null)
                {
                    DriverModel driver = _driverService.GetDriver(model.userNumber);
                    if (driver != null)
                    {
                        var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.tripId)).FirstOrDefault();
                        if (trip != null)
                        {
                            var Event = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "StartStopBan").FirstOrDefault();
                            if (Event != null)
                            {
                                TripLog tripLog = new TripLog()
                                {
                                    ParentTrip = trip.ParentTrip,
                                    TripNumber = trip.TripNumber,
                                    Event = Event.LogName,
                                    LogId = Event.Id,
                                    Lat = model.lat,
                                    Long = model.lng,
                                    CreatedBy = driver.DriverNumber.ToString(),
                                    Date = model.startTime.ToString(),
                                    Comment = model.driverComment,
                                    IsDelted = false,
                                    IsVisible = true,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now,
                                };
                                _context.TripLogs.Add(tripLog);
                                await _context.SaveChangesAsync();

                                if (tripLog.Id > 0)
                                {
                                    TripStopBan fuel = new TripStopBan()
                                    {
                                        userNumber = driver.DriverNumber,
                                        TripLogId = tripLog.Id,
                                        truckId = model.truckId,
                                        tripId = trip.Id,
                                        stopOptionId = Convert.ToInt64(model.causeOfFailure),
                                        driverComment = model.driverComment,
                                        lat = model.lat,
                                        lng = model.lng,
                                        IsDelted = false,
                                        IsVisible = true,
                                        CreatedDate = DateTime.Now,
                                        UpdatedDate = DateTime.Now,
                                    };
                                    _context.TripStopBans.Add(fuel);
                                    await _context.SaveChangesAsync();

                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            return false;

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

        public async Task<bool> CreateEndStopBanAsync(sendStopStartTime model)
        {
            try
            {
                if (model != null)
                {
                    DriverModel driver = _driverService.GetDriver(model.userNumber);
                    if (driver != null)
                    {
                        var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.tripId)).FirstOrDefault();
                        if (trip != null)
                        {
                            var Event = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "EndStopBan").FirstOrDefault();
                            if (Event != null)
                            {
                                TripLog tripLog = new TripLog()
                                {
                                    ParentTrip = trip.ParentTrip,
                                    TripNumber = trip.TripNumber,
                                    Event = Event.LogName,
                                    LogId = Event.Id,
                                    Lat = model.lat,
                                    Long = model.lng,
                                    CreatedBy = driver.DriverNumber.ToString(),
                                    Date = model.endTime.ToString(),
                                    Comment = model.driverComment,
                                    IsDelted = false,
                                    IsVisible = true,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now,
                                };
                                _context.TripLogs.Add(tripLog);
                                await _context.SaveChangesAsync();

                                if (tripLog.Id > 0)
                                {
                                    TripStopBan fuel = new TripStopBan()
                                    {
                                        userNumber = driver.DriverNumber,
                                        TripLogId = tripLog.Id,
                                        truckId = model.truckId,
                                        tripId = trip.Id,
                                        stopOptionId = Convert.ToInt64(model.causeOfFailure),
                                       // wayOfDealId = Convert.ToInt64(model.wayOfDeal),
                                        driverComment = model.driverComment,
                                        lat = model.lat,
                                        lng = model.lng,
                                        IsDelted = false,
                                        IsVisible = true,
                                        CreatedDate = DateTime.Now,
                                        UpdatedDate = DateTime.Now,
                                    };
                                    _context.TripStopBans.Add(fuel);
                                    await _context.SaveChangesAsync();

                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            return false;

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

        public async Task<bool> CreateSiteProcessingAsync(sendTake5DataApiModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (model != null)
                {
                    DriverModel driver = _driverService.GetDriver(model.userNumber);
                    if (driver != null)
                    {
                        var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.tripId)).FirstOrDefault();
                        if (trip != null)
                        {
                            //adding take5 step1
                            var Event = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "Take5Step1").FirstOrDefault();
                            if (Event != null)
                            {
                                TripLog tripLog = new TripLog()
                                {
                                    ParentTrip = trip.ParentTrip,
                                    TripNumber = trip.TripNumber,
                                    Event = Event.LogName,
                                    LogId = Event.Id,
                                    Lat = model.step1.lat,
                                    Long = model.step1.lng,
                                    CreatedBy = driver.DriverNumber.ToString(),
                                    Date = model.step1.end.ToString(),
                                    IsDelted = false,
                                    IsVisible = true,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now,
                                };
                                _context.TripLogs.Add(tripLog);
                                await _context.SaveChangesAsync();

                                //if (tripLog.Id > 0)
                                //{
                                    TripTake5 step1 = new TripTake5()
                                    {
                                        userNumber = driver.DriverNumber,
                                        TripLogId = tripLog.Id,
                                        truckId = model.truckId,
                                        tripId = trip.Id,
                                        ActualTripId = model.tripLocationId,
                                        // wayOfDealId = Convert.ToInt64(model.wayOfDeal),
                                        Step = 1,
                                        lat =(double)model.step1.lat,
                                        lng = (double)model.step1.lng,
                                        StartTime=model.step1.start,
                                        EndTime=model.step1.end,
                                        IsDelted = false,
                                        IsVisible = true,
                                        CreatedDate = DateTime.Now,
                                        UpdatedDate = DateTime.Now,
                                    };
                                    _context.TripTake5s.Add(step1);
                                    await _context.SaveChangesAsync();
                                if (model.step1 != null)
                                {
                                    if (model.step1.falseIds != null)
                                    {
                                        if (model.step1.falseIds.Count > 0)
                                        {
                                            foreach (var id in model.step1.falseIds)
                                            {
                                                TripQuestion question = new TripQuestion()
                                                {
                                                    PlannedTripLocationId = model.tripLocationId,
                                                    ParentTrip = trip.ParentTrip,
                                                    TripNumber = trip.TripNumber,
                                                    Lat = (double)model.step1.lat,
                                                    Long = (double)model.step1.lng,
                                                    QuestionId = id,
                                                    Answer = false,
                                                };
                                                _context.TripQuestions.Add(question);
                                                await _context.SaveChangesAsync();
                                            }
                                        }
                                    }



                                }

                                    //await transaction.CommitAsync();
                                    //return true;
                                //}
                                //else
                                //{
                                //    return false;
                                //}
                            }


                            //adding unloading data 
                             Event = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "UnLoading").FirstOrDefault();
                            if (Event != null)
                            {
                                TripLog tripLog = new TripLog()
                                {
                                    ParentTrip = trip.ParentTrip,
                                    TripNumber = trip.TripNumber,
                                    Event = Event.LogName,
                                    LogId = Event.Id,
                                    Lat = model.unLoading.lat,
                                    Long = model.unLoading.lng,
                                    CreatedBy = driver.DriverNumber.ToString(),
                                    Date = model.step1.end.ToString(),
                                    StartDate=model.step1.end,
                                    EndDate=model.step2.start,
                                    IsDelted = false,
                                    IsVisible = true,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now,
                                };
                                _context.TripLogs.Add(tripLog);
                                await _context.SaveChangesAsync();

                                //if (tripLog.Id > 0)
                                //{
                                var plannedLocation=_context.PlannedTripLocations.Where(a=>a.IsVisible==true && a.Id==model.tripLocationId && a.Type.Trim()== "Dest".Trim()).FirstOrDefault();
                                var actuallocations=_context.ActualTripLocations.Where(b=>b.PlannedTripLocationId==model.tripLocationId).OrderBy(b=>b.Id).LastOrDefault();
                                double lastQuantity=plannedLocation !=null? plannedLocation.Qty:0;
                                if (actuallocations != null)
                                {
                                    lastQuantity = actuallocations.Remain;
                                }
                                ActualTripLocation location = new ActualTripLocation()
                                {
                                    PlannedTripLocationId = model.tripLocationId,

                                    ParentTrip = trip.ParentTrip,
                                    TripNumber = trip.TripNumber,
                                    TripLogId = tripLog.Id,
                                    Type = "Dist",
                                    Lat = (double)model.unLoading.lat,
                                    Long = (double)model.unLoading.lng,
                                    Material = plannedLocation != null ? plannedLocation.Material : "",
                                    Qty = plannedLocation != null ? plannedLocation.Qty : 0,
                                    Received = (double)model.unLoading.receivedQty,
                                    Remain = lastQuantity-(double)model.unLoading.receivedQty,
                                    Hours = model.unLoading.hours,
                                    Minutes = model.unLoading.minutes,
                                    Seconds = model.unLoading.seconds,
                                    IsDelted = false,
                                    IsVisible = true,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now,
                                };
                                _context.ActualTripLocations.Add(location);
                                await _context.SaveChangesAsync();
                               
                                if(model.onSiteRisks != null)
                                {
                                    if(model.onSiteRisks.Count > 0)
                                    {
                                        foreach(var risk in model.onSiteRisks)
                                        {
                                            string way = "";
                                            if(risk.waysToDealWithOnSiteRisk != null)
                                            {
                                                if(risk.waysToDealWithOnSiteRisk.Count > 0)
                                                {
                                                    way = risk.waysToDealWithOnSiteRisk[0];
                                                }
                                            }
                                            TripDanger danger = new TripDanger()
                                            {
                                                PlannedTripLocationId = model.tripLocationId,
                                                ParentTrip = trip.ParentTrip,
                                                TripNumber = trip.TripNumber,
                                                Lat = model.unLoading.lat,
                                                Long = model.unLoading.lng,
                                                MeasureControl = way,
                                                DangerId = risk.id,
                                                IsDelted = false,
                                                IsVisible = true,
                                                CreatedDate = DateTime.Now,
                                                UpdatedDate = DateTime.Now,
                                            };
                                            _context.TripDangers.Add(danger);
                                            await _context.SaveChangesAsync();
                                        }
                                    }
                                }

                                //await transaction.CommitAsync();
                                //return true;
                               
                            }


                            //adding take5 step2
                            Event = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "Take5Step2").FirstOrDefault();
                            if (Event != null)
                            {
                                TripLog tripLog = new TripLog()
                                {
                                    ParentTrip = trip.ParentTrip,
                                    TripNumber = trip.TripNumber,
                                    Event = Event.LogName,
                                    LogId = Event.Id,
                                    Lat = model.step2.lat,
                                    Long = model.step2.lng,
                                    CreatedBy = driver.DriverNumber.ToString(),
                                    Date = model.step2.end.ToString(),
                                    IsDelted = false,
                                    IsVisible = true,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now,
                                };
                                _context.TripLogs.Add(tripLog);
                                await _context.SaveChangesAsync();

                                //if (tripLog.Id > 0)
                                //{
                                TripTake5 step2 = new TripTake5()
                                {
                                    userNumber = driver.DriverNumber,
                                    TripLogId = tripLog.Id,
                                    truckId = model.truckId,
                                    tripId = trip.Id,
                                    ActualTripId = model.tripLocationId,
                                    // wayOfDealId = Convert.ToInt64(model.wayOfDeal),
                                    Step = 1,
                                    lat = (double)model.step2.lat,
                                    lng = (double)model.step2.lng,
                                    StartTime = model.step2.start,
                                    EndTime = model.step2.end,
                                    IsDelted = false,
                                    IsVisible = true,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now,
                                };
                                _context.TripTake5s.Add(step2);
                                await _context.SaveChangesAsync();
                                if (model.step2 != null)
                                {
                                    if (model.step2.falseIds != null)
                                    {
                                        if (model.step2.falseIds.Count > 0)
                                        {
                                            foreach (var id in model.step2.falseIds)
                                            {
                                                TripQuestion question = new TripQuestion()
                                                {
                                                    PlannedTripLocationId = model.tripLocationId,
                                                    ParentTrip = trip.ParentTrip,
                                                    TripNumber = trip.TripNumber,
                                                    Lat = (double)model.step2.lat,
                                                    Long = (double)model.step2.lng,
                                                    QuestionId = id,
                                                    Answer = false,
                                                };
                                                _context.TripQuestions.Add(question);
                                                await _context.SaveChangesAsync();
                                            }
                                        }
                                    }



                                }

                                await transaction.CommitAsync();
                                return true;
                                //}
                                //else
                                //{
                                //    return false;
                                //}
                            }
                        }
                        else
                        {
                            return false;

                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Roll back all changes if any fail
                await transaction.RollbackAsync();

                return false;
            }

           
        }

        public async Task<bool> CreateArriveSiteAsync(TruckStatusApiModel model)
        {
            try
            {
                if (model != null)
                {
                    DriverModel driver = _driverService.GetDriver(model.userNumber);
                    if (driver != null)
                    {
                        var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.subTrip)).FirstOrDefault();
                        if (trip != null)
                        {
                            string logName = "ArriveSite";
                            var trips =await _context.Trips.Where(t=>t.ParentTrip==trip.ParentTrip).ToListAsync();
                            if (trips != null)
                            {
                                if (trips.Count > 0)
                                {
                                    foreach (var tr in trips)
                                    {
                                        if(tr.StageEn !="Completed" &&  tr.StageEn != "Canceled")
                                        {
                                            logName = "ArriveSite";
                                            break;
                                        }
                                        else
                                        {
                                            logName = "ArrivePlant";
                                        }
                                    }
                                }
                                var Event = await _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == logName).FirstOrDefaultAsync();
                                if (Event != null)
                                {
                                    TripLog tripLog = new TripLog()
                                    {
                                        ParentTrip = trip.ParentTrip,
                                        TripNumber = trip.TripNumber,
                                        Event = Event.LogName,
                                        LogId = Event.Id,
                                        Lat = model.lat,
                                        Long = model.lng,
                                        CreatedBy = driver.DriverNumber.ToString(),
                                        Date = DateTime.Now.ToString(),
                                        IsDelted = false,
                                        IsVisible = true,
                                        CreatedDate = DateTime.Now,
                                        UpdatedDate = DateTime.Now,
                                    };
                                    _context.TripLogs.Add(tripLog);
                                    await _context.SaveChangesAsync();

                                    return true;
                                }
                            }
                            else { return false; }
                        }
                        else
                        {
                            return false;

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
    }
}
