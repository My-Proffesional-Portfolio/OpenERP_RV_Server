using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class Transactions : BaseService
    {
        public void RunVoidTransaction(Action actionDB)
        {

            using (var con = DbContext)
            {
                using (var transaction = con.Database.BeginTransaction())
                {
                    try
                    {
                        actionDB();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }

        }
        //https://stackoverflow.com/questions/2745815/in-c-what-is-t-after-a-method-declaration
        public T RunTransaction<T>(Func<T> function)
        {

            using var context = DbContext;
            using var transaction = context.Database.BeginTransaction();
           

            try
            {
                var result = function();
                transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
               
                throw new Exception(ex.Message);
                //result.ErrorMessages.Add(ex.Message);
                //return result;
            }
            //using (var transactionContext = DbContext)
            //{
            //    using (var db_transactionContext = transactionContext.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            var result = function();
            //            db_transactionContext.Commit();
            //            return result;
            //        }
            //        catch (Exception ex)
            //        {

            //            db_transactionContext.Rollback();
            //            throw new Exception(ex.Message);
            //        }
            //    }

            //}
        }

    }
}
