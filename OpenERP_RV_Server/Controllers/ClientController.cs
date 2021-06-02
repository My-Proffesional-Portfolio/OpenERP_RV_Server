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
            BaseService.accessor = accessor;
        }
        // GET: api/<ClientController>
        [HttpGet]
        [SessionTokenManager]
        public IActionResult Get(int currentPage = 0, int pageSize = 10)
        {
            return Ok(new ClientService().GetPagedClients(currentPage, pageSize));
        }

        // POST api/<ClientController>
        [HttpPost]
        [SessionTokenManager]
        public IActionResult Post([FromBody] ClientModel clientModel)
        {
            return Ok(new ClientService().AddNewClient(clientModel));
        }

    }
}
