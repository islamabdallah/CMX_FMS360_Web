using FleetM360_PLL.APIViewModels.Hazard;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;



namespace FleetM360_PLL.Services.Implementation
{
    public class ShipmentRiskRepository
    {
        private IDbConnection _db;
       // private readonly string connectionString = Utility.GetDatabaseConnectionstring();
        public ShipmentRiskRepository()
        {
          // _db = new SqlConnection(connectionString);
        }

        /// <summary>
        /// Check if the risk already linked with the active shipment or not
        /// </summary>
        /// <param name="shipment_ID"></param>
        /// <param name="riskID"></param>
        /// <returns></returns>
        //public async Task<bool> IsExistsAsync(ApiTemplate template)
        //{
        //    if (template != null)
        //    {
        //        string _query = string.Format(@"Select Count(1) FROM [ShipmentRisk] Where [Shipment_ID] Like '%{0}%' and  [Risk_ID] = {1} and [Country] Like '%{2}%'  and company like  '%{3}%' ", template.Shipment_ID, template.Risk_ID, template.Country, template.Company);
        //        //var _result = await _db.ExecuteScalarAsync<bool>(_query, new { template.Shipment_ID, template.Risk_ID });
        //        return true;//_result;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Insert Risk for the active shipment 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        //public async Task<int> AddAsync(ApiTemplate template)
        //{
        //    if (template != null)
        //    {
        //        string _query = string.Format(@"Insert into [ShipmentRisk] ([Shipment_ID],[Risk_ID], [Date],[Lat],[Long],[Mobile] , Country , Company ) values
        //                                                                 ('{0}',{1},'{2}','{3}','{4}','{5}','{6}','{7}' )", template.Shipment_ID, template.Risk_ID, DateTime.Now.ToString(), template.Lat, template.Long, template.MobileNumber, template.Country, template.Company);
        //        //var _result = await _db.ExecuteAsync(_query, template);
        //        return 0;//_result;
        //    }
        //    return -1;
        //}
    }
}
