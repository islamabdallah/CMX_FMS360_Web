using FleetM360_PLL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Contracts.TermsConditions
{
    public interface ITermsConditionsService
    {
        public Task<TermsOfConditionsModel> LoadEnglishTerms();


        public Task<TermsOfConditionsModel> LoadArabicTerms();
    }
}
