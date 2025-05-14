using AutoMapper;
using FleetM360_DAL.Data.Repository;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_DAL.Repository.EntityFramework;
using FleetM360_PLL.APIViewModels.Drivers;
using FleetM360_PLL.APIViewModels.Trip;
using FleetM360_PLL.Services.Contracts;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Implementation
{

    public class PreCheckService : IPreCheckService
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

        public PreCheckService(IRepository<Trip, long> repository, IRepository<Truck, long> truckRepository, IRepository<TripDriver, long> tripDriverRepository,
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
        public async Task<bool> AddTripPrecheck(TripPreCheckApiModel model)
        {
           // await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                string state = "Success";
                string MainCategory = "";
                if (model.category == 1)
                {
                    MainCategory = "الحالة الصحية".Trim(); //model.category.ToString();
                }
                else if (model.category == 2)
                {
                    MainCategory = "معدات".Trim(); //model.category.ToString();
                }
                else if (model.category == 3)
                {
                    MainCategory = "فحص".Trim(); //model.category.ToString();
                }
                if (model.questionIds != null)
                {
                    if (model.questionIds.Count > 0)
                    {
                        state = "Failed";
                    }
                }
                var answer = _context.PreCheckAnswers.Where(a => a.PreCheckQuestionId == 1 && a.AnswerValue == false).FirstOrDefault();
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.tripId)).FirstOrDefault();
                // Add second record to TableB
                var itemB = new TripPrecheck
                {
                    TripId = Convert.ToInt64(model.tripId),
                    ParentTrip = trip.ParentTrip,
                    TripNumber = trip.TripNumber,
                    TruckNumber = trip.TruckNumber,
                    SiloNumber = trip.SiloNumber,
                    DriverId = "14811",
                    Date = DateTime.Now,
                    Lat = model.lat != null ? (double)model.lat : 0,
                    Lng = model.lng != null ? (double)model.lng : 0,
                    Category = MainCategory,
                    Status = state,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    IsDelted = false,
                    IsVisible = true
                };
                _context.TripPrechecks.Add(itemB);
                 await _context.SaveChangesAsync();

                var Event = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "PreCheck").FirstOrDefault();
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
                        CreatedBy = model.UserNumber.ToString(),
                        Date = model.date.ToString(),
                        CreatedDate= DateTime.Now,
                        UpdatedDate= DateTime.Now,
                        IsDelted = false,
                        IsVisible = true
                    };
                    _context.TripLogs.Add(tripLog);
                    await _context.SaveChangesAsync();
                    //return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = "" });
                }
                trip.StageAR = "قيد الفحص";
                trip.StageEn = "Under Inspection";
                trip.UpdatedDate = DateTime.Now;
                _context.Trips.Update(trip);
                await _context.SaveChangesAsync();
                if (model.questionIds != null)
                {
                    if(model.questionIds.Count > 0)
                    {
                        //state = "Failed";
                        foreach (int qId in model.questionIds)
                        {


                            // Add first record to TableA
                            var itemA = new TripPrecheckDetails
                            {
                                TripPrecheckId =itemB.Id,//Convert.ToInt64(model.tripId),
                                QuestionId = qId,
                                AnswerId = answer.PreCheckAnswerId,
                                CreatedDate = DateTime.Now,
                                UpdatedDate = DateTime.Now,
                                IsDelted = false,
                                IsVisible = true
                            };
                            _context.TripPrecheckDetails.Add(itemA);
                            await _context.SaveChangesAsync();


                            // Commit transaction if all successful
                            // await transaction.CommitAsync();

                            //return Ok("Data inserted successfully.");
                        }
                        var Eventt = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "Maintainance").FirstOrDefault();
                        if (Eventt != null)
                        {
                            TripLog tripLog = new TripLog()
                            {
                                ParentTrip = trip.ParentTrip,
                                TripNumber = trip.TripNumber,
                                Event = Eventt.LogName,
                                LogId = Eventt.Id,
                                Lat = model.lat,
                                Long = model.lng,
                                CreatedBy = model.UserNumber.ToString(),
                                Date = model.date.ToString(),
                                CreatedDate= DateTime.Now,
                                UpdatedDate= DateTime.Now,
                                IsDelted= false,
                                IsVisible = true
                            };
                            _context.TripLogs.Add(tripLog);
                            await _context.SaveChangesAsync();
                            //return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = "" });
                        }
                        trip.StageAR = "قيد الفحص";
                        trip.StageEn = "Under Inspection";
                        trip.UpdatedDate = DateTime.Now;
                        _context.Trips.Update(trip);
                        await _context.SaveChangesAsync();
                    }
                }
               
                return true; 
            }
            catch (Exception ex)
            {
                // Rollback transaction if error occurred
                //await transaction.RollbackAsync();
               // return StatusCode(500, $"Transaction failed: {ex.Message}");
               return false;
            }

           // throw new NotImplementedException();
        }
    }
}
