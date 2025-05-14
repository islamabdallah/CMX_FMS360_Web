using FleetM360_DAL.Models;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_DAL.Models.MasterModels.HazardData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Repository.EntityFramework
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        { }
        public DbSet<Driver> Drivers { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<JobSite> JobSites { get; set; }

        public DbSet<Trip> Trips { get; set; }

        public DbSet<PlannedTripLocation> PlannedTripLocations { get; set; }

        public DbSet<ActualTripLocation> ActualTripLocations { get; set; }
        public DbSet<TripDriver> TripDrivers { get; set; }
        public DbSet<TripLog> TripLogs { get; set; }
        public DbSet<LogLookup> LogLookups { get; set; }
        public DbSet<Truck> Trucks { get; set; }
        public DbSet<TruckSilo> TruckSilos { get; set; }
        public DbSet<PreCheckAnswer> PreCheckAnswers { get; set; }
        public DbSet<PreCheckQuestion> PreCheckQuestions { get; set; }
        public DbSet<TripPrecheck> TripPrechecks { get; set; }
        public DbSet<TripPrecheckDetails> TripPrecheckDetails { get; set; }
        public DbSet<StopOption> StopOptions { get; set; }
        public DbSet<GasStation> GasStations { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<TripFuel> TripFuels { get; set; }
        public DbSet<TripFuelAttachment> TripFuelAttachments { get; set; }
        public DbSet<Risk> Risks { get; set; }
        public DbSet<RiskLevel> RiskLevel { get; set; }
        public DbSet<TripRoadMaintenance> TripRoadMaintenances { get; set; }
        public DbSet<WayToDealWithTruckBreakdown> WayToDealWithTruckBreakdowns { get; set; }
        public DbSet<CauseOfTruckFailure> CauseOfTruckFailures { get; set; }
        public DbSet<TripStopBan> TripStopBans { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Danger> Dangers { get; set; }
        public DbSet<DangerCategory> DangerCategories { get; set; }
        public DbSet<MeasureControl> MeasureControls { get; set; }
        public DbSet<TripQuestion> TripQuestions { get; set; }
        public DbSet<TripDanger> TripDangers { get; set; }
        public DbSet<TripTake5> TripTake5s {  get; set; }
        public DbSet<QuestionAnswer> QuestionAnswers { get; set; }
        public DbSet<DriverFeedback> DriverFeedbacks { get; set; }
    }
}
