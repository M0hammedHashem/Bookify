﻿using Bookify.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.DataAccess.Repository
{
    public interface IUnitOfWork
    {
        public ICategoryRepository Category { get;  }
        public IProductRepository Product { get;  }
        public IProductImageRepository ProductImage { get;  }

        public ICompanyRepository Company { get;  }
        public IShoppingCartRepository ShoppingCart { get;  }
        public IApplicationUserRepository ApplicationUser { get;  }
        public IOrderDetailRepository OrderDetail { get;  }
        public IOrderHeaderRepository OrderHeader { get;  }


        
        Task SaveAsync();

    }

}
