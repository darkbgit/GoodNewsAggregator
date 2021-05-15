using GoodNewsAggregator.DAL.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.DAL.Core
{
    public class RoleInitilizer
    {
        public static async Task InitializeAsync(UserManager<User> userManeger,
            RoleManager<Role> roleManager)
        {
            string adminEmail = "admin@admin.com";
            string password = "_Aa12345";

            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new Role { Id = Guid.NewGuid(), Name = "admin" });
            }
            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new Role { Id = Guid.NewGuid(), Name = "user" });
            }

            if (await userManeger.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User { Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManeger.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManeger.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}
