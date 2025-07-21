using Repositories.WorkSeeds.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class RatingRepository : GenericRepository<Rating, Guid>, IRatingRepository
    {
        public RatingRepository(SWD392_G3DBcontext context) : base(context)
        {
        }
    }
}
