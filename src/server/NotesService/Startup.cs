using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(NotesService.Startup))]

namespace NotesService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}