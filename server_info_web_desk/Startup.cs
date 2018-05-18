using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(server_info_web_desk.Startup))]
namespace server_info_web_desk
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
