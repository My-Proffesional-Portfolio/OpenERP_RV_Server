using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public static class ExtentionMethods
    {
        /// <summary>
        /// Extention Method for IOrderedQueryable<T> where T is the Entity Database Model (DbSet)
        /// </summary>
        /// <typeparam name="T">T is the Entity Database Model (DbSet)</typeparam>
        /// <param name="IQueryableEntity">Raw query data from table or DbSet </param>
        /// <param name="currentPage">current page request</param>
        /// <param name="itemsPerPage">items per page in request</param>
        /// <returns></returns>
        public static IQueryable<T> GetPagedData<T>(this IOrderedQueryable<T> IQueryableEntity, int currentPage, int itemsPerPage) where T : class
        {
            var pagedItems = IQueryableEntity.Skip(currentPage * itemsPerPage).Take(itemsPerPage).AsQueryable();
            return pagedItems;
        }
    }
}
