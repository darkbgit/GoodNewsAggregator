using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GoodNewsAggregator.DAL.Core.Entities;
using GoodNewsAggregator.DAL.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GoodNewsAggregator.DAL.CQRS.QueryHandlers
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, User>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public LoginQueryHandler(UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<User> Handle(LoginQuery query, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(query.Email);
            if (user == null)
            {
                //throw new RestException()
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, query.Password, false);

            //if (result.Succeeded)
            //{
            //    return new User()
            //    {
            //        DisplayName = user.DisplayName,
            //        Token = "test",
            //        UserName = user.UserName,
            //        Image = null
            //    };
            //}

            //throw ;
            return default;
        }

    }
}
