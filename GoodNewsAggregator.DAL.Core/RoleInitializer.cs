using GoodNewsAggregator.DAL.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.DAL.Core
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            const string adminEmail = "admin@admin.com";
            const string moderatorEmail = "moderator@moderator.com";
            const string authorEmail = "author@author.com";
            const string userEmail = "user@user.com";

            string password = "_Aa12345";

            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new Role( "admin" ));
            }
            if (await roleManager.FindByNameAsync("moderator") == null)
            {
                await roleManager.CreateAsync(new Role("moderator"));
            }
            if (await roleManager.FindByNameAsync("author") == null)
            {
                await roleManager.CreateAsync(new Role("author"));
            }
            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new Role("user"));
            }

            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User user = new() { Email = adminEmail, UserName = adminEmail };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "admin");
                }
            }

            if (await userManager.FindByNameAsync(moderatorEmail) == null)
            {
                User user = new() { Email = moderatorEmail, UserName = moderatorEmail };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "moderator");
                }
            }

            if (await userManager.FindByNameAsync(authorEmail) == null)
            {
                User user = new() { Email = authorEmail, UserName = authorEmail };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "author");
                }
            }

            if (await userManager.FindByNameAsync(userEmail) == null)
            {
                User user = new() { Email = userEmail, UserName = userEmail };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "user");
                }
            }
        }
    }
}
