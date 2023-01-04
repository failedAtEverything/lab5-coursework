using CPApplication.Core.MiddlewareBases;
using CPApplication.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;

namespace CPApplication.Core.Middlewares
{
    public class DatabasesFillerMiddleware
    {
        protected readonly RequestDelegate _next;

        public DatabasesFillerMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context, 
            TvChannelContext channelContext,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await DataFiller.ExecuteAsync(channelContext);
            await IdentityFiller.ExecuteAsync(userManager, roleManager, channelContext);

            await _next.Invoke(context);
        }
    }
}
