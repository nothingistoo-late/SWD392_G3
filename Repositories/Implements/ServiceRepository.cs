using Repositories.WorkSeeds.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class ServiceRepository : GenericRepository<Service, Guid>, IServiceRepository
    {
        public ServiceRepository(SWD392_G3DBcontext context) : base(context)
        {
        }
    }
}
