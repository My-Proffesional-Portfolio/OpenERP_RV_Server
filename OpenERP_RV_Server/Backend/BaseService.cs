using Microsoft.AspNetCore.Http;
using OpenERP_RV_Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class BaseService
    {
        protected OpenERP_RVContext DbContext = new OpenERP_RVContext();

        public static HttpContext HttpContext { get;  set; }
    }
}
