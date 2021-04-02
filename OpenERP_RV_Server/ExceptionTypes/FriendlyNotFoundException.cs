using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.ExceptionTypes
{
    public class FriendlyNotFoundException : Exception
    {
        public FriendlyNotFoundException(string ExceptionMessage) : base(ExceptionMessage)
        {
        }
    }
}
