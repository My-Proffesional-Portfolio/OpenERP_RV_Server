using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.ExceptionTypes
{
    public class FriendlyTransactionException : Exception
    {
        public FriendlyTransactionException(string ExceptionMessage) : base(ExceptionMessage)
        {
        }
    }
}
