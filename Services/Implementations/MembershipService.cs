using DTOs.MemberShip.Request;
using DTOs.MemberShip.Respond;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class MembershipService : BaseService<MembershipService, Guid>, IMembershipService
    {
        public MembershipService(IGenericRepository<MembershipService, Guid> repository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ICurrentTime currentTime) : base(repository, currentUserService, unitOfWork, currentTime)
        {
        }

        public Task<ApiResult<List<MembershipResponse>>> CreateAsync(CreateMembershipRequest dto)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResult<List<MembershipResponse>>> GetAllAsync()
        {
            //try
            //{

            //}
            //catch (Exception ex)
            //{
            //    // Log the exception or handle it as needed
            //    return ApiResult<List<MembershipResponse>>.Failure(new Exception("An error occurred while fetching memberships."));
            //}
            throw new NotImplementedException();
    
        }

        public Task<ApiResult<List<MembershipResponse?>>> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<List<MembershipResponse?>>> UpdateAsync(Guid id, CreateMembershipRequest dto)
        {
            throw new NotImplementedException();
        }

        Task<ApiResult<List<bool>>> IMembershipService.DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
