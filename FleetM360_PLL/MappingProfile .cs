using AutoMapper;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_PLL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Truck, TruckModel>();
            CreateMap<TruckModel, Truck>();
            CreateMap<JobSite, JobSiteModel>();
            CreateMap<JobSiteModel, JobSite>();
            CreateMap<Driver, DriverModel>();
            CreateMap<DriverModel, Driver>();
            CreateMap<Employee, EmployeeModel>();
            CreateMap<EmployeeModel, Employee>();
        }
    }
}
