using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SafetyZoneAPI.Startup))]
namespace SafetyZoneAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
