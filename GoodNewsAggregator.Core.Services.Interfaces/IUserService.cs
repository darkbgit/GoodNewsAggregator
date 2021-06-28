using System;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.WebApi.Models;
using GoodNewsAggregator.DAL.Core.Entities;


namespace GoodNewsAggregator.Core.Services.Interfaces
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterModel model);
        Task<AuthenticationModel> GetTokenAsync(TokenRequestModel model);
        Task<string> AddRoleAsync(AddRoleModel model);

        Task<AuthenticationModel> RefreshTokenAsync(string token);

        Task<bool> RevokeToken(string token);
        Task<User> GetById(Guid id);
    }
}
