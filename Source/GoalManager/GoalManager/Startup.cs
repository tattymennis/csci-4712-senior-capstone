using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GoalManager.Startup))]
namespace GoalManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
