using Bookify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository:IRepository<OrderHeader>
    {
        Task UpdateStatusAsync(int id, string orderStatus, string? paymentStatus = null);
        Task UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
        void Update(OrderHeader orderHeader);

    }
}
