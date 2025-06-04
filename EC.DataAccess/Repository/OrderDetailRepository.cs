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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(AppDbContext db) : base(db)
        {
        }
        public void Update(OrderDetail OrderDetail)
        {
            _db.Update(OrderDetail);
        }
    }
}
