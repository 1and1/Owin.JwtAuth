using System.Web.Http;

namespace OwinJwtAuthSample
{
    /// <summary>
    /// Configuration for Web API.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Builds a Web API configuration object.
        /// </summary>
        public static HttpConfiguration Build()
        {
            var config = new HttpConfiguration
            {
                IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always
            };
            config.MapHttpAttributeRoutes();
            config.EnsureInitialized();
            return config;
        }
    }
}
