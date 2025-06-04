using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.DataAccess.Repository.IRepository
{
    public interface IRepository<T>where T : class
    {
        
            Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? include = null);
            Task AddAsync(T entity);
            Task<T> GetAsync(Expression<Func<T, bool>> filter, string? include = null);
            Task DeleteAsync(T entity);
            Task DeleteRangeAsync(IEnumerable<T> entities);
        

    }
}
