using Bookify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.DataAccess.Repository.IRepository
{
    public interface IProductRepository :IRepository<Product>
    {
        void Update(Product product);
    }
}
