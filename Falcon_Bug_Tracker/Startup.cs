using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Falcon_Bug_Tracker.Startup))]
namespace Falcon_Bug_Tracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
