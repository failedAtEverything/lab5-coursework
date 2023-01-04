using CPApplication.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace CPApplication.Core.MiddlewareBases
{
    public class IdentityFiller
    {
        public static async Task ExecuteAsync(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            TvChannelContext tvChannel)
        {
            if (await roleManager.FindByNameAsync("Admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (await userManager.FindByNameAsync("its.admin@mail.ru") == null)
            {
                var admin = new IdentityUser 
                { 
                    Email = "its.admin@mail.ru", 
                    UserName = "its.admin@mail.ru" 
                };
                IdentityResult result = await userManager.CreateAsync(admin, "aA_pAssw0rd_");
                if (result.Succeeded)
                {
                    await userManager.CreateAsync(admin, "aA_pAsswOrd_");
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            for (int id = 0; id < 100; id++)
            {
                var name = $"its.customer{id}@mail.ru";

                if (await userManager.FindByNameAsync(name) == null)
                {
                    var employee = new IdentityUser
                    {
                        Email = name,
                        UserName = name
                    };

                    await userManager.CreateAsync(employee, $"_Cc_pAsswOrd{id}");
                }
            }
        }
    }
}
