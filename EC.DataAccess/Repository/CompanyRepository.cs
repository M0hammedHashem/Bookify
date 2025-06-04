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
