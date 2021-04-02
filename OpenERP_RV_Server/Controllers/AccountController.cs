using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenERP_RV_Server.Backend;
using OpenERP_RV_Server.Filters;
using OpenERP_RV_Server.Models.Account.Request;
using OpenERP_RV_Server.Models.Account.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OpenERP_RV_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AutomaticExceptionHandler]
    public class AccountController : ControllerBase
    {

        //https://stackoverflow.com/questions/42731686/using-httpcontext-outside-of-a-controller
        public AccountController(IHttpContextAccessor accessor)
        {
            BaseService.HttpContext = accessor.HttpContext;
        }
        // GET: api/<AccountController>
        [HttpGet]
        [Route("currentSession")]
        public IActionResult Get()
        {
            //var userName = HttpContext.Session.GetString("userName");
            //var companyID = HttpContext.Session.GetString("companyID");
            return Ok(new UserService().GetCurrentUserSession());
        }

        // POST api/<AccountController>
        [HttpPost]
        [Route("login")]
        public IActionResult Post([FromBody] LoginModel login)
        {
            var user = new UserService().Login(login);
            return Ok(user);
        }

        // DELETE api/<AccountController>/5
        [HttpDelete()]
        [Route("logout")]
        public void Delete(int id)
        {
        }
    }
}
