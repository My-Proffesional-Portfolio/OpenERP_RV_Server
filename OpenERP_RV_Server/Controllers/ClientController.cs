using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenERP_RV_Server.Backend;
using OpenERP_RV_Server.Filters;
using OpenERP_RV_Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OpenERP_RV_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [AutomaticExceptionHandler]
    public class ClientController : ControllerBase
    {

        public ClientController(IHttpContextAccessor accessor)
        {
            BaseService.HttpContext = accessor.HttpContext;
        }
        // GET: api/<ClientController>
        [HttpGet]
        [SessionTokenManager]
        public IActionResult Get(int currentPage = 0, int pageSize = 10)
        {
            return Ok(new ClientService().GetPagedClients(currentPage, pageSize));
        }

        // GET api/<ClientController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ClientController>
        [HttpPost]
        [SessionTokenManager]
        public IActionResult Post([FromBody] ClientModel clientModel)
        {
            return Ok(new ClientService().AddNewClient(clientModel));
        }

        // PUT api/<ClientController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ClientController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
