using Bookify.DataAccess.Data;
using Bookify.DataAccess.Repository.IRepository;
using Bookify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext db) : base(db)
        {
        }
        public void Update(Product product)
        {
            _db.Update(product);
        }
    }
}
