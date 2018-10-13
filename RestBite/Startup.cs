using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RestBite.Startup))]
namespace RestBite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
