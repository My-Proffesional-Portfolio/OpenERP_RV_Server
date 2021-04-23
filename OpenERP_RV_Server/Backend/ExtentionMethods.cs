using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public static class ExtentionMethods
    {

        public static IQueryable<T> GetPagedData<T>(this IOrderedQueryable<T> IQueryableEntity, int currentPage, int itemsPerPage) where T : class
        {
            var pagedItems = IQueryableEntity.Skip(currentPage * itemsPerPage).Take(itemsPerPage).AsQueryable();
            return pagedItems;
        }
    }
}
