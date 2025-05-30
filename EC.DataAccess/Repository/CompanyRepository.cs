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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly AppDbContext _context;

        public CompanyRepository(AppDbContext context):base(context) 
        {
            _context=context;
        }
        public void Update(Company company)
        {
            _context.Update(company);
        }
    }
}
