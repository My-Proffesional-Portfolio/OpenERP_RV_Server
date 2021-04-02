using Microsoft.AspNetCore.Mvc;
using OpenERP_RV_Server.Backend;
using OpenERP_RV_Server.Filters;
using OpenERP_RV_Server.Models.CompanyOrganization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AutomaticExceptionHandler]
    public class CompanyOrganizationController : ControllerBase
    {

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            return Ok(new CompanyOrganizationService().GetCorporateInfoById(id));
        }

        [HttpPost]
        public IActionResult Post([FromBody] NewCompanyOrganizationModel company)
        {
            //TODO: run in a generic Transaction method
            //Func<NewCompanyOrganizationResult> add = () =>
            //{
            //    return new CompanyOrganizationService().AddNewCompanyOrganization(company);
            //};

            //var addCompanyTransactionResult = new Transactions().RunTransaction(add);
            //return Ok(addCompanyTransactionResult);

            return Ok(new CompanyOrganizationService().AddNewCompanyOrganization(company));

        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
