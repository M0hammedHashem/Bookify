using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository.IRepository
{
    public interface IRepository<T>where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>> ?filter = null, string? include = null);
        void Add(T entity);

        T Get(Expression<Func<T,bool>>filter, string? include = null);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entity);
         

    }
}
