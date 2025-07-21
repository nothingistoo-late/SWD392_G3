using Repositories.WorkSeeds.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class MembershipRepository : GenericRepository<Membership, Guid>, IMembershipRepository
    {
        public MembershipRepository(SWD392_G3DBcontext context) : base(context)
        {
        }
    }
}
