using Bookify.DataAccess.Data;
using Bookify.DataAccess.Repository.IRepository;
using Bookify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        
        public ShoppingCartRepository(AppDbContext db):base(db) 
        {
            
        }
        public void Update  (ShoppingCart shoppingCart)
        {
            _db.Update(shoppingCart);
        }
    }
}
