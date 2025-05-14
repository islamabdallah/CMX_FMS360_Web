using FleetM360_PLL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Contracts
{
    public interface IDriverService
    {
        List<DriverModel> GetAllDrivers();
        Task<bool> CreateDriver(DriverModel model);
        //Task<bool> UpdateDriver(DriverModel model);
        //bool DeleteDriver(long id);
        DriverModel GetDriver(long id);
        Task<bool> AcceptCondition(DriverModel model);
        //Driver GetDriverByUserId(string userId);

        //List<DriverAPIModel> GetAllDriversForMobile();

        //List<DriverModel> GetAllActiveDrivers();

        //DriverModel GetDriverModelByUserId(string userId);
        //public string CreateRandomPassword(int length);

    }
}
