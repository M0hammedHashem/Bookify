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
