using Microsoft.Owin;
using Owin;
using Hangfire;
using Hangfire.SqlServer;

[assembly: OwinStartupAttribute(typeof(Korea.Startup))]
namespace Korea
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            app.UseHangfire(config =>
            {
                config.UseSqlServerStorage("DefaultConnection");
                config.UseServer();
            });
        }
    }
}
