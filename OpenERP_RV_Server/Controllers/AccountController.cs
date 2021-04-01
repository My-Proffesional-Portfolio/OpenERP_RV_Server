using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenERP_RV_Server.Backend;
using OpenERP_RV_Server.Models.Account.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OpenERP_RV_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        //https://stackoverflow.com/questions/42731686/using-httpcontext-outside-of-a-controller
        public AccountController(IHttpContextAccessor accessor)
        {
            BaseService.HttpContext = accessor.HttpContext;
        }
        // GET: api/<AccountController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AccountController>/5
        [HttpGet("{id}")]
       
        public string Get(int id)
        {
            //var userName = HttpContext.Session.GetString("userName");
            //var companyID = HttpContext.Session.GetString("companyID");
            return "value";
        }

        // POST api/<AccountController>
        [HttpPost]
        [Route("login")]
        public IActionResult Post([FromBody] LoginModel login)
        {
            var user = new UserService().Login(login);
            return Ok(user);
        }

        // PUT api/<AccountController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AccountController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
