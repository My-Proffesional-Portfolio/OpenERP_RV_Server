using Microsoft.AspNetCore.Mvc;
using OpenERP_RV_Server.Backend;
using OpenERP_RV_Server.Models.CompanyOrganization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OpenERP_RV_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyOrganizationController : ControllerBase
    {
        // GET: api/<CompanyOrganizationController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<CompanyOrganizationController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CompanyOrganizationController>
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

        // PUT api/<CompanyOrganizationController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CompanyOrganizationController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
