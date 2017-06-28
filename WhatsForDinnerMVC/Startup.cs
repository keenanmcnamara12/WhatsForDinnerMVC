using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WhatsForDinnerMVC.Startup))]
namespace WhatsForDinnerMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
