using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Capstone4.Startup))]
namespace Capstone4
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
