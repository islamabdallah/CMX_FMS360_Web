using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.APIViewModels.Trip
{
    public class WeightApiModel
    {
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
        public double? weight { get; set; }
    }
    public class WeightDataApiModel
    {
        public WeightApiModel? emptyWeight { get; set; }
        public WeightApiModel? grossWeight { get; set; }
    }

}
