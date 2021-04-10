using Microsoft.AspNetCore.Http;
using OpenERP_RV_Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class BaseService
    {
        //protected  OpenERP_RVContext DbContext = new OpenERP_RVContext();
        public OpenERP_RVContext DbContext = new OpenERP_RVContext();

        protected  OpenERP_RVContext GetScopedDbContext(OpenERP_RVContext trasnsactionContext = null)
        {
            var _dbContext = trasnsactionContext == null ? DbContext : trasnsactionContext;
            return _dbContext;
        }
        public static HttpContext HttpContext { get;  set; }
    }
}
