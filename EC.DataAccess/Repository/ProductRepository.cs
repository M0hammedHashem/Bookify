using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository
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
