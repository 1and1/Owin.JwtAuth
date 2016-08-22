using Microsoft.Owin;
using Owin;
using Owin.JwtAuth;
using OwinJwtAuthSample;

[assembly: OwinStartup(typeof(OwinStartup))]

namespace OwinJwtAuthSample
{
    /// <summary>
    /// Entry point for OWIN.
    /// </summary>
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app
                .UseJwtAuth()
                .UseWebApi(WebApiConfig.Build());
        }
    }
}
